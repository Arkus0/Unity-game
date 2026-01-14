using UnityEngine;
using UnityEditor;
using JRPG.Player;
using JRPG.Interaction;
using JRPG.Systems;

namespace JRPG.Editor
{
    public class QuickSetup
    {
        [MenuItem("Tools/JRPG/Create Test Scene")]
        public static void CreateTestScene()
        {
            // 1. Create Encounter Manager
            CreateEncounterManager();

            // 2. Create Player
            GameObject player = CreatePlayer();

            // 3. Create Enemy
            CreateEnemy(new Vector3(3, 0, 0));

            // 4. Create NPC
            CreateNPC(new Vector3(-3, 0, 0));

            Debug.Log("JRPG Test Scene Created! don't forget to set the Tags and Layers as described in the README.");
            Selection.activeGameObject = player;
        }

        private static void CreateEncounterManager()
        {
            GameObject obj = new GameObject("GameManager");
            obj.AddComponent<EncounterManager>();
        }

        private static GameObject CreatePlayer()
        {
            GameObject player = new GameObject("Player");

            // Add Components
            SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            sr.color = Color.blue;

            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            BoxCollider2D col = player.AddComponent<BoxCollider2D>();

            player.AddComponent<PlayerController>();

            // Attempt to set Tag (requires tag to exist, usually "Player" exists by default)
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
            GameObject enemy = new GameObject("Enemy_Slime");
            enemy.transform.position = position;

            SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            sr.color = Color.red;

            BoxCollider2D col = enemy.AddComponent<BoxCollider2D>();
            // Not a trigger, so we can collide. EnemyOverworld uses OnCollisionEnter2D

            enemy.AddComponent<EnemyOverworld>();
        }

        private static void CreateNPC(Vector3 position)
        {
            GameObject npc = new GameObject("Villager");
            npc.transform.position = position;

            SpriteRenderer sr = npc.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            sr.color = Color.green;

            BoxCollider2D col = npc.AddComponent<BoxCollider2D>();

            npc.AddComponent<NPC>();

            // Note: Layers cannot be easily set if they don't exist. User must do this manually.
        }
    }
}
