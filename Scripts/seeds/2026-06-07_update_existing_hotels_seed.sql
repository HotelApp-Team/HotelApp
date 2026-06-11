USE HotelAppDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
GO

DECLARE @ids TABLE (Id INT);

INSERT INTO @ids (Id)
SELECT Id
FROM Hotels
WHERE Name IN (
    N'Dedeman Konya',
    N'Divan Ankara',
    N'DoubleTree by Hilton Gaziantep',
    N'Hilton Bursa Convention Center',
    N'Novotel Kayseri',
    N'Ramada Plaza Trabzon',
    N'Ramada Resort Kusadasi',
    N'Swissotel The Bosphorus',
    N'Wyndham Grand Izmir Ozdilek',
    N'Yazz Collective Fethiye'
);

DELETE r  FROM Reservations r        JOIN HotelRooms hr ON r.HotelRoomId = hr.Id  WHERE hr.HotelId IN (SELECT Id FROM @ids);
DELETE ri FROM RoomImages ri         JOIN HotelRooms hr ON ri.HotelRoomId = hr.Id WHERE hr.HotelId IN (SELECT Id FROM @ids);
DELETE ra FROM HotelRoomAmenities ra JOIN HotelRooms hr ON ra.HotelRoomId = hr.Id WHERE hr.HotelId IN (SELECT Id FROM @ids);
DELETE FROM HotelRooms     WHERE HotelId IN (SELECT Id FROM @ids);
DELETE FROM HotelImages    WHERE HotelId IN (SELECT Id FROM @ids);
DELETE FROM HotelAmenities WHERE HotelId IN (SELECT Id FROM @ids);
DELETE FROM Reviews        WHERE HotelId IN (SELECT Id FROM @ids);
DELETE FROM Hotels         WHERE Id IN (SELECT Id FROM @ids);
GO

INSERT INTO Amenities (Name, Icon)
SELECT v.Name, v.Icon
FROM (VALUES
(N'Toplantý Salonu', N'briefcase'),
(N'Konferans Salonu', N'briefcase'),
(N'Kapalý Havuz', N'water'),
(N'Otopark', N'p-square'),
(N'Vale Servisi', N'car-front'),
(N'Kuru Temizleme', N'washing-machine'),
(N'24 Saat Resepsiyon', N'clock'),
(N'Kütüphane', N'book'),
(N'Asansör', N'arrow-up-square'),
(N'Teras', N'sun'),
(N'Oda Servisi', N'bell'),
(N'Bagaj Odasý', N'briefcase'),
(N'Kahvaltý', N'cup-hot'),
(N'Çocuk Parký', N'emoji-smile'),
(N'Jakuzi', N'droplet-half'),
(N'Evcil Hayvan Dostu', N'heart'),
(N'Denize Sýfýr', N'water'),
(N'Termal', N'thermometer'),
(N'Buhar Odasý', N'cloud'),
(N'Tenis Kortu', N'trophy')
) v(Name, Icon)
WHERE NOT EXISTS (SELECT 1 FROM Amenities a WHERE a.Name = v.Name);
GO

INSERT INTO RoomAmenities (Name, Icon)
SELECT v.Name, v.Icon
FROM (VALUES
(N'Oda Servisi', N'bell'),
(N'Sigara Ýçilmez', N'slash-circle'),
(N'Balkon veya Teras', N'door-open'),
(N'Buklet Banyo Ürünleri', N'basket'),
(N'Havlu', N'layers'),
(N'Duþ veya Küvet', N'droplet'),
(N'Merkezi Klima', N'snow'),
(N'Uyandýrma Servisi', N'alarm'),
(N'Telefon', N'telephone'),
(N'Su Isýtýcýsý', N'cup-hot'),
(N'Elektronik Anahtar Sistemi', N'key'),
(N'Gardýrop', N'archive'),
(N'Ücretsiz Þiþe Su', N'droplet'),
(N'Oturma Alaný', N'chair'),
(N'Kahve Makinesi', N'cup-hot'),
(N'Bornoz', N'shirt'),
(N'Terlik', N'footprints'),
(N'Yastýk Menüsü', N'layers'),
(N'Executive Lounge Giriþi', N'star'),
(N'Balkonda Jakuzi', N'droplet-half'),
(N'2 Yatak Odasý', N'house'),
(N'Oturma Odasý', N'chair'),
(N'Buzdolabý', N'box')
) v(Name, Icon)
WHERE NOT EXISTS (SELECT 1 FROM RoomAmenities ra WHERE ra.Name = v.Name);
GO

