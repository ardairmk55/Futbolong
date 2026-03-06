using System;

namespace Futbol
{
    // Bütün mevki sınıflarının atası olan Temel Sınıf
    public class Futbolcu
    {
        // 1. Statik ve Bellek Dostu Random
        protected static readonly Random rasgele = new Random();

        // 2. Temel Fiziksel Özellikler (Kimlik Kartı)
        public string isim;
        public double boy;

        // 3. Yetenek Puanları (0-100 arası)
        public int guc;
        public int hiz;
        public int teknik;
        public int atak;
        public int defans;
        public int kalecilik;

        // YENİ: Ultra Profesyonel Oyuncu Nitelikleri
        public int oyunZekasi;   // Doğru kararı verme ihtimalini artırır
        public int dayaniklilik; // Yorgunluğa direnç katsayısı
        public int tecrube;      // Kritik hataları (panik) önler

        // 4. Dinamik Durum Değişkenleri
        public int kondisyon;         // 100 üzerinden fiziksel enerji
        public int moral;             // 100 üzerinden psikolojik durum
        public double macPuani;       // 1.0 ile 10.0 arası maç sonu reytingi

        public bool sakatMi;          // Sakatlık durumu
        public int sariKartSayisi;    // Toplam sarı kart
        public bool kirmiziKartGorduMu; // Oyundan atılma durumu
        public int yapilanFaul;       // Disiplin istatistiği

        // --- YAPICI METOT (CONSTRUCTOR) ---
        // (Form2.cs'nin hata vermemesi için yeni özelliklere varsayılan olarak =75 atandı)
        public Futbolcu(string _isim, double _boy, int _guc, int _hiz, int _teknik, int _atak, int _defans, int _kalecilik, int _oyunZekasi = 75, int _dayaniklilik = 75, int _tecrube = 75)
        {
            this.isim = _isim;
            this.boy = _boy;
            this.guc = _guc;
            this.hiz = _hiz;
            this.teknik = _teknik;
            this.atak = _atak;
            this.defans = _defans;
            this.kalecilik = _kalecilik;

            this.oyunZekasi = _oyunZekasi;
            this.dayaniklilik = _dayaniklilik;
            this.tecrube = _tecrube;

            // Varsayılan maç başlangıç değerleri
            this.kondisyon = 100;
            this.moral = 100;
            this.macPuani = 6.0; // Her oyuncu maça 6 reyting ile başlar

            this.sakatMi = false;
            this.sariKartSayisi = 0;
            this.kirmiziKartGorduMu = false;
            this.yapilanFaul = 0;
        }

        // ========================================================================
        // --- GET/SET YERİNE KULLANILAN GÜVENLİ GÜNCELLEME METOTLARI ---
        // Bu metotlar değerlerin 0'ın altına inmesini veya 100'ü geçmesini engeller
        // ========================================================================

        public void KondisyonGuncelle(int eklenecekMiktar)
        {
            // Eğer eksi bir değer geliyorsa, oyuncunun dayanıklılığı efor kaybını azaltır!
            if (eklenecekMiktar < 0)
            {
                // Dayanıklılığı yüksek olan daha az yorulur (örn: dayanıklılık 80 ise %80 yansır)
                double direncCarpani = 1.0 - (this.dayaniklilik / 400.0);
                eklenecekMiktar = Convert.ToInt32(eklenecekMiktar * direncCarpani);
            }

            this.kondisyon += eklenecekMiktar;
            if (this.kondisyon > 100) this.kondisyon = 100;
            if (this.kondisyon < 0) this.kondisyon = 0;
        }

        public void MoralGuncelle(int eklenecekMiktar)
        {
            this.moral += eklenecekMiktar;
            if (this.moral > 100) this.moral = 100;
            if (this.moral < 0) this.moral = 0;
        }

        public void MacPuaniGuncelle(double eklenecekMiktar)
        {
            this.macPuani += eklenecekMiktar;
            if (this.macPuani > 10.0) this.macPuani = 10.0;
            if (this.macPuani < 1.0) this.macPuani = 1.0;
        }

        // --- GELİŞMİŞ FİZİK VE MANTIK KATMANI ---

        // Doğrusal Olmayan (Non-Linear) Yorgunluk Motoru
        protected double KondisyonCarpaniGetir()
        {
            double carpan = this.kondisyon / 100.0;

            // Tecrübeli oyuncular yorgun olsalar bile enerjilerini idareli kullanır!
            double tecrubeBonusu = this.tecrube / 200.0;

            if (this.kondisyon < 40) carpan *= (0.8 + tecrubeBonusu);
            if (this.kondisyon < 20) carpan *= (0.5 + tecrubeBonusu);

            if (carpan < 0.3) carpan = 0.3; // Ne kadar yorulursa yorulsun minimum %30 performans
            if (carpan > 1.0) carpan = 1.0;

            return carpan;
        }

