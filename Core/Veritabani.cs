using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace berber_randavu_sitesmi
{
    public class Veritabani
    {
        private readonly string constr = "Server=localhost;Database=berber_db;Uid=root;Pwd=sifre;";

        public DataTable PersonelleriGetir()
        {
            using (MySqlConnection conn = new MySqlConnection(constr))
            {
                conn.Open();
                string sql = "SELECT id, AdSoyad FROM personeller ORDER BY AdSoyad ASC";

                using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public bool RandevuSaatiDoluMu(DateTime tarih, int personelId)
        {
            using (MySqlConnection conn = new MySqlConnection(constr))
            {
                conn.Open();

                string sql = @"SELECT COUNT(*) 
                               FROM randevular 
                               WHERE PersonelId = @personelId
                               AND Durum = 'Aktif'
                               AND ABS(TIMESTAMPDIFF(MINUTE, TarihSaat, @tarih)) < 30";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@personelId", personelId);
                    cmd.Parameters.AddWithValue("@tarih", tarih);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public void RandevuEkle(int personelId, string ad, string tel, string hizmet, DateTime tarih, int fiyat)
        {
            using (MySqlConnection conn = new MySqlConnection(constr))
            {
                conn.Open();

                string sql = @"INSERT INTO randevular 
                               (PersonelId, MusteriAd, Telefon, Hizmet, TarihSaat, Fiyat, Durum)
                               VALUES
                               (@personelId, @ad, @tel, @hizmet, @tarih, @fiyat, 'Aktif')";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@personelId", personelId);
                    cmd.Parameters.AddWithValue("@ad", ad);
                    cmd.Parameters.AddWithValue("@tel", tel);
                    cmd.Parameters.AddWithValue("@hizmet", hizmet);
                    cmd.Parameters.AddWithValue("@tarih", tarih);
                    cmd.Parameters.AddWithValue("@fiyat", fiyat);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RandevuIptalEt(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(constr))
            {
                conn.Open();

                string sql = "UPDATE randevular SET Durum = 'İptal' WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable RandevulariGetir(bool sadeceBugun)
        {
            using (MySqlConnection conn = new MySqlConnection(constr))
            {
                conn.Open();

                string sql = @"SELECT 
                                    r.id,
                                    IFNULL(p.AdSoyad, 'Personel Yok') AS 'Personel',
                                    r.MusteriAd AS 'Müşteri',
                                    r.Telefon,
                                    r.Hizmet AS 'İşlem',
                                    r.TarihSaat AS 'Tarih',
                                    r.Fiyat AS 'Ücret (₺)',
                                    r.Durum
                               FROM randevular r
                               LEFT JOIN personeller p ON r.PersonelId = p.id";

                if (sadeceBugun)
                    sql += " WHERE DATE(r.TarihSaat) = CURDATE()";

                sql += " ORDER BY r.TarihSaat ASC";

                using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public string[] GunlukIstatistikGetir(DateTime tarih, int personelId)
        {
            using (MySqlConnection conn = new MySqlConnection(constr))
            {
                conn.Open();

                string sqlWhere = @"WHERE Durum = 'Aktif'
                                    AND DATE(TarihSaat) = DATE(@tarih)
                                    AND PersonelId = @personelId";

                using (MySqlCommand cmdSayi = new MySqlCommand("SELECT COUNT(*) FROM randevular " + sqlWhere, conn))
                using (MySqlCommand cmdCiro = new MySqlCommand("SELECT IFNULL(SUM(Fiyat), 0) FROM randevular " + sqlWhere, conn))
                {
                    cmdSayi.Parameters.AddWithValue("@tarih", tarih);
                    cmdSayi.Parameters.AddWithValue("@personelId", personelId);

                    cmdCiro.Parameters.AddWithValue("@tarih", tarih);
                    cmdCiro.Parameters.AddWithValue("@personelId", personelId);

                    return new string[]
                    {
                        cmdSayi.ExecuteScalar().ToString(),
                        cmdCiro.ExecuteScalar().ToString()
                    };
                }
            }
        }

        public DataTable SeciliGunRandevulari(DateTime tarih, int personelId)
        {
            using (MySqlConnection conn = new MySqlConnection(constr))
            {
                conn.Open();

                string sql = @"SELECT 
                                    id,
                                    MusteriAd,
                                    Telefon,
                                    Hizmet,
                                    TarihSaat,
                                    Fiyat,
                                    Durum
                               FROM randevular
                               WHERE DATE(TarihSaat) = DATE(@tarih)
                               AND PersonelId = @personelId
                               AND Durum = 'Aktif'
                               ORDER BY TarihSaat ASC";

                using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@tarih", tarih);
                    da.SelectCommand.Parameters.AddWithValue("@personelId", personelId);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public DataTable YaklasanRandevulariGetir(int dakikaKala)
        {
            using (MySqlConnection conn = new MySqlConnection(constr))
            {
                conn.Open();

                string sql = @"SELECT 
                                    id,
                                    MusteriAd,
                                    Telefon,
                                    Hizmet,
                                    TarihSaat,
                                    Fiyat
                               FROM randevular
                               WHERE Durum = 'Aktif'
                               AND Telefon IS NOT NULL
                               AND Telefon <> ''
                               AND TarihSaat BETWEEN NOW() AND DATE_ADD(NOW(), INTERVAL @dakikaKala MINUTE)
                               ORDER BY TarihSaat ASC";

                using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@dakikaKala", dakikaKala);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
