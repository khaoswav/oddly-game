using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Lets the player hear one ending-aware memory for a completed hidden chapter.
    /// Echo discoveries are persistent and intentionally survive a normal replay.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class AfterChoiceEchoPillar : MonoBehaviour
    {
        private const string EchoMaskKey = "oddly.afterchoice.echoMask";

        [SerializeField, Range(1, 8)] private int chapterNumber = 1;
        [SerializeField] private Transform pillarVisual;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private float interactionCooldown = 1.25f;

        private bool playerInside;
        private float nextInteractionTime;

        public static int DiscoveredMask => PlayerPrefs.GetInt(EchoMaskKey, 0) & 0xFF;
        public static int DiscoveredCount
        {
            get
            {
                int mask = DiscoveredMask;
                int count = 0;
                for (int i = 0; i < 8; i++)
                    if ((mask & (1 << i)) != 0) count++;
                return count;
            }
        }

        private void Start()
        {
            RefreshVisual();
        }

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() == null) return;
            playerInside = true;
            SetPrompt();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() != null)
                playerInside = false;
        }

        private void Update()
        {
            if (!playerInside || !Input.GetKeyDown(KeyCode.E) ||
                Time.unscaledTime < nextInteractionTime) return;

            nextInteractionTime = Time.unscaledTime + interactionCooldown;
            int bit = 1 << (chapterNumber - 1);
            if (!HiddenChapterArchive.IsCompleted(chapterNumber))
            {
                SetStatus(IsTurkish
                    ? "YANKI YOK / GİZLİ ODA " + chapterNumber.ToString("00") + " HENÜZ TAMAMLANMADI"
                    : "NO ECHO / HIDDEN ROOM " + chapterNumber.ToString("00") + " IS NOT COMPLETE");
                DialogueDirector.Instance?.Say("afterchoice_echo_locked");
                return;
            }

            int mask = DiscoveredMask;
            if ((mask & bit) != 0)
            {
                SetStatus(IsTurkish
                    ? "YANKI " + chapterNumber.ToString("00") + " / HATIRLANDI"
                    : "ECHO " + chapterNumber.ToString("00") + " / REMEMBERED");
                DialogueDirector.Instance?.Say("afterchoice_echo_repeat");
                return;
            }

            mask |= bit;
            PlayerPrefs.SetInt(EchoMaskKey, mask);
            PlayerPrefs.Save();
            SetStatus(IsTurkish
                ? "YANKI " + chapterNumber.ToString("00") + " KAYDEDİLDİ / " + DiscoveredCount + " / 8"
                : "ECHO " + chapterNumber.ToString("00") + " RECORDED / " + DiscoveredCount + " / 8");
            DialogueDirector.Instance?.Say(GetDialogueId());
            WorldAudioDirector.Instance?.PlayRevealCue();
            RefreshVisual();
        }

        private string GetDialogueId()
        {
            string ending;
            switch (Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2))
            {
                case 0: ending = "narrator"; break;
                case 1: ending = "system"; break;
                default: ending = "player"; break;
            }
            return "afterchoice_echo_" + chapterNumber.ToString("00") + "_" + ending;
        }

        private void SetPrompt()
        {
            if ((DiscoveredMask & (1 << (chapterNumber - 1))) != 0)
                SetStatus(IsTurkish
                    ? "YANKI " + chapterNumber.ToString("00") + " / E'YE BAS"
                    : "ECHO " + chapterNumber.ToString("00") + " / PRESS E");
            else
                SetStatus(IsTurkish
                    ? "HAFIZAYI DİNLE / E'YE BAS"
                    : "LISTEN TO THE MEMORY / PRESS E");
        }

        private void RefreshVisual()
        {
            if (pillarVisual == null) return;
            Renderer target = pillarVisual.GetComponent<Renderer>();
            if (target == null) return;

            bool discovered = (DiscoveredMask & (1 << (chapterNumber - 1))) != 0;
            if (!HiddenChapterArchive.IsCompleted(chapterNumber)) return;

            Color endingColor = GetEndingColor();
            target.material.color = discovered
                ? Color.Lerp(endingColor, Color.white, 0.58f)
                : Color.Lerp(endingColor, Color.white, 0.24f);
            target.material.EnableKeyword("_EMISSION");
            target.material.SetColor("_EmissionColor",
                endingColor * (discovered ? 1.15f : 0.55f));
        }

        private void SetStatus(string value)
        {
            if (statusSign != null) statusSign.text = value;
        }

        private static bool IsTurkish => PlayerPrefs.GetInt("oddly.language", 1) == 0;

        private static Color GetEndingColor()
        {
            switch (Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2))
            {
                case 0: return new Color(1f, 0.48f, 0.22f);
                case 1: return new Color(0.24f, 0.52f, 1f);
                default: return new Color(0.28f, 0.9f, 0.62f);
            }
        }
    }
}