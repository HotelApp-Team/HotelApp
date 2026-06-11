

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
    N'Swissôtel Büyük Efes İzmir',
    N'Key Hotel - Boutique Class',
    N'Key Hotel İzmir',
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
(N'Bahçe Manzaralı', N'bi-flower2', 5),
(N'Deniz Manzaralı', N'bi-water', 1),
(N'Şehir Manzaralı', N'bi-buildings', 2),
(N'Teras', N'bi-door-open', 6)
) v(Name, Icon, DisplayOrder)
WHERE NOT EXISTS (SELECT 1 FROM RoomViews rv WHERE rv.Name = v.Name);
GO

INSERT INTO Amenities (Name, Icon)
SELECT v.Name, v.Icon
FROM (VALUES
(N'Kablosuz İnternet', N'wifi'),
(N'WiFi', N'wifi'),
(N'Otopark', N'car-front-fill'),
(N'Ücretsiz Otopark', N'car-front-fill'),
(N'Konsiyerj Hizmeti', N'bell'),
(N'Bahçe', N'flower2'),
(N'Teras', N'sun'),
(N'Kapalı Yüzme Havuzu', N'water'),
(N'Açık Yüzme Havuzu', N'water'),
(N'Havuz', N'water'),
(N'Fitness', N'activity'),
(N'Spor Salonu', N'heart-pulse'),
(N'İskele', N'water'),
(N'Plaj Şemsiyesi', N'umbrella'),
(N'Plaj Havlusu', N'bag'),
(N'Çocuk Kulübü', N'emoji-smile'),
(N'Çocuk Havuzu', N'water'),
(N'Sauna', N'thermometer-half'),
(N'Buhar Odası', N'cloud'),
(N'SPA Merkezi', N'flower1'),
(N'Hamam', N'droplet'),
(N'Bar', N'cup-straw'),
(N'Restoran', N'egg-fried'),
(N'Klima', N'snow'),
(N'Asansör', N'arrow-up-square'),
(N'Emanet Kasası', N'safe'),
(N'Günlük Temizlik', N'sparkles'),
(N'Toplantı Salonu', N'briefcase'),
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
(N'Sigara İçilmez', N'slash-circle'),
(N'Minibar', N'cup-straw'),
(N'LCD / Plazma TV', N'tv'),
(N'LCD TV', N'tv'),
(N'Televizyon', N'tv'),
(N'Kablosuz İnternet', N'wifi'),
(N'WiFi', N'wifi'),
(N'Balkon veya Teras', N'door-open'),
(N'Bornoz', N'basket'),
(N'Bornoz ve Terlik', N'person'),
(N'Çalışma Masası', N'briefcase'),
(N'Duş', N'droplet'),
(N'Kahve Makinesi', N'cup-hot'),
(N'Kahve / Çay Makinesi', N'cup-hot'),
(N'Çay / Kahve Seti', N'cup-hot'),
(N'Terlik', N'person-standing'),
(N'Saç Kurutma Makinesi', N'wind'),
(N'Su Isıtıcısı', N'cup-hot'),
(N'Ücretsiz Şişe Su', N'droplet'),
(N'Kasa', N'safe'),
(N'Klima', N'snow'),
(N'Buklet Banyo Ürünleri', N'basket'),
(N'Havlu', N'layers'),
(N'Makyaj Aynası', N'mirror'),
(N'Sofa', N'chair'),
(N'Uyandırma Servisi', N'alarm'),
(N'Telefon', N'telephone'),
(N'Uydu Yayını', N'tv'),
(N'Elektronik Anahtar Sistemi', N'key'),
(N'Gardırop', N'archive'),
(N'Duman Dedektörü', N'broadcast'),
(N'Günlük Temizlik', N'sparkles')
) v(Name, Icon)
WHERE NOT EXISTS (SELECT 1 FROM RoomAmenities ra WHERE ra.Name = v.Name);
GO

