using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace berber_randavu_sitesmi.Services
{
    public static class WhatsAppServis
    {
        public static string TelefonuWhatsAppFormatinaCevir(string telefon)
        {
            if (string.IsNullOrWhiteSpace(telefon))
                return "";

            string tel = "";

            foreach (char c in telefon)
            {
                if (char.IsDigit(c))
                    tel += c;
            }

            if (tel.StartsWith("0"))
                tel = "9" + tel;

            if (tel.Length == 10 && tel.StartsWith("5"))
                tel = "90" + tel;

            if (!tel.StartsWith("90"))
                return "";

            if (tel.Length != 12)
                return "";

            return tel;
        }

        public static void WhatsAppAc(string musteri, string telefon, DateTime tarih, string hizmet)
        {
            string tel = TelefonuWhatsAppFormatinaCevir(telefon);

            if (string.IsNullOrWhiteSpace(tel))
            {
                MessageBox.Show("Telefon numarası geçerli değil. Örnek: 0500 111 22 33");
                return;
            }

            string mesaj = "Merhaba " + musteri + ", " + tarih.ToString("dd.MM.yyyy HH:mm") + " tarihli " + hizmet + " randevunuzu hatırlatmak isteriz. Görüşmek üzere.";

            string url = "https://web.whatsapp.com/send?phone=" + tel + "&text=" + Uri.EscapeDataString(mesaj);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("WhatsApp açılamadı. Link: " + url);
            }
        }
    }
}
