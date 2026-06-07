

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
    N'Swissotel Resort Bodrum Beach',
    N'Swissôtel Resort Bodrum Beach',
    N'Swissotel Büyük Efes Izmir',
    N'Swissôtel Büyük Efes Ýzmir',
    N'Key Hotel - Boutique Class',
    N'Key Hotel Ýzmir',
    N'DoubleTree by Hilton Çanakkale'
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

INSERT INTO RoomViews (Name, Icon, DisplayOrder, IsActive)
SELECT v.Name, v.Icon, v.DisplayOrder, 1
FROM (VALUES
(N'Bahçe Manzaralý', N'bi-flower2', 5),
(N'Deniz Manzaralý', N'bi-water', 1),
(N'Þehir Manzaralý', N'bi-buildings', 2),
(N'Teras', N'bi-door-open', 6)
) v(Name, Icon, DisplayOrder)
WHERE NOT EXISTS (SELECT 1 FROM RoomViews rv WHERE rv.Name = v.Name);
GO

INSERT INTO Amenities (Name, Icon)
SELECT v.Name, v.Icon
FROM (VALUES
(N'Kablosuz Ýnternet', N'wifi'),
(N'WiFi', N'wifi'),
(N'Otopark', N'car-front-fill'),
(N'Ücretsiz Otopark', N'car-front-fill'),
(N'Konsiyerj Hizmeti', N'bell'),
(N'Bahçe', N'flower2'),
(N'Teras', N'sun'),
(N'Kapalý Yüzme Havuzu', N'water'),
(N'Açýk Yüzme Havuzu', N'water'),
(N'Havuz', N'water'),
(N'Fitness', N'activity'),
(N'Spor Salonu', N'heart-pulse'),
(N'Ýskele', N'water'),
(N'Plaj Þemsiyesi', N'umbrella'),
(N'Plaj Havlusu', N'bag'),
(N'Çocuk Kulübü', N'emoji-smile'),
(N'Çocuk Havuzu', N'water'),
(N'Sauna', N'thermometer-half'),
(N'Buhar Odasý', N'cloud'),
(N'SPA Merkezi', N'flower1'),
(N'Hamam', N'droplet'),
(N'Bar', N'cup-straw'),
(N'Restoran', N'egg-fried'),
(N'Klima', N'snow'),
(N'Asansör', N'arrow-up-square'),
(N'Emanet Kasasý', N'safe'),
(N'Günlük Temizlik', N'sparkles'),
(N'Toplantý Salonu', N'briefcase'),
(N'24 Saat Resepsiyon', N'clock'),
(N'24 Saat Resepsiyon Hizmeti', N'clock'),
(N'Vale Servisi', N'car-front'),
(N'Masaj', N'heart-pulse')
) v(Name, Icon)
WHERE NOT EXISTS (SELECT 1 FROM Amenities a WHERE a.Name = v.Name);
GO

INSERT INTO RoomAmenities (Name, Icon)
SELECT v.Name, v.Icon
FROM (VALUES
(N'Oda Servisi', N'bag-heart'),
(N'Sigara Ýçilmez', N'slash-circle'),
(N'Minibar', N'cup-straw'),
(N'LCD / Plazma TV', N'tv'),
(N'LCD TV', N'tv'),
(N'Televizyon', N'tv'),
(N'Kablosuz Ýnternet', N'wifi'),
(N'WiFi', N'wifi'),
(N'Balkon veya Teras', N'door-open'),
(N'Bornoz', N'basket'),
(N'Bornoz ve Terlik', N'person'),
(N'Çalýþma Masasý', N'briefcase'),
(N'Duþ', N'droplet'),
(N'Kahve Makinesi', N'cup-hot'),
(N'Kahve / Çay Makinesi', N'cup-hot'),
(N'Çay / Kahve Seti', N'cup-hot'),
(N'Terlik', N'person-standing'),
(N'Saç Kurutma Makinesi', N'wind'),
(N'Su Isýtýcýsý', N'cup-hot'),
(N'Ücretsiz Þiþe Su', N'droplet'),
(N'Kasa', N'safe'),
(N'Klima', N'snow'),
(N'Buklet Banyo Ürünleri', N'basket'),
(N'Havlu', N'layers'),
(N'Makyaj Aynasý', N'mirror'),
(N'Sofa', N'chair'),
(N'Uyandýrma Servisi', N'alarm'),
(N'Telefon', N'telephone'),
(N'Uydu Yayýný', N'tv'),
(N'Elektronik Anahtar Sistemi', N'key'),
(N'Gardýrop', N'archive'),
(N'Duman Dedektörü', N'broadcast'),
(N'Günlük Temizlik', N'sparkles')
) v(Name, Icon)
WHERE NOT EXISTS (SELECT 1 FROM RoomAmenities ra WHERE ra.Name = v.Name);
GO

