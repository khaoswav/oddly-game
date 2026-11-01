using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class FinalChoiceTrigger : MonoBehaviour
    {
        [SerializeField] private FinalDecisionPuzzle puzzle;
        [SerializeField, Range(0, 2)] private int endingIndex;

        private bool playerInside;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<CharacterController>() != null)
                playerInside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<CharacterController>() != null)
                playerInside = false;
        }

        private void Update()
        {
            if (playerInside && Input.GetKeyDown(KeyCode.E))
                puzzle?.ChooseEnding(endingIndex);
        }
    }
}
