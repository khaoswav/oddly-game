using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oddly
{
    public sealed class DialogueDirector : MonoBehaviour
    {
        public static DialogueDirector Instance { get; private set; }
        public bool IsTurkish => language == SystemLanguage.Turkish;

        [SerializeField] private Text speakerLabel;
        [SerializeField] private Text subtitleLabel;
        [SerializeField] private SystemLanguage language = SystemLanguage.English;
        [SerializeField, Min(0f)] private float repeatedLineCooldown = 3f;

        private readonly Dictionary<string, List<DialogueLine>> lines =
            new Dictionary<string, List<DialogueLine>>();
        private readonly Dictionary<string, float> lastSpokenAt = new Dictionary<string, float>();
        private readonly Dictionary<string, int> lastVariant = new Dictionary<string, int>();

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
            if (!lines.TryGetValue(id, out List<DialogueLine> variants) || variants.Count == 0)
                return;

            if (lastSpokenAt.TryGetValue(id, out float previousTime) &&
                Time.unscaledTime - previousTime < repeatedLineCooldown)
                return;

            int selected = variants.Count == 1 ? 0 : Random.Range(0, variants.Count);
            if (variants.Count > 1 && lastVariant.TryGetValue(id, out int previousVariant) &&
                selected == previousVariant)
                selected = (selected + 1) % variants.Count;

            lastSpokenAt[id] = Time.unscaledTime;
            lastVariant[id] = selected;
            DialogueLine line = variants[selected];
            if (speakerLabel != null) speakerLabel.text = line.speaker;
            if (subtitleLabel != null) subtitleLabel.text = line.Resolve(language);
        }

        private void LoadLines()
        {
            Add("chapter_welcome", "ANLATICI", "Hoş geldin! ODDLY Test Merkezi'ne giriş yaptın. Ayakkabılarını çıkarmana gerek yok. Zaten ayakkabılarını göremiyorum.", "Welcome to the ODDLY Testing Facility! No need to remove your shoes. I can't see them anyway.");
            Add("chapter_system", "SYSTEM-02", "Güvenlik bilgilendirmesi: Çıkış tabelalarına güvenmeyin. Önceki kullanıcılar hâlâ çıkışı arıyor.", "Safety briefing: Do not trust exit signs. Previous users are still looking for the exit.");
            Add("chapter_narrator", "ANLATICI", "Abartıyor. Burada kaybolmak imkânsız. Bina sadece... biraz esnek bir fikre sahip.", "That's an exaggeration. Getting lost here is impossible. The building just has... a flexible understanding of space.");
            Add("chapter_go", "SYSTEM-02", "Kontroller etkin. Lütfen test alanına ilerleyin. Ve lütfen hiçbir şeye basmayın. Özellikle kırmızı şeylere.", "Controls enabled. Please proceed into the test area. And please do not press anything. Especially red things.");
            Add("false_exit", "ANLATICI", "Tabelayı değiştirdiler. Bu, çıkışın yerini değiştirdikleri anlamına gelmiyor. Sanırım.", "They changed the sign. That does not mean they moved the exit. I think.");
            Add("chapter_reveal", "SYSTEM-02", "Gizli geçit açıldı. Lütfen bekleme odasına geçin. Beklemek zorunda değilsiniz. Şimdilik.", "Hidden passage opened. Please proceed to the waiting room. You are not required to wait. For now.");
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
            Add("impossible_room", "ANLATICI", "Bir dakika... Az önce nereden geldin? Boş ver. Ben de artık takip etmiyorum.", "Wait... where did you just come from? Never mind. I've stopped keeping track too.");
            Add("meta_notice", "SYSTEM-02", "UYARI: KIRMIZI DÜĞMEYE BASMAYIN. Bu uyarıyı okuduğunuz kaydedildi.", "NOTICE: DO NOT PRESS THE RED BUTTON. Your reading of this notice has been recorded.");
            Add("meta_wrong", "ANLATICI", "Önce bir şeyi fark etmen gerekiyordu. Tabela süs olsun diye burada değil.", "You were supposed to notice something first. The sign isn't here for decoration.");
            Add("meta_solved", "SYSTEM-02", "Talimat ihlali doğrulandı. Kapı açılabilir. Kurallarımızı okuduğunuz için teşekkürler.", "Instruction violation confirmed. Door access granted. Thank you for reading our rules.");
            Add("idle", "SYSTEM-02", "Kullanıcı hareketsiz. Düşünme işlemi devam ediyor olabilir.", "User is stationary. A thinking process may be occurring.");
            AddVariant("idle", "ANLATICI", "Düşünüyorsan acele etme. Benim de yapacak daha iyi bir işim yok.", "If you're thinking, take your time. I don't have anything better to do.");
            AddVariant("idle", "SYSTEM-02", "Hareketsizlik süresi uzadı. Kullanıcı dekorun parçası olmuş olabilir.", "Inactivity prolonged. The user may have become part of the decor.");
            Add("repeat", "ANLATICI", "Aynı şeyi tekrar denemek cesur bir strateji. Yanlış, ama cesur.", "Repeating the same thing is a bold strategy. Wrong, but bold.");
            AddVariant("repeat", "SYSTEM-02", "Tekrar algılandı. Sonuç değişmedi. Şaşırtıcı değil.", "Repetition detected. Result unchanged. Not surprising.");
            AddVariant("repeat", "ANLATICI", "Bir kez daha yaparsan bu sefer kesin farklı olacak. Muhtemelen.", "Do it one more time and it'll definitely work. Probably.");
            Add("failed", "SYSTEM-02", "Etkileşim algılanmadı. Parmağınızdan şüpheleniyorum.", "No interaction detected. I am beginning to suspect your finger.");
            AddVariant("failed", "ANLATICI", "Orada bir şey yok. Ama kararlılığını takdir ediyorum.", "There's nothing there. But I admire the commitment.");
        }

        private void Add(string id, string speaker, string tr, string en)
        {
            AddVariant(id, speaker, tr, en);
        }

        private void AddVariant(string id, string speaker, string tr, string en)
        {
            if (!lines.TryGetValue(id, out List<DialogueLine> variants))
            {
                variants = new List<DialogueLine>();
                lines.Add(id, variants);
            }

            variants.Add(new DialogueLine
            {
                id = id,
                speaker = speaker,
                turkish = tr,
                english = en
            });
        }
    }
}
