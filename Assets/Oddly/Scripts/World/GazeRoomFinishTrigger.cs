using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class GazeRoomFinishTrigger : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        private bool completed;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (completed || other.GetComponentInParent<FirstPersonController>() == null) return;
            completed = true;
            if (statusSign != null)
                statusSign.text = dialogue != null && dialogue.IsTurkish
                    ? "GÖRÜNMEYEN DUVARLAR GEÇİLDİ\nÇIKIŞ AÇIK"
                    : "UNSEEN WALLS CROSSED\nEXIT OPEN";
            dialogue?.Say("hidden_gaze_room_complete");
            exitDoor?.Unlock();
        }
    }
}
