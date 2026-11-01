using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Fires a localized chapter beat once when the player enters a new area.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class ChapterEntryTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh chapterSign;
        [SerializeField] private FallRecovery fallRecovery;
        [SerializeField] private Transform checkpoint;
        [SerializeField] private string dialogueId = "chapter_two_entry";
        [SerializeField] private string englishSign = "CHAPTER 02  /  SIZE DOES MATTER";
        [SerializeField] private string turkishSign = "BÖLÜM 02  /  BOYUT ÖNEMLİDİR";

        private bool triggered;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered || other.GetComponentInParent<CharacterController>() == null) return;
            triggered = true;
            fallRecovery?.SetCheckpoint(checkpoint);

            if (chapterSign != null)
                chapterSign.text = dialogue != null && dialogue.IsTurkish
                    ? turkishSign : englishSign;

            dialogue?.Say(dialogueId);
        }
    }
}