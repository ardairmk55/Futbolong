using System;

namespace Futbol
{
    public class Kaleci : Futbolcu
    {
        // 1. İstatistikler
        public int kurtarisSayisi;
        public int yenenGolSayisi;
        public int kalesiniGoleKapattigiMac; // Clean Sheet

        // 2. Form1.cs Uyumluluğu ve Kaleciye Özel Yetenekler
        public int refleks;
        public int pozisyonAlma;
        public int iletisim; // Defansı organize etme ve liderlik

        // 3. Gelişmiş Yapıcı Metot (Constructor)
        public Kaleci(string _isim, double _boy, int _guc, int _hiz, int _teknik, int _atak, int _defans, int _kalecilik, int _refleks, int _pozisyonAlma = 80, int _iletisim = 80)
            : base(_isim, _boy, _guc, _hiz, _teknik, _atak, _defans, _kalecilik)
        {
            this.refleks = _refleks;
            this.pozisyonAlma = _pozisyonAlma;
            this.iletisim = _iletisim;

            this.kurtarisSayisi = 0;
            this.yenenGolSayisi = 0;
            this.kalesiniGoleKapattigiMac = 0;
        }

        // --- KURTARIŞ MEKANİKLERİ ---

        // 1. Standart Şut Kurtarışı (Pozisyon alma ve kalecilik ön planda)
        public bool KurtarisYap(Futbolcu rakip)
        {
            if (!OynayabilirMi()) return false;

            this.kondisyon = Math.Max(0, this.kondisyon - 3);

            // Boy avantajı logaritması (1.70m üzeri her cm altın değerinde)
            int boyAvantaji = (this.boy > 1.70) ? Convert.ToInt32((this.boy - 1.70) * 80) : 0;
            double verimlilik = KondisyonCarpaniGetir() * (this.moral / 100.0);

            int kurtarisKatsayi = Convert.ToInt32((this.kalecilik * 0.5 + this.pozisyonAlma * 0.3 + this.refleks * 0.2 + boyAvantaji) * verimlilik);
            int sutZorlugu = Convert.ToInt32(rakip.atak * 0.45 + rakip.guc * 0.35 + rakip.teknik * 0.20);

            if (rasgele.Next(0, kurtarisKatsayi + sutZorlugu) < kurtarisKatsayi)
            {
                KurtarisIslemleri(10); // Normal kurtarış
                return true;
            }

            GolYemeIslemleri(10);
            return false;
        }

        // 2. Birebir Kalma Senaryosu (Hız ve cesaret)
        public bool BirebirKapat(Futbolcu rakip)
        {
            if (!OynayabilirMi()) return false;

            this.kondisyon = Math.Max(0, this.kondisyon - 5);

            // Birebirde açıyı çabuk kapatmak (hız) ve refleks hayatidir
            int aciKapatmaGucu = Convert.ToInt32((this.hiz * 0.4 + this.refleks * 0.4 + this.kalecilik * 0.2) * KondisyonCarpaniGetir());
            int forvetGucu = Convert.ToInt32(rakip.teknik * 0.6 + rakip.hiz * 0.4);

            if (rasgele.Next(0, aciKapatmaGucu + forvetGucu) < aciKapatmaGucu)
            {
                KurtarisIslemleri(20); // Birebir kurtarmak efsanedir, moral uçar
                return true;
            }

            GolYemeIslemleri(15);
            return false;
        }

        // 3. Penaltı Kurtarışı (Saf Sezgi ve Refleks)
        public bool PenaltiKurtar(Futbolcu rakip)
        {
            if (!OynayabilirMi()) return false;

            // Penaltıda fiziksel yorgunluk etkisizdir, saf refleks konuşur.
            int refleksGucu = Convert.ToInt32(this.refleks * 0.7 + this.kalecilik * 0.3);
            int sutGucu = Convert.ToInt32(rakip.teknik * 0.7 + rakip.guc * 0.3);

            // Forvet penaltıda her zaman +40 avantajlıdır
            if (rasgele.Next(0, refleksGucu + sutGucu + 40) < refleksGucu)
            {
                KurtarisIslemleri(40); // Penaltı kurtaran kaleci maçın adamı olur!
                return true;
            }

            // Penaltıdan gol yemek kaleciyi çok bozmaz (Beklenen bir şeydir)
            GolYemeIslemleri(5);
            return false;
        }

