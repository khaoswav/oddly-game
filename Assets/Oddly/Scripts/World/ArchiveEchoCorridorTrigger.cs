using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Plays a one-shot visual memory leak in the corridor leading to Chapter 12.
    /// Corrupted snapshots jitter and sink; the stable snapshot is revealed.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class ArchiveEchoCorridorTrigger : MonoBehaviour
    {
        [SerializeField] private Transform[] snapshots;
        [SerializeField] private TextMesh sign;
        [SerializeField] private Light eventLight;
        [SerializeField, Min(0.5f)] private float duration = 2.4f;

        private Vector3[] startPositions;
        private Quaternion[] startRotations;
        private Color originalSignColor;
        private Color originalLightColor;
        private float originalLightIntensity;
        private bool played;

        private void Start()
        {
            startPositions = new Vector3[snapshots.Length];
            startRotations = new Quaternion[snapshots.Length];
            for (int i = 0; i < snapshots.Length; i++)
            {
                if (snapshots[i] == null) continue;
                startPositions[i] = snapshots[i].localPosition;
                startRotations[i] = snapshots[i].localRotation;
            }

            if (sign != null) originalSignColor = sign.color;
            if (eventLight != null)
            {
                originalLightColor = eventLight.color;
                originalLightIntensity = eventLight.intensity;
            }
        }

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (played || other.GetComponentInParent<CharacterController>() == null)
                return;

            played = true;
            StartCoroutine(ReplayArchiveEcho());
        }

        private IEnumerator ReplayArchiveEcho()
        {
            bool turkish = DialogueDirector.Instance != null && DialogueDirector.Instance.IsTurkish;
            if (sign != null)
            {
                sign.text = turkish ? "KAYIT NOKTASI BULUNAMADI" : "RESTORE POINT NOT FOUND";
                sign.color = new Color(0.95f, 0.35f, 0.28f);
            }
            if (eventLight != null)
            {
                eventLight.color = new Color(1f, 0.22f, 0.18f);
                eventLight.intensity = originalLightIntensity * 1.7f;
            }
            WorldAudioDirector.Instance?.PlaySurpriseCue();

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                for (int i = 0; i < snapshots.Length; i++)
                {
                    Transform snapshot = snapshots[i];
                    if (snapshot == null) continue;

                    if (i == 1)
                    {
                        snapshot.localPosition = startPositions[i] +
                            Vector3.up * (Mathf.Sin(elapsed * 3f) * 0.16f);
                        snapshot.localRotation = Quaternion.Slerp(startRotations[i],
                            Quaternion.Euler(0f, elapsed * 55f, 0f), progress);
                    }
                    else
                    {
                        float direction = i == 0 ? -1f : 1f;
                        snapshot.localPosition = startPositions[i] +
                            new Vector3(direction * Mathf.Sin(elapsed * 19f) * 0.18f,
                                -0.45f * progress + Mathf.Sin(elapsed * 23f) * 0.08f, 0f);
                        snapshot.localRotation = startRotations[i] *
                            Quaternion.Euler(elapsed * 110f, elapsed * direction * 140f, 0f);
                    }
                }
                yield return null;
            }

            if (sign != null)
            {
                sign.text = turkish ? "SON SAĞLAM KAYIT: YUVA 02" : "LAST GOOD SAVE: SLOT 02";
                sign.color = new Color(0.3f, 0.82f, 0.58f);
            }
            if (eventLight != null)
            {
                eventLight.color = new Color(0.3f, 0.82f, 0.58f);
                eventLight.intensity = originalLightIntensity * 1.25f;
            }
            WorldAudioDirector.Instance?.PlayRevealCue();
        }
    }
}