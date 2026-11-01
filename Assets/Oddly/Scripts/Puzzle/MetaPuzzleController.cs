using UnityEngine;

namespace Oddly
{
    public sealed class MetaPuzzleController : MonoBehaviour
    {
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;

        private bool noticeRead;
        private bool solved;

        public void Interact(MetaPuzzleAction action)
        {
            if (solved) return;

            if (action == MetaPuzzleAction.ReadNotice)
            {
                noticeRead = true;
                dialogue?.Say("meta_notice");
                return;
            }

            if (!noticeRead)
            {
                dialogue?.Say("meta_wrong");
                return;
            }

            solved = true;
            linkedDoor?.Unlock();
            dialogue?.Say("meta_solved");
        }
    }
}