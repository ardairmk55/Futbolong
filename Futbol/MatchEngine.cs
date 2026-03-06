using System;
using System.Collections.Generic;
using System.Linq;

namespace Futbol
{
    public class MatchEngine
    {
        // --- 1. MAÇ DURUM DEĞİŞKENLERİ ---
        public Takim EvSahibi { get; private set; }
        public Takim Deplasman { get; private set; }
        public int Dakika { get; private set; }
        public int EvSkor { get; private set; }
        public int DepSkor { get; private set; }
        public bool TopEvSahibinde { get; private set; }
        public int SahaBolgesi { get; private set; } // 0: Defans, 1: Orta Saha, 2: Hücum

        private Random rnd = new Random();

        public MatchEngine(Takim ev, Takim dep)
        {
            EvSahibi = ev;
            Deplasman = dep;
            Dakika = 0;
            TopEvSahibinde = true;
            SahaBolgesi = 0;

            // Maç başlarken kaptanları belirle (Takım içi bonuslar için)
            EvSahibi.KaptanBelirle();
            Deplasman.KaptanBelirle();
        }

        // --- 2. ANA MAÇ MOTORU (HAMLE YAP) ---
        public string HamleYap(string aksiyon)
        {
            if (Dakika >= 90) return "🏁 Maç sona erdi! Maçın adamını görmek için istatistiklere bakın.";

            Takim hucum = TopEvSahibinde ? EvSahibi : Deplasman;
            Takim savunma = TopEvSahibinde ? Deplasman : EvSahibi;

            // ADIM 1: Aktif Oyuncu ve Rakip Seçimi (Null kontrolleri yapıldı)
            Futbolcu aktif = GetAktifOyuncu(hucum);
            if (aktif == null || !aktif.OynayabilirMi())
                return "⚠️ Teknik direktör müdahalesi gerekiyor! Sahada aktif oyuncu bulunamadı.";

            Defans rakipDefans = (Defans)savunma.MevkiGetir("Defans").FirstOrDefault() ??
                                 (Defans)savunma.kadro.FirstOrDefault(o => o is Defans);

            Kaleci rakipKaleci = (Kaleci)savunma.MevkiGetir("Kaleci").FirstOrDefault();

            // ADIM 2: Menajer AI Taktik Kontrolü
            string aiTaktikMesaji = hucum.YapayZekaTaktikKontrolu(Dakika,
                                    TopEvSahibinde ? EvSkor : DepSkor,
                                    TopEvSahibinde ? DepSkor : EvSkor);

            bool basari = false;
            string yorum = "";

            // ADIM 3: Aksiyon Mantığı
            switch (aksiyon)
            {
                case "Pas":
                    Dakika += 1;
                    basari = aktif.PasAt(rakipDefans);
                    if (basari)
                    {
                        if (SahaBolgesi < 2) SahaBolgesi++;
                        yorum = $"✅ {aktif.isim} pas trafiğinde çok başarılı. Top ileri taşındı.";
                    }
                    else
                    {
                        yorum = $"❌ {aktif.isim} hatalı pas! Savunma araya giriyor.";
                    }
                    break;

                case "Calim":
                    Dakika += 2;
                    basari = aktif.CalimAt(rakipDefans);
                    if (basari)
                    {
                        if (SahaBolgesi < 2) SahaBolgesi++;
                        yorum = $"🔥 ŞOV! {aktif.isim} müthiş bir çalımla rakibini geride bıraktı!";
                    }
                    else
                    {
                        // Başarısız çalımda darbe alma riski (Şiddet: 10)
                        if (rnd.Next(0, 100) < 25)
                            yorum = aktif.DarbeAl(10);
                        else
                            yorum = $"❌ {aktif.isim} topu ayağından fazla açtı, rakipte kaldı.";
                    }
                    break;

                case "Sut":
                    Dakika += 1;
                    if (SahaBolgesi < 2)
                        return "⛔ Henüz çok uzaktasın! Şut çekmek için hücum bölgesine yaklaşmalısın.";

                    if (aktif is Forvet golcu)
                    {
                        // Kalecinin kurtarış metodu forvetin şut metodu içinden veya dışından çağrılabilir
                        basari = golcu.SutAt(rakipKaleci);
                        if (basari)
                        {
                            if (TopEvSahibinde) EvSkor++; else DepSkor++;
                            yorum = $"🔴 GOOOOOOOL! {golcu.isim.ToUpper()} topu iğne deliğinden geçirdi!";
                            hucum.MoralGuncelle(true);
                            savunma.MoralGuncelle(false);
                            SahaBolgesi = 1; // Gol sonrası santra
                        }
                        else
                        {
                            yorum = $"🧤 {rakipKaleci.isim} kalesinde devleşti! Geçit vermiyor.";
                            // Kaçan şuttan sonra top kalecide kalır
                        }
                    }
                    else
                    {
                        // Orta saha oyuncusu şut çekiyorsa (Forvet değilse) şans %20 düşer
                        basari = rnd.Next(0, 100) < (aktif.teknik / 2);
                        if (basari) { if (TopEvSahibinde) EvSkor++; else DepSkor++; yorum = $"🔴 GOOOL! Beklenmedik bir füze! {aktif.isim}!"; }
                        else yorum = "❌ Uzaktan deneme, top auta gidiyor.";
                    }
                    basari = false; // Şut başarılı olsa da olmasa da top rakibe geçer (Santra veya Degaj)
                    break;
            }

            // ADIM 4: Dinamik Yorgunluk ve Top Kaybı Yönetimi
            hucum.TakimYorul(1);
            savunma.TakimYorul(1);

            if (!basari)
            {
                TopEvSahibinde = !TopEvSahibinde;
                SahaBolgesi = 0; // Top kaptırılınca geriye çekilme
            }

            // ADIM 5: Final Mesajı ve AI Taktik Birleştirmesi
            string sonuc = $"{Dakika}' | {yorum}";
            if (!string.IsNullOrEmpty(aiTaktikMesaji)) sonuc += "\n" + aiTaktikMesaji;

            return sonuc;
        }

        // --- 3. YARDIMCI METOTLAR ---
        private Futbolcu GetAktifOyuncu(Takim t)
        {
            // O an topun olduğu bölgeye göre oyuncu seçer (Kırmızı kartlı/sakat olmayan)
            try
            {
                if (SahaBolgesi == 0) return t.MevkiGetir("Defans").FirstOrDefault(o => o.OynayabilirMi());
                if (SahaBolgesi == 1) return t.MevkiGetir("Ortasaha").FirstOrDefault(o => o.OynayabilirMi());
                return t.MevkiGetir("Forvet").FirstOrDefault(o => o.OynayabilirMi());
            }
            catch
            {
                return t.kadro.FirstOrDefault(o => o.OynayabilirMi());
            }
        }
    }
}