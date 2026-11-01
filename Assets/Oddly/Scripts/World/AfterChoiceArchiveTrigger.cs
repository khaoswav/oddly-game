using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Opens the post-choice archive only when a previous ending and enough
    /// hidden-room discoveries exist. The saved archive deliberately survives replay.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class AfterChoiceArchiveTrigger : MonoBehaviour
    {
        [SerializeField] private Transform archiveDoor;
        [SerializeField] private TextMesh statusSign;
        [SerializeField, Min(0)] private int requiredDiscoveries = 3;
        [SerializeField, Min(0.1f)] private float openingDuration = 1.4f;

        private bool playerInside;
        private bool opened;
        private Vector3 openPosition;
        private Coroutine opening;

        private void Awake()
        {
            if (archiveDoor != null)
                openPosition = archiveDoor.localPosition + Vector3.up * 4.5f;
            opened = PlayerPrefs.GetInt("oddly.epilogue.archiveFound", 0) == 1;
            if (opened && archiveDoor != null)
                archiveDoor.localPosition = openPosition;
        }

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() == null) return;
            playerInside = true;
            SetStatus(opened
                ? (IsTurkish ? "ARŞİV AÇIK / E'YE BAS" : "ARCHIVE OPEN / PRESS E")
                : (IsTurkish ? "BURASI NEREYE GİDİYOR? / E'YE BAS" : "WHERE DOES THIS GO? / PRESS E"));
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() != null)
                playerInside = false;
        }

        private void Update()
        {
            if (!playerInside || !Input.GetKeyDown(KeyCode.E)) return;
            if (opened)
            {
                SetStatus(IsTurkish ? "ARŞİV AÇIK / GEÇMİŞİNİ GÖR" : "ARCHIVE OPEN / ENTER YOUR HISTORY");
                return;
            }

            if (!PlayerPrefs.HasKey("oddly.ending") ||
                HiddenChapterArchive.CompletedCount < requiredDiscoveries)
            {
                SetStatus(IsTurkish
                    ? "KİLİTLİ / BİR FİNAL VE EN AZ " + requiredDiscoveries + " GİZLİ ODA GEREKLİ"
                    : "SEALED / A REMEMBERED ENDING AND " + requiredDiscoveries + " HIDDEN ROOMS REQUIRED");
                DialogueDirector.Instance?.Say("afterchoice_archive_locked");
                return;
            }

            opened = true;
            PlayerPrefs.SetInt("oddly.epilogue.archiveFound", 1);
            PlayerPrefs.Save();
            SetStatus(IsTurkish ? "ARŞİV HATIRLADI / KAPI AÇILIYOR" : "THE ARCHIVE REMEMBERS / OPENING");
            switch (Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2))
            {
                case 0: DialogueDirector.Instance?.Say("afterchoice_archive_open_narrator"); break;
                case 1: DialogueDirector.Instance?.Say("afterchoice_archive_open_system"); break;
                default: DialogueDirector.Instance?.Say("afterchoice_archive_open_player"); break;
            }
            WorldAudioDirector.Instance?.PlayRevealCue();
            if (opening != null) StopCoroutine(opening);
            opening = StartCoroutine(OpenDoor());
        }

        private IEnumerator OpenDoor()
        {
            if (archiveDoor == null) yield break;
            Vector3 start = archiveDoor.localPosition;
            Vector3 target = openPosition;
            float elapsed = 0f;
            while (elapsed < openingDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / openingDuration));
                archiveDoor.localPosition = Vector3.Lerp(start, target, t);
                yield return null;
            }
            archiveDoor.localPosition = target;
            opening = null;
        }

        private void SetStatus(string value)
        {
            if (statusSign != null) statusSign.text = value;
        }

        private static bool IsTurkish => PlayerPrefs.GetInt("oddly.language", 1) == 0;
    }
}