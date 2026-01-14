using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using JRPG.Interaction;
using JRPG.Systems;

namespace JRPG.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Interaction Settings")]
        [SerializeField] private float interactDistance = 1f;
        [SerializeField] private LayerMask interactLayer = ~0;

        private Rigidbody rb;
        private Vector3 movementInput;
        private Vector3 lastMovedDirection;

        // Input Actions defined in code to avoid Asset dependency
        private InputAction moveAction;
        private InputAction interactAction;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            lastMovedDirection = Vector3.back;

            SetupInput();
        }

        private void SetupInput()
        {
            // Define Movement Action (WASD + Arrows)
            moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
            moveAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");

            // Define Interact Action (E, Space, Gamepad South)
            interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
            interactAction.AddBinding("<Keyboard>/space");
            interactAction.AddBinding("<Gamepad>/buttonSouth");

            interactAction.performed += ctx => Interact();
        }

        private void OnEnable()
        {
            moveAction.Enable();
            interactAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            interactAction.Disable();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.TryLoadPlayerState(out Vector3 savedPos, out Quaternion savedRot))
                {
                    transform.position = savedPos;
                    transform.rotation = savedRot;
                    Debug.Log("Player Position Restored from GameManager.");
                }
            }
        }

        private void Update()
        {
            ProcessInputs();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void ProcessInputs()
        {
            // Read Vector2 from new input system
            Vector2 input = moveAction.ReadValue<Vector2>();

            // Convert to Vector3 (X, 0, Z)
            movementInput = new Vector3(input.x, 0f, input.y);

            if (movementInput != Vector3.zero)
            {
                lastMovedDirection = movementInput.normalized;
            }
        }

        private void Move()
        {
            rb.velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.z * moveSpeed);

            if (movementInput != Vector3.zero)
            {
                transform.forward = movementInput.normalized;
            }
        }

        private void Interact()
        {
            Vector3 interactPos = transform.position + lastMovedDirection * interactDistance;

            Collider[] colliders = Physics.OverlapSphere(interactPos, 0.5f, interactLayer);

            foreach (Collider col in colliders)
            {
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    return;
                }
            }
        }

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