INSERT INTO RoomTypes (Name, Description, MaxGuests)
SELECT v.Name, v.Description, v.MaxGuests
FROM (VALUES
(N'Dedeman - Superior Room', N'Dedeman Konya superior oda, 32 m2', 2),
(N'Dedeman - Superior Twin Room', N'Dedeman Konya superior iki ayrý yataklý oda, 32 m2', 2),
(N'Dedeman - Deluxe Room', N'Dedeman Konya deluxe oda, 38 m2', 3),
(N'Dedeman - Family Room', N'Dedeman Konya aile odasý, 57 m2', 4),
(N'Dedeman - Junior Suite', N'Dedeman Konya junior suite, 60 m2', 3),

(N'Divan Ankara - Superior Room', N'Divan Ankara superior oda, 29 m2', 2),
(N'Divan Ankara - Executive Room', N'Divan Ankara executive oda, 29 m2', 2),
(N'Divan Ankara - Deluxe Room', N'Divan Ankara deluxe oda, 35 m2', 3),
(N'Divan Ankara - Junior Suite', N'Divan Ankara junior suite, 60 m2', 3),
(N'Divan Ankara - Executive Suite', N'Divan Ankara executive suite, 100 m2', 4),

(N'Gaziantep - Queen Guest Room', N'DoubleTree by Hilton Gaziantep queen guest room', 2),
(N'Gaziantep - King Guest Room', N'DoubleTree by Hilton Gaziantep king guest room', 2),
(N'Gaziantep - King One Bedroom Suite', N'DoubleTree by Hilton Gaziantep king one bedroom suite', 4),

(N'Bursa Hilton - Queen Guest Room', N'Hilton Bursa queen guest room, 30 m2', 2),
(N'Bursa Hilton - Twin Guest Room', N'Hilton Bursa twin guest room, 30 m2', 2),
(N'Bursa Hilton - Executive Room', N'Hilton Bursa executive oda, 30 m2', 2),

(N'Novotel - Modern Room', N'Novotel Kayseri modern oda', 2),
(N'Novotel - Family Room', N'Novotel Kayseri aile dostu modern oda', 4),
(N'Novotel - Suite Room', N'Novotel Kayseri ayrý oturma alaný bulunan suite oda', 3),

(N'Trabzon - Deluxe City View Room', N'Ramada Plaza Trabzon þehir manzaralý balkonlu deluxe oda, 27 m2', 2),
(N'Trabzon - Deluxe Sea View Room', N'Ramada Plaza Trabzon deniz manzaralý balkonlu deluxe oda, 27 m2', 2),
(N'Trabzon - Corner Suite', N'Ramada Plaza Trabzon corner suite, 37 m2', 3),
(N'Trabzon - Business Room', N'Ramada Plaza Trabzon business room, 46 m2', 3),
(N'Trabzon - Family Connect Room', N'Ramada Plaza Trabzon family connect room, 54 m2', 4),
(N'Trabzon - Suite', N'Ramada Plaza Trabzon suite, 54 m2', 4),

(N'Kusadasi - Resort Room', N'Ramada Resort Kuþadasý balkon veya teraslý resort oda', 2),
(N'Kusadasi - Family Room', N'Ramada Resort Kuþadasý aile konaklamasýna uygun oda', 4),
(N'Kusadasi - Suite Room', N'Ramada Resort Kuþadasý oturma alanlý suite oda', 4),

(N'Swissotel - Bosphorus View Room', N'Swissotel The Bosphorus Boðaz manzaralý oda', 2),
(N'Swissotel - Residence Suite', N'Swissotel Living rezidans suite', 4),
(N'Swissotel - Family Residence', N'Swissotel aileler için geniþ rezidans dairesi', 5),

(N'Wyndham Izmir - Club Room', N'Wyndham Grand Ýzmir Özdilek kara manzaralý club room, 27 m2', 2),
(N'Wyndham Izmir - Standard Sea View Room', N'Wyndham Grand Ýzmir Özdilek deniz manzaralý standart oda, 27 m2', 3),
(N'Wyndham Izmir - Standard Triple Room', N'Wyndham Grand Ýzmir Özdilek standart üç kiþilik oda, 27 m2', 3),
(N'Wyndham Izmir - Standard Single Room', N'Wyndham Grand Ýzmir Özdilek standart single oda, 27 m2', 1),

(N'Yazz - Deluxe Room', N'Yazz Collective bahçe manzaralý teraslý deluxe oda, 36 m2', 2),
(N'Yazz - Grand Deluxe Room', N'Yazz Collective bahçe manzaralý teraslý grand deluxe oda, 50 m2', 2),
(N'Yazz - Executive Suite', N'Yazz Collective executive suite, 55 m2', 3),
(N'Yazz - King Suite', N'Yazz Collective king suite, 63 m2', 3),
(N'Yazz - Bungalow', N'Yazz Collective bungalov, 45 m2', 3),
(N'Yazz - Villa', N'Yazz Collective villa, 45 m2', 3),
(N'Yazz - Connecting Room', N'Yazz Collective baðlantýlý oda, 90 m2', 4)
) v(Name, Description, MaxGuests)
WHERE NOT EXISTS (SELECT 1 FROM RoomTypes rt WHERE rt.Name = v.Name);
GO

