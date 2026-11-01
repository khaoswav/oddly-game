using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Three rival control channels must be routed to the stable ownership pattern.
    /// Each console cycles through Narrator, SYSTEM-02 and Offline.
    /// </summary>
    public sealed class ControlRoomPuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private Transform[] channelProps;
        [SerializeField] private Light controlLight;

        private readonly int[] owners = { 0, 1, 2 };
        private readonly int[] requiredOwners = { 1, 0, 1 };
        private bool solved;

        private void Start()
        {
            RefreshStatus();
            ApplyChannelVisuals();
        }

        public void CycleConsole(int index)
        {
            if (solved || index < 0 || index >= owners.Length) return;

            owners[index] = (owners[index] + 1) % 3;
            ApplyChannelVisuals();
            RefreshStatus();

            if (IsCorrectConfiguration())
            {
                solved = true;
                if (exitDoor != null) exitDoor.Unlock();
                if (controlLight != null)
                {
                    controlLight.color = new Color(0.35f, 0.95f, 0.72f);
                    controlLight.intensity = 2f;
                }
                if (dialogue != null) dialogue.Say("control_room_stable");
            }
            else if (dialogue != null)
            {
                dialogue.Say("control_room_changed");
            }
        }

        private bool IsCorrectConfiguration()
        {
            for (int i = 0; i < owners.Length; i++)
                if (owners[i] != requiredOwners[i]) return false;
            return true;
        }

        private void ApplyChannelVisuals()
        {
            if (channelProps == null) return;
            for (int i = 0; i < channelProps.Length && i < owners.Length; i++)
            {
                Transform channel = channelProps[i];
                if (channel == null) continue;
                channel.localRotation = Quaternion.Euler(0f, owners[i] * 120f, 0f);
                channel.localScale = owners[i] == 2
                    ? new Vector3(1f, 0.35f, 1f)
                    : new Vector3(1f, 1f, 1f);
            }

            if (controlLight != null && !solved)
            {
                controlLight.color = owners[0] == owners[2]
                    ? new Color(1f, 0.48f, 0.28f)
                    : new Color(0.55f, 0.48f, 1f);
            }
        }

        private void RefreshStatus()
        {
            if (statusSign == null) return;
            bool english = PlayerPrefs.GetInt("oddly.language", 1) == 1;
            string[] labels = english
                ? new[] { "NARRATOR", "SYSTEM-02", "OFFLINE" }
                : new[] { "ANLATICI", "SYSTEM-02", "KAPALI" };
            string prefix = english ? "CHANNEL ROUTING" : "KANAL YÖNLENDİRME";
            string ready = english ? "STABLE / EXIT OPEN" : "DENGELİ / ÇIKIŞ AÇIK";
            statusSign.text = solved
                ? ready
                : prefix + "\nA: " + labels[owners[0]] +
                  "   B: " + labels[owners[1]] +
                  "   C: " + labels[owners[2]];
        }
    }
}