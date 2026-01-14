using UnityEngine;

namespace JRPG.Interaction
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [TextArea]
        [SerializeField] private string dialogueText = "Hello traveler!";

        public void SetDialogue(string text)
        {
            dialogueText = text;
        }

        public void Interact()
        {
            // In a real game, this would call a DialogueManager.
            // For now, we'll just log to the console.
            Debug.Log($"NPC Says: {dialogueText}");
        }
    }
}
