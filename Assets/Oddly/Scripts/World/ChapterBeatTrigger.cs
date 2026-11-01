using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Fires a one-time narrative beat when the player reaches a marked area.
    /// The environment changes with the line, making the reveal visible as well
    /// as readable in subtitles.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class ChapterBeatTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh sign;
        [SerializeField] private string dialogueId = "false_exit";
        [SerializeField] private string revealedSign = "NOT AN EXIT";
        private bool triggered;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered || other.GetComponentInParent<CharacterController>() == null)
                return;

            triggered = true;
            if (sign != null)
                sign.text = revealedSign;
            dialogue?.Say(dialogueId);
        }
    }
}