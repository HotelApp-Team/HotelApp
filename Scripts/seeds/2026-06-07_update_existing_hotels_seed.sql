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
(N'Toplantı Salonu', N'briefcase'),
(N'Konferans Salonu', N'briefcase'),
(N'Kapalı Havuz', N'water'),
(N'Otopark', N'p-square'),
(N'Vale Servisi', N'car-front'),
(N'Kuru Temizleme', N'washing-machine'),
(N'24 Saat Resepsiyon', N'clock'),
(N'Kütüphane', N'book'),
(N'Asansör', N'arrow-up-square'),
(N'Teras', N'sun'),
(N'Oda Servisi', N'bell'),
(N'Bagaj Odası', N'briefcase'),
(N'Kahvaltı', N'cup-hot'),
(N'Çocuk Parkı', N'emoji-smile'),
(N'Jakuzi', N'droplet-half'),
(N'Evcil Hayvan Dostu', N'heart'),
(N'Denize Sıfır', N'water'),
(N'Termal', N'thermometer'),
(N'Buhar Odası', N'cloud'),
(N'Tenis Kortu', N'trophy')
) v(Name, Icon)
WHERE NOT EXISTS (SELECT 1 FROM Amenities a WHERE a.Name = v.Name);
GO

INSERT INTO RoomAmenities (Name, Icon)
SELECT v.Name, v.Icon
FROM (VALUES
(N'Oda Servisi', N'bell'),
(N'Sigara İçilmez', N'slash-circle'),
(N'Balkon veya Teras', N'door-open'),
(N'Buklet Banyo Ürünleri', N'basket'),
(N'Havlu', N'layers'),
(N'Duş veya Küvet', N'droplet'),
(N'Merkezi Klima', N'snow'),
(N'Uyandırma Servisi', N'alarm'),
(N'Telefon', N'telephone'),
(N'Su Isıtıcısı', N'cup-hot'),
(N'Elektronik Anahtar Sistemi', N'key'),
(N'Gardırop', N'archive'),
(N'Ücretsiz Şişe Su', N'droplet'),
(N'Oturma Alanı', N'chair'),
(N'Kahve Makinesi', N'cup-hot'),
(N'Bornoz', N'shirt'),
(N'Terlik', N'footprints'),
(N'Yastık Menüsü', N'layers'),
(N'Executive Lounge Girişi', N'star'),
(N'Balkonda Jakuzi', N'droplet-half'),
(N'2 Yatak Odası', N'house'),
(N'Oturma Odası', N'chair'),
(N'Buzdolabı', N'box')
) v(Name, Icon)
WHERE NOT EXISTS (SELECT 1 FROM RoomAmenities ra WHERE ra.Name = v.Name);
GO

