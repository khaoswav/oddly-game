using System.Collections;
using UnityEngine;

namespace Oddly
{
    public sealed class ChapterIntroDirector : MonoBehaviour
    {
        [SerializeField] private FirstPersonController movement;
        [SerializeField] private InteractionController interaction;
        [SerializeField] private BehaviorTracker behavior;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField, Min(0.5f)] private float beatDuration = 2.6f;

        private Coroutine sequence;
        private bool started;

        public void Begin()
        {
            if (started) return;
            started = true;
            if (sequence != null) StopCoroutine(sequence);
            sequence = StartCoroutine(PlayOpening());
        }

        private IEnumerator PlayOpening()
        {
            SetGameplayEnabled(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            dialogue?.Say("chapter_welcome");
            yield return new WaitForSecondsRealtime(beatDuration);

            dialogue?.Say("chapter_system");
            yield return new WaitForSecondsRealtime(beatDuration);

            dialogue?.Say("chapter_narrator");
            yield return new WaitForSecondsRealtime(beatDuration);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetGameplayEnabled(true);
            dialogue?.Say("chapter_go");
            sequence = null;
        }

        private void SetGameplayEnabled(bool enabled)
        {
            if (movement != null) movement.enabled = enabled;
            if (interaction != null) interaction.enabled = enabled;
            if (behavior != null) behavior.enabled = enabled;
        }
    }
}