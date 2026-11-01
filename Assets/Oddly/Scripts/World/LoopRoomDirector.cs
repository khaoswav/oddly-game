using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Sends the player back through a deliberately short architectural loop.
    /// Each traversal changes the room; the third pass breaks the loop.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class LoopRoomDirector : MonoBehaviour
    {
        [SerializeField] private Transform returnPoint;
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh loopSign;
        [SerializeField] private Transform[] changingProps;
        [SerializeField] private Light roomLight;
        [SerializeField, Min(1)] private int loopsRequired = 3;
        [SerializeField, Min(0.1f)] private float cooldown = 1.1f;

        private Vector3[] propPositions;
        private Quaternion[] propRotations;
        private float nextAllowedTime;
        private int completedLoops;
        private bool broken;

        private void Start()
        {
            propPositions = new Vector3[changingProps.Length];
            propRotations = new Quaternion[changingProps.Length];
            for (int i = 0; i < changingProps.Length; i++)
            {
                if (changingProps[i] == null) continue;
                propPositions[i] = changingProps[i].position;
                propRotations[i] = changingProps[i].rotation;
            }
            UpdateSign();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (broken || Time.time < nextAllowedTime) return;
            CharacterController player = other.GetComponentInParent<CharacterController>();
            if (player == null || !player.enabled || returnPoint == null) return;

            completedLoops++;
            nextAllowedTime = Time.time + cooldown;
            player.enabled = false;
            player.transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
            player.enabled = true;

            RearrangeRoom();
            UpdateSign();

            if (completedLoops == 1)
                dialogue?.Say("loop_first");
            else if (completedLoops == 2)
                dialogue?.Say("loop_second");

            if (completedLoops >= loopsRequired)
            {
                broken = true;
                exitDoor?.Unlock();
                if (loopSign != null)
                    loopSign.text = dialogue != null && dialogue.IsTurkish
                        ? "DÖNGÜ KIRILDI / ARTIK GERÇEKTEN İLERLE"
                        : "LOOP BROKEN / NOW ACTUALLY PROCEED";
                dialogue?.Say("loop_broken");
            }
        }

        private void RearrangeRoom()
        {
            for (int i = 0; i < changingProps.Length; i++)
            {
                Transform item = changingProps[i];
                if (item == null) continue;
                float direction = (completedLoops + i) % 2 == 0 ? 1f : -1f;
                item.position = propPositions[i] + new Vector3(direction * (0.45f + 0.25f * completedLoops), 0.1f * completedLoops, 0f);
                item.rotation = propRotations[i] * Quaternion.Euler(0f, 35f * completedLoops, 8f * direction);
            }

            if (roomLight != null)
            {
                roomLight.color = completedLoops % 2 == 0
                    ? new Color(0.35f, 0.65f, 1f)
                    : new Color(1f, 0.38f, 0.48f);
                roomLight.intensity = 0.9f + completedLoops * 0.35f;
            }
        }

        private void UpdateSign()
        {
            if (loopSign == null || broken) return;
            string[] english = { "PASS 0 / THIS IS THE WAY OUT", "PASS 1 / YOU WERE HERE", "PASS 2 / STILL HERE" };
            string[] turkish = { "GEÇİŞ 0 / ÇIKIŞ BU TARAFTA", "GEÇİŞ 1 / BURADAYDIN", "GEÇİŞ 2 / HÂLÂ BURADASIN" };
            int index = Mathf.Clamp(completedLoops, 0, 2);
            loopSign.text = dialogue != null && dialogue.IsTurkish ? turkish[index] : english[index];
        }
    }
}