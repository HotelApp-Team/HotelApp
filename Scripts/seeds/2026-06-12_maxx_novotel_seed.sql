USE HotelAppDB;
GO
SET NOCOUNT ON;
GO

-- GEREKLI MANZARALAR
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name=N'Deniz Manzaralı')
    INSERT INTO RoomViews VALUES(N'Deniz Manzaralı',N'bi-water',1,1);
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name=N'Şehir Manzaralı')
    INSERT INTO RoomViews VALUES(N'Şehir Manzaralı',N'bi-buildings',2,1);
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name=N'Kara Manzaralı')
    INSERT INTO RoomViews VALUES(N'Kara Manzaralı',N'bi-tree',3,1);
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name=N'Havuz Manzaralı')
    INSERT INTO RoomViews VALUES(N'Havuz Manzaralı',N'bi-droplet',4,1);
GO

-- OTEL BILGILERI
UPDATE Hotels SET
    Description=N'Belek sahilinde yer alan Maxx Royal Belek Golf Resort; geniş süitleri, özel plajı, havuzları, aquaparkı, çocuk kulübü, spa merkezi ve Montgomerie Maxx Royal Golf Sahası ile lüks aile tatili sunar.',
    Address=N'İskele Mevkii, Belek, Serik, Antalya',
    City=N'Antalya', Country=N'Türkiye',
    AvgRating=9.5, StarRating=5, IsActive=1, UpdatedAt=GETDATE()
WHERE Name=N'Maxx Royal Belek Golf Resort';

IF NOT EXISTS (SELECT 1 FROM Hotels WHERE Name=N'Maxx Royal Belek Golf Resort')
INSERT INTO Hotels
(Name,Description,Address,City,Country,Phone,Email,Latitude,Longitude,
 AvgRating,StarRating,IsActive,CreatedAt,UpdatedAt)
VALUES
(N'Maxx Royal Belek Golf Resort',
 N'Belek sahilinde yer alan Maxx Royal Belek Golf Resort; geniş süitleri, özel plajı, havuzları, aquaparkı, çocuk kulübü, spa merkezi ve golf sahasıyla lüks aile tatili sunar.',
 N'İskele Mevkii, Belek, Serik, Antalya',
 N'Antalya',N'Türkiye',NULL,NULL,NULL,NULL,9.5,5,1,GETDATE(),NULL);

UPDATE Hotels SET
    Description=N'Novotel Kayseri; şehir merkezi ve havalimanına kolay erişimi, Erciyes manzarası, modern odaları, restoranı, barı, fitness merkezi, toplantı salonları ve otoparkıyla dört yıldızlı bir şehir otelidir.',
    Address=N'Alsancak Mahallesi, Kocasinan Bulvarı No:161/1, 38110 Kocasinan, Kayseri',
    City=N'Kayseri', Country=N'Türkiye',
    Phone=N'+90 352 207 30 00', Email=N'H6904@accor.com',
    Latitude=38.735816, Longitude=35.498283,
    AvgRating=8.6, StarRating=4, IsActive=1, UpdatedAt=GETDATE()
WHERE Name=N'Novotel Kayseri';
GO

-- ESKI ODA VE GORSELLERI TEMIZLE
-- Dikkat: Bu iki otele bağlı demo rezervasyonlar da silinir.
DECLARE @HotelIds TABLE(Id INT);

INSERT INTO @HotelIds
SELECT Id FROM Hotels
WHERE Name IN(N'Maxx Royal Belek Golf Resort',N'Novotel Kayseri');

DELETE r FROM Reservations r
JOIN HotelRooms hr ON hr.Id=r.HotelRoomId
WHERE hr.HotelId IN(SELECT Id FROM @HotelIds);

DELETE ri FROM RoomImages ri
JOIN HotelRooms hr ON hr.Id=ri.HotelRoomId
WHERE hr.HotelId IN(SELECT Id FROM @HotelIds);

DELETE hra FROM HotelRoomAmenities hra
JOIN HotelRooms hr ON hr.Id=hra.HotelRoomId
WHERE hr.HotelId IN(SELECT Id FROM @HotelIds);

DELETE FROM HotelRooms
WHERE HotelId IN(SELECT Id FROM @HotelIds);

DELETE FROM HotelImages
WHERE HotelId IN(SELECT Id FROM @HotelIds);
GO

