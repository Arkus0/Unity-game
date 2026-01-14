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
            CreateVillageEnvironment();
            SetupCamera();

            // 2. Create Encounter Manager
            CreateEncounterManager();

            // 3. Create Player (Capsule)
            GameObject player = CreatePlayer();

            // 4. Create Enemy (Red Cube) near the forest edge
            CreateEnemy(new Vector3(8, 1, 8));

            // 5. Create NPCs (Green Cylinders) near houses
            CreateNPC(new Vector3(-5, 1, -2), "Welcome to our village!");
            CreateNPC(new Vector3(5, 1, -5), "Beware of the red cube... it bites.");

            Debug.Log("JRPG 3D Village Scene Created! Don't forget to set Tags and Layers.");
            Selection.activeGameObject = player;
        }

        private static void CreateVillageEnvironment()
        {
            GameObject environment = new GameObject("Environment");

            // Create Floor Grid (10x10)
            GameObject floorParent = new GameObject("Floor_Grid");
            floorParent.transform.parent = environment.transform;

            for (int x = -10; x <= 10; x += 2)
            {
                for (int z = -10; z <= 10; z += 2)
                {
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    tile.name = $"FloorTile_{x}_{z}";
                    tile.transform.parent = floorParent.transform;
                    tile.transform.position = new Vector3(x, 0, z);
                    tile.transform.localScale = new Vector3(0.2f, 1, 0.2f); // 2x2 meters approx
                }
            }

            // Create Placeholder Houses
            CreateHousePlaceholder(new Vector3(-6, 0, 4), environment.transform);
            CreateHousePlaceholder(new Vector3(6, 0, -4), environment.transform);
        }

        private static void CreateHousePlaceholder(Vector3 position, Transform parent)
        {
            GameObject house = new GameObject("House_Placeholder");
            house.transform.position = position;
            house.transform.parent = parent;

            // Simple Cube representing a house
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "House_Body";
            body.transform.parent = house.transform;
            body.transform.localPosition = new Vector3(0, 2, 0);
            body.transform.localScale = new Vector3(4, 4, 4);

            Renderer rend = body.GetComponent<Renderer>();
            if (rend != null) rend.material.color = new Color(0.6f, 0.4f, 0.2f); // Brown
        }

        private static void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }

            // Top-down view
            cam.transform.position = new Vector3(0, 15, -12);
            cam.transform.rotation = Quaternion.Euler(55, 0, 0);
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

            try
            {
                player.tag = "Player";
            }
            catch
            {
                Debug.LogWarning("Tag 'Player' not defined. Please add it in Project Settings.");
            }

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
