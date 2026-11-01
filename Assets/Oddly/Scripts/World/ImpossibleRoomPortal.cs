using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class ImpossibleRoomPortal : MonoBehaviour
    {
        [SerializeField] private Transform destination;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private float exitOffset = 1.35f;
        [SerializeField] private float transferCooldown = 0.8f;

        private float nextTransferTime;

        private void Reset()
        {
            BoxCollider trigger = GetComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1.45f, 2.1f, 0.55f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (destination == null || Time.time < nextTransferTime) return;

            CharacterController controller = other.GetComponentInParent<CharacterController>();
            if (controller == null || !controller.enabled) return;

            Transform player = controller.transform;
            nextTransferTime = Time.time + transferCooldown;

            Vector3 exitPosition = destination.position +
                                   destination.forward * exitOffset +
                                   Vector3.up * 0.05f;
            Quaternion exitRotation = destination.rotation;

            controller.enabled = false;
            player.SetPositionAndRotation(exitPosition, exitRotation);
            controller.enabled = true;

            dialogue?.Say("impossible_room");
        }
    }
}