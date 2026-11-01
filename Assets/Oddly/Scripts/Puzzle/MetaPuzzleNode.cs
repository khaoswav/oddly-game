using UnityEngine;

namespace Oddly
{
    public enum MetaPuzzleAction
    {
        ReadNotice,
        PressForbiddenButton
    }

    public sealed class MetaPuzzleNode : MonoBehaviour
    {
        [SerializeField] private MetaPuzzleController puzzle;
        [SerializeField] private MetaPuzzleAction action;

        public void Interact()
        {
            puzzle?.Interact(action);
        }

        public string GetPrompt()
        {
            bool turkish = DialogueDirector.Instance != null && DialogueDirector.Instance.IsTurkish;
            if (action == MetaPuzzleAction.ReadNotice)
                return turkish ? "E  NOTU OKU" : "E  READ THE NOTICE";
            return turkish ? "E  YASAK DÜĞMEYE BAS" : "E  PRESS THE FORBIDDEN BUTTON";
        }
    }
}