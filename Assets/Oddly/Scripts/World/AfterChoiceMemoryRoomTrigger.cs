using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Turns the eight persistent hidden-room records into a visual post-finale
    /// memory wall and delivers an ending-aware environmental callback.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class AfterChoiceMemoryRoomTrigger : MonoBehaviour
    {
        [SerializeField] private Transform[] memoryPillars = new Transform[8];
        [SerializeField] private TextMesh statusSign;
        private bool hasEntered;

        private void Start() => RefreshPillars();
        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (hasEntered || other.GetComponentInParent<CharacterController>() == null) return;
            hasEntered = true;
            int count = HiddenChapterArchive.CompletedCount;
            bool turkish = PlayerPrefs.GetInt("oddly.language", 1) == 0;
            if (statusSign != null)
                statusSign.text = turkish
                    ? "SEÇİMDEN SONRA\nODALAR: " + count + " / 8  •  YANKILAR: " + AfterChoiceEchoPillar.DiscoveredCount + " / 8"
                    : "AFTER THE CHOICE\nROOMS: " + count + " / 8  •  ECHOES: " + AfterChoiceEchoPillar.DiscoveredCount + " / 8";

            switch (Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2))
            {
                case 0: DialogueDirector.Instance?.Say("afterchoice_memory_narrator"); break;
                case 1: DialogueDirector.Instance?.Say("afterchoice_memory_system"); break;
                default: DialogueDirector.Instance?.Say("afterchoice_memory_player"); break;
            }
            WorldAudioDirector.Instance?.PlayRevealCue();
        }

        private void RefreshPillars()
        {
            for (int i = 0; i < memoryPillars.Length; i++)
            {
                Transform pillar = memoryPillars[i];
                if (pillar == null) continue;
                bool completed = HiddenChapterArchive.IsCompleted(i + 1);
                Renderer renderer = pillar.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Color endingColor = GetEndingColor();
                    renderer.material.color = completed
                        ? Color.Lerp(endingColor, Color.white, 0.24f)
                        : new Color(0.12f, 0.14f, 0.19f);
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", completed ? endingColor * 0.55f : Color.black);
                }
                pillar.localScale = completed
                    ? new Vector3(0.9f, 3.4f, 0.9f)
                    : new Vector3(0.58f, 1.45f, 0.58f);
            }
        }

        private static Color GetEndingColor()
        {
            switch (Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2))
            {
                case 0: return new Color(1f, 0.48f, 0.22f);
                case 1: return new Color(0.24f, 0.52f, 1f);
                default: return new Color(0.28f, 0.9f, 0.62f);
            }
        }
    }
}