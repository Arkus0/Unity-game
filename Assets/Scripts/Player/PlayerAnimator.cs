using UnityEngine;

namespace JRPG.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator animator;
        private Rigidbody rb;
        private PlayerController controller;

        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int InputXHash = Animator.StringToHash("InputX");
        private static readonly int InputYHash = Animator.StringToHash("InputY");

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponentInParent<Rigidbody>(); // Rigidbody is on the parent (Capsule)
            controller = GetComponentInParent<PlayerController>();
        }

        private void Update()
        {
            if (rb == null || controller == null) return;

            // Get velocity directly from Rigidbody
            Vector3 velocity = rb.velocity;

            // Ignore small movements
            bool isMoving = velocity.magnitude > 0.1f;

            animator.SetBool(IsMovingHash, isMoving);

            if (isMoving)
            {
                // We use the facing direction stored in the controller to ensure
                // the character keeps looking at the last direction when stopped if we wanted,
                // but for Blend Trees, usually velocity or input is used.
                // Let's use the normalized velocity for Blend Trees.
                Vector3 localVelocity = velocity.normalized;

                animator.SetFloat(InputXHash, localVelocity.x);
                animator.SetFloat(InputYHash, localVelocity.z);
            }
        }
    }
}
