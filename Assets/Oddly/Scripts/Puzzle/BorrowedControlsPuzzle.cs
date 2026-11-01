using UnityEngine;

namespace Oddly
{
    public sealed class BorrowedControlsPuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        private FirstPersonController player;
        private bool entered;
        private bool completed;

        public void Enter(FirstPersonController controller)
        {
            if (completed || entered || controller == null) return;
            player = controller;
            entered = true;
            player.SetControlsInverted(true);
            if (statusSign != null)
                statusSign.text = dialogue != null && dialogue.IsTurkish
                    ? "KONTROLLER ÖDÜNÇ ALINDI\nİLERİ GİTMEK İÇİN GERİ TUŞUNA BAS"
                    : "CONTROLS BORROWED\nPRESS BACK TO MOVE FORWARD";
            dialogue?.Say("hidden_controls_borrowed");
        }

        public void Complete(FirstPersonController controller)
        {
            if (!entered || completed || controller == null) return;
            completed = true;
            controller.SetControlsInverted(false);
            if (statusSign != null)
                statusSign.text = dialogue != null && dialogue.IsTurkish
                    ? "KONTROLLER İADE EDİLDİ\nÇIKIŞ AÇIK"
                    : "CONTROLS RETURNED\nEXIT OPEN";
            dialogue?.Say("hidden_controls_returned");
            exitDoor?.Unlock();
        }

        private void OnDisable()
        {
            if (player != null) player.SetControlsInverted(false);
        }
    }
}
