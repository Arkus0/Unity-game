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

            // 1. Save Player Position before switching scenes
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && GameManager.Instance != null)
            {
                GameManager.Instance.SavePlayerState(player.transform.position, player.transform.rotation, SceneManager.GetActiveScene().name);
            }

            // 2. Load Combat Scene
            // For now we just log it, but here is where you would call:
            // SceneManager.LoadScene("BattleScene");
        }
    }
}
