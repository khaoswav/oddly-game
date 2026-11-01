using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A reactive room that physically rearranges its props whenever the
    /// player repeats a recorded habit. It only listens while the player is inside.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class LaughingRoomDirector : MonoBehaviour
    {
        [SerializeField] private BehaviorTracker behavior;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private Transform[] prankObjects;
        [SerializeField] private Light roomLight;
        [SerializeField] private Transform movingWall;
        [SerializeField] private float wallShift = 1.4f;
        [SerializeField, Min(1)] private int reactionsToUnlock = 5;
        [SerializeField] private DoorController linkedDoor;

        private Vector3[] originalPositions;
        private Quaternion[] originalRotations;
        private Vector3 wallOriginalPosition;
        private int reactionCount;
        private bool active;
        private bool exitUnlocked;
        private Coroutine lightPulse;

        private void Start()
        {
            if (behavior == null) behavior = FindObjectOfType<BehaviorTracker>();
            originalPositions = new Vector3[prankObjects.Length];
            originalRotations = new Quaternion[prankObjects.Length];
            for (int i = 0; i < prankObjects.Length; i++)
            {
                if (prankObjects[i] == null) continue;
                originalPositions[i] = prankObjects[i].position;
                originalRotations[i] = prankObjects[i].rotation;
            }
            if (movingWall != null) wallOriginalPosition = movingWall.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (active || other.GetComponentInParent<CharacterController>() == null) return;
            active = true;
            if (behavior != null)
            {
                behavior.FailedInteractionRegistered += OnFailedInteraction;
                behavior.ObjectDropped += OnObjectDropped;
            }
            dialogue?.Say("laughing_room_entry");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<CharacterController>() == null) return;
            Deactivate();
        }

        private void OnDisable() => Deactivate();

        private void Deactivate()
        {
            if (!active) return;
            active = false;
            if (behavior != null)
            {
                behavior.FailedInteractionRegistered -= OnFailedInteraction;
                behavior.ObjectDropped -= OnObjectDropped;
            }
        }

        private void OnFailedInteraction() => React(false);
        private void OnObjectDropped() => React(true);

        private void React(bool dropped)
        {
            if (!active) return;
            reactionCount++;
            int index = prankObjects.Length > 0 ? (reactionCount - 1) % prankObjects.Length : 0;
            Transform target = prankObjects.Length > 0 ? prankObjects[index] : null;
            if (target != null)
            {
                Vector3 original = originalPositions[index];
                float side = reactionCount % 2 == 0 ? 1f : -1f;
                target.position = original + new Vector3(side * (0.7f + 0.15f * (reactionCount % 3)), 0.15f * (reactionCount % 2), 0f);
                target.rotation = originalRotations[index] * Quaternion.Euler(0f, 25f * reactionCount, 12f * side);
            }

            if (movingWall != null)
            {
                float side = reactionCount % 2 == 0 ? 1f : -1f;
                movingWall.position = wallOriginalPosition + Vector3.right * wallShift * side;
            }

            if (roomLight != null)
            {
                if (lightPulse != null) StopCoroutine(lightPulse);
                lightPulse = StartCoroutine(PulseLight());
            }

            if (reactionCount == 1) dialogue?.Say(dropped ? "laughing_room_drop" : "laughing_room_fail");
            else if (reactionCount % 3 == 0) dialogue?.Say("laughing_room_escalate");

            if (!exitUnlocked && reactionCount >= reactionsToUnlock)
            {
                exitUnlocked = true;
                linkedDoor?.Unlock();
                dialogue?.Say("laughing_room_open");
            }
        }

        private IEnumerator PulseLight()
        {
            float original = roomLight.intensity;
            roomLight.intensity = original * 0.25f;
            yield return new WaitForSeconds(0.12f);
            roomLight.intensity = original * 1.35f;
            yield return new WaitForSeconds(0.12f);
            roomLight.intensity = original;
            lightPulse = null;
        }
    }
}