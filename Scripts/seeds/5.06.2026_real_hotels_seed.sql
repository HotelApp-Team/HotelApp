-- =============================================================================
-- 10 OTEL SEED SCRIPT
-- Reset + reseed mantigi: listedeki otelleri ve bagli kayitlari siler, tekrar ekler.
-- Dosyayi UTF-8 kaydet.
-- =============================================================================

USE HotelAppDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
GO

-- =============================================================================
-- 0) TEMIZLIK
-- =============================================================================
DECLARE @ids TABLE (Id INT);

INSERT INTO @ids (Id)
SELECT Id
FROM Hotels
WHERE Name IN (
    N'Swissôtel Resort Bodrum Beach',
    N'Swissôtel Büyük Efes Ýzmir',
    N'Key Hotel Ýzmir',
    N'DoubleTree by Hilton Çanakkale',
    N'Yazz Collective Fethiye',
    N'Adam & Eve - Adult Only',
    N'Liberty Fabay',
    N'Jiva Beach Resort',
    N'Yacht Classic Hotel',
    N'Sundia by Liberty Ölüdeniz',
    N'SwissÃ´tel Resort Bodrum Beach',
    N'SwissÃ´tel BÃ¼yÃ¼k Efes Ä°zmir',
    N'Key Hotel Ä°zmir',
    N'DoubleTree by Hilton Ã‡anakkale',
    N'Sundia by Liberty Ã–lÃ¼deniz'
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

-- =============================================================================
-- 1) ROOM VIEWS
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name = N'Deniz Manzaralý')
    INSERT INTO RoomViews (Name, Icon, DisplayOrder, IsActive) VALUES (N'Deniz Manzaralý', N'bi-water', 1, 1);
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name = N'Þehir Manzaralý')
    INSERT INTO RoomViews (Name, Icon, DisplayOrder, IsActive) VALUES (N'Þehir Manzaralý', N'bi-buildings', 2, 1);
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name = N'Dað Manzaralý')
    INSERT INTO RoomViews (Name, Icon, DisplayOrder, IsActive) VALUES (N'Dað Manzaralý', N'bi-tree', 3, 1);
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name = N'Havuz Manzaralý')
    INSERT INTO RoomViews (Name, Icon, DisplayOrder, IsActive) VALUES (N'Havuz Manzaralý', N'bi-droplet', 4, 1);
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name = N'Bahçe Manzaralý')
    INSERT INTO RoomViews (Name, Icon, DisplayOrder, IsActive) VALUES (N'Bahçe Manzaralý', N'bi-flower2', 5, 1);
IF NOT EXISTS (SELECT 1 FROM RoomViews WHERE Name = N'Manzara Yok')
    INSERT INTO RoomViews (Name, Icon, DisplayOrder, IsActive) VALUES (N'Manzara Yok', N'bi-dash-circle', 99, 1);
GO

-- =============================================================================
-- 2) ROOM TYPES
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Bodrum - Swiss Advantage Garden View Twin') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Bodrum - Swiss Advantage Garden View Twin', N'Bodrum Beach bahçe manzaralý twin oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Bodrum - Swiss Advantage Garden View King') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Bodrum - Swiss Advantage Garden View King', N'Bodrum Beach bahçe manzaralý king oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Bodrum - Swiss Select Sea-view Room Twin') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Bodrum - Swiss Select Sea-view Room Twin', N'Bodrum Beach deniz manzaralý twin oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Bodrum - Swiss Select Sea-view Room King') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Bodrum - Swiss Select Sea-view Room King', N'Bodrum Beach deniz manzaralý king oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Bodrum - Corner Garden View Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Bodrum - Corner Garden View Suite', N'Bodrum Beach bahçe manzaralý köþe süit', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Bodrum - Corner Sea View Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Bodrum - Corner Sea View Suite', N'Bodrum Beach deniz manzaralý köþe süit', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Bodrum - Signature Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Bodrum - Signature Suite', N'Bodrum Beach signature süit', 4);

IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Izmir - Executive Room City View') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Izmir - Executive Room City View', N'Ýzmir þehir manzaralý executive oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Izmir - Swiss Advantage City View Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Izmir - Swiss Advantage City View Room', N'Ýzmir þehir manzaralý advantage oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Izmir - Executive Room Garden View') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Izmir - Executive Room Garden View', N'Ýzmir bahçe manzaralý executive oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Izmir - Classic Room City View') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Izmir - Classic Room City View', N'Ýzmir þehir manzaralý classic oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Izmir - Classic Room Garden View') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Izmir - Classic Room Garden View', N'Ýzmir bahçe manzaralý classic oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Izmir - Swiss Advantage Garden View Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Izmir - Swiss Advantage Garden View Room', N'Ýzmir bahçe manzaralý advantage oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Izmir - Swiss Executive Room Sea View') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Izmir - Swiss Executive Room Sea View', N'Ýzmir deniz manzaralý executive oda', 2);

IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Key - Standard Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Key - Standard Room', N'Key Hotel standart oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Key - Superior Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Key - Superior Room', N'Key Hotel superior oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Key - Family Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Key - Family Suite', N'Key Hotel family suite', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Key - Executive Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Key - Executive Suite', N'Key Hotel executive suite', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Key - Deluxe Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Key - Deluxe Suite', N'Key Hotel deluxe suite', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Key - Grand Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Key - Grand Suite', N'Key Hotel grand suite', 2);

IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Canakkale - King Deluxe Bed') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Canakkale - King Deluxe Bed', N'Çanakkale king deluxe oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Canakkale - Twin Guest Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Canakkale - Twin Guest Room', N'Çanakkale twin misafir odasý', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Canakkale - King Bedroom Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Canakkale - King Bedroom Suite', N'Çanakkale king bedroom suite', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Canakkale - Accessible King Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Canakkale - Accessible King Room', N'Çanakkale eriþilebilir king oda', 2);

IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Yazz - Signature Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Yazz - Signature Suite', N'Yazz signature suite', 4);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Yazz - Comfort Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Yazz - Comfort Suite', N'Yazz comfort suite', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Yazz - Junior Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Yazz - Junior Suite', N'Yazz junior suite', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Yazz - Deluxe Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Yazz - Deluxe Room', N'Yazz deluxe room', 2);

IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Design Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Design Room', N'Adam & Eve 64 m2 design room', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Design Sea View Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Design Sea View Room', N'Adam & Eve direct sea view design room', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Design Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Design Suite', N'Adam & Eve 120 m2 design suite', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'King Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'King Suite', N'Lüks king suite', 6);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Adam Villa') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Adam Villa', N'Adam & Eve Adam Villa', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Eve Villa') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Eve Villa', N'Adam & Eve Eve Villa', 4);

IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Standard Land View Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Standard Land View Room', N'Kara manzaralý standart oda', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Standard Sea View Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Standard Sea View Room', N'Deniz manzaralý standart oda', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Swim Up Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Swim Up Room', N'Havuza baðlantýlý oda', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Large Swim Up Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Large Swim Up Room', N'Havuza baðlantýlý geniþ oda', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Family Land View Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Family Land View Room', N'Kara manzaralý aile odasý', 4);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Family Sea View Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Family Sea View Room', N'Deniz manzaralý aile odasý', 4);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Private Suite Via Pool') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Private Suite Via Pool', N'Özel havuzlu suite', 4);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Duplex Lake House') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Duplex Lake House', N'154 m2 iki katlý lake house', 6);

IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Attic Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Attic Room', N'Jiva çatý katý odasý', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Suite Family Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Suite Family Room', N'Jiva suite aile odasý', 5);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Corner Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Corner Suite', N'Jiva corner suite', 5);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Honeymoon Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Honeymoon Room', N'Balayý odasý', 2);

IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Classic Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Classic Room', N'Yacht Classic klasik oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Junior Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Junior Suite', N'Geniþ oturma alanlý junior suite', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Loft Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Loft Room', N'Yacht Classic loft oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Pool Suite') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Pool Suite', N'Özel havuzlu suite', 4);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Water Villa') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Water Villa', N'Swim-up water villa', 3);

IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Superior Double Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Superior Double Room', N'Sundia superior iki kiþilik oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Superior Triple Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Superior Triple Room', N'Sundia superior üç kiþilik oda', 3);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Superior Family Room') INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Superior Family Room', N'Sundia superior aile odasý', 4);
GO

-- =============================================================================
-- 3) AMENITIES
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'WiFi') INSERT INTO Amenities (Name, Icon) VALUES (N'WiFi', N'wifi');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Havuz') INSERT INTO Amenities (Name, Icon) VALUES (N'Havuz', N'water');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Restoran') INSERT INTO Amenities (Name, Icon) VALUES (N'Restoran', N'cup-straw');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Bar') INSERT INTO Amenities (Name, Icon) VALUES (N'Bar', N'cup-straw');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Spa') INSERT INTO Amenities (Name, Icon) VALUES (N'Spa', N'heart-pulse');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Hamam') INSERT INTO Amenities (Name, Icon) VALUES (N'Hamam', N'droplet');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Türk Hamamý') INSERT INTO Amenities (Name, Icon) VALUES (N'Türk Hamamý', N'droplet');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Sauna') INSERT INTO Amenities (Name, Icon) VALUES (N'Sauna', N'thermometer');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Özel Plaj') INSERT INTO Amenities (Name, Icon) VALUES (N'Özel Plaj', N'umbrella');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Fitness') INSERT INTO Amenities (Name, Icon) VALUES (N'Fitness', N'dumbbell');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Fitness Merkezi') INSERT INTO Amenities (Name, Icon) VALUES (N'Fitness Merkezi', N'heart-pulse');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Çocuk Kulübü') INSERT INTO Amenities (Name, Icon) VALUES (N'Çocuk Kulübü', N'emoji-smile');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Aquapark') INSERT INTO Amenities (Name, Icon) VALUES (N'Aquapark', N'waves');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Su Parký') INSERT INTO Amenities (Name, Icon) VALUES (N'Su Parký', N'water');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Tenis Kortu') INSERT INTO Amenities (Name, Icon) VALUES (N'Tenis Kortu', N'trophy');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Beach Club') INSERT INTO Amenities (Name, Icon) VALUES (N'Beach Club', N'sun');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Su Sporlarý') INSERT INTO Amenities (Name, Icon) VALUES (N'Su Sporlarý', N'life-preserver');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Concierge') INSERT INTO Amenities (Name, Icon) VALUES (N'Concierge', N'bell');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Marina') INSERT INTO Amenities (Name, Icon) VALUES (N'Marina', N'anchor');
GO

-- =============================================================================
-- 4) ROOM AMENITIES
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'WiFi') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'WiFi', N'wifi');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Klima') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Klima', N'snow');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Kahvaltý') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Kahvaltý', N'cup-hot');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Minibar') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Minibar', N'cup-straw');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Smart TV') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Smart TV', N'tv');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'LCD TV') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'LCD TV', N'tv');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Kasa') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Kasa', N'safe2');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Saç Kurutma Makinesi') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Saç Kurutma Makinesi', N'wind');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Balkon') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Balkon', N'door-open');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Jakuzi') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Jakuzi', N'droplet-half');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Çay/Kahve Makinesi') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Çay/Kahve Makinesi', N'cup-hot');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Çay-Kahve Seti') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Çay-Kahve Seti', N'coffee');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Bornoz & Terlik') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Bornoz & Terlik', N'basket');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Bornoz') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Bornoz', N'shirt');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Özel Havuz') INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Özel Havuz', N'water');
GO

