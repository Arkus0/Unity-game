using UnityEngine;
using UnityEditor;
using JRPG.Player;
using JRPG.Interaction;
using JRPG.Systems;

namespace JRPG.Editor
{
    public class QuickSetup
    {
        [MenuItem("Tools/JRPG/Create 3D Test Scene")]
        public static void CreateTestScene()
        {
            // 1. Setup Environment (Village Grid)
            GameObject env = CreateVillageEnvironment();

            // 2. Create Encounter Manager
            CreateEncounterManager();

            // 3. Create Player (Capsule -> AssetIntegration will fix visuals)
            GameObject player = CreatePlayer();

            // Setup Camera to follow Player
            SetupCamera(player.transform);

            // 4. Create Enemy
            CreateEnemy(new Vector3(12, 1, 12));

            // 5. Create NPCs
            CreateNPC(new Vector3(-5, 0.5f, -2), "Welcome to our village!");
            CreateNPC(new Vector3(5, 0.5f, -5), "Beware of the red cube... it bites.");

            // 6. Apply Assets Automatically
            // This is crucial: We call the AssetIntegration logic we built to flesh out the placeholders
            AssetIntegration.ApplyAssets();

            Debug.Log("JRPG 3D Village Scene Created!");
            Selection.activeGameObject = player;
        }

        private static GameObject CreateVillageEnvironment()
        {
            GameObject environment = new GameObject("Environment");

            // Create Floor Grid (Ground)
            GameObject floorParent = new GameObject("Floor_Grid");
            floorParent.transform.parent = environment.transform;

            // Bigger Grid: 20x20
            for (int x = -20; x <= 20; x += 2)
            {
                for (int z = -20; z <= 20; z += 2)
                {
                    // Create a floor tile. AssetIntegration will replace this with "Floor_UnevenBrick"
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    tile.name = $"FloorTile_{x}_{z}";
                    tile.transform.parent = floorParent.transform;
                    tile.transform.position = new Vector3(x, 0, z);
                    tile.transform.localScale = new Vector3(0.2f, 1, 0.2f); // 2x2m
                }
            }

            // Create Houses at specific spots
            // AssetIntegration looks for "House_Placeholder" and builds walls/roofs
            CreateHousePlaceholder(new Vector3(-8, 0, 8), environment.transform);
            CreateHousePlaceholder(new Vector3(8, 0, -8), environment.transform);
            CreateHousePlaceholder(new Vector3(-8, 0, -8), environment.transform);

            // Create Props
            CreateProp("Prop_Wagon", new Vector3(2, 0, 4), environment.transform);
            CreateProp("Prop_Crate", new Vector3(3, 0, 4.5f), environment.transform);
            CreateProp("Prop_Crate", new Vector3(2.5f, 0.8f, 4.2f), environment.transform);

            // Fences
            for(int i = -4; i < 4; i+=2)
            {
                CreateProp("Prop_WoodenFence_Single", new Vector3(10, 0, i), environment.transform, Quaternion.Euler(0, 90, 0));
            }

            return environment;
        }

        private static void CreateHousePlaceholder(Vector3 position, Transform parent)
        {
            GameObject house = new GameObject("House_Placeholder");
            house.transform.position = position;
            house.transform.parent = parent;

            // This placeholder name signals AssetIntegration.cs to run ConstructHouse()
            // We just leave it empty or with a temporary visual
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "House_Body"; // Will be destroyed by AssetIntegration
            body.transform.parent = house.transform;
            body.transform.localPosition = new Vector3(0, 2, 0);
            body.transform.localScale = new Vector3(4, 4, 4);
        }

        private static void CreateProp(string propName, Vector3 position, Transform parent, Quaternion? rotation = null)
        {
            GameObject prop = new GameObject(propName);
            prop.transform.position = position;
            prop.transform.rotation = rotation ?? Quaternion.identity;
            prop.transform.parent = parent;

            // AssetIntegration doesn't currently auto-replace generic props by name in the main loop,
            // but we can add a simple script or just assume the user will drag it?
            // BETTER: Let's try to find the prefab immediately here if possible.
            string[] guids = AssetDatabase.FindAssets(propName + " t:GameObject");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    instance.transform.SetParent(prop.transform, false);

                    // Fix materials if needed (using the helper we made in AssetIntegration if accessible, or just rely on it being correct)
                    // Since EnsureMaterial is private in AssetIntegration, we might get green props if textures are missing.
                    // But typically props share materials with buildings.
                }
            }
            else
            {
                // Fallback placeholder
                GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube);
                p.transform.parent = prop.transform;
                p.transform.localScale = Vector3.one * 0.5f;
                p.name = "Placeholder";
            }
        }

        private static void SetupCamera(Transform target)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }

            // Add CameraFollow script
            CameraFollow follow = cam.gameObject.GetComponent<CameraFollow>();
            if (follow == null) follow = cam.gameObject.AddComponent<CameraFollow>();

            follow.target = target;

            if (target != null)
            {
                cam.transform.position = target.position + follow.offset;
                cam.transform.LookAt(target);
            }
        }

        private static void CreateEncounterManager()
        {
            GameObject obj = new GameObject("GameManager");
            obj.AddComponent<EncounterManager>();
            obj.AddComponent<GameManager>();
        }

        private static GameObject CreatePlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0, 1, 0);

            Renderer rend = player.GetComponent<Renderer>();
            if (rend != null) rend.material.color = Color.blue;

            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            player.AddComponent<PlayerController>();

            // Ensure tag
            try { player.tag = "Player"; } catch {}

            return player;
        }

        private static void CreateEnemy(Vector3 position)
        {
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy.name = "Enemy_Slime";
            enemy.transform.position = position;

            Renderer rend = enemy.GetComponent<Renderer>();
            if (rend != null) rend.material.color = Color.red;

            enemy.AddComponent<EnemyOverworld>();
        }

        private static void CreateNPC(Vector3 position, string dialogue)
        {
            GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            npc.name = "Villager";
            npc.transform.position = position;

            Renderer rend = npc.GetComponent<Renderer>();
            if (rend != null) rend.material.color = Color.green;

            NPC npcScript = npc.AddComponent<NPC>();
            npcScript.SetDialogue(dialogue);
        }
    }
}
