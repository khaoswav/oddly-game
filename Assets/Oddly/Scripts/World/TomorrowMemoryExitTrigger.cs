using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class TomorrowMemoryExitTrigger : MonoBehaviour
    {
        [SerializeField] private TomorrowMemoryPuzzle puzzle;
        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() != null)
                puzzle?.ReachExit();
        }
    }
}