-- =============================================================================
-- 5) HOTELS
-- =============================================================================
INSERT INTO Hotels
(Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, AvgRating, StarRating, IsActive, CreatedAt, UpdatedAt)
VALUES
(N'Swissôtel Resort Bodrum Beach', N'Turgutreis sahilinde yer alan, deniz kýyýsý konumu ve Purovel Spa & Sport olanaklarýyla öne çýkan 5 yýldýzlý resort otel.', N'Turgutreis Mah. Gazi Mustafa Kemal Bulvarý No:42, 48400 Bodrum/Muðla, Türkiye', N'Bodrum', N'Türkiye', N'+90 252 311 33 33', N'bodrum@swissotel.com', NULL, NULL, 4.4, 5, 1, GETDATE(), NULL),
(N'Swissôtel Büyük Efes Ýzmir', N'Ýzmir merkezinde yer alan, geniþ bahçeleri ve spa olanaklarýyla öne çýkan 5 yýldýzlý þehir oteli.', N'Gaziosmanpaþa Bulvarý No:1, 35210 Alsancak, Ýzmir, Türkiye', N'Ýzmir', N'Türkiye', N'+90 232 414 00 00', N'izmir@swissotel.com', NULL, NULL, NULL, 5, 1, GETDATE(), NULL),
(N'Key Hotel Ýzmir', N'Ýzmir Kordon üzerinde, eski Merkez Bankasý binasýnda konumlanan premium butik otel.', N'Akdeniz, Atatürk Cd. No:36, 35210 Konak/Ýzmir, Türkiye', N'Ýzmir', N'Türkiye', N'+90 232 482 11 11', N'reservation@keyhotel.com', NULL, NULL, NULL, 5, 1, GETDATE(), NULL),
(N'DoubleTree by Hilton Çanakkale', N'Çanakkale merkezinde, Dardanel Boðazý manzarasý, spa, restoran, kapalý ve açýk havuzlarýyla öne çýkan þehir oteli.', N'Barbaros Mahallesi, Atatürk Cd. 306, Çanakkale, 17020, Türkiye', N'Çanakkale', N'Türkiye', N'+90 286 295 55 55', N'ckzcd_dt_hotel@hilton.com', NULL, NULL, 4.8, 5, 1, GETDATE(), NULL),
(N'Yazz Collective Fethiye', N'Pýnaraltý Turunç Pýnarý Koyu''nda, yalnýzca deniz yoluyla eriþilen, doða odaklý lüks retreat.', N'Pýnaraltý Turunç Pýnarý Koyu, Fethiye/Muðla, Türkiye', N'Fethiye', N'Türkiye', N'+90 530 277 92 99', N'reservation@yazzcollective.com', 36.613333, 29.053333, NULL, 5, 1, GETDATE(), NULL),
(N'Adam & Eve - Adult Only', N'Belek Ýskele mevkiinde, +16 adult only konseptli; Design Room, Design Suite, King Suite, Adam Villa ve Eve Villa oda seçenekleri bulunan lüks Stone Group oteli.', N'Ýskele Mevkii, Belek 07500, Antalya', N'Antalya', N'Türkiye', N'+90 242 710 1400', N'info@adamevehotel.com', 36.858000, 31.069000, NULL, 5, 1, GETDATE(), NULL),
(N'Liberty Fabay', N'Fethiye Kargý''da Adalar manzaralý, denize sýfýr konumlu; standart, aile, swim-up, private suite ve Duplex Lake House oda seçenekleri bulunan lüks Liberty oteli.', N'Kargý Mh. 202 Manolya Sk No:4/2 Fethiye, Muðla', N'Fethiye', N'Türkiye', N'0252 623 99 38', N'info@libertyfabay.com', 36.686000, 29.087000, NULL, 5, 1, GETDATE(), NULL),
(N'Jiva Beach Resort', N'Fethiye Çalýþ bölgesinde, ultra her þey dahil konsepti; standart oda, aile odasý, suite aile odasý, corner suite, king suite, swim-up ve balayý odalarý bulunan resort.', N'Karagedik, 1104. Sk. No:14, 48300 Fethiye, Muðla', N'Fethiye', N'Türkiye', N'+90 (252) 622 00 55', N'info@jivabeachresort.com', 36.674933, 29.101131, NULL, 5, 1, GETDATE(), NULL),
(N'Yacht Classic Hotel', N'Fethiye merkez Karagözler''de denize sýfýr konumlu; marina, havuz, spa, La Brise restoran ve Classic, Junior Suite, Loft, Pool Suite, Water Villa oda seçenekleri bulunan butik otel.', N'Fevzi Çakmak Cd. No:24, 48300 Fethiye, Muðla', N'Fethiye', N'Türkiye', N'+90 252 612 5067', N'info@yachtclassichotel.com', 36.623000, 29.103000, 9.4, 4, 1, GETDATE(), NULL),
(N'Sundia by Liberty Ölüdeniz', N'Ölüdeniz merkezinde, Belcekýz Plajý''na yakýn; superior odalar, aile odasý, jakuzili balayý odasý, restoran-bar, havuzlar ve özel plaj alaný sunan otel.', N'Belceðiz Mevkii. 226 Sk. No:10, 48340 Ölüdeniz, Fethiye, Muðla', N'Fethiye', N'Türkiye', N'+90 (252) 617 01 54', N'info@sundiaoludeniz.com', 36.547000, 29.123000, NULL, 4, 1, GETDATE(), NULL);
GO

-- =============================================================================
-- 6) HOTEL ROOMS
-- =============================================================================
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId)
SELECT h.Id, rt.Id, s.Price, s.Extra, s.TotalRooms, 1, rv.Id
FROM (VALUES
(N'Swissôtel Resort Bodrum Beach', N'Bodrum - Swiss Advantage Garden View Twin', N'Bahçe Manzaralý', 12900.00, 1750.00, 14),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum - Swiss Advantage Garden View King', N'Bahçe Manzaralý', 13900.00, 1750.00, 14),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum - Swiss Select Sea-view Room Twin', N'Deniz Manzaralý', 15900.00, 2000.00, 12),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum - Swiss Select Sea-view Room King', N'Deniz Manzaralý', 16900.00, 2000.00, 12),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum - Corner Garden View Suite', N'Bahçe Manzaralý', 21900.00, 3000.00, 6),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum - Corner Sea View Suite', N'Deniz Manzaralý', 24900.00, 3500.00, 6),
(N'Swissôtel Resort Bodrum Beach', N'Bodrum - Signature Suite', N'Deniz Manzaralý', 29900.00, 4000.00, 2),

