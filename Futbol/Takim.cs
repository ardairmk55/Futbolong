using System;
using System.Collections.Generic;
using System.Linq;

namespace Futbol
{
    public class Takim
    {
        #region 1. TEMEL BİLGİLER VE İSTATİSTİKLER
        public string takimIsmi;
        public string taktik; // "Ofansif", "Defansif", "Dengeli", "TumHatlarlaHucum", "OtobusuCek"
        public int takimPuani;
        public int takimMorali; // 0-100 arası
        public double takimKimyasi; // Oyuncuların birbiriyle uyumu (1.0 ile 1.5 arası)
        public int toplamAtilanGol;
        public int toplamYenenGol;

        // YENİ: Takım Kaptanı Sistemi (Sahadaki lider)
        public Futbolcu takimKaptani;
        #endregion

        #region 2. KADRO VE VERİ YAPILARI
        public List<Futbolcu> kadro;
        public List<Futbolcu> yedekler;
        public List<string> macOlaylari;
        #endregion

        #region 3. YAPICI METOT (CONSTRUCTOR)
        public Takim(string _isim, string _taktik)
        {
            this.takimIsmi = _isim;
            this.taktik = _taktik;
            this.takimPuani = 0;
            this.takimMorali = 100;
            this.takimKimyasi = 1.1; // Başlangıç uyumu
            this.toplamAtilanGol = 0;
            this.toplamYenenGol = 0;

            this.kadro = new List<Futbolcu>();
            this.yedekler = new List<Futbolcu>();
            this.macOlaylari = new List<string>();
        }
        #endregion

        #region 4. KADRO VE KAPTANLIK YÖNETİMİ
        public void OyuncuEkle(Futbolcu yeniOyuncu)
        {
            if (yeniOyuncu != null) kadro.Add(yeniOyuncu);
            KaptanBelirle(); // Yeni oyuncu gelince kaptanlık pazubandını kontrol et
        }

        public void YedekEkle(Futbolcu yedekOyuncu)
        {
            if (yedekOyuncu != null) yedekler.Add(yedekOyuncu);
        }

        public List<Futbolcu> MevkiGetir(string arananMevki)
        {
            return kadro.Where(oyuncu =>
                (arananMevki == "Forvet" && oyuncu is Forvet) ||
                (arananMevki == "Ortasaha" && oyuncu is Ortasaha) ||
                (arananMevki == "Defans" && oyuncu is Defans) ||
                (arananMevki == "Kaleci" && oyuncu is Kaleci)
            ).ToList();
        }

        // YENİ: Sahadaki en tecrübeli/güçlü kişiyi kaptan yapar. Kaptan takıma ekstra kimya sağlar.
        public void KaptanBelirle()
        {
            if (kadro.Count > 0)
            {
                this.takimKaptani = kadro.OrderByDescending(o => o.guc + o.moral).First();
                this.takimKimyasi = Math.Min(1.5, this.takimKimyasi + 0.05); // Liderlik bonusu
            }
        }
        #endregion

        #region 5. YAPAY ZEKA (MENAJER) SİSTEMİ
        // Maçın durumuna ve takımın o anki "Psikolojisine" göre taktik değiştirir
        public string YapayZekaTaktikKontrolu(int dakika, int takimSkoru, int rakipSkor)
        {
            string eskiTaktik = this.taktik;

            // Takım fark yemiş ve morali çökmüşse, daha fazla yememek için geriye çekilir (Gerçekçilik)
            if (rakipSkor - takimSkoru >= 3 && this.takimMorali < 40 && this.taktik != "OtobusuCek")
            {
                this.taktik = "OtobusuCek";
            }
            // Son 15 dakika draması
            else if (dakika > 75)
            {
                if (takimSkoru < rakipSkor && this.taktik != "TumHatlarlaHucum")
                    this.taktik = "TumHatlarlaHucum"; // Yeniliyorsa ölümüne saldır!
                else if (takimSkoru > rakipSkor && rakipSkor - takimSkoru == 1 && this.taktik != "OtobusuCek")
                    this.taktik = "OtobusuCek"; // Tek farkla yeniyorsa skoru koru!
            }
            // Rahat galibiyet durumu
            else if (dakika > 30 && takimSkoru - rakipSkor >= 2 && this.taktik != "Dengeli")
            {
                this.taktik = "Dengeli"; // Rölantiye alıp yorulmayı engelle
            }

            if (eskiTaktik != this.taktik)
            {
                this.takimKimyasi = Math.Max(1.0, this.takimKimyasi - 0.05); // Taktik değişimi anlık bocalama yaratır
                return $"📣 {this.takimIsmi} yedek kulübesi hareketli! Yeni Taktik: {this.taktik}";
            }
            return "";
        }
        #endregion

        #region 6. GELİŞMİŞ GÜÇ VE SİMÜLASYON MOTORLARI

