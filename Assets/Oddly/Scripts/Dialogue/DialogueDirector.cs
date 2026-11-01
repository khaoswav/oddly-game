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
            Add("perspective_grab_scaled", "ANLATICI", "Bir dakika... Elindeyken boyutu değişti. Mesafeyi hesaba katmayı unutmuşsun.", "Wait... it changed size when you picked it up. Distance matters. Who knew?");
            Add("door_locked", "ANLATICI", "Kapı açılmıyor mu? Çok şaşırdım. Gerçekten.", "The door won't open? I'm absolutely shocked. Truly.");
            Add("door_unlocked", "SYSTEM-02", "Bulmaca çözüldü. Beklenmedik bir gelişme.", "Puzzle solved. An unexpected development.");
            Add("door_open", "ANLATICI", "Açıldı! Bak, bazen sana güvenmek işe yarıyor.", "It opened! See? Sometimes trusting you works.");
            Add("puzzle_solved", "ANLATICI", "Güzel. Küpü doğru yere koydun. Bilim ağlıyor.", "Nice. You put the cube in the right place. Science is weeping.");
            Add("object_too_big", "SYSTEM-02", "Küp biraz iri. Küçültmeyi dene. Evet, bu oyunda bunu yapabiliyorsun.", "The cube is a little oversized. Try shrinking it. Yes, you can do that in this game.");
            Add("object_too_small", "SYSTEM-02", "Bu kez fazla küçüldü. Biraz büyüt. Ölçü diye bir şey var.", "That is a little too tiny. Make it bigger. Measurements exist for a reason.");
            Add("perspective_align", "SYSTEM-02", "Küpü çerçevenin tam ortasına getir. Hayır, göz kararı değil. Biraz daha dikkat.", "Line the cube up with the frame. No, not approximately. Try looking properly.");
            Add("perspective_size", "ANLATICI", "Konum fena değil. Şimdi ekranda kapladığı boyutu çerçeveyle eşleştir.", "Not bad positioning. Now match its apparent size to the frame.");
            Add("perspective_solved", "SYSTEM-02", "Görüş açısı doğrulandı. Fizik kuralları itiraz dilekçesi hazırlıyor.", "Viewpoint confirmed. The laws of physics are drafting a complaint.");
            Add("dual_scale_small_shrink", "SYSTEM-02", "Küçük platform küçük bir nesne istiyor. Biraz daha yakına getir ya da boyutunu küçült.", "The small pad needs a smaller object. Bring it closer or reduce its scale.");
            Add("dual_scale_small_grow", "ANLATICI", "Bu kadar da minik olmasın. Küçük platformun bile standartları var.", "Not that tiny. Even the small pad has standards.");
            Add("dual_scale_large_shrink", "ANLATICI", "Büyük platforma geldin ama nesne henüz yeterince büyük değil. Biraz uzaklaştır.", "You found the big pad, but the object isn't big enough yet. Move it farther away.");
            Add("dual_scale_large_grow", "SYSTEM-02", "Bu nesne fazla büyük. Büyük platformun da bir sınırı var.", "That object is too large. The large pad has limits too.");
            Add("dual_scale_solved", "ANLATICI", "İki nesne, iki boyut, tek kapı. Matematik sonunda bir işe yaradı.", "Two objects, two sizes, one door. Mathematics finally did something useful.");
            Add("sightline_solved", "SYSTEM-02", "Doğru noktadan baktınız. Mimari artık sizi tanıyor. Lütfen bunu kişisel algılamayın.", "You looked from the correct point. The architecture recognises you now. Please do not take that personally.");
            Add("sightline_adjust", "ANLATICI", "İşaretin üstündesin. Şimdi iki parçayı tek bir şekil gibi üst üste getir. Kafanı biraz oynat, dünyayı değil.", "You found the mark. Now overlap the two fragments into one shape. Move your head a little, not the world.");
            Add("chapter_two_entry", "SYSTEM-02", "BÖLÜM 02: Boyut Önemlidir. Önünüzdeki boşluk tasarım hatası değildir. En azından bu kez değil.", "CHAPTER 02: Size Does Matter. The gap ahead is not a design error. At least not this time.");
            AddVariant("chapter_two_entry", "ANLATICI", "İyi haber: Artık boyutları değiştirebiliyorsun. Kötü haber: Köprü bütçemiz sıfır.", "Good news: You can change sizes now. Bad news: our bridge budget is zero.");
            Add("bridge_grow", "SYSTEM-02", "Bu tahta karşıya geçecek kadar uzun değil. Uzaklaştır ve biraz daha büyüt.", "That plank is not long enough to cross. Move it farther away and make it bigger.");
            Add("bridge_shrink", "ANLATICI", "Köprü değil, duvar yaptın. Biraz küçült. Karşıya geçmek istiyoruz, binayı kapatmak değil.", "You made a wall, not a bridge. Shrink it a little. We need to cross, not renovate the building.");
            Add("bridge_settle", "SYSTEM-02", "Bırak ve bekle. Köprü olmayı kabul etmesi için tahtaya bir saniye ver.", "Drop it and wait. Give the plank a moment to accept its new career as a bridge.");
            Add("bridge_align", "ANLATICI", "Boyu tamam. Şimdi boşluğun tam ortasına yerleştir. Hassasiyet, lütfen.", "The length is fine. Now place it right across the middle of the gap. Precision, please.");
            Add("bridge_solved", "SYSTEM-02", "Geçiş doğrulandı. Bir tahta, iki platform ve sıfır inşaat ruhsatı.", "Crossing verified. One plank, two platforms, and absolutely no building permit.");
            Add("fall_recover", "ANLATICI", "Aşağısı biraz fazla aşağıymış. Seni son güvenli noktaya aldık. Rica ederim.", "Turns out down was a little too far down. We put you back at the last safe spot. You are welcome.");
            Add("chapter_three_entry", "SYSTEM-02", "BÖLÜM 03: Yanlış Boyut. Duvarın arkasında bir hedef var. Senin için değil. Şimdilik.", "CHAPTER 03: Wrong Dimension. There is a target behind that wall. Not for you. For now.");
            AddVariant("chapter_three_entry", "ANLATICI", "Kapı yok. Geçit yok. Sadece küçük bir delik. Sonunda mimari bütçemiz dengelendi.", "No door. No passage. Just a tiny slot. At last, our architecture budget is balanced.");
            Add("dimension_slot_grow", "SYSTEM-02", "Nesne bu açıklıktan geçemeyecek kadar küçük. Biraz büyüt. Evet, küçük de olsa fazla küçük olabilir.", "The object is too small to register beyond the slot. Enlarge it a little. Yes, something can be too small even for a small hole.");
            Add("dimension_slot_shrink", "ANLATICI", "Bu delikten geçmeye çalışmıyor, duvarı taşımaya çalışıyor. Biraz küçült.", "It's not trying to fit through the slot. It's trying to move the wall. Shrink it.");
            Add("dimension_slot_drop", "SYSTEM-02", "Nesneyi açıklığın ötesine taşı ve bırak. Senin geçmen gerekmiyor.", "Move the object through the opening and drop it. You do not need to fit.");
            Add("dimension_slot_align", "ANLATICI", "Nesne doğru boyutta ama hedefi ıskaladın. Duvarın arkasındaki platforma bırak.", "The size is right, but you missed the target. Drop it onto the platform beyond the wall.");
            Add("dimension_slot_settle", "SYSTEM-02", "Hedefe yakın. Bırak ve fiziğin işini bitirmesini bekle.", "Close to the target. Let go and wait for physics to finish its paperwork.");
            Add("dimension_slot_solved", "ANLATICI", "Tebrikler. Sen geçemedin ama küp geçti. Takım çalışması böyle bir şey.", "Congratulations. You couldn't fit, but the cube did. That's what teamwork looks like.");
            Add("chapter_four_entry", "SYSTEM-02", "BÖLÜM 04: Sistem Hatası. Lütfen yalnızca benim talimatlarımı izleyin. Anlatıcı güvenilir değildir.", "CHAPTER 04: System Error. Please follow my instructions only. The Narrator is unreliable.");
            AddVariant("chapter_four_entry", "ANLATICI", "Bana bakma. Ben de ne dediğini bilmiyorum. Mavi kutulara basma, kırmızı olanlara bas. Ya da tam tersi.", "Don't look at me. I have no idea what it's saying either. Don't press the blue ones. Press the red ones. Or the other way around.");
            Add("system_error_first", "ANLATICI", "Birinci adım tamam. Şimdi sakın SYSTEM-02'yi dinleme. Özellikle şimdi.", "First step done. Now do not listen to SYSTEM-02. Especially now.");
            Add("system_error_second", "SYSTEM-02", "İkinci giriş kaydedildi. Bu işlem benim talimatlarımla uyuşmuyor. İptal etmeye çalışıyorum.", "Second input recorded. This does not match my instructions. Attempting to cancel.");
            Add("system_error_wrong", "SYSTEM-02", "Sıra hatalı. Giriş dizisi sıfırlandı. Bu kez doğru sırayı izleyin.", "Sequence mismatch. Input sequence reset. Please follow the correct order this time.");
            Add("system_error_solved", "ANLATICI", "Açıldı! Demek ki beni dinlemeliydin. Bunu her zaman söylemiyorum. Sadece çoğu zaman.", "It opened! So you should have listened to me. I don't say that often. Just most of the time.");
            Add("chapter_five_entry", "ANLATICI", "BÖLÜM 05: Gözlerine Güvenme. Çıkış tam karşında. Sakın başka tarafa bakma. Ciddiyim.", "CHAPTER 05: Don't Trust Your Eyes. The exit is right in front of you. Do not look anywhere else. I'm serious.");
            AddVariant("chapter_five_entry", "SYSTEM-02", "Görsel doğrulama başlatıldı. Lütfen görünen çıkışa odaklanın. Bu kez gerçekten.", "Visual verification started. Please focus on the visible exit. For real this time.");
            Add("blind_spot_find_mark", "SYSTEM-02", "Zemindeki işarete geçin. Görüş doğrulaması için konumunuz önemli.", "Move to the floor marker. Your position matters for visual verification.");
            Add("blind_spot_look_away", "ANLATICI", "İşarettesin. Şimdi çıkışa bakmayı bırak. Evet, tam tersini söylüyorum. Ben de farkındayım.", "You're on the mark. Now stop looking at the exit. Yes, I'm telling you the exact opposite. I'm aware.");
            Add("blind_spot_solved", "SYSTEM-02", "Görüş doğrulaması tamamlandı. Çıkış, kullanıcı bakmadığı sırada açıldı. Bu tasarım kararı raporlanmayacak.", "Visual verification complete. The exit opened while the user was not looking. This design decision will not be documented.");
            Add("chapter_six_entry", "SYSTEM-02", "BÖLÜM 06: Yine Sen? Önceki davranışların kaydedildi. Gizlilik politikasını birazdan güncelleyeceğiz.", "CHAPTER 06: You Again? Your previous behaviour has been recorded. We will update the privacy policy shortly.");
            AddVariant("chapter_six_entry", "ANLATICI", "Seni tanıdığımızı söylemek istemiyorum. Ama şu ana kadar kaç şeyi yere bıraktığını biliyoruz.", "I don't want to say we know you. But we do know how many things you've dropped so far.");
            Add("behavior_memory_wrong", "ANLATICI", "Güzel tahmin. Ama bu odanın hafızası senden biraz daha iyi.", "Nice guess. But this room has a slightly better memory than you do.");
            Add("behavior_memory_solved", "SYSTEM-02", "Alışkanlık doğrulandı. Kullanıcı kendini tanıyor. Bu gelişme kayda değer.", "Habit verified. The user recognises themself. This is a noteworthy development.");
            Add("chapter_seven_entry", "ANLATICI", "BÖLÜM 07: Gülen Oda. Burada hiçbir şey kişisel değil. Özellikle birazdan olacaklar.", "CHAPTER 07: The Room That Laughs. Nothing here is personal. Especially what happens next.");
            AddVariant("chapter_seven_entry", "SYSTEM-02", "Uyarı: Oda, tekrarlanan kullanıcı davranışlarına tepki verecek şekilde tasarlanmıştır. Lütfen normal davranın.", "Warning: This room reacts to repeated user behaviour. Please act normally.");
            Add("laughing_room_entry", "SYSTEM-02", "Davranış sensörleri etkin. Her hareketiniz değerlendirilecektir.", "Behaviour sensors active. Every action will be evaluated.");
            Add("laughing_room_fail", "ANLATICI", "Bunu bilerek yaptın, değil mi? Oda da öyle düşündü.", "You did that on purpose, right? The room thought so too.");
            Add("laughing_room_drop", "ANLATICI", "Yere bıraktın. O da yer değiştirdi. Şimdi kimin eşyası?", "You dropped it. It moved. Whose object is it now?");
            Add("laughing_room_escalate", "SYSTEM-02", "Oda düzeni güncellendi. Lütfen mobilyalarla tartışmayın. Mobilyalar kazanıyor.", "Room layout updated. Please do not argue with the furniture. The furniture is winning.");
            Add("laughing_room_open", "ANLATICI", "Tamam, tamam! Yeterince güldük. Kapıyı açın, bu oda artık şaka yapamıyor.", "Okay, okay! That's enough laughing. Open the door. This room is out of jokes.");
            Add("chapter_eight_entry", "SYSTEM-02", "BÖLÜM 08: Döngü. Önünüzdeki koridor standart uzunluktadır. Tekrar ediyorum: standart.", "CHAPTER 08: The Loop. The corridor ahead is standard length. Repeating: standard.");
            AddVariant("chapter_eight_entry", "ANLATICI", "Bu koridoru daha önce görmüş olabilirsin. Görmediysen de birazdan göreceksin.", "You may have seen this corridor before. If not, you will in a moment.");
            Add("loop_first", "ANLATICI", "Bir dakika... Az önce başladığımız yere mi döndük? Hayır, kesinlikle hayır. Devam et.", "Wait... Did we just return to where we started? No. Absolutely not. Keep going.");
            Add("loop_second", "SYSTEM-02", "İkinci geçiş kaydedildi. Koridorun değişmediği teyit edilemedi.", "Second traversal recorded. The corridor's unchanged status could not be confirmed.");
            Add("loop_broken", "ANLATICI", "Üçüncü kez aynı yere geldik ve sonunda oda pes etti. Mimariyle inatlaşmanın kazananı sensin.", "We came back here for the third time and the room finally gave up. You win this argument with architecture.");
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
