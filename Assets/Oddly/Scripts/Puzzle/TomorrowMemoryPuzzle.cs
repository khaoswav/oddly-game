using UnityEngine;

namespace Oddly
{
    public sealed class TomorrowMemoryPuzzle : MonoBehaviour
    {
        [SerializeField] private Transform[] memoryBridges = new Transform[3];
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private Light roomLight;
        [SerializeField] private float activeBridgeHeight = 0.12f;
        [SerializeField] private float hiddenBridgeHeight = -3.5f;

        private readonly string[] routeNames = { "DROP", "RETRY", "WAIT" };
        private readonly string[] routeNamesTurkish = { "DÜŞÜR", "TEKRAR DENE", "BEKLE" };
        private int selectedRoute = -1;
        private int recordedDrops;
        private int recordedRetries;
        private int recordedWaits;
        private bool initialized;
        private bool completed;

        public void InitializeFromHistory(BehaviorTracker history)
        {
            if (initialized) return;
            initialized = true;

            int drops = history != null ? history.DropCount : 0;
            int retries = history != null
                ? history.FailedInteractionCount + history.DoorAttemptCount : 0;
            int waits = history != null ? history.IdleEpisodeCount : 0;
            recordedDrops = drops;
            recordedRetries = retries;
            recordedWaits = waits;

            // A clean run defaults to WAIT. Ties resolve deterministically: DROP, then RETRY, then WAIT.
            selectedRoute = drops > 0 && drops >= retries && drops >= waits ? 0
                : retries > 0 && retries >= waits ? 1
                : waits > 0 ? 2 : 2;

            for (int i = 0; i < memoryBridges.Length; i++)
            {
                Transform bridge = memoryBridges[i];
                if (bridge == null) continue;
                Vector3 position = bridge.position;
                position.y = i == selectedRoute ? activeBridgeHeight : hiddenBridgeHeight;
                bridge.position = position;
            }

            if (roomLight != null)
            {
                roomLight.color = selectedRoute == 0 ? new Color(1f, 0.48f, 0.24f)
                    : selectedRoute == 1 ? new Color(0.38f, 0.65f, 1f)
                    : new Color(0.72f, 0.48f, 1f);
                roomLight.intensity = 1.5f;
            }

            if (dialogue != null)
                dialogue.Say(selectedRoute == 0 ? "tomorrow_route_drop"
                    : selectedRoute == 1 ? "tomorrow_route_retry" : "tomorrow_route_wait");
            RefreshStatus();
        }

        public void ReachExit()
        {
            if (!initialized || completed) return;
            completed = true;
            if (statusSign != null)
                statusSign.text = dialogue != null && dialogue.IsTurkish
                    ? "GEÇMİŞİN YOLU AÇILDI / ÇIKIŞ AÇIK"
                    : "YOUR PAST MADE A WAY / EXIT OPEN";
            dialogue?.Say("tomorrow_memory_complete");
            exitDoor?.Unlock();
        }

        private void RefreshStatus()
        {
            if (statusSign == null || selectedRoute < 0) return;
            bool turkish = dialogue != null && dialogue.IsTurkish;
            string route = turkish ? routeNamesTurkish[selectedRoute] : routeNames[selectedRoute];
            statusSign.text = turkish
                ? $"YARININ YOLU: {route}\nDÜŞÜR {recordedDrops} / DENE {recordedRetries} / BEKLE {recordedWaits}"
                : $"TOMORROW'S ROUTE: {route}\nDROP {recordedDrops} / RETRY {recordedRetries} / WAIT {recordedWaits}";
        }
    }
}
