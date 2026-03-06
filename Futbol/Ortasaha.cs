using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Futbol
{
    // "public" anahtarı ile Takim.cs tarafından doğrudan erişilebilir durumda
    public class Ortasaha : Futbolcu
    {
        // 1. Mevkiye Özel İstatistikler
        public int basariliPasSayisi;
        public int kilitPasSayisi;
        public int kazanilanTopSayisi;
        public int asistSayisi; // Yeni: Forvete gol attıran paslar için

        // 2. Yapıcı Metot (Constructor) ve Kalıtım (Inheritance)
        public Ortasaha(string _isim, double _boy, int _guc, int _hiz, int _teknik, int _atak, int _defans, int _kalecilik)
            : base(_isim, _boy, _guc, _hiz, _teknik, _atak, _defans, _kalecilik)
        {
            basariliPasSayisi = 0;
            kilitPasSayisi = 0;
            kazanilanTopSayisi = 0;
            asistSayisi = 0;
        }

        // --- HÜCUM VE OYUN KURMA METOTLARI ---

        // 1. Tehlikeli Ara Pas (Asist potansiyeli yüksek)
        public bool AraPasVer(Futbolcu rakipDefans)
        {
            if (kirmiziKartGorduMu || sakatMi) return false;

            this.kondisyon -= 3;
            // Orta sahanın oyun görüşü (teknik ve moral birleşimi)
            double vizyonCarpani = (this.teknik * 0.7 + this.moral * 0.3) / 100.0;

            int pasKatsayi = Convert.ToInt32((this.teknik * 0.50 + this.atak * 0.30 + this.hiz * 0.20) * KondisyonCarpaniGetir() * vizyonCarpani);
            int engKatsayi = Convert.ToInt32(rakipDefans.defans * 0.50 + rakipDefans.hiz * 0.30 + rakipDefans.guc * 0.20);

            if (rasgele.Next(0, pasKatsayi + engKatsayi) < pasKatsayi)
            {
                basariliPasSayisi++;
                kilitPasSayisi++;
                this.moral += 5; // Başarılı kilit pas morali artırır
                return true;
            }
            this.moral -= 2;
            return false;
        }

        // 2. Rutin Oyun Kurma (Düşük riskli, takımı rahatlatan paslar)
        public bool OyunKur(Futbolcu rakip)
        {
            if (kirmiziKartGorduMu || sakatMi) return false;

            this.kondisyon -= 1;
            int pasKatsayi = Convert.ToInt32((this.teknik * 0.75 + this.moral * 0.25) * KondisyonCarpaniGetir());

            // Oyun kurarken pasın geçme ihtimali her zaman daha yüksektir (+40 Bonus)
            if (rasgele.Next(0, 100) < pasKatsayi + 40)
            {
                basariliPasSayisi++;
                return true;
            }
            return false;
        }

        // --- ŞUT VE DURAN TOP METOTLARI ---

        // 3. Uzaktan Şut (Sürpriz goller için)
        public bool UzaktanSutCek(Futbolcu kaleci)
        {
            if (kirmiziKartGorduMu || sakatMi) return false;

            this.kondisyon -= 4;
            int sutKatsayi = Convert.ToInt32((this.teknik * 0.50 + this.guc * 0.40 + this.atak * 0.10) * KondisyonCarpaniGetir());
            int kurtarisKatsayi = Convert.ToInt32(kaleci.kalecilik * 0.70 + (kaleci.boy > 1.85 ? 15 : 0));

            if (rasgele.Next(0, sutKatsayi + kurtarisKatsayi) < sutKatsayi)
            {
                this.moral += 20;
                return true;
            }
            return false;
        }

        // 4. Frikik Kullanma
        public bool FrikikKullan(Futbolcu kaleci)
        {
            if (kirmiziKartGorduMu || sakatMi) return false;

            this.kondisyon -= 2;
            // Frikikte yorgunluktan ziyade saf teknik ve moral önemlidir
            int frikikKatsayi = Convert.ToInt32((this.teknik * 0.80 + this.moral * 0.20));
            int kaleciEngeli = Convert.ToInt32(kaleci.kalecilik * 0.75 + kaleci.hiz * 0.25);

            if (rasgele.Next(0, frikikKatsayi + kaleciEngeli) < frikikKatsayi)
            {
                this.moral += 25;
                return true;
            }
            return false;
        }

        // --- SAVUNMA METOTLARI ---

        // 5. Pres Yapma ve Top Kesme
        public bool TopKes(Futbolcu rakip)
        {
            if (kirmiziKartGorduMu || sakatMi) return false;

            this.kondisyon -= 3;
            int presKatsayi = Convert.ToInt32((this.defans * 0.60 + this.hiz * 0.20 + this.guc * 0.20) * KondisyonCarpaniGetir());
            int rakipKatsayi = Convert.ToInt32(rakip.teknik * 0.70 + rakip.hiz * 0.30);

            if (rasgele.Next(0, presKatsayi + rakipKatsayi) < presKatsayi)
            {
                kazanilanTopSayisi++;
                this.moral += 5;
                return true;
            }
            // Topu kapamazsa faul yapma ihtimali (Orta sahalar genelde taktik faul yapar)
            if (rasgele.Next(0, 100) < 20) this.yapilanFaul++;

            return false;
        }

        // --- 6. MAÇ PUANI HESAPLAMA (Performans Analizi) ---
        public double MacPuaniHesapla()
        {
            double puan = 6.0; // Orta sahalar oyunun merkezinde olduğu için 6 ile başlar

            puan += (basariliPasSayisi * 0.05); // Her pas +0.05
            puan += (kilitPasSayisi * 0.5);     // Her kilit pas +0.5
            puan += (asistSayisi * 2.0);        // Her asist +2.0
            puan += (kazanilanTopSayisi * 0.4); // Her top kapma +0.4
            puan -= (yapilanFaul * 0.3);        // Her faul -0.3

            if (kirmiziKartGorduMu) puan = 0;

            if (puan > 10) puan = 10;
            if (puan < 0) puan = 0;

            return Math.Round(puan, 1);
        }
    }
}