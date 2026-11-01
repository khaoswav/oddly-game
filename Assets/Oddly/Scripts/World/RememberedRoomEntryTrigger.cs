using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class RememberedRoomEntryTrigger : MonoBehaviour
    {
        [SerializeField] private RememberedSelfPuzzle puzzle;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh chapterSign;
        [SerializeField] private FallRecovery fallRecovery;
        [SerializeField] private Transform checkpoint;
        private bool entered;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (entered || other.GetComponentInParent<FirstPersonController>() == null) return;
            entered = true;
            fallRecovery?.SetCheckpoint(checkpoint);
            if (chapterSign != null)
                chapterSign.text = dialogue != null && dialogue.IsTurkish
                    ? "GİZLİ 08 / SENİ HATIRLAYAN ODA"
                    : "HIDDEN 08 / THE ROOM THAT REMEMBERS YOU";
            puzzle?.Begin();
        }
    }
}
