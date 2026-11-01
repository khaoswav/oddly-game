using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class FinalChoiceTrigger : MonoBehaviour
    {
        [SerializeField] private FinalDecisionPuzzle puzzle;
        [SerializeField, Range(0, 2)] private int endingIndex;

        private void Reset() { GetComponent<BoxCollider>().isTrigger = true; }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<CharacterController>() == null) return;
            if (Input.GetKeyDown(KeyCode.E)) puzzle?.ChooseEnding(endingIndex);
        }
    }
}