        public bool OynayabilirMi()
        {
            // 5 kondisyonun altı kramptır, hamle yapamaz
            return !this.kirmiziKartGorduMu && !this.sakatMi && this.kondisyon > 5;
        }

        // --- DİSİPLİN VE SAĞLIK SİSTEMİ ---

        public string KartGor(bool direktKirmizi = false)
        {
            MacPuaniGuncelle(-1.5); // Kart görmek maç puanını çökertir

            if (direktKirmizi)
            {
                this.kirmiziKartGorduMu = true;
                return $"🟥 DİREKT KIRMIZI KART! {isim} acımasız bir müdahale yaptı ve atıldı!";
            }

            this.sariKartSayisi++;
            if (this.sariKartSayisi >= 2)
            {
                this.kirmiziKartGorduMu = true;
                return $"🟥 İKİNCİ SARI! {isim} takımını eksik bırakıyor!";
            }

            MoralGuncelle(-10); // Sarı kart moral bozar
            return $"🟨 SARI KART: Hakem {isim} için kartını çıkardı.";
        }

        public string DarbeAl(int siddet)
        {
            KondisyonGuncelle(-siddet);
            MoralGuncelle(-(siddet / 2));

            // Şiddet ve tecrübe bağlantısı (Tecrübeli oyuncular darbelerden daha iyi kaçar)
            int sakatlikRiski = siddet - (this.tecrube / 20);
            if (sakatlikRiski < 2) sakatlikRiski = 2; // Minimum %2 sakatlanma ihtimali

            if (rasgele.Next(0, 100) < sakatlikRiski)
            {
                this.sakatMi = true;
                return $"🚑 EYVAH! {isim} acı içinde yerde! Ciddi bir sakatlık geçiriyor!";
            }
            return $"⚠️ {isim} sert bir darbe aldı ama dişini sıkarak ayağa kalktı.";
        }

        // --- TEMEL AKSİYONLAR ---

        public virtual bool PasAt(Futbolcu rakip)
        {
            if (!OynayabilirMi()) return false;

            KondisyonGuncelle(-2);
            double moralCarpani = this.moral / 100.0;

            // Oyun Zekası pas katsayısını doğrudan etkiler
            int pasKatsayi = Convert.ToInt32((this.teknik * 0.7 + this.oyunZekasi * 0.3) * KondisyonCarpaniGetir() * moralCarpani);
            int engKatsayi = Convert.ToInt32(rakip.defans * 0.7 + rakip.hiz * 0.2 + rakip.oyunZekasi * 0.1);

            // Kritik Hata Şansı: Tecrübesi yüksek olanlar daha az hata yapar
            int kritikHataIhtimali = 10 - (this.tecrube / 20);
            if (kritikHataIhtimali < 1) kritikHataIhtimali = 1;

            if (rasgele.Next(0, 100) < kritikHataIhtimali)
            {
                MoralGuncelle(-5);
                MacPuaniGuncelle(-0.2);
                return false;
            }

            if (rasgele.Next(0, pasKatsayi + engKatsayi + 10) < pasKatsayi + 10)
            {
                MoralGuncelle(2);
                MacPuaniGuncelle(0.1);
                return true;
            }

            MoralGuncelle(-2);
            MacPuaniGuncelle(-0.1);
            return false;
        }

        public virtual bool CalimAt(Futbolcu rakip)
        {
            if (!OynayabilirMi()) return false;

            KondisyonGuncelle(-5);

            int calimKatsayi = Convert.ToInt32((this.teknik * 0.6 + this.hiz * 0.4) * KondisyonCarpaniGetir());
            int engKatsayi = Convert.ToInt32(rakip.defans * 0.6 + rakip.guc * 0.4);

            // %5 KRİTİK BAŞARI (Adrenalin Patlaması)
            if (rasgele.Next(0, 100) > 95)
            {
                MoralGuncelle(15);
                MacPuaniGuncelle(0.4); // Çalım şov maç puanını çok artırır
                return true;
            }

            if (rasgele.Next(0, calimKatsayi + engKatsayi) < calimKatsayi)
            {
                MoralGuncelle(5);
                MacPuaniGuncelle(0.2);
                return true;
            }

            MoralGuncelle(-5);
            MacPuaniGuncelle(-0.2);
            return false;
        }
    }
}