INSERT INTO RoomTypes (Name, Description, MaxGuests)
SELECT v.Name, v.Description, v.MaxGuests
FROM (VALUES
(N'Bodrum Swiss - Standart Bahçe 2 Pax', N'Swissôtel Resort Bodrum Beach standart oda, bahçe manzaralý, 40 m2, 2 kiþi', 2),
(N'Bodrum Swiss - Deniz Manzaralý 2 Pax', N'Swissôtel Resort Bodrum Beach çift kiþilik deniz manzaralý oda, 40 m2, 2 kiþi', 2),
(N'Bodrum Swiss - Standart Bahçe 3 Pax', N'Swissôtel Resort Bodrum Beach standart oda, bahçe manzaralý, 40 m2, 3 kiþi', 3),
(N'Bodrum Swiss - Deniz Manzaralý 3 Pax', N'Swissôtel Resort Bodrum Beach çift kiþilik deniz manzaralý oda, 40 m2, 3 kiþi', 3),

(N'Efes Swiss - Classic Room', N'Swissôtel Büyük Efes Ýzmir classic oda, 27 m2', 2),
(N'Efes Swiss - Swiss Advantage Room', N'Swissôtel Büyük Efes Ýzmir Swiss Advantage oda, 27 m2', 2),
(N'Efes Swiss - Executive Oda', N'Swissôtel Büyük Efes Ýzmir executive oda, 27 m2', 2),
(N'Efes Swiss - Suite', N'Swissôtel Büyük Efes Ýzmir business suite, 58 m2', 3),
(N'Efes Swiss - Executive Suite', N'Swissôtel Büyük Efes Ýzmir executive suite', 3),

(N'Key Hotel - Standard Oda', N'Key Hotel Ýzmir standart oda, 26 m2', 3),
(N'Key Hotel - Superior King Room', N'Key Hotel Ýzmir superior king room', 3),
(N'Key Hotel - Family Süit', N'Key Hotel Ýzmir 77 m2, 2 yatak odalý deniz manzaralý aile süiti', 4),
(N'Key Hotel - Executive Süit', N'Key Hotel Ýzmir 52 m2 teraslý executive süit', 3),

(N'Canakkale - King Deluxe Bed', N'DoubleTree by Hilton Çanakkale king deluxe oda', 2),
(N'Canakkale - Twin Guest Room', N'DoubleTree by Hilton Çanakkale twin guest room', 2),
(N'Canakkale - King Bedroom Suite', N'DoubleTree by Hilton Çanakkale king bedroom suite', 3),
(N'Canakkale - Accessible King Room', N'DoubleTree by Hilton Çanakkale eriþilebilir king oda', 2)
) v(Name, Description, MaxGuests)
WHERE NOT EXISTS (SELECT 1 FROM RoomTypes rt WHERE rt.Name = v.Name);
GO