INSERT INTO RoomTypes (Name, Description, MaxGuests)
SELECT v.Name, v.Description, v.MaxGuests
FROM (VALUES
(N'Bodrum Swiss - Standart Bahçe 2 Pax', N'Swissôtel Resort Bodrum Beach standart oda, bahçe manzaralı, 40 m2, 2 kişi', 2),
(N'Bodrum Swiss - Deniz Manzaralı 2 Pax', N'Swissôtel Resort Bodrum Beach çift kişilik deniz manzaralı oda, 40 m2, 2 kişi', 2),
(N'Bodrum Swiss - Standart Bahçe 3 Pax', N'Swissôtel Resort Bodrum Beach standart oda, bahçe manzaralı, 40 m2, 3 kişi', 3),
(N'Bodrum Swiss - Deniz Manzaralı 3 Pax', N'Swissôtel Resort Bodrum Beach çift kişilik deniz manzaralı oda, 40 m2, 3 kişi', 3),

(N'Efes Swiss - Classic Room', N'Swissôtel Büyük Efes İzmir classic oda, 27 m2', 2),
(N'Efes Swiss - Swiss Advantage Room', N'Swissôtel Büyük Efes İzmir Swiss Advantage oda, 27 m2', 2),
(N'Efes Swiss - Executive Oda', N'Swissôtel Büyük Efes İzmir executive oda, 27 m2', 2),
(N'Efes Swiss - Suite', N'Swissôtel Büyük Efes İzmir business suite, 58 m2', 3),
(N'Efes Swiss - Executive Suite', N'Swissôtel Büyük Efes İzmir executive suite', 3),

(N'Key Hotel - Standard Oda', N'Key Hotel İzmir standart oda, 26 m2', 3),
(N'Key Hotel - Superior King Room', N'Key Hotel İzmir superior king room', 3),
(N'Key Hotel - Family Süit', N'Key Hotel İzmir 77 m2, 2 yatak odalı deniz manzaralı aile süiti', 4),
(N'Key Hotel - Executive Süit', N'Key Hotel İzmir 52 m2 teraslı executive süit', 3),

(N'Canakkale - King Deluxe Bed', N'DoubleTree by Hilton Çanakkale king deluxe oda', 2),
(N'Canakkale - Twin Guest Room', N'DoubleTree by Hilton Çanakkale twin guest room', 2),
(N'Canakkale - King Bedroom Suite', N'DoubleTree by Hilton Çanakkale king bedroom suite', 3),
(N'Canakkale - Accessible King Room', N'DoubleTree by Hilton Çanakkale erişilebilir king oda', 2)
) v(Name, Description, MaxGuests)
WHERE NOT EXISTS (SELECT 1 FROM RoomTypes rt WHERE rt.Name = v.Name);
GO

INSERT INTO Hotels
(Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, AvgRating, StarRating, IsActive, CreatedAt, UpdatedAt)
VALUES
(N'Swissôtel Resort Bodrum Beach',
 N'Turgutreis mevkiinde denize sıfır konumda yer alan Swissôtel Resort Bodrum Beach; özel plaj alanı, iskele, havuz, SPA alanı, fitness merkezi, çocuk kulübü, bahçe ve deniz manzaralı odalarıyla hizmet verir.',
 N'Turgutreis Mahallesi, Gazi Mustafa Kemal Bulvarı No:42, Turgutreis, Bodrum',
 N'Bodrum', N'Türkiye', N'+90 252 311 33 33', NULL, NULL, NULL, 8.5, 5, 1, GETDATE(), NULL),

(N'Swissôtel Büyük Efes İzmir',
 N'İzmir merkezinde yer alan Swissôtel Büyük Efes İzmir; bahçe, şehir ve deniz manzaralı odaları, açık-kapalı havuz, SPA merkezi, fitness, restoran, bar, toplantı salonları ve merkezi konumuyla öne çıkar.',
 N'Gaziosmanpaşa Bulvarı No:1, Alsancak, İzmir',
 N'İzmir', N'Türkiye', N'+90 232 414 00 00', N'izmir@swissotel.com', 38.42797542703062, 27.135758561492104, 8.4, 5, 1, GETDATE(), NULL),

