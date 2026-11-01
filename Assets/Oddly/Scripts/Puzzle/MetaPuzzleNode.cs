using UnityEngine;

namespace Oddly
{
    public enum MetaPuzzleAction
    {
        ReadNotice,
        PressForbiddenButton,
        PressNarratorSwitch,
        PressSystemSwitch,
        ChooseHabitDrop,
        ChooseHabitRetry,
        ChooseHabitWait
    }

    public sealed class MetaPuzzleNode : MonoBehaviour
    {
        [SerializeField] private MetaPuzzleController puzzle;
        [SerializeField] private MetaPuzzleAction action;
        [SerializeField] private SystemErrorPuzzle systemErrorPuzzle;
        [SerializeField] private BehaviorMemoryPuzzle behaviorMemoryPuzzle;

        public void Interact()
        {
            if (action == MetaPuzzleAction.PressNarratorSwitch)
                systemErrorPuzzle?.Press(0);
            else if (action == MetaPuzzleAction.PressSystemSwitch)
                systemErrorPuzzle?.Press(1);
            else if (action == MetaPuzzleAction.ChooseHabitDrop)
                behaviorMemoryPuzzle?.Choose(0);
            else if (action == MetaPuzzleAction.ChooseHabitRetry)
                behaviorMemoryPuzzle?.Choose(1);
            else if (action == MetaPuzzleAction.ChooseHabitWait)
                behaviorMemoryPuzzle?.Choose(2);
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
            if (action == MetaPuzzleAction.ChooseHabitDrop)
                return turkish ? "E  NESNE BIRAK" : "E  DROP OBJECT";
            if (action == MetaPuzzleAction.ChooseHabitRetry)
                return turkish ? "E  TEKRAR DENE" : "E  TRY AGAIN";
            if (action == MetaPuzzleAction.ChooseHabitWait)
                return turkish ? "E  BEKLE" : "E  WAIT";
            return turkish ? "E  YASAK DÜĞMEYE BAS" : "E  PRESS THE FORBIDDEN BUTTON";
        }
    }
}