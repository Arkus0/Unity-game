using UnityEngine;

namespace JRPG.Systems
{
    // Simple serializable class to hold data.
    // In the future, this can be converted to JSON easily.
    [System.Serializable]
    public class PlayerData
    {
        public Vector3 position;
        public Quaternion rotation;
        public string currentScene;
        // Future: public List<InventoryItem> inventory;
        // Future: public CharacterStats stats;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public PlayerData playerData = new PlayerData();
        private bool hasSavedData = false;

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

        public void SavePlayerState(Vector3 position, Quaternion rotation, string sceneName)
        {
            playerData.position = position;
            playerData.rotation = rotation;
            playerData.currentScene = sceneName;
            hasSavedData = true;

            Debug.Log($"Game Saved: Pos {position}, Scene {sceneName}");
        }

        public bool TryLoadPlayerState(out Vector3 position, out Quaternion rotation)
        {
            if (hasSavedData)
            {
                position = playerData.position;
                rotation = playerData.rotation;
                return true;
            }

            position = Vector3.zero;
            rotation = Quaternion.identity;
            return false;
        }

        // Future: Methods to Save to Disk (JSON)
        // public void SaveToDisk() { ... }
        // public void LoadFromDisk() { ... }
    }
}
