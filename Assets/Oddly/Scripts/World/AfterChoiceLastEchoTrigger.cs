using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Reveals the final archive reward after all eight hidden rooms and all
    /// eight ending-aware memory echoes have been completed.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class AfterChoiceLastEchoTrigger : MonoBehaviour
    {
        private const string ClaimedKey = "oddly.afterchoice.lastEchoClaimed";

        [SerializeField] private Transform rewardCore;
        [SerializeField] private TextMesh statusSign;
        [SerializeField, Min(0.1f)] private float idlePulse = 0.08f;

        private bool playerInside;
        private Vector3 coreStartPosition;
        private Vector3 coreStartScale;
        private bool claimed;

        private void Awake()
        {
            if (rewardCore != null)
            {
                coreStartPosition = rewardCore.localPosition;
                coreStartScale = rewardCore.localScale;
            }

            claimed = PlayerPrefs.GetInt(ClaimedKey, 0) == 1;
            RefreshRewardVisual();
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
            if (rewardCore != null && claimed)
            {
                float pulse = 1f + Mathf.Sin(Time.unscaledTime * 1.8f) * idlePulse;
                rewardCore.localPosition = coreStartPosition + Vector3.up * (0.12f * pulse);
                rewardCore.localScale = coreStartScale * pulse;
            }

            if (!playerInside || !Input.GetKeyDown(KeyCode.E)) return;

            if (claimed)
            {
                SetStatus(IsTurkish
                    ? "SON YANKI / ARŞİV TAMAMLANDI"
                    : "THE LAST ECHO / ARCHIVE COMPLETE");
                return;
            }

            if (HiddenChapterArchive.CompletedCount < 8 ||
                AfterChoiceEchoPillar.DiscoveredMask != 0xFF)
            {
                SetStatus(IsTurkish
                    ? "KİLİTLİ / GİZLİ ODALAR " + HiddenChapterArchive.CompletedCount +
                      " / 8  •  YANKILAR " + AfterChoiceEchoPillar.DiscoveredCount + " / 8"
                    : "SEALED / HIDDEN ROOMS " + HiddenChapterArchive.CompletedCount +
                      " / 8  •  ECHOES " + AfterChoiceEchoPillar.DiscoveredCount + " / 8");
                DialogueDirector.Instance?.Say("afterchoice_reward_locked");
                return;
            }

            claimed = true;
            PlayerPrefs.SetInt(ClaimedKey, 1);
            PlayerPrefs.Save();
            SetStatus(IsTurkish
                ? "SON YANKI AÇILDI / ARŞİV TAMAMLANDI"
                : "THE LAST ECHO AWAKENS / ARCHIVE COMPLETE");
            DialogueDirector.Instance?.Say(GetRewardDialogueId());
            WorldAudioDirector.Instance?.PlayRevealCue();
            RefreshRewardVisual();
        }

        private void SetPrompt()
        {
            if (claimed)
                SetStatus(IsTurkish
                    ? "SON YANKI / E'YE BAS"
                    : "THE LAST ECHO / PRESS E");
            else
                SetStatus(IsTurkish
                    ? "SEKİZ HAFIZAYI BİRLEŞTİR / E'YE BAS"
                    : "BRING THE EIGHT MEMORIES TOGETHER / PRESS E");
        }

        private string GetRewardDialogueId()
        {
            switch (Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2))
            {
                case 0: return "afterchoice_reward_narrator";
                case 1: return "afterchoice_reward_system";
                default: return "afterchoice_reward_player";
            }
        }

        private void RefreshRewardVisual()
        {
            if (rewardCore == null) return;
            Renderer renderer = rewardCore.GetComponent<Renderer>();
            if (renderer == null) return;

            Color rewardColor = GetEndingColor();
            renderer.material.color = claimed ? Color.white : rewardColor;
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", rewardColor * (claimed ? 2.2f : 0.7f));
            rewardCore.localScale = claimed ? coreStartScale * 1.25f : coreStartScale;
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