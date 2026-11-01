using UnityEngine;

namespace Oddly
{
    public sealed class DoorController : MonoBehaviour
    {
        [SerializeField] private Transform movingPanel;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField, Min(0.1f)] private float openingDuration = 1.15f;
        [SerializeField] private AnimationCurve openingEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        private bool unlocked;
        private bool opening;
        private Vector3 closedPosition;
        private Vector3 openPosition;
        private float openingElapsed;

        private void Awake()
        {
            if (movingPanel == null) movingPanel = transform;
            closedPosition = movingPanel.localPosition;
            openPosition = closedPosition + Vector3.up * 4.5f;
        }

        private void Update()
        {
            if (!opening) return;
            openingElapsed += Time.deltaTime;
            float normalized = Mathf.Clamp01(openingElapsed / openingDuration);
            float eased = openingEase != null ? openingEase.Evaluate(normalized) : Mathf.SmoothStep(0f, 1f, normalized);
            movingPanel.localPosition = Vector3.LerpUnclamped(closedPosition, openPosition, eased);
            if (normalized >= 1f)
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

            openingElapsed = 0f;
            opening = true;
            dialogue?.Say("door_open");
        }
    }
}
