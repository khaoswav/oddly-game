using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(Collider))]
    public sealed class GoalZone : MonoBehaviour
    {
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField, Range(0.2f, 2f)] private float requiredObjectScale = 0.55f;
        [SerializeField, Range(0.02f, 0.5f)] private float scaleTolerance = 0.12f;
        [SerializeField] private float hintCooldown = 3.5f;

        private bool completed;
        private float nextHintTime;

        private void OnTriggerEnter(Collider other) => Check(other);
        private void OnTriggerStay(Collider other) => Check(other);

        private void Check(Collider other)
        {
            if (completed) return;

            PerspectiveObject puzzleObject = other.GetComponentInParent<PerspectiveObject>();
            if (puzzleObject == null || puzzleObject.IsHeld) return;

            Vector3 local = transform.InverseTransformPoint(puzzleObject.transform.position);
            bool insidePlacement = Mathf.Abs(local.x) < 0.65f &&
                                   Mathf.Abs(local.z) < 0.65f &&
                                   local.y > -0.1f && local.y < 1.2f;
            if (!insidePlacement) return;

            float objectScale = puzzleObject.CurrentUniformScale;
            if (Mathf.Abs(objectScale - requiredObjectScale) > scaleTolerance)
            {
                if (Time.time >= nextHintTime)
                {
                    nextHintTime = Time.time + hintCooldown;
                    dialogue?.Say(objectScale > requiredObjectScale ? "object_too_big" : "object_too_small");
                }
                return;
            }

            completed = true;
            if (linkedDoor != null) linkedDoor.Unlock();
            dialogue?.Say("puzzle_solved");
        }
    }
}
