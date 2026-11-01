using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// The room studies the player's earlier habits and asks them to repeat
    /// the most common one. The answer is derived from this run, not hard-coded.
    /// </summary>
    public sealed class BehaviorMemoryPuzzle : MonoBehaviour
    {
        [SerializeField] private BehaviorTracker behavior;
        [SerializeField] private DoorController linkedDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh dropCountLabel;
        [SerializeField] private TextMesh retryCountLabel;
        [SerializeField] private TextMesh waitCountLabel;

        private int dominantHabit;
        private bool initialized;
        private bool solved;

        private void Start()
        {
            if (behavior == null) behavior = FindObjectOfType<BehaviorTracker>();
            int drops = behavior != null ? behavior.DropCount : 0;
            int retries = behavior != null ? behavior.FailedInteractionCount + behavior.DoorAttemptCount : 0;
            int waits = behavior != null ? behavior.IdleEpisodeCount : 0;

            // Stable tie-breaking keeps the puzzle fair and its answer visible.
            dominantHabit = drops >= retries && drops >= waits ? 0 :
                retries >= waits ? 1 : 2;
            if (dropCountLabel != null) dropCountLabel.text = "DROP\\n" + drops;
            if (retryCountLabel != null) retryCountLabel.text = "RETRY\\n" + retries;
            if (waitCountLabel != null) waitCountLabel.text = "WAIT\\n" + waits;
            initialized = true;
        }

        public void Choose(int habit)
        {
            if (solved || !initialized) return;
            if (habit == dominantHabit)
            {
                solved = true;
                linkedDoor?.Unlock();
                dialogue?.Say("behavior_memory_solved");
            }
            else
            {
                dialogue?.Say("behavior_memory_wrong");
            }
        }

        public int GetCount(int habit)
        {
            if (behavior == null) return 0;
            if (habit == 0) return behavior.DropCount;
            if (habit == 1) return behavior.FailedInteractionCount + behavior.DoorAttemptCount;
            return behavior.IdleEpisodeCount;
        }
    }
}