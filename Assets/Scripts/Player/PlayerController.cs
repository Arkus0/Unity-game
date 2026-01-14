using UnityEngine;
using JRPG.Interaction;

namespace JRPG.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Interaction Settings")]
        [SerializeField] private float interactDistance = 1f;
        [SerializeField] private LayerMask interactLayer;

        private Rigidbody2D rb;
        private Vector2 movementInput;
        private Vector2 lastMovedDirection; // Useful for interactions (facing direction)

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            // Default facing direction (down)
            lastMovedDirection = Vector2.down;
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
            float moveY = Input.GetAxisRaw("Vertical");

            movementInput = new Vector2(moveX, moveY).normalized;

            if (movementInput != Vector2.zero)
            {
                lastMovedDirection = movementInput;
            }
        }

        private void Move()
        {
            rb.velocity = movementInput * moveSpeed;
        }

        private void Interact()
        {
            Vector2 interactPos = (Vector2)transform.position + lastMovedDirection * interactDistance;

            // Check for colliders at the interaction position
            Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.5f, interactLayer);

            if (collider != null)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }

        // Public getter for other systems to know where the player is looking
        public Vector2 GetFacingDirection()
        {
            return lastMovedDirection;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector2 interactPos = (Vector2)transform.position + lastMovedDirection * interactDistance;
            Gizmos.DrawWireSphere(interactPos, 0.5f);
        }
    }
}
