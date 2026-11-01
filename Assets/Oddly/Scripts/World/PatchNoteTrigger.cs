using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class PatchNoteTrigger : MonoBehaviour
    {
        [SerializeField] private PatchNotesPuzzle puzzle;
        [SerializeField, Range(0, 2)] private int patchIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() != null)
                puzzle?.ApplyPatch(patchIndex);
        }
    }
}