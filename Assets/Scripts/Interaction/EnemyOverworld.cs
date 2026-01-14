using UnityEngine;
using JRPG.Systems;

namespace JRPG.Interaction
{
    [RequireComponent(typeof(Collider))]
    public class EnemyOverworld : MonoBehaviour
    {
        [SerializeField] private string enemyId = "Slime";

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player touched enemy!");

                if (EncounterManager.Instance != null)
                {
                    EncounterManager.Instance.StartBattle(enemyId);

                    // Visual feedback: Destroy this enemy object to simulate "defeat" or "encounter started"
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("EncounterManager instance not found!");
                }
            }
        }
    }
}