(N'Swissôtel Büyük Efes Ýzmir', N'Izmir - Executive Room City View', N'Þehir Manzaralý', 8900.00, 1200.00, 80),
(N'Swissôtel Büyük Efes Ýzmir', N'Izmir - Swiss Advantage City View Room', N'Þehir Manzaralý', 9400.00, 1200.00, 70),
(N'Swissôtel Büyük Efes Ýzmir', N'Izmir - Executive Room Garden View', N'Bahçe Manzaralý', 9800.00, 1300.00, 60),
(N'Swissôtel Büyük Efes Ýzmir', N'Izmir - Classic Room City View', N'Þehir Manzaralý', 10200.00, 1300.00, 80),
(N'Swissôtel Büyük Efes Ýzmir', N'Izmir - Classic Room Garden View', N'Bahçe Manzaralý', 10800.00, 1300.00, 60),
(N'Swissôtel Büyük Efes Ýzmir', N'Izmir - Swiss Advantage Garden View Room', N'Bahçe Manzaralý', 11200.00, 1400.00, 30),
(N'Swissôtel Büyük Efes Ýzmir', N'Izmir - Swiss Executive Room Sea View', N'Deniz Manzaralý', 14900.00, 1800.00, 22),

(N'Key Hotel Ýzmir', N'Key - Standard Room', N'Deniz Manzaralý', 7900.00, 1000.00, 23),
(N'Key Hotel Ýzmir', N'Key - Superior Room', N'Deniz Manzaralý', 8400.00, 1000.00, 2),
(N'Key Hotel Ýzmir', N'Key - Family Suite', N'Deniz Manzaralý', 11900.00, 1500.00, 3),
(N'Key Hotel Ýzmir', N'Key - Executive Suite', N'Deniz Manzaralý', 14900.00, 1800.00, 1),
(N'Key Hotel Ýzmir', N'Key - Deluxe Suite', N'Deniz Manzaralý', 17900.00, 2000.00, 1),
(N'Key Hotel Ýzmir', N'Key - Grand Suite', N'Deniz Manzaralý', 23900.00, 2500.00, 1),

(N'DoubleTree by Hilton Çanakkale', N'Canakkale - King Deluxe Bed', N'Þehir Manzaralý', 6900.00, 900.00, 40),
(N'DoubleTree by Hilton Çanakkale', N'Canakkale - Twin Guest Room', N'Þehir Manzaralý', 7200.00, 900.00, 25),
(N'DoubleTree by Hilton Çanakkale', N'Canakkale - King Bedroom Suite', N'Deniz Manzaralý', 9900.00, 1500.00, 10),
(N'DoubleTree by Hilton Çanakkale', N'Canakkale - Accessible King Room', N'Þehir Manzaralý', 7600.00, 900.00, 9),

(N'Yazz Collective Fethiye', N'Yazz - Signature Suite', N'Deniz Manzaralý', 15900.00, 2000.00, 2),
(N'Yazz Collective Fethiye', N'Yazz - Comfort Suite', N'Bahçe Manzaralý', 12900.00, 1500.00, 7),
(N'Yazz Collective Fethiye', N'Yazz - Junior Suite', N'Bahçe Manzaralý', 11900.00, 1500.00, 4),
(N'Yazz Collective Fethiye', N'Yazz - Deluxe Room', N'Bahçe Manzaralý', 9900.00, 1200.00, 3),

(N'Adam & Eve - Adult Only', N'Design Room', N'Deniz Manzaralý', 18500.00, 2500.00, 215),
(N'Adam & Eve - Adult Only', N'Design Sea View Room', N'Deniz Manzaralý', 22500.00, 3000.00, 20),
(N'Adam & Eve - Adult Only', N'Design Suite', N'Deniz Manzaralý', 34500.00, 4000.00, 8),
(N'Adam & Eve - Adult Only', N'King Suite', N'Deniz Manzaralý', 58500.00, 6000.00, 2),
(N'Adam & Eve - Adult Only', N'Adam Villa', N'Havuz Manzaralý', 72500.00, 7500.00, 9),
(N'Adam & Eve - Adult Only', N'Eve Villa', N'Havuz Manzaralý', 87500.00, 9000.00, 10),

(N'Liberty Fabay', N'Standard Land View Room', N'Bahçe Manzaralý', 14500.00, 2000.00, 70),
(N'Liberty Fabay', N'Standard Sea View Room', N'Deniz Manzaralý', 16500.00, 2200.00, 50),
(N'Liberty Fabay', N'Swim Up Room', N'Havuz Manzaralý', 21500.00, 3000.00, 35),
(N'Liberty Fabay', N'Large Swim Up Room', N'Havuz Manzaralý', 26500.00, 3500.00, 20),
(N'Liberty Fabay', N'Family Land View Room', N'Bahçe Manzaralý', 24500.00, 3500.00, 30),
(N'Liberty Fabay', N'Family Sea View Room', N'Deniz Manzaralý', 28500.00, 4000.00, 25),
(N'Liberty Fabay', N'Private Suite Via Pool', N'Havuz Manzaralý', 46500.00, 5500.00, 10),
(N'Liberty Fabay', N'Duplex Lake House', N'Havuz Manzaralý', 62500.00, 7000.00, 10),

