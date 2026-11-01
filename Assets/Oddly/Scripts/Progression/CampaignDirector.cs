using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Oddly
{
    /// <summary>
    /// Persistent campaign progress and restrained, full-screen chapter cards.
    /// This owns presentation and progress metadata; it never moves the player.
    /// </summary>
    public sealed class CampaignDirector : MonoBehaviour
    {
        public static CampaignDirector Instance { get; private set; }

        [SerializeField] private CanvasGroup cardGroup;
        [SerializeField] private Image cardBackground;
        [SerializeField] private Text chapterLabel;
        [SerializeField] private Text titleLabel;
        [SerializeField] private Text subtitleLabel;
        [SerializeField, Min(0f)] private float fadeDuration = 0.55f;
        [SerializeField, Min(0f)] private float holdDuration = 2.15f;

        private Coroutine activeCard;
        private bool turkish => PlayerPrefs.GetInt("oddly.language", 1) == 0;

        private void Start()
        {
            SetCardVisible(false);
            if (!HasCampaignSave)
                EnterChapter(1, "THE TEST CENTER", "TEST MERKEZİ", false);
        }

        public static int LastMainChapter => PlayerPrefs.GetInt("oddly.campaign.lastMainChapter", 1);
        public static int FurthestMainChapter => PlayerPrefs.GetInt("oddly.campaign.furthestMainChapter", 1);
        public static bool HasCampaignSave => PlayerPrefs.GetInt("oddly.campaign.started", 0) == 1;
        public static bool IsCampaignComplete => PlayerPrefs.GetInt("oddly.campaign.complete", 0) == 1;
        public static int ReplayCount => PlayerPrefs.GetInt("oddly.campaign.replayCount", 0);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            SetCardVisible(false);
        }

        public void EnterChapter(int chapterIndex, string englishTitle, string turkishTitle, bool hiddenChapter)
        {
            if (chapterIndex < 1) return;

            PlayerPrefs.SetInt("oddly.campaign.started", 1);
            if (hiddenChapter)
            {
                int mask = PlayerPrefs.GetInt("oddly.campaign.hiddenVisited", 0);
                PlayerPrefs.SetInt("oddly.campaign.hiddenVisited", mask | (1 << (chapterIndex - 1)));
            }
            else
            {
                PlayerPrefs.SetInt("oddly.campaign.lastMainChapter", chapterIndex);
                PlayerPrefs.SetInt("oddly.campaign.furthestMainChapter",
                    Mathf.Max(FurthestMainChapter, chapterIndex));
            }
            PlayerPrefs.Save();

            string title = ExtractCardTitle(turkish ? turkishTitle : englishTitle);
            string chapter = hiddenChapter
                ? (turkish ? $"GİZLİ BÖLÜM {chapterIndex:00}" : $"HIDDEN CHAPTER {chapterIndex:00}")
                : (turkish ? $"BÖLÜM {chapterIndex:00}" : $"CHAPTER {chapterIndex:00}");
            string subtitle = hiddenChapter
                ? (turkish ? "ARŞİV DIŞI BİR YER" : "SOMEWHERE OUTSIDE THE ARCHIVE")
                : (turkish ? "TEST DEVAM EDİYOR. NE YAZIK Kİ." : "THE TEST CONTINUES. UNFORTUNATELY.");

            if (cardGroup == null || titleLabel == null || chapterLabel == null || subtitleLabel == null) return;
            if (activeCard != null) StopCoroutine(activeCard);
            activeCard = StartCoroutine(PlayCard(chapter, title, subtitle));
        }

        public void CompleteCampaign(int endingIndex)
        {
            PlayerPrefs.SetInt("oddly.campaign.complete", 1);
            PlayerPrefs.SetInt("oddly.campaign.finalEnding", Mathf.Clamp(endingIndex, 0, 2));
            PlayerPrefs.SetInt("oddly.campaign.lastMainChapter", 14);
            PlayerPrefs.SetInt("oddly.campaign.furthestMainChapter", 14);
            PlayerPrefs.Save();
            if (cardGroup != null && titleLabel != null && chapterLabel != null && subtitleLabel != null)
            {
                string[] endingsEn = { "THE STORYTELLER", "THE SYSTEM", "THE PLAYER" };
                string[] endingsTr = { "HİKÂYE ANLATICISI", "SİSTEM", "OYUNCU" };
                string[] subtitlesEn = { "THE STORY ENDS WHERE IT BEGAN.", "ORDER IS RESTORED. OR IS IT?", "NO ONE ELSE GETS THE FINAL WORD." };
                string[] subtitlesTr = { "HİKÂYE BAŞLADIĞI YERDE BİTİYOR.", "DÜZEN GERİ GELDİ. PEKİ YA?", "SON SÖZÜ ARTIK KİMSE ELİNDEN ALAMAZ." };
                if (activeCard != null) StopCoroutine(activeCard);
                activeCard = StartCoroutine(PlayCard(
                    turkish ? "HİKÂYE TAMAMLANDI" : "CAMPAIGN COMPLETE",
                    turkish ? endingsTr[Mathf.Clamp(endingIndex, 0, 2)] : endingsEn[Mathf.Clamp(endingIndex, 0, 2)],
                    turkish ? subtitlesTr[Mathf.Clamp(endingIndex, 0, 2)] : subtitlesEn[Mathf.Clamp(endingIndex, 0, 2)]));
            }
        }

        /// <summary>
        /// Starts a fresh story run while preserving the chosen ending and
        /// completed hidden-room archive for callbacks and the final memory room.
        /// </summary>
        public static void BeginReplayPreservingArchive()
        {
            PlayerPrefs.SetInt("oddly.campaign.replayCount", ReplayCount + 1);
            PlayerPrefs.DeleteKey("oddly.campaign.started");
            PlayerPrefs.DeleteKey("oddly.campaign.lastMainChapter");
            PlayerPrefs.DeleteKey("oddly.campaign.furthestMainChapter");
            PlayerPrefs.DeleteKey("oddly.campaign.hiddenVisited");
            PlayerPrefs.DeleteKey("oddly.campaign.complete");
            PlayerPrefs.DeleteKey("oddly.campaign.finalEnding");
            PlayerPrefs.DeleteKey("oddly.campaign.checkpoint.saved");
            for (int i = 0; i < 3; i++)
            {
                PlayerPrefs.DeleteKey("oddly.campaign.checkpoint.position." + i);
                PlayerPrefs.DeleteKey("oddly.campaign.checkpoint.rotation." + i);
            }

            // Keep the ending, hidden completions, discovered echoes and reward for replay.
            // The epilogue archive and Last Echo reward are intentionally persistent.
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void ClearCampaignProgress()
        {
            PlayerPrefs.DeleteKey("oddly.campaign.started");
            PlayerPrefs.DeleteKey("oddly.campaign.lastMainChapter");
            PlayerPrefs.DeleteKey("oddly.campaign.furthestMainChapter");
            PlayerPrefs.DeleteKey("oddly.campaign.hiddenVisited");
            PlayerPrefs.DeleteKey("oddly.campaign.complete");
            PlayerPrefs.DeleteKey("oddly.campaign.finalEnding");
            PlayerPrefs.DeleteKey("oddly.campaign.checkpoint.saved");
            for (int i = 0; i < 3; i++)
            {
                PlayerPrefs.DeleteKey("oddly.campaign.checkpoint.position." + i);
                PlayerPrefs.DeleteKey("oddly.campaign.checkpoint.rotation." + i);
            }
            for (int i = 1; i <= 8; i++)
                PlayerPrefs.DeleteKey("oddly.hidden.completed." + i);
            PlayerPrefs.DeleteKey("oddly.epilogue.archiveFound");
            PlayerPrefs.DeleteKey("oddly.afterchoice.echoMask");
            PlayerPrefs.DeleteKey("oddly.afterchoice.lastEchoClaimed");
            PlayerPrefs.DeleteKey("oddly.afterchoice.ninthRecordFound");
            PlayerPrefs.DeleteKey("oddly.afterchoice.alternateEndingUnlocked");
            PlayerPrefs.DeleteKey("oddly.afterchoice.alternateEndingSeen");
            PlayerPrefs.DeleteKey("oddly.campaign.endingHistoryMask");
            PlayerPrefs.DeleteKey("oddly.campaign.replayCount");
            PlayerPrefs.DeleteKey("oddly.afterchoice.unwrittenEpilogueSeen");
            PlayerPrefs.DeleteKey("oddly.ending");
            PlayerPrefs.DeleteKey("oddly.ending.name");
            PlayerPrefs.Save();
        }

        private static string ExtractCardTitle(string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            int separator = source.IndexOf('/');
            return separator >= 0 ? source.Substring(separator + 1).Trim() : source.Trim();
        }

        private IEnumerator PlayCard(string chapter, string title, string subtitle)
        {
            chapterLabel.text = chapter;
            titleLabel.text = title;
            subtitleLabel.text = subtitle;
            if (cardBackground != null)
                cardBackground.color = new Color(0.035f, 0.045f, 0.075f, 1f);
            SetCardVisible(true);
            yield return Fade(0f, 1f, fadeDuration);
            float elapsed = 0f;
            while (elapsed < holdDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            yield return Fade(1f, 0f, fadeDuration);
            SetCardVisible(false);
            activeCard = null;
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                cardGroup.alpha = to;
                yield break;
            }
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                cardGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
            cardGroup.alpha = to;
        }

        private void SetCardVisible(bool visible)
        {
            if (cardGroup == null) return;
            cardGroup.alpha = visible ? 1f : 0f;
            cardGroup.interactable = false;
            cardGroup.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
