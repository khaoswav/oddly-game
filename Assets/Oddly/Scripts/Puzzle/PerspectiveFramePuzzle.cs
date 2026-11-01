using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Solves when a dropped object visually matches a fixed target from the
    /// player's current viewpoint. World-space distance alone never completes it.
    /// </summary>
    public sealed class PerspectiveFramePuzzle : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private PerspectiveObject puzzleObject;
        [SerializeField] private Transform target;
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField, Range(0.005f, 0.12f)] private float alignmentTolerance = 0.035f;
        [SerializeField, Range(0.05f, 0.5f)] private float sizeTolerance = 0.18f;
        [SerializeField, Min(0.1f)] private float requiredDwell = 0.7f;
        [SerializeField, Min(0.1f)] private float hintCooldown = 4f;

        private Renderer objectRenderer;
        private float alignedSince = -1f;
        private float nextHintTime;
        private bool completed;

        private void Awake()
        {
            if (puzzleObject != null)
                objectRenderer = puzzleObject.GetComponentInChildren<Renderer>();
        }

        private void Update()
        {
            // Scene bootstrap assigns references immediately after AddComponent,
            // so resolve the renderer lazily as well as in Awake.
            if (objectRenderer == null && puzzleObject != null)
                objectRenderer = puzzleObject.GetComponentInChildren<Renderer>();

            if (completed || playerCamera == null || puzzleObject == null ||
                target == null || objectRenderer == null || puzzleObject.IsHeld)
            {
                alignedSince = -1f;
                return;
            }

            Vector3 objectViewport = playerCamera.WorldToViewportPoint(objectRenderer.bounds.center);
            Vector3 targetViewport = playerCamera.WorldToViewportPoint(target.position);
            if (objectViewport.z <= 0f || targetViewport.z <= 0f)
            {
                alignedSince = -1f;
                return;
            }

            float centerError = Vector2.Distance(
                new Vector2(objectViewport.x, objectViewport.y),
                new Vector2(targetViewport.x, targetViewport.y));

            Bounds bounds = objectRenderer.bounds;
            Vector3 edge = playerCamera.WorldToViewportPoint(
                bounds.center + playerCamera.transform.right * bounds.extents.x);
            float objectHalfWidth = Mathf.Abs(edge.x - objectViewport.x);
            Vector3 targetEdge = playerCamera.WorldToViewportPoint(
                target.position + playerCamera.transform.right * (target.localScale.x * 0.5f));
            float targetHalfWidth = Mathf.Abs(targetEdge.x - targetViewport.x);

            if (centerError <= alignmentTolerance && targetHalfWidth > 0.0001f)
            {
                float sizeError = Mathf.Abs(objectHalfWidth - targetHalfWidth) / targetHalfWidth;
                if (sizeError <= sizeTolerance)
                {
                    if (alignedSince < 0f) alignedSince = Time.time;
                    if (Time.time - alignedSince >= requiredDwell)
                        Solve();
                    return;
                }
            }

            alignedSince = -1f;
            if (Time.time >= nextHintTime && centerError < 0.16f)
            {
                nextHintTime = Time.time + hintCooldown;
                dialogue?.Say(centerError > alignmentTolerance * 2f
                    ? "perspective_align" : "perspective_size");
            }
        }

        private void Solve()
        {
            completed = true;
            linkedDoor?.Unlock();
            dialogue?.Say("perspective_solved");
        }
    }
}