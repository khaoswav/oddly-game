using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// The audience only applauds when the player refuses to perform for it:
    /// stand in the spotlight and turn their back to the seats.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class WrongAudiencePuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh stageSign;
        [SerializeField] private Transform[] audienceMembers;
        [SerializeField] private Light stageLight;
        [SerializeField, Min(0.5f)] private float holdDuration = 2.2f;
        [SerializeField, Range(-1f, 1f)] private float facingThreshold = -0.65f;

        private Transform player;
        private float turnedAwayTime;
        private bool solved;

        private void Update()
        {
            if (solved || player == null) return;

            float facingAudience = Vector3.Dot(player.forward, Vector3.forward);
            if (facingAudience <= facingThreshold)
            {
                turnedAwayTime += Time.deltaTime;
                UpdateSign();
                if (turnedAwayTime >= holdDuration)
                    Solve();
            }
            else
            {
                turnedAwayTime = 0f;
                UpdateSign();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            CharacterController controller = other.GetComponentInParent<CharacterController>();
            if (controller == null) return;
            player = controller.transform;
            if (!solved) dialogue?.Say("audience_notice");
        }

        private void OnTriggerExit(Collider other)
        {
            CharacterController controller = other.GetComponentInParent<CharacterController>();
            if (controller == null || player != controller.transform) return;
            player = null;
            turnedAwayTime = 0f;
            UpdateSign();
        }

        private void Solve()
        {
            solved = true;
            exitDoor?.Unlock();

            for (int i = 0; i < audienceMembers.Length; i++)
            {
                if (audienceMembers[i] == null) continue;
                audienceMembers[i].rotation *= Quaternion.Euler(0f, 180f, 0f);
            }

            if (stageLight != null)
            {
                stageLight.color = new Color(0.35f, 0.85f, 1f);
                stageLight.intensity = 0.45f;
            }

            if (stageSign != null)
                stageSign.text = dialogue != null && dialogue.IsTurkish
                    ? "AYAKTA ALKIŞ / LÜTFEN SAHNEYİ TERK EDİN"
                    : "STANDING OVATION / PLEASE LEAVE THE STAGE";

            dialogue?.Say("audience_solved");
        }

        private void UpdateSign()
        {
            if (stageSign == null || solved) return;
            if (player == null)
            {
                stageSign.text = dialogue != null && dialogue.IsTurkish
                    ? "SAHNEYE ÇIK / SEYİRCİYE SIRTINI DÖN"
                    : "STEP INTO THE SPOTLIGHT / TURN YOUR BACK";
                return;
            }

            float progress = Mathf.Clamp01(turnedAwayTime / holdDuration);
            int percent = Mathf.RoundToInt(progress * 100f);
            stageSign.text = dialogue != null && dialogue.IsTurkish
                ? "SEYİRCİYİ GÖRMEZDEN GEL: %" + percent
                : "IGNORE THE AUDIENCE: " + percent + "%";
        }
    }
}