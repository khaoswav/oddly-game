using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class TomorrowMemoryEntryTrigger : MonoBehaviour
    {
        [SerializeField] private TomorrowMemoryPuzzle puzzle;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh chapterSign;
        [SerializeField] private FallRecovery fallRecovery;
        [SerializeField] private Transform checkpoint;
        private bool entered;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (entered || other.GetComponentInParent<CharacterController>() == null) return;
            entered = true;
            fallRecovery?.SetCheckpoint(checkpoint);
            if (chapterSign != null)
                chapterSign.text = dialogue != null && dialogue.IsTurkish
                    ? "GİZLİ 04 / YARINI HATIRLAYAN ODA"
                    : "HIDDEN 04 / THE ROOM THAT REMEMBERS TOMORROW";
            puzzle?.InitializeFromHistory(FindObjectOfType<BehaviorTracker>());
            dialogue?.Say("hidden_chapter_four_entry");
        }
    }
}
