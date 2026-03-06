using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Futbol
{
    public partial class Form2 : Form
    {
        // --- UI BİLEŞENLERİ ---
        ComboBox cbEvSahibi, cbDeplasman;
        Button btnMacaBasla;
        List<Takim> takimlar = new List<Takim>();

        Label lblEvStats, lblDepStats, lblVs, lblUyari;
        Panel pnlEvKart, pnlDepKart;
        Timer girisAnimasyonu;

        public Form2()
        {
            InitializeComponent();

            // FORM AYARLARI (SEÇKİN KOYU ARAYÜZ)
            this.Size = new Size(950, 700);
            this.Text = "SEÇKİN MAÇ MOTORU PRO 2026 - Lobi";
            this.BackColor = Color.FromArgb(15, 15, 20);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Yumuşak Giriş Animasyonu
            this.Opacity = 0;
            girisAnimasyonu = new Timer { Interval = 10 };
            girisAnimasyonu.Tick += (s, e) => { if (this.Opacity < 1) this.Opacity += 0.1; else girisAnimasyonu.Stop(); };
            girisAnimasyonu.Start();

            TakimlariHazirla();
            ArayuzuInsaEt();
        }

        private void TakimlariHazirla()
        {
            takimlar.Add(TakimUret("Galatasaray", "Ofansif"));
            takimlar.Add(TakimUret("Fenerbahçe", "Dengeli"));
            takimlar.Add(TakimUret("Beşiktaş", "Hücum"));
            takimlar.Add(TakimUret("Trabzonspor", "Defansif"));
            takimlar.Add(TakimUret("Samsunspor", "Dengeli"));
            takimlar.Add(TakimUret("Eyüpspor", "Kontra"));
        }

        // --- GÜNCEL KADRO MOTORU (Hatasız Parametreler & Türkçe) ---
        private Takim TakimUret(string isim, string taktik)
        {
            Takim t = new Takim(isim, taktik);

            if (isim == "Galatasaray")
            {
                // Kaleci: (isim, boy, guc, hiz, teknik, atak, defans, kalecilik, refleks, pozisyonAlma, iletisim)
                t.OyuncuEkle(new Kaleci("Fernando Muslera", 1.90, 82, 68, 75, 10, 45, 90, 92, 90, 96));
                t.OyuncuEkle(new Defans("Davinson Sanchez", 1.87, 90, 85, 72, 40, 88, 15));
                t.OyuncuEkle(new Defans("Abdülkerim Bardakcı", 1.85, 92, 75, 78, 55, 87, 10));
                t.OyuncuEkle(new Ortasaha("Lucas Torreira", 1.66, 88, 84, 82, 60, 90, 10));
                t.OyuncuEkle(new Ortasaha("Gabriel Sara", 1.81, 82, 78, 89, 84, 75, 10));
                // Forvet: (..., bitiricilik, sogukkanlilik, kafaVurusu, sezgi, ceviklik)
                t.OyuncuEkle(new Forvet("Mauro Icardi", 1.81, 82, 76, 92, 95, 30, 10, 96, 98, 92, 98, 85));
                t.OyuncuEkle(new Forvet("Victor Osimhen", 1.85, 94, 92, 85, 94, 35, 10, 95, 92, 96, 94, 92));
            }
            else if (isim == "Fenerbahçe")
            {
                t.OyuncuEkle(new Kaleci("Dominik Livakovic", 1.88, 78, 72, 65, 10, 40, 86, 92, 88, 85));
                t.OyuncuEkle(new Defans("Alexander Djiku", 1.82, 88, 82, 74, 45, 88, 10));
                t.OyuncuEkle(new Defans("Çağlar Söyüncü", 1.85, 90, 78, 70, 40, 86, 10));
                t.OyuncuEkle(new Ortasaha("Fred", 1.69, 82, 86, 88, 80, 82, 10));
                t.OyuncuEkle(new Ortasaha("Sofyan Amrabat", 1.85, 92, 75, 80, 65, 88, 10));
                t.OyuncuEkle(new Forvet("Edin Dzeko", 1.93, 85, 70, 90, 92, 35, 10, 94, 95, 98, 96, 75));
                t.OyuncuEkle(new Forvet("Youssef En-Nesyri", 1.88, 88, 86, 78, 90, 40, 10, 89, 85, 99, 90, 80));
            }
            else if (isim == "Beşiktaş")
            {
                t.OyuncuEkle(new Kaleci("Mert Günok", 1.96, 85, 65, 72, 10, 40, 88, 86, 90, 92));
                t.OyuncuEkle(new Defans("Gabriel Paulista", 1.87, 89, 78, 70, 40, 88, 10));
                t.OyuncuEkle(new Ortasaha("Gedson Fernandes", 1.84, 90, 92, 84, 75, 82, 10));
                t.OyuncuEkle(new Ortasaha("Rafa Silva", 1.72, 75, 90, 94, 88, 45, 10));
                t.OyuncuEkle(new Forvet("Ciro Immobile", 1.85, 80, 84, 88, 94, 25, 10, 97, 96, 88, 98, 88));
            }
            else if (isim == "Trabzonspor")
            {
                t.OyuncuEkle(new Kaleci("Uğurcan Çakır", 1.91, 84, 68, 70, 10, 40, 89, 94, 88, 90));
                t.OyuncuEkle(new Defans("Stefan Savic", 1.88, 88, 72, 68, 35, 90, 10));
                t.OyuncuEkle(new Ortasaha("Batista Mendy", 1.91, 92, 82, 80, 65, 86, 10));
                t.OyuncuEkle(new Forvet("Simon Banza", 1.89, 88, 82, 82, 88, 30, 10, 90, 88, 94, 90, 82));
            }
            else if (isim == "Samsunspor")
            {
                t.OyuncuEkle(new Kaleci("Okan Kocuk", 1.91, 80, 65, 60, 10, 40, 85, 88, 82, 80));
                t.OyuncuEkle(new Defans("Rick Van Drongelen", 1.88, 88, 72, 65, 40, 86, 10));
                t.OyuncuEkle(new Ortasaha("Olivier Ntcham", 1.80, 85, 75, 88, 82, 65, 10));
                t.OyuncuEkle(new Forvet("Marius Mouandilmadji", 1.90, 89, 80, 78, 85, 30, 10, 85, 80, 92, 85, 75));
            }
            else // Diğer Takımlar (Eyüpspor vb.)
            {
                t.OyuncuEkle(new Kaleci(isim + " GK", 1.90, 80, 60, 60, 10, 40, 82, 80, 80, 80));
                t.OyuncuEkle(new Defans(isim + " DEF", 1.85, 80, 75, 65, 40, 80, 10));
                t.OyuncuEkle(new Ortasaha(isim + " MID", 1.78, 75, 80, 82, 70, 65, 10));
                t.OyuncuEkle(new Forvet(isim + " FWD", 1.82, 82, 85, 80, 85, 30, 10, 80, 80, 80, 80, 80));
            }
            return t;
        }

        private void ArayuzuInsaEt()
        {
            // BAŞLIK - LOGO
            Label lblLogo = new Label { Text = "SEÇKİN MAÇ MOTORU", ForeColor = Color.White, Font = new Font("Segoe UI Black", 28), Location = new Point(0, 30), Size = new Size(950, 55), TextAlign = ContentAlignment.MiddleCenter };
            Label lblSub = new Label { Text = "SÜPER LİG 2026 RESMİ SİMÜLATÖRÜ", ForeColor = Color.FromArgb(200, 170, 50), Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(0, 85), Size = new Size(950, 20), TextAlign = ContentAlignment.MiddleCenter };
            this.Controls.Add(lblLogo); this.Controls.Add(lblSub);

            // KARTLAR
            pnlEvKart = KartInsa(70, 150, Color.FromArgb(200, 30, 45), "EV SAHİBİ KADROSU");
            cbEvSahibi = ComboInsa();
            lblEvStats = StatEtiketiInsa();
            pnlEvKart.Controls.Add(cbEvSahibi); pnlEvKart.Controls.Add(lblEvStats);

            pnlDepKart = KartInsa(620, 150, Color.FromArgb(30, 120, 240), "DEPLASMAN KADROSU");
            cbDeplasman = ComboInsa();
            lblDepStats = StatEtiketiInsa();
            pnlDepKart.Controls.Add(cbDeplasman); pnlDepKart.Controls.Add(lblDepStats);

            // VS İKONU
            lblVs = new Label { Text = "VS", ForeColor = Color.FromArgb(40, 40, 45), Font = new Font("Segoe UI Black", 54, FontStyle.Italic), Location = new Point(375, 240), Size = new Size(200, 100), TextAlign = ContentAlignment.MiddleCenter };

            // UYARI ETİKETİ
            lblUyari = new Label { Text = "ÇAKIŞMA TESPİT EDİLDİ: Lütfen iki farklı kadro seçin.", ForeColor = Color.Tomato, Font = new Font("Segoe UI Semibold", 9), Location = new Point(0, 470), Size = new Size(950, 25), TextAlign = ContentAlignment.MiddleCenter, Visible = false };

            // MAÇI BAŞLAT BUTONU
            btnMacaBasla = new Button { Text = "MAÇI BAŞLAT", Location = new Point(350, 520), Size = new Size(250, 75), BackColor = Color.FromArgb(0, 160, 255), ForeColor = Color.White, Font = new Font("Segoe UI Black", 14), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnMacaBasla.FlatAppearance.BorderSize = 0;
            btnMacaBasla.MouseEnter += (s, e) => { if (btnMacaBasla.Enabled) btnMacaBasla.BackColor = Color.FromArgb(0, 190, 255); };
            btnMacaBasla.MouseLeave += (s, e) => { if (btnMacaBasla.Enabled) btnMacaBasla.BackColor = Color.FromArgb(0, 160, 255); };
            btnMacaBasla.Click += (s, e) => {
                Form1 arena = new Form1(takimlar[cbEvSahibi.SelectedIndex], takimlar[cbDeplasman.SelectedIndex]);
                arena.Show(); this.Hide();
            };

            this.Controls.Add(pnlEvKart); this.Controls.Add(pnlDepKart);
            this.Controls.Add(lblVs); this.Controls.Add(lblUyari); this.Controls.Add(btnMacaBasla);

            foreach (var t in takimlar) { cbEvSahibi.Items.Add(t.takimIsmi); cbDeplasman.Items.Add(t.takimIsmi); }
            cbEvSahibi.SelectedIndex = 0; cbDeplasman.SelectedIndex = 1;

            cbEvSahibi.SelectedIndexChanged += (s, e) => VeriGuncelle();
            cbDeplasman.SelectedIndexChanged += (s, e) => VeriGuncelle();
            VeriGuncelle();
        }

        private Panel KartInsa(int x, int y, Color c, string baslik)
        {
            Panel p = new Panel { Location = new Point(x, y), Size = new Size(260, 300), BackColor = Color.FromArgb(25, 25, 30) };
            Label l = new Label { Text = baslik, Dock = DockStyle.Top, Height = 40, BackColor = c, ForeColor = Color.White, Font = new Font("Segoe UI Black", 10), TextAlign = ContentAlignment.MiddleCenter };
            p.Controls.Add(l);
            return p;
        }

        private ComboBox ComboInsa() => new ComboBox { Location = new Point(20, 60), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(40, 40, 45), ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 13), FlatStyle = FlatStyle.Flat };

        private Label StatEtiketiInsa() => new Label { Location = new Point(20, 115), Size = new Size(220, 170), ForeColor = Color.Silver, Font = new Font("Consolas", 10) };

        private void VeriGuncelle()
        {
            if (cbEvSahibi.SelectedIndex == cbDeplasman.SelectedIndex)
            {
                btnMacaBasla.Enabled = false; btnMacaBasla.BackColor = Color.FromArgb(45, 45, 50);
                lblUyari.Visible = true; lblVs.ForeColor = Color.FromArgb(30, 30, 35);
            }
            else
            {
                btnMacaBasla.Enabled = true; btnMacaBasla.BackColor = Color.FromArgb(0, 160, 255);
                lblUyari.Visible = false; lblVs.ForeColor = Color.White;
            }

            DisplayStats(cbEvSahibi, lblEvStats);
            DisplayStats(cbDeplasman, lblDepStats);
        }

        private void DisplayStats(ComboBox cb, Label lbl)
        {
            var t = takimlar[cb.SelectedIndex];
            int aP = Math.Min(100, t.GuncelHucumGucu() / 8);
            int dP = Math.Min(100, t.GuncelSavunmaGucu() / 8);

            lbl.Text = $"HÜCUM GÜCÜ  : {aP}%\n{GucCubugu(aP)}\n\nSAVUNMA GÜCÜ: {dP}%\n{GucCubugu(dP)}\n\nSTRATEJİ    : {t.taktik.ToUpper()}";
        }

        private string GucCubugu(int val)
        {
            int fill = val / 10;
            return new string('■', fill) + new string('□', 10 - fill);
        }
    }
}