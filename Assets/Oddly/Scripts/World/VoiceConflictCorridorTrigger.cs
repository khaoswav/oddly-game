using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Shows the competing narrator and SYSTEM-02 signals bleeding into
    /// the control-room approach. The effect is visual and never blocks movement.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class VoiceConflictCorridorTrigger : MonoBehaviour
    {
        [SerializeField] private Transform[] signalNodes;
        [SerializeField] private TextMesh sign;
        [SerializeField] private Light eventLight;
        [SerializeField, Min(0.5f)] private float duration = 3f;

        private Vector3[] baseScales;
        private Quaternion[] baseRotations;
        private Color baseLightColor;
        private Color baseSignColor;
        private float baseLightIntensity;
        private bool played;

        private void Start()
        {
            baseScales = new Vector3[signalNodes.Length];
            baseRotations = new Quaternion[signalNodes.Length];
            for (int i = 0; i < signalNodes.Length; i++)
            {
                if (signalNodes[i] == null) continue;
                baseScales[i] = signalNodes[i].localScale;
                baseRotations[i] = signalNodes[i].localRotation;
            }

            if (sign != null) baseSignColor = sign.color;
            if (eventLight != null)
            {
                baseLightColor = eventLight.color;
                baseLightIntensity = eventLight.intensity;
            }
        }

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (played || other.GetComponentInParent<CharacterController>() == null) return;
            played = true;
            StartCoroutine(PlaySignalConflict());
        }

        private IEnumerator PlaySignalConflict()
        {
            bool turkish = DialogueDirector.Instance != null && DialogueDirector.Instance.IsTurkish;
            if (sign != null)
            {
                sign.text = turkish ? "İKİ SES / TEK KONTROL" : "TWO VOICES / ONE CONTROL";
                sign.color = new Color(1f, 0.42f, 0.3f);
            }
            WorldAudioDirector.Instance?.PlaySurpriseCue();

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float pulse = 0.5f + 0.5f * Mathf.Sin(elapsed * 8f);
                if (eventLight != null)
                {
                    eventLight.color = Color.Lerp(new Color(1f, 0.28f, 0.2f),
                        new Color(0.35f, 0.48f, 1f), pulse);
                    eventLight.intensity = baseLightIntensity * (1.1f + pulse * 1.2f);
                }

                for (int i = 0; i < signalNodes.Length; i++)
                {
                    Transform node = signalNodes[i];
                    if (node == null) continue;
                    float phase = elapsed * 7f + i * 1.7f;
                    float scale = 0.72f + 0.55f * (0.5f + 0.5f * Mathf.Sin(phase));
                    node.localScale = baseScales[i] * scale;
                    node.localRotation = baseRotations[i] *
                        Quaternion.Euler(phase * 12f, phase * (i % 2 == 0 ? 28f : -28f), 0f);
                }
                yield return null;
            }

            if (sign != null)
            {
                sign.text = turkish ? "SİSTEM-02: KONTROL ODASINA GİRİN" :
                    "SYSTEM-02: ENTER THE CONTROL ROOM";
                sign.color = new Color(0.35f, 0.85f, 0.68f);
            }
            if (eventLight != null)
            {
                eventLight.color = new Color(0.35f, 0.85f, 0.68f);
                eventLight.intensity = baseLightIntensity;
            }
            for (int i = 0; i < signalNodes.Length; i++)
            {
                if (signalNodes[i] == null) continue;
                signalNodes[i].localScale = baseScales[i];
                signalNodes[i].localRotation = baseRotations[i];
            }
            WorldAudioDirector.Instance?.PlayRevealCue();
        }
    }
}