(N'Jiva Beach Resort', N'Standard Land View Room', N'Bahçe Manzaralý', 10500.00, 1500.00, 60),
(N'Jiva Beach Resort', N'Attic Room', N'Havuz Manzaralý', 12500.00, 1700.00, 20),
(N'Jiva Beach Resort', N'Family Land View Room', N'Bahçe Manzaralý', 16500.00, 2200.00, 25),
(N'Jiva Beach Resort', N'Suite Family Room', N'Havuz Manzaralý', 20500.00, 2700.00, 15),
(N'Jiva Beach Resort', N'Corner Suite', N'Deniz Manzaralý', 24500.00, 3200.00, 10),
(N'Jiva Beach Resort', N'King Suite', N'Deniz Manzaralý', 32500.00, 4000.00, 8),
(N'Jiva Beach Resort', N'Swim Up Room', N'Havuz Manzaralý', 21500.00, 3000.00, 20),
(N'Jiva Beach Resort', N'Honeymoon Room', N'Bahçe Manzaralý', 18500.00, 2500.00, 10),

(N'Yacht Classic Hotel', N'Classic Room', N'Bahçe Manzaralý', 9537.88, 0.00, 10),
(N'Yacht Classic Hotel', N'Junior Suite', N'Þehir Manzaralý', 13916.28, 0.00, 8),
(N'Yacht Classic Hotel', N'Loft Room', N'Deniz Manzaralý', 13916.28, 0.00, 6),
(N'Yacht Classic Hotel', N'Pool Suite', N'Bahçe Manzaralý', 9873.42, 0.00, 6),
(N'Yacht Classic Hotel', N'Water Villa', N'Deniz Manzaralý', 13916.28, 0.00, 4),

(N'Sundia by Liberty Ölüdeniz', N'Superior Double Room', N'Bahçe Manzaralý', 9500.00, 1200.00, 45),
(N'Sundia by Liberty Ölüdeniz', N'Superior Triple Room', N'Bahçe Manzaralý', 12500.00, 1500.00, 25),
(N'Sundia by Liberty Ölüdeniz', N'Superior Family Room', N'Bahçe Manzaralý', 16500.00, 2200.00, 20),
(N'Sundia by Liberty Ölüdeniz', N'Honeymoon Room', N'Bahçe Manzaralý', 18500.00, 2500.00, 8)
) s(HotelName, RoomTypeName, RoomViewName, Price, Extra, TotalRooms)
JOIN Hotels h ON h.Name = s.HotelName
JOIN RoomTypes rt ON rt.Name = s.RoomTypeName
JOIN RoomViews rv ON rv.Name = s.RoomViewName;
GO

-- =============================================================================
-- 7) HOTEL AMENITIES
-- =============================================================================
INSERT INTO HotelAmenities (HotelId, AmenityId)
SELECT DISTINCT h.Id, a.Id
FROM Hotels h
JOIN Amenities a ON a.Name IN (
    N'WiFi', N'Havuz', N'Restoran', N'Bar', N'Spa', N'Hamam', N'Sauna',
    N'Özel Plaj', N'Fitness', N'Fitness Merkezi', N'Çocuk Kulübü',
    N'Aquapark', N'Su Parký', N'Beach Club', N'Su Sporlarý', N'Concierge', N'Marina'
)
WHERE h.Name IN (
    N'Swissôtel Resort Bodrum Beach', N'Swissôtel Büyük Efes Ýzmir', N'Key Hotel Ýzmir',
    N'DoubleTree by Hilton Çanakkale', N'Yazz Collective Fethiye',
    N'Adam & Eve - Adult Only', N'Liberty Fabay', N'Jiva Beach Resort',
    N'Yacht Classic Hotel', N'Sundia by Liberty Ölüdeniz'
)
AND NOT EXISTS (
    SELECT 1 FROM HotelAmenities x WHERE x.HotelId = h.Id AND x.AmenityId = a.Id
);
GO

-- =============================================================================
-- 8) ROOM AMENITIES
-- =============================================================================
INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId)
SELECT DISTINCT hr.Id, ra.Id
FROM HotelRooms hr
JOIN Hotels h ON h.Id = hr.HotelId
JOIN RoomAmenities ra ON ra.Name IN (
    N'WiFi', N'Klima', N'Kahvaltý', N'Minibar', N'Smart TV', N'LCD TV',
    N'Kasa', N'Saç Kurutma Makinesi', N'Balkon',
    N'Çay/Kahve Makinesi', N'Çay-Kahve Seti', N'Bornoz & Terlik'
)
WHERE h.Name IN (
    N'Swissôtel Resort Bodrum Beach', N'Swissôtel Büyük Efes Ýzmir', N'Key Hotel Ýzmir',
    N'DoubleTree by Hilton Çanakkale', N'Yazz Collective Fethiye',
    N'Adam & Eve - Adult Only', N'Liberty Fabay', N'Jiva Beach Resort',
    N'Yacht Classic Hotel', N'Sundia by Liberty Ölüdeniz'
)
AND NOT EXISTS (
    SELECT 1 FROM HotelRoomAmenities x WHERE x.HotelRoomId = hr.Id AND x.RoomAmenityId = ra.Id
);

INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId)
SELECT DISTINCT hr.Id, ra.Id
FROM HotelRooms hr
JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
JOIN Hotels h ON h.Id = hr.HotelId
JOIN RoomAmenities ra ON ra.Name IN (N'Jakuzi', N'Özel Havuz', N'Bornoz')
WHERE h.Name IN (
    N'Swissôtel Resort Bodrum Beach', N'Swissôtel Büyük Efes Ýzmir', N'Key Hotel Ýzmir',
    N'DoubleTree by Hilton Çanakkale', N'Yazz Collective Fethiye',
    N'Adam & Eve - Adult Only', N'Liberty Fabay', N'Jiva Beach Resort',
    N'Yacht Classic Hotel', N'Sundia by Liberty Ölüdeniz'
)
AND rt.Name IN (
    N'Bodrum - Corner Sea View Suite', N'Bodrum - Signature Suite',
    N'Key - Deluxe Suite', N'Key - Grand Suite',
    N'Design Room', N'Design Suite', N'King Suite', N'Adam Villa', N'Eve Villa',
    N'Private Suite Via Pool', N'Pool Suite', N'Water Villa', N'Honeymoon Room'
)
AND NOT EXISTS (
    SELECT 1 FROM HotelRoomAmenities x WHERE x.HotelRoomId = hr.Id AND x.RoomAmenityId = ra.Id
);
GO

