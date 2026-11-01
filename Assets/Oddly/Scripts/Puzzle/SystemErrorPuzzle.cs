using UnityEngine;

namespace Oddly
{
    public sealed class SystemErrorPuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField, Min(0f)] private float inputCooldown = 0.35f;

        private readonly int[] correctSequence = { 0, 1, 0 };
        private int progress;
        private float nextInputTime;
        private bool solved;

        public void Press(int switchIndex)
        {
            if (solved || Time.time < nextInputTime) return;
            nextInputTime = Time.time + inputCooldown;

            if (switchIndex == correctSequence[progress])
            {
                progress++;
                if (progress >= correctSequence.Length)
                {
                    solved = true;
                    linkedDoor?.Unlock();
                    dialogue?.Say("system_error_solved");
                }
                else
                {
                    dialogue?.Say(progress == 1 ? "system_error_first" : "system_error_second");
                }
                return;
            }

            progress = switchIndex == correctSequence[0] ? 1 : 0;
            dialogue?.Say("system_error_wrong");
        }
    }
}