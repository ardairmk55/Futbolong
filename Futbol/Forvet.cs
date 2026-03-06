using System;

namespace Futbol
{
    public class Forvet : Futbolcu
    {
        // 1. İstatistikler
        public int golSayisi;
        public int cekilenSutSayisi;
        public int kullanilanPenalti;

        // 2. Forvete Özel Yetenekler
        public int bitiricilik;
        public int sogukkanlilik;
        public int kafaVurusu;
        public int sezgi;    // Ofsayta düşmeme ve doğru yerde bekleme
        public int ceviklik; // Akrobatik hareketler (Röveşata/Vole)

        // 3. Gelişmiş Constructor
        public Forvet(string _isim, double _boy, int _guc, int _hiz, int _teknik, int _atak, int _defans, int _kalecilik, int _bitiricilik, int _sogukkanlilik = 80, int _kafaVurusu = 80, int _sezgi = 80, int _ceviklik = 80)
            : base(_isim, _boy, _guc, _hiz, _teknik, _atak, _defans, _kalecilik)
        {
            this.bitiricilik = _bitiricilik;
            this.sogukkanlilik = _sogukkanlilik;
            this.kafaVurusu = _kafaVurusu;
            this.sezgi = _sezgi;
            this.ceviklik = _ceviklik;

            this.golSayisi = 0;
            this.cekilenSutSayisi = 0;
            this.kullanilanPenalti = 0;
        }

        // --- TEMEL ŞUT MEKANİKLERİ ---

        public bool SutAt(Futbolcu rakip)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-4); // Yeni güvenli metodu kullanıyoruz
            this.cekilenSutSayisi++;

            double verimlilik = KondisyonCarpaniGetir() * (this.moral / 100.0);

            int sutKatsayi = Convert.ToInt32((this.teknik * 0.3 + this.guc * 0.3 + this.bitiricilik * 0.4) * verimlilik);
            int engKatsayi = Convert.ToInt32(rakip.hiz * 0.3 + rakip.guc * 0.1 + rakip.defans * 0.2 + rakip.kalecilik * 0.4);

            if (rasgele.Next(0, sutKatsayi + engKatsayi) < sutKatsayi)
            {
                GolAtmaIslemleri(15);
                return true;
            }

