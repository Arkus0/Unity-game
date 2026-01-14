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
            // 1. Setup Environment
            CreateFloor();
            SetupCamera();

            // 2. Create Encounter Manager
            CreateEncounterManager();

            // 3. Create Player (Capsule)
            GameObject player = CreatePlayer();

            // 4. Create Enemy (Red Cube)
            CreateEnemy(new Vector3(3, 1, 0));

            // 5. Create NPC (Green Cylinder)
            CreateNPC(new Vector3(-3, 1, 0));

            Debug.Log("JRPG 3D Test Scene Created! don't forget to set the Tags and Layers as described in the README.");
            Selection.activeGameObject = player;
        }

        private static void CreateFloor()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(2, 1, 2); // 20x20 meters
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
            cam.transform.position = new Vector3(0, 10, -8);
            cam.transform.rotation = Quaternion.Euler(50, 0, 0);
        }

        private static void CreateEncounterManager()
        {
            GameObject obj = new GameObject("GameManager");
            obj.AddComponent<EncounterManager>();
        }

        private static GameObject CreatePlayer()
        {
            // Create Capsule for Player
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0, 1, 0); // Above floor

            // Color it Blue
            Renderer rend = player.GetComponent<Renderer>();
            if (rend != null) rend.material.color = Color.blue;

            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tipping over

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

            // BoxCollider is added automatically by CreatePrimitive
            // Rigidbody is optional for static enemies, but if they move they need it.
            // For now, let's keep them as static obstacles that trigger battle on touch.

            enemy.AddComponent<EnemyOverworld>();
        }

        private static void CreateNPC(Vector3 position)
        {
            GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            npc.name = "Villager";
            npc.transform.position = position;

            Renderer rend = npc.GetComponent<Renderer>();
            if (rend != null) rend.material.color = Color.green;

            // CylinderCollider/CapsuleCollider is added automatically

            npc.AddComponent<NPC>();
        }
    }
}