        // --- YENİ NESİL KALECİ MEKANİKLERİ ---

        // 4. Libero Çıkışı (Sweeper Keeper - Defansın arkasına atılan topu kesme)
        public bool LiberoCikisiYap(Futbolcu rakipForvet)
        {
            if (!OynayabilirMi()) return false;

            this.kondisyon = Math.Max(0, this.kondisyon - 6); // Çok riskli ve yorucu bir depar

            int cikisZamanlamasi = Convert.ToInt32((this.hiz * 0.5 + this.pozisyonAlma * 0.5) * KondisyonCarpaniGetir());
            int forvetHizi = Convert.ToInt32(rakipForvet.hiz * 0.8 + rakipForvet.teknik * 0.2);

            if (rasgele.Next(0, cikisZamanlamasi + forvetHizi) < cikisZamanlamasi)
            {
                this.moral = Math.Min(100, this.moral + 15);
                return true; // Tehlikeyi büyümeden uzaklaştırdı
            }

            // Zamanlamayı tutturamazsa faul yapma ve atılma riski çok yüksek!
            if (rasgele.Next(0, 100) < 30)
            {
                this.yapilanFaul++;
                this.moral = Math.Max(0, this.moral - 20);
                // Burada ana sınıftaki KartGor() metodunu tetikleyebilirsin
            }
            return false; // Forvet kaleciyi çalımladı!
        }

        // 5. Defansı Organize Etme (Takım savunmasını rahatlatır)
        public void DefansiOrganizeEt()
        {
            if (this.kondisyon > 20)
            {
                // Kaleci bağırarak takımı uyarır, kendi morali ve konsantrasyonu artar
                this.moral = Math.Min(100, this.moral + (this.iletisim / 10));
                this.kondisyon = Math.Max(0, this.kondisyon - 1); // Bağırmak bile efor ister
            }
        }

        // --- OYUN KURMA METOTLARI ---

        public bool DegajDik()
        {
            this.kondisyon = Math.Max(0, this.kondisyon - 2);
            int isabet = Convert.ToInt32((this.teknik * 0.4 + this.guc * 0.6) * KondisyonCarpaniGetir());
            return rasgele.Next(0, 100) < isabet;
        }

        public bool TopuElleOyunaSok()
        {
            this.kondisyon = Math.Max(0, this.kondisyon - 1);
            int isabet = Convert.ToInt32((this.teknik * 0.7 + this.kalecilik * 0.3) * KondisyonCarpaniGetir());
            return rasgele.Next(0, 100) < isabet;
        }

        // --- YARDIMCI METOTLAR VE PUANLAMA ---

        private void KurtarisIslemleri(int moralArtisi)
        {
            this.kurtarisSayisi++;
            this.moral = Math.Min(100, this.moral + moralArtisi);
        }

        private void GolYemeIslemleri(int moralDususu)
        {
            this.yenenGolSayisi++;
            this.moral = Math.Max(0, this.moral - moralDususu);
        }

        public double MacPuaniHesapla()
        {
            double puan = 6.0; // Standart başlangıç puanı

            puan += (kurtarisSayisi * 0.7);
            puan -= (yenenGolSayisi * 1.0);

            if (yenenGolSayisi == 0) puan += 1.5; // Clean Sheet Bonusu

            if (kirmiziKartGorduMu) puan = 0.0;

            // Sınırlandırma (0 ile 10 arası)
            if (puan > 10.0) puan = 10.0;
            if (puan < 0.0) puan = 0.0;

            return Math.Round(puan, 1);
        }
    }
}