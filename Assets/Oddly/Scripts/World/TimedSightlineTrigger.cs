using UnityEngine;

namespace Oddly
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class TimedSightlineTrigger : MonoBehaviour
    {
        [SerializeField] private TimedSightlinePuzzle puzzle;
        [SerializeField, Range(0, 2)] private int stationIndex;

        private void Reset() => GetComponent<BoxCollider>().isTrigger = true;

        private void OnTriggerStay(Collider other)
        {
            CharacterController player = other.GetComponent<CharacterController>();
            if (player != null) puzzle?.ObserveStation(stationIndex, player);
        }
    }
}