(N'Key Hotel İzmir',
 N'İzmir şehir merkezinde, Konak Meydanı ve Kemeraltı Çarşısı yakınında yer alan lüks butik otel. Restoran, bar, spa, spor salonu, ücretsiz kablosuz internet ve ücretsiz otopark olanakları sunar.',
 N'Mimar Kemalettin Cad. No:1 Konak, İzmir, 35260',
 N'İzmir', N'Türkiye', N'+90 232 482 11 11', NULL, 38.423441, 27.131196, 9.6, 4, 1, GETDATE(), NULL),

(N'DoubleTree by Hilton Çanakkale',
 N'Çanakkale şehir merkezinde yer alan DoubleTree by Hilton Çanakkale; Troy Park AVM’ye yakın konumu, açık ve kapalı yüzme havuzu, SPA merkezi, toplantı salonu, vale servisi ve 24 saat resepsiyon hizmetiyle öne çıkar.',
 N'Çanakkale Şehir Merkezi, Çanakkale',
 N'Çanakkale', N'Türkiye', N'444 0 387', NULL, NULL, NULL, 8.9, 5, 1, GETDATE(), NULL);
GO

INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId)
SELECT h.Id, rt.Id, s.Price, s.Extra, s.TotalRooms, 1, rv.Id
FROM (VALUES
(N'Swissôtel Resort Bodrum Beach', N'Bodrum Swiss - Standart Bahçe 2 Pax', N'Bahçe Manzaralı', 14221.90, 2500.00, 24),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum Swiss - Deniz Manzaralı 2 Pax', N'Deniz Manzaralı', 17178.46, 2500.00, 20),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum Swiss - Standart Bahçe 3 Pax', N'Bahçe Manzaralı', 16409.27, 2500.00, 16),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum Swiss - Deniz Manzaralı 3 Pax', N'Deniz Manzaralı', 18000.00, 2500.00, 14),

(N'Swissôtel Büyük Efes İzmir', N'Efes Swiss - Classic Room', N'Bahçe Manzaralı', 7500.00, 1200.00, 90),
(N'Swissôtel Büyük Efes İzmir', N'Efes Swiss - Classic Room', N'Şehir Manzaralı', 8200.00, 1200.00, 90),
(N'Swissôtel Büyük Efes İzmir', N'Efes Swiss - Swiss Advantage Room', N'Bahçe Manzaralı', 9200.00, 1400.00, 70),
(N'Swissôtel Büyük Efes İzmir', N'Efes Swiss - Executive Oda', N'Bahçe Manzaralı', 11500.00, 1600.00, 50),
(N'Swissôtel Büyük Efes İzmir', N'Efes Swiss - Executive Oda', N'Şehir Manzaralı', 12500.00, 1600.00, 50),
(N'Swissôtel Büyük Efes İzmir', N'Efes Swiss - Executive Oda', N'Deniz Manzaralı', 14500.00, 1800.00, 28),
(N'Swissôtel Büyük Efes İzmir', N'Efes Swiss - Suite', N'Deniz Manzaralı', 22000.00, 2500.00, 14),
(N'Swissôtel Büyük Efes İzmir', N'Efes Swiss - Executive Suite', N'Deniz Manzaralı', 30000.00, 3000.00, 10),

