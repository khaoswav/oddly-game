using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class BackwardsStepTrigger : MonoBehaviour
    {
        [SerializeField] private BackwardsWalkPuzzle puzzle;
        [SerializeField, Min(0)] private int stepIndex;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerStay(Collider other)
        {
            CharacterController player = other.GetComponent<CharacterController>();
            if (player != null) puzzle?.TryStep(stepIndex, player);
        }
    }
}
