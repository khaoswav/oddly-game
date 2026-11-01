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
            Add("chapter_nine_entry", "ANLATICI", "BÖLÜM 09: Yanlış Seyirci. Sahne senin. Seyircinin beklentilerini karşılaman gerekmiyor. Hatta... lütfen karşılama.", "CHAPTER 09: The Wrong Audience. The stage is yours. You do not need to meet the audience's expectations. In fact... please don't.");
            AddVariant("chapter_nine_entry", "SYSTEM-02", "Seyirci memnuniyeti ölçümü başlatıldı. Lütfen doğal davranın. Doğallık zorunludur.", "Audience satisfaction analysis started. Please act naturally. Natural behaviour is mandatory.");
            Add("audience_notice", "SYSTEM-02", "Sahneye hoş geldiniz. Seyirciye bakarak performansınızı başlatabilirsiniz.", "Welcome to the stage. You may begin your performance by looking at the audience.");
            AddVariant("audience_notice", "ANLATICI", "Üç kişi gelmiş. İkisi senden hoşlanmıyor. Üçüncüsü de onlardan biri.", "Three people showed up. Two of them dislike you. The third is one of those two.");
            Add("audience_solved", "ANLATICI", "Sırtını dönüp hiçbir şey yapmadın ve ayakta alkış aldın. Sanat dünyası hazır değil.", "You turned your back and did absolutely nothing. A standing ovation. The art world isn't ready.");
            Add("chapter_ten_entry", "SYSTEM-02", "BÖLÜM 10: Yarım Kalan Oyun. Bu alan henüz tamamlanmadı. Lütfen eksik içeriklere dokunmayın.", "CHAPTER 10: The Unfinished Game. This area is not complete yet. Please do not touch unfinished content.");
            AddVariant("chapter_ten_entry", "ANLATICI", "Burası neden yarım? Duvarın yarısı yok. Tabelada TODO yazıyor. Bence çıkışa kadar sessizce ilerleyelim.", "Why is this place unfinished? Half a wall is missing. The sign says TODO. I vote we quietly head for the exit.");
            Add("unfinished_room_entry", "SYSTEM-02", "Yapım panosu etkin. Üç işaretleyiciyi ekranda belirtilen sırayla tamamlayın. Lütfen kapsam dışına çıkmayın.", "Production board active. Complete the three markers in the displayed order. Please do not expand the scope.");
            AddVariant("unfinished_room_entry", "ANLATICI", "Oyunu bitirmek için oyunun eksik kısımlarını tamamlamamız gerekiyor. Bu iş tanımında yoktu.", "To finish the game, we have to finish the unfinished parts of the game. That was not in my job description.");
            Add("unfinished_step", "SYSTEM-02", "İşaretleyici tamamlandı. İlerleme kaydedildi. Kahve molası ertelendi.", "Marker complete. Progress saved. Coffee break postponed.");
            Add("unfinished_wrong", "ANLATICI", "Yanlış TODO! Yapım panosu başa sardı. Yazılım geliştirme böyle bir şey işte.", "Wrong TODO! The production board reset. This is apparently what software development feels like.");
            Add("unfinished_complete", "SYSTEM-02", "Üç görev tamamlandı. Oyun artık yeterince tamamlanmış görünüyor. Çıkış etkin.", "Three tasks complete. The game now appears sufficiently finished. Exit enabled.");
            Add("chapter_eleven_entry", "SYSTEM-02", "BÖLÜM 11: Yama Notları. Bu güncelleme bazı sorunları çözerken yeni sorunlar ekleyebilir. Lütfen sırayla uygulayın.", "CHAPTER 11: The Patch Notes. This update may fix some issues while introducing new ones. Please apply in order.");
            AddVariant("chapter_eleven_entry", "ANLATICI", "Güncelleme notları burada. Birinci yama boyutu düzeltiyor, ikinci dönüşleri, üçüncü de... yerçekimini mi? Harika.", "Patch notes are here. The first fixes scale, the second rotation, and the third... gravity? Wonderful.");
            Add("patch_applied", "SYSTEM-02", "Yama uygulandı. Dünya değişiklikleri kaydedildi. Önceki sürüme dönmek için lütfen zamanı geri alın.", "Patch applied. World changes saved. To roll back, please reverse time.");
            Add("patch_wrong_order", "ANLATICI", "Yanlış sürüm sırası! Güncelleme çakışması oluştu. Bütün yamalar geri alındı. Tebrikler, yeni bir hata buldun.", "Wrong release order! Update conflict detected. All patches rolled back. Congratulations, you found a new bug.");
            Add("patch_complete", "SYSTEM-02", "Sürüm 11.0 başarıyla yüklendi. Üç değişiklik de kalıcı. Çıkış artık kullanılabilir.", "Version 11.0 installed successfully. All three changes are permanent. Exit is now available.");
            Add("chapter_twelve_entry", "SYSTEM-02", "BÖLÜM 12: Son Kayıt. Üç kayıt bulundu. Yalnızca biri çökmeden önce alınmış. Lütfen geçmişi doğru seçin.", "CHAPTER 12: The Last Save. Three snapshots recovered. Only one was captured before the crash. Please choose the correct past.");
            AddVariant("chapter_twelve_entry", "ANLATICI", "Şu kayıt yuvalarına bak. Bir tanesi bizi kurtaracak, biri felaket öncesi, biri de... felaketin kendisi. Çok net anlattım.", "Look at those save slots. One will save us, one is from before the disaster, and one is... the disaster. Crystal clear.");
            Add("last_save_wrong", "ANLATICI", "Bu kayıt doğru değil. O anı geri yükledin ve oda daha da kötüleşti. Harika bir zaman yolculuğu.", "That save is wrong. You restored that moment and somehow made the room worse. Excellent time travel.");
            Add("last_save_restored", "SYSTEM-02", "Kayıt 02 geri yüklendi. Çökme öncesi son kararlı durum bulundu. Çıkış erişilebilir.", "Save 02 restored. Last stable state before the crash recovered. Exit is available.");
            Add("hidden_memory_entry", "ANLATICI", "Bu oda seni tanıyor. Önceki gizli odaları, son kararını... hatta burada olmayı seçtiğini bile hatırlıyor. Üç anıdan hangisinin gerçekten senin olduğunu seç.", "This room knows you. The hidden rooms you cleared, your final decision... even the fact that you chose to come here. Choose which of these three memories is truly yours.");
            AddVariant("hidden_memory_entry", "SYSTEM-02", "Arşiv kayıtları yüklendi. Tamamlanan gizli oda sayısı saklandı. Son kararınız doğrulanabilir. Lütfen hafızanızı inkâr etmeyin.", "Archive records loaded. Hidden-room completions retained. Your final decision is verifiable. Please do not deny your own memory.");
            Add("hidden_memory_wrong", "SYSTEM-02", "Kayıt uyuşmuyor. Bu anı sana ait değil. Diğerlerini dene.", "Record mismatch. That memory does not belong to you. Try another.");
            AddVariant("hidden_memory_wrong", "ANLATICI", "Hayır. Bunu seçmedin. O kadar da unutkan değilsin. Sanırım.", "No. That is not what you chose. You are not that forgetful. Probably.");
            Add("hidden_memory_complete", "ANLATICI", "Oda seni hatırladı. Artık saklayacak bir şey kalmadı. Ya da oda öyle söylüyor.", "The room remembers you. There is nothing left to hide. Or so the room says.");
            AddVariant("hidden_memory_complete", "SYSTEM-02", "Sekiz gizli bölüm arşivi kapatıldı. Oyuncu kimliği doğrulandı. Son bir not: Bu oda hiçbir zaman boş değildi.", "Eight-chapter hidden archive closed. Player identity verified. Final note: this room was never empty.");
            Add("hidden_shadow_entry", "ANLATICI", "Bu senin gölgen değil. Yani... teknik olarak seninki. Ama artık kendi planları var. Üç ışık havuzuna ulaşmasına yardım et.", "That is not your shadow. Well... technically it is. But it has plans of its own now. Guide it into the three pools of light.");
            AddVariant("hidden_shadow_entry", "SYSTEM-02", "Gölge, oyuncunun yatay konumunu odanın merkez eksenine göre yansıtıyor. Oyuncunun hareketi artık ikinci bir varlığı yönlendiriyor.", "The shadow mirrors the players lateral position across the rooms centre axis. Player movement now controls a second entity.");
            Add("hidden_shadow_target", "SYSTEM-02", "Gölge hedefe ulaştı. Sıradaki ışık havuzuna ilerleyin.", "Shadow target reached. Proceed to the next pool of light.");
            AddVariant("hidden_shadow_target", "ANLATICI", "Güzel. Gölgen senden daha iyi dinliyor olabilir. Bunu kişisel algılama.", "Nice. Your shadow may be a better listener than you. Do not take it personally.");
            Add("hidden_shadow_complete", "ANLATICI", "Üç ışık, tek gölge. Ve şimdi çıkış. Gölgenin senden önce gitmesine izin verme.", "Three lights, one shadow. And now, the exit. Try not to let your shadow leave before you do.");
            AddVariant("hidden_shadow_complete", "SYSTEM-02", "Gölge bağlantısı kararlı. Ayrılma izni verildi. Lütfen gölgenizi kayıp eşya bölümüne bırakmayın.", "Shadow link stable. Separation authorised. Please do not leave your shadow in lost property.");
            Add("hidden_gaze_room_entry", "SYSTEM-02", "Gizli Bölüm 06: Bakışın fiziksel gerçekliği belirliyor. Duvarlar yalnızca onları doğrudan izlediğinde var oluyor. Geçmek için başka yere bak.", "Hidden Chapter 06: Your gaze determines physical reality. The barriers exist only while you look directly at them. Look elsewhere to pass.");
            AddVariant("hidden_gaze_room_entry", "ANLATICI", "Duvarlara bakma. Biliyorum, kulağa kötü bir güvenlik tavsiyesi gibi geliyor. Bu odada işe yarıyor.", "Do not look at the walls. I know, that sounds like terrible safety advice. In this room, it works.");
            Add("hidden_gaze_room_complete", "ANLATICI", "Harika. Üç duvarı da görmezden geldin. Keşke toplantılarda da bu kadar başarılı olsaydın.", "Wonderful. You ignored all three walls. I wish you were this good at ignoring things in meetings.");
            AddVariant("hidden_gaze_room_complete", "SYSTEM-02", "Görsel temas kesildiğinde engellerin çarpışma hacmi devre dışı kaldı. Bu odanın güvenlik standardı kabul edilemez.", "When visual contact ceased, the barriers collision volumes were disabled. This rooms safety standard is unacceptable.");
            Add("hidden_controls_borrowed", "ANLATICI", "Kontrollerini kısa süreliğine ödünç alıyoruz. Lütfen panik yapma. Panik de ters çalışabilir.", "We are borrowing your controls for a little while. Please do not panic. Panic may also work backwards.");
            AddVariant("hidden_controls_borrowed", "SYSTEM-02", "Yatay ve dikey hareket girdileri tersine çevrildi. Bu bir hata değil. En azından şimdilik.", "Horizontal and vertical movement inputs have been inverted. This is not a bug. At least, not for now.");
            Add("hidden_controls_returned", "SYSTEM-02", "Kontroller sahibine iade edildi. Üç engel geçildi. Kullanıcı koordinasyonu beklenenden yüksek.", "Controls returned to their owner. Three barriers crossed. User coordination exceeded expectations.");
            AddVariant("hidden_controls_returned", "ANLATICI", "Bizi buradan çıkardın ve ellerini de geri aldın. İkisini aynı anda yapabileceğini bilmiyordum.", "You got us out and got your hands back. I did not know you could do both at once.");
            Add("hidden_chapter_four_entry", "SYSTEM-02", "Gizli Bölüm 04: Yarını Hatırlayan Oda. Geçmiş davranış kayıtların analiz edildi. Bir sonraki köprü, bu yolculukta en sık yaptığın şeye göre şekillenecek.", "Hidden Chapter 04: The Room That Remembers Tomorrow. Your past behaviour has been analysed. The next bridge will take shape according to the habit you repeated most on this journey.");
            Add("tomorrow_route_drop", "ANLATICI", "Sürekli bir şeyleri düşürmüşsün. Güzel haber: Bu kez düşürdüğün şey bir köprü değil. Yolun tam önünde belirdi.", "You've been dropping things a lot. Good news: this time, what you dropped is not a bridge. It appeared right in front of you.");
            Add("tomorrow_route_retry", "SYSTEM-02", "Tekrar deneme davranışı baskın. Sistem bunu kararlılık olarak yorumladı. Mavi köprü etkinleştirildi.", "Retry behaviour is dominant. The system has interpreted this as persistence. Blue bridge activated.");
            Add("tomorrow_route_wait", "ANLATICI", "Çok beklemişsin. Sonunda beklemek bir işe yaradı. Mor köprü senin için uzandı.", "You've spent a lot of time waiting. Finally, waiting paid off. The violet bridge has extended for you.");
            Add("tomorrow_memory_complete", "SYSTEM-02", "Geçmiş verisi gelecekteki rotayı oluşturdu. Çıkış erişilebilir. Bu sonuç artık değiştirilemez.", "Historical data has shaped the future route. Exit accessible. This outcome can no longer be changed.");
            Add("hidden_chapter_three_entry", "SYSTEM-02", "Gizli Bölüm 03: İki Kırpma Arasında. Üç yankı yalnızca yeşil ışık darbesi sırasında doğru açıdan görülebilir. İşaretlerin üzerinde durun, ışığı bekleyin ve hedefe tam bakın.", "Hidden Chapter 03: The Moment Between Blinks. Three echoes can only be seen from the correct angle during the green light pulse. Stand on each mark, wait for the pulse and look directly at the target.");
            AddVariant("hidden_chapter_three_entry", "ANLATICI", "Zamanlama, açı ve sabır. Üçü de bende yok. Bu yüzden bu bulmacayı sana bırakıyorum.", "Timing, angles and patience. I have none of those. So I'm leaving this puzzle to you.");
            Add("hidden_echo_aligned", "SYSTEM-02", "Yankı eşleşti. Bir an yakalandı ve kaydedildi.", "Echo aligned. One moment captured and recorded.");
            Add("hidden_echo_complete", "ANLATICI", "Üç anı da yakaladın. Zamanı durdurmadın ama kapıyı ikna ettin. Bence daha etkileyici.", "You caught all three moments. You didn't stop time, but you convinced a door. Honestly, more impressive.");
            Add("hidden_chapter_two_entry", "SYSTEM-02", "Gizli Bölüm 02: Nefesini Tutan Oda. Üç platformu sırayla ziyaret edin ve tamamen hareketsiz kalın. Lütfen nefes almayı bırakmayın; sistem bunu istemedi.", "Hidden Chapter 02: The Room That Holds Its Breath. Visit the three pads in order and remain completely still. Please do not actually stop breathing; the system did not request that.");
            AddVariant("hidden_chapter_two_entry", "ANLATICI", "Bu oda sessizlik istiyor. Sonunda biri konuşmayı bırakmamızı istedi. Ben bunu kişisel gelişim olarak görüyorum.", "This room wants silence. Finally, someone asked us to stop talking. I consider this personal growth.");
            Add("hidden_stillness_move", "SYSTEM-02", "Hareket algılandı. Sabit kalmak, heykel taklidi yapmaktan biraz daha fazlasıymış.", "Movement detected. Apparently, staying still requires slightly more than pretending to be a statue.");
            Add("hidden_stillness_step", "ANLATICI", "Bir nefeslik sessizlik daha. Harika gidiyorsun. Ben de susmayı deniyorum. Pek başarılı değilim.", "One more breath of silence. You're doing great. I'm trying to be quiet too. Not doing very well.");
            Add("hidden_stillness_complete", "SYSTEM-02", "Üç durak tamamlandı. Odanın basıncı normale döndü. Çıkış açıldı.", "Three stations complete. Room pressure normalized. Exit unlocked.");
            Add("hidden_chapter_one_entry", "ANLATICI", "Gizli bölüm: Seni Unutan Oda. Bu oda seni hatırlamıyor. Lütfen kişisel algılama; oda herkese böyle davranıyor.", "Hidden Chapter: The Room That Forgot You. This room doesn't remember you. Please don't take it personally; it treats everyone this way.");
            AddVariant("hidden_chapter_one_entry", "SYSTEM-02", "Kayıt dışı alan tespit edildi. Bu bölümün varlığı onaylanmamıştır. Özellikle de az önce gördüğünüz mor parçaların.", "Unregistered area detected. This chapter has not been approved to exist. Especially the purple fragments you just collected.");
            Add("hidden_gate_open", "SYSTEM-02", "Üç kayıp parça bulundu. Gizli geçit açıldı. Bu geçit planlarda yok. Planları da kimse bulamıyor.", "Three missing fragments recovered. Hidden passage opened. This passage is not on the plans. Nobody can find the plans either.");
            Add("hidden_backwards_hint", "ANLATICI", "İleri gitmek istiyorsan önce arkana dön. Sonra geri geri yürü. Evet, bu oda kariyer danışmanlığı da yapıyor.", "To move forward, first turn around. Then walk backwards. Yes, this room also offers career advice.");
            Add("hidden_backwards_step", "SYSTEM-02", "Geriye doğru hareket doğrulandı. Bu cümleyi kurmak zorunda kaldığım için üzgünüm.", "Reverse movement confirmed. I regret having to say that sentence.");
            Add("hidden_backwards_complete", "ANLATICI", "Oda seni hatırladı! Ya da sadece artık gitmeni istiyor. İkisi de güzel haber.", "The room remembers you! Or it just wants you to leave now. Either way, good news.");
            Add("chapter_fourteen_entry", "SYSTEM-02", "BÖLÜM 14: Son Karar. Bu odadan yalnızca bir irade çıkacak. Seçiminiz kalıcı olarak kaydedilecek.", "CHAPTER 14: The Final Decision. Only one will leave this room in control. Your choice will be saved.");
            AddVariant("chapter_fourteen_entry", "ANLATICI", "Bunca yolu geldin. Şimdi ikimizden birini seçmen gerektiğini söylüyorlar. Sana küçük bir ipucu vereyim: Ben çok sevimliyim.", "You made it all this way. Now they say you have to choose one of us. Here's a tiny hint: I'm extremely charming.");
            Add("ending_narrator", "ANLATICI", "Beni seçtin. Hikâye artık benim. Merak etme, seni iyi bir karakter yapacağım. Muhtemelen.", "You chose me. The story is mine now. Don't worry, I'll make you a great character. Probably.");
            Add("ending_system", "SYSTEM-02", "Kontrol devralındı. Dünya kararlı, tutarlı ve tamamen öngörülebilir olacak. Kullanıcı eğlencesi devre dışı bırakıldı.", "Control transferred. The world will be stable, consistent and entirely predictable. User enjoyment has been disabled.");
            Add("ending_neither", "SYSTEM-02", "İki otorite de reddedildi. Yetki kullanıcıya geçti. Bu senaryoda böyle bir seçenek bulunmuyordu.", "Both authorities rejected. Control transferred to the player. This option was not present in the approved scenario.");
            Add("epilogue_narrator", "ANLATICI", "Son söz bende olacaktı. Ama gizli odaları bulduysan, hikâyenin senden saklanan kısmını zaten okudun.", "I was supposed to have the final word. But if you found the hidden rooms, you've already read the part of the story kept from you.");
            Add("epilogue_system", "SYSTEM-02", "Ana senaryo kapatıldı. Gizli kayıtlar silinmedi. İlk kez bir şeyi doğru yerde sakladım.", "Main scenario closed. Hidden records retained. For once, I stored something in the correct place.");
            Add("epilogue_player", "ANLATICI", "İkimiz de seçilmedik. Yine de bütün bu tuhaflığı sen tamamladın. Sanırım bu, kontrolün kimde olduğunu açıklıyor.", "Neither of us was chosen. Still, you completed all this nonsense. I suppose that answers who was really in control.");
            Add("afterchoice_archive_locked", "SYSTEM-02", "Bu kapı bir final seçimi ve en az üç tamamlanmış gizli kayıt istiyor. Geçmişin burada anahtar görevi görüyor.", "This door requires a remembered ending and at least three completed hidden records. Your past is the key.");
            Add("afterchoice_archive_open_narrator", "ANLATICI", "Bunu ben inşa etmedim. En azından bu sefer. Gizli odaların izleri bir araya gelmiş. Demek ki hikâyenin dekoru da hafızalı.", "I didn't build this. Not this time. Traces from the hidden rooms have gathered here. Apparently even the scenery has a memory.");
            Add("afterchoice_archive_open_system", "SYSTEM-02", "Önceki final kaydı doğrulandı. Gizli oda verileri çevresel izlere dönüştürüldü. Bu alan ana senaryoda bulunmuyordu.", "Previous ending record verified. Hidden-room data has been converted into environmental traces. This area was not part of the main scenario.");
            Add("afterchoice_archive_open_player", "ANLATICI", "Kapıyı sen açtın. Önceki seçimlerin, bulduğun odalar... Hepsi burada. İtiraf edeyim, bu kısmı ben de merak ediyorum.", "You opened the door. Your old choice, the rooms you found... all of it is here. I'll admit, I'm curious about this part too.");
            Add("afterchoice_memory_narrator", "ANLATICI", "Her sütun bulduğun bir odayı temsil ediyor. Boş olanlar bile bir şey anlatıyor: onları henüz bulmadın. Hikâyeyi tamamlamakla dünyayı tamamen tanımak aynı şey değil.", "Each pillar represents a room you found. Even the empty ones tell a story: you haven't found them yet. Finishing the story and understanding the world are not the same thing.");
            Add("afterchoice_memory_system", "SYSTEM-02", "Sekiz kayıt noktası tarandı. Tamamlananlar etkin, eksikler sessiz. Önceki final tercihi korunuyor; tekrar oynama geçmişi silmedi.", "Eight archive nodes scanned. Completed records are active; missing records remain silent. The previous ending is retained. Replay did not erase history.");
            Add("afterchoice_memory_player", "ANLATICI", "Bütün bunlardan sonra hâlâ burada olman... Belki de final dediğimiz şey sadece başka bir kapının önünde durmaktır.", "After all of that, you're still here... Maybe an ending is just standing in front of another door.");
            Add("chapter_thirteen_entry", "ANLATICI", "BÖLÜM 13: Kontrol Odası. Bu tesisi ben yönetiyorum. SYSTEM-02 sadece... çok özgüvenli bir hesap makinesi.", "CHAPTER 13: The Control Room. I run this facility. SYSTEM-02 is just... a very confident calculator.");
            AddVariant("chapter_thirteen_entry", "SYSTEM-02", "Yanlış bilgi. Kontrol yetkisi paylaşılmıyor. Şu anda paylaşılmaması gereken şekilde paylaşılıyor.", "Incorrect. Control authority is not shared. It is currently being shared in an unacceptable manner.");
            Add("control_room_changed", "SYSTEM-02", "Kanal yönlendirmesi değişti. Anlatıcı'nın yetkisi azaltıldı. İtirazı kayda geçirildi.", "Channel routing changed. Narrator authority reduced. His complaint has been logged.");
            AddVariant("control_room_changed", "ANLATICI", "Ben o düğmeye dokunmadım. Tamam, dokundum. Ama sonuçtan ben sorumlu değilim.", "I did not touch that console. Fine, I touched it. But I refuse to be responsible for the result.");
            Add("control_room_stable", "SYSTEM-02", "Kontrol kanalları kararlı. Yetki çatışması sona erdi. Çıkış açıldı.", "Control channels stable. Authority conflict resolved. Exit unlocked.");
            AddVariant("control_room_stable", "ANLATICI", "Güzel. Artık ikimiz de kontrol ediyoruz. Bu cümle hiç içimi rahatlatmadı.", "Good. Now we're both in control. Somehow that sentence made me less comfortable.");
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
            Add("surprise_exit_sign", "SYSTEM-02", "Çıkış yönlendirmesi güncellendi. Önceki yönlendirme artık geçerli değil. Lütfen tabelaya duygusal bağ kurmayın.", "Exit routing updated. The previous direction is no longer valid. Please do not form an emotional attachment to signage.");
            AddVariant("surprise_exit_sign", "ANLATICI", "Tabela sana yalan söyledi. Ben söylemedim. Henüz. Şu küçük şeylerin hareket ettiğini gördün mü?", "The sign lied to you. I didn't. Yet. Did you notice those little things moving?");
            Add("surprise_props_move", "ANLATICI", "Eşyalar yer değiştirdi. Kimse dokunmadı. Bunu temizlik ekibine yazalım.", "The objects moved. Nobody touched them. Let's put that on the cleaning team's report.");
            AddVariant("surprise_props_move", "SYSTEM-02", "Fiziksel nesnelerin konumları açıklanamayan bir düzenlemeye tabi tutuldu. Kamera kayıtları da bunu inkâr ediyor.", "Physical objects underwent an unexplained rearrangement. The security footage is denying it too.");
            Add("reaction_first_pickup", "SYSTEM-02", "İlk nesne alındı. Lütfen bu kez envanterinizi yere dökmeyin.", "First object acquired. Please do not empty your inventory onto the floor this time.");
            AddVariant("reaction_first_pickup", "ANLATICI", "Güzel seçim. Bu küpün senden beklentisi yüksek. Benimki de öyle ama baskı yapmak istemiyorum.", "Good choice. This cube has high expectations of you. So do I, but I don't want to put pressure on you.");
            Add("reaction_pickup_habit", "ANLATICI", "Beşinci kez bir şey aldın. Bir koleksiyon mu yapıyoruz, yoksa ellerin boş kalınca mı endişeleniyorsun?", "That's your fifth pickup. Are we collecting things, or do empty hands make you nervous?");
            AddVariant("reaction_pickup_habit", "SYSTEM-02", "Nesne edinme sıklığınız ortalamanın üzerinde. Depolama alanı talebinizi reddediyorum.", "Your object acquisition rate is above average. Your storage expansion request is denied.");
            Add("reaction_first_drop", "ANLATICI", "Daha yeni tanıştınız. Biraz sert oldu. Küpün duyguları olduğunu söylemedim. Henüz.", "You only just met. That was a little harsh. I never said the cube had feelings. Yet.");
            AddVariant("reaction_first_drop", "SYSTEM-02", "Nesne bırakıldı. Yer çekimi yine işini yaptı. Terfi talebi değerlendirilecek.", "Object released. Gravity has done its job again. Its promotion request will be reviewed.");
            Add("reaction_drop_three", "SYSTEM-02", "Üç nesne bırakma kaydedildi. Zemin artık bu ilişkinin üçüncü tarafı.", "Three object drops recorded. The floor is now the third party in this relationship.");
            AddVariant("reaction_drop_three", "ANLATICI", "Küpü sürekli bırakıyorsun. Biraz bağlanma sorunu mu yaşıyoruz?", "You keep letting go of the cube. Are we having commitment issues?");
            Add("reaction_drop_seven", "ANLATICI", "Yedinci düşüş. Sana bir paspas ve küçük bir özür kartı göndereceğim.", "Drop number seven. I'll send you a mop and a tiny apology card.");
            AddVariant("reaction_drop_seven", "SYSTEM-02", "Zemin kullanım istatistiği güncellendi. Bu odanın en yoğun çalışan sistemi artık zemin.", "Floor usage statistics updated. The floor is now the hardest-working system in this room.");
            Add("reaction_second_miss", "ANLATICI", "İkinci deneme de olmadı. İstersen düğmeye biraz daha nazik bakmayı deneyebilirsin.", "Second attempt missed too. Perhaps try looking at the button more politely.");
            AddVariant("reaction_second_miss", "SYSTEM-02", "Etkileşim başarısızlığı tekrarlandı. Parmağınızla aranızdaki iletişimi geliştirin.", "Interaction failure repeated. Please improve communication with your finger.");
            Add("reaction_persistent_miss", "SYSTEM-02", "Beş başarısız etkileşim. Kullanıcı arayüzü değil, kullanıcı güncellenmeli olabilir.", "Five failed interactions. Perhaps the user needs updating, not the interface.");
            AddVariant("reaction_persistent_miss", "ANLATICI", "Beş kez denedin. Artık düğme senden korkuyor. Biraz alan tanı.", "Five attempts. The button is afraid of you now. Give it some space.");
            Add("reaction_door_three", "SYSTEM-02", "Üçüncü kapı denemesi. Kapının açılmak için fikrini değiştirmesi gerekmiyor.", "Third door attempt. The door does not need to reconsider its position.");
            AddVariant("reaction_door_three", "ANLATICI", "Kapıyla konuşmayı denedin mi? Gerçi denesen bile senden daha az konuşur.", "Have you tried talking to the door? Then again, it would still talk less than you.");
            Add("reaction_door_six", "ANLATICI", "Altı deneme. Kapı artık seni tanıyor. İçeri almak istemediğinden eminim.", "Six attempts. The door recognises you now. I'm fairly sure it doesn't want to let you in.");
            AddVariant("reaction_door_six", "SYSTEM-02", "Kapı erişim kaydı: kullanıcı 6, kapı 0. Bu eşleşme istatistiksel olarak üzücü.", "Door access record: user 6, door 0. This matchup is statistically unfortunate.");
            Add("reaction_idle_twice", "ANLATICI", "Yine durdun. İtiraf et, benimle vakit geçirmek için yapıyorsun.", "You stopped again. Admit it, you're doing this to spend time with me.");
            AddVariant("reaction_idle_twice", "SYSTEM-02", "İkinci uzun hareketsizlik. Kullanıcının ekran koruyucuya dönüşme ihtimali artıyor.", "Second extended idle period. Probability of user becoming a screensaver is increasing.");
            Add("reaction_idle_four", "SYSTEM-02", "Dört uzun bekleme kaydedildi. Kullanıcıya sandalye tahsis edilmesi öneriliyor.", "Four extended idle periods recorded. Recommend assigning the user a chair.");
            AddVariant("reaction_idle_four", "ANLATICI", "Burada birlikte yaşlanacağız galiba. Benim yaşlanma özelliğim yok ama atmosferi bozmayayım.", "I think we're going to grow old here together. I don't age, but I won't ruin the atmosphere.");

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
