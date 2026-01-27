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
            // Kept for backward compatibility or if user wants to retry,
            // but QuickSetup will call BuildMedievalVillage directly.
            Debug.Log("Please use 'Create 3D Test Scene' to build the new village.");
        }

        // --- New Village Generation for "Medieval Fantasy Town" ---
        public static void BuildMedievalVillage(Transform parent)
        {
            // 1. Find Ground
            BuildGround(parent);

            // 2. Find Houses
            List<GameObject> housePrefabs = FindPrefabsByKeywords("House", "Cottage", "Building", "Structure", "Home");
            if (housePrefabs.Count > 0)
            {
                // Simple Street Layout
                // Left side
                for (int z = -10; z <= 10; z += 10)
                {
                    GameObject prefab = housePrefabs[Random.Range(0, housePrefabs.Count)];
                    SpawnPrefab(prefab, parent, new Vector3(-8, 0, z), Quaternion.Euler(0, 90, 0));
                }
                // Right side
                for (int z = -10; z <= 10; z += 10)
                {
                    GameObject prefab = housePrefabs[Random.Range(0, housePrefabs.Count)];
                    SpawnPrefab(prefab, parent, new Vector3(8, 0, z), Quaternion.Euler(0, -90, 0));
                }
                Debug.Log($"Placed {housePrefabs.Count} unique house types in the village.");
            }
            else
            {
                Debug.LogWarning("No House/Cottage prefabs found. Village will be empty.");
            }

            // 3. Find Props (Market, Crate, Wagon)
            List<GameObject> propPrefabs = FindPrefabsByKeywords("Market", "Stall", "Wagon", "Cart", "Crate", "Barrel", "Fence");
            if (propPrefabs.Count > 0)
            {
                // Scatter some props
                for (int i = 0; i < 5; i++)
                {
                     GameObject p = propPrefabs[Random.Range(0, propPrefabs.Count)];
                     Vector3 pos = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-10f, 10f));
                     SpawnPrefab(p, parent, pos, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
                }
            }

            // 4. Update Player Visuals (T-Pose Fix)
            ApplyToPlayer();
        }

        private static void BuildGround(Transform parent)
        {
            // Try to find a Terrain or Ground prefab first
            GameObject groundPrefab = FindAssetByName("Terrain");
            if (groundPrefab == null) groundPrefab = FindAssetByName("Ground");
            if (groundPrefab == null) groundPrefab = FindAssetByName("Village_Ground"); // Guessing

            if (groundPrefab != null)
            {
                SpawnPrefab(groundPrefab, parent, Vector3.zero, Quaternion.identity);
            }
            else
            {
                // Fallback: Plane with Grass Texture
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.name = "Ground_Plane";
                plane.transform.parent = parent;
                plane.transform.localScale = new Vector3(10, 1, 10); // 100x100m

                Renderer rend = plane.GetComponent<Renderer>();
                if (rend != null)
                {
                    Material grassMat = FindMaterial("Grass");
                    if (grassMat == null) grassMat = FindMaterial("Ground");
                    if (grassMat == null) grassMat = FindMaterial("Terrain");

                    if (grassMat != null)
                    {
                        rend.material = grassMat;
                    }
                    else
                    {
                        rend.material.color = new Color(0.3f, 0.6f, 0.2f); // Green
                    }
                }
            }
        }

        // --- Helper Methods ---

        private static GameObject SpawnPrefab(GameObject prefab, Transform parent, Vector3 pos, Quaternion rot)
        {
            if (prefab == null) return null;
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(parent, false);
            instance.transform.position = pos;
            instance.transform.rotation = rot;
            return instance;
        }

        private static List<GameObject> FindPrefabsByKeywords(params string[] keywords)
        {
            List<GameObject> results = new List<GameObject>();
            string[] allGuids = AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in allGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string name = System.IO.Path.GetFileNameWithoutExtension(path);

                foreach (string keyword in keywords)
                {
                    if (name.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        if (go != null && !results.Contains(go))
                        {
                            results.Add(go);
                        }
                        break; // Found a matching keyword for this asset
                    }
                }
            }
            return results;
        }

        private static GameObject FindAssetByName(string partialName)
        {
            string[] guids = AssetDatabase.FindAssets(partialName + " t:Prefab"); // Look for prefabs primarily
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            return null;
        }

        private static Material FindMaterial(string partialName)
        {
             string[] guids = AssetDatabase.FindAssets(partialName + " t:Material");
             if (guids.Length > 0)
             {
                 string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                 return AssetDatabase.LoadAssetAtPath<Material>(path);
             }
             return null;
        }

        // --- Player T-Pose & Hair Fixes (Preserved) ---

        public static void ApplyToPlayer()
        {
            // Hardcoded names from user's provided assets
            GameObject bodyPrefab = FindAssetByName("Superhero_Male_FullBody"); // Likely FBX, not prefab, but LoadAsset works
            if (bodyPrefab == null) bodyPrefab = FindModel("Superhero_Male_FullBody"); // Fallback to model search

            GameObject hairPrefab = FindAssetByName("Hair_Buzzed");
            if (hairPrefab == null) hairPrefab = FindModel("Hair_Buzzed");

            GameObject eyebrowsPrefab = FindAssetByName("Eyebrows_Regular");
            if (eyebrowsPrefab == null) eyebrowsPrefab = FindModel("Eyebrows_Regular");

            if (bodyPrefab == null)
            {
                Debug.LogWarning("Superhero model not found. Skipping player visual update.");
                return;
            }

            PlayerController player = GameObject.FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // 1. Replace Body
                GameObject visual = ReplaceVisuals(player.gameObject, bodyPrefab, 1.0f);

                // 2. Attach Hair/Eyebrows
                if (visual != null)
                {
                    Transform headBone = FindDeepChild(visual.transform, "Head");
                    if (headBone == null) headBone = FindDeepChild(visual.transform, "mixamorig:Head");
                    if (headBone == null) headBone = FindDeepChild(visual.transform, "Bip01 Head");

                    if (headBone != null)
                    {
                        if (hairPrefab != null)
                        {
                            GameObject hair = (GameObject)PrefabUtility.InstantiatePrefab(hairPrefab);
                            hair.transform.SetParent(headBone, false);
                            hair.transform.localPosition = Vector3.zero;
                            hair.transform.localRotation = Quaternion.identity;
                            hair.transform.localScale = Vector3.one;
                        }
                        if (eyebrowsPrefab != null)
                        {
                            GameObject brows = (GameObject)PrefabUtility.InstantiatePrefab(eyebrowsPrefab);
                            brows.transform.SetParent(headBone, false);
                            brows.transform.localPosition = Vector3.zero;
                            brows.transform.localRotation = Quaternion.identity;
                        }
                    }
                }

                // 3. Setup Animator
                Animator anim = visual != null ? visual.GetComponent<Animator>() : null;
                if (anim)
                {
                    if (visual.GetComponent<PlayerAnimator>() == null) visual.AddComponent<PlayerAnimator>();
                    SetupPlayerAnimatorController(anim);
                }
            }
        }

        private static GameObject FindModel(string name)
        {
            string[] guids = AssetDatabase.FindAssets(name + " t:Model");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            return null;
        }

        private static GameObject ReplaceVisuals(GameObject target, GameObject newModelPrefab, float scale)
        {
            // Cleanup old
            List<GameObject> toDestroy = new List<GameObject>();
            foreach (Transform child in target.transform)
            {
                if (child.name.Contains("(Clone)") || child.GetComponent<Animator>())
                {
                     toDestroy.Add(child.gameObject);
                }
            }
            foreach (var go in toDestroy) GameObject.DestroyImmediate(go);

            foreach (var rend in target.GetComponentsInChildren<MeshRenderer>())
                if (rend.gameObject == target) GameObject.DestroyImmediate(rend);
            foreach (var filter in target.GetComponentsInChildren<MeshFilter>())
                if (filter.gameObject == target) GameObject.DestroyImmediate(filter);

            // Instantiate
            GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(newModelPrefab);
            visual.transform.SetParent(target.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one * scale;

            return visual;
        }

        private static Transform FindDeepChild(Transform aParent, string aName)
        {
            foreach(Transform child in aParent)
            {
                if(child.name == aName ) return child;
                Transform result = FindDeepChild(child, aName);
                if (result != null) return result;
            }
            return null;
        }

        private static void SetupPlayerAnimatorController(Animator animator)
        {
            string controllerPath = "Assets/PlayerController_Generated.controller";
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
                controller.AddParameter("InputX", AnimatorControllerParameterType.Float);
                controller.AddParameter("InputY", AnimatorControllerParameterType.Float);

                AnimationClip idleClip = FindAnimationClip("Idle");
                AnimationClip runClip = FindAnimationClip("Run");
                if (runClip == null) runClip = FindAnimationClip("Walk");

                AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
                AnimatorState idleState = rootStateMachine.AddState("Idle");
                idleState.motion = idleClip;

                if (runClip != null)
                {
                    AnimatorState moveState = rootStateMachine.AddState("Movement");

                    BlendTree blendTree = new BlendTree();
                    blendTree.name = "Movement Blend Tree";
                    moveState.motion = blendTree;
                    AssetDatabase.AddObjectToAsset(blendTree, controller);

                    blendTree.blendType = BlendTreeType.SimpleDirectional2D;
                    blendTree.blendParameter = "InputX";
                    blendTree.blendParameterY = "InputY";

                    blendTree.AddChild(runClip, new Vector2(0, 1));
                    blendTree.AddChild(runClip, new Vector2(0, -1));
                    blendTree.AddChild(runClip, new Vector2(-1, 0));
                    blendTree.AddChild(runClip, new Vector2(1, 0));

                    var toMove = idleState.AddTransition(moveState);
                    toMove.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
                    toMove.duration = 0.1f;

                    var toIdle = moveState.AddTransition(idleState);
                    toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
                    toIdle.duration = 0.1f;
                }
            }

            animator.runtimeAnimatorController = controller;
            animator.Rebind();
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
    }
}
