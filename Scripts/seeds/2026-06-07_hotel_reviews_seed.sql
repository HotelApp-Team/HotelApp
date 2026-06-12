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
(1, N'Adam & Eve - Adult Only', N'hotel.review.001@hotelapp.local', 4, N'Eğlence, barlar ve açık büfe güçlü bulunmuş; otelin büyük yapısı nedeniyle ilk gün alışma süreci yaşanmış.'),
(2, N'Adam & Eve - Adult Only', N'hotel.review.002@hotelapp.local', 4, N'Oda, temizlik, yemek ve havuz imkanları beğenilmiş; yoğun parti ortamı bazı misafirler için yorucu olmuş.'),
(3, N'Adam & Eve - Adult Only', N'hotel.review.003@hotelapp.local', 5, N'Oda erken teslim edilmiş, eğlence programları ve dans gösterileri çok başarılı bulunmuş.'),
(4, N'Belcekiz Beach Club', N'hotel.review.004@hotelapp.local', 5, N'Girişten itibaren ilgi, oda temizliği, servis ve yiyecek içecek kalitesi beklentinin üzerinde bulunmuş.'),
(5, N'Belcekiz Beach Club', N'hotel.review.005@hotelapp.local', 5, N'Personel profesyonelliği, denize yakınlık, canlı müzik ve fiyat performans çok beğenilmiş.'),
(6, N'Belcekiz Beach Club', N'hotel.review.006@hotelapp.local', 5, N'Temizlik, rahatlık, yemekler ve lokasyon başarılı bulunmuş; tekrar tercih edilebilecek bir tesis olarak değerlendirilmiş.'),
(7, N'Dedeman Konya', N'hotel.review.007@hotelapp.local', 5, N'Kahvaltı, spor alanları, temizlik ve personel ilgisi sürekli aynı kalitede bulunmuş.'),
(8, N'Dedeman Konya', N'hotel.review.008@hotelapp.local', 5, N'Karşılama, uğurlama, oda hijyeni, yemekler, havuz ve merkezi konum çok olumlu değerlendirilmiş.'),
(9, N'Dedeman Konya', N'hotel.review.009@hotelapp.local', 4, N'Otel temizliği ve çalışan ilgisi beğenilmiş; kahvaltı çeşitliliği yeterli ama çok geniş bulunmamış.'),
(10, N'Divan Ankara', N'hotel.review.010@hotelapp.local', 5, N'Oda temizliği, kalite, hizmet ve personel karşılaması nedeniyle otel tavsiye edilmiş.'),
(11, N'Divan Ankara', N'hotel.review.011@hotelapp.local', 5, N'Geniş ve temiz oda, zengin kahvaltı, ilgili personel ve geniş otopark olumlu bulunmuş.'),
(12, N'Divan Ankara', N'hotel.review.012@hotelapp.local', 5, N'Odanın temizliği ve kullanışlılığı beğenilmiş; yaşanan klima sorununa hızlı çözüm üretilmiş.'),
(13, N'DoubleTree by Hilton Çanakkale', N'hotel.review.013@hotelapp.local', 5, N'Konum, temizlik ve personel ilgisi başarılı bulunmuş; şehir merkezine yakınlığı avantaj sağlamış.'),
(14, N'DoubleTree by Hilton Çanakkale', N'hotel.review.014@hotelapp.local', 4, N'Oda konforu ve kahvaltı olumlu değerlendirilmiş; kısa konaklama için yeterli ve rahat bulunmuş.'),
(15, N'DoubleTree by Hilton Çanakkale', N'hotel.review.015@hotelapp.local', 5, N'Havuz, spa ve genel hizmet kalitesi beğenilmiş; aile konaklaması için uygun görülmüş.'),
(16, N'DoubleTree by Hilton Gaziantep', N'hotel.review.016@hotelapp.local', 4, N'Şehir merkezi konumu ve personel ilgisi beğenilmiş; iş seyahati için pratik bulunmuş.'),
(17, N'DoubleTree by Hilton Gaziantep', N'hotel.review.017@hotelapp.local', 4, N'Oda temizliği, kahvaltı ve vale hizmeti olumlu değerlendirilmiş.'),
(18, N'DoubleTree by Hilton Gaziantep', N'hotel.review.018@hotelapp.local', 3, N'Genel konaklama yeterli bulunmuş; bazı hizmetlerde yenileme ve hız beklentisi dile getirilmiş.'),
(19, N'Hilton Bursa Convention Center', N'hotel.review.019@hotelapp.local', 5, N'Konum, oda kalitesi, kahvaltı ve personel ilgisi çok başarılı bulunmuş.'),
(20, N'Hilton Bursa Convention Center', N'hotel.review.020@hotelapp.local', 5, N'Spa, havuz ve temizlik standartları beğenilmiş; iş ve aile konaklaması için uygun görülmüş.'),
(21, N'Hilton Bursa Convention Center', N'hotel.review.021@hotelapp.local', 4, N'Otel genel olarak kaliteli bulunmuş; yoğun dönemlerde servis hızının artması beklenmiş.'),
(22, N'Jiva Beach Resort', N'hotel.review.022@hotelapp.local', 5, N'Yemek çeşitliliği, temizlik, plaj ve personel ilgisi yüksek memnuniyet oluşturmuş.'),
(23, N'Jiva Beach Resort', N'hotel.review.023@hotelapp.local', 5, N'Aile tatili için aktiviteler, havuzlar ve çocuk imkanları başarılı bulunmuş.'),
(24, N'Jiva Beach Resort', N'hotel.review.024@hotelapp.local', 4, N'Tesisin doğası ve konumu beğenilmiş; bazı alanlarda yoğunluk hissedildiği belirtilmiş.'),
(25, N'Key Hotel İzmir', N'hotel.review.025@hotelapp.local', 5, N'Merkezi konum, oda konforu ve butik otel atmosferi çok başarılı bulunmuş.'),
(26, N'Key Hotel İzmir', N'hotel.review.026@hotelapp.local', 5, N'Personel ilgisi, temizlik ve kahvaltı kalitesi olumlu değerlendirilmiş.'),
(27, N'Key Hotel İzmir', N'hotel.review.027@hotelapp.local', 5, N'İzmir merkezde konforlu ve sakin konaklama isteyenler için tavsiye edilmiş.'),
(28, N'Liberty Fabay', N'hotel.review.028@hotelapp.local', 5, N'Oda temizliği, yemek çeşitliliği, ısıtmalı havuz ve aktiviteler çok beğenilmiş.'),
(29, N'Liberty Fabay', N'hotel.review.029@hotelapp.local', 5, N'Yemek, eğlence, çocuk imkanları ve personel hizmeti başarılı bulunmuş.'),
(30, N'Liberty Fabay', N'hotel.review.030@hotelapp.local', 4, N'Genel tesis kalitesi beğenilmiş; bazı hizmetlerde yerli misafire ilginin artması beklenmiş.'),
(31, N'Liberty Lykia', N'hotel.review.031@hotelapp.local', 5, N'Doğa, deniz, yemek kalitesi ve geniş tesis imkanları çok başarılı bulunmuş.'),
(32, N'Liberty Lykia', N'hotel.review.032@hotelapp.local', 5, N'Çocuklu aileler için aktiviteler ve plaj olanakları memnuniyet yaratmış.'),
(33, N'Liberty Lykia', N'hotel.review.033@hotelapp.local', 4, N'Tesis çok geniş ve keyifli bulunmuş; bazı alanlarda ulaşım ve yoğunluk yorucu gelebilmiş.'),
(34, N'Lujo Hotel Bodrum', N'hotel.review.034@hotelapp.local', 5, N'Hizmet kalitesi, restoranlar, deniz ve oda konforu üst düzey bulunmuş.'),
(35, N'Lujo Hotel Bodrum', N'hotel.review.035@hotelapp.local', 5, N'Personel ilgisi, temizlik ve konsept detayları lüks tatil beklentisini karşılamış.'),
(36, N'Lujo Hotel Bodrum', N'hotel.review.036@hotelapp.local', 4, N'Genel kalite çok yüksek bulunmuş; fiyat performans tarafında beklenti kişiye göre değişmiş.'),
(37, N'Mandarin Oriental, Bodrum', N'hotel.review.037@hotelapp.local', 5, N'Deniz, oda kalitesi, servis ve sakin atmosfer çok üst düzey değerlendirilmiş.'),
(38, N'Mandarin Oriental, Bodrum', N'hotel.review.038@hotelapp.local', 5, N'Personel ilgisi ve tesisin lüks standartları misafirleri memnun etmiş.'),
(39, N'Mandarin Oriental, Bodrum', N'hotel.review.039@hotelapp.local', 5, N'Balayı ve özel tatil için konforlu, kaliteli ve huzurlu bir tesis olarak görülmüş.'),
(40, N'Maxx Royal Belek Golf Resort', N'hotel.review.040@hotelapp.local', 5, N'Yemek kalitesi, oda konforu, çocuk imkanları ve servis seviyesi çok beğenilmiş.'),
(41, N'Maxx Royal Belek Golf Resort', N'hotel.review.041@hotelapp.local', 5, N'Personel ilgisi, plaj, golf ve aktivite imkanları üst düzey bulunmuş.'),
(42, N'Maxx Royal Belek Golf Resort', N'hotel.review.042@hotelapp.local', 5, N'Tesis lüks, düzenli ve aile tatili için güçlü bir seçenek olarak değerlendirilmiş.'),
(43, N'Novotel Kayseri', N'hotel.review.043@hotelapp.local', 4, N'Merkezi konum, temiz oda ve pratik konaklama imkanı olumlu bulunmuş.'),
(44, N'Novotel Kayseri', N'hotel.review.044@hotelapp.local', 4, N'Kahvaltı ve personel ilgisi beğenilmiş; iş seyahatleri için uygun görülmüş.'),
(45, N'Novotel Kayseri', N'hotel.review.045@hotelapp.local', 4, N'Oda konforu yeterli bulunmuş; fiyat performans dengesi olumlu değerlendirilmiş.'),
(46, N'Ramada Plaza Trabzon', N'hotel.review.046@hotelapp.local', 5, N'Denize sıfır konum, manzara, oda temizliği ve kahvaltı başarılı bulunmuş.'),
(47, N'Ramada Plaza Trabzon', N'hotel.review.047@hotelapp.local', 4, N'Havuz, spa ve genel tesis olanakları beğenilmiş; yoğunlukta servis hızı değişebilmiş.'),
(48, N'Ramada Plaza Trabzon', N'hotel.review.048@hotelapp.local', 5, N'Personel ilgisi ve aile konaklamasına uygun atmosfer olumlu değerlendirilmiş.'),
(49, N'Ramada Resort Kusadasi', N'hotel.review.049@hotelapp.local', 4, N'Oda büyüklüğü, havuz alanları ve resort imkanları beğenilmiş.'),
(50, N'Ramada Resort Kusadasi', N'hotel.review.050@hotelapp.local', 4, N'Aile konaklaması için rahat bulunmuş; personel ilgisi ve temizlik olumlu değerlendirilmiş.'),
(51, N'Ramada Resort Kusadasi', N'hotel.review.051@hotelapp.local', 4, N'Genel tesis olanakları yeterli görülmüş; plaj erişimi ve servis beklentileri değişebilmiş.'),
(52, N'Sundia by Liberty Ölüdeniz', N'hotel.review.052@hotelapp.local', 4, N'Personel ilgisi, animasyon ekibi ve konum beğenilmiş; otopark konusu zayıf bulunmuş.'),
(53, N'Sundia by Liberty Ölüdeniz', N'hotel.review.053@hotelapp.local', 4, N'Yemek çeşitleri ve plaja yakınlık olumlu; bazı lezzetlerde beklenti daha yüksek olmuş.'),
(54, N'Sundia by Liberty Ölüdeniz', N'hotel.review.054@hotelapp.local', 4, N'Aile ve çift tatili için uygun bulunmuş; yenilenen odalar olumlu karşılanmış.'),
(55, N'Swissôtel Büyük Efes İzmir', N'hotel.review.055@hotelapp.local', 5, N'Merkezi konum, bahçe, oda konforu ve kahvaltı kalitesi çok beğenilmiş.'),
(56, N'Swissôtel Büyük Efes İzmir', N'hotel.review.056@hotelapp.local', 5, N'Personel ilgisi, temizlik ve şehir oteli standardı yüksek bulunmuş.'),
(57, N'Swissôtel Büyük Efes İzmir', N'hotel.review.057@hotelapp.local', 4, N'Tesis genel olarak kaliteli bulunmuş; yoğun dönemlerde hizmet temposu değişebilmiş.'),
(58, N'Swissôtel Resort Bodrum Beach', N'hotel.review.058@hotelapp.local', 5, N'Denize sıfır konum, plaj, oda konforu ve sakin atmosfer çok beğenilmiş.'),
(59, N'Swissôtel Resort Bodrum Beach', N'hotel.review.059@hotelapp.local', 4, N'Kahvaltı, temizlik ve personel ilgisi olumlu değerlendirilmiş.'),
(60, N'Swissôtel Resort Bodrum Beach', N'hotel.review.060@hotelapp.local', 4, N'Tesis kaliteli bulunmuş; bazı dönemlerde fiyat performans beklentisi daha yüksek olmuş.'),
(61, N'Swissotel The Bosphorus', N'hotel.review.061@hotelapp.local', 5, N'Boğaz manzarası, oda kalitesi, spa ve merkezi konum çok başarılı bulunmuş.'),
(62, N'Swissotel The Bosphorus', N'hotel.review.062@hotelapp.local', 5, N'Personel ilgisi, temizlik ve kahvaltı kalitesi üst düzey değerlendirilmiş.'),
(63, N'Swissotel The Bosphorus', N'hotel.review.063@hotelapp.local', 5, N'Özel gün ve şehir tatili için konforlu, prestijli ve huzurlu bir otel olarak görülmüş.'),
(64, N'The Bodrum EDITION', N'hotel.review.064@hotelapp.local', 5, N'Deniz, plaj, tasarım ve oda konforu çok beğenilmiş.'),
(65, N'The Bodrum EDITION', N'hotel.review.065@hotelapp.local', 5, N'Servis kalitesi, restoranlar ve sakin atmosfer lüks tatil beklentisini karşılamış.'),
(66, N'The Bodrum EDITION', N'hotel.review.066@hotelapp.local', 4, N'Tesis çok kaliteli bulunmuş; fiyat seviyesi yüksek olduğu için beklenti de yüksek olmuş.'),
(67, N'Wyndham Grand Izmir Ozdilek', N'hotel.review.067@hotelapp.local', 5, N'Deniz manzarası, termal alan, havuzlar ve oda temizliği çok başarılı bulunmuş.'),
(68, N'Wyndham Grand Izmir Ozdilek', N'hotel.review.068@hotelapp.local', 5, N'Personel ilgisi, kahvaltı ve spa olanakları olumlu değerlendirilmiş.'),
(69, N'Wyndham Grand Izmir Ozdilek', N'hotel.review.069@hotelapp.local', 4, N'Konum ve hizmet kalitesi beğenilmiş; bazı misafirler şehir merkezine mesafeyi not etmiş.'),
(70, N'Yacht Classic Hotel', N'hotel.review.070@hotelapp.local', 5, N'Marina manzarası, oda konforu, kahvaltı ve butik atmosfer çok beğenilmiş.'),
(71, N'Yacht Classic Hotel', N'hotel.review.071@hotelapp.local', 5, N'Personel ilgisi, temizlik ve restoran deneyimi çok olumlu değerlendirilmiş.'),
(72, N'Yacht Classic Hotel', N'hotel.review.072@hotelapp.local', 5, N'Romantik ve sakin tatil için tekrar tercih edilebilecek bir otel olarak görülmüş.'),
(73, N'Yazz Collective Fethiye', N'hotel.review.073@hotelapp.local', 5, N'Doğayla iç içe atmosfer, deniz, oda tasarımı ve servis kalitesi çok beğenilmiş.'),
(74, N'Yazz Collective Fethiye', N'hotel.review.074@hotelapp.local', 5, N'Sakinlik, restoran deneyimi ve özel plaj hissi olumlu değerlendirilmiş.'),
(75, N'Yazz Collective Fethiye', N'hotel.review.075@hotelapp.local', 4, N'Tesis konsepti çok özel bulunmuş; ulaşım ve fiyat seviyesi beklentiye göre değişebilmiş.')
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
    N'Key Hotel İzmir',
    N'Liberty Fabay',
    N'Liberty Lykia',
    N'Lujo Hotel Bodrum',
    N'Mandarin Oriental, Bodrum',
    N'Maxx Royal Belek Golf Resort',
    N'Novotel Kayseri',
    N'Ramada Plaza Trabzon',
    N'Ramada Resort Kusadasi',
    N'Sundia by Liberty Ölüdeniz',
    N'Swissôtel Büyük Efes İzmir',
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