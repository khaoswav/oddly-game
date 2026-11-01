using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public sealed class PerspectiveObject : MonoBehaviour
    {
        [SerializeField] private float minimumScale = 0.35f;
        [SerializeField] private float maximumScale = 3.2f;

        private Rigidbody body;
        private Vector3 originalScale;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Transform activeHoldPoint;
        private float scaleAtGrab;
        private float cameraDistanceAtGrab;

        public bool IsHeld { get; private set; }
        public bool IsSettled => !IsHeld && body != null && !body.isKinematic &&
            (body.IsSleeping() || (body.velocity.sqrMagnitude < 0.04f && body.angularVelocity.sqrMagnitude < 0.04f));
        public float CurrentUniformScale => transform.lossyScale.x;
        public float LastPerspectiveScaleFactor { get; private set; } = 1f;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            originalScale = transform.localScale;
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        private void Update()
        {
            if (IsHeld || transform.position.y >= -12f) return;

            body.isKinematic = true;
            transform.SetParent(null, true);
            transform.SetPositionAndRotation(originalPosition, originalRotation);
            transform.localScale = originalScale;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = false;
            body.Sleep();
        }

        public void PickUp(Transform holdPoint, Camera viewCamera)
        {
            if (IsHeld || holdPoint == null || viewCamera == null) return;

            float worldDistance = Vector3.Distance(viewCamera.transform.position, transform.position);
            float holdDistance = Mathf.Max(0.1f, holdPoint.localPosition.z);
            scaleAtGrab = transform.lossyScale.x;
            cameraDistanceAtGrab = Mathf.Max(0.1f, worldDistance);
            LastPerspectiveScaleFactor = Mathf.Clamp(holdDistance / cameraDistanceAtGrab, 0.35f, 2.8f);

            IsHeld = true;
            activeHoldPoint = holdPoint;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
            transform.SetParent(holdPoint, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            ApplyPerspectiveScale(holdDistance);
        }

        // Moving the held object through depth changes its real-world scale
        // while keeping its apparent size approximately constant on screen.
        public void UpdateHeldPerspective(float cameraDistance)
        {
            if (!IsHeld || activeHoldPoint == null) return;

            float distance = Mathf.Max(0.1f, cameraDistance);
            Vector3 holdPosition = activeHoldPoint.localPosition;
            holdPosition.z = distance;
            activeHoldPoint.localPosition = holdPosition;
            transform.localPosition = Vector3.zero;
            ApplyPerspectiveScale(distance);
        }

        private void ApplyPerspectiveScale(float cameraDistance)
        {
            float perspectiveScale = scaleAtGrab * (cameraDistance / cameraDistanceAtGrab);
            float clampedScale = Mathf.Clamp(perspectiveScale, minimumScale, maximumScale);
            // Scale the object's original dimensions uniformly instead of
            // flattening every non-cubic prop into a cube.
            float baseWidth = Mathf.Max(0.0001f, originalScale.x);
            transform.localScale = originalScale * (clampedScale / baseWidth);
        }

        public void Drop()
        {
            if (!IsHeld) return;
            transform.SetParent(null, true);
            body.isKinematic = false;
            IsHeld = false;
            activeHoldPoint = null;
        }

        public void ResetScale()
        {
            if (IsHeld) return;
            transform.localScale = originalScale;
        }
    }
}
