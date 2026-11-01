using UnityEngine;

namespace Oddly
{
    /// <summary>
    /// The final hidden room recalls the player's saved finale choice and
    /// completed hidden rooms. Choose the ending the archive remembers.
    /// </summary>
    public sealed class RememberedSelfPuzzle : MonoBehaviour
    {
        [SerializeField] private DoorController exitDoor;
        [SerializeField] private DialogueDirector dialogue;
        [SerializeField] private TextMesh statusSign;
        [SerializeField] private TextMesh[] memoryLabels = new TextMesh[3];
        [SerializeField] private Light roomLight;

        private int rememberedEnding;
        private int completedRooms;
        private bool solved;
        private bool active;

        private void Start()
        {
            rememberedEnding = Mathf.Clamp(PlayerPrefs.GetInt("oddly.ending", 2), 0, 2);
            completedRooms = HiddenChapterArchive.CompletedCount;
            RefreshStatus();
        }

        public void Begin()
        {
            if (active || solved) return;
            active = true;
            dialogue?.Say("hidden_memory_entry");
            RefreshStatus();
        }

        public void Choose(int endingIndex)
        {
            if (!active || solved || endingIndex < 0 || endingIndex > 2) return;
            if (endingIndex == rememberedEnding)
            {
                solved = true;
                HiddenChapterArchive.MarkCompleted(8);
                if (roomLight != null)
                {
                    roomLight.color = new Color(0.35f, 1f, 0.72f);
                    roomLight.intensity = 1.8f;
                }
                if (statusSign != null)
                    statusSign.text = dialogue != null && dialogue.IsTurkish
                        ? $"HAFIZA DOĞRULANDI\n{completedRooms}/8 ODA HATIRLANDI\nÇIKIŞ AÇIK"
                        : $"MEMORY VERIFIED\n{completedRooms}/8 ROOMS REMEMBERED\nEXIT OPEN";
                dialogue?.Say("hidden_memory_complete");
                exitDoor?.Unlock();
            }
            else
            {
                dialogue?.Say("hidden_memory_wrong");
                if (roomLight != null)
                {
                    roomLight.color = new Color(1f, 0.32f, 0.38f);
                    roomLight.intensity = 1.15f;
                }
            }
        }

        private void RefreshStatus()
        {
            string[] en = { "THE STORYTELLER", "THE SYSTEM", "THE PLAYER" };
            string[] tr = { "ANLATICI", "SİSTEM", "OYUNCU" };
            for (int i = 0; i < memoryLabels.Length; i++)
            {
                if (memoryLabels[i] == null) continue;
                memoryLabels[i].text = dialogue != null && dialogue.IsTurkish ? tr[i] + "\\n[E]" : en[i] + "\\n[ E ]";
            }
            if (statusSign != null)
                statusSign.text = dialogue != null && dialogue.IsTurkish
                    ? $"ODA SENİ HATIRLIYOR\n{completedRooms}/8 GİZLİ ODA TAMAMLANDI\nSON KARARINI BUL"
                    : $"THE ROOM REMEMBERS YOU\n{completedRooms}/8 HIDDEN ROOMS CLEARED\nFIND YOUR LAST CHOICE";
        }
    }
}
