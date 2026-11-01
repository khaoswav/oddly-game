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

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            originalScale = transform.localScale;
        }

        public void PickUp(Transform holdPoint)
        {
            if (IsHeld) return;
            IsHeld = true;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
            transform.SetParent(holdPoint, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
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
