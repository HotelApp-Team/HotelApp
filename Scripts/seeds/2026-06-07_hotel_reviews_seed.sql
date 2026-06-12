USE HotelAppDB;
GO

SET NOCOUNT ON;

DELETE rv
FROM Reviews rv
JOIN Users u ON u.Id = rv.UserId
WHERE u.Email LIKE N'hotel.review.%@hotelapp.local';

INSERT INTO Users
(FirstName, LastName, Email, PasswordHash, Phone, IsActive, IsEmailVerified, CreatedAt, FailedLoginAttempts)
SELECT DISTINCT
    N'Misafir',
    v.LastName,
    v.UserEmail,
    N'hotel-seed-password-hash',
    N'5550000000',
    1,
    1,
    GETDATE(),
    0
FROM (VALUES
(N'hotel.review.001@hotelapp.local', N'Yorum 001'), (N'hotel.review.002@hotelapp.local', N'Yorum 002'), (N'hotel.review.003@hotelapp.local', N'Yorum 003'),
(N'hotel.review.004@hotelapp.local', N'Yorum 004'), (N'hotel.review.005@hotelapp.local', N'Yorum 005'), (N'hotel.review.006@hotelapp.local', N'Yorum 006'),
(N'hotel.review.007@hotelapp.local', N'Yorum 007'), (N'hotel.review.008@hotelapp.local', N'Yorum 008'), (N'hotel.review.009@hotelapp.local', N'Yorum 009'),
(N'hotel.review.010@hotelapp.local', N'Yorum 010'), (N'hotel.review.011@hotelapp.local', N'Yorum 011'), (N'hotel.review.012@hotelapp.local', N'Yorum 012'),
(N'hotel.review.013@hotelapp.local', N'Yorum 013'), (N'hotel.review.014@hotelapp.local', N'Yorum 014'), (N'hotel.review.015@hotelapp.local', N'Yorum 015'),
(N'hotel.review.016@hotelapp.local', N'Yorum 016'), (N'hotel.review.017@hotelapp.local', N'Yorum 017'), (N'hotel.review.018@hotelapp.local', N'Yorum 018'),
(N'hotel.review.019@hotelapp.local', N'Yorum 019'), (N'hotel.review.020@hotelapp.local', N'Yorum 020'), (N'hotel.review.021@hotelapp.local', N'Yorum 021'),
(N'hotel.review.022@hotelapp.local', N'Yorum 022'), (N'hotel.review.023@hotelapp.local', N'Yorum 023'), (N'hotel.review.024@hotelapp.local', N'Yorum 024'),
(N'hotel.review.025@hotelapp.local', N'Yorum 025'), (N'hotel.review.026@hotelapp.local', N'Yorum 026'), (N'hotel.review.027@hotelapp.local', N'Yorum 027'),
(N'hotel.review.028@hotelapp.local', N'Yorum 028'), (N'hotel.review.029@hotelapp.local', N'Yorum 029'), (N'hotel.review.030@hotelapp.local', N'Yorum 030'),
(N'hotel.review.031@hotelapp.local', N'Yorum 031'), (N'hotel.review.032@hotelapp.local', N'Yorum 032'), (N'hotel.review.033@hotelapp.local', N'Yorum 033'),
(N'hotel.review.034@hotelapp.local', N'Yorum 034'), (N'hotel.review.035@hotelapp.local', N'Yorum 035'), (N'hotel.review.036@hotelapp.local', N'Yorum 036'),
(N'hotel.review.037@hotelapp.local', N'Yorum 037'), (N'hotel.review.038@hotelapp.local', N'Yorum 038'), (N'hotel.review.039@hotelapp.local', N'Yorum 039'),
(N'hotel.review.040@hotelapp.local', N'Yorum 040'), (N'hotel.review.041@hotelapp.local', N'Yorum 041'), (N'hotel.review.042@hotelapp.local', N'Yorum 042'),
(N'hotel.review.043@hotelapp.local', N'Yorum 043'), (N'hotel.review.044@hotelapp.local', N'Yorum 044'), (N'hotel.review.045@hotelapp.local', N'Yorum 045'),
(N'hotel.review.046@hotelapp.local', N'Yorum 046'), (N'hotel.review.047@hotelapp.local', N'Yorum 047'), (N'hotel.review.048@hotelapp.local', N'Yorum 048'),
(N'hotel.review.049@hotelapp.local', N'Yorum 049'), (N'hotel.review.050@hotelapp.local', N'Yorum 050'), (N'hotel.review.051@hotelapp.local', N'Yorum 051'),
(N'hotel.review.052@hotelapp.local', N'Yorum 052'), (N'hotel.review.053@hotelapp.local', N'Yorum 053'), (N'hotel.review.054@hotelapp.local', N'Yorum 054'),
(N'hotel.review.055@hotelapp.local', N'Yorum 055'), (N'hotel.review.056@hotelapp.local', N'Yorum 056'), (N'hotel.review.057@hotelapp.local', N'Yorum 057'),
(N'hotel.review.058@hotelapp.local', N'Yorum 058'), (N'hotel.review.059@hotelapp.local', N'Yorum 059'), (N'hotel.review.060@hotelapp.local', N'Yorum 060'),
(N'hotel.review.061@hotelapp.local', N'Yorum 061'), (N'hotel.review.062@hotelapp.local', N'Yorum 062'), (N'hotel.review.063@hotelapp.local', N'Yorum 063'),
(N'hotel.review.064@hotelapp.local', N'Yorum 064'), (N'hotel.review.065@hotelapp.local', N'Yorum 065'), (N'hotel.review.066@hotelapp.local', N'Yorum 066'),
(N'hotel.review.067@hotelapp.local', N'Yorum 067'), (N'hotel.review.068@hotelapp.local', N'Yorum 068'), (N'hotel.review.069@hotelapp.local', N'Yorum 069'),
(N'hotel.review.070@hotelapp.local', N'Yorum 070'), (N'hotel.review.071@hotelapp.local', N'Yorum 071'), (N'hotel.review.072@hotelapp.local', N'Yorum 072'),
(N'hotel.review.073@hotelapp.local', N'Yorum 073'), (N'hotel.review.074@hotelapp.local', N'Yorum 074'), (N'hotel.review.075@hotelapp.local', N'Yorum 075')
) v(UserEmail, LastName)
WHERE NOT EXISTS (
    SELECT 1 FROM Users u WHERE u.Email = v.UserEmail
);