INSERT INTO Hotels
(Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, AvgRating, StarRating, IsActive, CreatedAt, UpdatedAt)
VALUES
(N'Swissôtel Resort Bodrum Beach',
 N'Turgutreis mevkiinde denize sýfýr konumda yer alan Swissôtel Resort Bodrum Beach; özel plaj alaný, iskele, havuz, SPA alaný, fitness merkezi, çocuk kulübü, bahçe ve deniz manzaralý odalarýyla hizmet verir.',
 N'Turgutreis Mahallesi, Gazi Mustafa Kemal Bulvarý No:42, Turgutreis, Bodrum',
 N'Bodrum', N'Türkiye', N'+90 252 311 33 33', NULL, NULL, NULL, 8.5, 5, 1, GETDATE(), NULL),

(N'Swissôtel Büyük Efes Ýzmir',
 N'Ýzmir merkezinde yer alan Swissôtel Büyük Efes Ýzmir; bahçe, þehir ve deniz manzaralý odalarý, açýk-kapalý havuz, SPA merkezi, fitness, restoran, bar, toplantý salonlarý ve merkezi konumuyla öne çýkar.',
 N'Gaziosmanpaþa Bulvarý No:1, Alsancak, Ýzmir',
 N'Ýzmir', N'Türkiye', N'+90 232 414 00 00', N'izmir@swissotel.com', 38.42797542703062, 27.135758561492104, 8.4, 5, 1, GETDATE(), NULL),

(N'Key Hotel Ýzmir',
 N'Ýzmir þehir merkezinde, Konak Meydaný ve Kemeraltý Çarþýsý yakýnýnda yer alan lüks butik otel. Restoran, bar, spa, spor salonu, ücretsiz kablosuz internet ve ücretsiz otopark olanaklarý sunar.',
 N'Mimar Kemalettin Cad. No:1 Konak, Ýzmir, 35260',
 N'Ýzmir', N'Türkiye', N'+90 232 482 11 11', NULL, 38.423441, 27.131196, 9.6, 4, 1, GETDATE(), NULL),

(N'DoubleTree by Hilton Çanakkale',
 N'Çanakkale þehir merkezinde yer alan DoubleTree by Hilton Çanakkale; Troy Park AVM’ye yakýn konumu, açýk ve kapalý yüzme havuzu, SPA merkezi, toplantý salonu, vale servisi ve 24 saat resepsiyon hizmetiyle öne çýkar.',
 N'Çanakkale Þehir Merkezi, Çanakkale',
 N'Çanakkale', N'Türkiye', N'444 0 387', NULL, NULL, NULL, 8.9, 5, 1, GETDATE(), NULL);
GO

INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId)
SELECT h.Id, rt.Id, s.Price, s.Extra, s.TotalRooms, 1, rv.Id
FROM (VALUES
(N'Swissôtel Resort Bodrum Beach', N'Bodrum Swiss - Standart Bahçe 2 Pax', N'Bahçe Manzaralý', 14221.90, 2500.00, 24),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum Swiss - Deniz Manzaralý 2 Pax', N'Deniz Manzaralý', 17178.46, 2500.00, 20),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum Swiss - Standart Bahçe 3 Pax', N'Bahçe Manzaralý', 16409.27, 2500.00, 16),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum Swiss - Deniz Manzaralý 3 Pax', N'Deniz Manzaralý', 18000.00, 2500.00, 14),

(N'Swissôtel Büyük Efes Ýzmir', N'Efes Swiss - Classic Room', N'Bahçe Manzaralý', 7500.00, 1200.00, 90),
(N'Swissôtel Büyük Efes Ýzmir', N'Efes Swiss - Classic Room', N'Þehir Manzaralý', 8200.00, 1200.00, 90),
(N'Swissôtel Büyük Efes Ýzmir', N'Efes Swiss - Swiss Advantage Room', N'Bahçe Manzaralý', 9200.00, 1400.00, 70),
(N'Swissôtel Büyük Efes Ýzmir', N'Efes Swiss - Executive Oda', N'Bahçe Manzaralý', 11500.00, 1600.00, 50),
(N'Swissôtel Büyük Efes Ýzmir', N'Efes Swiss - Executive Oda', N'Þehir Manzaralý', 12500.00, 1600.00, 50),
(N'Swissôtel Büyük Efes Ýzmir', N'Efes Swiss - Executive Oda', N'Deniz Manzaralý', 14500.00, 1800.00, 28),
(N'Swissôtel Büyük Efes Ýzmir', N'Efes Swiss - Suite', N'Deniz Manzaralý', 22000.00, 2500.00, 14),
(N'Swissôtel Büyük Efes Ýzmir', N'Efes Swiss - Executive Suite', N'Deniz Manzaralý', 30000.00, 3000.00, 10),

