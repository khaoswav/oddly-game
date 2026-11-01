using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Plays the alternate finale after the ninth record has been discovered.
    /// This is a separate coda and does not overwrite the player's three-ending history.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class AfterChoiceUnwrittenEndingTrigger : MonoBehaviour
    {
        private const string SeenKey = "oddly.afterchoice.alternateEndingSeen";

        [SerializeField] private Transform finalCore;
        [SerializeField] private Light endingLight;
        [SerializeField] private TextMesh statusSign;
        [SerializeField, Min(0.1f)] private float finaleDuration = 2.8f;

        private bool playerInside;
        private bool seen;
        private Vector3 coreBaseScale;
        private Color originalAmbient;
        private Color originalFog;
        private float originalFogDensity;
        private float originalLightIntensity;
        private Coroutine finale;

        public static bool HasSeen => PlayerPrefs.GetInt(SeenKey, 0) == 1;

        private void Awake()
        {
            if (finalCore != null) coreBaseScale = finalCore.localScale;
            originalAmbient = RenderSettings.ambientLight;
            originalFog = RenderSettings.fogColor;
            originalFogDensity = RenderSettings.fogDensity;
            if (endingLight != null) originalLightIntensity = endingLight.intensity;

            seen = HasSeen;
            if (seen) ApplyFinaleLook();
        }

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() == null) return;
            playerInside = true;
            SetStatus(seen
                ? (IsTurkish ? "YAZILMAMIŞ SON / E'YE BAS" : "THE UNWRITTEN ENDING / PRESS E")
                : (IsTurkish ? "SON KAYDI DİNLE / E'YE BAS" : "HEAR THE FINAL RECORD / PRESS E"));
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() != null)
                playerInside = false;
        }

        private void Update()
        {
            if (seen && finalCore != null)
            {
                float pulse = 1f + Mathf.Sin(Time.unscaledTime * 1.25f) * 0.045f;
                finalCore.localScale = coreBaseScale * pulse;
                finalCore.Rotate(Vector3.up, 9f * Time.unscaledDeltaTime, Space.World);
            }

            if (!playerInside || !Input.GetKeyDown(KeyCode.E)) return;
            if (!AfterChoiceNinthRecordTrigger.IsAlternateEndingUnlocked)
            {
                SetStatus(IsTurkish
                    ? "KİLİTLİ / ÖNCE DOKUZUNCU KAYDI BUL"
                    : "SEALED / DISCOVER THE NINTH RECORD FIRST");
                DialogueDirector.Instance?.Say("afterchoice_unwritten_locked");
                return;
            }

            if (seen)
            {
                SetStatus(IsTurkish ? "YAZILMAMIŞ SON HATIRLANDI" : "THE UNWRITTEN ENDING REMEMBERED");
                DialogueDirector.Instance?.Say("afterchoice_unwritten_repeat");
                return;
            }

            seen = true;
            PlayerPrefs.SetInt(SeenKey, 1);
            PlayerPrefs.Save();
            SetStatus(IsTurkish
                ? "YAZILMAMIŞ SON / TAMAMLANDI"
                : "THE UNWRITTEN ENDING / COMPLETE");
            DialogueDirector.Instance?.Say("afterchoice_unwritten_complete");
            WorldAudioDirector.Instance?.PlayRevealCue();
            if (finale != null) StopCoroutine(finale);
            finale = StartCoroutine(PlayFinale());
        }

        private IEnumerator PlayFinale()
        {
            Color targetAmbient = new Color(0.62f, 0.66f, 0.72f);
            Color targetFog = new Color(0.12f, 0.14f, 0.19f);
            float elapsed = 0f;
            while (elapsed < finaleDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / finaleDuration));
                RenderSettings.ambientLight = Color.Lerp(originalAmbient, targetAmbient, t);
                RenderSettings.fogColor = Color.Lerp(originalFog, targetFog, t);
                RenderSettings.fogDensity = Mathf.Lerp(originalFogDensity, 0.018f, t);
                if (endingLight != null)
                    endingLight.intensity = Mathf.Lerp(originalLightIntensity, 2.6f, t);
                if (finalCore != null)
                    finalCore.localScale = Vector3.Lerp(coreBaseScale, coreBaseScale * 1.45f, t);
                yield return null;
            }

            ApplyFinaleLook();
            finale = null;
        }

        private void ApplyFinaleLook()
        {
            RenderSettings.ambientLight = new Color(0.62f, 0.66f, 0.72f);
            RenderSettings.fogColor = new Color(0.12f, 0.14f, 0.19f);
            RenderSettings.fogDensity = 0.018f;
            if (endingLight != null) endingLight.intensity = 2.6f;
            if (finalCore != null)
            {
                Renderer renderer = finalCore.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white;
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", new Color(0.82f, 0.9f, 1f) * 2.4f);
                }
                finalCore.localScale = coreBaseScale * 1.45f;
            }
        }

        private void SetStatus(string value)
        {
            if (statusSign != null) statusSign.text = value;
        }

        private static bool IsTurkish => PlayerPrefs.GetInt("oddly.language", 1) == 0;
    }
}