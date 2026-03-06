using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Futbol
{
    public partial class Form1 : Form
    {
        // --- DATA LAYER ---
        public Takim evSahibi, deplasman;
        int dakika = 0, evSkor = 0, depSkor = 0, sahaBolgesi = 0, momentum = 0, ekSure = 0;
        bool topEvSahibinde = true, ilkYariBitti = false, macBitti = false, araDevreMode = false;
        Random rnd = new Random();

        // --- ZAMANLAYICI ---
        Timer macMotoru;

        // --- UI COMPONENTS ---
        Panel pnlEvKontrol, pnlDepKontrol, pnlMerkez, pnlHeader, pnlSahaRadar;
        Label lblSkor, lblDakika, lblSahaDurumu, lblEvIstatistik, lblDepIstatistik, lblMomentum;
        Label lblEvBaslik, lblDepBaslik; 
        ListBox macLog;
        ProgressBar pbSahaIlerleme;

        Button[] btnEvAksiyonlari = new Button[3];
        Button[] btnDepAksiyonlari = new Button[3];

        // --- DİNAMİK YORUM HAVUZU (GENİŞLETİLDİ) ---
        string[] pasOk = { "İğne deliğinden geçirdi pası!", "Boş alanı çok iyi gördü.", "Şık bir pasla takımı ileri taşıdı.", "Maestro gibi pas dağıtıyor.", "Tek top, harika bir organizasyon!", "Rakip savunmanın dengesini bozan bir pas." };
        string[] pasFail = { "Pasın şiddeti çok fazla!", "Kötü bir pas, savunma araya girdi.", "Arkadaşıyla anlaşamadı, top kaybı.", "Pas hatası! Tribünlerden homurdanmalar yükseliyor.", "Zeminin azizliğine uğradı, top rakipte." };
        string[] calimOk = { "Rakibini adeta bakkala gönderdi!", "Müthiş bir vücut çalımı!", "Rakiplerini tek tek ipe diziyor!", "Bileklerine çok hakim!", "Klas bir hareketle rakibinden sıyrıldı." };
        string[] sutKacti = { "Top direği yalayarak dışarı çıktı!", "Kaleci mükemmel uzadı ve topu kornere çeldi!", "Tribünler 'nasıl kaçar' diyor!", "Farklı şekilde auta gitti, kötü bir vuruş.", "Savunma son anda araya girdi ve şutu blokladı!", "Top adeta göklere uçtu!", "Kaleci kalesinde devleşti! İnanılmaz bir kurtarış!" };

        // MODERN RENK PALETİ
        Color renkHucum = Color.FromArgb(39, 174, 96);       // Zümrüt Yeşili
        Color renkHucumHover = Color.FromArgb(46, 204, 113); 
        Color renkSavunma = Color.FromArgb(192, 57, 43);     // Kızıl Kırmızı
        Color renkSavunmaHover = Color.FromArgb(231, 76, 60);

        public Form1(Takim ev, Takim dep)
        {
            evSahibi = ev; deplasman = dep;
            InitializeComponent();

            this.Size = new Size(1300, 900);
            this.Text = "Süper Lig FM 2026 - Gümüşhane Match Engine Pro";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // --- ARKA PLAN GÖRSELİ ENTEGRASYONU ---
            this.BackgroundImage = Properties.Resources.sahaa;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            StadyumuInsaEt();
            ZamanMotorunuBaslat();
        }

        private void ZamanMotorunuBaslat()
        {
            macMotoru = new Timer();
            macMotoru.Interval = 1200; // Gerçekçi akış hızı
            macMotoru.Tick += MacMotoru_Tick;
            macMotoru.Start();
        }

        private void MacMotoru_Tick(object sender, EventArgs e)
        {
            if (macBitti || araDevreMode) return;

            dakika++;
            if (rnd.Next(0, 10) > 7) momentum = Math.Max(0, momentum - 1); // Pasiflik momentumu düşürür

            ZamanKontrolu();
            ArayuzGuncelle();
        }

        private void ZamanKontrolu()
        {
            if (dakika >= 45 && !ilkYariBitti)
            {
                if (ekSure == 0) { ekSure = rnd.Next(1, 5); SpikerMesaji($"⏱️ İlk yarı sonuna +{ekSure} dakika eklendi."); }
                if (dakika >= 45 + ekSure) { DevreArasiBaslat(); }
            }
            else if (dakika >= 90)
            {
                if (ekSure < 5) { ekSure = 90 + rnd.Next(2, 7); SpikerMesaji($"⏱️ Maç sonuna +{ekSure - 90} dakika eklendi."); }
                if (dakika >= ekSure) { OyunBitir(); }
            }
        }

        private void StadyumuInsaEt()
        {
            this.Controls.Clear();

            // 1. HEADER 
            pnlHeader = new Panel { Size = new Size(1300, 140), Dock = DockStyle.Top, BackColor = Color.FromArgb(200, 18, 18, 22) }; 
            lblDakika = new Label { Text = "00:00", ForeColor = Color.FromArgb(241, 196, 15), Font = new Font("Consolas", 26, FontStyle.Bold), Size = new Size(200, 40), Location = new Point(550, 15), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };
            lblSkor = new Label { Text = $"{evSahibi.takimIsmi.ToUpper()}   0 - 0   {deplasman.takimIsmi.ToUpper()}", ForeColor = Color.White, Font = new Font("Segoe UI Black", 36, FontStyle.Bold), Size = new Size(1300, 80), Location = new Point(0, 55), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };
            pnlHeader.Controls.Add(lblDakika); pnlHeader.Controls.Add(lblSkor); this.Controls.Add(pnlHeader);

            // 2. SAHA RADARI 
            pnlSahaRadar = new Panel { Size = new Size(740, 90), Location = new Point(280, 160), BackColor = Color.FromArgb(200, 25, 30, 25) };
            lblSahaDurumu = new Label { Text = "⚽ MAÇ BAŞLIYOR", ForeColor = Color.FromArgb(46, 204, 113), Font = new Font("Segoe UI Black", 14, FontStyle.Bold), Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, Height = 40, BackColor = Color.Transparent };
            pbSahaIlerleme = new ProgressBar { Size = new Size(680, 10), Location = new Point(30, 55), Maximum = 2, Value = 0 }; 
            pnlSahaRadar.Controls.Add(lblSahaDurumu); pnlSahaRadar.Controls.Add(pbSahaIlerleme); this.Controls.Add(pnlSahaRadar);

            // 3. KONTROL PANELLERİ
            pnlEvKontrol = PanelUret(25, 160, Color.Crimson, evSahibi.takimIsmi, true, btnEvAksiyonlari, out lblEvBaslik);
            pnlDepKontrol = PanelUret(1035, 160, Color.DodgerBlue, deplasman.takimIsmi, false, btnDepAksiyonlari, out lblDepBaslik);
            this.Controls.Add(pnlEvKontrol); this.Controls.Add(pnlDepKontrol);

            // 4. ANLATIM EKRANI 
            pnlMerkez = new Panel { Size = new Size(740, 520), Location = new Point(280, 270), BackColor = Color.FromArgb(190, 15, 15, 18) };
            macLog = new ListBox
            {
                Location = new Point(15, 15), Size = new Size(710, 490),
                BackColor = Color.FromArgb(25, 25, 30), ForeColor = Color.WhiteSmoke, 
                Font = new Font("Segoe UI Semibold", 12), BorderStyle = BorderStyle.None,
                ItemHeight = 28, HorizontalScrollbar = true
            };
            pnlMerkez.Controls.Add(macLog); this.Controls.Add(pnlMerkez);

            // 5. ALT ANALİZ PANELİ
            lblEvIstatistik = new Label { ForeColor = Color.White, Location = new Point(35, 810), AutoSize = true, Font = new Font("Segoe UI Semibold", 12), BackColor = Color.Transparent };
            lblDepIstatistik = new Label { ForeColor = Color.White, Location = new Point(1045, 810), AutoSize = true, Font = new Font("Segoe UI Semibold", 12), TextAlign = ContentAlignment.TopRight, BackColor = Color.Transparent };
            lblMomentum = new Label { ForeColor = Color.FromArgb(243, 156, 18), Location = new Point(550, 815), AutoSize = true, Font = new Font("Segoe UI Black", 14, FontStyle.Bold), BackColor = Color.Transparent };

            this.Controls.Add(lblEvIstatistik); this.Controls.Add(lblDepIstatistik); this.Controls.Add(lblMomentum);

            SpikerMesaji("🏟️ Gümüşhane Arena'da hakem düdüğünü çalıyor ve dev maç başlıyor!");
            ButonlariGuncelle();
            ArayuzGuncelle();
        }

        private Panel PanelUret(int x, int y, Color c, string isim, bool evMi, Button[] butonDizisi, out Label baslikLabel)
        {
            Panel p = new Panel { Location = new Point(x, y), Size = new Size(240, 630), BackColor = Color.FromArgb(180, 20, 20, 24) };

            baslikLabel = new Label { Text = isim.ToUpper(), Dock = DockStyle.Top, Height = 55, BackColor = c, ForeColor = Color.White, Font = new Font("Segoe UI Black", 13), TextAlign = ContentAlignment.MiddleCenter };
            p.Controls.Add(baslikLabel);

            for (int i = 0; i < 3; i++)
            {
                Button b = new Button { Location = new Point(20, 95 + (i * 115)), Size = new Size(200, 80), FlatStyle = FlatStyle.Flat, ForeColor = Color.White, Font = new Font("Segoe UI Black", 11), Cursor = Cursors.Hand };
                b.FlatAppearance.BorderSize = 0; 
                int butonIndeksi = i;
                b.Click += (s, e) => { if (araDevreMode) DevreArasiniBitir(); else HamleYap(evMi, butonIndeksi); };

                b.MouseEnter += (s, e) => {
                    if (b.Text.Contains("PAS") || b.Text.Contains("ÇALIM") || b.Text.Contains("ŞUT")) b.BackColor = renkHucumHover;
                    else b.BackColor = renkSavunmaHover;
                };
                b.MouseLeave += (s, e) => ButonlariGuncelle(); 

                butonDizisi[i] = b;
                p.Controls.Add(b);
            }
            return p;
        }

        private void ButonlariGuncelle()
        {
            string[] hucum = { "⚽ PASLA İLERLE", "🔥 ÇALIM DENE", "🎯 ŞUT ÇEK!" };
            string[] savunma = { "🏃‍♂️ PRES YAP", "🪓 KAYARAK MÜDAHALE", "🛑 TAKTİK FAUL" };

            for (int i = 0; i < 3; i++)
            {
                btnEvAksiyonlari[i].Text = topEvSahibinde ? hucum[i] : savunma[i];
                btnEvAksiyonlari[i].BackColor = topEvSahibinde ? renkHucum : renkSavunma;

                btnDepAksiyonlari[i].Text = !topEvSahibinde ? hucum[i] : savunma[i];
                btnDepAksiyonlari[i].BackColor = !topEvSahibinde ? renkHucum : renkSavunma;
            }
        }

        // =========================================================================================
        // --- ULTRA GELİŞMİŞ SİMÜLASYON MOTORU (BACKEND) ---
        // =========================================================================================
        private void HamleYap(bool hamleyiYapanEvMi, int butonIndeksi)
        {
            if (macBitti || araDevreMode) return;

            bool hucumMuYapiyor = (hamleyiYapanEvMi == topEvSahibinde);
            Takim saldiranTakim = topEvSahibinde ? evSahibi : deplasman;
            Takim savunanTakim = topEvSahibinde ? deplasman : evSahibi;

            Futbolcu aktifHucumcu = GetAktifOyuncu(saldiranTakim, true);
            Futbolcu aktifSavunmaci = GetAktifOyuncu(savunanTakim, false);
            Kaleci k = (Kaleci)savunanTakim.MevkiGetir("Kaleci").FirstOrDefault();

            if (aktifHucumcu == null || aktifSavunmaci == null) return;

            // YENİ: YAPAY ZEKA MENAJER KONTROLÜ (Skora göre taktik değiştirir)
            string taktikYorumu = saldiranTakim.YapayZekaTaktikKontrolu(dakika, topEvSahibinde ? evSkor : depSkor, topEvSahibinde ? depSkor : evSkor);
            if (!string.IsNullOrEmpty(taktikYorumu)) SpikerMesaji(taktikYorumu);

            bool basari = false;
            string yorum = "";

            // Ev Sahibi Avantajı (%5 Ekstra Güç)
            int evSahibiBonusu = (hamleyiYapanEvMi) ? 5 : 0;

            if (hucumMuYapiyor)
            {
                dakika++;
                
                // Yorgunluk Çarpanı: Kondisyon 40'ın altındaysa oyuncunun gücü dramatik düşer
                double yorgunlukCarpani = (aktifHucumcu.kondisyon < 40) ? 0.7 : 1.0;

                if (butonIndeksi == 0) // PAS
                {
                    int pasGucu = Convert.ToInt32(((aktifHucumcu.teknik * 2 + aktifHucumcu.hiz) / 3 * yorgunlukCarpani)) + rnd.Next(0, 30) + evSahibiBonusu;
                    int defansGucu = (aktifSavunmaci.defans + aktifSavunmaci.hiz) / 2 + rnd.Next(0, 25);
                    
                    // Kondisyon 20'nin altındaysa %15 ihtimalle kendi kendine topu taca atar (Gerçekçilik)
                    if (aktifHucumcu.kondisyon < 20 && rnd.Next(0, 100) < 15)
                    {
                        basari = false; yorum = "⚠️ Çok yorgun, pası doğrudan taca attı!";
                    }
                    else
                    {
                        basari = pasGucu > defansGucu;
                        yorum = basari ? pasOk[rnd.Next(pasOk.Length)] : pasFail[rnd.Next(pasFail.Length)];
                    }
                    if (basari) { momentum++; if (sahaBolgesi < 2) sahaBolgesi++; }
                }
                else if (butonIndeksi == 1) // ÇALIM
                {
                    int calimGucu = Convert.ToInt32(((aktifHucumcu.hiz + aktifHucumcu.teknik * 2) / 3 * yorgunlukCarpani)) + rnd.Next(0, 40) + evSahibiBonusu;
                    int defansGucu = (aktifSavunmaci.hiz + aktifSavunmaci.defans * 2) / 3 + rnd.Next(0, 30);
                    
                    basari = calimGucu > defansGucu;
                    yorum = basari ? calimOk[rnd.Next(calimOk.Length)] : $"❌ {aktifSavunmaci.isim} vücudunu araya koyarak topu çalıyor!";
                    if (basari) { momentum += 2; if (sahaBolgesi < 2) sahaBolgesi++; }
                }
                else if (butonIndeksi == 2) // ŞUT (GERÇEKÇİ MATEMATİK)
                {
                    if (sahaBolgesi < 2) { yorum = "⚠️ Çok uzak! Şut cılız kaldı ve savunmadan döndü."; basari = false; }
                    else
                    {
                        int bitiricilikBonus = (aktifHucumcu is Forvet f) ? f.bitiricilik : 50; 
                        
                        // Güçleri toplamıyoruz, ORTALAMASINI alıyoruz (Böylece gol oranı düşer)
                        int hucumOrtalamasi = Convert.ToInt32(((aktifHucumcu.atak + aktifHucumcu.guc + bitiricilikBonus) / 3) * yorgunlukCarpani);
                        int kaleciOrtalamasi = (k.kalecilik + k.refleks) / 2;

                        // Şut şansı hesaplama (Momentum ve Ev sahibi bonusu eklendi)
                        int sutZari = hucumOrtalamasi + (momentum * 4) + rnd.Next(0, 50) + evSahibiBonusu;
                        int kurtarisZari = kaleciOrtalamasi + 15 + rnd.Next(0, 60); // Kaleciye defansif avantaj

                        if (sutZari > kurtarisZari)
                        {
                            basari = true;
                            if (topEvSahibinde) evSkor++; else depSkor++;
                            yorum = $"🔴 GOOOOOOOOL! İnanılmaz bir sevinç! Ağları havalandıran isim: {aktifHucumcu.isim.ToUpper()}!";
                            saldiranTakim.MoralGuncelle(true); savunanTakim.MoralGuncelle(false);
                            sahaBolgesi = 1;
                            topEvSahibinde = !topEvSahibinde;
                        }
                        else 
                        { 
                            basari = false; 
                            yorum = sutKacti[rnd.Next(sutKacti.Length)]; 
                            k.moral = Math.Min(100, k.moral + 5); // Kurtarış morali artırır
                        }
                    }
                    momentum = 0; 
                }

                if (!basari && hucumMuYapiyor && butonIndeksi != 2)
                {
                    topEvSahibinde = !topEvSahibinde;
                    sahaBolgesi = 0;
                    momentum = 0;
                }
            }
            else // SAVUNMA HAMLESİ
            {
                // Savunan takım da ev sahibi avantajından yararlanır
                if (butonIndeksi == 0) // PRES
                {
                    int presGucu = (aktifSavunmaci.hiz + aktifSavunmaci.defans) / 2 + rnd.Next(0, 20) + evSahibiBonusu;
                    int hucumGucu = (aktifHucumcu.teknik + aktifHucumcu.hiz) / 2 + rnd.Next(0, 20);

                    basari = presGucu > hucumGucu;
                    if (basari) { yorum = $"🏃‍♂️ {aktifSavunmaci.isim} agresif presle topu kazandı!"; topEvSahibinde = !topEvSahibinde; momentum = 1; }
                    else { yorum = $"⚠️ {aktifHucumcu.isim} presten şık sıyrıldı!"; momentum += 1; basari = true; }
                }
                else if (butonIndeksi == 1) // KAYARAK MÜDAHALE
                {
                    int risk = rnd.Next(0, 100);
                    // Defans oyuncularının top çalma başarısı ortasahalara göre daha yüksektir
                    int basariLimiti = (aktifSavunmaci is Defans) ? 50 : 35; 

                    if (risk < basariLimiti)
                    {
                        yorum = $"🪓 {aktifSavunmaci.isim}'den jilet gibi kayarak müdahale! Topu tertemiz aldı.";
                        topEvSahibinde = !topEvSahibinde; momentum = 2;
                    }
                    else if (risk < 85)
                    {
                        aktifSavunmaci.yapilanFaul++;
                        yorum = $"🛑 Faul! {aktifSavunmaci.isim} zamanlamayı tutturamadı. Hakem serbest vuruş veriyor.";
                        momentum += 1;
                    }
                    else
                    {
                        aktifSavunmaci.sariKartSayisi++;
                        yorum = $"🟨 SARI KART! {aktifSavunmaci.isim} rakibini sert biçti!";
                        if (aktifSavunmaci.sariKartSayisi == 2) { aktifSavunmaci.kirmiziKartGorduMu = true; yorum += " 🟥 KIZARDI! İkinci sarıdan atılıyor!"; }
                    }
                }
                else if (butonIndeksi == 2) // TAKTİK FAUL
                {
                    aktifSavunmaci.yapilanFaul++;
                    yorum = $"🛑 {aktifSavunmaci.isim} tehlikeyi büyümeden taktik bir faulle kesti.";
                    if (rnd.Next(0, 100) > 40) { aktifSavunmaci.sariKartSayisi++; yorum += " Hakem tereddütsüz 🟨 Sarı Kartını gösterdi."; }
                    momentum = 0;
                }
            }

            SpikerMesaji(yorum);
            saldiranTakim.TakimYorul(1); savunanTakim.TakimYorul(1);
            ButonlariGuncelle();
            ArayuzGuncelle();
        }

        // =========================================================================================

        private Futbolcu GetAktifOyuncu(Takim t, bool hucumMu)
        {
            if (hucumMu)
            {
                if (sahaBolgesi == 0) return t.MevkiGetir("Defans").FirstOrDefault();
                if (sahaBolgesi == 1) return t.MevkiGetir("Ortasaha").FirstOrDefault();
                return t.MevkiGetir("Forvet").FirstOrDefault();
            }
            else
            {
                if (sahaBolgesi == 0) return t.MevkiGetir("Forvet").FirstOrDefault();
                if (sahaBolgesi == 1) return t.MevkiGetir("Ortasaha").FirstOrDefault();
                return t.MevkiGetir("Defans").FirstOrDefault();
            }
        }

        private void DevreArasiBaslat()
        {
            macMotoru.Stop();
            araDevreMode = true; ilkYariBitti = true; ekSure = 0; sahaBolgesi = 1; momentum = 0;
            SpikerMesaji("☕ İLK YARI BİTTİ. Teknik direktörler soyunma odasında taktiklerini veriyor.");
            for (int i = 0; i < 3; i++) { btnEvAksiyonlari[i].Text = btnDepAksiyonlari[i].Text = (i == 1) ? "2. YARIYI BAŞLAT" : ""; }
        }

        private void DevreArasiniBitir()
        {
            araDevreMode = false; dakika = 45; topEvSahibinde = false;
            SpikerMesaji("🏟️ İkinci yarı başladı! Her iki takıma da başarılar.");
            ButonlariGuncelle(); ArayuzGuncelle();
            macMotoru.Start();
        }

        private void OyunBitir()
        {
            macMotoru.Stop();
            macBitti = true; pnlEvKontrol.Enabled = pnlDepKontrol.Enabled = false;
            SpikerMesaji("🏁 SON DÜDÜK! Gümüşhane Arena'da nefes kesen maç sona erdi.");
            MessageBox.Show($"Maç Bitti!\nSkor: {evSkor} - {depSkor}", "Maç Sonu Raporu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SpikerMesaji(string m) { macLog.Items.Add($"{FormatTime()} | {m}"); macLog.TopIndex = macLog.Items.Count - 1; }
        private string FormatTime()
        {
            if (dakika > 45 && !ilkYariBitti) return $"45+{dakika - 45}'";
            if (dakika > 90) return $"90+{dakika - 90}'";
            return $"{dakika:00}'";
        }

        private void ArayuzGuncelle()
        {
            lblSkor.Text = $"{evSahibi.takimIsmi.ToUpper()}   {evSkor} - {depSkor}   {deplasman.takimIsmi.ToUpper()}";
            lblDakika.Text = FormatTime();
            pbSahaIlerleme.Value = sahaBolgesi;

            string bIsmi = sahaBolgesi == 0 ? "DEFANS" : (sahaBolgesi == 1 ? "ORTA SAHA" : "HÜCUM");
            lblSahaDurumu.Text = topEvSahibinde ? $"⚔️ {evSahibi.takimIsmi.ToUpper()} {bIsmi} BÖLGESİNDE" : $"🛡️ {deplasman.takimIsmi.ToUpper()} {bIsmi} BÖLGESİNDE";
            lblSahaDurumu.ForeColor = sahaBolgesi == 2 ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113);

            lblEvIstatistik.Text = $"{evSahibi.takimIsmi}\nMoral: %{evSahibi.takimMorali}";
            lblDepIstatistik.Text = $"{deplasman.takimIsmi}\nMoral: %{deplasman.takimMorali}";
            lblMomentum.Text = momentum > 0 ? $"📈 BASKI: %{momentum * 10} MOMENTUM" : "";

            // PROFESYONEL ODAKLANMA EFEKTİ
            pnlEvKontrol.BackColor = topEvSahibinde ? Color.FromArgb(220, 25, 25, 30) : Color.FromArgb(120, 15, 15, 18);
            pnlDepKontrol.BackColor = !topEvSahibinde ? Color.FromArgb(220, 25, 25, 30) : Color.FromArgb(120, 15, 15, 18);

            lblEvBaslik.BackColor = topEvSahibinde ? Color.Crimson : Color.FromArgb(100, 220, 20, 60);
            lblDepBaslik.BackColor = !topEvSahibinde ? Color.DodgerBlue : Color.FromArgb(100, 30, 144, 255);
        }
    }
}