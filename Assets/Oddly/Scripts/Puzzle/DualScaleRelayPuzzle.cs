using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A two-object perspective relay. Both objects must be placed on their
    /// own pads at different world scales before the next door unlocks.
    /// </summary>
    public sealed class DualScaleRelayPuzzle : MonoBehaviour
    {
        [SerializeField] private PerspectiveObject smallObject;
        [SerializeField] private Transform smallTarget;
        [SerializeField] private float smallRequiredScale = 0.55f;
        [SerializeField] private PerspectiveObject largeObject;
        [SerializeField] private Transform largeTarget;
        [SerializeField] private float largeRequiredScale = 1.8f;
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField, Min(0.1f)] private float placementRadius = 0.9f;
        [SerializeField, Min(0.02f)] private float scaleTolerance = 0.18f;
        [SerializeField, Min(0.1f)] private float hintCooldown = 4f;

        private bool completed;
        private float nextHintTime;

        private void Update()
        {
            if (completed || smallObject == null || largeObject == null ||
                smallTarget == null || largeTarget == null) return;

            bool smallPlaced = IsCorrectlyPlaced(smallObject, smallTarget, smallRequiredScale);
            bool largePlaced = IsCorrectlyPlaced(largeObject, largeTarget, largeRequiredScale);

            if (smallPlaced && largePlaced)
            {
                completed = true;
                linkedDoor?.Unlock();
                dialogue?.Say("dual_scale_solved");
                return;
            }

            if (Time.time < nextHintTime) return;
            if (!smallPlaced && IsNearTarget(smallObject, smallTarget))
            {
                nextHintTime = Time.time + hintCooldown;
                dialogue?.Say(smallObject.CurrentUniformScale > smallRequiredScale
                    ? "dual_scale_small_shrink" : "dual_scale_small_grow");
            }
            else if (!largePlaced && IsNearTarget(largeObject, largeTarget))
            {
                nextHintTime = Time.time + hintCooldown;
                dialogue?.Say(largeObject.CurrentUniformScale > largeRequiredScale
                    ? "dual_scale_large_shrink" : "dual_scale_large_grow");
            }
        }

        private bool IsCorrectlyPlaced(PerspectiveObject item, Transform target, float requiredScale)
        {
            if (item == null || item.IsHeld || !IsNearTarget(item, target)) return false;
            return Mathf.Abs(item.CurrentUniformScale - requiredScale) <= scaleTolerance;
        }

        private bool IsNearTarget(PerspectiveObject item, Transform target)
        {
            if (item == null || target == null) return false;
            Vector2 itemPosition = new Vector2(item.transform.position.x, item.transform.position.z);
            Vector2 targetPosition = new Vector2(target.position.x, target.position.z);
            return Vector2.Distance(itemPosition, targetPosition) <= placementRadius &&
                   Mathf.Abs(item.transform.position.y - target.position.y) <= 0.9f;
        }
    }
}