INSERT INTO RoomTypes (Name, Description, MaxGuests)
SELECT v.Name, v.Description, v.MaxGuests
FROM (VALUES
(N'Dedeman - Superior Room', N'Dedeman Konya superior oda, 32 m2', 2),
(N'Dedeman - Superior Twin Room', N'Dedeman Konya superior iki ayrı yataklı oda, 32 m2', 2),
(N'Dedeman - Deluxe Room', N'Dedeman Konya deluxe oda, 38 m2', 3),
(N'Dedeman - Family Room', N'Dedeman Konya aile odası, 57 m2', 4),
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
(N'Novotel - Suite Room', N'Novotel Kayseri ayrı oturma alanı bulunan suite oda', 3),

(N'Trabzon - Deluxe City View Room', N'Ramada Plaza Trabzon şehir manzaralı balkonlu deluxe oda, 27 m2', 2),
(N'Trabzon - Deluxe Sea View Room', N'Ramada Plaza Trabzon deniz manzaralı balkonlu deluxe oda, 27 m2', 2),
(N'Trabzon - Corner Suite', N'Ramada Plaza Trabzon corner suite, 37 m2', 3),
(N'Trabzon - Business Room', N'Ramada Plaza Trabzon business room, 46 m2', 3),
(N'Trabzon - Family Connect Room', N'Ramada Plaza Trabzon family connect room, 54 m2', 4),
(N'Trabzon - Suite', N'Ramada Plaza Trabzon suite, 54 m2', 4),

(N'Kusadasi - Resort Room', N'Ramada Resort Kuşadası balkon veya teraslı resort oda', 2),
(N'Kusadasi - Family Room', N'Ramada Resort Kuşadası aile konaklamasına uygun oda', 4),
(N'Kusadasi - Suite Room', N'Ramada Resort Kuşadası oturma alanlı suite oda', 4),

(N'Swissotel - Bosphorus View Room', N'Swissotel The Bosphorus Boğaz manzaralı oda', 2),
(N'Swissotel - Residence Suite', N'Swissotel Living rezidans suite', 4),
(N'Swissotel - Family Residence', N'Swissotel aileler için geniş rezidans dairesi', 5),

(N'Wyndham Izmir - Club Room', N'Wyndham Grand İzmir Özdilek kara manzaralı club room, 27 m2', 2),
(N'Wyndham Izmir - Standard Sea View Room', N'Wyndham Grand İzmir Özdilek deniz manzaralı standart oda, 27 m2', 3),
(N'Wyndham Izmir - Standard Triple Room', N'Wyndham Grand İzmir Özdilek standart üç kişilik oda, 27 m2', 3),
(N'Wyndham Izmir - Standard Single Room', N'Wyndham Grand İzmir Özdilek standart single oda, 27 m2', 1),

(N'Yazz - Deluxe Room', N'Yazz Collective bahçe manzaralı teraslı deluxe oda, 36 m2', 2),
(N'Yazz - Grand Deluxe Room', N'Yazz Collective bahçe manzaralı teraslı grand deluxe oda, 50 m2', 2),
(N'Yazz - Executive Suite', N'Yazz Collective executive suite, 55 m2', 3),
(N'Yazz - King Suite', N'Yazz Collective king suite, 63 m2', 3),
(N'Yazz - Bungalow', N'Yazz Collective bungalov, 45 m2', 3),
(N'Yazz - Villa', N'Yazz Collective villa, 45 m2', 3),
(N'Yazz - Connecting Room', N'Yazz Collective bağlantılı oda, 90 m2', 4)
) v(Name, Description, MaxGuests)
WHERE NOT EXISTS (SELECT 1 FROM RoomTypes rt WHERE rt.Name = v.Name);
GO

INSERT INTO Hotels
(Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, AvgRating, StarRating, IsActive, CreatedAt, UpdatedAt)
VALUES
(N'Dedeman Konya', N'Selçuklu’da yer alan Dedeman Konya; 206 oda, 12 toplantı salonu, 2 restoran, On8 Lounge Restaurant, Türk hamamı, fitness merkezi, 2 yüzme havuzu, squash ve SPA olanaklarıyla iş ve turizm odaklı konaklama sunar.', N'Selçuklu, Konya Şehir Merkezi, Konya', N'Konya', N'Türkiye', N'+90 332 221 66 00', NULL, NULL, NULL, 8.9, 5, 1, GETDATE(), NULL),

(N'Divan Ankara', N'Ankara Çankaya’da hizmet veren Divan Ankara; restoran, bar, 3000 m2 SPA alanı, 12 toplantı salonundan oluşan 1500 m2 etkinlik merkezi ve iş-kongre odaklı konaklama olanakları sunar.', N'Ankara Çankaya, Ankara Merkez, Ankara', N'Ankara', N'Türkiye', N'+90 312 409 30 00', NULL, NULL, NULL, 9.3, 5, 1, GETDATE(), NULL),

(N'DoubleTree by Hilton Gaziantep', N'Gaziantep şehir merkezinde yer alan DoubleTree by Hilton Gaziantep; konferans alanı, hızlı giriş, kuru temizleme, ücretsiz vale otopark, kütüphane ve fitness olanaklarıyla öne çıkar.', N'Gaziantep Şehir Merkezi, Gaziantep', N'Gaziantep', N'Türkiye', N'+90 342 999 41 11', N'reservation.gaziantep@hilton.com', NULL, NULL, 7.2, 4, 1, GETDATE(), NULL),

