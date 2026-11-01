using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class ShadowRoomEntryTrigger : MonoBehaviour
    {
        [SerializeField] private ShadowTrailPuzzle puzzle;
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
                    ? "GİZLİ 07 / GÖLGESİNİ ÇALAN ODA"
                    : "HIDDEN 07 / THE ROOM THAT STEALS YOUR SHADOW";
            puzzle?.Begin(controller);
            CampaignDirector.Instance?.EnterChapter(7, "THE ROOM THAT STEALS YOUR SHADOW", "GÖLGESİNİ ÇALAN ODA", true);
        }
    }
}
