using UnityEngine;
using UnityEngine.SceneManagement;

namespace JRPG.Systems
{
    public class EncounterManager : MonoBehaviour
    {
        public static EncounterManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartBattle(string enemyId)
        {
            Debug.Log($"Starting Battle with Enemy: {enemyId}");

            // Logic to load combat scene would go here.
            // SceneManager.LoadScene("BattleScene");

            // You might want to save the player's position before loading the battle scene.
        }
    }
}
