using UnityEngine;
namespace Oddly
{
    public class LastSavePuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private Transform[] snapshotProps;
        [SerializeField] private Light roomLight;

        private Vector3[] originalPositions;
        private Quaternion[] originalRotations;
        private bool solved;
        private int lastSelected = -1;

        private void Start()
        {
            if (snapshotProps != null)
            {
                originalPositions = new Vector3[snapshotProps.Length];
                originalRotations = new Quaternion[snapshotProps.Length];
                for (int i = 0; i < snapshotProps.Length; i++)
                {
                    if (snapshotProps[i] == null) continue;
                    originalPositions[i] = snapshotProps[i].position;
                    originalRotations[i] = snapshotProps[i].rotation;
                }
            }
            UpdateStatus("NO SAVE LOADED / KAYIT YÜKLENMEDİ");
        }

        public void LoadSlot(int slot)
        {
            if (solved || slot < 0 || slot > 2 || slot == lastSelected) return;
            lastSelected = slot;

            if (slot == 1)
            {
                RestoreOriginalSnapshot();
                solved = true;
                UpdateStatus("SAVE 02 RESTORED / EXIT AVAILABLE");
                if (roomLight != null)
                {
                    roomLight.color = new Color(0.35f, 0.95f, 0.72f);
                    roomLight.intensity = 1.8f;
                }
                if (exitDoor != null) exitDoor.Unlock();
                if (dialogue != null) dialogue.Play("last_save_restored");
                return;
            }

            ApplyCorruptSnapshot(slot);
            UpdateStatus(slot == 0
                ? "SAVE 01 / BEFORE THE CRASH"
                : "SAVE 03 / AFTER THE CRASH");
            if (roomLight != null)
            {
                roomLight.color = slot == 0
                    ? new Color(1f, 0.55f, 0.25f)
                    : new Color(0.65f, 0.35f, 1f);
                roomLight.intensity = 1.15f;
            }
            if (dialogue != null) dialogue.Play("last_save_wrong");
        }

        private void RestoreOriginalSnapshot()
        {
            if (snapshotProps == null) return;
            for (int i = 0; i < snapshotProps.Length; i++)
            {
                if (snapshotProps[i] == null) continue;
                snapshotProps[i].SetPositionAndRotation(originalPositions[i], originalRotations[i]);
            }
        }

        private void ApplyCorruptSnapshot(int slot)
        {
            if (snapshotProps == null) return;
            for (int i = 0; i < snapshotProps.Length; i++)
            {
                if (snapshotProps[i] == null) continue;
                snapshotProps[i].position = originalPositions[i] + (slot == 0
                    ? new Vector3(0f, i == 1 ? 1.8f : 0f, i == 0 ? 1.2f : 0f)
                    : new Vector3(i == 2 ? 1.8f : 0f, 0f, i == 1 ? -1.2f : 0f));
                snapshotProps[i].rotation = originalRotations[i] *
                    Quaternion.Euler(0f, slot == 0 ? 28f : -42f, 0f);
            }
        }

        private void UpdateStatus(string value)
        {
            if (statusSign == null) return;
            bool english = PlayerPrefs.GetInt("oddly.language", 1) == 1;
            statusSign.text = value.Contains("/") && !english
                ? value.Substring(value.IndexOf('/') + 1).Trim() + " / " + value.Substring(0, value.IndexOf('/')).Trim()
                : value;
        }
    }
}