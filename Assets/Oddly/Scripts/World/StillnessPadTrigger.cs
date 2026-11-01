using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class StillnessPadTrigger : MonoBehaviour
    {
        [SerializeField] private StillnessSequencePuzzle puzzle;
        [SerializeField, Range(0, 2)] private int padIndex;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerStay(Collider other)
        {
            CharacterController player = other.GetComponent<CharacterController>();
            if (player != null) puzzle?.TickPad(padIndex, player);
        }
    }
}
