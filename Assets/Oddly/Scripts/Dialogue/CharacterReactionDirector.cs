using System.Collections;
using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Turns repeated player habits into escalating, character-specific banter.
    /// Reactions are event-driven and never interfere with puzzle logic.
    /// </summary>
    [RequireComponent(typeof(BehaviorTracker))]
    public sealed class CharacterReactionDirector : MonoBehaviour
    {
        [SerializeField] private BehaviorTracker tracker;
        [SerializeField] private DialogueDirector dialogue;

        private int pickups;
        private int drops;
        private int failedActions;
        private int doorAttempts;
        private int idleEpisodes;
        private float lastReactionAt = -100f;
        [SerializeField, Min(0f)] private float reactionCooldown = 4.5f;

        private void Awake()
        {
            if (tracker == null) tracker = GetComponent<BehaviorTracker>();
        }

        private void Start()
        {
            if (dialogue == null) dialogue = DialogueDirector.Instance;
            if (tracker == null) return;
            tracker.ObjectPickedUp += OnPickup;
            tracker.ObjectDropped += OnDrop;
            tracker.FailedInteractionRegistered += OnFailedInteraction;
            tracker.DoorAttemptRegistered += OnDoorAttempt;
            tracker.IdleEpisodeRegistered += OnIdleEpisode;
        }

        private void QueueReaction(string dialogueId) => StartCoroutine(PlayReactionAfterCurrentBeat(dialogueId));

        private IEnumerator PlayReactionAfterCurrentBeat(string dialogueId)
        {
            yield return new WaitForSecondsRealtime(0.22f);
            if (CanReact()) dialogue.Say(dialogueId);
        }

        private bool CanReact()
        {
            if (dialogue == null || Time.unscaledTime - lastReactionAt < reactionCooldown)
                return false;
            lastReactionAt = Time.unscaledTime;
            return true;
        }

        private void OnPickup()
        {
            pickups++;
            if (pickups == 5)
                QueueReaction("reaction_pickup_habit");
        }

        private void OnDrop()
        {
            drops++;
            if (drops == 1)
                QueueReaction("reaction_first_drop");
            else if (drops == 3)
                QueueReaction("reaction_drop_three");
            else if (drops == 7)
                QueueReaction("reaction_drop_seven");
        }

        private void OnFailedInteraction()
        {
            failedActions++;
            if (failedActions == 2)
                QueueReaction("reaction_second_miss");
            else if (failedActions == 5)
                QueueReaction("reaction_persistent_miss");
        }

        private void OnDoorAttempt()
        {
            doorAttempts++;
            if (doorAttempts == 3)
                QueueReaction("reaction_door_three");
            else if (doorAttempts == 6)
                QueueReaction("reaction_door_six");
        }

        private void OnIdleEpisode()
        {
            idleEpisodes++;
            if (idleEpisodes == 2)
                QueueReaction("reaction_idle_twice");
            else if (idleEpisodes == 4)
                QueueReaction("reaction_idle_four");
        }

        private void OnDisable()
        {
            if (tracker == null) return;
            tracker.ObjectPickedUp -= OnPickup;
            tracker.ObjectDropped -= OnDrop;
            tracker.FailedInteractionRegistered -= OnFailedInteraction;
            tracker.DoorAttemptRegistered -= OnDoorAttempt;
            tracker.IdleEpisodeRegistered -= OnIdleEpisode;
        }
    }
}