INSERT INTO Reviews (HotelId, UserId, Rating, Comment, CreatedAt)
SELECT h.Id, u.Id, v.Rating, v.Comment, DATEADD(DAY, -v.SortNo, GETDATE())
FROM (VALUES
(1, N'Adam & Eve - Adult Only', N'hotel.review.001@hotelapp.local', 4, N'Eðlence, barlar ve açýk büfe güçlü bulunmuþ; otelin büyük yapýsý nedeniyle ilk gün alýþma süreci yaþanmýþ.'),
(2, N'Adam & Eve - Adult Only', N'hotel.review.002@hotelapp.local', 4, N'Oda, temizlik, yemek ve havuz imkanlarý beðenilmiþ; yoðun parti ortamý bazý misafirler için yorucu olmuþ.'),
(3, N'Adam & Eve - Adult Only', N'hotel.review.003@hotelapp.local', 5, N'Oda erken teslim edilmiþ, eðlence programlarý ve dans gösterileri çok baþarýlý bulunmuþ.'),
(4, N'Belcekiz Beach Club', N'hotel.review.004@hotelapp.local', 5, N'Giriþten itibaren ilgi, oda temizliði, servis ve yiyecek içecek kalitesi beklentinin üzerinde bulunmuþ.'),
(5, N'Belcekiz Beach Club', N'hotel.review.005@hotelapp.local', 5, N'Personel profesyonelliði, denize yakýnlýk, canlý müzik ve fiyat performans çok beðenilmiþ.'),
(6, N'Belcekiz Beach Club', N'hotel.review.006@hotelapp.local', 5, N'Temizlik, rahatlýk, yemekler ve lokasyon baþarýlý bulunmuþ; tekrar tercih edilebilecek bir tesis olarak deðerlendirilmiþ.'),
(7, N'Dedeman Konya', N'hotel.review.007@hotelapp.local', 5, N'Kahvaltý, spor alanlarý, temizlik ve personel ilgisi sürekli ayný kalitede bulunmuþ.'),
(8, N'Dedeman Konya', N'hotel.review.008@hotelapp.local', 5, N'Karþýlama, uðurlama, oda hijyeni, yemekler, havuz ve merkezi konum çok olumlu deðerlendirilmiþ.'),
(9, N'Dedeman Konya', N'hotel.review.009@hotelapp.local', 4, N'Otel temizliði ve çalýþan ilgisi beðenilmiþ; kahvaltý çeþitliliði yeterli ama çok geniþ bulunmamýþ.'),
(10, N'Divan Ankara', N'hotel.review.010@hotelapp.local', 5, N'Oda temizliði, kalite, hizmet ve personel karþýlamasý nedeniyle otel tavsiye edilmiþ.'),
(11, N'Divan Ankara', N'hotel.review.011@hotelapp.local', 5, N'Geniþ ve temiz oda, zengin kahvaltý, ilgili personel ve geniþ otopark olumlu bulunmuþ.'),
(12, N'Divan Ankara', N'hotel.review.012@hotelapp.local', 5, N'Odanýn temizliði ve kullanýþlýlýðý beðenilmiþ; yaþanan klima sorununa hýzlý çözüm üretilmiþ.'),
(13, N'DoubleTree by Hilton Çanakkale', N'hotel.review.013@hotelapp.local', 5, N'Konum, temizlik ve personel ilgisi baþarýlý bulunmuþ; þehir merkezine yakýnlýðý avantaj saðlamýþ.'),
(14, N'DoubleTree by Hilton Çanakkale', N'hotel.review.014@hotelapp.local', 4, N'Oda konforu ve kahvaltý olumlu deðerlendirilmiþ; kýsa konaklama için yeterli ve rahat bulunmuþ.'),
(15, N'DoubleTree by Hilton Çanakkale', N'hotel.review.015@hotelapp.local', 5, N'Havuz, spa ve genel hizmet kalitesi beðenilmiþ; aile konaklamasý için uygun görülmüþ.'),
(16, N'DoubleTree by Hilton Gaziantep', N'hotel.review.016@hotelapp.local', 4, N'Þehir merkezi konumu ve personel ilgisi beðenilmiþ; iþ seyahati için pratik bulunmuþ.'),
(17, N'DoubleTree by Hilton Gaziantep', N'hotel.review.017@hotelapp.local', 4, N'Oda temizliði, kahvaltý ve vale hizmeti olumlu deðerlendirilmiþ.'),
(18, N'DoubleTree by Hilton Gaziantep', N'hotel.review.018@hotelapp.local', 3, N'Genel konaklama yeterli bulunmuþ; bazý hizmetlerde yenileme ve hýz beklentisi dile getirilmiþ.'),
(19, N'Hilton Bursa Convention Center', N'hotel.review.019@hotelapp.local', 5, N'Konum, oda kalitesi, kahvaltý ve personel ilgisi çok baþarýlý bulunmuþ.'),
(20, N'Hilton Bursa Convention Center', N'hotel.review.020@hotelapp.local', 5, N'Spa, havuz ve temizlik standartlarý beðenilmiþ; iþ ve aile konaklamasý için uygun görülmüþ.'),
(21, N'Hilton Bursa Convention Center', N'hotel.review.021@hotelapp.local', 4, N'Otel genel olarak kaliteli bulunmuþ; yoðun dönemlerde servis hýzýnýn artmasý beklenmiþ.'),
(22, N'Jiva Beach Resort', N'hotel.review.022@hotelapp.local', 5, N'Yemek çeþitliliði, temizlik, plaj ve personel ilgisi yüksek memnuniyet oluþturmuþ.'),
(23, N'Jiva Beach Resort', N'hotel.review.023@hotelapp.local', 5, N'Aile tatili için aktiviteler, havuzlar ve çocuk imkanlarý baþarýlý bulunmuþ.'),
(24, N'Jiva Beach Resort', N'hotel.review.024@hotelapp.local', 4, N'Tesisin doðasý ve konumu beðenilmiþ; bazý alanlarda yoðunluk hissedildiði belirtilmiþ.'),
(25, N'Key Hotel Ýzmir', N'hotel.review.025@hotelapp.local', 5, N'Merkezi konum, oda konforu ve butik otel atmosferi çok baþarýlý bulunmuþ.'),
(26, N'Key Hotel Ýzmir', N'hotel.review.026@hotelapp.local', 5, N'Personel ilgisi, temizlik ve kahvaltý kalitesi olumlu deðerlendirilmiþ.'),
(27, N'Key Hotel Ýzmir', N'hotel.review.027@hotelapp.local', 5, N'Ýzmir merkezde konforlu ve sakin konaklama isteyenler için tavsiye edilmiþ.'),
(28, N'Liberty Fabay', N'hotel.review.028@hotelapp.local', 5, N'Oda temizliði, yemek çeþitliliði, ýsýtmalý havuz ve aktiviteler çok beðenilmiþ.'),
(29, N'Liberty Fabay', N'hotel.review.029@hotelapp.local', 5, N'Yemek, eðlence, çocuk imkanlarý ve personel hizmeti baþarýlý bulunmuþ.'),
(30, N'Liberty Fabay', N'hotel.review.030@hotelapp.local', 4, N'Genel tesis kalitesi beðenilmiþ; bazý hizmetlerde yerli misafire ilginin artmasý beklenmiþ.'),
(31, N'Liberty Lykia', N'hotel.review.031@hotelapp.local', 5, N'Doða, deniz, yemek kalitesi ve geniþ tesis imkanlarý çok baþarýlý bulunmuþ.'),
(32, N'Liberty Lykia', N'hotel.review.032@hotelapp.local', 5, N'Çocuklu aileler için aktiviteler ve plaj olanaklarý memnuniyet yaratmýþ.'),
(33, N'Liberty Lykia', N'hotel.review.033@hotelapp.local', 4, N'Tesis çok geniþ ve keyifli bulunmuþ; bazý alanlarda ulaþým ve yoðunluk yorucu gelebilmiþ.'),
(34, N'Lujo Hotel Bodrum', N'hotel.review.034@hotelapp.local', 5, N'Hizmet kalitesi, restoranlar, deniz ve oda konforu üst düzey bulunmuþ.'),
(35, N'Lujo Hotel Bodrum', N'hotel.review.035@hotelapp.local', 5, N'Personel ilgisi, temizlik ve konsept detaylarý lüks tatil beklentisini karþýlamýþ.'),
(36, N'Lujo Hotel Bodrum', N'hotel.review.036@hotelapp.local', 4, N'Genel kalite çok yüksek bulunmuþ; fiyat performans tarafýnda beklenti kiþiye göre deðiþmiþ.'),
(37, N'Mandarin Oriental, Bodrum', N'hotel.review.037@hotelapp.local', 5, N'Deniz, oda kalitesi, servis ve sakin atmosfer çok üst düzey deðerlendirilmiþ.'),
(38, N'Mandarin Oriental, Bodrum', N'hotel.review.038@hotelapp.local', 5, N'Personel ilgisi ve tesisin lüks standartlarý misafirleri memnun etmiþ.'),
(39, N'Mandarin Oriental, Bodrum', N'hotel.review.039@hotelapp.local', 5, N'Balayý ve özel tatil için konforlu, kaliteli ve huzurlu bir tesis olarak görülmüþ.'),
(40, N'Maxx Royal Belek Golf Resort', N'hotel.review.040@hotelapp.local', 5, N'Yemek kalitesi, oda konforu, çocuk imkanlarý ve servis seviyesi çok beðenilmiþ.'),
(41, N'Maxx Royal Belek Golf Resort', N'hotel.review.041@hotelapp.local', 5, N'Personel ilgisi, plaj, golf ve aktivite imkanlarý üst düzey bulunmuþ.'),
(42, N'Maxx Royal Belek Golf Resort', N'hotel.review.042@hotelapp.local', 5, N'Tesis lüks, düzenli ve aile tatili için güçlü bir seçenek olarak deðerlendirilmiþ.'),
(43, N'Novotel Kayseri', N'hotel.review.043@hotelapp.local', 4, N'Merkezi konum, temiz oda ve pratik konaklama imkaný olumlu bulunmuþ.'),
(44, N'Novotel Kayseri', N'hotel.review.044@hotelapp.local', 4, N'Kahvaltý ve personel ilgisi beðenilmiþ; iþ seyahatleri için uygun görülmüþ.'),
(45, N'Novotel Kayseri', N'hotel.review.045@hotelapp.local', 4, N'Oda konforu yeterli bulunmuþ; fiyat performans dengesi olumlu deðerlendirilmiþ.'),
(46, N'Ramada Plaza Trabzon', N'hotel.review.046@hotelapp.local', 5, N'Denize sýfýr konum, manzara, oda temizliði ve kahvaltý baþarýlý bulunmuþ.'),
(47, N'Ramada Plaza Trabzon', N'hotel.review.047@hotelapp.local', 4, N'Havuz, spa ve genel tesis olanaklarý beðenilmiþ; yoðunlukta servis hýzý deðiþebilmiþ.'),
(48, N'Ramada Plaza Trabzon', N'hotel.review.048@hotelapp.local', 5, N'Personel ilgisi ve aile konaklamasýna uygun atmosfer olumlu deðerlendirilmiþ.'),
(49, N'Ramada Resort Kusadasi', N'hotel.review.049@hotelapp.local', 4, N'Oda büyüklüðü, havuz alanlarý ve resort imkanlarý beðenilmiþ.'),
(50, N'Ramada Resort Kusadasi', N'hotel.review.050@hotelapp.local', 4, N'Aile konaklamasý için rahat bulunmuþ; personel ilgisi ve temizlik olumlu deðerlendirilmiþ.'),
(51, N'Ramada Resort Kusadasi', N'hotel.review.051@hotelapp.local', 4, N'Genel tesis olanaklarý yeterli görülmüþ; plaj eriþimi ve servis beklentileri deðiþebilmiþ.'),
(52, N'Sundia by Liberty Ölüdeniz', N'hotel.review.052@hotelapp.local', 4, N'Personel ilgisi, animasyon ekibi ve konum beðenilmiþ; otopark konusu zayýf bulunmuþ.'),
(53, N'Sundia by Liberty Ölüdeniz', N'hotel.review.053@hotelapp.local', 4, N'Yemek çeþitleri ve plaja yakýnlýk olumlu; bazý lezzetlerde beklenti daha yüksek olmuþ.'),
(54, N'Sundia by Liberty Ölüdeniz', N'hotel.review.054@hotelapp.local', 4, N'Aile ve çift tatili için uygun bulunmuþ; yenilenen odalar olumlu karþýlanmýþ.'),
(55, N'Swissôtel Büyük Efes Ýzmir', N'hotel.review.055@hotelapp.local', 5, N'Merkezi konum, bahçe, oda konforu ve kahvaltý kalitesi çok beðenilmiþ.'),
(56, N'Swissôtel Büyük Efes Ýzmir', N'hotel.review.056@hotelapp.local', 5, N'Personel ilgisi, temizlik ve þehir oteli standardý yüksek bulunmuþ.'),
(57, N'Swissôtel Büyük Efes Ýzmir', N'hotel.review.057@hotelapp.local', 4, N'Tesis genel olarak kaliteli bulunmuþ; yoðun dönemlerde hizmet temposu deðiþebilmiþ.'),
(58, N'Swissôtel Resort Bodrum Beach', N'hotel.review.058@hotelapp.local', 5, N'Denize sýfýr konum, plaj, oda konforu ve sakin atmosfer çok beðenilmiþ.'),
(59, N'Swissôtel Resort Bodrum Beach', N'hotel.review.059@hotelapp.local', 4, N'Kahvaltý, temizlik ve personel ilgisi olumlu deðerlendirilmiþ.'),
(60, N'Swissôtel Resort Bodrum Beach', N'hotel.review.060@hotelapp.local', 4, N'Tesis kaliteli bulunmuþ; bazý dönemlerde fiyat performans beklentisi daha yüksek olmuþ.'),
(61, N'Swissotel The Bosphorus', N'hotel.review.061@hotelapp.local', 5, N'Boðaz manzarasý, oda kalitesi, spa ve merkezi konum çok baþarýlý bulunmuþ.'),
(62, N'Swissotel The Bosphorus', N'hotel.review.062@hotelapp.local', 5, N'Personel ilgisi, temizlik ve kahvaltý kalitesi üst düzey deðerlendirilmiþ.'),
(63, N'Swissotel The Bosphorus', N'hotel.review.063@hotelapp.local', 5, N'Özel gün ve þehir tatili için konforlu, prestijli ve huzurlu bir otel olarak görülmüþ.'),
(64, N'The Bodrum EDITION', N'hotel.review.064@hotelapp.local', 5, N'Deniz, plaj, tasarým ve oda konforu çok beðenilmiþ.'),
(65, N'The Bodrum EDITION', N'hotel.review.065@hotelapp.local', 5, N'Servis kalitesi, restoranlar ve sakin atmosfer lüks tatil beklentisini karþýlamýþ.'),
(66, N'The Bodrum EDITION', N'hotel.review.066@hotelapp.local', 4, N'Tesis çok kaliteli bulunmuþ; fiyat seviyesi yüksek olduðu için beklenti de yüksek olmuþ.'),
(67, N'Wyndham Grand Izmir Ozdilek', N'hotel.review.067@hotelapp.local', 5, N'Deniz manzarasý, termal alan, havuzlar ve oda temizliði çok baþarýlý bulunmuþ.'),
(68, N'Wyndham Grand Izmir Ozdilek', N'hotel.review.068@hotelapp.local', 5, N'Personel ilgisi, kahvaltý ve spa olanaklarý olumlu deðerlendirilmiþ.'),
(69, N'Wyndham Grand Izmir Ozdilek', N'hotel.review.069@hotelapp.local', 4, N'Konum ve hizmet kalitesi beðenilmiþ; bazý misafirler þehir merkezine mesafeyi not etmiþ.'),
(70, N'Yacht Classic Hotel', N'hotel.review.070@hotelapp.local', 5, N'Marina manzarasý, oda konforu, kahvaltý ve butik atmosfer çok beðenilmiþ.'),
(71, N'Yacht Classic Hotel', N'hotel.review.071@hotelapp.local', 5, N'Personel ilgisi, temizlik ve restoran deneyimi çok olumlu deðerlendirilmiþ.'),
(72, N'Yacht Classic Hotel', N'hotel.review.072@hotelapp.local', 5, N'Romantik ve sakin tatil için tekrar tercih edilebilecek bir otel olarak görülmüþ.'),
(73, N'Yazz Collective Fethiye', N'hotel.review.073@hotelapp.local', 5, N'Doðayla iç içe atmosfer, deniz, oda tasarýmý ve servis kalitesi çok beðenilmiþ.'),
(74, N'Yazz Collective Fethiye', N'hotel.review.074@hotelapp.local', 5, N'Sakinlik, restoran deneyimi ve özel plaj hissi olumlu deðerlendirilmiþ.'),
(75, N'Yazz Collective Fethiye', N'hotel.review.075@hotelapp.local', 4, N'Tesis konsepti çok özel bulunmuþ; ulaþým ve fiyat seviyesi beklentiye göre deðiþebilmiþ.')
) v(SortNo, HotelName, UserEmail, Rating, Comment)
JOIN Hotels h ON h.Name = v.HotelName
JOIN Users u ON u.Email = v.UserEmail
WHERE NOT EXISTS (
    SELECT 1
    FROM Reviews x
    WHERE x.HotelId = h.Id
      AND x.UserId = u.Id
);

