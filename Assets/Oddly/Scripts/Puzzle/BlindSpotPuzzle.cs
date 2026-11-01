using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A gaze-reversal puzzle: the exit only unlocks while the player stands
    /// on the marked spot and deliberately looks away from it.
    /// </summary>
    public sealed class BlindSpotPuzzle : MonoBehaviour
    {
        [SerializeField] private Transform viewpoint;
        [SerializeField] private Transform exitDirection;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField, Min(0.1f)] private float viewpointRadius = 0.8f;
        [SerializeField, Range(0.1f, 1f)] private float lookAwayDot = 0.72f;
        [SerializeField, Min(0.1f)] private float requiredDwell = 1.6f;
        [SerializeField, Min(0.1f)] private float hintCooldown = 4f;

        private float alignedSince = -1f;
        private float nextHintTime;
        private bool solved;

        private void Update()
        {
            if (solved || viewpoint == null || exitDirection == null || playerCamera == null)
                return;

            Vector3 playerPosition = playerCamera.transform.position;
            Vector3 markerPosition = viewpoint.position;
            playerPosition.y = markerPosition.y;
            bool atMarker = Vector3.Distance(playerPosition, markerPosition) <= viewpointRadius;

            Vector3 toExit = exitDirection.position - playerCamera.transform.position;
            toExit.y = 0f;
            Vector3 gaze = playerCamera.transform.forward;
            gaze.y = 0f;
            float facingExit = toExit.sqrMagnitude > 0.001f && gaze.sqrMagnitude > 0.001f
                ? Vector3.Dot(gaze.normalized, toExit.normalized) : 1f;

            if (!atMarker || facingExit > -lookAwayDot)
            {
                alignedSince = -1f;
                if (Time.time >= nextHintTime)
                {
                    nextHintTime = Time.time + hintCooldown;
                    dialogue?.Say(atMarker ? "blind_spot_look_away" : "blind_spot_find_mark");
                }
                return;
            }

            if (alignedSince < 0f) alignedSince = Time.time;
            if (Time.time - alignedSince < requiredDwell) return;

            solved = true;
            linkedDoor?.Unlock();
            dialogue?.Say("blind_spot_solved");
        }
    }
}