using UnityEngine;

namespace JRPG.Systems
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;

        [Header("Settings")]
        public Vector3 offset = new Vector3(0, 5, -8); // Default 3rd person-ish offset
        public float smoothSpeed = 5f; // Adjusted for Time.deltaTime
        public bool lookAtTarget = true;

        private void LateUpdate()
        {
            if (target == null)
                return;

            Vector3 desiredPosition = target.position + offset;
            // Simple smoothing using Lerp with deltaTime
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            if (lookAtTarget)
            {
                transform.LookAt(target);
            }
        }
    }
}
