using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A short, optional-feeling corridor beat between the loop chamber and
    /// theatre: props perform a tiny ovation before the sign asks the player
    /// to continue. The event never blocks the route.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class EncoreCorridorTrigger : MonoBehaviour
    {
        [SerializeField] private Transform[] props;
        [SerializeField] private TextMesh sign;
        [SerializeField] private Light eventLight;
        [SerializeField, Min(0.1f)] private float animationDuration = 1.15f;
        [SerializeField, Min(0.1f)] private float propLift = 0.72f;

        private Vector3[] startPositions;
        private Quaternion[] startRotations;
        private Color originalLightColor;
        private float originalLightIntensity;
        private Color originalSignColor;
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
            StartCoroutine(PlayEncore());
        }

        private IEnumerator PlayEncore()
        {
            bool turkish = DialogueDirector.Instance != null && DialogueDirector.Instance.IsTurkish;
            if (sign != null)
            {
                sign.text = turkish ? "TEŞEKKÜRLER! BİR TUR DAHA?" : "THANK YOU! ONE MORE ROUND?";
                sign.color = new Color(0.88f, 0.25f, 0.39f);
            }

            if (eventLight != null)
            {
                eventLight.color = new Color(1f, 0.35f, 0.58f);
                eventLight.intensity = originalLightIntensity * 1.8f;
            }

            WorldAudioDirector.Instance?.PlaySurpriseCue();
            yield return AnimateProps(true);

            if (sign != null)
                sign.text = turkish ? "ALKIŞLARINIZ İÇİN SAĞ OLUN" : "THANK YOU FOR THE APPLAUSE";
            if (eventLight != null)
                eventLight.color = new Color(0.35f, 0.72f, 1f);
            WorldAudioDirector.Instance?.PlayRevealCue();

            yield return new WaitForSeconds(0.7f);
            if (sign != null)
            {
                sign.text = turkish ? "ŞİMDİ LÜTFEN DEVAM EDİN" : "NOW PLEASE MOVE ALONG";
                sign.color = originalSignColor;
            }

            yield return AnimateProps(false);
            if (eventLight != null)
            {
                eventLight.color = originalLightColor;
                eventLight.intensity = originalLightIntensity;
            }
        }

        private IEnumerator AnimateProps(bool rising)
        {
            Vector3[] fromPositions = new Vector3[props.Length];
            Vector3[] toPositions = new Vector3[props.Length];
            Quaternion[] fromRotations = new Quaternion[props.Length];
            Quaternion[] toRotations = new Quaternion[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] == null) continue;
                fromPositions[i] = props[i].localPosition;
                fromRotations[i] = props[i].localRotation;
                toPositions[i] = rising
                    ? startPositions[i] + Vector3.up * propLift
                    : startPositions[i];
                toRotations[i] = rising
                    ? startRotations[i] * Quaternion.Euler(0f, (i - 1) * 155f, (i % 2 == 0 ? 1f : -1f) * 28f)
                    : startRotations[i];
            }

            float elapsed = 0f;
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / animationDuration));
                for (int i = 0; i < props.Length; i++)
                {
                    if (props[i] == null) continue;
                    props[i].localPosition = Vector3.LerpUnclamped(fromPositions[i], toPositions[i], t);
                    props[i].localRotation = Quaternion.SlerpUnclamped(fromRotations[i], toRotations[i], t);
                }
                yield return null;
            }

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] == null) continue;
                props[i].localPosition = toPositions[i];
                props[i].localRotation = toRotations[i];
            }
        }
    }
}