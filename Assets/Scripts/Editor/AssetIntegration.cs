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
            // Try to find typical model names from the pack
            // Note: The user might rename folders, so we search by asset name.
            // Common names in Quaternius packs: "Warrior", "Mage", "Rogue", "Skeleton", "Slime" etc.
            // Since the specific pack is "Modular Character Outfits", files might be named differently.
            // We will search for generic types and ask the user to confirm if not found,
            // but for automation we try to find *any* suitable model.

            GameObject playerModel = FindAssetByName("Warrior_Male"); // Example name
            if (playerModel == null) playerModel = FindAssetByName("Human_Male"); // Fallback

            GameObject enemyModel = FindAssetByName("Skeleton");
            if (enemyModel == null) enemyModel = FindAssetByName("Monster");

            GameObject npcModel = FindAssetByName("Civilian_Male");
            if (npcModel == null) npcModel = FindAssetByName("Human_Female");

            if (playerModel == null && enemyModel == null && npcModel == null)
            {
                EditorUtility.DisplayDialog("Assets Not Found",
                    "Could not auto-detect specific Quaternius models (Warrior, Skeleton, Civilian). \n\n" +
                    "Make sure you have imported the FBX files into your Assets folder.", "OK");
                return;
            }

            ApplyToPlayer(playerModel);
            ApplyToEnemy(enemyModel);
            ApplyToNPCs(npcModel);

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

        private static void ApplyToPlayer(GameObject modelPrefab)
        {
            if (modelPrefab == null) return;

            PlayerController player = GameObject.FindObjectOfType<PlayerController>();
            if (player != null)
            {
                ReplaceVisuals(player.gameObject, modelPrefab, 1.0f); // Default scale
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

        private static void ReplaceVisuals(GameObject target, GameObject newModelPrefab, float scale)
        {
            // 1. Remove old mesh renderer/filter components (primitives)
            foreach (var rend in target.GetComponentsInChildren<MeshRenderer>())
            {
                GameObject.DestroyImmediate(rend);
            }
            foreach (var filter in target.GetComponentsInChildren<MeshFilter>())
            {
                GameObject.DestroyImmediate(filter);
            }

            // Also destroy any child objects that might be visual placeholders
            // (careful not to destroy scripts or logic, but primitives usually don't have children unless complex)
            // Ideally, we instantiate the model as a child.

            // 2. Instantiate new model as child
            GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(newModelPrefab);
            visual.transform.SetParent(target.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one * scale;

            // 3. Adjust Collider if necessary
            // (For now we keep the existing primitive collider on the parent)

            // 4. Handle Animator
            Animator anim = visual.GetComponent<Animator>();
            if (anim != null)
            {
                // If we had a controller, we would assign it here.
                // anim.runtimeAnimatorController = ...
            }
        }
    }
}