-- GERCEK ODA TIPLERI
INSERT INTO RoomTypes(Name,Description,MaxGuests)
SELECT v.Name,v.Description,v.MaxGuests
FROM (VALUES
(N'Maxx Royal - Suite Land View',N'Kara manzaralı, balkonlu ve jakuzili 80 m2 süit.',3),
(N'Maxx Royal - Suite Sea View',N'Deniz manzaralı, balkonlu ve jakuzili 80 m2 süit.',3),
(N'Maxx Royal - Family Suite Land View',N'İki yatak odalı, kara manzaralı 100 m2 aile süiti.',4),
(N'Maxx Royal - Family Suite Sea View',N'İki yatak odalı, deniz manzaralı 100 m2 aile süiti.',4),
(N'Maxx Royal - Terrace Family Suite',N'İki balkonlu ve jakuzili 100 m2 aile süiti.',4),
(N'Maxx Royal - Terrace Lagoon Family Suite',N'Terastan havuza geçişli 100 m2 aile süiti.',4),
(N'Maxx Royal - Royal Suite',N'Deniz manzaralı ve jakuzili 100 m2 Royal Suite.',3),
(N'Maxx Royal - Royal Deluxe Suite',N'İki yatak odalı ve deniz manzaralı 170 m2 süit.',6),
(N'Maxx Royal - Lagoon Duplex Suite',N'Havuz manzaralı ve teraslı 135 m2 dubleks süit.',4),
(N'Novotel Kayseri - Superior Queen Room',N'Queen yatak ve çekyatlı 24 m2 superior oda.',4),
(N'Novotel Kayseri - Superior Twin Room',N'İki tek kişilik yataklı 24 m2 superior oda.',2),
(N'Novotel Kayseri - Executive Queen Room',N'Queen yatak ve çekyatlı 24 m2 executive oda.',2),
(N'Novotel Kayseri - Superior Suite',N'Queen yatak ve çekyatlı 50 m2 superior süit.',4)
) v(Name,Description,MaxGuests)
WHERE NOT EXISTS
(
    SELECT 1 FROM RoomTypes rt WHERE rt.Name=v.Name
);
GO

-- ODALAR: FIYAT VE STOKLAR DEMO DEGERIDIR
INSERT INTO HotelRooms
(HotelId,RoomTypeId,PricePerNight,ExtraGuestPricePerNight,
 TotalRooms,IsActive,RoomViewId)
SELECT h.Id,rt.Id,v.Price,v.Extra,v.TotalRooms,1,rv.Id
FROM (VALUES
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Suite Land View',N'Kara Manzaralı',45000,6000,80),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Suite Sea View',N'Deniz Manzaralı',52000,7000,70),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Family Suite Land View',N'Kara Manzaralı',68000,8000,35),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Family Suite Sea View',N'Deniz Manzaralı',76000,9000,30),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Terrace Family Suite',N'Kara Manzaralı',84000,10000,20),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Terrace Lagoon Family Suite',N'Havuz Manzaralı',94000,11000,18),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Royal Suite',N'Deniz Manzaralı',110000,14000,12),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Royal Deluxe Suite',N'Deniz Manzaralı',165000,18000,6),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Lagoon Duplex Suite',N'Havuz Manzaralı',125000,15000,10),
(N'Novotel Kayseri',N'Novotel Kayseri - Superior Queen Room',N'Şehir Manzaralı',3600,500,55),
(N'Novotel Kayseri',N'Novotel Kayseri - Superior Twin Room',N'Şehir Manzaralı',3600,0,35),
(N'Novotel Kayseri',N'Novotel Kayseri - Executive Queen Room',N'Şehir Manzaralı',4500,600,20),
(N'Novotel Kayseri',N'Novotel Kayseri - Superior Suite',N'Şehir Manzaralı',6200,800,10)
) v(HotelName,RoomTypeName,ViewName,Price,Extra,TotalRooms)
JOIN Hotels h ON h.Name=v.HotelName
JOIN RoomTypes rt ON rt.Name=v.RoomTypeName
JOIN RoomViews rv ON rv.Name=v.ViewName;
GO

