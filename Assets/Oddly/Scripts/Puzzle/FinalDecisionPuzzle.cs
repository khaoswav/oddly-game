using UnityEngine;

namespace Oddly
{
    public sealed class FinalDecisionPuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh decisionSign;
        [SerializeField] private Light endingLight;
        [SerializeField] private Transform[] endingRelics = new Transform[3];

        private bool decided;

        private void Start()
        {
            if (decisionSign != null)
                decisionSign.text = dialogue != null && dialogue.IsTurkish
                    ? "SON KARAR\nBİRİNİ SEÇ: ANLATICI / SYSTEM / HİÇBİRİ"
                    : "FINAL DECISION\nCHOOSE: NARRATOR / SYSTEM / NEITHER";
        }

        public void ChooseEnding(int endingIndex)
        {
            if (decided || endingIndex < 0 || endingIndex > 2) return;
            decided = true;

            string endingName;
            Color endingColor;
            string dialogueId;

            switch (endingIndex)
            {
                case 0:
                    endingName = "THE STORYTELLER";
                    endingColor = new Color(1f, 0.58f, 0.28f);
                    dialogueId = "ending_narrator";
                    break;
                case 1:
                    endingName = "THE SYSTEM";
                    endingColor = new Color(0.28f, 0.62f, 1f);
                    dialogueId = "ending_system";
                    break;
                default:
                    endingName = "THE PLAYER";
                    endingColor = new Color(0.32f, 0.92f, 0.62f);
                    dialogueId = "ending_neither";
                    break;
            }

            PlayerPrefs.SetInt("oddly.ending", endingIndex);
            PlayerPrefs.SetString("oddly.ending.name", endingName);
            PlayerPrefs.Save();
            CampaignDirector.Instance?.CompleteCampaign(endingIndex);

            if (endingLight != null)
            {
                endingLight.color = endingColor;
                endingLight.intensity = 2.1f;
            }

            for (int i = 0; i < endingRelics.Length; i++)
            {
                if (endingRelics[i] == null) continue;
                endingRelics[i].localScale = i == endingIndex
                    ? Vector3.one * 1.45f : Vector3.one * 0.55f;
                endingRelics[i].localRotation = Quaternion.Euler(0f, i == endingIndex ? 0f : 35f, 0f);
            }

            if (decisionSign != null)
            {
                bool turkish = dialogue != null && dialogue.IsTurkish;
                string[] tr = { "SON: HİKÂYE ANLATICININ", "SON: SİSTEMİN KONTROLÜ", "SON: OYUNCUNUN ÖZGÜRLÜĞÜ" };
                string[] en = { "ENDING: THE STORYTELLER", "ENDING: SYSTEM CONTROL", "ENDING: PLAYER FREEDOM" };
                decisionSign.text = turkish ? tr[endingIndex] : en[endingIndex];
                decisionSign.color = endingColor;
            }

            dialogue?.Say(dialogueId);
            exitDoor?.Unlock();
        }
    }
}
