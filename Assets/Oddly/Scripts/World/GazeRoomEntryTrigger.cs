using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class GazeRoomEntryTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh chapterSign;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private FallRecovery fallRecovery;
        [SerializeField] private Transform checkpoint;
        private bool entered;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (entered || other.GetComponentInParent<FirstPersonController>() == null) return;
            entered = true;
            fallRecovery?.SetCheckpoint(checkpoint);
            bool turkish = dialogue != null && dialogue.IsTurkish;
            if (chapterSign != null)
                chapterSign.text = turkish
                    ? "GİZLİ 06 / BAKTIĞINDA VAR OLAN ODA"
                    : "HIDDEN 06 / THE ROOM THAT EXISTS WHEN WATCHED";
            if (statusSign != null)
                statusSign.text = turkish
                    ? "DUVARLARA BAKMA.\nBAKIŞINI ÇEVİR VE GEÇ."
                    : "DO NOT LOOK AT THE WALLS.\nTURN AWAY TO PASS.";
            dialogue?.Say("hidden_gaze_room_entry");
        }
    }
}
