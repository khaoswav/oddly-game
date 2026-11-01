using UnityEngine;
using UnityEngine.UI;

namespace Oddly
{
    public sealed class PrototypeHUD : MonoBehaviour
    {
        [SerializeField] private InteractionController interaction;
        [SerializeField] private Text prompt;

        private void Update()
        {
            if (prompt != null && interaction != null)
                prompt.text = interaction.GetPrompt();
        }
    }
}
