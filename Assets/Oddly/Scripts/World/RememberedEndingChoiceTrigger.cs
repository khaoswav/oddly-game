using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class RememberedEndingChoiceTrigger : MonoBehaviour
    {
        [SerializeField] private RememberedSelfPuzzle puzzle;
        [SerializeField, Range(0, 2)] private int endingIndex;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponentInParent<FirstPersonController>() == null) return;
            if (Input.GetKeyDown(KeyCode.E)) puzzle?.Choose(endingIndex);
        }
    }
}