SELECT h.Name, COUNT(rv.Id) AS ReviewCount
FROM Hotels h
LEFT JOIN Reviews rv ON rv.HotelId = h.Id
WHERE h.Name IN (
    N'Adam & Eve - Adult Only',
    N'Belcekiz Beach Club',
    N'Dedeman Konya',
    N'Divan Ankara',
    N'DoubleTree by Hilton Çanakkale',
    N'DoubleTree by Hilton Gaziantep',
    N'Hilton Bursa Convention Center',
    N'Jiva Beach Resort',
    N'Key Hotel Ýzmir',
    N'Liberty Fabay',
    N'Liberty Lykia',
    N'Lujo Hotel Bodrum',
    N'Mandarin Oriental, Bodrum',
    N'Maxx Royal Belek Golf Resort',
    N'Novotel Kayseri',
    N'Ramada Plaza Trabzon',
    N'Ramada Resort Kusadasi',
    N'Sundia by Liberty Ölüdeniz',
    N'Swissôtel Büyük Efes Ýzmir',
    N'Swissôtel Resort Bodrum Beach',
    N'Swissotel The Bosphorus',
    N'The Bodrum EDITION',
    N'Wyndham Grand Izmir Ozdilek',
    N'Yacht Classic Hotel',
    N'Yazz Collective Fethiye'
)
GROUP BY h.Name
ORDER BY h.Name;
GO