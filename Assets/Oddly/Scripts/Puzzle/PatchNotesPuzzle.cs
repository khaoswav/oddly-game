using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Patch notes are not documentation: applying each one changes the room.
    /// The release order is 3.0, 1.0, 2.0, not chronological order.
    /// </summary>
    public sealed class PatchNotesPuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private Transform[] patchProps = new Transform[3];
        [SerializeField] private Light roomLight;
        [SerializeField] private int[] releaseOrder = { 2, 0, 1 };

        private readonly bool[] applied = new bool[3];
        private int progress;
        private bool solved;

        public void ApplyPatch(int patchIndex)
        {
            if (solved || patchIndex < 0 || patchIndex >= 3 || applied[patchIndex])
                return;

            if (patchIndex != releaseOrder[progress])
            {
                progress = 0;
                for (int i = 0; i < applied.Length; i++)
                    applied[i] = false;
                ResetProps();
                dialogue?.Say("patch_wrong_order");
                UpdateSign();
                return;
            }

            applied[patchIndex] = true;
            ApplyWorldChange(patchIndex);
            progress++;
            dialogue?.Say(progress == 3 ? "patch_complete" : "patch_applied");

            if (progress >= releaseOrder.Length)
            {
                solved = true;
                exitDoor?.Unlock();
                if (roomLight != null)
                {
                    roomLight.color = new Color(0.35f, 0.9f, 0.65f);
                    roomLight.intensity = 1.6f;
                }
            }

            UpdateSign();
        }

        private void ApplyWorldChange(int index)
        {
            if (patchProps[index] != null)
            {
                switch (index)
                {
                    case 0:
                        patchProps[index].localScale = new Vector3(1.8f, 0.55f, 1.8f);
                        break;
                    case 1:
                        patchProps[index].Rotate(0f, 90f, 0f, Space.World);
                        break;
                    case 2:
                        patchProps[index].position += Vector3.up * 1.1f;
                        break;
                }
            }

            if (roomLight != null)
            {
                roomLight.color = index == 0
                    ? new Color(0.95f, 0.55f, 0.25f)
                    : index == 1
                        ? new Color(0.45f, 0.55f, 1f)
                        : new Color(0.8f, 0.35f, 0.85f);
                roomLight.intensity = 1.1f + progress * 0.15f;
            }
        }

        private void ResetProps()
        {
            if (patchProps[0] != null) patchProps[0].localScale = Vector3.one;
            if (patchProps[1] != null) patchProps[1].localRotation = Quaternion.identity;
            if (patchProps[2] != null) patchProps[2].localPosition = new Vector3(
                patchProps[2].localPosition.x, 0.65f, patchProps[2].localPosition.z);
            if (roomLight != null)
            {
                roomLight.color = Color.white;
                roomLight.intensity = 0.8f;
            }
        }

        private void UpdateSign()
        {
            if (statusSign == null) return;
            bool turkish = dialogue != null && dialogue.IsTurkish;
            statusSign.text = solved
                ? (turkish ? "YAMA TAMAMLANDI / SÜRÜM 11.0" : "PATCH COMPLETE / VERSION 11.0")
                : (turkish
                    ? "YAMA SIRASI: 3.0 → 1.0 → 2.0   (" + progress + "/3)"
                    : "PATCH ORDER: 3.0 → 1.0 → 2.0   (" + progress + "/3)");
        }
    }
}