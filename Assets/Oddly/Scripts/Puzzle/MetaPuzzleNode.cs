using UnityEngine;

namespace Oddly
{
    public enum MetaPuzzleAction
    {
        ReadNotice,
        PressForbiddenButton,
        PressNarratorSwitch,
        PressSystemSwitch
    }

    public sealed class MetaPuzzleNode : MonoBehaviour
    {
        [SerializeField] private MetaPuzzleController puzzle;
        [SerializeField] private MetaPuzzleAction action;
        [SerializeField] private SystemErrorPuzzle systemErrorPuzzle;

        public void Interact()
        {
            if (action == MetaPuzzleAction.PressNarratorSwitch)
                systemErrorPuzzle?.Press(0);
            else if (action == MetaPuzzleAction.PressSystemSwitch)
                systemErrorPuzzle?.Press(1);
            else
                puzzle?.Interact(action);
        }

        public string GetPrompt()
        {
            bool turkish = DialogueDirector.Instance != null && DialogueDirector.Instance.IsTurkish;
            if (action == MetaPuzzleAction.ReadNotice)
                return turkish ? "E  NOTU OKU" : "E  READ THE NOTICE";
            if (action == MetaPuzzleAction.PressNarratorSwitch)
                return turkish ? "E  ANLATICI ANAHTARI" : "E  NARRATOR SWITCH";
            if (action == MetaPuzzleAction.PressSystemSwitch)
                return turkish ? "E  SYSTEM-02 ANAHTARI" : "E  SYSTEM-02 SWITCH";
            return turkish ? "E  YASAK DÜĞMEYE BAS" : "E  PRESS THE FORBIDDEN BUTTON";
        }
    }
}