using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A barrier that exists only while the player looks directly at it.
    /// Looking away removes both its render and collision, allowing passage.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Renderer))]
    public sealed class GazeReactiveBarrier : MonoBehaviour
    {
        [SerializeField] private Transform playerView;
        [SerializeField, Range(0.5f, 0.99f)] private float gazeThreshold = 0.91f;
        [SerializeField, Min(1f)] private float activationDistance = 10f;

        private Collider barrierCollider;
        private Renderer barrierRenderer;
        private bool currentlyObserved;

        private void Awake()
        {
            barrierCollider = GetComponent<Collider>();
            barrierRenderer = GetComponent<Renderer>();
        }

        private void Start()
        {
            if (playerView == null)
            {
                FirstPersonController player = FindObjectOfType<FirstPersonController>();
                if (player != null) playerView = player.CameraTransform;
            }
            SetObserved(false);
        }

        private void Update()
        {
            if (playerView == null) return;

            Vector3 toBarrier = transform.position - playerView.position;
            float distance = toBarrier.magnitude;
            bool observed = distance <= activationDistance && distance > 0.01f
                && Vector3.Dot(playerView.forward, toBarrier / distance) >= gazeThreshold;

            if (observed != currentlyObserved) SetObserved(observed);
        }

        private void SetObserved(bool observed)
        {
            currentlyObserved = observed;
            if (barrierCollider != null) barrierCollider.enabled = observed;
            if (barrierRenderer != null) barrierRenderer.enabled = observed;
        }
    }
}