(N'Key Hotel Ýzmir', N'Key Hotel - Standard Oda', N'Þehir Manzaralý', 8619.00, 1000.00, 8),
(N'Key Hotel Ýzmir', N'Key Hotel - Standard Oda', N'Deniz Manzaralý', 9800.00, 1200.00, 8),
(N'Key Hotel Ýzmir', N'Key Hotel - Superior King Room', N'Deniz Manzaralý', 12500.00, 1500.00, 6),
(N'Key Hotel Ýzmir', N'Key Hotel - Family Süit', N'Deniz Manzaralý', 18500.00, 2200.00, 5),
(N'Key Hotel Ýzmir', N'Key Hotel - Executive Süit', N'Teras', 21000.00, 2500.00, 4),

(N'DoubleTree by Hilton Çanakkale', N'Canakkale - King Deluxe Bed', N'Þehir Manzaralý', 6900.00, 900.00, 40),
(N'DoubleTree by Hilton Çanakkale', N'Canakkale - Twin Guest Room', N'Þehir Manzaralý', 7200.00, 900.00, 25),
(N'DoubleTree by Hilton Çanakkale', N'Canakkale - King Bedroom Suite', N'Deniz Manzaralý', 9900.00, 1500.00, 10),
(N'DoubleTree by Hilton Çanakkale', N'Canakkale - Accessible King Room', N'Þehir Manzaralý', 7600.00, 900.00, 9)
) s(HotelName, RoomTypeName, ViewName, Price, Extra, TotalRooms)
JOIN Hotels h ON h.Name = s.HotelName
JOIN RoomTypes rt ON rt.Name = s.RoomTypeName
JOIN RoomViews rv ON rv.Name = s.ViewName;
GO

