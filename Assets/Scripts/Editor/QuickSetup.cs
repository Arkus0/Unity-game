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
            // 1. Create Managers
            CreateEncounterManager();

            // 2. Create Player
            GameObject player = CreatePlayer();

            // 3. Setup Camera
            SetupCamera(player.transform);

            // 4. Build Village Environment using new assets
            GameObject env = new GameObject("Environment");
            AssetIntegration.BuildMedievalVillage(env.transform);

            // 5. Create NPCs (placed relatively safe, though depends on random village gen)
            CreateNPC(new Vector3(-5, 0.5f, -2), "Welcome to our town!");
            CreateNPC(new Vector3(5, 0.5f, -5), "Have you seen the market?");

            // 6. Create Enemy
            CreateEnemy(new Vector3(15, 1, 15));

            Debug.Log("JRPG Medieval Town Scene Created!");
            Selection.activeGameObject = player;
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
            if (GameObject.Find("GameManager") == null)
            {
                GameObject obj = new GameObject("GameManager");
                obj.AddComponent<EncounterManager>();
                obj.AddComponent<GameManager>();
            }
        }

        private static GameObject CreatePlayer()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                player.name = "Player";
                player.tag = "Player";
                player.transform.position = new Vector3(0, 1, 0);

                Rigidbody rb = player.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotation;

                player.AddComponent<PlayerController>();
            }
            return player;
        }

        private static void CreateEnemy(Vector3 position)
        {
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy.name = "Enemy_Slime";
            enemy.transform.position = position;

            Renderer rend = enemy.GetComponent<Renderer>();
            if(rend) rend.material.color = Color.red;

            enemy.AddComponent<EnemyOverworld>();
        }

        private static void CreateNPC(Vector3 position, string dialogue)
        {
            GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            npc.name = "Villager";
            npc.transform.position = position;

            Renderer rend = npc.GetComponent<Renderer>();
            if(rend) rend.material.color = Color.green;

            NPC npcScript = npc.AddComponent<NPC>();
            npcScript.SetDialogue(dialogue);
        }
    }
}
