using UnityEngine;
using System.Collections;

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
        [SerializeField] private Transform revealPanel;
        [SerializeField, Min(0.1f)] private float revealDistance = 5.5f;
        [SerializeField, Min(0.1f)] private float revealSpeed = 3.2f;
        [SerializeField, Min(0f)] private float revealDelay = 1.2f;
        private Vector3 panelClosedPosition;
        private Vector3 panelOpenPosition;
        private bool opening;
        [SerializeField] private string dialogueId = "false_exit";
        [SerializeField] private string revealedSignEnglish = "NOT AN EXIT";
        [SerializeField] private string revealedSignTurkish = "ÇIKIŞ DEĞİL";
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
                sign.text = dialogue != null && dialogue.IsTurkish
                    ? revealedSignTurkish : revealedSignEnglish;
            dialogue?.Say(dialogueId);\n            if (revealPanel != null)\n                StartCoroutine(OpenRevealAfterDelay());
        }
    }
}