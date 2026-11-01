using UnityEngine;
using UnityEngine.UI;

namespace Oddly
{
    public sealed class OptionsMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private FirstPersonController movement;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Text sensitivityValue;
        [SerializeField] private Text fieldOfViewValue;

        private float sensitivity;
        private float fieldOfView;

        private void Start()
        {
            sensitivity = PlayerPrefs.GetFloat("oddly.sensitivity", 2f);
            fieldOfView = PlayerPrefs.GetFloat("oddly.fov", 78f);
            Apply();
            RefreshLabels();
        }

        public void Open()
        {
            if (titlePanel != null) titlePanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(true);
        }

        public void Close()
        {
            if (optionsPanel != null) optionsPanel.SetActive(false);
            if (titlePanel != null) titlePanel.SetActive(true);
        }

        public void AdjustSensitivity(float amount)
        {
            sensitivity = Mathf.Clamp(Mathf.Round((sensitivity + amount) * 10f) / 10f, 0.5f, 5f);
            PlayerPrefs.SetFloat("oddly.sensitivity", sensitivity);
            PlayerPrefs.Save();
            Apply();
            RefreshLabels();
        }

        public void AdjustFieldOfView(float amount)
        {
            fieldOfView = Mathf.Clamp(Mathf.Round((fieldOfView + amount) / 2f) * 2f, 60f, 100f);
            PlayerPrefs.SetFloat("oddly.fov", fieldOfView);
            PlayerPrefs.Save();
            Apply();
            RefreshLabels();
        }

        private void Apply()
        {
            if (movement != null) movement.SetLookSensitivity(sensitivity);
            if (playerCamera != null) playerCamera.fieldOfView = fieldOfView;
        }

        private void RefreshLabels()
        {
            if (sensitivityValue != null) sensitivityValue.text = sensitivity.ToString("0.0");
            if (fieldOfViewValue != null) fieldOfViewValue.text = fieldOfView.ToString("0");
        }
    }
}