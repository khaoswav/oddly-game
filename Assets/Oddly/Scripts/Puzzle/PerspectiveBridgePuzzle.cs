using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Chapter 2's bridge puzzle. The player must turn a small perspective plank
    /// into a full-size physical bridge and let it settle across the gap.
    /// </summary>
    public sealed class PerspectiveBridgePuzzle : MonoBehaviour
    {
        [SerializeField] private PerspectiveObject bridge;
        [SerializeField] private Transform landingTarget;
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField, Min(0.1f)] private float placementRadius = 1.15f;
        [SerializeField, Min(0.1f)] private float minimumBridgeScale = 2.55f;
        [SerializeField, Min(0.1f)] private float maximumBridgeScale = 3.2f;
        [SerializeField, Min(0.1f)] private float hintCooldown = 4f;

        private bool completed;
        private float nextHintTime;

        private void Update()
        {
            if (completed || bridge == null || landingTarget == null) return;

            bool nearLanding = IsNearLanding();
            bool settled = !bridge.IsHeld && bridge.IsSettled;
            float scale = bridge.CurrentUniformScale;
            bool correctScale = scale >= minimumBridgeScale && scale <= maximumBridgeScale;

            if (nearLanding && settled && correctScale)
            {
                completed = true;
                linkedDoor?.Unlock();
                dialogue?.Say("bridge_solved");
                return;
            }

            if (!nearLanding || Time.time < nextHintTime) return;
            nextHintTime = Time.time + hintCooldown;

            if (scale < minimumBridgeScale)
                dialogue?.Say("bridge_grow");
            else if (scale > maximumBridgeScale)
                dialogue?.Say("bridge_shrink");
            else if (!settled)
                dialogue?.Say("bridge_settle");
            else
                dialogue?.Say("bridge_align");
        }

        private bool IsNearLanding()
        {
            Vector2 current = new Vector2(bridge.transform.position.x, bridge.transform.position.z);
            Vector2 target = new Vector2(landingTarget.position.x, landingTarget.position.z);
            return Vector2.Distance(current, target) <= placementRadius &&
                   Mathf.Abs(bridge.transform.position.y - landingTarget.position.y) <= 0.85f;
        }
    }
}