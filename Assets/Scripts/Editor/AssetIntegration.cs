using UnityEngine;
using UnityEditor;
using JRPG.Player;
using JRPG.Interaction;

namespace JRPG.Editor
{
    public class AssetIntegration
    {
        [MenuItem("Tools/JRPG/Apply Quaternius Assets")]
        public static void ApplyAssets()
        {
            // --- Characters ---
            GameObject playerModel = FindAssetByName("Warrior_Male");
            if (playerModel == null) playerModel = FindAssetByName("Human_Male");

            GameObject enemyModel = FindAssetByName("Skeleton");
            if (enemyModel == null) enemyModel = FindAssetByName("Monster");

            GameObject npcModel = FindAssetByName("Civilian_Male");
            if (npcModel == null) npcModel = FindAssetByName("Human_Female");

            // --- Environment ---
            // Generic names that might be in the Medieval Village pack
            GameObject floorModel = FindAssetByName("Floor_Stone");
            if (floorModel == null) floorModel = FindAssetByName("Ground_Grass");

            GameObject houseModel = FindAssetByName("House_Type1");
            if (houseModel == null) houseModel = FindAssetByName("House_Small");


            // Check if we found at least something
            if (playerModel == null && floorModel == null)
            {
                EditorUtility.DisplayDialog("Assets Not Found",
                    "Could not auto-detect Quaternius models.\n\n" +
                    "Make sure you have imported 'Modular Character Outfits' and/or 'Medieval Village MegaKit'.", "OK");
                return;
            }

            // Apply Characters
            ApplyToPlayer(playerModel);
            ApplyToEnemy(enemyModel);
            ApplyToNPCs(npcModel);

            // Apply Environment
            ApplyToEnvironment(floorModel, houseModel);

            Debug.Log("Asset Integration Complete!");
        }

        private static GameObject FindAssetByName(string partialName)
        {
            string[] guids = AssetDatabase.FindAssets(partialName + " t:GameObject");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            return null;
        }

        private static void ApplyToEnvironment(GameObject floorPrefab, GameObject housePrefab)
        {
            // Apply Floor
            if (floorPrefab != null)
            {
                GameObject floorGrid = GameObject.Find("Floor_Grid");
                if (floorGrid != null)
                {
                    foreach (Transform child in floorGrid.transform)
                    {
                        ReplaceVisuals(child.gameObject, floorPrefab, 1.0f);
                    }
                    Debug.Log("Updated Floor Visuals.");
                }
            }

            // Apply Houses
            if (housePrefab != null)
            {
                GameObject[] houses = GameObject.FindGameObjectsWithTag("Untagged"); // We find by name manually
                foreach (var obj in houses)
                {
                    if (obj.name == "House_Placeholder")
                    {
                        // The placeholder has a child "House_Body" which is the cube.
                        // We want to replace the whole "House_Body" with the prefab.
                        Transform body = obj.transform.Find("House_Body");
                        if (body != null)
                        {
                            ReplaceVisuals(obj, housePrefab, 1.0f);
                            GameObject.DestroyImmediate(body.gameObject);
                        }
                    }
                }
                Debug.Log("Updated House Visuals.");
            }
        }

        private static void ApplyToPlayer(GameObject modelPrefab)
        {
            if (modelPrefab == null) return;

            PlayerController player = GameObject.FindObjectOfType<PlayerController>();
            if (player != null)
            {
                GameObject visual = ReplaceVisuals(player.gameObject, modelPrefab, 1.0f);

                // Attach Animator Helper
                if (visual.GetComponent<Animator>())
                {
                    if (visual.GetComponent<PlayerAnimator>() == null)
                    {
                        visual.AddComponent<PlayerAnimator>();
                    }
                }

                Debug.Log("Updated Player Visuals.");
            }
        }

        private static void ApplyToEnemy(GameObject modelPrefab)
        {
            if (modelPrefab == null) return;

            EnemyOverworld[] enemies = GameObject.FindObjectsOfType<EnemyOverworld>();
            foreach (var enemy in enemies)
            {
                ReplaceVisuals(enemy.gameObject, modelPrefab, 1.0f);
            }
            if (enemies.Length > 0) Debug.Log("Updated Enemy Visuals.");
        }

        private static void ApplyToNPCs(GameObject modelPrefab)
        {
            if (modelPrefab == null) return;

            NPC[] npcs = GameObject.FindObjectsOfType<NPC>();
            foreach (var npc in npcs)
            {
                ReplaceVisuals(npc.gameObject, modelPrefab, 1.0f);
            }
            if (npcs.Length > 0) Debug.Log("Updated NPC Visuals.");
        }

        private static GameObject ReplaceVisuals(GameObject target, GameObject newModelPrefab, float scale)
        {
            // 1. Remove old mesh renderer/filter components (primitives)
            foreach (var rend in target.GetComponentsInChildren<MeshRenderer>())
            {
                // Don't destroy if it's not the target itself (unless it's a primitive child we want to clear)
                // But for Character capsules, the renderer is on the object.
                // For Floor tiles, it is on the object.
                if (rend.gameObject == target)
                {
                    GameObject.DestroyImmediate(rend);
                }
            }
            foreach (var filter in target.GetComponentsInChildren<MeshFilter>())
            {
                 if (filter.gameObject == target)
                 {
                    GameObject.DestroyImmediate(filter);
                 }
            }

            // 2. Instantiate new model as child
            GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(newModelPrefab);
            visual.transform.SetParent(target.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one * scale;

            return visual;
        }
    }
}
