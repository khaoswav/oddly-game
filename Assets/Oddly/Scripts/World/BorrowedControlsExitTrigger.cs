using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class BorrowedControlsExitTrigger : MonoBehaviour
    {
        [SerializeField] private BorrowedControlsPuzzle puzzle;
        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            FirstPersonController controller = other.GetComponentInParent<FirstPersonController>();
            if (controller != null) puzzle?.Complete(controller);
        }
    }
}