-- =============================================================================
-- 9) HOTEL IMAGES
-- =============================================================================
INSERT INTO HotelImages (HotelId, ImageUrl, IsMain, DisplayOrder)
SELECT h.Id, v.Url, v.IsMain, v.DisplayOrder
FROM Hotels h
JOIN (VALUES
(N'Adam & Eve - Adult Only', N'https://www.adamevehotel.com/uploads/orj/konak1.jpg', 1, 0),
(N'Adam & Eve - Adult Only', N'https://www.adamevehotel.com/uploads/orj/konak2.jpg', 0, 1),
(N'Adam & Eve - Adult Only', N'https://www.adamevehotel.com/uploads/orj/konak4.jpg', 0, 2),
(N'Adam & Eve - Adult Only', N'https://www.adamevehotel.com/uploads/orj/standartdesign.jpg', 0, 3),

(N'Liberty Fabay', N'https://www.libertyhotels.com/wp-content/uploads/2025/12/Havuz-scaled.jpg', 1, 0),
(N'Liberty Fabay', N'https://www.libertyhotels.com/wp-content/uploads/2025/12/Restoran--scaled.jpg', 0, 1),

(N'Jiva Beach Resort', N'https://jivabeachresort.com/dacegug/2024/02/1-12-scaled.jpg', 1, 0),
(N'Jiva Beach Resort', N'https://jivabeachresort.com/dacegug/2023/03/DJI_0409-1-1.jpg', 0, 1),
(N'Jiva Beach Resort', N'https://jivabeachresort.com/dacegug/2023/03/restaurant.jpg', 0, 2),
(N'Jiva Beach Resort', N'https://jivabeachresort.com/dacegug/2023/03/Yemek-2-1.png', 0, 3),

(N'Yacht Classic Hotel', N'https://yachtclassichotel.com/wp-content/uploads/2023/06/rezervasyon.png.webp', 1, 0),
(N'Yacht Classic Hotel', N'https://yachtclassichotel.com/wp-content/uploads/2023/06/classic-room-balcony-garden-view-1.jpg', 0, 1),
(N'Yacht Classic Hotel', N'https://yachtclassichotel.com/wp-content/uploads/2023/06/water-villa-swim-up-spa-bathtub-sea-view-1.jpg', 0, 2),

(N'Sundia by Liberty Ölüdeniz', N'https://www.sundiabyliberty.com/wp-content/uploads/2026/02/sundia-oludeniz-balayi-odasi-card.webp', 1, 0),
(N'Sundia by Liberty Ölüdeniz', N'https://www.sundiabyliberty.com/wp-content/uploads/2026/02/Sundia-by-Liberty-Oludeniz-Araxa-Bar.webp', 0, 1),
(N'Sundia by Liberty Ölüdeniz', N'https://www.sundiabyliberty.com/wp-content/uploads/2026/02/Sundia-by-Liberty-Oludeniz-Quiet-Pool.webp', 0, 2),
(N'Sundia by Liberty Ölüdeniz', N'https://www.sundiabyliberty.com/wp-content/uploads/2026/02/Sundia-by-Liberty-Oludeniz-Fitness.webp', 0, 3),

(N'Swissôtel Resort Bodrum Beach', N'https://images.unsplash.com/photo-1571896349842-33c89424de2d?auto=format&fit=crop&w=1200&q=80', 1, 0),
(N'Swissôtel Büyük Efes Ýzmir', N'https://images.unsplash.com/photo-1596178060810-72f53ce9a65c?auto=format&fit=crop&w=1200&q=80', 1, 0),
(N'Key Hotel Ýzmir', N'https://images.unsplash.com/photo-1600619754865-8fe927da0701?auto=format&fit=crop&w=1200&q=80', 1, 0),
(N'DoubleTree by Hilton Çanakkale', N'https://images.unsplash.com/photo-1549294413-26f195200c16?auto=format&fit=crop&w=1200&q=80', 1, 0),
(N'Yazz Collective Fethiye', N'https://images.unsplash.com/photo-1610641818989-c2051b5e2cfd?auto=format&fit=crop&w=1200&q=80', 1, 0)
) v(HotelName, Url, IsMain, DisplayOrder) ON v.HotelName = h.Name;
GO

-- =============================================================================
-- 10) ROOM IMAGES
-- =============================================================================
INSERT INTO RoomImages (HotelRoomId, ImageUrl, IsMain, DisplayOrder)
SELECT hr.Id, v.Url, 1, 0
FROM HotelRooms hr
JOIN Hotels h ON h.Id = hr.HotelId
JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
JOIN (VALUES
(N'Adam & Eve - Adult Only', N'Design Room', N'https://www.adamevehotel.com/uploads/orj/standartdesign.jpg'),
(N'Jiva Beach Resort', N'Standard Land View Room', N'https://jivabeachresort.com/dacegug/2024/02/1-12-scaled.jpg'),
(N'Yacht Classic Hotel', N'Classic Room', N'https://yachtclassichotel.com/wp-content/uploads/2023/06/classic-room-balcony-garden-view-1.jpg'),
(N'Yacht Classic Hotel', N'Water Villa', N'https://yachtclassichotel.com/wp-content/uploads/2023/06/water-villa-swim-up-spa-bathtub-sea-view-1.jpg'),
(N'Sundia by Liberty Ölüdeniz', N'Honeymoon Room', N'https://www.sundiabyliberty.com/wp-content/uploads/2026/02/sundia-oludeniz-balayi-odasi-card.webp')
) v(HotelName, RoomTypeName, Url) ON v.HotelName = h.Name AND v.RoomTypeName = rt.Name;

