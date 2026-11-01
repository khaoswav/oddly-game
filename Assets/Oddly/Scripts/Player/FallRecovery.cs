using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Recovers the player from unfinished space and keeps the latest chapter
    /// entry as a lightweight checkpoint.
    /// </summary>
    [RequireComponent(typeof(CharacterController), typeof(FirstPersonController))]
    public sealed class FallRecovery : MonoBehaviour
    {
        [SerializeField, Min(-100f)] private float fallThreshold = -8f;
        [SerializeField, Min(0.1f)] private float recoveryCooldown = 1f;
        [SerializeField] private DialogueDirector dialogue;

        private CharacterController character;
        private FirstPersonController movement;
        private Vector3 checkpointPosition;
        private Quaternion checkpointRotation;
        private float nextRecoveryTime;

        private void Awake()
        {
            character = GetComponent<CharacterController>();
            movement = GetComponent<FirstPersonController>();
            checkpointPosition = transform.position;
            checkpointRotation = transform.rotation;
            if (PlayerPrefs.GetInt("oddly.campaign.checkpoint.saved", 0) == 1)
            {
                checkpointPosition = new Vector3(
                    PlayerPrefs.GetFloat("oddly.campaign.checkpoint.position.0", transform.position.x),
                    PlayerPrefs.GetFloat("oddly.campaign.checkpoint.position.1", transform.position.y),
                    PlayerPrefs.GetFloat("oddly.campaign.checkpoint.position.2", transform.position.z));
                checkpointRotation = Quaternion.Euler(0f,
                    PlayerPrefs.GetFloat("oddly.campaign.checkpoint.rotation.1", transform.eulerAngles.y), 0f);
                transform.SetPositionAndRotation(checkpointPosition, checkpointRotation);
            }
        }

        public void SetCheckpoint(Transform checkpoint)
        {
            if (checkpoint == null) return;
            checkpointPosition = checkpoint.position;
            checkpointRotation = checkpoint.rotation;
            PlayerPrefs.SetFloat("oddly.campaign.checkpoint.position.0", checkpointPosition.x);
            PlayerPrefs.SetFloat("oddly.campaign.checkpoint.position.1", checkpointPosition.y);
            PlayerPrefs.SetFloat("oddly.campaign.checkpoint.position.2", checkpointPosition.z);
            PlayerPrefs.SetFloat("oddly.campaign.checkpoint.rotation.1", checkpointRotation.eulerAngles.y);
            PlayerPrefs.SetInt("oddly.campaign.checkpoint.saved", 1);
            PlayerPrefs.Save();
        }

        public void SetDialogue(DialogueDirector director)
        {
            dialogue = director;
        }

        private void Update()
        {
            if (transform.position.y >= fallThreshold || Time.time < nextRecoveryTime) return;

            nextRecoveryTime = Time.time + recoveryCooldown;
            character.enabled = false;
            transform.SetPositionAndRotation(checkpointPosition, checkpointRotation);
            character.enabled = true;
            movement.ResetMotion();
            dialogue?.Say("fall_recover");
        }
    }
}