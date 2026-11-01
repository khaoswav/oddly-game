using UnityEngine;
using Oddly.Scripts.Puzzle;

namespace Oddly.Scripts.World
{
    public class LastSaveSlotTrigger : MonoBehaviour
    {
        [SerializeField] private LastSavePuzzle puzzle;
        [SerializeField] private int slotIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (puzzle == null || other.GetComponent<CharacterController>() == null) return;
            puzzle.LoadSlot(slotIndex);
        }
    }
}