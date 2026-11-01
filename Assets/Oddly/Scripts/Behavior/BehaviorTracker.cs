using UnityEngine;

namespace Oddly
{
    public sealed class BehaviorTracker : MonoBehaviour
    {
        [SerializeField] private float idleThreshold = 22f;
        private float idleTimer;
        private int failedInteractions;
        private int doorAttempts;
        private bool idleCommentUsed;

        private DialogueDirector dialogue;

        private void Start() => dialogue = FindObjectOfType<DialogueDirector>();

        private void Update()
        {
            if (Input.anyKey || Mathf.Abs(Input.GetAxisRaw("Mouse X")) > 0.1f ||
                Mathf.Abs(Input.GetAxisRaw("Mouse Y")) > 0.1f)
            {
                idleTimer = 0f;
                idleCommentUsed = false;
            }
            else
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleThreshold && !idleCommentUsed)
                {
                    idleCommentUsed = true;
                    dialogue?.Say("idle");
                }
            }
        }

        public void RegisterPickup() { failedInteractions = 0; }
        public void RegisterDrop() { }
        public void RegisterDoorAttempt()
        {
            doorAttempts++;
            if (doorAttempts >= 2) dialogue?.Say("repeat");
        }
        public void RegisterFailedInteraction()
        {
            failedInteractions++;
            if (failedInteractions >= 3)
            {
                dialogue?.Say("repeat");
                failedInteractions = 0;
            }
            else if (failedInteractions == 1) dialogue?.Say("failed");
        }
    }
}
