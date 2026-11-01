using UnityEngine;

namespace Oddly
{
    public sealed class DimensionSlotPuzzle : MonoBehaviour
    {
        [SerializeField] private PerspectiveObject slotObject;
        [SerializeField] private Transform landingTarget;
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField, Min(0.1f)] private float minimumScale = 0.35f;
        [SerializeField, Min(0.1f)] private float maximumScale = 0.62f;
        [SerializeField, Min(0.1f)] private float landingRadius = 1.15f;
        [SerializeField, Min(0.1f)] private float heightTolerance = 0.8f;
        [SerializeField, Min(0.1f)] private float hintCooldown = 4f;

        private float nextHintTime;
        private bool solved;

        private void Update()
        {
            if (solved || slotObject == null || landingTarget == null) return;

            float scale = slotObject.transform.lossyScale.x;
            float distance = Vector3.Distance(slotObject.transform.position, landingTarget.position);
            bool correctScale = scale >= minimumScale && scale <= maximumScale;
            bool landed = distance <= landingRadius &&
                          Mathf.Abs(slotObject.transform.position.y - landingTarget.position.y) <= heightTolerance;

            if (!slotObject.IsHeld && slotObject.IsSettled && correctScale && landed)
            {
                solved = true;
                linkedDoor?.Unlock();
                dialogue?.Say("dimension_slot_solved");
                return;
            }

            if (Time.time < nextHintTime) return;
            nextHintTime = Time.time + hintCooldown;

            if (slotObject.IsHeld)
                dialogue?.Say("dimension_slot_drop");
            else if (!correctScale)
                dialogue?.Say(scale > maximumScale ? "dimension_slot_shrink" : "dimension_slot_grow");
            else if (!landed)
                dialogue?.Say("dimension_slot_align");
            else
                dialogue?.Say("dimension_slot_settle");
        }
    }
}
