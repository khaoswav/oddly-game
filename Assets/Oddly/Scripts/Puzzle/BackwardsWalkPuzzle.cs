using UnityEngine;

namespace Oddly
{
    public sealed class BackwardsWalkPuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private Light roomLight;
        [SerializeField] private Vector3 pathDirection = Vector3.right;
        [SerializeField] private int requiredSteps = 3;

        private int completedSteps;
        private float lastHintAt = -10f;
        private bool solved;

        private void Start() => RefreshStatus();

        public void TryStep(int stepIndex, CharacterController player)
        {
            if (solved || player == null || stepIndex != completedSteps) return;

            Vector3 direction = pathDirection.normalized;
            float facingAway = Vector3.Dot(player.transform.forward, -direction);
            float backwardsSpeed = Vector3.Dot(player.velocity, direction);

            if (facingAway < 0.72f || backwardsSpeed < 0.45f)
            {
                if (Time.time - lastHintAt > 2.5f)
                {
                    lastHintAt = Time.time;
                    dialogue?.Say("hidden_backwards_hint");
                }
                return;
            }

            completedSteps++;
            if (roomLight != null)
            {
                roomLight.color = Color.Lerp(new Color(0.45f, 0.3f, 0.8f),
                    new Color(0.3f, 0.95f, 0.72f),
                    completedSteps / (float)requiredSteps);
                roomLight.intensity = 1.1f + completedSteps * 0.3f;
            }

            dialogue?.Say("hidden_backwards_step");
            RefreshStatus();

            if (completedSteps < requiredSteps) return;
            solved = true;
            if (statusSign != null)
                statusSign.text = dialogue != null && dialogue.IsTurkish
                    ? "YOL HATIRLANDI / ÇIKIŞ AÇIK"
                    : "PATH REMEMBERED / EXIT OPEN";
            dialogue?.Say("hidden_backwards_complete");
            HiddenChapterArchive.MarkCompleted(1);
            exitDoor?.Unlock();
        }

        private void RefreshStatus()
        {
            if (statusSign == null) return;
            statusSign.text = dialogue != null && dialogue.IsTurkish
                ? $"GERİYE DOĞRU YÜRÜ / {completedSteps}/{requiredSteps}"
                : $"WALK BACKWARDS / {completedSteps}/{requiredSteps}";
        }
    }
}
