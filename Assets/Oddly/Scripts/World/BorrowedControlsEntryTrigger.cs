using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class BorrowedControlsEntryTrigger : MonoBehaviour
    {
        [SerializeField] private BorrowedControlsPuzzle puzzle;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh chapterSign;
        [SerializeField] private FallRecovery fallRecovery;
        [SerializeField] private Transform checkpoint;
        private bool entered;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            FirstPersonController controller = other.GetComponentInParent<FirstPersonController>();
            if (entered || controller == null) return;
            entered = true;
            fallRecovery?.SetCheckpoint(checkpoint);
            if (chapterSign != null)
                chapterSign.text = dialogue != null && dialogue.IsTurkish
                    ? "GİZLİ 05 / KONTROLLERİ ÖDÜNÇ ALAN ODA"
                    : "HIDDEN 05 / THE ROOM THAT BORROWS YOUR HANDS";
            puzzle?.Enter(controller);
            CampaignDirector.Instance?.EnterChapter(5, "THE ROOM THAT BORROWS YOUR HANDS", "KONTROLLERİ ÖDÜNÇ ALAN ODA", true);
        }
    }
}
