using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A harmless visual transition between Chapter 10's unfinished build and
    /// Chapter 11's patch notes. The corridor briefly appears to hot-reload.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class VersionSeamTransitionTrigger : MonoBehaviour
    {
        [SerializeField] private Transform[] panels;
        [SerializeField] private TextMesh sign;
        [SerializeField] private Light eventLight;
        [SerializeField, Min(0.2f)] private float transitionDuration = 1.2f;
        [SerializeField, Min(0.1f)] private float panelTravel = 0.85f;

        private Vector3[] initialPositions;
        private Quaternion[] initialRotations;
        private Color initialSignColor;
        private Color initialLightColor;
        private float initialLightIntensity;
        private bool played;

        private void Start()
        {
            initialPositions = new Vector3[panels.Length];
            initialRotations = new Quaternion[panels.Length];
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i] == null) continue;
                initialPositions[i] = panels[i].localPosition;
                initialRotations[i] = panels[i].localRotation;
            }

            if (sign != null) initialSignColor = sign.color;
            if (eventLight != null)
            {
                initialLightColor = eventLight.color;
                initialLightIntensity = eventLight.intensity;
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
            StartCoroutine(PlayVersionSeam());
        }

        private IEnumerator PlayVersionSeam()
        {
            bool turkish = DialogueDirector.Instance != null && DialogueDirector.Instance.IsTurkish;
            if (sign != null)
            {
                sign.text = turkish ? "GÜNCELLEME YÜKLENİYOR..." : "INSTALLING UPDATE...";
                sign.color = new Color(0.95f, 0.55f, 0.22f);
            }
            if (eventLight != null)
            {
                eventLight.color = new Color(1f, 0.58f, 0.2f);
                eventLight.intensity = initialLightIntensity * 1.8f;
            }

            WorldAudioDirector.Instance?.PlaySurpriseCue();

            Vector3[] fromPositions = new Vector3[panels.Length];
            Vector3[] toPositions = new Vector3[panels.Length];
            Quaternion[] fromRotations = new Quaternion[panels.Length];
            Quaternion[] toRotations = new Quaternion[panels.Length];

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i] == null) continue;
                fromPositions[i] = panels[i].localPosition;
                float direction = i == 0 ? -1f : 1f;
                toPositions[i] = initialPositions[i] + Vector3.right * direction * panelTravel;
                toRotations[i] = initialRotations[i] * Quaternion.Euler(0f, 0f, direction * 12f);
            }

            yield return AnimatePanels(fromPositions, toPositions, fromRotations, toRotations);

            if (sign != null)
            {
                sign.text = turkish ? "BU BİR HATA DEĞİL, ÖZELLİK." : "NOT A BUG. A FEATURE.";
                sign.color = new Color(0.3f, 0.75f, 0.55f);
            }
            if (eventLight != null)
            {
                eventLight.color = new Color(0.3f, 0.75f, 0.55f);
                eventLight.intensity = initialLightIntensity * 1.25f;
            }
            WorldAudioDirector.Instance?.PlayRevealCue();
            yield return new WaitForSeconds(0.55f);

            if (sign != null)
            {
                sign.text = turkish ? "YAMA NOTLARINA HOŞ GELDİNİZ" : "WELCOME TO THE PATCH NOTES";
                sign.color = initialSignColor;
            }

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i] == null) continue;
                fromPositions[i] = panels[i].localPosition;
                fromRotations[i] = panels[i].localRotation;
                toPositions[i] = initialPositions[i];
                toRotations[i] = initialRotations[i];
            }
            yield return AnimatePanels(fromPositions, toPositions, fromRotations, toRotations);

            if (eventLight != null)
            {
                eventLight.color = initialLightColor;
                eventLight.intensity = initialLightIntensity;
            }
        }

        private IEnumerator AnimatePanels(Vector3[] fromPositions, Vector3[] toPositions,
            Quaternion[] fromRotations, Quaternion[] toRotations)
        {
            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / transitionDuration));
                for (int i = 0; i < panels.Length; i++)
                {
                    if (panels[i] == null) continue;
                    panels[i].localPosition = Vector3.LerpUnclamped(fromPositions[i], toPositions[i], t);
                    panels[i].localRotation = Quaternion.SlerpUnclamped(fromRotations[i], toRotations[i], t);
                }
                yield return null;
            }

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i] == null) continue;
                panels[i].localPosition = toPositions[i];
                panels[i].localRotation = toRotations[i];
            }
        }
    }
}