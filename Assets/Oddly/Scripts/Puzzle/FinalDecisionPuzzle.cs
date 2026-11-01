using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Resolves the finale choice and lets that choice visibly rewrite the
    /// atmosphere of the world. The selected ending is restored on relaunch.
    /// </summary>
    public sealed class FinalDecisionPuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh decisionSign;
        [SerializeField] private Light endingLight;
        [SerializeField] private Transform[] endingRelics = new Transform[3];
        [SerializeField, Min(0.5f)] private float worldTransitionDuration = 2.8f;

        private bool decided;
        private Light[] worldLights;
        private Color[] originalLightColors;
        private float[] originalLightIntensities;
        private Color originalAmbient;
        private Color originalFogColor;
        private float originalFogDensity;
        private bool originalFogEnabled;
        private Vector3[] relicBasePositions;
        private Coroutine endingAnimation;

        private static readonly Color[] EndingColors =
        {
            new Color(1f, 0.48f, 0.22f),
            new Color(0.24f, 0.52f, 1f),
            new Color(0.28f, 0.9f, 0.62f)
        };

        private static readonly Color[] EndingAmbients =
        {
            new Color(0.34f, 0.19f, 0.16f),
            new Color(0.13f, 0.2f, 0.34f),
            new Color(0.19f, 0.34f, 0.27f)
        };

        private static readonly Color[] EndingFogColors =
        {
            new Color(0.22f, 0.11f, 0.12f),
            new Color(0.08f, 0.13f, 0.25f),
            new Color(0.1f, 0.24f, 0.19f)
        };

        private void Awake()
        {
            originalAmbient = RenderSettings.ambientLight;
            originalFogColor = RenderSettings.fogColor;
            originalFogDensity = RenderSettings.fogDensity;
            originalFogEnabled = RenderSettings.fog;

            worldLights = FindObjectsOfType<Light>();
            originalLightColors = new Color[worldLights.Length];
            originalLightIntensities = new float[worldLights.Length];
            for (int i = 0; i < worldLights.Length; i++)
            {
                originalLightColors[i] = worldLights[i] != null ? worldLights[i].color : Color.white;
                originalLightIntensities[i] = worldLights[i] != null ? worldLights[i].intensity : 1f;
            }

            relicBasePositions = new Vector3[endingRelics.Length];
            for (int i = 0; i < endingRelics.Length; i++)
                if (endingRelics[i] != null)
                    relicBasePositions[i] = endingRelics[i].localPosition;
        }

        private void Start()
        {
            if (decisionSign != null)
                decisionSign.text = dialogue != null && dialogue.IsTurkish
                    ? "SON KARAR\nBİRİNİ SEÇ: ANLATICI / SYSTEM / HİÇBİRİ"
                    : "FINAL DECISION\nCHOOSE: NARRATOR / SYSTEM / NEITHER";

            if (PlayerPrefs.HasKey("oddly.ending"))
            {
                int savedEnding = Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending"), 0, 2);
                if (PlayerPrefs.GetInt("oddly.campaign.complete", 0) == 1)
                {
                    decided = true;
                    ApplyEndingPresentation(savedEnding, false);
                }
            }
        }

        public void ChooseEnding(int endingIndex)
        {
            if (decided || endingIndex < 0 || endingIndex > 2) return;
            decided = true;

            string endingName;
            string dialogueId;
            switch (endingIndex)
            {
                case 0:
                    endingName = "THE STORYTELLER";
                    dialogueId = "ending_narrator";
                    break;
                case 1:
                    endingName = "THE SYSTEM";
                    dialogueId = "ending_system";
                    break;
                default:
                    endingName = "THE PLAYER";
                    dialogueId = "ending_neither";
                    break;
            }

            PlayerPrefs.SetInt("oddly.ending", endingIndex);
            PlayerPrefs.SetString("oddly.ending.name", endingName);
            PlayerPrefs.Save();
            CampaignDirector.Instance?.CompleteCampaign(endingIndex);

            ApplyEndingPresentation(endingIndex, true);

            if (decisionSign != null)
            {
                bool turkish = dialogue != null && dialogue.IsTurkish;
                string[] tr =
                {
                    "SON: HİKÂYE ANLATICININ",
                    "SON: SİSTEMİN KONTROLÜ",
                    "SON: OYUNCUNUN ÖZGÜRLÜĞÜ"
                };
                string[] en =
                {
                    "ENDING: THE STORYTELLER",
                    "ENDING: SYSTEM CONTROL",
                    "ENDING: PLAYER FREEDOM"
                };
                decisionSign.text = turkish ? tr[endingIndex] : en[endingIndex];
                decisionSign.color = EndingColors[endingIndex];
            }

            dialogue?.Say(dialogueId);
            exitDoor?.Unlock();
        }

        private void ApplyEndingPresentation(int endingIndex, bool animate)
        {
            endingIndex = Mathf.Clamp(endingIndex, 0, 2);
            Color endingColor = EndingColors[endingIndex];
            Color targetAmbient = EndingAmbients[endingIndex];
            Color targetFog = EndingFogColors[endingIndex];

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = targetFog;
            RenderSettings.fogDensity = endingIndex == 0 ? 0.012f :
                endingIndex == 1 ? 0.026f : 0.008f;
            RenderSettings.ambientLight = targetAmbient;

            for (int i = 0; i < worldLights.Length; i++)
            {
                Light worldLight = worldLights[i];
                if (worldLight == null) continue;
                worldLight.color = Color.Lerp(originalLightColors[i], endingColor,
                    worldLight == endingLight ? 1f : 0.48f);
                worldLight.intensity = originalLightIntensities[i] *
                    (worldLight == endingLight ? 1.9f : 0.82f);
            }

            if (endingLight != null)
            {
                endingLight.color = endingColor;
                endingLight.intensity = 2.1f;
            }

            for (int i = 0; i < endingRelics.Length; i++)
            {
                Transform relic = endingRelics[i];
                if (relic == null) continue;
                bool selected = i == endingIndex;
                relic.localScale = Vector3.one * (selected ? 1.45f : 0.42f);
                relic.localPosition = relicBasePositions[i] +
                    Vector3.up * (selected ? 0.35f : -0.18f);
                relic.localRotation = Quaternion.Euler(0f, selected ? 0f : 35f, 0f);
            }

            if (animate)
            {
                if (endingAnimation != null) StopCoroutine(endingAnimation);
                endingAnimation = StartCoroutine(AnimateEnding(endingIndex));
                WorldAudioDirector.Instance?.PlayEndingCue(endingIndex);
            }
        }

        private IEnumerator AnimateEnding(int endingIndex)
        {
            float elapsed = 0f;
            Color startAmbient = originalAmbient;
            Color startFog = originalFogColor;
            float startFogDensity = originalFogDensity;
            float targetFogDensity = endingIndex == 0 ? 0.012f :
                endingIndex == 1 ? 0.026f : 0.008f;

            while (elapsed < worldTransitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f,
                    Mathf.Clamp01(elapsed / worldTransitionDuration));
                RenderSettings.ambientLight = Color.Lerp(startAmbient, EndingAmbients[endingIndex], t);
                RenderSettings.fogColor = Color.Lerp(startFog, EndingFogColors[endingIndex], t);
                RenderSettings.fogDensity = Mathf.Lerp(startFogDensity, targetFogDensity, t);

                for (int i = 0; i < endingRelics.Length; i++)
                {
                    Transform relic = endingRelics[i];
                    if (relic == null) continue;

                    float phase = elapsed * (endingIndex == 1 ? 2.4f : 1.35f) + i * 1.7f;
                    if (i == endingIndex)
                    {
                        if (endingIndex == 0)
                            relic.Rotate(Vector3.up, 85f * Time.unscaledDeltaTime, Space.Self);
                        else if (endingIndex == 1)
                            relic.localPosition = relicBasePositions[i] +
                                Vector3.up * (0.35f + Mathf.Sin(phase * 2f) * 0.045f);
                        else
                        {
                            relic.localPosition = relicBasePositions[i] +
                                new Vector3(Mathf.Sin(phase) * 0.16f,
                                    0.35f + Mathf.Sin(phase * 1.7f) * 0.12f,
                                    Mathf.Cos(phase) * 0.16f);
                            relic.Rotate(Vector3.up, 42f * Time.unscaledDeltaTime, Space.Self);
                        }
                    }
                }
                yield return null;
            }

            RenderSettings.ambientLight = EndingAmbients[endingIndex];
            RenderSettings.fogColor = EndingFogColors[endingIndex];
            RenderSettings.fogDensity = targetFogDensity;
            endingAnimation = null;
            WorldAudioDirector.Instance?.PlayRevealCue();
        }
    }
}