        // YENİ: Kırmızı kart hesaplayıcı. Eğer takım 10 kişi kalırsa gücü ciddi oranda düşer!
        private double KirmiziKartCezasiHesapla(int aktifOyuncuSayisi)
        {
            int eksikKişi = 11 - aktifOyuncuSayisi;
            if (eksikKişi <= 0) return 1.0; // Eksik yoksa ceza yok

            // Her 1 kırmızı kart, takımın tüm istatistiklerini %15 düşürür!
            return Math.Max(0.3, 1.0 - (eksikKişi * 0.15));
        }

        public int GuncelHucumGucu()
        {
            var aktifKadro = kadro.Where(o => !o.kirmiziKartGorduMu && !o.sakatMi).ToList();
            if (!aktifKadro.Any()) return 0;

            double ortalamaHucum = aktifKadro.Average(oyuncu => (oyuncu.atak * 0.6 + oyuncu.teknik * 0.4) * (oyuncu.kondisyon / 100.0));

            // Taktik Çarpanları
            if (taktik == "Ofansif") ortalamaHucum *= 1.15;
            else if (taktik == "TumHatlarlaHucum") ortalamaHucum *= 1.30;
            else if (taktik == "Defansif" || taktik == "OtobusuCek") ortalamaHucum *= 0.85;

            // Kırmızı kart cezası ve Kaptanlık bonusu eklendi
            double kartCezasi = KirmiziKartCezasiHesapla(aktifKadro.Count);
            double kaptanEtkisi = (takimKaptani != null && !takimKaptani.kirmiziKartGorduMu && !takimKaptani.sakatMi) ? 1.05 : 1.0;

            return (int)(ortalamaHucum * (this.takimMorali / 100.0) * this.takimKimyasi * kartCezasi * kaptanEtkisi);
        }

        public int GuncelSavunmaGucu()
        {
            var aktifKadro = kadro.Where(o => !o.kirmiziKartGorduMu && !o.sakatMi).ToList();
            if (!aktifKadro.Any()) return 0;

            double ortalamaSavunma = aktifKadro.Average(oyuncu => (oyuncu.defans * 0.7 + oyuncu.guc * 0.3) * (oyuncu.kondisyon / 100.0));

            if (taktik == "Defansif") ortalamaSavunma *= 1.15;
            else if (taktik == "OtobusuCek") ortalamaSavunma *= 1.30;
            else if (taktik == "TumHatlarlaHucum") ortalamaSavunma *= 0.70; // Ölümüne hücum yaparken defans bomboş kalır!

            double kartCezasi = KirmiziKartCezasiHesapla(aktifKadro.Count);
            return (int)(ortalamaSavunma * (this.takimMorali / 100.0) * this.takimKimyasi * kartCezasi);
        }
        #endregion

        #region 7. DİNAMİK DURUM GÜNCELLEMELERİ (YORGUNLUK VE MORAL)

        public void MoralGuncelle(bool golAtildiMi)
        {
            if (golAtildiMi)
            {
                this.takimMorali = Math.Min(100, this.takimMorali + 15);
                this.takimKimyasi = Math.Min(1.5, this.takimKimyasi + 0.05);
            }
            else
            {
                // Takım kaptanı sahadaysa, gol yendiğinde takımın moralinin çok çökmesini engeller!
                int cokmeMiktari = (takimKaptani != null && !takimKaptani.kirmiziKartGorduMu) ? 12 : 20;

                this.takimMorali = Math.Max(30, this.takimMorali - cokmeMiktari);
                this.takimKimyasi = Math.Max(1.0, this.takimKimyasi - 0.03);
            }
        }

        // YENİ: Mevkiye Göre Gerçekçi Yorgunluk Modeli
        public void TakimYorul(int miktar)
        {
            // Adrenalin: Moral çok yüksekse acı hissetmezler, daha az yorulurlar
            double adrenalinCarpani = (this.takimMorali > 85) ? 0.7 : 1.0;

            // Taktiksel Yorgunluk: Pres veya Tüm Hatlarla Hücum takımı mahveder
            double taktikCarpani = (taktik == "TumHatlarlaHucum") ? 1.5 : 1.0;

            foreach (Futbolcu oyuncu in kadro.Where(o => !o.kirmiziKartGorduMu && !o.sakatMi))
            {
                double mevkiYorgunlugu = miktar;

                // Futbolda en çok orta sahalar, en az kaleciler koşar
                if (oyuncu is Ortasaha) mevkiYorgunlugu *= 1.4;
                else if (oyuncu is Forvet || oyuncu is Defans) mevkiYorgunlugu *= 1.1;
                else if (oyuncu is Kaleci) mevkiYorgunlugu *= 0.3;

                // Toplam yorgunluğu hesapla ve düş
                int dusulecekKondisyon = Convert.ToInt32(mevkiYorgunlugu * adrenalinCarpani * taktikCarpani);
                oyuncu.kondisyon = Math.Max(5, oyuncu.kondisyon - dusulecekKondisyon); // Kondisyon 5'in altına düşmez (kramp sınırı)
            }
        }
        #endregion
    }
}