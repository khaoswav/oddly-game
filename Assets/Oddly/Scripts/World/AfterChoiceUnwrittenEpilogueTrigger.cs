using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Delivers the final post-credits memory after The Unwritten Ending.
    /// The epilogue acknowledges the player's ending history and replay count.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class AfterChoiceUnwrittenEpilogueTrigger : MonoBehaviour
    {
        private const string SeenKey = "oddly.afterchoice.unwrittenEpilogueSeen";

        [SerializeField] private TextMesh statusSign;

        private bool playerInside;
        private bool revealed;

        public static bool HasBeenRevealed => PlayerPrefs.GetInt(SeenKey, 0) == 1;

        private void Awake()
        {
            revealed = HasBeenRevealed;
        }

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() == null) return;
            playerInside = true;
            SetStatus(revealed
                ? (IsTurkish ? "SON SÖZ / E'YE BAS  •  TEKRAR: R" : "FINAL WORD / PRESS E  •  REPLAY: R")
                : (IsTurkish ? "SON KAYDI DİNLE / E'YE BAS" : "HEAR THE FINAL MEMORY / PRESS E"));
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() != null)
                playerInside = false;
        }

        private void Update()
        {
            if (!playerInside) return;

            if (!AfterChoiceUnwrittenEndingTrigger.HasSeen)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SetStatus(IsTurkish
                        ? "ÖNCE YAZILMAMIŞ SONU TAMAMLA"
                        : "COMPLETE THE UNWRITTEN ENDING FIRST");
                    DialogueDirector.Instance?.Say("afterchoice_final_epilogue_locked");
                }
                return;
            }

            if (!revealed && Input.GetKeyDown(KeyCode.E))
                Reveal();
            else if (revealed && Input.GetKeyDown(KeyCode.R))
                CampaignDirector.BeginReplayPreservingArchive();
            else if (revealed && Input.GetKeyDown(KeyCode.E))
                DialogueDirector.Instance?.Say(GetRepeatDialogue());
        }

        private void Reveal()
        {
            int endingMask = FinalDecisionPuzzle.EndingHistoryMask;
            int replayCount = CampaignDirector.ReplayCount;
            int hiddenCount = HiddenChapterArchive.CompletedCount;
            revealed = true;
            PlayerPrefs.SetInt(SeenKey, 1);
            PlayerPrefs.Save();

            SetStatus(IsTurkish
                ? "SON SÖZ\nFİNALLER: " + CountBits(endingMask) + " / 3\nGİZLİ ODALAR: " +
                  hiddenCount + " / 8\nTEKRAR: " + replayCount + "\nYENİDEN OYNAMAK İÇİN R"
                : "THE FINAL WORD\nENDINGS: " + CountBits(endingMask) + " / 3\nHIDDEN ROOMS: " +
                  hiddenCount + " / 8\nREPLAYS: " + replayCount + "\nPRESS R TO BEGIN AGAIN");

            DialogueDirector.Instance?.Say(GetFinalDialogue());
            WorldAudioDirector.Instance?.PlayRevealCue();
        }

        private string GetFinalDialogue()
        {
            if (FinalDecisionPuzzle.EndingHistoryMask == 0x7)
                return "afterchoice_final_epilogue_converged";

            switch (Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2))
            {
                case 0: return "afterchoice_final_epilogue_narrator";
                case 1: return "afterchoice_final_epilogue_system";
                default: return "afterchoice_final_epilogue_player";
            }
        }

        private string GetRepeatDialogue()
        {
            return FinalDecisionPuzzle.EndingHistoryMask == 0x7
                ? "afterchoice_final_epilogue_repeat_converged"
                : "afterchoice_final_epilogue_repeat";
        }

        private void SetStatus(string value)
        {
            if (statusSign != null) statusSign.text = value;
        }

        private static int CountBits(int mask)
        {
            int count = 0;
            for (int i = 0; i < 3; i++)
                if ((mask & (1 << i)) != 0) count++;
            return count;
        }

        private static bool IsTurkish => PlayerPrefs.GetInt("oddly.language", 1) == 0;
    }
}