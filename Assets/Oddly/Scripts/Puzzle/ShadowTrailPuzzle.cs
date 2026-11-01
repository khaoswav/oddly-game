using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// Mirrors the player's lateral position across the room so their shadow
    /// becomes a second character that must be guided onto three light marks.
    /// </summary>
    public sealed class ShadowTrailPuzzle : MonoBehaviour
    {
        [SerializeField] private Transform shadowVisual;
        [SerializeField] private Transform[] shadowTargets;
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private Light roomLight;
        [SerializeField] private float mirrorAxisZ = 495f;
        [SerializeField] private float targetRadius = 1.35f;

        private FirstPersonController player;
        private int nextTarget;
        private bool active;
        private bool completed;

        public void Begin(FirstPersonController controller)
        {
            if (active || completed || controller == null) return;
            player = controller;
            active = true;
            RefreshStatus();
            dialogue?.Say("hidden_shadow_entry");
        }

        private void Update()
        {
            if (!active || completed || player == null || shadowVisual == null) return;

            Vector3 playerPosition = player.transform.position;
            shadowVisual.position = new Vector3(
                playerPosition.x,
                0.055f,
                mirrorAxisZ - (playerPosition.z - mirrorAxisZ));
            shadowVisual.rotation = Quaternion.Euler(
                0f, -player.transform.eulerAngles.y, 0f);

            if (shadowTargets == null || nextTarget >= shadowTargets.Length) return;
            Transform target = shadowTargets[nextTarget];
            if (target == null) return;

            Vector2 shadowXZ = new Vector2(shadowVisual.position.x, shadowVisual.position.z);
            Vector2 targetXZ = new Vector2(target.position.x, target.position.z);
            if (Vector2.Distance(shadowXZ, targetXZ) > targetRadius) return;

            nextTarget++;
            if (roomLight != null)
            {
                roomLight.color = nextTarget == shadowTargets.Length
                    ? new Color(0.35f, 1f, 0.72f)
                    : Color.Lerp(new Color(0.55f, 0.38f, 1f), new Color(0.35f, 0.85f, 1f),
                        nextTarget / (float)shadowTargets.Length);
            }

            if (nextTarget >= shadowTargets.Length)
            {
                completed = true;
                if (statusSign != null)
                    statusSign.text = dialogue != null && dialogue.IsTurkish
                        ? "GÖLGE SERBEST\nÇIKIŞ AÇIK"
                        : "SHADOW RELEASED\nEXIT OPEN";
                dialogue?.Say("hidden_shadow_complete");
                exitDoor?.Unlock();
            }
            else
            {
                dialogue?.Say("hidden_shadow_target");
                RefreshStatus();
            }
        }

        private void RefreshStatus()
        {
            if (statusSign == null) return;
            bool turkish = dialogue != null && dialogue.IsTurkish;
            statusSign.text = turkish
                ? $"GÖLGENİ IŞIĞA TAŞI\n{nextTarget} / {shadowTargets.Length}"
                : $"GUIDE YOUR SHADOW INTO LIGHT\n{nextTarget} / {shadowTargets.Length}";
        }
    }
}
