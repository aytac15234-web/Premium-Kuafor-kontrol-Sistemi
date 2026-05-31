using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using berber_randavu_sitesmi.Services; 
using berber_randavu_sitesmi.UI_Components; 

namespace berber_randavu_sitesmi
{
    public partial class Form1 : Form
    {
        private Veritabani db = new Veritabani();
        

        private TextBox txtMusteriAd, txtTelefon, txtFiyat, txtAra;
        private ComboBox cmbPersonel, cmbHizmet, cmbSaat, cmbHatirlatmaDakika;
        private DateTimePicker dtpTarih, dtpGridTarih;
        private DataGridView tabloRandevular;
        
        private FlowLayoutPanel flwZamanGrid;
        private CheckBox chkSadeceBugun, chkHatirlatmaAktif;
        private Label lblGunlukRandevu, lblGunlukCiro, lblSeciliPersonel, lblSeciliGun;
        private Label lblDetayBaslik, lblDetayMusteri, lblDetayTelefon, lblDetayHizmet, lblDetayTarih, lblDetayFiyat;
        private Button btnDetayIptal, btnDetayWhatsApp;



        private System.Windows.Forms.Timer hatirlatmaTimer;
        private DataTable randevuTablosu;
        private List<TimeSpan> aktifSaatler = new List<TimeSpan>();
        private HashSet<int> uyarisiVerilenRandevular = new HashSet<int>();
        private int seciliRandevuId = 0;
        

        private readonly Color bgMain = Color.FromArgb(245, 247, 250);
        private readonly Color bgCard = Color.White;
        private readonly Color textDark = Color.FromArgb(30, 41, 59);
        private readonly Color textMuted = Color.FromArgb(100, 116, 139);
        private readonly Color primaryColor = Color.FromArgb(37, 99, 235);
        private readonly Color greenSuccess = Color.FromArgb(22, 163, 74);
        private readonly Color redDanger = Color.FromArgb(239, 68, 68);



        public Form1()
        {
            SaatListesiniVarsayilanYap();
            ArayuzuHazirla();
            VerileriGuncelle();
        }




