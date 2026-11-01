using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A forced-perspective sightline puzzle. The player must stand at a
    /// deliberately chosen viewpoint and aim through separated architectural
    /// fragments so they visually resolve into one complete symbol.
    /// </summary>
    public sealed class SightlinePuzzle : MonoBehaviour
    {
        [SerializeField] private Transform viewpoint;
        [SerializeField] private Transform sightlineTarget;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private DoorController linkedDoor;
        [SerializeField, Min(0.1f)] private float viewpointRadius = 0.85f;
        [SerializeField, Range(0.5f, 12f)] private float aimTolerance = 3.5f;
        [SerializeField, Min(0.1f)] private float requiredDwell = 1.25f;
        [SerializeField] private DialogueDirector dialogue;

        private float alignedSince = -1f;
        private bool completed;

        private void Update()
        {
            if (completed || viewpoint == null || sightlineTarget == null || playerCamera == null)
                return;

            Vector3 playerPosition = playerCamera.transform.position;
            playerPosition.y = viewpoint.position.y;
            Vector3 markerPosition = viewpoint.position;
            markerPosition.y = playerPosition.y;

            if (Vector3.Distance(playerPosition, markerPosition) > viewpointRadius)
            {
                alignedSince = -1f;
                return;
            }

            Vector3 toTarget = sightlineTarget.position - playerCamera.transform.position;
            float angle = Vector3.Angle(playerCamera.transform.forward, toTarget);
            if (angle > aimTolerance)
            {
                alignedSince = -1f;
                return;
            }

            if (alignedSince < 0f) alignedSince = Time.time;
            if (Time.time - alignedSince < requiredDwell) return;

            completed = true;
            linkedDoor?.Unlock();
            dialogue?.Say("sightline_solved");
        }
    }
}
