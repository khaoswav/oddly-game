using UnityEngine;

namespace Oddly
{
    public sealed class TimedSightlinePuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private Light pulseLight;
        [SerializeField] private Transform[] echoTargets = new Transform[3];
        [SerializeField, Min(1f)] private float pulsePeriod = 5.2f;
        [SerializeField, Min(0.2f)] private float pulseWindow = 1.45f;
        [SerializeField, Range(0.9f, 0.999f)] private float aimTolerance = 0.978f;

        private int completed;
        private bool solved;
        private bool pulseOpen;

        private void Update()
        {
            pulseOpen = Mathf.Repeat(Time.time, pulsePeriod) < pulseWindow;
            if (pulseLight != null)
            {
                pulseLight.color = pulseOpen
                    ? new Color(0.28f, 1f, 0.82f)
                    : new Color(0.48f, 0.22f, 0.72f);
                pulseLight.intensity = pulseOpen ? 2.1f : 0.55f;
            }
            RefreshStatus();
        }

        public void ObserveStation(int stationIndex, CharacterController player)
        {
            if (solved || player == null || stationIndex != completed ||
                stationIndex < 0 || stationIndex >= echoTargets.Length ||
                echoTargets[stationIndex] == null || !pulseOpen)
                return;

            FirstPersonController controller = player.GetComponent<FirstPersonController>();
            Transform view = controller != null ? controller.CameraTransform : null;
            if (view == null) return;

            Vector3 toTarget = (echoTargets[stationIndex].position - view.position).normalized;
            if (Vector3.Dot(view.forward, toTarget) < aimTolerance) return;

            completed++;
            dialogue?.Say("hidden_echo_aligned");
            if (completed >= echoTargets.Length)
            {
                solved = true;
                dialogue?.Say("hidden_echo_complete");
                exitDoor?.Unlock();
            }
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            if (statusSign == null) return;
            bool turkish = dialogue != null && dialogue.IsTurkish;
            string state = pulseOpen
                ? (turkish ? "DARBE AÇIK / HEDEFİ BUL" : "PULSE OPEN / FIND THE ECHO")
                : (turkish ? "MOR IŞIKTA BEKLE" : "WAIT FOR THE GREEN PULSE");
            if (solved)
                state = turkish ? "ÜÇ YANKI EŞLENDİ / ÇIKIŞ AÇIK" : "THREE ECHOES ALIGNED / EXIT OPEN";
            statusSign.text = $"{(turkish ? "YANKI" : "ECHO")} {completed}/3  |  {state}";
        }
    }
}
