using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Changes the false-exit sign and raises the wall panel to reveal the next
    /// playable space. Scene references are wired by the editor scene builder,
    /// so initialization happens in Start rather than Awake.
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
        [SerializeField] private string dialogueId = "false_exit";
        [SerializeField] private string revealedSignEnglish = "NOT AN EXIT";
        [SerializeField] private string revealedSignTurkish = "ÇIKIŞ DEĞİL";

        private Vector3 panelOpenPosition;
        private bool opening;
        private bool triggered;

        private void Start()
        {
            if (revealPanel != null)
                panelOpenPosition = revealPanel.localPosition + Vector3.up * revealDistance;
        }

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

            dialogue?.Say(dialogueId);
            if (revealPanel != null)
                StartCoroutine(OpenRevealAfterDelay());
        }

        private IEnumerator OpenRevealAfterDelay()
        {
            yield return new WaitForSeconds(revealDelay);
            opening = true;
        }

        private void Update()
        {
            if (!opening || revealPanel == null)
                return;

            revealPanel.localPosition = Vector3.MoveTowards(
                revealPanel.localPosition, panelOpenPosition, revealSpeed * Time.deltaTime);

            if (Vector3.SqrMagnitude(revealPanel.localPosition - panelOpenPosition) < 0.0001f)
            {
                revealPanel.localPosition = panelOpenPosition;
                opening = false;
                dialogue?.Say("chapter_reveal");
            }
        }
    }
}