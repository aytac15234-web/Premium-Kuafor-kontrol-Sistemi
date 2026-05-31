CREATE DATABASE IF NOT EXISTS berber_db;
USE berber_db;

CREATE TABLE IF NOT EXISTS personeller (
    id INT AUTO_INCREMENT PRIMARY KEY,
    AdSoyad VARCHAR(100) NOT NULL,
    Uzmanlik VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS randevular (
    id INT AUTO_INCREMENT PRIMARY KEY,
    PersonelId INT,
    MusteriAd VARCHAR(100) NOT NULL,
    Telefon VARCHAR(20),
    Hizmet VARCHAR(50),
    TarihSaat DATETIME NOT NULL,
    Fiyat INT NOT NULL,
    Durum VARCHAR(20) DEFAULT 'Aktif',
    FOREIGN KEY (PersonelId) REFERENCES personeller(id)
);

INSERT INTO personeller (AdSoyad, Uzmanlik)
SELECT 'Ahmet Usta', 'Saç Kesimi'
WHERE NOT EXISTS (SELECT 1 FROM personeller WHERE AdSoyad = 'Ahmet Usta');

INSERT INTO personeller (AdSoyad, Uzmanlik)
SELECT 'Mehmet Usta', 'Sakal Tıraşı'
WHERE NOT EXISTS (SELECT 1 FROM personeller WHERE AdSoyad = 'Mehmet Usta');

INSERT INTO personeller (AdSoyad, Uzmanlik)
SELECT 'Zeynep Hanım', 'Cilt Bakımı'
WHERE NOT EXISTS (SELECT 1 FROM personeller WHERE AdSoyad = 'Zeynep Hanım');
