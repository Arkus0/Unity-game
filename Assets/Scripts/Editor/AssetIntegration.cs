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
            GameObject playerBody = FindAssetByName("Superhero_Male_FullBody");
            GameObject playerHair = FindAssetByName("Hair_Buzzed");
            GameObject playerEyebrows = FindAssetByName("Eyebrows_Regular");

            // --- Environment ---
            GameObject floorModel = FindAssetByName("Floor_UnevenBrick");

            // House components
            GameObject wallStraight = FindAssetByName("Wall_Plaster_Straight");
            GameObject wallWindow = FindAssetByName("Wall_Plaster_Window_Wide_Round");
            GameObject doorFrame = FindAssetByName("DoorFrame_Round_WoodDark");
            GameObject door = FindAssetByName("Door_1_Round");
            GameObject roof = FindAssetByName("Roof_RoundTiles_4x4");


            // Check minimal requirements
            if (playerBody == null && floorModel == null)
            {
                EditorUtility.DisplayDialog("Assets Not Found",
                    "Could not find specific assets (e.g., Superhero_Male_FullBody, Floor_UnevenBrick).\n\n" +
                    "Make sure you have imported the FBX files into your Assets folder.", "OK");
                return;
            }

            // Apply Characters
            ApplyToPlayer(playerBody, playerHair, playerEyebrows);

            // Apply Environment
            ApplyToEnvironment(floorModel, wallStraight, wallWindow, doorFrame, door, roof);

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

        private static void ApplyToEnvironment(GameObject floorPrefab, GameObject wallS, GameObject wallW, GameObject dFrame, GameObject dDoor, GameObject roof)
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

            // Construct Houses
            if (wallS != null && roof != null)
            {
                GameObject[] houses = GameObject.FindGameObjectsWithTag("Untagged");
                foreach (var obj in houses)
                {
                    if (obj.name == "House_Placeholder")
                    {
                        // Remove placeholder cube
                        Transform body = obj.transform.Find("House_Body");
                        if (body != null) GameObject.DestroyImmediate(body.gameObject);

                        ConstructHouse(obj.transform, wallS, wallW, dFrame, dDoor, roof);
                    }
                }
                Debug.Log("Constructed Houses.");
            }
        }

        private static void ConstructHouse(Transform parent, GameObject wallS, GameObject wallW, GameObject dFrame, GameObject dDoor, GameObject roof)
        {
            // Simple 2x2 House Construction
            // 0,0 is center. Walls are usually 2m or 4m wide. Quaternius walls are often modular.

            // Create Walls
            InstantiatePart(wallS, parent, new Vector3(-2, 0, 2), Quaternion.Euler(0, 0, 0)); // Back Left
            InstantiatePart(wallW, parent, new Vector3(2, 0, 2), Quaternion.Euler(0, 0, 0)); // Back Right (Window)

            InstantiatePart(wallS, parent, new Vector3(-2, 0, -2), Quaternion.Euler(0, 180, 0)); // Front Left

            // Doorway
            if (dFrame != null)
            {
                GameObject frame = InstantiatePart(dFrame, parent, new Vector3(2, 0, -2), Quaternion.Euler(0, 180, 0));
                if (dDoor != null)
                {
                    // Door is child of frame usually, or placed inside
                    InstantiatePart(dDoor, frame.transform, Vector3.zero, Quaternion.identity);
                }
            }
            else
            {
                InstantiatePart(wallS, parent, new Vector3(2, 0, -2), Quaternion.Euler(0, 180, 0));
            }

            // Side Walls
            InstantiatePart(wallS, parent, new Vector3(-2, 0, -2), Quaternion.Euler(0, 270, 0)); // Left Front
            InstantiatePart(wallW, parent, new Vector3(-2, 0, 2), Quaternion.Euler(0, 270, 0)); // Left Back (Window)

            InstantiatePart(wallS, parent, new Vector3(2, 0, 2), Quaternion.Euler(0, 90, 0)); // Right Back
            InstantiatePart(wallS, parent, new Vector3(2, 0, -2), Quaternion.Euler(0, 90, 0)); // Right Front

            // Roof
            if (roof != null)
            {
                // Centered roof
                GameObject r = InstantiatePart(roof, parent, new Vector3(0, 4, 0), Quaternion.identity); // Height 4m approx
                r.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); // Make it slightly bigger to cover edges
            }
        }

        private static GameObject InstantiatePart(GameObject prefab, Transform parent, Vector3 pos, Quaternion rot)
        {
            if (prefab == null) return null;
            GameObject part = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            part.transform.SetParent(parent, false);
            part.transform.localPosition = pos;
            part.transform.localRotation = rot;
            return part;
        }

        private static void ApplyToPlayer(GameObject bodyPrefab, GameObject hairPrefab, GameObject eyebrowsPrefab)
        {
            if (bodyPrefab == null) return;

            PlayerController player = GameObject.FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // 1. Replace Body
                GameObject visual = ReplaceVisuals(player.gameObject, bodyPrefab, 1.0f);

                // 2. Attach Hair/Eyebrows
                if (visual != null)
                {
                    Transform headBone = FindDeepChild(visual.transform, "Head");
                    if (headBone != null)
                    {
                        if (hairPrefab != null)
                        {
                            GameObject hair = (GameObject)PrefabUtility.InstantiatePrefab(hairPrefab);
                            hair.transform.SetParent(headBone, false);
                            hair.transform.localPosition = Vector3.zero;
                            hair.transform.localRotation = Quaternion.identity;
                        }
                        if (eyebrowsPrefab != null)
                        {
                            GameObject brows = (GameObject)PrefabUtility.InstantiatePrefab(eyebrowsPrefab);
                            brows.transform.SetParent(headBone, false);
                            brows.transform.localPosition = Vector3.zero;
                            brows.transform.localRotation = Quaternion.identity;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Could not find 'Head' bone in player model. Hair not attached.");
                    }

                    // 3. Attach Animator Helper
                    if (visual.GetComponent<Animator>())
                    {
                        if (visual.GetComponent<PlayerAnimator>() == null)
                        {
                            visual.AddComponent<PlayerAnimator>();
                        }
                    }
                }

                Debug.Log("Updated Player Visuals with Hair/Eyebrows.");
            }
        }

        private static Transform FindDeepChild(Transform aParent, string aName)
        {
            foreach(Transform child in aParent)
            {
                if(child.name == aName )
                    return child;
                Transform result = FindDeepChild(child, aName);
                if (result != null)
                    return result;
            }
            return null;
        }

        private static GameObject ReplaceVisuals(GameObject target, GameObject newModelPrefab, float scale)
        {
            // Remove old primitives
            foreach (var rend in target.GetComponentsInChildren<MeshRenderer>())
            {
                if (rend.gameObject == target) GameObject.DestroyImmediate(rend);
            }
            foreach (var filter in target.GetComponentsInChildren<MeshFilter>())
            {
                 if (filter.gameObject == target) GameObject.DestroyImmediate(filter);
            }

            // Instantiate new model
            GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(newModelPrefab);
            visual.transform.SetParent(target.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one * scale;

            return visual;
        }
    }
}