INSERT INTO Hotels
(Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, AvgRating, StarRating, IsActive, CreatedAt, UpdatedAt)
VALUES
(N'Dedeman Konya', N'Selçuklu’da yer alan Dedeman Konya; 206 oda, 12 toplantý salonu, 2 restoran, On8 Lounge Restaurant, Türk hamamý, fitness merkezi, 2 yüzme havuzu, squash ve SPA olanaklarýyla iþ ve turizm odaklý konaklama sunar.', N'Selçuklu, Konya Þehir Merkezi, Konya', N'Konya', N'Türkiye', N'+90 332 221 66 00', NULL, NULL, NULL, 8.9, 5, 1, GETDATE(), NULL),

(N'Divan Ankara', N'Ankara Çankaya’da hizmet veren Divan Ankara; restoran, bar, 3000 m2 SPA alaný, 12 toplantý salonundan oluþan 1500 m2 etkinlik merkezi ve iþ-kongre odaklý konaklama olanaklarý sunar.', N'Ankara Çankaya, Ankara Merkez, Ankara', N'Ankara', N'Türkiye', N'+90 312 409 30 00', NULL, NULL, NULL, 9.3, 5, 1, GETDATE(), NULL),

(N'DoubleTree by Hilton Gaziantep', N'Gaziantep þehir merkezinde yer alan DoubleTree by Hilton Gaziantep; konferans alaný, hýzlý giriþ, kuru temizleme, ücretsiz vale otopark, kütüphane ve fitness olanaklarýyla öne çýkar.', N'Gaziantep Þehir Merkezi, Gaziantep', N'Gaziantep', N'Türkiye', N'+90 342 999 41 11', N'reservation.gaziantep@hilton.com', NULL, NULL, 7.2, 4, 1, GETDATE(), NULL),

(N'Hilton Bursa Convention Center', N'Bursa Osmangazi’deki Hilton Bursa Convention Center And Spa; merkezi konumu, kapalý havuzu, saunasý, tam donanýmlý fitness merkezi ve restoranýyla iþ ve turistik seyahatlere hitap eder.', N'Bursa Merkez - Osmangazi, Bursa', N'Bursa', N'Türkiye', N'+90 224 500 05 05', N'bursa.sales@hilton.com', NULL, NULL, 9.2, 5, 1, GETDATE(), NULL),

(N'Novotel Kayseri', N'Yenipervane Mahallesi Kocasinan Bulvarý üzerindeki Novotel Kayseri; zengin açýk büfe kahvaltý, Erciyes Daðý manzaralý odalar, bahçe, teras, aile dostu odalar, restoran, bar, fitness ve merkezi konumuyla öne çýkar.', N'Yenipervane Mah, Kocasinan Blv. 161-1, 38110 Kayseri, Türkiye', N'Kayseri', N'Türkiye', N'+90 352 207 30 00', N'info@novotelkayseri.com', NULL, NULL, 8.7, 4, 1, GETDATE(), NULL),

(N'Ramada Plaza Trabzon', N'Ortahisar Trabzon’da denize sýfýr konumda yer alan Ramada Plaza; 350 odasý, deniz ve dað manzarasý, geniþ kumsalý, yetiþkin ve çocuk havuzlarý, SPA, fitness, toplantý salonlarý ve restoranlarýyla hizmet verir.', N'Ortahisar, Trabzon', N'Trabzon', N'Türkiye', N'+90 462 335 30 30', N'info@ramadaplazatrabzon.com', NULL, NULL, 8.8, 5, 1, GETDATE(), NULL),

(N'Ramada Resort Kusadasi', N'Kuþadasý Aydýn’da yer alan Ramada Resort By Wyndham Kuþadasý & Golf; plajlara, Kuþadasý Kalesi’ne ve ulaþým noktalarýna yakýn konumu, balkon/teraslý odalarý ve resort olanaklarýyla öne çýkar.', N'Türkmen Mahallesi Selçuk Bulvarý No:9/3 Kuþadasý, Aydýn', N'Aydýn', N'Türkiye', N'+90 256 420 07 00', NULL, NULL, NULL, NULL, 5, 1, GETDATE(), NULL),

(N'Swissotel The Bosphorus', N'Ýstanbul Viþnezade’de yer alan Swissôtel The Bosphorus; Boðaz manzarasý, 16 ROOF, Swissotel Living rezidanslarý, Purovel Spa & Sport, kapalý-açýk havuz, fitness, tenis kortlarý ve merkezi konumuyla öne çýkar.', N'Viþnezade, Acýsu Sk. No:19, 34357 Ýstanbul', N'Ýstanbul', N'Türkiye', N'+90 212 326 11 00', N'istanbul@swissotel.com', NULL, NULL, 9.4, 5, 1, GETDATE(), NULL),