            GolKacirmaIslemleri(5);
            return false;
        }

        public bool KafaVurusuYap(Futbolcu rakip)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-3);
            this.cekilenSutSayisi++;

            int boyAvantaji = (this.boy > 1.70) ? Convert.ToInt32((this.boy - 1.70) * 100) : 0;
            int kafaSutKatsayi = Convert.ToInt32((this.guc * 0.3 + this.kafaVurusu * 0.4 + boyAvantaji) * KondisyonCarpaniGetir());
            int engKatsayi = Convert.ToInt32(rakip.guc * 0.3 + rakip.defans * 0.3 + rakip.kalecilik * 0.4);

            if (rasgele.Next(0, kafaSutKatsayi + engKatsayi) < kafaSutKatsayi)
            {
                GolAtmaIslemleri(15);
                return true;
            }

            GolKacirmaIslemleri(5);
            return false;
        }

        public bool PlaseVurusYap(Futbolcu kaleci)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-2);
            this.cekilenSutSayisi++;

            int plaseKatsayi = Convert.ToInt32((this.teknik * 0.6 + this.sogukkanlilik * 0.4) * (this.moral / 100.0));
            int kaleciSezgi = Convert.ToInt32(kaleci.kalecilik * 0.6 + kaleci.hiz * 0.4);

            if (rasgele.Next(0, plaseKatsayi + kaleciSezgi) < plaseKatsayi)
            {
                GolAtmaIslemleri(20);
                return true;
            }

            GolKacirmaIslemleri(5);
            return false;
        }

        public bool PenaltiKullan(Futbolcu kaleci)
        {
            if (!OynayabilirMi()) return false;

            this.kullanilanPenalti++;
            this.cekilenSutSayisi++;

            int sutGucu = Convert.ToInt32(this.bitiricilik * 0.6 + this.sogukkanlilik * 0.4);
            int kaleciRefleks = Convert.ToInt32(kaleci.kalecilik * 0.8 + kaleci.hiz * 0.2);

            if (rasgele.Next(0, sutGucu + kaleciRefleks) < sutGucu + 30)
            {
                GolAtmaIslemleri(5);
                return true;
            }

            GolKacirmaIslemleri(20);
            return false;
        }

        // --- ULTRA GELİŞMİŞ YENİ MEKANİKLER ---

        // 1. Röveşata Vuruşu
        public bool RovesataVur(Futbolcu kaleci)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-8);
            this.cekilenSutSayisi++;

            int rovesataGucu = Convert.ToInt32((this.teknik * 0.4 + this.ceviklik * 0.4 + this.bitiricilik * 0.2) * KondisyonCarpaniGetir());
            int kaleciGucu = Convert.ToInt32(kaleci.kalecilik * 0.7 + kaleci.hiz * 0.3);

            if (rasgele.Next(0, 100) > 80)
            {
                if (rovesataGucu + rasgele.Next(0, 30) > kaleciGucu)
                {
                    GolAtmaIslemleri(50);
                    return true;
                }
            }
            else if (rasgele.Next(0, 100) < 10)
            {
                // HATANIN ÇÖZÜLDÜĞÜ YER: Röveşatada ters düşme şiddeti 15 olarak belirlendi
                this.DarbeAl(15);
            }

            GolKacirmaIslemleri(10);
            return false;
        }

        // 2. Gelişine Vole
        public bool GelisineVoleVur(Futbolcu kaleci)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-5);
            this.cekilenSutSayisi++;

            int voleKatsayi = Convert.ToInt32((this.teknik * 0.5 + this.atak * 0.3 + this.guc * 0.2) * KondisyonCarpaniGetir());
            int kaleciGucu = Convert.ToInt32(kaleci.kalecilik * 0.6 + kaleci.hiz * 0.4);

            if (rasgele.Next(0, voleKatsayi + kaleciGucu) < (voleKatsayi - 10))
            {
                GolAtmaIslemleri(25);
                return true;
            }

            GolKacirmaIslemleri(5);
            return false;
        }

        // 3. Defans Arkasına Sarkma
        public bool DefansArkasinaSark(Futbolcu rakipDefans)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-4);

            int kacuSkatSayisi = Convert.ToInt32((this.hiz * 0.6 + this.sezgi * 0.4) * KondisyonCarpaniGetir());
            int defansHamle = Convert.ToInt32((rakipDefans.defans * 0.5 + rakipDefans.hiz * 0.5) * (rakipDefans.moral / 100.0));

            if (rasgele.Next(0, kacuSkatSayisi + defansHamle) < kacuSkatSayisi)
            {
                this.MoralGuncelle(10);
                return true;
            }

            this.MoralGuncelle(-5);
            return false;
        }

        // 4. İleride Pres
        public bool IleridePresYap(Futbolcu rakipDefans)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-5);
            int presKatsayisi = Convert.ToInt32((this.hiz * 0.5 + this.guc * 0.3 + this.atak * 0.2) * KondisyonCarpaniGetir());
            int defansKatsayisi = Convert.ToInt32(rakipDefans.teknik * 0.6 + rakipDefans.guc * 0.4);

            if (rasgele.Next(0, presKatsayisi + defansKatsayisi) < presKatsayisi)
            {
                this.MoralGuncelle(5);
                return true;
            }

            if (rasgele.Next(0, 100) < 25)
            {
                // HATANIN ÇÖZÜLDÜĞÜ YER: İkili mücadelede alınan darbe şiddeti 10 olarak belirlendi
                this.DarbeAl(10);
            }
            return false;
        }

        // 5. Top Saklama
        public bool TopSakla(Futbolcu rakipDefans)
        {
            if (!OynayabilirMi()) return false;

            this.KondisyonGuncelle(-3);

            int saklamaGucu = Convert.ToInt32((this.guc * 0.6 + this.teknik * 0.4) * KondisyonCarpaniGetir());
            if (this.boy > 1.85) saklamaGucu += 15;

            int defansBaskisi = Convert.ToInt32(rakipDefans.guc * 0.5 + rakipDefans.defans * 0.5);

            if (rasgele.Next(0, saklamaGucu + defansBaskisi) < saklamaGucu)
            {
                this.MoralGuncelle(5);
                return true;
            }

            this.MoralGuncelle(-2);
            return false;
        }

        // --- YARDIMCI METOTLAR ---

        private void GolAtmaIslemleri(int moralArtisi)
        {
            this.golSayisi++;
            this.MoralGuncelle(moralArtisi);
            this.MacPuaniGuncelle(1.0); // Gol atmak reytingi çok artırır
        }

        private void GolKacirmaIslemleri(int moralDususu)
        {
            this.MoralGuncelle(-moralDususu);
            this.MacPuaniGuncelle(-0.1); // Kaçan şut reytingi çok hafif düşürür
        }
    }
}