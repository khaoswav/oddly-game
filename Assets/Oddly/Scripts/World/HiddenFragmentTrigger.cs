using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class HiddenFragmentTrigger : MonoBehaviour
    {
        [SerializeField] private HiddenChapterGate gate;
        private bool collected;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (collected || other.GetComponent<CharacterController>() == null) return;
            collected = true;
            Renderer visual = GetComponentInChildren<Renderer>();
            if (visual != null) visual.enabled = false;
            Collider trigger = GetComponent<Collider>();
            if (trigger != null) trigger.enabled = false;
            gate?.RegisterFragment();
        }
    }
}
