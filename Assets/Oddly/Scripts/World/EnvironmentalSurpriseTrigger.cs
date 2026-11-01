using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A one-shot environmental gag: signage lies, props react and the room
    /// briefly changes its lighting when the player walks through.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class EnvironmentalSurpriseTrigger : MonoBehaviour
    {
        [SerializeField] private Transform[] props;
        [SerializeField] private TextMesh sign;
        [SerializeField] private Light eventLight;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private string englishSign = "EXIT THIS WAY";
        [SerializeField] private string turkishSign = "ÇIKIŞ BU TARAFTA";
        [SerializeField, Min(0.05f)] private float beatDuration = 0.55f;

        private Vector3[] startPositions;
        private Quaternion[] startRotations;
        private Vector3[] startScales;
        private Color originalSignColor;
        private Color originalLightColor;
        private float originalLightIntensity;
        private bool played;

        private void Start()
        {
            startPositions = new Vector3[props.Length];
            startRotations = new Quaternion[props.Length];
            startScales = new Vector3[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] == null) continue;
                startPositions[i] = props[i].localPosition;
                startRotations[i] = props[i].localRotation;
                startScales[i] = props[i].localScale;
            }

            if (sign != null) originalSignColor = sign.color;
            if (eventLight != null)
            {
                originalLightColor = eventLight.color;
                originalLightIntensity = eventLight.intensity;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (played || other.GetComponentInParent<CharacterController>() == null) return;
            played = true;
            WorldAudioDirector.Instance?.PlaySurpriseCue();
            StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            if (sign != null)
            {
                sign.text = dialogue != null && dialogue.IsTurkish
                    ? "ÇIKIŞ TAM TERSİ YÖNDE"
                    : "EXIT IS THE OTHER WAY";
                sign.color = new Color(0.88f, 0.22f, 0.36f);
            }

            if (dialogue != null) dialogue.Say("surprise_exit_sign");
            if (eventLight != null)
            {
                eventLight.color = new Color(1f, 0.25f, 0.42f);
                eventLight.intensity = originalLightIntensity * 2.1f;
            }

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] == null) continue;
                Vector3 targetPosition = startPositions[i] + new Vector3(i % 2 == 0 ? 0.65f : -0.65f, 0.45f, 0f);
                Quaternion targetRotation = startRotations[i] * Quaternion.Euler(0f, 95f + i * 37f, 25f);
                Vector3 targetScale = startScales[i] * (i == 1 ? 0.55f : 1.25f);
                StartCoroutine(AnimateProp(props[i], targetPosition, targetRotation, targetScale));
            }

            yield return new WaitForSeconds(beatDuration);
            if (sign != null)
            {
                sign.text = dialogue != null && dialogue.IsTurkish
                    ? "ŞAKA YAPTIM. BELKİ."
                    : "JUST KIDDING. MAYBE.";
                sign.color = new Color(0.22f, 0.25f, 0.32f);
            }
            WorldAudioDirector.Instance?.PlayRevealCue();
            if (eventLight != null)
            {
                eventLight.color = new Color(0.35f, 0.72f, 1f);
                eventLight.intensity = originalLightIntensity * 0.45f;
            }

            yield return new WaitForSeconds(beatDuration * 1.6f);
            if (dialogue != null) dialogue.Say("surprise_props_move");
            if (eventLight != null)
            {
                eventLight.color = originalLightColor;
                eventLight.intensity = originalLightIntensity;
            }
            if (sign != null) sign.color = originalSignColor;
        }

        private IEnumerator AnimateProp(Transform item, Vector3 targetPosition,
            Quaternion targetRotation, Vector3 targetScale)
        {
            Vector3 fromPosition = item.localPosition;
            Quaternion fromRotation = item.localRotation;
            Vector3 fromScale = item.localScale;
            float elapsed = 0f;
            while (elapsed < beatDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / beatDuration));
                item.localPosition = Vector3.LerpUnclamped(fromPosition, targetPosition, t);
                item.localRotation = Quaternion.SlerpUnclamped(fromRotation, targetRotation, t);
                item.localScale = Vector3.LerpUnclamped(fromScale, targetScale, t);
                yield return null;
            }
            item.localPosition = targetPosition;
            item.localRotation = targetRotation;
            item.localScale = targetScale;
        }
    }
}