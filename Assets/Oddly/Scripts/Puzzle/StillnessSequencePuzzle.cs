using UnityEngine;

namespace Oddly
{
    public sealed class StillnessSequencePuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private Light roomLight;
        [SerializeField, Min(0.1f)] private float stillnessRequired = 1.8f;
        [SerializeField, Min(0.01f)] private float movementTolerance = 0.12f;

        private readonly float[] dwell = new float[3];
        private int completed;
        private bool solved;
        private float lastHint = -10f;

        private void Start() => RefreshStatus();

        public void TickPad(int padIndex, CharacterController player)
        {
            if (solved || player == null || padIndex != completed || padIndex < 0 || padIndex >= 3)
                return;

            Vector3 horizontalVelocity = player.velocity;
            horizontalVelocity.y = 0f;
            if (horizontalVelocity.magnitude > movementTolerance)
            {
                dwell[padIndex] = 0f;
                if (Time.time - lastHint > 2.5f)
                {
                    lastHint = Time.time;
                    dialogue?.Say("hidden_stillness_move");
                }
                RefreshStatus();
                return;
            }

            dwell[padIndex] += Time.deltaTime;
            if (dwell[padIndex] < stillnessRequired)
            {
                RefreshStatus();
                return;
            }

            completed++;
            if (roomLight != null)
            {
                roomLight.color = Color.Lerp(new Color(0.42f, 0.28f, 0.72f),
                    new Color(0.38f, 0.95f, 0.82f), completed / 3f);
                roomLight.intensity = 1f + completed * 0.35f;
            }

            dialogue?.Say("hidden_stillness_step");
            RefreshStatus();

            if (completed < 3) return;
            solved = true;
            if (statusSign != null)
                statusSign.text = dialogue != null && dialogue.IsTurkish
                    ? "SESSİZLİK TAMAMLANDI / ÇIKIŞ AÇIK"
                    : "STILLNESS COMPLETE / EXIT OPEN";
            dialogue?.Say("hidden_stillness_complete");
            exitDoor?.Unlock();
        }

        private void RefreshStatus()
        {
            if (statusSign == null) return;
            bool turkish = dialogue != null && dialogue.IsTurkish;
            string prefix = turkish ? "NEFESİNİ TUT" : "HOLD YOUR BREATH";
            statusSign.text = $"{prefix} / {completed}/3";
        }
    }
}
