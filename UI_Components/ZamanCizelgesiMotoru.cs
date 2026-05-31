using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace berber_randavu_sitesmi.UI_Components
{
    public static class ZamanCizelgesiMotoru
    {
        public static void Ciz(FlowLayoutPanel flwZamanGrid, DateTime gun, List<TimeSpan> aktifSaatler, DataTable randevular, EventHandler bosTiklandi, EventHandler doluTiklandi)
        {
            flwZamanGrid.Controls.Clear();
            Color textDark = Color.FromArgb(30, 41, 59);
            Color greenSuccess = Color.FromArgb(22, 163, 74);
            Color redDanger = Color.FromArgb(239, 68, 68);

            foreach (TimeSpan saat in aktifSaatler)
            {
                Button btn = new Button
                {
                    Width = 132,
                    Height = 46,
                    Margin = new Padding(6),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                DateTime tamTarih = gun.Add(saat);

                if (saat >= new TimeSpan(12, 30, 0) && saat < new TimeSpan(13, 30, 0))
                {
                    btn.Text = saat.ToString(@"hh\:mm") + " - MOLA";
                    btn.BackColor = Color.FromArgb(226, 232, 240);
                    btn.ForeColor = textDark;
                    btn.Enabled = false;
                }
                else
                {
                    DataRow dolu = null;

                    foreach (DataRow row in randevular.Rows)
                    {
                        if (Convert.ToDateTime(row["TarihSaat"]) == tamTarih)
                        {
                            dolu = row;
                            break;
                        }
                    }

                    if (dolu != null)
                    {
                        string musteri = dolu["MusteriAd"].ToString();
                        if (musteri.Contains(" ")) musteri = musteri.Split(' ')[0];

                        btn.Text = saat.ToString(@"hh\:mm") + "\n" + musteri;
                        btn.BackColor = redDanger;
                        btn.ForeColor = Color.White;
                        btn.Cursor = Cursors.Hand;
                        btn.Tag = dolu;
                        btn.Click += doluTiklandi;
                    }
                    else
                    {
                        btn.Text = saat.ToString(@"hh\:mm") + " - BOŞ";
                        btn.BackColor = Color.White;
                        btn.ForeColor = greenSuccess;
                        btn.FlatAppearance.BorderColor = greenSuccess;
                        btn.Cursor = Cursors.Hand;
                        btn.Tag = tamTarih;
                        btn.Click += bosTiklandi;
                    }
                }

                flwZamanGrid.Controls.Add(btn);
            }
        }
    }
}
