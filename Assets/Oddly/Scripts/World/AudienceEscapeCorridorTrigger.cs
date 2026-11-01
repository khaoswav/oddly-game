using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// The Chapter 9 audience makes an awkwardly synchronized exit while the
    /// player crosses toward Chapter 10. This is a one-shot visual gag and
    /// never blocks movement or puzzle progression.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class AudienceEscapeCorridorTrigger : MonoBehaviour
    {
        [SerializeField] private Transform[] props;
        [SerializeField] private TextMesh sign;
        [SerializeField] private Light eventLight;
        [SerializeField, Min(0.2f)] private float escapeDuration = 1.35f;
        [SerializeField, Min(0.1f)] private float escapeDistance = 2.1f;

        private Vector3[] startPositions;
        private Quaternion[] startRotations;
        private Color originalSignColor;
        private Color originalLightColor;
        private float originalLightIntensity;
        private bool played;

        private void Start()
        {
            startPositions = new Vector3[props.Length];
            startRotations = new Quaternion[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] == null) continue;
                startPositions[i] = props[i].localPosition;
                startRotations[i] = props[i].localRotation;
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
            StartCoroutine(PlayEscape());
        }

        private IEnumerator PlayEscape()
        {
            bool turkish = DialogueDirector.Instance != null && DialogueDirector.Instance.IsTurkish;
            if (sign != null)
            {
                sign.text = turkish ? "GÖSTERİ BİTTİ. LÜTFEN DAĞILIN." : "THE SHOW IS OVER. PLEASE DISPERSE.";
                sign.color = new Color(0.88f, 0.25f, 0.39f);
            }

            if (eventLight != null)
            {
                eventLight.color = new Color(1f, 0.42f, 0.2f);
                eventLight.intensity = originalLightIntensity * 1.7f;
            }

            WorldAudioDirector.Instance?.PlaySurpriseCue();

            Vector3[] fromPositions = new Vector3[props.Length];
            Vector3[] toPositions = new Vector3[props.Length];
            Quaternion[] fromRotations = new Quaternion[props.Length];
            Quaternion[] toRotations = new Quaternion[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] == null) continue;
                fromPositions[i] = props[i].localPosition;
                float direction = i == 1 ? 1f : (i == 0 ? -1f : 1f);
                toPositions[i] = startPositions[i] +
                    new Vector3(direction * escapeDistance, 0.25f + (i % 2) * 0.2f, 0f);
                toRotations[i] = startRotations[i] *
                    Quaternion.Euler(0f, (i - 1) * 135f, direction * 24f);
            }

            float elapsed = 0f;
            while (elapsed < escapeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / escapeDuration));
                for (int i = 0; i < props.Length; i++)
                {
                    if (props[i] == null) continue;
                    props[i].localPosition = Vector3.LerpUnclamped(fromPositions[i], toPositions[i], t);
                    props[i].localRotation = Quaternion.SlerpUnclamped(fromRotations[i], toRotations[i], t);
                }

                if (eventLight != null)
                    eventLight.intensity = Mathf.Lerp(originalLightIntensity * 1.7f,
                        originalLightIntensity * 0.65f, t);
                yield return null;
            }

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] == null) continue;
                props[i].localPosition = toPositions[i];
                props[i].localRotation = toRotations[i];
            }

            if (sign != null)
            {
                sign.text = turkish ? "ALKIŞLAR İÇİN TEŞEKKÜRLER. BİZ KAÇTIK." :
                    "THANKS FOR THE APPLAUSE. WE'RE LEAVING.";
                sign.color = originalSignColor;
            }

            if (eventLight != null)
            {
                eventLight.color = new Color(0.35f, 0.72f, 1f);
                eventLight.intensity = originalLightIntensity;
            }
            WorldAudioDirector.Instance?.PlayRevealCue();
        }
    }
}