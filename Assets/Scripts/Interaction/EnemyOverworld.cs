using UnityEngine;
using JRPG.Systems;

namespace JRPG.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyOverworld : MonoBehaviour
    {
        [SerializeField] private string enemyId = "Slime";

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player touched enemy!");

                if (EncounterManager.Instance != null)
                {
                    EncounterManager.Instance.StartBattle(enemyId);

                    // Optional: Destroy this enemy object or disable it
                    // Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("EncounterManager instance not found!");
                }
            }
        }
    }
}
