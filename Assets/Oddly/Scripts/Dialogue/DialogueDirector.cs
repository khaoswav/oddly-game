using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oddly
{
    public sealed class DialogueDirector : MonoBehaviour
    {
        public static DialogueDirector Instance { get; private set; }

        [SerializeField] private Text speakerLabel;
        [SerializeField] private Text subtitleLabel;
        [SerializeField] private SystemLanguage language = SystemLanguage.English;

        private readonly Dictionary<string, DialogueLine> lines = new Dictionary<string, DialogueLine>();

        private void Awake()
        {
            Instance = this;
            language = PlayerPrefs.GetInt("oddly.language", 1) == 0
                ? SystemLanguage.Turkish : SystemLanguage.English;
            LoadLines();
        }

        public void SetLanguage(SystemLanguage next)
        {
            language = next;
            PlayerPrefs.SetInt("oddly.language", language == SystemLanguage.Turkish ? 0 : 1);
            PlayerPrefs.Save();
        }

        public void Say(string id)
        {
            if (!lines.TryGetValue(id, out DialogueLine line)) return;
            if (speakerLabel != null) speakerLabel.text = line.speaker;
            if (subtitleLabel != null) subtitleLabel.text = line.Resolve(language);
        }

        private void LoadLines()
        {
            Add("intro", "ANLATICI", "Hoş geldin. Test çok kolay. Hatta senin için biraz fazla kolay olabilir.", "Welcome. This test is easy. Possibly a little too easy for you.");
            Add("picked_up", "SYSTEM-02", "Nesne alındı. Lütfen onu bir daha kaybetmeyin. Geçen sefer kötü olmuştu.", "Object acquired. Please do not lose it again. Last time was embarrassing.");
            Add("door_locked", "ANLATICI", "Kapı açılmıyor mu? Çok şaşırdım. Gerçekten.", "The door won't open? I'm absolutely shocked. Truly.");
            Add("door_unlocked", "SYSTEM-02", "Bulmaca çözüldü. Beklenmedik bir gelişme.", "Puzzle solved. An unexpected development.");
            Add("door_open", "ANLATICI", "Açıldı! Bak, bazen sana güvenmek işe yarıyor.", "It opened! See? Sometimes trusting you works.");
            Add("puzzle_solved", "ANLATICI", "Güzel. Küpü doğru yere koydun. Bilim ağlıyor.", "Nice. You put the cube in the right place. Science is weeping.");
            Add("object_too_big", "SYSTEM-02", "Küp biraz iri. Küçültmeyi dene. Evet, bu oyunda bunu yapabiliyorsun.", "The cube is a little oversized. Try shrinking it. Yes, you can do that in this game.");
            Add("object_too_small", "SYSTEM-02", "Bu kez fazla küçüldü. Biraz büyüt. Ölçü diye bir şey var.", "That is a little too tiny. Make it bigger. Measurements exist for a reason.");
            Add("perspective_align", "SYSTEM-02", "Küpü çerçevenin tam ortasına getir. Hayır, göz kararı değil. Biraz daha dikkat.", "Line the cube up with the frame. No, not approximately. Try looking properly.");
            Add("perspective_size", "ANLATICI", "Konum fena değil. Şimdi ekranda kapladığı boyutu çerçeveyle eşleştir.", "Not bad positioning. Now match its apparent size to the frame.");
            Add("perspective_solved", "SYSTEM-02", "Görüş açısı doğrulandı. Fizik kuralları itiraz dilekçesi hazırlıyor.", "Viewpoint confirmed. The laws of physics are drafting a complaint.");
            Add("idle", "SYSTEM-02", "Kullanıcı hareketsiz. Düşünme işlemi devam ediyor olabilir.", "User is stationary. A thinking process may be occurring.");
            Add("repeat", "ANLATICI", "Aynı şeyi tekrar denemek cesur bir strateji. Yanlış, ama cesur.", "Repeating the same thing is a bold strategy. Wrong, but bold.");
            Add("failed", "SYSTEM-02", "Etkileşim algılanmadı. Parmağınızdan şüpheleniyorum.", "No interaction detected. I am beginning to suspect your finger.");
        }

        private void Add(string id, string speaker, string tr, string en)
        {
            lines[id] = new DialogueLine { id = id, speaker = speaker, turkish = tr, english = en };
        }
    }
}