(N'Wyndham Grand Izmir Ozdilek', N'Balçova Ýzmir’de yer alan Wyndham Grand Ýzmir Özdilek Thermal & Spa; termal wellness merkezi, açýk-kapalý havuz, termal havuz, jakuzi, sauna, Türk hamamý, buhar odalarý, fitness ve 10 salonuyla hizmet verir.', N'Balçova, Ýzmir', N'Ýzmir', N'Türkiye', N'+90 232 292 13 00', NULL, NULL, NULL, 9.4, 5, 1, GETDATE(), NULL),

(N'Yazz Collective Fethiye', N'Fethiye Muðla’da doðayla iç içe, denize sýfýr konumda yer alan Yazz Collective; modern tasarýmý, bahçe manzaralý teraslý odalarý, bungalovlarý, villalarý ve lüks kaçýþ atmosferiyle öne çýkar.', N'Fethiye, Muðla', N'Fethiye', N'Türkiye', N'+90 530 972 92 99', N'reservation@yazzcollective.com', NULL, NULL, 9.1, 5, 1, GETDATE(), NULL);
GO

INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId)
SELECT h.Id, rt.Id, s.Price, s.Extra, s.TotalRooms, 1, rv.Id
FROM (VALUES
(N'Dedeman Konya', N'Dedeman - Superior Room', N'Þehir Manzaralý', 5200, 700, 72),
(N'Dedeman Konya', N'Dedeman - Superior Twin Room', N'Þehir Manzaralý', 5400, 700, 60),
(N'Dedeman Konya', N'Dedeman - Deluxe Room', N'Þehir Manzaralý', 6500, 900, 36),
(N'Dedeman Konya', N'Dedeman - Family Room', N'Þehir Manzaralý', 7900, 1200, 18),
(N'Dedeman Konya', N'Dedeman - Junior Suite', N'Þehir Manzaralý', 9800, 1500, 10),

(N'Divan Ankara', N'Divan Ankara - Superior Room', N'Þehir Manzaralý', 7200, 1000, 70),
(N'Divan Ankara', N'Divan Ankara - Executive Room', N'Þehir Manzaralý', 8500, 1200, 45),
(N'Divan Ankara', N'Divan Ankara - Deluxe Room', N'Þehir Manzaralý', 9400, 1300, 30),
(N'Divan Ankara', N'Divan Ankara - Junior Suite', N'Þehir Manzaralý', 13500, 1800, 12),
(N'Divan Ankara', N'Divan Ankara - Executive Suite', N'Þehir Manzaralý', 22000, 2500, 4),

(N'DoubleTree by Hilton Gaziantep', N'Gaziantep - Queen Guest Room', N'Þehir Manzaralý', 4300, 650, 45),
(N'DoubleTree by Hilton Gaziantep', N'Gaziantep - King Guest Room', N'Þehir Manzaralý', 4600, 650, 35),
(N'DoubleTree by Hilton Gaziantep', N'Gaziantep - King One Bedroom Suite', N'Þehir Manzaralý', 7600, 1100, 8),

(N'Hilton Bursa Convention Center', N'Bursa Hilton - Queen Guest Room', N'Þehir Manzaralý', 6800, 900, 65),
(N'Hilton Bursa Convention Center', N'Bursa Hilton - Twin Guest Room', N'Þehir Manzaralý', 7000, 900, 50),
(N'Hilton Bursa Convention Center', N'Bursa Hilton - Executive Room', N'Þehir Manzaralý', 9200, 1300, 28),

(N'Novotel Kayseri', N'Novotel - Modern Room', N'Dað Manzaralý', 3600, 500, 60),
(N'Novotel Kayseri', N'Novotel - Family Room', N'Dað Manzaralý', 4800, 700, 20),
(N'Novotel Kayseri', N'Novotel - Suite Room', N'Dað Manzaralý', 6200, 900, 8),

(N'Ramada Plaza Trabzon', N'Trabzon - Deluxe City View Room', N'Þehir Manzaralý', 6200, 900, 80),
(N'Ramada Plaza Trabzon', N'Trabzon - Deluxe Sea View Room', N'Deniz Manzaralý', 7200, 1000, 90),
(N'Ramada Plaza Trabzon', N'Trabzon - Corner Suite', N'Deniz Manzaralý', 9800, 1400, 25),
(N'Ramada Plaza Trabzon', N'Trabzon - Business Room', N'Deniz Manzaralý', 11200, 1600, 18),
(N'Ramada Plaza Trabzon', N'Trabzon - Family Connect Room', N'Deniz Manzaralý', 13500, 1900, 16),
(N'Ramada Plaza Trabzon', N'Trabzon - Suite', N'Deniz Manzaralý', 15500, 2200, 12),

(N'Ramada Resort Kusadasi', N'Kusadasi - Resort Room', N'Deniz Manzaralý', 8200, 1200, 90),
(N'Ramada Resort Kusadasi', N'Kusadasi - Family Room', N'Deniz Manzaralý', 11800, 1700, 35),
(N'Ramada Resort Kusadasi', N'Kusadasi - Suite Room', N'Deniz Manzaralý', 14500, 2100, 18),

(N'Swissotel The Bosphorus', N'Swissotel - Bosphorus View Room', N'Deniz Manzaralý', 16500, 2500, 120),
(N'Swissotel The Bosphorus', N'Swissotel - Residence Suite', N'Deniz Manzaralý', 28000, 4000, 45),
(N'Swissotel The Bosphorus', N'Swissotel - Family Residence', N'Deniz Manzaralý', 42000, 6000, 18),

(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Club Room', N'Þehir Manzaralý', 7800, 1100, 45),
(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Standard Sea View Room', N'Deniz Manzaralý', 8500, 1200, 80),
(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Standard Triple Room', N'Deniz Manzaralý', 9600, 1400, 30),
(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Standard Single Room', N'Deniz Manzaralý', 6900, 0, 25),

(N'Yazz Collective Fethiye', N'Yazz - Deluxe Room', N'Bahçe Manzaralý', 9900, 1200, 12),
(N'Yazz Collective Fethiye', N'Yazz - Grand Deluxe Room', N'Bahçe Manzaralý', 12900, 1500, 10),
(N'Yazz Collective Fethiye', N'Yazz - Executive Suite', N'Bahçe Manzaralý', 15900, 1900, 6),
(N'Yazz Collective Fethiye', N'Yazz - King Suite', N'Bahçe Manzaralý', 18900, 2300, 4),
(N'Yazz Collective Fethiye', N'Yazz - Bungalow', N'Bahçe Manzaralý', 14900, 1800, 8),
(N'Yazz Collective Fethiye', N'Yazz - Villa', N'Bahçe Manzaralý', 17500, 2200, 5),
(N'Yazz Collective Fethiye', N'Yazz - Connecting Room', N'Bahçe Manzaralý', 24500, 3000, 3)
) s(HotelName, RoomTypeName, ViewName, Price, Extra, TotalRooms)
JOIN Hotels h ON h.Name = s.HotelName
JOIN RoomTypes rt ON rt.Name = s.RoomTypeName
JOIN RoomViews rv ON rv.Name = s.ViewName;
GO

INSERT INTO HotelAmenities (HotelId, AmenityId)
SELECT DISTINCT h.Id, a.Id
FROM Hotels h
JOIN (VALUES
(N'Dedeman Konya', N'WiFi'), (N'Dedeman Konya', N'Havuz'), (N'Dedeman Konya', N'Kapalý Havuz'), (N'Dedeman Konya', N'Restoran'), (N'Dedeman Konya', N'Hamam'), (N'Dedeman Konya', N'Sauna'), (N'Dedeman Konya', N'Spa'), (N'Dedeman Konya', N'Fitness'), (N'Dedeman Konya', N'Toplantý Salonu'), (N'Dedeman Konya', N'Otopark'),
(N'Divan Ankara', N'WiFi'), (N'Divan Ankara', N'Restoran'), (N'Divan Ankara', N'Bar'), (N'Divan Ankara', N'Spa'), (N'Divan Ankara', N'Fitness'), (N'Divan Ankara', N'Toplantý Salonu'),
(N'DoubleTree by Hilton Gaziantep', N'Fitness'), (N'DoubleTree by Hilton Gaziantep', N'Konferans Salonu'), (N'DoubleTree by Hilton Gaziantep', N'Kuru Temizleme'), (N'DoubleTree by Hilton Gaziantep', N'Vale Servisi'), (N'DoubleTree by Hilton Gaziantep', N'Kütüphane'), (N'DoubleTree by Hilton Gaziantep', N'24 Saat Resepsiyon'),
(N'Hilton Bursa Convention Center', N'WiFi'), (N'Hilton Bursa Convention Center', N'Restoran'), (N'Hilton Bursa Convention Center', N'Spa'), (N'Hilton Bursa Convention Center', N'Kapalý Havuz'), (N'Hilton Bursa Convention Center', N'Sauna'), (N'Hilton Bursa Convention Center', N'Fitness'), (N'Hilton Bursa Convention Center', N'Toplantý Salonu'),
(N'Novotel Kayseri', N'WiFi'), (N'Novotel Kayseri', N'Restoran'), (N'Novotel Kayseri', N'Bar'), (N'Novotel Kayseri', N'Fitness'), (N'Novotel Kayseri', N'Teras'), (N'Novotel Kayseri', N'Konferans Salonu'), (N'Novotel Kayseri', N'Kahvaltý'), (N'Novotel Kayseri', N'Oda Servisi'),
(N'Ramada Plaza Trabzon', N'WiFi'), (N'Ramada Plaza Trabzon', N'Havuz'), (N'Ramada Plaza Trabzon', N'Özel Plaj'), (N'Ramada Plaza Trabzon', N'Denize Sýfýr'), (N'Ramada Plaza Trabzon', N'Spa'), (N'Ramada Plaza Trabzon', N'Fitness'), (N'Ramada Plaza Trabzon', N'Toplantý Salonu'), (N'Ramada Plaza Trabzon', N'Restoran'), (N'Ramada Plaza Trabzon', N'Bar'), (N'Ramada Plaza Trabzon', N'Evcil Hayvan Dostu'),
(N'Ramada Resort Kusadasi', N'WiFi'), (N'Ramada Resort Kusadasi', N'Havuz'), (N'Ramada Resort Kusadasi', N'Restoran'), (N'Ramada Resort Kusadasi', N'Bar'), (N'Ramada Resort Kusadasi', N'Spa'), (N'Ramada Resort Kusadasi', N'Fitness'), (N'Ramada Resort Kusadasi', N'Özel Plaj'),
(N'Swissotel The Bosphorus', N'WiFi'), (N'Swissotel The Bosphorus', N'Havuz'), (N'Swissotel The Bosphorus', N'Kapalý Havuz'), (N'Swissotel The Bosphorus', N'Spa'), (N'Swissotel The Bosphorus', N'Restoran'), (N'Swissotel The Bosphorus', N'Bar'), (N'Swissotel The Bosphorus', N'Fitness'), (N'Swissotel The Bosphorus', N'Tenis Kortu'), (N'Swissotel The Bosphorus', N'Hamam'), (N'Swissotel The Bosphorus', N'Sauna'), (N'Swissotel The Bosphorus', N'Jakuzi'),
(N'Wyndham Grand Izmir Ozdilek', N'WiFi'), (N'Wyndham Grand Izmir Ozdilek', N'Havuz'), (N'Wyndham Grand Izmir Ozdilek', N'Kapalý Havuz'), (N'Wyndham Grand Izmir Ozdilek', N'Termal'), (N'Wyndham Grand Izmir Ozdilek', N'Spa'), (N'Wyndham Grand Izmir Ozdilek', N'Sauna'), (N'Wyndham Grand Izmir Ozdilek', N'Hamam'), (N'Wyndham Grand Izmir Ozdilek', N'Jakuzi'), (N'Wyndham Grand Izmir Ozdilek', N'Fitness'), (N'Wyndham Grand Izmir Ozdilek', N'Toplantý Salonu'), (N'Wyndham Grand Izmir Ozdilek', N'Restoran'),
(N'Yazz Collective Fethiye', N'WiFi'), (N'Yazz Collective Fethiye', N'Restoran'), (N'Yazz Collective Fethiye', N'Bar'), (N'Yazz Collective Fethiye', N'Denize Sýfýr'), (N'Yazz Collective Fethiye', N'Özel Plaj'), (N'Yazz Collective Fethiye', N'Oda Servisi')
) v(HotelName, AmenityName)
ON v.HotelName = h.Name
JOIN Amenities a ON a.Name = v.AmenityName
WHERE NOT EXISTS (
    SELECT 1 FROM HotelAmenities x
    WHERE x.HotelId = h.Id AND x.AmenityId = a.Id
);
GO

DELETE ri
FROM RoomImages ri
JOIN HotelRooms hr ON ri.HotelRoomId = hr.Id
JOIN Hotels h ON h.Id = hr.HotelId
WHERE h.Name IN (
    N'Dedeman Konya',
    N'Divan Ankara',
    N'DoubleTree by Hilton Gaziantep',
    N'Hilton Bursa Convention Center',
    N'Novotel Kayseri',
    N'Ramada Plaza Trabzon',
    N'Ramada Resort Kusadasi',
    N'Swissotel The Bosphorus',
    N'Wyndham Grand Izmir Ozdilek',
    N'Yazz Collective Fethiye'
);
GO

INSERT INTO RoomImages (HotelRoomId, ImageUrl, IsMain, DisplayOrder)
SELECT hr.Id, v.Url, v.IsMain, v.DisplayOrder
FROM HotelRooms hr
JOIN Hotels h ON h.Id = hr.HotelId
JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
JOIN (VALUES
(N'Dedeman Konya', N'Dedeman - Superior Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041139167_RHN00141_1.jpg', 1, 0),
(N'Dedeman Konya', N'Dedeman - Superior Twin Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041136440_RHN00182_1.jpg', 1, 0),
(N'Dedeman Konya', N'Dedeman - Deluxe Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041129586_RHN00040_1.jpg', 1, 0),
(N'Dedeman Konya', N'Dedeman - Family Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041127085_RHN00112_1.jpg', 1, 0),
(N'Dedeman Konya', N'Dedeman - Junior Suite', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041135229_ECW07680_1.jpg', 1, 0),

(N'Divan Ankara', N'Divan Ankara - Superior Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9U3VwZXJpb3ItS2luZy1BMg_20220429142311.jpg', 1, 0),
(N'Divan Ankara', N'Divan Ankara - Executive Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9RXhlY3V0aXZlLVJvb20tS2luZy1BMQ_20220510155731.jpg', 1, 0),
(N'Divan Ankara', N'Divan Ankara - Deluxe Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9RGVsdXhlLVJvb20tQTI_20220429151127.jpg', 1, 0),
(N'Divan Ankara', N'Divan Ankara - Junior Suite', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9SnVuaW9yLVN1aXQtTGl2aW5nLVJvb20_20220429144326.jpg', 1, 0),
(N'Divan Ankara', N'Divan Ankara - Executive Suite', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9RXhlY3V0aXZlLVN1aXQtTGl2aW5nLVJvb20tQTE_20220510153643.jpg', 1, 0),

(N'DoubleTree by Hilton Gaziantep', N'Gaziantep - Queen Guest Room', N'https://www.hilton.com/im/en/GZTDTDI/7793837/gztdt-queen-guest-room1.jpg?ch=3000&cw=4500&gravity=NorthWest&impolicy=crop&rh=427&rw=640&xposition=0&yposition=0', 1, 0),
(N'DoubleTree by Hilton Gaziantep', N'Gaziantep - King Guest Room', N'https://www.hilton.com/im/en/GZTDTDI/7793861/king-guest-room.jpg?ch=2857&cw=4286&gravity=NorthWest&impolicy=crop&rh=427&rw=640&xposition=0&yposition=71', 1, 0),
(N'DoubleTree by Hilton Gaziantep', N'Gaziantep - King One Bedroom Suite', N'https://www.hilton.com/im/en/GZTDTDI/7793819/gztdt-one-bedroom-suite-bedroom.jpg?ch=2893&cw=4340&gravity=NorthWest&impolicy=crop&rh=427&rw=640&xposition=0&yposition=53', 1, 0),

(N'Hilton Bursa Convention Center', N'Bursa Hilton - Queen Guest Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Hilton_Bursa_Convention_Center_Spa_202601131324149_Queen_standard_room_a.jpg', 1, 0),
(N'Hilton Bursa Convention Center', N'Bursa Hilton - Twin Guest Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Hilton_Bursa_Convention_Center_Spa_202601131334229__M3A1123_1s.jpg', 1, 0),
(N'Hilton Bursa Convention Center', N'Bursa Hilton - Executive Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Hilton_Bursa_Convention_Center_Spa_202601131336268_King_Executive_room_1_.jpg', 1, 0),

(N'Novotel Kayseri', N'Novotel - Modern Room', N'https://imgcy.trivago.com/c_fill,d_dummy.jpeg,e_sharpen:60,f_auto,h_627,q_auto,w_1200/partner-images/aa/a5/f3cfbe1bff89c9869cfb3ba17ef5d39d34cdf88ebb49bb711a67e5d24fc7.jpeg', 1, 0),
(N'Novotel Kayseri', N'Novotel - Family Room', N'https://imgcy.trivago.com/c_fill,d_dummy.jpeg,e_sharpen:60,f_auto,h_627,q_auto,w_1200/partner-images/aa/a5/f3cfbe1bff89c9869cfb3ba17ef5d39d34cdf88ebb49bb711a67e5d24fc7.jpeg', 1, 0),
(N'Novotel Kayseri', N'Novotel - Suite Room', N'https://imgcy.trivago.com/c_fill,d_dummy.jpeg,e_sharpen:60,f_auto,h_627,q_auto,w_1200/partner-images/aa/a5/f3cfbe1bff89c9869cfb3ba17ef5d39d34cdf88ebb49bb711a67e5d24fc7.jpeg', 1, 0),

(N'Ramada Plaza Trabzon', N'Trabzon - Deluxe City View Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/z5zu46hligngue0fxajd.jpg', 1, 0),
(N'Ramada Plaza Trabzon', N'Trabzon - Deluxe Sea View Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Ramada_Plaza_by_Wyndham_Trabzon_202408231239159__DSC1457.jpg', 1, 0),
(N'Ramada Plaza Trabzon', N'Trabzon - Corner Suite', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/dmcbwgyjhhichxgsnhcj.jpg', 1, 0),
(N'Ramada Plaza Trabzon', N'Trabzon - Business Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/qmuple4pbojf7qgcbddk.jpg', 1, 0),
(N'Ramada Plaza Trabzon', N'Trabzon - Family Connect Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Ramada_Plaza_by_Wyndham_Trabzon_202408231241499__DSC1438.jpg', 1, 0),
(N'Ramada Plaza Trabzon', N'Trabzon - Suite', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Ramada_Plaza_by_Wyndham_Trabzon_202408231249112__DSC1460.jpg', 1, 0),

(N'Ramada Resort Kusadasi', N'Kusadasi - Resort Room', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/54018/l/Ramada-Resort-Kusadasi---Golf-Oda-147490.jpg', 1, 0),
(N'Ramada Resort Kusadasi', N'Kusadasi - Family Room', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/54018/l/Ramada-Resort-Kusadasi---Golf-Oda-147691.jpg', 1, 0),
(N'Ramada Resort Kusadasi', N'Kusadasi - Suite Room', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/54018/l/Ramada-Resort-Kusadasi---Golf-Oda-147691.jpg', 1, 0),

(N'Swissotel The Bosphorus', N'Swissotel - Bosphorus View Room', N'https://dq5r178u4t83b.cloudfront.net/wp-content/uploads/sites/149/2021/06/29113752/SWISS-ADVANTAGE-ROOM-BOSPHORUS-Copy-1024x683.jpg', 1, 0),
(N'Swissotel The Bosphorus', N'Swissotel - Residence Suite', N'https://bos.swissotel.com/assets/0/92/2119/2932/2971/2973/6442451685/13b6df09-a8af-4230-b8a9-720e3402a3e7.jpg', 1, 0),
(N'Swissotel The Bosphorus', N'Swissotel - Family Residence', N'https://dq5r178u4t83b.cloudfront.net/wp-content/uploads/sites/149/2025/10/20182623/HOP31221-1024x768.jpg', 1, 0),

(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Club Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9Q2x1Yi1PZGEtRnJlbmNoLUthcmE_20230824154904.jpg', 1, 0),
(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Standard Sea View Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9U3RhbmRhcmQtLSYtQ2x1Yi1Sb29tLUZyZW5jaC1TZWEtVmlldw_20211216164217.jpg', 1, 0),
(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Standard Triple Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9U3RhbmRhcmQtLSYtQ2x1Yi1Sb29tLUZyZW5jaC1TZWEtVmlldw_20211216164217.jpg', 1, 0),
(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Standard Single Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9Q2x1Yi1PZGEtRnJlbmNoLUthcmE_20230824154904.jpg', 1, 0),

(N'Yazz Collective Fethiye', N'Yazz - Deluxe Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202511271147168_WhatsApp_Image_2025_11_27_at_11_46_22.jpeg', 1, 0),
(N'Yazz Collective Fethiye', N'Yazz - Grand Deluxe Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202502181420092__LNS0152_HDR_min.jpg', 1, 0),
(N'Yazz Collective Fethiye', N'Yazz - Executive Suite', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202502181500164_yazz1x_2_11zon.jpg', 1, 0),
(N'Yazz Collective Fethiye', N'Yazz - King Suite', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202507261519074_Ekran_g_r_nt_s_2025_07_12_175011.png', 1, 0),
(N'Yazz Collective Fethiye', N'Yazz - Bungalow', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202507261511235_WhatsApp_Image_2025_07_12_at_17_17_26.jpeg', 1, 0),
(N'Yazz Collective Fethiye', N'Yazz - Villa', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202502181500155_yazza_8_1_11zon.jpg', 1, 0),
(N'Yazz Collective Fethiye', N'Yazz - Connecting Room', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202507261520537_WhatsApp_Image_2025_07_12_at_17_18_08.jpeg', 1, 0)
) v(HotelName, RoomTypeName, Url, IsMain, DisplayOrder)
ON v.HotelName = h.Name AND v.RoomTypeName = rt.Name;
GO

SELECT h.Name,
       h.City,
       COUNT(DISTINCT hr.Id) AS RoomCount,
       COUNT(DISTINCT hi.Id) AS HotelImageCount,
       COUNT(DISTINCT ha.AmenityId) AS AmenityCount
FROM Hotels h
LEFT JOIN HotelRooms hr ON hr.HotelId = h.Id
LEFT JOIN HotelImages hi ON hi.HotelId = h.Id
LEFT JOIN HotelAmenities ha ON ha.HotelId = h.Id
WHERE h.Name IN (
    N'Dedeman Konya',
    N'Divan Ankara',
    N'DoubleTree by Hilton Gaziantep',
    N'Hilton Bursa Convention Center',
    N'Novotel Kayseri',
    N'Ramada Plaza Trabzon',
    N'Ramada Resort Kusadasi',
    N'Swissotel The Bosphorus',
    N'Wyndham Grand Izmir Ozdilek',
    N'Yazz Collective Fethiye'
)
GROUP BY h.Name, h.City
ORDER BY h.Name;
GO