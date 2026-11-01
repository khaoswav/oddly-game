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
            movingPanel.localPosition = Vector3.Lerp(movingPanel.localPosition, openPosition, Time.deltaTime * 2.2f);
        }

        public void Unlock()
        {
            unlocked = true;
            dialogue?.Say("door_unlocked");
        }

        public void TryOpen()
        {
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
