using UnityEngine;

namespace Oddly
{
    public sealed class DoorController : MonoBehaviour
    {
        [SerializeField] private Transform movingPanel;
        [SerializeField] private DialogueDirector dialogue;
        private bool unlocked;
        private bool opening;
        private Vector3 closedPosition;
        private Vector3 openPosition;

        private void Awake()
        {
            if (movingPanel == null) movingPanel = transform;
            closedPosition = movingPanel.localPosition;
            openPosition = closedPosition + Vector3.up * 4.5f;
        }

        private void Update()
        {
            if (!opening) return;
            movingPanel.localPosition = Vector3.MoveTowards(
                movingPanel.localPosition, openPosition, 3.2f * Time.deltaTime);
            if (Vector3.SqrMagnitude(movingPanel.localPosition - openPosition) < 0.0001f)
            {
                movingPanel.localPosition = openPosition;
                opening = false;
            }
        }

        public void Unlock()
        {
            unlocked = true;
            dialogue?.Say("door_unlocked");
        }

        public void TryOpen()
        {
            if (opening || movingPanel.localPosition == openPosition) return;

            if (!unlocked)
            {
                dialogue?.Say("door_locked");
                FindObjectOfType<BehaviorTracker>()?.RegisterDoorAttempt();
                return;
            }

            opening = true;
            dialogue?.Say("door_open");
        }
    }
}
