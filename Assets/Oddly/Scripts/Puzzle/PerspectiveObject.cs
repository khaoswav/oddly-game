using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public sealed class PerspectiveObject : MonoBehaviour
    {
        [SerializeField] private float minimumScale = 0.35f;
        [SerializeField] private float maximumScale = 3.2f;
        [SerializeField] private float resizeStep = 0.18f;

        private Rigidbody body;
        private Vector3 originalScale;

        public bool IsHeld { get; private set; }
        public float CurrentUniformScale => transform.lossyScale.x;
        public float LastPerspectiveScaleFactor { get; private set; } = 1f;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            originalScale = transform.localScale;
        }

        public void PickUp(Transform holdPoint, Camera viewCamera)
        {
            if (IsHeld || holdPoint == null || viewCamera == null) return;

            // Preserve the object's apparent size as it moves from the world
            // into the fixed holding distance. This makes pickup distance a
            // real puzzle variable instead of treating every grab identically.
            float worldDistance = Vector3.Distance(viewCamera.transform.position, transform.position);
            float holdDistance = Vector3.Distance(viewCamera.transform.position, holdPoint.position);
            LastPerspectiveScaleFactor = worldDistance > 0.01f
                ? Mathf.Clamp(holdDistance / worldDistance, 0.35f, 2.8f)
                : 1f;
            float heldScale = transform.lossyScale.x * LastPerspectiveScaleFactor;

            IsHeld = true;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
            transform.SetParent(holdPoint, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * heldScale;
        }

        public void Drop()
        {
            if (!IsHeld) return;
            transform.SetParent(null, true);
            body.isKinematic = false;
            IsHeld = false;
        }

        public void Resize(float wheelDelta)
        {
            if (!IsHeld || Mathf.Abs(wheelDelta) < 0.01f) return;
            float factor = 1f + Mathf.Sign(wheelDelta) * resizeStep;
            Vector3 next = transform.localScale * factor;
            float scale = Mathf.Clamp(next.x, minimumScale, maximumScale);
            transform.localScale = Vector3.one * scale;
        }

        public void ResetScale()
        {
            transform.localScale = originalScale;
        }
    }
}