DECLARE @RoomFallback TABLE (rn INT IDENTITY(1,1), Url NVARCHAR(500));
INSERT INTO @RoomFallback (Url) VALUES
(N'https://images.unsplash.com/photo-1631049421450-348ccd7f8949?auto=format&fit=crop&w=900&q=80'),
(N'https://images.unsplash.com/photo-1572177215152-32f247303126?auto=format&fit=crop&w=900&q=80'),
(N'https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=900&q=80'),
(N'https://images.unsplash.com/photo-1631048730670-ff5cd0d08f15?auto=format&fit=crop&w=900&q=80');

;WITH MissingRoomImages AS
(
    SELECT hr.Id, ROW_NUMBER() OVER (ORDER BY hr.Id) AS rn
    FROM HotelRooms hr
    JOIN Hotels h ON h.Id = hr.HotelId
    WHERE h.Name IN (
        N'Swissôtel Resort Bodrum Beach', N'Swissôtel Büyük Efes Ýzmir', N'Key Hotel Ýzmir',
        N'DoubleTree by Hilton Çanakkale', N'Yazz Collective Fethiye',
        N'Adam & Eve - Adult Only', N'Liberty Fabay', N'Jiva Beach Resort',
        N'Yacht Classic Hotel', N'Sundia by Liberty Ölüdeniz'
    )
    AND NOT EXISTS (SELECT 1 FROM RoomImages ri WHERE ri.HotelRoomId = hr.Id)
)
INSERT INTO RoomImages (HotelRoomId, ImageUrl, IsMain, DisplayOrder)
SELECT m.Id, f.Url, 1, 0
FROM MissingRoomImages m
JOIN @RoomFallback f ON f.rn = ((m.rn - 1) % 4) + 1;
GO

PRINT N'10 otellik seed script basariyla tamamlandi.';
GO



USE HotelAppDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
GO

-- =============================================================================
-- 5 OTEL ETS FOTO SCRIPT
-- Sadece HotelImages ve RoomImages temizler, yeniden ekler.
-- Otel / oda / fiyat kayitlarini silmez.
-- =============================================================================

DECLARE @HotelIds TABLE (Id INT);

INSERT INTO @HotelIds (Id)
SELECT Id
FROM Hotels
WHERE Name IN (
    N'Adam & Eve - Adult Only',
    N'Adam & Eve +16',
    N'Adam Eve +16',
    N'Yacht Classic Hotel',
    N'Liberty Fabay',
    N'Jiva Beach Resort',
    N'Sundia by Liberty Ölüdeniz',
    N'Sundia By Liberty Ölüdeniz'
);

-- Once bu 5 otelin eski oda fotolarini sil
DELETE ri
FROM RoomImages ri
JOIN HotelRooms hr ON hr.Id = ri.HotelRoomId
WHERE hr.HotelId IN (SELECT Id FROM @HotelIds);

-- Sonra bu 5 otelin eski otel fotolarini sil
DELETE FROM HotelImages
WHERE HotelId IN (SELECT Id FROM @HotelIds);
GO