(N'Hilton Bursa Convention Center', N'Bursa Osmangazi’deki Hilton Bursa Convention Center And Spa; merkezi konumu, kapalı havuzu, saunası, tam donanımlı fitness merkezi ve restoranıyla iş ve turistik seyahatlere hitap eder.', N'Bursa Merkez - Osmangazi, Bursa', N'Bursa', N'Türkiye', N'+90 224 500 05 05', N'bursa.sales@hilton.com', NULL, NULL, 9.2, 5, 1, GETDATE(), NULL),

(N'Novotel Kayseri', N'Yenipervane Mahallesi Kocasinan Bulvarı üzerindeki Novotel Kayseri; zengin açık büfe kahvaltı, Erciyes Dağı manzaralı odalar, bahçe, teras, aile dostu odalar, restoran, bar, fitness ve merkezi konumuyla öne çıkar.', N'Yenipervane Mah, Kocasinan Blv. 161-1, 38110 Kayseri, Türkiye', N'Kayseri', N'Türkiye', N'+90 352 207 30 00', N'info@novotelkayseri.com', NULL, NULL, 8.7, 4, 1, GETDATE(), NULL),

(N'Ramada Plaza Trabzon', N'Ortahisar Trabzon’da denize sıfır konumda yer alan Ramada Plaza; 350 odası, deniz ve dağ manzarası, geniş kumsalı, yetişkin ve çocuk havuzları, SPA, fitness, toplantı salonları ve restoranlarıyla hizmet verir.', N'Ortahisar, Trabzon', N'Trabzon', N'Türkiye', N'+90 462 335 30 30', N'info@ramadaplazatrabzon.com', NULL, NULL, 8.8, 5, 1, GETDATE(), NULL),

(N'Ramada Resort Kusadasi', N'Kuşadası Aydın’da yer alan Ramada Resort By Wyndham Kuşadası & Golf; plajlara, Kuşadası Kalesi’ne ve ulaşım noktalarına yakın konumu, balkon/teraslı odaları ve resort olanaklarıyla öne çıkar.', N'Türkmen Mahallesi Selçuk Bulvarı No:9/3 Kuşadası, Aydın', N'Aydın', N'Türkiye', N'+90 256 420 07 00', NULL, NULL, NULL, NULL, 5, 1, GETDATE(), NULL),

(N'Swissotel The Bosphorus', N'İstanbul Vişnezade’de yer alan Swissôtel The Bosphorus; Boğaz manzarası, 16 ROOF, Swissotel Living rezidansları, Purovel Spa & Sport, kapalı-açık havuz, fitness, tenis kortları ve merkezi konumuyla öne çıkar.', N'Vişnezade, Acısu Sk. No:19, 34357 İstanbul', N'İstanbul', N'Türkiye', N'+90 212 326 11 00', N'istanbul@swissotel.com', NULL, NULL, 9.4, 5, 1, GETDATE(), NULL),

(N'Wyndham Grand Izmir Ozdilek', N'Balçova İzmir’de yer alan Wyndham Grand İzmir Özdilek Thermal & Spa; termal wellness merkezi, açık-kapalı havuz, termal havuz, jakuzi, sauna, Türk hamamı, buhar odaları, fitness ve 10 salonuyla hizmet verir.', N'Balçova, İzmir', N'İzmir', N'Türkiye', N'+90 232 292 13 00', NULL, NULL, NULL, 9.4, 5, 1, GETDATE(), NULL),

(N'Yazz Collective Fethiye', N'Fethiye Muğla’da doğayla iç içe, denize sıfır konumda yer alan Yazz Collective; modern tasarımı, bahçe manzaralı teraslı odaları, bungalovları, villaları ve lüks kaçış atmosferiyle öne çıkar.', N'Fethiye, Muğla', N'Fethiye', N'Türkiye', N'+90 530 972 92 99', N'reservation@yazzcollective.com', NULL, NULL, 9.1, 5, 1, GETDATE(), NULL);
GO

INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId)
SELECT h.Id, rt.Id, s.Price, s.Extra, s.TotalRooms, 1, rv.Id
FROM (VALUES
(N'Dedeman Konya', N'Dedeman - Superior Room', N'Şehir Manzaralı', 5200, 700, 72),
(N'Dedeman Konya', N'Dedeman - Superior Twin Room', N'Şehir Manzaralı', 5400, 700, 60),
(N'Dedeman Konya', N'Dedeman - Deluxe Room', N'Şehir Manzaralı', 6500, 900, 36),
(N'Dedeman Konya', N'Dedeman - Family Room', N'Şehir Manzaralı', 7900, 1200, 18),
(N'Dedeman Konya', N'Dedeman - Junior Suite', N'Şehir Manzaralı', 9800, 1500, 10),

(N'Divan Ankara', N'Divan Ankara - Superior Room', N'Şehir Manzaralı', 7200, 1000, 70),
(N'Divan Ankara', N'Divan Ankara - Executive Room', N'Şehir Manzaralı', 8500, 1200, 45),
(N'Divan Ankara', N'Divan Ankara - Deluxe Room', N'Şehir Manzaralı', 9400, 1300, 30),
(N'Divan Ankara', N'Divan Ankara - Junior Suite', N'Şehir Manzaralı', 13500, 1800, 12),
(N'Divan Ankara', N'Divan Ankara - Executive Suite', N'Şehir Manzaralı', 22000, 2500, 4),

(N'DoubleTree by Hilton Gaziantep', N'Gaziantep - Queen Guest Room', N'Şehir Manzaralı', 4300, 650, 45),
(N'DoubleTree by Hilton Gaziantep', N'Gaziantep - King Guest Room', N'Şehir Manzaralı', 4600, 650, 35),
(N'DoubleTree by Hilton Gaziantep', N'Gaziantep - King One Bedroom Suite', N'Şehir Manzaralı', 7600, 1100, 8),

(N'Hilton Bursa Convention Center', N'Bursa Hilton - Queen Guest Room', N'Şehir Manzaralı', 6800, 900, 65),
(N'Hilton Bursa Convention Center', N'Bursa Hilton - Twin Guest Room', N'Şehir Manzaralı', 7000, 900, 50),
(N'Hilton Bursa Convention Center', N'Bursa Hilton - Executive Room', N'Şehir Manzaralı', 9200, 1300, 28),

(N'Novotel Kayseri', N'Novotel - Modern Room', N'Dağ Manzaralı', 3600, 500, 60),
(N'Novotel Kayseri', N'Novotel - Family Room', N'Dağ Manzaralı', 4800, 700, 20),
(N'Novotel Kayseri', N'Novotel - Suite Room', N'Dağ Manzaralı', 6200, 900, 8),

(N'Ramada Plaza Trabzon', N'Trabzon - Deluxe City View Room', N'Şehir Manzaralı', 6200, 900, 80),
(N'Ramada Plaza Trabzon', N'Trabzon - Deluxe Sea View Room', N'Deniz Manzaralı', 7200, 1000, 90),
(N'Ramada Plaza Trabzon', N'Trabzon - Corner Suite', N'Deniz Manzaralı', 9800, 1400, 25),
(N'Ramada Plaza Trabzon', N'Trabzon - Business Room', N'Deniz Manzaralı', 11200, 1600, 18),
(N'Ramada Plaza Trabzon', N'Trabzon - Family Connect Room', N'Deniz Manzaralı', 13500, 1900, 16),
(N'Ramada Plaza Trabzon', N'Trabzon - Suite', N'Deniz Manzaralı', 15500, 2200, 12),

(N'Ramada Resort Kusadasi', N'Kusadasi - Resort Room', N'Deniz Manzaralı', 8200, 1200, 90),
(N'Ramada Resort Kusadasi', N'Kusadasi - Family Room', N'Deniz Manzaralı', 11800, 1700, 35),
(N'Ramada Resort Kusadasi', N'Kusadasi - Suite Room', N'Deniz Manzaralı', 14500, 2100, 18),

(N'Swissotel The Bosphorus', N'Swissotel - Bosphorus View Room', N'Deniz Manzaralı', 16500, 2500, 120),
(N'Swissotel The Bosphorus', N'Swissotel - Residence Suite', N'Deniz Manzaralı', 28000, 4000, 45),
(N'Swissotel The Bosphorus', N'Swissotel - Family Residence', N'Deniz Manzaralı', 42000, 6000, 18),

(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Club Room', N'Şehir Manzaralı', 7800, 1100, 45),
(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Standard Sea View Room', N'Deniz Manzaralı', 8500, 1200, 80),
(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Standard Triple Room', N'Deniz Manzaralı', 9600, 1400, 30),
(N'Wyndham Grand Izmir Ozdilek', N'Wyndham Izmir - Standard Single Room', N'Deniz Manzaralı', 6900, 0, 25),

(N'Yazz Collective Fethiye', N'Yazz - Deluxe Room', N'Bahçe Manzaralı', 9900, 1200, 12),
(N'Yazz Collective Fethiye', N'Yazz - Grand Deluxe Room', N'Bahçe Manzaralı', 12900, 1500, 10),
(N'Yazz Collective Fethiye', N'Yazz - Executive Suite', N'Bahçe Manzaralı', 15900, 1900, 6),
(N'Yazz Collective Fethiye', N'Yazz - King Suite', N'Bahçe Manzaralı', 18900, 2300, 4),
(N'Yazz Collective Fethiye', N'Yazz - Bungalow', N'Bahçe Manzaralı', 14900, 1800, 8),
(N'Yazz Collective Fethiye', N'Yazz - Villa', N'Bahçe Manzaralı', 17500, 2200, 5),
(N'Yazz Collective Fethiye', N'Yazz - Connecting Room', N'Bahçe Manzaralı', 24500, 3000, 3)
) s(HotelName, RoomTypeName, ViewName, Price, Extra, TotalRooms)
JOIN Hotels h ON h.Name = s.HotelName
JOIN RoomTypes rt ON rt.Name = s.RoomTypeName
JOIN RoomViews rv ON rv.Name = s.ViewName;
GO

INSERT INTO HotelAmenities (HotelId, AmenityId)
SELECT DISTINCT h.Id, a.Id
FROM Hotels h
JOIN (VALUES
(N'Dedeman Konya', N'WiFi'), (N'Dedeman Konya', N'Havuz'), (N'Dedeman Konya', N'Kapalı Havuz'), (N'Dedeman Konya', N'Restoran'), (N'Dedeman Konya', N'Hamam'), (N'Dedeman Konya', N'Sauna'), (N'Dedeman Konya', N'Spa'), (N'Dedeman Konya', N'Fitness'), (N'Dedeman Konya', N'Toplantı Salonu'), (N'Dedeman Konya', N'Otopark'),
(N'Divan Ankara', N'WiFi'), (N'Divan Ankara', N'Restoran'), (N'Divan Ankara', N'Bar'), (N'Divan Ankara', N'Spa'), (N'Divan Ankara', N'Fitness'), (N'Divan Ankara', N'Toplantı Salonu'),
(N'DoubleTree by Hilton Gaziantep', N'Fitness'), (N'DoubleTree by Hilton Gaziantep', N'Konferans Salonu'), (N'DoubleTree by Hilton Gaziantep', N'Kuru Temizleme'), (N'DoubleTree by Hilton Gaziantep', N'Vale Servisi'), (N'DoubleTree by Hilton Gaziantep', N'Kütüphane'), (N'DoubleTree by Hilton Gaziantep', N'24 Saat Resepsiyon'),
(N'Hilton Bursa Convention Center', N'WiFi'), (N'Hilton Bursa Convention Center', N'Restoran'), (N'Hilton Bursa Convention Center', N'Spa'), (N'Hilton Bursa Convention Center', N'Kapalı Havuz'), (N'Hilton Bursa Convention Center', N'Sauna'), (N'Hilton Bursa Convention Center', N'Fitness'), (N'Hilton Bursa Convention Center', N'Toplantı Salonu'),
(N'Novotel Kayseri', N'WiFi'), (N'Novotel Kayseri', N'Restoran'), (N'Novotel Kayseri', N'Bar'), (N'Novotel Kayseri', N'Fitness'), (N'Novotel Kayseri', N'Teras'), (N'Novotel Kayseri', N'Konferans Salonu'), (N'Novotel Kayseri', N'Kahvaltı'), (N'Novotel Kayseri', N'Oda Servisi'),
(N'Ramada Plaza Trabzon', N'WiFi'), (N'Ramada Plaza Trabzon', N'Havuz'), (N'Ramada Plaza Trabzon', N'Özel Plaj'), (N'Ramada Plaza Trabzon', N'Denize Sıfır'), (N'Ramada Plaza Trabzon', N'Spa'), (N'Ramada Plaza Trabzon', N'Fitness'), (N'Ramada Plaza Trabzon', N'Toplantı Salonu'), (N'Ramada Plaza Trabzon', N'Restoran'), (N'Ramada Plaza Trabzon', N'Bar'), (N'Ramada Plaza Trabzon', N'Evcil Hayvan Dostu'),
(N'Ramada Resort Kusadasi', N'WiFi'), (N'Ramada Resort Kusadasi', N'Havuz'), (N'Ramada Resort Kusadasi', N'Restoran'), (N'Ramada Resort Kusadasi', N'Bar'), (N'Ramada Resort Kusadasi', N'Spa'), (N'Ramada Resort Kusadasi', N'Fitness'), (N'Ramada Resort Kusadasi', N'Özel Plaj'),
(N'Swissotel The Bosphorus', N'WiFi'), (N'Swissotel The Bosphorus', N'Havuz'), (N'Swissotel The Bosphorus', N'Kapalı Havuz'), (N'Swissotel The Bosphorus', N'Spa'), (N'Swissotel The Bosphorus', N'Restoran'), (N'Swissotel The Bosphorus', N'Bar'), (N'Swissotel The Bosphorus', N'Fitness'), (N'Swissotel The Bosphorus', N'Tenis Kortu'), (N'Swissotel The Bosphorus', N'Hamam'), (N'Swissotel The Bosphorus', N'Sauna'), (N'Swissotel The Bosphorus', N'Jakuzi'),
(N'Wyndham Grand Izmir Ozdilek', N'WiFi'), (N'Wyndham Grand Izmir Ozdilek', N'Havuz'), (N'Wyndham Grand Izmir Ozdilek', N'Kapalı Havuz'), (N'Wyndham Grand Izmir Ozdilek', N'Termal'), (N'Wyndham Grand Izmir Ozdilek', N'Spa'), (N'Wyndham Grand Izmir Ozdilek', N'Sauna'), (N'Wyndham Grand Izmir Ozdilek', N'Hamam'), (N'Wyndham Grand Izmir Ozdilek', N'Jakuzi'), (N'Wyndham Grand Izmir Ozdilek', N'Fitness'), (N'Wyndham Grand Izmir Ozdilek', N'Toplantı Salonu'), (N'Wyndham Grand Izmir Ozdilek', N'Restoran'),
(N'Yazz Collective Fethiye', N'WiFi'), (N'Yazz Collective Fethiye', N'Restoran'), (N'Yazz Collective Fethiye', N'Bar'), (N'Yazz Collective Fethiye', N'Denize Sıfır'), (N'Yazz Collective Fethiye', N'Özel Plaj'), (N'Yazz Collective Fethiye', N'Oda Servisi')
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