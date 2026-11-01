using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// A room that admits it is unfinished. The player completes three
    /// build markers in the order printed on the broken production board.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class UnfinishedGamePuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private Transform[] buildMarks = new Transform[3];
        [SerializeField] private TextMesh[] markLabels = new TextMesh[3];
        [SerializeField] private int[] buildOrder = { 1, 2, 0 };
        [SerializeField, Min(0.2f)] private float markRadius = 0.9f;

        private Transform player;
        private int progress;
        private bool armed = true;
        private bool solved;

        private void OnTriggerEnter(Collider other)
        {
            CharacterController controller = other.GetComponentInParent<CharacterController>();
            if (controller == null) return;
            player = controller.transform;
            UpdateDisplay();
            dialogue?.Say("unfinished_room_entry");
        }

        private void OnTriggerExit(Collider other)
        {
            CharacterController controller = other.GetComponentInParent<CharacterController>();
            if (controller == null || player != controller.transform) return;
            player = null;
            armed = true;
        }

        private void Update()
        {
            if (solved || player == null || buildMarks == null || buildMarks.Length == 0)
                return;

            int expected = buildOrder[Mathf.Clamp(progress, 0, buildOrder.Length - 1)];
            Transform mark = buildMarks[expected];
            if (mark == null) return;

            float distance = Vector3.Distance(
                new Vector3(player.position.x, 0f, player.position.z),
                new Vector3(mark.position.x, 0f, mark.position.z));

            if (distance > markRadius * 1.35f)
                armed = true;

            if (armed && distance <= markRadius)
            {
                armed = false;
                RegisterBuildStep(expected);
            }
        }

        private void RegisterBuildStep(int markIndex)
        {
            if (markIndex != buildOrder[progress])
            {
                progress = 0;
                dialogue?.Say("unfinished_wrong");
                UpdateDisplay();
                return;
            }

            progress++;
            dialogue?.Say(progress == buildOrder.Length ? "unfinished_complete" : "unfinished_step");
            if (progress >= buildOrder.Length)
            {
                solved = true;
                exitDoor?.Unlock();
            }
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            bool turkish = dialogue != null && dialogue.IsTurkish;
            for (int i = 0; i < markLabels.Length; i++)
            {
                if (markLabels[i] == null) continue;
                bool completed = false;
                for (int j = 0; j < progress; j++)
                    if (buildOrder[j] == i) completed = true;
                markLabels[i].text = completed ? (turkish ? "TAMAM" : "DONE") : "TODO";
                markLabels[i].color = completed
                    ? new Color(0.25f, 0.85f, 0.55f)
                    : new Color(0.95f, 0.55f, 0.22f);
            }

            if (statusSign == null) return;
            if (solved)
                statusSign.text = turkish ? "DERLEME TAMAMLANDI / ÇIKIŞ AÇIK" : "BUILD COMPLETE / EXIT UNLOCKED";
            else
                statusSign.text = turkish
                    ? "YAPIM SIRASI: 2 → 3 → 1    (" + progress + "/3)"
                    : "BUILD ORDER: 2 → 3 → 1    (" + progress + "/3)";
        }
    }
}