(N'Key Hotel İzmir', N'Key Hotel - Standard Oda', N'Şehir Manzaralı', 8619.00, 1000.00, 8),
(N'Key Hotel İzmir', N'Key Hotel - Standard Oda', N'Deniz Manzaralı', 9800.00, 1200.00, 8),
(N'Key Hotel İzmir', N'Key Hotel - Superior King Room', N'Deniz Manzaralı', 12500.00, 1500.00, 6),
(N'Key Hotel İzmir', N'Key Hotel - Family Süit', N'Deniz Manzaralı', 18500.00, 2200.00, 5),
(N'Key Hotel İzmir', N'Key Hotel - Executive Süit', N'Teras', 21000.00, 2500.00, 4),

(N'DoubleTree by Hilton Çanakkale', N'Canakkale - King Deluxe Bed', N'Şehir Manzaralı', 6900.00, 900.00, 40),
(N'DoubleTree by Hilton Çanakkale', N'Canakkale - Twin Guest Room', N'Şehir Manzaralı', 7200.00, 900.00, 25),
(N'DoubleTree by Hilton Çanakkale', N'Canakkale - King Bedroom Suite', N'Deniz Manzaralı', 9900.00, 1500.00, 10),
(N'DoubleTree by Hilton Çanakkale', N'Canakkale - Accessible King Room', N'Şehir Manzaralı', 7600.00, 900.00, 9)
) s(HotelName, RoomTypeName, ViewName, Price, Extra, TotalRooms)
JOIN Hotels h ON h.Name = s.HotelName
JOIN RoomTypes rt ON rt.Name = s.RoomTypeName
JOIN RoomViews rv ON rv.Name = s.ViewName;
GO