        private void ArayuzuHazirla()
        {
            Text = "Premium Kuaför & Berber Yönetim Sistemi";
            Size = new Size(1380, 820);
            MinimumSize = new Size(1220, 740);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = bgMain;

            Font baslikFont = new Font("Segoe UI", 17, FontStyle.Bold);
            Font labelFont = new Font("Segoe UI Semibold", 9);
            Font inputFont = new Font("Segoe UI", 10);

            TableLayoutPanel ana = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = bgMain };
            ana.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 330));
            ana.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(ana);
            

            Panel solPanel = new Panel { Dock = DockStyle.Fill, BackColor = bgCard, Padding = new Padding(20) };
            ana.Controls.Add(solPanel, 0, 0);

            TableLayoutPanel sol = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 17 };
            sol.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            solPanel.Controls.Add(sol);
            

            sol.Controls.Add(new Label { Text = "YENİ RANDEVU", Font = baslikFont, ForeColor = primaryColor, Dock = DockStyle.Fill, Height = 45 });

            txtMusteriAd = TextBoxOlustur(inputFont);
            txtTelefon = TextBoxOlustur(inputFont);
            txtFiyat = TextBoxOlustur(new Font("Segoe UI", 14, FontStyle.Bold));
            txtFiyat.ForeColor = greenSuccess;


            cmbPersonel = ComboOlustur(inputFont);
            cmbHizmet = ComboOlustur(inputFont);
            cmbSaat = ComboOlustur(inputFont);

            dtpTarih = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Long, Font = inputFont };

            sol.Controls.Add(LabelOlustur("Müşteri Adı Soyadı:", labelFont));           
            sol.Controls.Add(txtMusteriAd); 
            
            sol.Controls.Add(LabelOlustur("Telefon Numarası:", labelFont));     
            sol.Controls.Add(txtTelefon);
            
            sol.Controls.Add(LabelOlustur("Personel / Kuaför:", labelFont));
            sol.Controls.Add(cmbPersonel);
            
            sol.Controls.Add(LabelOlustur("Hizmet Türü:", labelFont));
            sol.Controls.Add(cmbHizmet);
            
            sol.Controls.Add(LabelOlustur("Randevu Tarihi:", labelFont));
            sol.Controls.Add(dtpTarih);
            
            sol.Controls.Add(LabelOlustur("Randevu Saati:", labelFont));
            sol.Controls.Add(cmbSaat);
            
            sol.Controls.Add(LabelOlustur("Fiyat (TL):", labelFont));
            sol.Controls.Add(txtFiyat);


            Button btnKaydet = ButonOlustur("RANDEVUYU KAYDET", primaryColor, Color.White);
            btnKaydet.Height = 48;
            btnKaydet.Click += BtnKaydet_Click;
            sol.Controls.Add(btnKaydet);



            Button btnTemizle = ButonOlustur("FORMU TEMİZLE", Color.FromArgb(226, 232, 240), textDark);
            btnTemizle.Height = 40;
            btnTemizle.Click += (s, e) => FormuTemizle();
            sol.Controls.Add(btnTemizle);

            TableLayoutPanel sag = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), BackColor = bgMain, ColumnCount = 1, RowCount = 5 };
            
            sag.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            sag.RowStyles.Add(new RowStyle(SizeType.Absolute, 105));
            sag.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            sag.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            sag.RowStyles.Add(new RowStyle(SizeType.Absolute, 230));
            ana.Controls.Add(sag, 1, 0);

            

            sag.Controls.Add(new Label { Text = "YÖNETİM PANELİ", Font = baslikFont, ForeColor = primaryColor, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft });

            TableLayoutPanel kartlar = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3 };
            kartlar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));
            kartlar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));
            kartlar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
            sag.Controls.Add(kartlar);


            lblGunlukRandevu = KartEkle(kartlar, "Günlük Randevu", "0", primaryColor, 0);
            lblGunlukCiro = KartEkle(kartlar, "Günlük Ciro", "0 TL", greenSuccess, 1);

            Panel secimKart = KartPanel();
            secimKart.Controls.Add(new Label { Text = "Seçili Çizelge", Location = new Point(16, 12), AutoSize = true, Font = labelFont, ForeColor = textMuted });
            lblSeciliPersonel = new Label { Text = "-", Location = new Point(16, 36), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = textDark };
            lblSeciliGun = new Label { Text = "-", Location = new Point(16, 62), AutoSize = true, Font = inputFont, ForeColor = textMuted };

            
            secimKart.Controls.Add(lblSeciliPersonel);
            secimKart.Controls.Add(lblSeciliGun);
            kartlar.Controls.Add(secimKart, 2, 0);

            Panel filtre = new Panel { Dock = DockStyle.Fill, BackColor = bgMain, Padding = new Padding(0, 8, 0, 8) };
            sag.Controls.Add(filtre);


            chkSadeceBugun = new CheckBox { Text = "Sadece bugünü göster", Location = new Point(0, 21), AutoSize = true, Font = labelFont, ForeColor = textDark };
            chkSadeceBugun.CheckedChanged += (s, e) => VerileriGuncelle();
            filtre.Controls.Add(chkSadeceBugun);


            filtre.Controls.Add(new Label { Text = "Ara:", Location = new Point(180, 23), AutoSize = true, Font = labelFont, ForeColor = textDark });
            txtAra = new TextBox { Location = new Point(220, 18), Width = 240, Height = 28, Font = inputFont, BorderStyle = BorderStyle.FixedSingle };
            txtAra.TextChanged += (s, e) => TabloyuFiltrele();
            filtre.Controls.Add(txtAra);



            Button btnBugun = MiniButonAbsolute("BUGÜN", 480, 16);
            btnBugun.Click += (s, e) => TarihiAyarla(DateTime.Today);
            filtre.Controls.Add(btnBugun);


            Button btnYarin = MiniButonAbsolute("YARIN", 570, 16);
            btnYarin.Click += (s, e) => TarihiAyarla(DateTime.Today.AddDays(1));
            filtre.Controls.Add(btnYarin);

            Button btnHafta = MiniButonAbsolute("+1 HAFTA", 660, 16);
            btnHafta.Click += (s, e) => TarihiAyarla(DateTime.Today.AddDays(7));
            filtre.Controls.Add(btnHafta);

            chkHatirlatmaAktif = new CheckBox { Text = "Hatırlatma aktif", Location = new Point(840, 21), AutoSize = true, Font = labelFont, ForeColor = textDark };
            filtre.Controls.Add(chkHatirlatmaAktif);

            cmbHatirlatmaDakika = new ComboBox { Location = new Point(990, 17), Width = 70, Font = inputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbHatirlatmaDakika.Items.AddRange(new string[] { "10", "20", "30", "60" });
            cmbHatirlatmaDakika.SelectedItem = "20";
            filtre.Controls.Add(cmbHatirlatmaDakika);

            filtre.Controls.Add(new Label { Text = "dk kala", Location = new Point(1068, 22), AutoSize = true, Font = labelFont, ForeColor = textDark });

            TableLayoutPanel ortaAlan = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            ortaAlan.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            ortaAlan.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340));

            
            sag.Controls.Add(ortaAlan);

            tabloRandevular = new DataGridView
            {
                Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = bgCard, ForeColor = textDark, GridColor = Color.FromArgb(226, 232, 240),
                BorderStyle = BorderStyle.FixedSingle, RowTemplate = { Height = 34 }, Cursor = Cursors.Hand
            };
            tabloRandevular.EnableHeadersVisualStyles = false;
            tabloRandevular.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 241, 248);
            tabloRandevular.ColumnHeadersDefaultCellStyle.ForeColor = textDark;
            tabloRandevular.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            tabloRandevular.DefaultCellStyle.SelectionBackColor = primaryColor;
            tabloRandevular.DefaultCellStyle.SelectionForeColor = Color.White;

            
            tabloRandevular.CellClick += TabloRandevular_CellClick;
            tabloRandevular.CellDoubleClick += TabloRandevular_CellDoubleClick;
            ortaAlan.Controls.Add(tabloRandevular, 0, 0);

            Panel detay = KartPanel();
            
            detay.Padding = new Padding(18);
            ortaAlan.Controls.Add(detay, 1, 0);

            lblDetayBaslik = new Label { Text = "Randevu Detayı", Dock = DockStyle.Top, Height = 38, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = primaryColor };
            lblDetayMusteri = DetayLabel("Müşteri: -");
            lblDetayTelefon = DetayLabel("Telefon: -");
            lblDetayHizmet = DetayLabel("Hizmet: -");
            lblDetayTarih = DetayLabel("Tarih: -");
            lblDetayFiyat = DetayLabel("Fiyat: -");



            btnDetayIptal = new Button { Text = "SEÇİLİ RANDEVUYU İPTAL ET", Dock = DockStyle.Bottom, Height = 42, BackColor = redDanger, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Enabled = false, Cursor = Cursors.Hand };
            btnDetayIptal.FlatAppearance.BorderSize = 0;
            btnDetayIptal.Click += BtnDetayIptal_Click;

            btnDetayWhatsApp = new Button { Text = "WHATSAPP HATIRLAT", Dock = DockStyle.Bottom, Height = 42, BackColor = greenSuccess, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Enabled = false, Cursor = Cursors.Hand };
            btnDetayWhatsApp.FlatAppearance.BorderSize = 0;
            btnDetayWhatsApp.Click += BtnDetayWhatsApp_Click;

            detay.Controls.Add(btnDetayIptal);
            detay.Controls.Add(btnDetayWhatsApp);
            detay.Controls.Add(lblDetayFiyat);
            detay.Controls.Add(lblDetayTarih);
            detay.Controls.Add(lblDetayHizmet);
            detay.Controls.Add(lblDetayTelefon);
            detay.Controls.Add(lblDetayMusteri);
            
            detay.Controls.Add(lblDetayBaslik);
            

            TableLayoutPanel cizelge = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, BackColor = bgMain };
            cizelge.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            cizelge.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            sag.Controls.Add(cizelge);

            Panel cizelgeUst = new Panel { Dock = DockStyle.Fill, BackColor = bgMain };
            cizelge.Controls.Add(cizelgeUst);

            cizelgeUst.Controls.Add(new Label { Text = "GÜNLÜK ÇİZELGE", Location = new Point(0, 10), AutoSize = true, Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = primaryColor });

            dtpGridTarih = new DateTimePicker { Location = new Point(170, 7), Width = 300, Font = inputFont, Format = DateTimePickerFormat.Long };
            dtpGridTarih.ValueChanged += (s, e) => { dtpTarih.Value = dtpGridTarih.Value.Date; SeciliBilgileriYaz(); VerileriGuncelle(); };
            cizelgeUst.Controls.Add(dtpGridTarih);

            Button btnSaatEkle = MiniButonAbsolute("+30 DK", 490, 6); btnSaatEkle.Click += (s, e) => SaatEkle(); cizelgeUst.Controls.Add(btnSaatEkle);
            Button btnSaatSil = MiniButonAbsolute("-30 DK", 580, 6); btnSaatSil.Click += (s, e) => SaatSil(); cizelgeUst.Controls.Add(btnSaatSil);

            flwZamanGrid = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = true, FlowDirection = FlowDirection.LeftToRight, BackColor = bgCard, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10) };
            cizelge.Controls.Add(flwZamanGrid);

            cmbHizmet.Items.AddRange(new string[] { "Saç Kesimi", "Sakal Tıraşı", "Saç & Sakal", "Cilt Bakımı", "Damada Özel VIP Paket" });
            cmbHizmet.SelectedIndexChanged += CmbHizmet_SelectedIndexChanged;
            cmbHizmet.SelectedIndex = 0;

            SaatCombosunuDoldur();
            if (cmbSaat.Items.Count > 0) cmbSaat.SelectedIndex = 0;

            cmbPersonel.DataSource = db.PersonelleriGetir();
            cmbPersonel.DisplayMember = "AdSoyad";
            cmbPersonel.ValueMember = "id";
            cmbPersonel.SelectedIndexChanged += (s, e) => { SeciliBilgileriYaz(); VerileriGuncelle(); };

            dtpTarih.ValueChanged += (s, e) => { if (dtpGridTarih != null) dtpGridTarih.Value = dtpTarih.Value.Date; };

            WhatsAppMenusunuKur();
            HatirlatmaTimerKur();
            SeciliBilgileriYaz();
        }

        private Label LabelOlustur(string text, Font font) { return new Label { Text = text, Dock = DockStyle.Fill, Font = font, ForeColor = textDark, TextAlign = ContentAlignment.BottomLeft }; }
        private TextBox TextBoxOlustur(Font font) { return new TextBox { Dock = DockStyle.Fill, Font = font, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White }; }
        private ComboBox ComboOlustur(Font font) { return new ComboBox { Dock = DockStyle.Fill, Font = font, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.White }; }

        
        private Button ButonOlustur(string text, Color bg, Color fg) { Button btn = new Button { Text = text, Dock = DockStyle.Fill, BackColor = bg, ForeColor = fg, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand }; btn.FlatAppearance.BorderSize = 0; return btn; }
        private Panel KartPanel() { return new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 12, 0), BackColor = bgCard, BorderStyle = BorderStyle.FixedSingle }; }
        private Label KartEkle(TableLayoutPanel parent, string baslik, string deger, Color renk, int kolon) { Panel kart = KartPanel(); kart.Controls.Add(new Label { Text = baslik, Location = new Point(16, 12), AutoSize = true, Font = new Font("Segoe UI Semibold", 9), ForeColor = textMuted }); Label lbl = new Label { Text = deger, Location = new Point(16, 36), AutoSize = true, Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = renk }; kart.Controls.Add(lbl); parent.Controls.Add(kart, kolon, 0); return lbl; }
        private Label DetayLabel(string text) { return new Label { Text = text, Dock = DockStyle.Top, Height = 34, Font = new Font("Segoe UI", 10), ForeColor = textDark }; }
        private Button MiniButonAbsolute(string text, int x, int y) { Button btn = new Button { Text = text, Location = new Point(x, y), Width = 82, Height = 32, BackColor = Color.White, ForeColor = primaryColor, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand }; btn.FlatAppearance.BorderColor = primaryColor; return btn; }


        private void SaatListesiniVarsayilanYap() { aktifSaatler.Clear(); for (TimeSpan saat = new TimeSpan(8, 0, 0); saat <= new TimeSpan(19, 0, 0); saat = saat.Add(TimeSpan.FromMinutes(30))) aktifSaatler.Add(saat); }
        private void SaatCombosunuDoldur() { cmbSaat.Items.Clear(); foreach (TimeSpan saat in aktifSaatler) { if (saat >= new TimeSpan(12, 30, 0) && saat < new TimeSpan(13, 30, 0)) continue; cmbSaat.Items.Add(saat.ToString(@"hh\:mm")); } }
        private void SaatEkle() { if (aktifSaatler.Count == 0) aktifSaatler.Add(new TimeSpan(8, 0, 0)); else aktifSaatler.Add(aktifSaatler[aktifSaatler.Count - 1].Add(TimeSpan.FromMinutes(30))); SaatCombosunuDoldur(); ZamanCizelgesiniCiz(); }
        private void SaatSil() { if (aktifSaatler.Count > 1) aktifSaatler.RemoveAt(aktifSaatler.Count - 1); SaatCombosunuDoldur(); if (cmbSaat.Items.Count > 0) cmbSaat.SelectedIndex = 0; ZamanCizelgesiniCiz(); }



        private int SeciliPersonelIdAl() { if (cmbPersonel == null || cmbPersonel.SelectedValue == null) return 0; int id; return int.TryParse(cmbPersonel.SelectedValue.ToString(), out id) ? id : 0; }
        private DateTime SeciliRandevuTarihiAl() { TimeSpan saat = TimeSpan.Parse(cmbSaat.SelectedItem.ToString()); return dtpTarih.Value.Date.Add(saat); }
        private void TarihiAyarla(DateTime tarih) { dtpTarih.Value = tarih; dtpGridTarih.Value = tarih; SeciliBilgileriYaz(); VerileriGuncelle(); }
        private void SeciliBilgileriYaz() { if (lblSeciliPersonel == null || lblSeciliGun == null) return; lblSeciliPersonel.Text = cmbPersonel != null && cmbPersonel.Text != "" ? cmbPersonel.Text : "-"; lblSeciliGun.Text = dtpGridTarih != null ? dtpGridTarih.Value.ToString("dd.MM.yyyy dddd") : "-"; }
        private void CmbHizmet_SelectedIndexChanged(object sender, EventArgs e) { string secilen = cmbHizmet.SelectedItem.ToString(); if (secilen == "Saç Kesimi") txtFiyat.Text = "200"; else if (secilen == "Sakal Tıraşı") txtFiyat.Text = "100"; else if (secilen == "Saç & Sakal") txtFiyat.Text = "280"; else if (secilen == "Cilt Bakımı") txtFiyat.Text = "150"; else if (secilen == "Damada Özel VIP Paket") txtFiyat.Text = "1500"; }






        private void SeciliSatirdanWhatsAppGonder()
        {
            if (tabloRandevular.SelectedRows.Count == 0) return;
            var satir = tabloRandevular.SelectedRows[0];
            
            WhatsAppServis.WhatsAppAc(
                satir.Cells["Müşteri"].Value.ToString(),
                satir.Cells["Telefon"].Value.ToString(),
                Convert.ToDateTime(satir.Cells["Tarih"].Value),
                satir.Cells["İşlem"].Value.ToString()
            );
        }
        

        private void BtnDetayWhatsApp_Click(object sender, EventArgs e)
        {
            if (seciliRandevuId <= 0) { MessageBox.Show("WhatsApp için önce bir randevu seçiniz."); return; }
            
            WhatsAppServis.WhatsAppAc(
                lblDetayMusteri.Text.Replace("Müsteri: ", ""),
                lblDetayTelefon.Text.Replace("Telefon: ", ""),
                DateTime.Parse(lblDetayTarih.Text.Replace("Tarih: ", "")),
                lblDetayHizmet.Text.Replace("Hizmet: ", "")
            );
        }


        private void ZamanCizelgesiniCiz()
        {
            if (flwZamanGrid == null || dtpGridTarih == null) return;
            int personelId = SeciliPersonelIdAl();
            if (personelId == 0) return;

            DateTime gun = dtpGridTarih.Value.Date;
            DataTable randevular = db.SeciliGunRandevulari(gun, personelId);



            ZamanCizelgesiMotoru.Ciz(flwZamanGrid, gun, aktifSaatler, randevular, BosSlot_Click, DoluSlot_Click);
        }

        
        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            int personelId = SeciliPersonelIdAl();
            if (personelId == 0) { MessageBox.Show("Lütfen personel seciniz."); return; }
            if (string.IsNullOrWhiteSpace(txtMusteriAd.Text)) { MessageBox.Show("Müsteri adı zorunludur."); return; }
            if (cmbSaat.SelectedItem == null) { MessageBox.Show("Lütfen saat seciniz."); return; }

            int fiyat;
            if (!int.TryParse(txtFiyat.Text, out fiyat)) { MessageBox.Show("Fiyat sadece sayı olmalıdır."); return; }

            DateTime tarih = SeciliRandevuTarihiAl();
            if (db.RandevuSaatiDoluMu(tarih, personelId)) { MessageBox.Show("Secilen personelin bu saate yakın başka bir randevusu var."); return; }

            try
            {
                string tel = WhatsAppServis.TelefonuWhatsAppFormatinaCevir(txtTelefon.Text);
                if (string.IsNullOrWhiteSpace(tel)) { MessageBox.Show("Telefon numarası geçerli değil. Örnek: 0500 111 22 33"); return; }

                db.RandevuEkle(personelId, txtMusteriAd.Text.Trim(), tel, cmbHizmet.SelectedItem.ToString(), tarih, fiyat);
                MessageBox.Show("Randevu basarıyla kaydedildi.");
                FormuTemizle();
                VerileriGuncelle();
            }
            catch (Exception ex) { MessageBox.Show("Kayıt hatası: " + ex.Message); }
        }

        private void BtnDetayIptal_Click(object sender, EventArgs e)
        {
            if (seciliRandevuId <= 0) { MessageBox.Show("İptal etmek için önce bir randevu seçiniz."); return; }
            if (MessageBox.Show("Secili randevuyu iptal etmek istiyor musunuz?", "randevu İptali", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                db.RandevuIptalEt(seciliRandevuId);
                MessageBox.Show("randevu iptal edildi.");
                DetaylariTemizle();
                VerileriGuncelle();
            }
        }

        private void FormuTemizle()
        {
            txtMusteriAd.Clear(); txtTelefon.Clear();
            if (cmbHizmet.Items.Count > 0) cmbHizmet.SelectedIndex = 0;
            if (cmbSaat.Items.Count > 0) cmbSaat.SelectedIndex = 0;
            DetaylariTemizle(); txtMusteriAd.Focus();
        }

        private void VerileriGuncelle()
        {
            try
            {
                if (tabloRandevular == null) return;
                int personelId = SeciliPersonelIdAl();
                
                randevuTablosu = db.RandevulariGetir(chkSadeceBugun != null && chkSadeceBugun.Checked);
                tabloRandevular.DataSource = randevuTablosu;
                if (tabloRandevular.Columns.Contains("id")) tabloRandevular.Columns["id"].Visible = false;

                if (personelId > 0 && dtpGridTarih != null)
                {
                    string[] gunluk = db.GunlukIstatistikGetir(dtpGridTarih.Value.Date, personelId);
                    lblGunlukRandevu.Text = gunluk[0];
                    lblGunlukCiro.Text = gunluk[1] + " TL";
                }

                TabloyuFiltrele();
                SeciliBilgileriYaz();
                ZamanCizelgesiniCiz();
            }
            catch (Exception ex) { MessageBox.Show("veri yenileme hatası: " + ex.Message); }
        }

        private void TabloyuFiltrele()
        {
            if (randevuTablosu == null || txtAra == null) return;
            string arama = txtAra.Text.Trim().Replace("'", "''");
            DataView dv = randevuTablosu.DefaultView;
            dv.RowFilter = arama == "" ? "" : $"[Müsteri] LIKE '%{arama}%' OR Telefon LIKE '%{arama}%' OR Personel LIKE '%{arama}%' OR [İşlem] LIKE '%{arama}%'";
            tabloRandevular.DataSource = dv;
        }

        private void BosSlot_Click(object sender, EventArgs e)
        {
            DateTime secilen = (DateTime)((Button)sender).Tag;
            dtpTarih.Value = secilen.Date;
            cmbSaat.SelectedItem = secilen.ToString("HH:mm");
            DetaylariTemizle();
            txtMusteriAd.Focus();

        }

        private void DoluSlot_Click(object sender, EventArgs e)
        {
            DetayGoster((DataRow)((Button)sender).Tag);
        }

        private void TabloRandevular_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = tabloRandevular.Rows[e.RowIndex];
            seciliRandevuId = Convert.ToInt32(row.Cells["id"].Value);
            lblDetayBaslik.Text = "Randevu Detayı";
            lblDetayMusteri.Text = "Müşteri: " + row.Cells["Müşteri"].Value.ToString();
            lblDetayTelefon.Text = "Telefon: " + row.Cells["Telefon"].Value.ToString();
            lblDetayHizmet.Text = "Hizmet: " + row.Cells["İşlem"].Value.ToString();
            lblDetayTarih.Text = "Tarih: " + Convert.ToDateTime(row.Cells["Tarih"].Value).ToString("dd.MM.yyyy HH:mm");
            lblDetayFiyat.Text = "Fiyat: " + row.Cells["Ücret (₺)"].Value.ToString() + " TL";
            btnDetayIptal.Enabled = true; btnDetayWhatsApp.Enabled = true;
        }



        private void DetayGoster(DataRow row)
        {
            seciliRandevuId = Convert.ToInt32(row["id"]);
            lblDetayBaslik.Text = "Dolu Saat Bilgisi";
            lblDetayMusteri.Text = "Müşteri: " + row["MusteriAd"].ToString();
            lblDetayTelefon.Text = "Telefon: " + row["Telefon"].ToString();
            lblDetayHizmet.Text = "Hizmet: " + row["Hizmet"].ToString();
            lblDetayTarih.Text = "Tarih: " + Convert.ToDateTime(row["TarihSaat"]).ToString("dd.MM.yyyy HH:mm");
            lblDetayFiyat.Text = "Fiyat: " + row["Fiyat"].ToString() + " TL";
            btnDetayIptal.Enabled = true; btnDetayWhatsApp.Enabled = true;
        }

        private void DetaylariTemizle()
        {
            seciliRandevuId = 0; lblDetayBaslik.Text = "Randevu Detayı"; lblDetayMusteri.Text = "Müşteri: -";
            
            lblDetayTelefon.Text = "Telefon: -"; lblDetayHizmet.Text = "Hizmet: -"; lblDetayTarih.Text = "Tarih: -"; lblDetayFiyat.Text = "Fiyat: -";
            if (btnDetayIptal != null) btnDetayIptal.Enabled = false;
            if (btnDetayWhatsApp != null) btnDetayWhatsApp.Enabled = false;
        }

        private void TabloRandevular_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            TabloRandevular_CellClick(sender, e);
            BtnDetayIptal_Click(sender, e);
        }

        private void WhatsAppMenusunuKur()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem itemWhatsApp = new ToolStripMenuItem("WhatsApp Hatırlat");
            itemWhatsApp.Click += (s, e) => SeciliSatirdanWhatsAppGonder();
            ToolStripMenuItem itemIptal = new ToolStripMenuItem("Randevu İptal Et");
            itemIptal.Click += (s, e) => BtnDetayIptal_Click(s, e);
            
            menu.Items.Add(itemWhatsApp); menu.Items.Add(itemIptal);
            tabloRandevular.ContextMenuStrip = menu;

            tabloRandevular.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    var hit = tabloRandevular.HitTest(e.X, e.Y);
                    if (hit.RowIndex >= 0)
                    {
                        tabloRandevular.ClearSelection();
                        tabloRandevular.Rows[hit.RowIndex].Selected = true;
                        TabloRandevular_CellClick(tabloRandevular, new DataGridViewCellEventArgs(hit.ColumnIndex, hit.RowIndex));
                    }
                }
            };
        }

        private void HatirlatmaTimerKur() { hatirlatmaTimer = new System.Windows.Forms.Timer(); hatirlatmaTimer.Interval = 60000; hatirlatmaTimer.Tick += HatirlatmaTimer_Tick; hatirlatmaTimer.Start(); }

        private void HatirlatmaTimer_Tick(object sender, EventArgs e)
        {
            if (chkHatirlatmaAktif == null || !chkHatirlatmaAktif.Checked) return;
            int dakikaKala = 20;
            if (cmbHatirlatmaDakika.SelectedItem != null) int.TryParse(cmbHatirlatmaDakika.SelectedItem.ToString(), out dakikaKala);

            try
            {
                DataTable dt = db.YaklasanRandevulariGetir(dakikaKala);
                foreach (DataRow row in dt.Rows)
                {
                    int id = Convert.ToInt32(row["id"]);
                    if (uyarisiVerilenRandevular.Contains(id)) continue;

                    string musteri = row["MusteriAd"].ToString();
                    string telefon = row["Telefon"].ToString();
                    string hizmet = row["Hizmet"].ToString();
                    DateTime tarih = Convert.ToDateTime(row["TarihSaat"]);
                    uyarisiVerilenRandevular.Add(id);

                    if (MessageBox.Show(musteri + " adlı müşterinin " + tarih.ToString("HH:mm") + " randevusuna " + dakikaKala + " dk kaldı.\nWhatsApp açılsın mı?", "Hatırlatma", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        WhatsAppServis.WhatsAppAc(musteri, telefon, tarih, hizmet); // Servise paslandı
                    }
                    break;
                }
            }
            catch (Exception ex) { MessageBox.Show("Hatırlatma hatası: " + ex.Message); }
        }
    }
}
