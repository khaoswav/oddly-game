using UnityEngine;

namespace Oddly
{
    public sealed class HiddenChapterGate : MonoBehaviour
    {
        [SerializeField] private Transform gatePanel;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private int fragmentsRequired = 3;

        private int fragmentsFound;
        private bool opened;

        private void Start() => RefreshStatus();

        public void RegisterFragment()
        {
            if (opened) return;
            fragmentsFound = Mathf.Min(fragmentsFound + 1, fragmentsRequired);
            RefreshStatus();
            if (fragmentsFound < fragmentsRequired) return;

            opened = true;
            if (gatePanel != null)
            {
                Vector3 position = gatePanel.position;
                position.y += 4.5f;
                gatePanel.position = position;
            }

            if (statusSign != null)
                statusSign.text = dialogue != null && dialogue.IsTurkish
                    ? "GİZLİ GEÇİT AÇILDI" : "HIDDEN PASSAGE OPEN";
            dialogue?.Say("hidden_gate_open");
        }

        private void RefreshStatus()
        {
            if (statusSign == null) return;
            statusSign.text = dialogue != null && dialogue.IsTurkish
                ? $"KAYIP PARÇALAR: {fragmentsFound}/{fragmentsRequired}"
                : $"MISSING FRAGMENTS: {fragmentsFound}/{fragmentsRequired}";
        }
    }
}