INSERT INTO HotelAmenities (HotelId, AmenityId)
SELECT DISTINCT h.Id, a.Id
FROM Hotels h
JOIN (VALUES
(N'Swissôtel Resort Bodrum Beach', N'Kablosuz Ýnternet'), (N'Swissôtel Resort Bodrum Beach', N'Otopark'), (N'Swissôtel Resort Bodrum Beach', N'Konsiyerj Hizmeti'), (N'Swissôtel Resort Bodrum Beach', N'Bahçe'), (N'Swissôtel Resort Bodrum Beach', N'Teras'), (N'Swissôtel Resort Bodrum Beach', N'Kapalý Yüzme Havuzu'), (N'Swissôtel Resort Bodrum Beach', N'Fitness'), (N'Swissôtel Resort Bodrum Beach', N'Spor Salonu'), (N'Swissôtel Resort Bodrum Beach', N'Ýskele'), (N'Swissôtel Resort Bodrum Beach', N'Plaj Þemsiyesi'), (N'Swissôtel Resort Bodrum Beach', N'Plaj Havlusu'), (N'Swissôtel Resort Bodrum Beach', N'Çocuk Kulübü'), (N'Swissôtel Resort Bodrum Beach', N'Çocuk Havuzu'), (N'Swissôtel Resort Bodrum Beach', N'Sauna'), (N'Swissôtel Resort Bodrum Beach', N'Buhar Odasý'), (N'Swissôtel Resort Bodrum Beach', N'SPA Merkezi'), (N'Swissôtel Resort Bodrum Beach', N'Hamam'), (N'Swissôtel Resort Bodrum Beach', N'Bar'), (N'Swissôtel Resort Bodrum Beach', N'Restoran'),

(N'Swissôtel Büyük Efes Ýzmir', N'Kablosuz Ýnternet'), (N'Swissôtel Büyük Efes Ýzmir', N'24 Saat Resepsiyon Hizmeti'), (N'Swissôtel Büyük Efes Ýzmir', N'Bahçe'), (N'Swissôtel Büyük Efes Ýzmir', N'Klima'), (N'Swissôtel Büyük Efes Ýzmir', N'Asansör'), (N'Swissôtel Büyük Efes Ýzmir', N'Açýk Yüzme Havuzu'), (N'Swissôtel Büyük Efes Ýzmir', N'Havuz'), (N'Swissôtel Büyük Efes Ýzmir', N'Teras'), (N'Swissôtel Büyük Efes Ýzmir', N'Konsiyerj Hizmeti'), (N'Swissôtel Büyük Efes Ýzmir', N'Günlük Temizlik'), (N'Swissôtel Büyük Efes Ýzmir', N'Emanet Kasasý'), (N'Swissôtel Büyük Efes Ýzmir', N'Otopark'), (N'Swissôtel Büyük Efes Ýzmir', N'Toplantý Salonu'), (N'Swissôtel Büyük Efes Ýzmir', N'Fitness'), (N'Swissôtel Büyük Efes Ýzmir', N'Spor Salonu'), (N'Swissôtel Büyük Efes Ýzmir', N'Kapalý Yüzme Havuzu'), (N'Swissôtel Büyük Efes Ýzmir', N'Çocuk Havuzu'), (N'Swissôtel Büyük Efes Ýzmir', N'SPA Merkezi'), (N'Swissôtel Büyük Efes Ýzmir', N'Sauna'), (N'Swissôtel Büyük Efes Ýzmir', N'Hamam'), (N'Swissôtel Büyük Efes Ýzmir', N'Masaj'), (N'Swissôtel Büyük Efes Ýzmir', N'Restoran'), (N'Swissôtel Büyük Efes Ýzmir', N'Bar'),

(N'Key Hotel Ýzmir', N'SPA Merkezi'), (N'Key Hotel Ýzmir', N'Ücretsiz Otopark'), (N'Key Hotel Ýzmir', N'Otopark'), (N'Key Hotel Ýzmir', N'Kablosuz Ýnternet'), (N'Key Hotel Ýzmir', N'Restoran'), (N'Key Hotel Ýzmir', N'Klima'), (N'Key Hotel Ýzmir', N'Spor Salonu'), (N'Key Hotel Ýzmir', N'Fitness'), (N'Key Hotel Ýzmir', N'24 Saat Resepsiyon Hizmeti'), (N'Key Hotel Ýzmir', N'24 Saat Resepsiyon'), (N'Key Hotel Ýzmir', N'Toplantý Salonu'), (N'Key Hotel Ýzmir', N'Asansör'), (N'Key Hotel Ýzmir', N'Emanet Kasasý'), (N'Key Hotel Ýzmir', N'Günlük Temizlik'), (N'Key Hotel Ýzmir', N'Konsiyerj Hizmeti'), (N'Key Hotel Ýzmir', N'Bar'),

(N'DoubleTree by Hilton Çanakkale', N'Toplantý Salonu'), (N'DoubleTree by Hilton Çanakkale', N'24 Saat Resepsiyon'), (N'DoubleTree by Hilton Çanakkale', N'Açýk Yüzme Havuzu'), (N'DoubleTree by Hilton Çanakkale', N'Kapalý Yüzme Havuzu'), (N'DoubleTree by Hilton Çanakkale', N'Asansör'), (N'DoubleTree by Hilton Çanakkale', N'Vale Servisi'), (N'DoubleTree by Hilton Çanakkale', N'SPA Merkezi')
) v(HotelName, AmenityName)
ON v.HotelName = h.Name
JOIN Amenities a ON a.Name = v.AmenityName
WHERE NOT EXISTS (
    SELECT 1 FROM HotelAmenities x
    WHERE x.HotelId = h.Id AND x.AmenityId = a.Id
);
GO

INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId)
SELECT DISTINCT hr.Id, ra.Id
FROM HotelRooms hr
JOIN Hotels h ON h.Id = hr.HotelId
JOIN RoomAmenities ra ON ra.Name IN (
    N'Oda Servisi', N'Sigara Ýçilmez', N'Minibar', N'LCD / Plazma TV',
    N'LCD TV', N'Televizyon', N'Kablosuz Ýnternet', N'WiFi',
    N'Balkon veya Teras', N'Bornoz', N'Bornoz ve Terlik',
    N'Çalýþma Masasý', N'Duþ', N'Kahve Makinesi',
    N'Kahve / Çay Makinesi', N'Çay / Kahve Seti', N'Terlik',
    N'Saç Kurutma Makinesi', N'Su Isýtýcýsý', N'Ücretsiz Þiþe Su',
    N'Kasa', N'Klima', N'Buklet Banyo Ürünleri', N'Havlu',
    N'Makyaj Aynasý', N'Sofa', N'Uyandýrma Servisi', N'Telefon',
    N'Uydu Yayýný', N'Elektronik Anahtar Sistemi', N'Gardýrop',
    N'Duman Dedektörü', N'Günlük Temizlik'
)
WHERE h.Name IN (
    N'Swissôtel Resort Bodrum Beach',
    N'Swissôtel Büyük Efes Ýzmir',
    N'Key Hotel Ýzmir',
    N'DoubleTree by Hilton Çanakkale'
)
AND NOT EXISTS (
    SELECT 1 FROM HotelRoomAmenities x
    WHERE x.HotelRoomId = hr.Id AND x.RoomAmenityId = ra.Id
);
GO