-- =============================================================================
-- OTEL FOTOÐRAFLARI
-- =============================================================================
INSERT INTO HotelImages (HotelId, ImageUrl, IsMain, DisplayOrder)
SELECT h.Id, v.ImageUrl, v.IsMain, v.DisplayOrder
FROM Hotels h
JOIN (VALUES
-- ADAM & EVE +16 - ETS
(N'Adam & Eve - Adult Only', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/33/Adam___Eve__16_20251203100141_4.jpg', 1, 0),
(N'Adam & Eve - Adult Only', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/33/Adam___Eve__16_20251203093629_21.jpg', 0, 1),
(N'Adam & Eve - Adult Only', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/33/Adam___Eve__16_20251203093632_24.jpg', 0, 2),
(N'Adam & Eve - Adult Only', N'https://images.etstur.com/imgproxy/files/images/hotelImages/test/TR/50454/l/royaladameve08.JPG', 0, 3),

-- YACHT CLASSIC HOTEL - ETS / ODAMAX CDN
(N'Yacht Classic Hotel', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/xujfdktbtaz3aesknmgh.jpg', 1, 0),
(N'Yacht Classic Hotel', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Yacht_Classic_Hotel_202512291448170_Ek1.jpg', 0, 1),
(N'Yacht Classic Hotel', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/j1haqtnykyofkqnvpz9w.jpg', 0, 2),
(N'Yacht Classic Hotel', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Yacht_Classic_Hotel_202512291448186_Ek3.jpg', 0, 3),
(N'Yacht Classic Hotel', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Yacht_Classic_Hotel_202512291451131_Ek8.jpg', 0, 4),

-- LIBERTY FABAY - ETS
(N'Liberty Fabay', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/939/Liberty_Fabay_20250414154928_2.jpg', 1, 0),
(N'Liberty Fabay', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/939/Liberty_Fabay_20250414154928_3.jpg', 0, 1),
(N'Liberty Fabay', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/939/Liberty_Fabay_20250414154928_1.jpg', 0, 2),

-- JIVA BEACH RESORT - ETS
(N'Jiva Beach Resort', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/51237/l/Jiva-Beach-Resort-Genel-388007.jpg', 1, 0),
(N'Jiva Beach Resort', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/51237/l/Jiva-Beach-Resort-Genel-388040.jpg', 0, 1),
(N'Jiva Beach Resort', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/51237/l/Jiva-Beach-Resort-Genel-388006.jpg', 0, 2),
(N'Jiva Beach Resort', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/51237/l/Jiva-Beach-Resort-Genel-388012.jpg', 0, 3),

-- SUNDIA BY LIBERTY ÖLÜDENÝZ - ETS
(N'Sundia by Liberty Ölüdeniz', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/504670/Sundia_By_Liberty_Oludeniz_20240822143534_1.png', 1, 0),
(N'Sundia by Liberty Ölüdeniz', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/504670/Sundia_By_Liberty_Oludeniz_20240822143534_3.png', 0, 1),
(N'Sundia by Liberty Ölüdeniz', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/504670/Sundia_By_Liberty_Oludeniz_20240822143534_0.jpg', 0, 2),
(N'Sundia by Liberty Ölüdeniz', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/504670/Sundia_By_Liberty_Oludeniz_20240822143534_9.jpg', 0, 3)
) v(HotelName, ImageUrl, IsMain, DisplayOrder)
    ON v.HotelName = h.Name;
GO

-- =============================================================================
-- ODA FOTOÐRAFLARI
-- Her otelin odalarina kendi ETS foto havuzundan 4'er foto ekler.
-- =============================================================================
DECLARE @RoomPhotoPool TABLE
(
    HotelName NVARCHAR(200),
    ImageUrl NVARCHAR(500),
    DisplayOrder INT
);

INSERT INTO @RoomPhotoPool (HotelName, ImageUrl, DisplayOrder)
VALUES
-- ADAM & EVE ODA FOTOÐRAFLARI
(N'Adam & Eve - Adult Only', N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/33/Adam___Eve__16_20251203100043_0.jpg', 0),
(N'Adam & Eve - Adult Only', N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/33/Adam___Eve__16_20251203095936_0.jpg', 1),
(N'Adam & Eve - Adult Only', N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/33/Adam___Eve__16_20251203095851_1.jpg', 2),
(N'Adam & Eve - Adult Only', N'https://images.etstur.com/imgproxy/resize/800x450/files/images/hotelImages/etsrooms/33/Adam___Eve__16_20251203100141_7.jpg', 3),

-- YACHT CLASSIC ODA FOTOÐRAFLARI
(N'Yacht Classic Hotel', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9UG9vbC1TdWl0ZS0xMQ_20210625153856.JPG', 0),
(N'Yacht Classic Hotel', N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9UG9vbC1TdWl0ZS0x_20210625153856.JPG', 1),
(N'Yacht Classic Hotel', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/Yacht_Classic_Hotel_202512291459059_Property_6_EW.jpg', 2),
(N'Yacht Classic Hotel', N'https://images.odamax.com/imgproxy/img/1024x768/odamax/image/upload/vblvba4uucdyvpjik29g.jpg', 3),

-- LIBERTY FABAY ODA FOTOÐRAFLARI
(N'Liberty Fabay', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/214153/m/Liberty-Fabay-Oda-350830.jpg', 0),
(N'Liberty Fabay', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/214153/m/Liberty-Fabay-Oda-350833.jpg', 1),
(N'Liberty Fabay', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/214153/m/Liberty-Fabay-Oda-350832.jpg', 2),
(N'Liberty Fabay', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/214153/m/Liberty-Fabay-Oda-350834.jpg', 3),

-- JIVA BEACH RESORT ODA FOTOÐRAFLARI
(N'Jiva Beach Resort', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/51237/l/Jiva-Beach-Resort-Genel-388040.jpg', 0),
(N'Jiva Beach Resort', N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/51237/l/Jiva-Beach-Resort-Genel-388006.jpg', 1),
(N'Jiva Beach Resort', N'https://jivabeachresort.com/dacegug/2023/03/standart-oda-.jpg', 2),
(N'Jiva Beach Resort', N'https://jivabeachresort.com/dacegug/2024/02/1-12-scaled.jpg', 3),

-- SUNDIA BY LIBERTY ÖLÜDENÝZ ODA FOTOÐRAFLARI
(N'Sundia by Liberty Ölüdeniz', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/504670/Sundia_By_Liberty_Oludeniz_20240822143534_1.png', 0),
(N'Sundia by Liberty Ölüdeniz', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/504670/Sundia_By_Liberty_Oludeniz_20240822143534_3.png', 1),
(N'Sundia by Liberty Ölüdeniz', N'https://images.etstur.com/imgproxy/resize/2560x1440/files/images/hotelImages/etsrooms/504670/Sundia_By_Liberty_Oludeniz_20240822143534_0.jpg', 2),
(N'Sundia by Liberty Ölüdeniz', N'https://www.sundiabyliberty.com/wp-content/uploads/2026/02/sundia-oludeniz-balayi-odasi-card.webp', 3);

INSERT INTO RoomImages (HotelRoomId, ImageUrl, IsMain, DisplayOrder)
SELECT hr.Id,
       p.ImageUrl,
       CASE WHEN p.DisplayOrder = 0 THEN 1 ELSE 0 END,
       p.DisplayOrder
FROM HotelRooms hr
JOIN Hotels h ON h.Id = hr.HotelId
JOIN @RoomPhotoPool p ON p.HotelName = h.Name
WHERE h.Name IN (
    N'Adam & Eve - Adult Only',
    N'Yacht Classic Hotel',
    N'Liberty Fabay',
    N'Jiva Beach Resort',
    N'Sundia by Liberty Ölüdeniz'
);
GO

-- =============================================================================
-- KONTROL
-- =============================================================================
SELECT h.Name, COUNT(hi.Id) AS HotelFotoSayisi
FROM Hotels h
LEFT JOIN HotelImages hi ON hi.HotelId = h.Id
WHERE h.Name IN (
    N'Adam & Eve - Adult Only',
    N'Yacht Classic Hotel',
    N'Liberty Fabay',
    N'Jiva Beach Resort',
    N'Sundia by Liberty Ölüdeniz'
)
GROUP BY h.Name
ORDER BY h.Name;

SELECT h.Name, COUNT(ri.Id) AS OdaFotoSayisi
FROM Hotels h
JOIN HotelRooms hr ON hr.HotelId = h.Id
LEFT JOIN RoomImages ri ON ri.HotelRoomId = hr.Id
WHERE h.Name IN (
    N'Adam & Eve - Adult Only',
    N'Yacht Classic Hotel',
    N'Liberty Fabay',
    N'Jiva Beach Resort',
    N'Sundia by Liberty Ölüdeniz'
)
GROUP BY h.Name
ORDER BY h.Name;
GO

PRINT N'5 otelin ETS fotoðraflarý baþarýyla eklendi.';
GO