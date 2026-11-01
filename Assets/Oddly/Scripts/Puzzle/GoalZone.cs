using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(Collider))]
    public sealed class GoalZone : MonoBehaviour
    {
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;
        private bool completed;

        private void OnTriggerEnter(Collider other) => Check(other);
        private void OnTriggerStay(Collider other) => Check(other);

        private void Check(Collider other)
        {
            if (completed) return;
            PerspectiveObject puzzleObject = other.GetComponentInParent<PerspectiveObject>();
            if (puzzleObject == null || puzzleObject.IsHeld) return;

            Vector3 local = transform.InverseTransformPoint(puzzleObject.transform.position);
            if (Mathf.Abs(local.x) < 0.65f && Mathf.Abs(local.z) < 0.65f && local.y > -0.1f && local.y < 1.2f)
            {
                completed = true;
                linkedDoor.Unlock();
                dialogue?.Say("puzzle_solved");
            }
        }
    }
}