INSERT INTO HotelImages (HotelId, ImageUrl, IsMain, DisplayOrder)
SELECT h.Id, v.Url, v.IsMain, v.DisplayOrder
FROM Hotels h
JOIN (VALUES
(N'Swissôtel Resort Bodrum Beach', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Swissotel_Resort_Bodrum_Beach_202407101517340_DJI_0514_Enhanced_NR_copy_2r.jpg', 1, 0),
(N'Swissôtel Resort Bodrum Beach', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/cskumsuadhptk7fjifrt.jpg', 0, 1),
(N'Swissôtel Resort Bodrum Beach', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Swissotel_Resort_Bodrum_Beach_202407101517375_DJI_0721r.jpg', 0, 2),
(N'Swissôtel Resort Bodrum Beach', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Swissotel_Resort_Bodrum_Beach_202407101517334_DJI_0479_copy.jpg', 0, 3),
(N'Swissôtel Resort Bodrum Beach', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Swissotel_Resort_Bodrum_Beach_202407101517346_DJI_0552_copy.jpg', 0, 4),

(N'Swissôtel Büyük Efes Ýzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/lvxjppbfda11udukq703.jpg', 1, 0),
(N'Swissôtel Büyük Efes Ýzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Swissotel_B_y_k_Efes_Izmir_202604131115527_Swissotel_Buyuk_Efes_Izmir_pool_2.jpg', 0, 1),
(N'Swissôtel Büyük Efes Ýzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/eiotkjibrkgldpfjsw6z.jpg', 0, 2),
(N'Swissôtel Büyük Efes Ýzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Swissotel_Buyuk_Efes_Izmir_20231102163034_0.jpg', 0, 3),
(N'Swissôtel Büyük Efes Ýzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/md17c5bosgjvozlaoovl.jpg', 0, 4),

(N'Key Hotel Ýzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/5247462b.jpg?impolicy=resizecrop&ra=fit&rw=598', 1, 0),
(N'Key Hotel Ýzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/141f1a30.jpg?impolicy=resizecrop&ra=fit&rw=297', 0, 1),
(N'Key Hotel Ýzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/1015cac4.jpg?impolicy=resizecrop&ra=fit&rw=297', 0, 2),
(N'Key Hotel Ýzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/21ff2106.jpg?impolicy=resizecrop&ra=fit&rw=297', 0, 3),
(N'Key Hotel Ýzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/5d58d22c.jpg?h=800&impolicy=fcrop&quality=medium&w=1200', 0, 4),

(N'DoubleTree by Hilton Çanakkale', N'https://i.travelapi.com/lodging/77000000/76520000/76510800/76510724/f0dae144_z.jpg', 1, 0),
(N'DoubleTree by Hilton Çanakkale', N'https://i.travelapi.com/lodging/77000000/76520000/76510800/76510724/ade3bda1_z.jpg', 0, 1),
(N'DoubleTree by Hilton Çanakkale', N'https://i.travelapi.com/lodging/77000000/76520000/76510800/76510724/899cab6b_z.jpg', 0, 2),
(N'DoubleTree by Hilton Çanakkale', N'https://i.travelapi.com/lodging/77000000/76520000/76510800/76510724/353685ae_z.jpg', 0, 3)
) v(HotelName, Url, IsMain, DisplayOrder)
ON v.HotelName = h.Name;
GO

INSERT INTO RoomImages (HotelRoomId, ImageUrl, IsMain, DisplayOrder)
SELECT hr.Id, v.Url, v.IsMain, v.DisplayOrder
FROM HotelRooms hr
JOIN Hotels h ON h.Id = hr.HotelId
JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
JOIN (VALUES
(N'Swissôtel Resort Bodrum Beach', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Swissotel_Resort_Bodrum_Beach_202407101517474_DSC_2285_copy.jpg', 1, 0),
(N'Swissôtel Resort Bodrum Beach', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/qotnm2edaq6jzvrdmchr.jpg', 0, 1),
(N'Swissôtel Resort Bodrum Beach', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Swissotel_Resort_Bodrum_Beach_202407101517478_DSC_2288_copy.jpg', 0, 2),

(N'Swissôtel Büyük Efes Ýzmir', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Swissotel_Buyuk_Efes_Izmir_20231102173211_1.jpg', 1, 0),
(N'Swissôtel Büyük Efes Ýzmir', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Swissotel_Buyuk_Efes_Izmir_20231102173312_1.jpg', 0, 1),
(N'Swissôtel Büyük Efes Ýzmir', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Swissotel_Buyuk_Efes_Izmir_20231102173311_0.jpg', 0, 2),

(N'Key Hotel Ýzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/14e05981.jpg?h=800&impolicy=fcrop&quality=medium&w=1200', 1, 0),
(N'Key Hotel Ýzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/841b62f0.jpg?h=800&impolicy=fcrop&quality=medium&w=1200', 0, 1),
(N'Key Hotel Ýzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/fc0d53b2.jpg?h=800&impolicy=fcrop&quality=medium&w=1200', 0, 2),

(N'DoubleTree by Hilton Çanakkale', N'https://i.travelapi.com/lodging/77000000/76520000/76510800/76510724/e65281af_z.jpg', 1, 0),
(N'DoubleTree by Hilton Çanakkale', N'https://i.travelapi.com/lodging/77000000/76520000/76510800/76510724/98c041ed_z.jpg', 0, 1),
(N'DoubleTree by Hilton Çanakkale', N'https://i.travelapi.com/lodging/77000000/76520000/76510800/76510724/a39b6c1b_z.jpg', 0, 2),
(N'DoubleTree by Hilton Çanakkale', N'https://i.travelapi.com/lodging/77000000/76520000/76510800/76510724/b02d240d_z.jpg', 0, 3)
) v(HotelName, Url, IsMain, DisplayOrder)
ON v.HotelName = h.Name;
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
    N'Swissôtel Resort Bodrum Beach',
    N'Swissôtel Büyük Efes Ýzmir',
    N'Key Hotel Ýzmir',
    N'DoubleTree by Hilton Çanakkale'
)
GROUP BY h.Name, h.City
ORDER BY h.Name;
GO