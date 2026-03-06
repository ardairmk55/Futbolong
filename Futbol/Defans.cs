using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Futbol
{
    public class Defans : Futbolcu
    {
        // 1. Mevkiye Özel Gelişmiş İstatistikler
        public int basariliMudehale;
        public int yaptirilanPenalti;
        public int kazanilanHavaTopu;
        public int topKesme; // Pas arası yapma istatistiği

        // 2. Yapıcı Metot (Constructor)
        public Defans(string _isim, double _boy, int _guc, int _hiz, int _teknik, int _atak, int _defans, int _kalecilik)
            : base(_isim, _boy, _guc, _hiz, _teknik, _atak, _defans, _kalecilik)
        {
            basariliMudehale = 0;
            yaptirilanPenalti = 0;
            kazanilanHavaTopu = 0;
            topKesme = 0;
            // yapilanFaul, base sınıftan (Futbolcu.cs) otomatik yönetilir.
        }

        // --- 1. TEMEL SAVUNMA: TOP KAPMA (Geliştirilmiş Faul Sistemi) ---
        public bool TopKap(Futbolcu rakip, bool cezaSahasiIciMi = false)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-3);
            double verimlilik = KondisyonCarpaniGetir() * (this.moral / 100.0);

            // Defans gücü hesabı (Güç ve Tecrübe faktörü eklendi)
            int kapKatsayi = Convert.ToInt32((this.defans * 0.50 + this.guc * 0.30 + this.tecrube * 0.20) * verimlilik);
            int rakipKatsayi = Convert.ToInt32(rakip.teknik * 0.50 + rakip.hiz * 0.50);

            if (rasgele.Next(0, kapKatsayi + rakipKatsayi) < kapKatsayi)
            {
                basariliMudehale++;
                this.MoralGuncelle(5);
                this.MacPuaniGuncelle(0.3);
                return true;
            }
            else
            {
                // Müdahale başarısız! Faul ihtimali (Teknik düşükse risk artar)
                int faulRiski = 35 + (100 - this.teknik) / 4;

                if (rasgele.Next(0, 100) < faulRiski)
                {
                    FaulYap(cezaSahasiIciMi);
                    // HATANIN ÇÖZÜLDÜĞÜ YER: Rakibe 15 şiddetinde darbe veriliyor
                    rakip.DarbeAl(15);
                }

                this.MoralGuncelle(-3);
                return false;
            }
        }

        // --- 2. ÖZEL YETENEK: KAYARAK MÜDAHALE (Yüksek Risk - Yüksek Ödül) ---
        public bool KayarakMudahale(Futbolcu rakip)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-6); // Çok yorucu

            // Başarı şansı hız ve defans becerisine bağlıdır
            int basariSansi = Convert.ToInt32((this.defans * 0.6 + this.hiz * 0.4) * KondisyonCarpaniGetir());

            if (rasgele.Next(0, 100) < basariSansi)
            {
                basariliMudehale++;
                this.MoralGuncelle(10);
                this.MacPuaniGuncelle(0.5);
                return true; // Topu tertemiz aldı
            }
            else
            {
                // Kayarak müdahale başarısızsa kart görme riski %80'dir
                FaulYap(false);
                rakip.DarbeAl(20); // Çok sert bir darbe
                return false;
            }
        }

        // --- 3. KART VE PENALTI SİSTEMİ ---
        private void FaulYap(bool cezaSahasiIciMi)
        {
            this.yapilanFaul++;
            int sertlik = (this.guc - this.tecrube) + rasgele.Next(0, 50);

            if (sertlik > 60)
            {
                int kartZari = rasgele.Next(0, 100);
                if (kartZari > 90) this.KartGor(true); // Direkt Kırmızı
                else if (kartZari > 50) this.KartGor(false); // Sarı Kart
            }

            if (cezaSahasiIciMi)
            {
                this.yaptirilanPenalti++;
                this.MacPuaniGuncelle(-2.0);
            }
        }

        // --- 4. HAVA TOPU MÜCADELESİ (Boy ve Güç Odaklı) ---
        public bool KafaTopunaCik(Futbolcu rakip)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-2);

            // Boy farkı (santim bazında) direkt etkili
            int boyFarki = Convert.ToInt32((this.boy - rakip.boy) * 100);
            int defKafa = Convert.ToInt32((this.defans * 0.4 + this.guc * 0.5 + boyFarki) * KondisyonCarpaniGetir());
            int rakKafa = Convert.ToInt32(rakip.atak * 0.4 + rakip.guc * 0.6);

            if (rasgele.Next(0, defKafa + rakKafa) < defKafa)
            {
                basariliMudehale++;
                kazanilanHavaTopu++;
                this.MacPuaniGuncelle(0.2);
                return true;
            }
            return false;
        }

        // --- 5. GERİDEN OYUN KURMA ---
        public bool GeridenOyunKur()
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-1);

            // Oyun kurarken teknik %70, oyun zekası %30 etkilidir
            int pasKalitesi = Convert.ToInt32((this.teknik * 0.7 + this.oyunZekasi * 0.3) * KondisyonCarpaniGetir());

            if (rasgele.Next(0, 100) < pasKalitesi)
            {
                this.MacPuaniGuncelle(0.1);
                return true;
            }
            this.MacPuaniGuncelle(-0.3); // Tehlikeli bölgede top kaybı reytingi düşürür
            return false;
        }

        // --- 6. MAÇ PUANI ANALİZİ ---
        public double MacPuaniHesapla()
        {
            if (kirmiziKartGorduMu) return 0.0;

            double puan = 6.0;

            puan += (basariliMudehale * 0.4);
            puan += (kazanilanHavaTopu * 0.2);
            puan += (topKesme * 0.3);
            puan -= (yapilanFaul * 0.3);
            puan -= (yaptirilanPenalti * 2.0);

            if (sariKartSayisi > 0) puan -= 1.0;

            // Sınırlandırma
            if (puan > 10.0) puan = 10.0;
            if (puan < 1.0) puan = 1.0;

            return Math.Round(puan, 1);
        }
    }
}