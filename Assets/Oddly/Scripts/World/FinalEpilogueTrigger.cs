using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A quiet post-finale coda. It acknowledges the selected ending and
    /// discovered hidden rooms, then offers a replay that preserves the archive.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class FinalEpilogueTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;

        private bool playerInside;
        private bool epilogueShown;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() == null) return;
            playerInside = true;

            if (!epilogueShown && statusSign != null)
                statusSign.text = dialogue != null && dialogue.IsTurkish
                    ? "SON KARARINI HATIRLA / E'YE BAS"
                    : "REMEMBER YOUR CHOICE / PRESS E";
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() != null)
                playerInside = false;
        }

        private void Update()
        {
            if (!playerInside || PlayerPrefs.GetInt("oddly.campaign.complete", 0) != 1)
                return;

            if (!epilogueShown && Input.GetKeyDown(KeyCode.E))
                RevealEpilogue();
            else if (epilogueShown && Input.GetKeyDown(KeyCode.R))
                CampaignDirector.BeginReplayPreservingArchive();
        }

        private void RevealEpilogue()
        {
            int ending = Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2);
            int hiddenCount = HiddenChapterArchive.CompletedCount;
            bool turkish = dialogue != null && dialogue.IsTurkish;

            string callbackTr;
            string callbackEn;
            string dialogueId;

            switch (ending)
            {
                case 0:
                    callbackTr = "Anlatıcı: Bütün odaları gezdiysen, hikâyenin bazı kısımlarını benden önce öğrenmişsindir.";
                    callbackEn = "Narrator: If you found every room, you learned parts of the story before I could tell them.";
                    dialogueId = "epilogue_narrator";
                    break;
                case 1:
                    callbackTr = "SYSTEM-02: Arşivdeki her gizli oda, ana senaryonun dışında kalan bir kayıt olarak korundu.";
                    callbackEn = "SYSTEM-02: Every hidden room remains archived as a record outside the main scenario.";
                    dialogueId = "epilogue_system";
                    break;
                default:
                    callbackTr = "İkisi birden: Bizi seçmedin. Ama bütün bu odalarda izimizi buldun. Bu da bir tür cevap.";
                    callbackEn = "Both voices: You chose neither of us. Yet you found traces of us in every room. That is an answer of its own.";
                    dialogueId = "epilogue_player";
                    break;
            }

            if (statusSign != null)
            {
                statusSign.text = turkish
                    ? "SONRASI\n" + callbackTr + "\n\nGİZLİ ODALAR: " + hiddenCount + " / 8\nTEKRAR OYNAMAK İÇİN R'YE BAS"
                    : "AFTER THE END\n" + callbackEn + "\n\nHIDDEN ROOMS: " + hiddenCount + " / 8\nPRESS R TO PLAY AGAIN";
                statusSign.color = ending == 0 ? new Color(1f, 0.58f, 0.28f) :
                    ending == 1 ? new Color(0.35f, 0.62f, 1f) : new Color(0.32f, 0.92f, 0.62f);
            }

            dialogue?.Say(dialogueId);
            WorldAudioDirector.Instance?.PlayRevealCue();
            epilogueShown = true;
        }
    }
}
