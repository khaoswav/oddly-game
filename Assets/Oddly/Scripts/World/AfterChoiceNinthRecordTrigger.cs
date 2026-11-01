using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Opens the ninth archive only after the player has experienced all three
    /// campaign endings and completed the eight hidden rooms and their echoes.
    /// The discovery and alternate ending unlock persist through replay.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class AfterChoiceNinthRecordTrigger : MonoBehaviour
    {
        private const string RecordKey = "oddly.afterchoice.ninthRecordFound";
        private const string AlternateEndingKey = "oddly.afterchoice.alternateEndingUnlocked";

        [SerializeField] private Transform sealedDoor;
        [SerializeField] private Transform[] convergenceRelics = new Transform[3];
        [SerializeField] private TextMesh statusSign;
        [SerializeField, Min(0.1f)] private float openingDuration = 1.6f;

        private bool playerInside;
        private bool opened;
        private Vector3 openDoorPosition;
        private Vector3[] relicBaseScales;
        private Coroutine opening;

        public static bool IsDiscovered => PlayerPrefs.GetInt(RecordKey, 0) == 1;
        public static bool IsAlternateEndingUnlocked =>
            PlayerPrefs.GetInt(AlternateEndingKey, 0) == 1;

        private void Awake()
        {
            if (sealedDoor != null)
                openDoorPosition = sealedDoor.localPosition + Vector3.left * 4.5f;

            relicBaseScales = new Vector3[convergenceRelics.Length];
            for (int i = 0; i < convergenceRelics.Length; i++)
                if (convergenceRelics[i] != null)
                    relicBaseScales[i] = convergenceRelics[i].localScale;

            opened = IsDiscovered;
            if (opened && sealedDoor != null)
                sealedDoor.localPosition = openDoorPosition;
            RefreshRelics();
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
            if (opened)
            {
                AnimateRelics();
                if (!playerInside || !Input.GetKeyDown(KeyCode.E)) return;
                SetStatus(IsTurkish
                    ? "DOKUZUNCU KAYIT AÇIK / YAZILMAMIŞ SON"
                    : "NINTH RECORD OPEN / THE UNWRITTEN ENDING");
                DialogueDirector.Instance?.Say("afterchoice_ninth_repeat");
                return;
            }

            if (!playerInside || !Input.GetKeyDown(KeyCode.E)) return;

            int endingMask = FinalDecisionPuzzle.EndingHistoryMask;
            int roomCount = HiddenChapterArchive.CompletedCount;
            int echoCount = AfterChoiceEchoPillar.DiscoveredCount;
            bool lastEchoClaimed = PlayerPrefs.GetInt("oddly.afterchoice.lastEchoClaimed", 0) == 1;
            bool archiveFound = PlayerPrefs.GetInt("oddly.epilogue.archiveFound", 0) == 1;

            if (endingMask != 0x7 || roomCount < 8 || AfterChoiceEchoPillar.DiscoveredMask != 0xFF ||
                !lastEchoClaimed || !archiveFound)
            {
                SetStatus(IsTurkish
                    ? "KİLİTLİ / FİNALLER " + CountBits(endingMask) + " / 3  •  ODALAR " +
                      roomCount + " / 8  •  YANKILAR " + echoCount + " / 8"
                    : "SEALED / ENDINGS " + CountBits(endingMask) + " / 3  •  ROOMS " +
                      roomCount + " / 8  •  ECHOES " + echoCount + " / 8");
                DialogueDirector.Instance?.Say("afterchoice_ninth_locked");
                return;
            }

            opened = true;
            PlayerPrefs.SetInt(RecordKey, 1);
            PlayerPrefs.SetInt(AlternateEndingKey, 1);
            PlayerPrefs.Save();
            SetStatus(IsTurkish
                ? "DOKUZUNCU KAYIT BULUNDU / GERÇEK SON AÇILDI"
                : "NINTH RECORD FOUND / TRUE ENDING UNLOCKED");
            DialogueDirector.Instance?.Say(GetDiscoveryDialogue());
            WorldAudioDirector.Instance?.PlayRevealCue();
            RefreshRelics();
            if (opening != null) StopCoroutine(opening);
            opening = StartCoroutine(OpenDoor());
        }

        private IEnumerator OpenDoor()
        {
            if (sealedDoor == null) yield break;
            Vector3 start = sealedDoor.localPosition;
            float elapsed = 0f;
            while (elapsed < openingDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / openingDuration));
                sealedDoor.localPosition = Vector3.Lerp(start, openDoorPosition, t);
                yield return null;
            }
            sealedDoor.localPosition = openDoorPosition;
            opening = null;
        }

        private void SetPrompt()
        {
            SetStatus(opened
                ? (IsTurkish ? "DOKUZUNCU KAYIT / E'YE BAS" : "NINTH RECORD / PRESS E")
                : (IsTurkish ? "ÜÇ SESİ DE DUYDUĞUNDA GERİ GEL / E'YE BAS" :
                    "RETURN WHEN YOU HAVE HEARD ALL THREE VOICES / PRESS E"));
        }

        private string GetDiscoveryDialogue()
        {
            switch (Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2))
            {
                case 0: return "afterchoice_ninth_narrator";
                case 1: return "afterchoice_ninth_system";
                default: return "afterchoice_ninth_player";
            }
        }

        private void RefreshRelics()
        {
            Color[] colors =
            {
                new Color(1f, 0.48f, 0.22f),
                new Color(0.24f, 0.52f, 1f),
                new Color(0.28f, 0.9f, 0.62f)
            };

            for (int i = 0; i < convergenceRelics.Length; i++)
            {
                Transform relic = convergenceRelics[i];
                if (relic == null) continue;
                Renderer renderer = relic.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = colors[i % colors.Length];
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", colors[i % colors.Length] *
                        (opened ? 1.65f : 0.32f));
                }
                if (relicBaseScales != null && i < relicBaseScales.Length && relicBaseScales[i] == Vector3.zero)
                    relicBaseScales[i] = relic.localScale;
            }
        }

        private void AnimateRelics()
        {
            for (int i = 0; i < convergenceRelics.Length; i++)
            {
                Transform relic = convergenceRelics[i];
                if (relic == null) continue;
                float phase = Time.unscaledTime * 1.15f + i * 2.1f;
                relic.localScale = relicBaseScales[i] * (1f + Mathf.Sin(phase) * 0.09f);
                relic.Rotate(Vector3.up, (12f + i * 4f) * Time.unscaledDeltaTime, Space.World);
            }
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