using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class ControlConsoleTrigger : MonoBehaviour
    {
        [SerializeField] private ControlRoomPuzzle puzzle;
        [SerializeField, Range(0, 2)] private int channelIndex;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (puzzle == null || other.GetComponentInParent<CharacterController>() == null) return;
            puzzle.CycleConsole(channelIndex);
        }
    }
}