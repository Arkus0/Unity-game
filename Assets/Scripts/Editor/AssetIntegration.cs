using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using JRPG.Player;
using System.Collections.Generic;
using System.Linq;

namespace JRPG.Editor
{
    public class AssetIntegration
    {
        [MenuItem("Tools/JRPG/Apply Quaternius Assets")]
        public static void ApplyAssets()
        {
            // --- Characters ---
            GameObject playerBody = FindAssetByName("Superhero_Male_FullBody", true);
            GameObject playerHair = FindAssetByName("Hair_Buzzed");
            GameObject playerEyebrows = FindAssetByName("Eyebrows_Regular");

            // --- Environment ---
            // Try looser matching for environment assets
            GameObject floorModel = FindAssetByName("Floor_UnevenBrick");

            // House components
            GameObject wallStraight = FindAssetByName("Wall_Plaster_Straight");
            GameObject wallWindow = FindAssetByName("Wall_Plaster_Window_Wide_Round");
            GameObject doorFrame = FindAssetByName("DoorFrame_Round_WoodDark");
            GameObject door = FindAssetByName("Door_1_Round");
            GameObject roof = FindAssetByName("Roof_RoundTiles_4x4");

            // Logging for missing critical assets
            if (playerBody == null) Debug.LogError("Missing Asset: Superhero_Male_FullBody");
            if (floorModel == null) Debug.LogWarning("Missing Asset: Floor_UnevenBrick");
            if (wallStraight == null) Debug.LogWarning("Missing Asset: Wall_Plaster_Straight");
            if (roof == null) Debug.LogWarning("Missing Asset: Roof_RoundTiles_4x4");

            // Apply Characters
            ApplyToPlayer(playerBody, playerHair, playerEyebrows);

            // Apply Environment
            ApplyToEnvironment(floorModel, wallStraight, wallWindow, doorFrame, door, roof);

            Debug.Log("Asset Integration Logic Completed.");
        }

        private static GameObject FindAssetByName(string partialName, bool forceExact = false)
        {
            // 1. Try exact/partial match as provided
            string[] guids = AssetDatabase.FindAssets(partialName + " t:GameObject");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }

            // 2. Try looser search (replace underscores with spaces, check contents)
            if (!forceExact)
            {
                string looseName = partialName.Replace("_", " ");
                string[] allGuids = AssetDatabase.FindAssets("t:GameObject"); // Get all GameObjects
                foreach (string guid in allGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

                    // Case insensitive contains
                    if (fileName.IndexOf(partialName, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        fileName.IndexOf(looseName, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Debug.Log($"Found loose match for '{partialName}': {path}");
                        return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    }
                }
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
                int houseCount = 0;
                foreach (var obj in houses)
                {
                    if (obj.name == "House_Placeholder")
                    {
                        // Remove placeholder cube
                        Transform body = obj.transform.Find("House_Body");
                        if (body != null) GameObject.DestroyImmediate(body.gameObject);

                        ConstructHouse(obj.transform, wallS, wallW, dFrame, dDoor, roof);
                        houseCount++;
                    }
                }
                if (houseCount > 0) Debug.Log($"Constructed {houseCount} Houses.");
            }
        }

        private static void ConstructHouse(Transform parent, GameObject wallS, GameObject wallW, GameObject dFrame, GameObject dDoor, GameObject roof)
        {
            // Simple 2x2 House Construction
            // 0,0 is center. Walls are usually 2m or 4m wide. Quaternius walls are often modular.

            // Clean previous children if any, to avoid duplication on multiple clicks
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in parent) children.Add(child.gameObject);
            foreach (var child in children) GameObject.DestroyImmediate(child);

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
                GameObject r = InstantiatePart(roof, parent, new Vector3(0, 4, 0), Quaternion.identity);
                r.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
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

                    // 3. Attach Animator Helper
                    Animator anim = visual.GetComponent<Animator>();
                    if (anim)
                    {
                        if (visual.GetComponent<PlayerAnimator>() == null)
                        {
                            visual.AddComponent<PlayerAnimator>();
                        }

                        // 4. Setup Controller to fix T-Pose
                        SetupPlayerAnimatorController(anim);
                    }
                }

                Debug.Log("Updated Player Visuals.");
            }
        }

        private static void SetupPlayerAnimatorController(Animator animator)
        {
            // Check if controller already exists on the animator
            if (animator.runtimeAnimatorController != null) return;

            // Define path for the generated controller
            string controllerPath = "Assets/PlayerController_Generated.controller";
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

                // Add Parameters
                controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
                controller.AddParameter("InputX", AnimatorControllerParameterType.Float);
                controller.AddParameter("InputY", AnimatorControllerParameterType.Float);

                // Find Animations
                AnimationClip idleClip = FindAnimationClip("Idle");
                AnimationClip runClip = FindAnimationClip("Run");
                if (runClip == null) runClip = FindAnimationClip("Walk");

                // Add States
                AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

                // Idle State
                AnimatorState idleState = rootStateMachine.AddState("Idle");
                idleState.motion = idleClip;

                // Movement State (Blend Tree)
                if (runClip != null)
                {
                    AnimatorState moveState = rootStateMachine.AddState("Movement");

                    BlendTree blendTree;
                    controller.CreateBlendTreeInState("Movement Blend Tree", moveState, out blendTree);
                    blendTree.blendType = BlendTreeType.Simple1D; // Simplifying to 1D based on Speed for now, or 2D if we want direction
                    // Let's do 2D Freeform Cartesian for X/Y
                    blendTree.blendType = BlendTreeType.SimpleDirectional2D;
                    blendTree.blendParameter = "InputX";
                    blendTree.blendParameterY = "InputY";

                    // Add motions to blend tree
                    // (0,0) is usually idle, but for movement state we usually want movement.
                    // However, we transition to this state when IsMoving is true.
                    blendTree.AddChild(runClip, new Vector2(0, 1)); // Forward
                    blendTree.AddChild(runClip, new Vector2(0, -1)); // Back
                    blendTree.AddChild(runClip, new Vector2(-1, 0)); // Left
                    blendTree.AddChild(runClip, new Vector2(1, 0)); // Right

                    // Transitions
                    AnimatorStateTransition toMove = idleState.AddTransition(moveState);
                    toMove.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
                    toMove.duration = 0.1f;

                    AnimatorStateTransition toIdle = moveState.AddTransition(idleState);
                    toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
                    toIdle.duration = 0.1f;
                }
                else
                {
                    Debug.LogWarning("Could not find 'Run' or 'Walk' animation clips. Movement state will not work.");
                }

                Debug.Log($"Created AnimatorController at {controllerPath}");
            }

            animator.runtimeAnimatorController = controller;
        }

        private static AnimationClip FindAnimationClip(string name)
        {
            string[] guids = AssetDatabase.FindAssets(name + " t:AnimationClip");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            }
            return null;
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

            // Remove old visual child if exists (prevent accumulation)
            List<GameObject> toDestroy = new List<GameObject>();
            foreach (Transform child in target.transform)
            {
                if (child.name.Contains("(Clone)") || child.GetComponent<Animator>())
                {
                     toDestroy.Add(child.gameObject);
                }
            }
            foreach (var go in toDestroy) GameObject.DestroyImmediate(go);

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
