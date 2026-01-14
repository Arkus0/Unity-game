using UnityEngine;
using JRPG.Interaction;

namespace JRPG.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Interaction Settings")]
        [SerializeField] private float interactDistance = 1f;
        [SerializeField] private LayerMask interactLayer;

        private Rigidbody rb;
        private Vector3 movementInput;
        private Vector3 lastMovedDirection; // Useful for interactions (facing direction)

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            // Default facing direction (Back/South in 3D usually)
            lastMovedDirection = Vector3.back;
        }

        private void Update()
        {
            // Process Inputs
            ProcessInputs();

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                Interact();
            }
        }

        private void FixedUpdate()
        {
            // Physics calculations
            Move();
        }

        private void ProcessInputs()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical"); // In 3D, Vertical is Z axis

            // We move on X and Z plane. Y is up/down.
            movementInput = new Vector3(moveX, 0f, moveZ).normalized;

            if (movementInput != Vector3.zero)
            {
                lastMovedDirection = movementInput;
            }
        }

        private void Move()
        {
            // Set velocity directly, preserving Y velocity (gravity)
            rb.velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.z * moveSpeed);

            // Optional: Rotate player to face movement direction
            if (movementInput != Vector3.zero)
            {
                transform.forward = movementInput;
            }
        }

        private void Interact()
        {
            Vector3 interactPos = transform.position + lastMovedDirection * interactDistance;

            // Check for colliders at the interaction position
            // Using OverlapSphere for 3D
            Collider[] colliders = Physics.OverlapSphere(interactPos, 0.5f, interactLayer);

            foreach (Collider col in colliders)
            {
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    return; // Interact with only one object at a time
                }
            }
        }

        // Public getter for other systems to know where the player is looking
        public Vector3 GetFacingDirection()
        {
            return lastMovedDirection;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 interactPos = transform.position + lastMovedDirection * interactDistance;
            Gizmos.DrawWireSphere(interactPos, 0.5f);
        }
    }
}