INSERT INTO HotelAmenities (HotelId, AmenityId)
SELECT DISTINCT h.Id, a.Id
FROM Hotels h
JOIN (VALUES
(N'Swissôtel Resort Bodrum Beach', N'Kablosuz İnternet'), (N'Swissôtel Resort Bodrum Beach', N'Otopark'), (N'Swissôtel Resort Bodrum Beach', N'Konsiyerj Hizmeti'), (N'Swissôtel Resort Bodrum Beach', N'Bahçe'), (N'Swissôtel Resort Bodrum Beach', N'Teras'), (N'Swissôtel Resort Bodrum Beach', N'Kapalı Yüzme Havuzu'), (N'Swissôtel Resort Bodrum Beach', N'Fitness'), (N'Swissôtel Resort Bodrum Beach', N'Spor Salonu'), (N'Swissôtel Resort Bodrum Beach', N'İskele'), (N'Swissôtel Resort Bodrum Beach', N'Plaj Şemsiyesi'), (N'Swissôtel Resort Bodrum Beach', N'Plaj Havlusu'), (N'Swissôtel Resort Bodrum Beach', N'Çocuk Kulübü'), (N'Swissôtel Resort Bodrum Beach', N'Çocuk Havuzu'), (N'Swissôtel Resort Bodrum Beach', N'Sauna'), (N'Swissôtel Resort Bodrum Beach', N'Buhar Odası'), (N'Swissôtel Resort Bodrum Beach', N'SPA Merkezi'), (N'Swissôtel Resort Bodrum Beach', N'Hamam'), (N'Swissôtel Resort Bodrum Beach', N'Bar'), (N'Swissôtel Resort Bodrum Beach', N'Restoran'),

(N'Swissôtel Büyük Efes İzmir', N'Kablosuz İnternet'), (N'Swissôtel Büyük Efes İzmir', N'24 Saat Resepsiyon Hizmeti'), (N'Swissôtel Büyük Efes İzmir', N'Bahçe'), (N'Swissôtel Büyük Efes İzmir', N'Klima'), (N'Swissôtel Büyük Efes İzmir', N'Asansör'), (N'Swissôtel Büyük Efes İzmir', N'Açık Yüzme Havuzu'), (N'Swissôtel Büyük Efes İzmir', N'Havuz'), (N'Swissôtel Büyük Efes İzmir', N'Teras'), (N'Swissôtel Büyük Efes İzmir', N'Konsiyerj Hizmeti'), (N'Swissôtel Büyük Efes İzmir', N'Günlük Temizlik'), (N'Swissôtel Büyük Efes İzmir', N'Emanet Kasası'), (N'Swissôtel Büyük Efes İzmir', N'Otopark'), (N'Swissôtel Büyük Efes İzmir', N'Toplantı Salonu'), (N'Swissôtel Büyük Efes İzmir', N'Fitness'), (N'Swissôtel Büyük Efes İzmir', N'Spor Salonu'), (N'Swissôtel Büyük Efes İzmir', N'Kapalı Yüzme Havuzu'), (N'Swissôtel Büyük Efes İzmir', N'Çocuk Havuzu'), (N'Swissôtel Büyük Efes İzmir', N'SPA Merkezi'), (N'Swissôtel Büyük Efes İzmir', N'Sauna'), (N'Swissôtel Büyük Efes İzmir', N'Hamam'), (N'Swissôtel Büyük Efes İzmir', N'Masaj'), (N'Swissôtel Büyük Efes İzmir', N'Restoran'), (N'Swissôtel Büyük Efes İzmir', N'Bar'),

(N'Key Hotel İzmir', N'SPA Merkezi'), (N'Key Hotel İzmir', N'Ücretsiz Otopark'), (N'Key Hotel İzmir', N'Otopark'), (N'Key Hotel İzmir', N'Kablosuz İnternet'), (N'Key Hotel İzmir', N'Restoran'), (N'Key Hotel İzmir', N'Klima'), (N'Key Hotel İzmir', N'Spor Salonu'), (N'Key Hotel İzmir', N'Fitness'), (N'Key Hotel İzmir', N'24 Saat Resepsiyon Hizmeti'), (N'Key Hotel İzmir', N'24 Saat Resepsiyon'), (N'Key Hotel İzmir', N'Toplantı Salonu'), (N'Key Hotel İzmir', N'Asansör'), (N'Key Hotel İzmir', N'Emanet Kasası'), (N'Key Hotel İzmir', N'Günlük Temizlik'), (N'Key Hotel İzmir', N'Konsiyerj Hizmeti'), (N'Key Hotel İzmir', N'Bar'),

(N'DoubleTree by Hilton Çanakkale', N'Toplantı Salonu'), (N'DoubleTree by Hilton Çanakkale', N'24 Saat Resepsiyon'), (N'DoubleTree by Hilton Çanakkale', N'Açık Yüzme Havuzu'), (N'DoubleTree by Hilton Çanakkale', N'Kapalı Yüzme Havuzu'), (N'DoubleTree by Hilton Çanakkale', N'Asansör'), (N'DoubleTree by Hilton Çanakkale', N'Vale Servisi'), (N'DoubleTree by Hilton Çanakkale', N'SPA Merkezi')
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
    N'Oda Servisi', N'Sigara İçilmez', N'Minibar', N'LCD / Plazma TV',
    N'LCD TV', N'Televizyon', N'Kablosuz İnternet', N'WiFi',
    N'Balkon veya Teras', N'Bornoz', N'Bornoz ve Terlik',
    N'Çalışma Masası', N'Duş', N'Kahve Makinesi',
    N'Kahve / Çay Makinesi', N'Çay / Kahve Seti', N'Terlik',
    N'Saç Kurutma Makinesi', N'Su Isıtıcısı', N'Ücretsiz Şişe Su',
    N'Kasa', N'Klima', N'Buklet Banyo Ürünleri', N'Havlu',
    N'Makyaj Aynası', N'Sofa', N'Uyandırma Servisi', N'Telefon',
    N'Uydu Yayını', N'Elektronik Anahtar Sistemi', N'Gardırop',
    N'Duman Dedektörü', N'Günlük Temizlik'
)
WHERE h.Name IN (
    N'Swissôtel Resort Bodrum Beach',
    N'Swissôtel Büyük Efes İzmir',
    N'Key Hotel İzmir',
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

(N'Swissôtel Büyük Efes İzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/lvxjppbfda11udukq703.jpg', 1, 0),
(N'Swissôtel Büyük Efes İzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Swissotel_B_y_k_Efes_Izmir_202604131115527_Swissotel_Buyuk_Efes_Izmir_pool_2.jpg', 0, 1),
(N'Swissôtel Büyük Efes İzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/eiotkjibrkgldpfjsw6z.jpg', 0, 2),
(N'Swissôtel Büyük Efes İzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Swissotel_Buyuk_Efes_Izmir_20231102163034_0.jpg', 0, 3),
(N'Swissôtel Büyük Efes İzmir', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/md17c5bosgjvozlaoovl.jpg', 0, 4),

(N'Key Hotel İzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/5247462b.jpg?impolicy=resizecrop&ra=fit&rw=598', 1, 0),
(N'Key Hotel İzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/141f1a30.jpg?impolicy=resizecrop&ra=fit&rw=297', 0, 1),
(N'Key Hotel İzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/1015cac4.jpg?impolicy=resizecrop&ra=fit&rw=297', 0, 2),
(N'Key Hotel İzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/21ff2106.jpg?impolicy=resizecrop&ra=fit&rw=297', 0, 3),
(N'Key Hotel İzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/5d58d22c.jpg?h=800&impolicy=fcrop&quality=medium&w=1200', 0, 4),

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

(N'Swissôtel Büyük Efes İzmir', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Swissotel_Buyuk_Efes_Izmir_20231102173211_1.jpg', 1, 0),
(N'Swissôtel Büyük Efes İzmir', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Swissotel_Buyuk_Efes_Izmir_20231102173312_1.jpg', 0, 1),
(N'Swissôtel Büyük Efes İzmir', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Swissotel_Buyuk_Efes_Izmir_20231102173311_0.jpg', 0, 2),

(N'Key Hotel İzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/14e05981.jpg?h=800&impolicy=fcrop&quality=medium&w=1200', 1, 0),
(N'Key Hotel İzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/841b62f0.jpg?h=800&impolicy=fcrop&quality=medium&w=1200', 0, 1),
(N'Key Hotel İzmir', N'https://images.trvl-media.com/lodging/6000000/5610000/5608300/5608297/fc0d53b2.jpg?h=800&impolicy=fcrop&quality=medium&w=1200', 0, 2),

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
    N'Swissôtel Büyük Efes İzmir',
    N'Key Hotel İzmir',
    N'DoubleTree by Hilton Çanakkale'
)
GROUP BY h.Name, h.City
ORDER BY h.Name;
GO



USE HotelAppDB;
GO

;WITH SeedUsers AS
(
    SELECT
        Id,
        ROW_NUMBER() OVER (ORDER BY Email) AS rn
    FROM Users
    WHERE Email LIKE N'hotel.review.%@hotelapp.local'
)
UPDATE u
SET
    FirstName = CASE ((s.rn - 1) % 10)
        WHEN 0 THEN N'S****'
        WHEN 1 THEN N'A****'
        WHEN 2 THEN N'M****'
        WHEN 3 THEN N'E****'
        WHEN 4 THEN N'B****'
        WHEN 5 THEN N'D****'
        WHEN 6 THEN N'K****'
        WHEN 7 THEN N'Y****'
        WHEN 8 THEN N'N****'
        ELSE N'C****'
    END,
    LastName = CASE ((s.rn - 1) % 10)
        WHEN 0 THEN N'F******'
        WHEN 1 THEN N'K******'
        WHEN 2 THEN N'Y******'
        WHEN 3 THEN N'A******'
        WHEN 4 THEN N'D******'
        WHEN 5 THEN N'T******'
        WHEN 6 THEN N'Ö******'
        WHEN 7 THEN N'S******'
        WHEN 8 THEN N'Ç******'
        ELSE N'G******'
    END
FROM Users u
JOIN SeedUsers s ON s.Id = u.Id;
GO