-- OTEL GORSELLERI
INSERT INTO HotelImages(HotelId,ImageUrl,IsMain,DisplayOrder)
SELECT h.Id,v.Url,v.IsMain,v.DisplayOrder
FROM Hotels h
JOIN (VALUES
(N'Maxx Royal Belek Golf Resort',
 N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/50340/l/Maxx-Royal-Belek-Golf-Resort-Genel-366749.jpg',1,0),
(N'Maxx Royal Belek Golf Resort',
 N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/50340/l/Maxx-Royal-Belek-Golf-Resort-Genel-372670.jpg',0,1),
(N'Maxx Royal Belek Golf Resort',
 N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/50340/l/Maxx-Royal-Belek-Golf-Resort-Genel-365540.jpg',0,2),
(N'Novotel Kayseri',
 N'https://www.ahstatic.com/photos/6904_ho_00_p_1024x768.jpg',1,0),
(N'Novotel Kayseri',
 N'https://www.ahstatic.com/photos/6904_ho_01_p_1024x768.jpg',0,1),
(N'Novotel Kayseri',
 N'https://www.ahstatic.com/photos/6904_ho_02_p_1024x768.jpg',0,2)
) v(HotelName,Url,IsMain,DisplayOrder)
ON h.Name=v.HotelName;
GO

-- HER ODA TIPININ GERCEK FOTOGRAFI
INSERT INTO RoomImages(HotelRoomId,ImageUrl,IsMain,DisplayOrder)
SELECT hr.Id,v.Url,1,0
FROM HotelRooms hr
JOIN Hotels h ON h.Id=hr.HotelId
JOIN RoomTypes rt ON rt.Id=hr.RoomTypeId
JOIN (VALUES
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Suite Land View',
 N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/121/Maxx_Royal_Belek_Golf_Resort_20250725121738_1.jpg'),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Suite Sea View',
 N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/121/Maxx_Royal_Belek_Golf_Resort_20250725121636_2.jpg'),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Family Suite Land View',
 N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/121/Maxx_Royal_Belek_Golf_Resort_20250725120454_4.jpg'),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Family Suite Sea View',
 N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/121/Maxx_Royal_Belek_Golf_Resort_20250725120319_2.jpg'),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Terrace Family Suite',
 N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/121/Maxx_Royal_Belek_Golf_Resort_20250725121911_3.jpg'),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Terrace Lagoon Family Suite',
 N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/121/Maxx_Royal_Belek_Golf_Resort_20250725122039_1.jpg'),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Royal Suite',
 N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/121/Maxx_Royal_Belek_Golf_Resort_20250725121458_0.jpg'),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Royal Deluxe Suite',
 N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/121/Maxx_Royal_Belek_Golf_Resort_20250725121403_2.jpg'),
(N'Maxx Royal Belek Golf Resort',N'Maxx Royal - Lagoon Duplex Suite',
 N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/121/Maxx_Royal_Belek_Golf_Resort_20250725120708_3.jpg'),
(N'Novotel Kayseri',N'Novotel Kayseri - Superior Queen Room',
 N'https://www.ahstatic.com/photos/6904_roqsb_00_p_1024x768.jpg'),
(N'Novotel Kayseri',N'Novotel Kayseri - Superior Twin Room',
 N'https://www.ahstatic.com/photos/6904_rotwb_00_p_1024x768.jpg'),
(N'Novotel Kayseri',N'Novotel Kayseri - Executive Queen Room',
 N'https://www.ahstatic.com/photos/6904_roqse_00_p_1024x768.jpg'),
(N'Novotel Kayseri',N'Novotel Kayseri - Superior Suite',
 N'https://www.ahstatic.com/photos/6904_roscj_00_p_1024x768.jpg')
) v(HotelName,RoomTypeName,Url)
ON h.Name=v.HotelName AND rt.Name=v.RoomTypeName;
GO

-- SONUC KONTROLU
SELECT
    h.Name,
    COUNT(DISTINCT hr.Id) AS RoomCount,
    COUNT(DISTINCT hi.Id) AS HotelImageCount,
    COUNT(DISTINCT ri.Id) AS RoomImageCount
FROM Hotels h
LEFT JOIN HotelRooms hr ON hr.HotelId=h.Id
LEFT JOIN HotelImages hi ON hi.HotelId=h.Id
LEFT JOIN RoomImages ri ON ri.HotelRoomId=hr.Id
WHERE h.Name IN
(
    N'Maxx Royal Belek Golf Resort',
    N'Novotel Kayseri'
)
GROUP BY h.Name;
GO