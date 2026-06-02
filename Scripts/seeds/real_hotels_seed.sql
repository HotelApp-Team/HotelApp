-- =============================================================================
-- GERCEK OTELLER SEED SCRIPT (v2: encoding fix + 5 otel gorseli + 4 oda gorseli)
-- 5 gercek otel: Liberty Lykia, Belcekiz Beach Club, Mandarin Oriental Bodrum,
--                The Bodrum EDITION, Lujo Hotel Bodrum
-- Veriler web arastirmasi ile toplandi. Gorseller Unsplash (telifsiz, gercek foto).
-- Gorseller otomatik havuz atamasiyla eklenir: her otele 5, her odaya 4 gorsel.
-- Bu script "reset + reseed" mantigindadir: bu 5 oteli her calismada silip yeniden
-- ekler; boylece tekrar calistirilabilir ve encoding daima dogru olur.
--
-- ONEMLI: UTF-8 calistir!  sqlcmd -f 65001 -i real_hotels_seed.sql
--         (yoksa Turkce karakterler bozulur: Olu -> Ã–lÃ¼)
-- =============================================================================

USE HotelAppDB;
GO
-- Filtered unique index'ler (RoomView) icin QUOTED_IDENTIFIER ON sarttir.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
GO

-- =============================================================================
-- 0) TEMIZLIK: Bu 5 otel ve bagli tum kayitlari sil (reseed icin)
-- =============================================================================
DECLARE @ids TABLE (Id INT);
INSERT INTO @ids (Id)
SELECT Id FROM Hotels
WHERE Name IN (N'Liberty Lykia', N'Belcekiz Beach Club', N'Mandarin Oriental, Bodrum',
               N'The Bodrum EDITION', N'Lujo Hotel Bodrum');

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
-- 1) YENI ODA TIPLERI (mevcut: Single, Double, Suite, Family, Deluxe, Business, Kral)
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Deniz Manzaralı Oda')
    INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Deniz Manzaralı Oda', N'Denize bakan balkonlu konforlu oda', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Junior Suite')
    INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Junior Suite', N'Geniş oturma alanlı junior suit', 2);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Villa')
    INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Villa', N'Özel havuzlu ve bahçeli müstakil villa', 6);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Bungalow')
    INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Bungalow', N'Doğayla iç içe tek katlı bungalov', 4);
IF NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = N'Penthouse Suit')
    INSERT INTO RoomTypes (Name, Description, MaxGuests) VALUES (N'Penthouse Suit', N'En üst katta panoramik manzaralı lüks suit', 4);
GO

-- =============================================================================
-- 2) YENI OTEL OLANAKLARI (Amenities)
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Özel Plaj')        INSERT INTO Amenities (Name, Icon) VALUES (N'Özel Plaj', N'umbrella');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Fitness Merkezi')  INSERT INTO Amenities (Name, Icon) VALUES (N'Fitness Merkezi', N'heart-pulse');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Bar')              INSERT INTO Amenities (Name, Icon) VALUES (N'Bar', N'cup-straw');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Su Parkı')         INSERT INTO Amenities (Name, Icon) VALUES (N'Su Parkı', N'water');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Tenis Kortu')      INSERT INTO Amenities (Name, Icon) VALUES (N'Tenis Kortu', N'trophy');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Çocuk Kulübü')     INSERT INTO Amenities (Name, Icon) VALUES (N'Çocuk Kulübü', N'emoji-smile');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Sauna')            INSERT INTO Amenities (Name, Icon) VALUES (N'Sauna', N'thermometer-half');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Türk Hamamı')      INSERT INTO Amenities (Name, Icon) VALUES (N'Türk Hamamı', N'droplet');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Beach Club')       INSERT INTO Amenities (Name, Icon) VALUES (N'Beach Club', N'sun');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Su Sporları')      INSERT INTO Amenities (Name, Icon) VALUES (N'Su Sporları', N'life-preserver');
IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Concierge')        INSERT INTO Amenities (Name, Icon) VALUES (N'Concierge', N'bell');
GO

-- =============================================================================
-- 3) YENI ODA OLANAKLARI (RoomAmenities) - mevcut: sadece Klima
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'WiFi')                  INSERT INTO RoomAmenities (Name, Icon) VALUES (N'WiFi', N'wifi');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Minibar')               INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Minibar', N'cup-straw');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Smart TV')              INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Smart TV', N'tv');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Kasa')                  INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Kasa', N'safe2');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Saç Kurutma Makinesi')  INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Saç Kurutma Makinesi', N'wind');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Balkon')                INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Balkon', N'door-open');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Jakuzi')                INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Jakuzi', N'droplet-half');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Çay/Kahve Makinesi')    INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Çay/Kahve Makinesi', N'cup-hot');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Bornoz & Terlik')       INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Bornoz & Terlik', N'basket');
IF NOT EXISTS (SELECT 1 FROM RoomAmenities WHERE Name = N'Özel Havuz')            INSERT INTO RoomAmenities (Name, Icon) VALUES (N'Özel Havuz', N'water');
GO

-- =============================================================================
-- 4) OTELLER + ODALAR + OLANAKLAR (gorseller asagida toplu eklenir)
-- =============================================================================
DECLARE @h INT, @r INT;

-- Oda olanak setleri tekrar kullanim icin tek satirlik WHERE IN listeleri:
--  TEMEL : WiFi, Klima, Smart TV, Kasa
--  STD   : + Minibar, Saç Kurutma Makinesi, Balkon
--  KONFOR: + Çay/Kahve Makinesi
--  LUX   : + Jakuzi, Bornoz & Terlik
--  VILLA : + Özel Havuz

-- ============ OTEL 1: LIBERTY LYKIA (Fethiye) ============
INSERT INTO Hotels (Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, StarRating, IsActive, CreatedAt)
VALUES (N'Liberty Lykia', N'Ölüdeniz Kıdrak koyunda; su parkı, özel plajı ve altı restoranıyla ultra her şey dahil 5 yıldızlı tatil köyü.',
        N'Faralya Mah. Kıdrak Sok. No:6, Ölüdeniz', N'Fethiye', N'Türkiye', N'02526170077', N'info@libertylykia.com', 36.548600, 29.118000, 5, 1, GETDATE());
SET @h = SCOPE_IDENTITY();
INSERT INTO HotelAmenities (HotelId, AmenityId) SELECT @h, Id FROM Amenities WHERE Name IN (N'WiFi',N'Havuz',N'Spa',N'Restoran',N'Su Parkı',N'Özel Plaj',N'Tenis Kortu',N'Fitness Merkezi',N'Bar',N'Çocuk Kulübü');

INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Double'), 14000, 2500, 60, 1, 5);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Family'), 22000, 3500, 40, 1, 4);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Balkon');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deluxe'), 26000, 4000, 30, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deniz Manzaralı Oda'), 30000, 4500, 25, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Balkon',N'Çay/Kahve Makinesi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Junior Suite'), 42000, 5000, 15, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Suite'), 65000, 6000, 8, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi',N'Bornoz & Terlik');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Single'), 9000, 0, 20, 1, 5);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Smart TV',N'Kasa');

-- ============ OTEL 2: BELCEKIZ BEACH CLUB (Fethiye) ============
INSERT INTO Hotels (Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, StarRating, IsActive, CreatedAt)
VALUES (N'Belcekiz Beach Club', N'Ölüdeniz plajının hemen önünde, her şey dahil konseptiyle ödüllü 5 yıldızlı sahil oteli.',
        N'Belcekız Mah. Ölüdeniz Cad. No:10', N'Fethiye', N'Türkiye', N'02526170400', N'info@belcekiz.com', 36.550500, 29.121000, 5, 1, GETDATE());
SET @h = SCOPE_IDENTITY();
INSERT INTO HotelAmenities (HotelId, AmenityId) SELECT @h, Id FROM Amenities WHERE Name IN (N'WiFi',N'Havuz',N'Spa',N'Restoran',N'Özel Plaj',N'Fitness Merkezi',N'Bar',N'Sauna',N'Türk Hamamı',N'Tenis Kortu');

INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Double'), 11000, 2000, 70, 1, 5);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Family'), 18000, 3000, 45, 1, 4);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Balkon');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deluxe'), 22000, 3500, 30, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deniz Manzaralı Oda'), 25000, 4000, 25, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Balkon',N'Çay/Kahve Makinesi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Suite'), 48000, 5000, 10, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi',N'Bornoz & Terlik');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Single'), 8000, 0, 15, 1, 5);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Smart TV',N'Kasa');

-- ============ OTEL 3: MANDARIN ORIENTAL, BODRUM (Bodrum) ============
INSERT INTO Hotels (Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, StarRating, IsActive, CreatedAt)
VALUES (N'Mandarin Oriental, Bodrum', N'Göltürkbükü Cennet Koyu''nda iki özel plaja sahip, plunge havuzlu süitleriyle ultra lüks 5 yıldızlı resort.',
        N'Göltürkbükü Mah. 314. Sok. No:12/1', N'Bodrum', N'Türkiye', N'02523111888', N'mobod-reservations@mohg.com', 37.118000, 27.365000, 5, 1, GETDATE());
SET @h = SCOPE_IDENTITY();
INSERT INTO HotelAmenities (HotelId, AmenityId) SELECT @h, Id FROM Amenities WHERE Name IN (N'WiFi',N'Havuz',N'Spa',N'Restoran',N'Özel Plaj',N'Fitness Merkezi',N'Bar',N'Beach Club',N'Su Sporları',N'Concierge');

INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deluxe'), 32000, 5000, 30, 1, 5);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Bornoz & Terlik');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deniz Manzaralı Oda'), 40000, 6000, 20, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Junior Suite'), 60000, 7000, 15, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi',N'Özel Havuz');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Suite'), 95000, 9000, 8, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi',N'Özel Havuz',N'Bornoz & Terlik');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Villa'), 180000, 12000, 4, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi',N'Özel Havuz',N'Bornoz & Terlik');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Penthouse Suit'), 250000, 15000, 2, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi',N'Özel Havuz',N'Bornoz & Terlik');

-- ============ OTEL 4: THE BODRUM EDITION (Bodrum) ============
INSERT INTO Hotels (Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, StarRating, IsActive, CreatedAt)
VALUES (N'The Bodrum EDITION', N'Yalıkavak''ta sonsuzluk havuzu ve özel plaj kulübüyle modern tasarımlı 5 yıldızlı butik lüks otel.',
        N'Dirmil Mah. Balyek Cad. No:5A', N'Bodrum', N'Türkiye', N'02523113131', N'bodrum@editionhotels.com', 37.105000, 27.290000, 5, 1, GETDATE());
SET @h = SCOPE_IDENTITY();
INSERT INTO HotelAmenities (HotelId, AmenityId) SELECT @h, Id FROM Amenities WHERE Name IN (N'WiFi',N'Havuz',N'Spa',N'Restoran',N'Özel Plaj',N'Fitness Merkezi',N'Bar',N'Beach Club',N'Sauna',N'Türk Hamamı');

INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deluxe'), 30000, 4500, 35, 1, 4);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deniz Manzaralı Oda'), 38000, 5500, 25, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Balkon',N'Çay/Kahve Makinesi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Junior Suite'), 55000, 7000, 15, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Suite'), 90000, 9000, 8, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi',N'Özel Havuz',N'Bornoz & Terlik');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Villa'), 200000, 13000, 3, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi',N'Özel Havuz',N'Bornoz & Terlik');

-- ============ OTEL 5: LUJO HOTEL BODRUM (Bodrum) ============
INSERT INTO Hotels (Name, Description, Address, City, Country, Phone, Email, Latitude, Longitude, StarRating, IsActive, CreatedAt)
VALUES (N'Lujo Hotel Bodrum', N'Güvercinlik''te yedi havuz, su parkı ve dokuz restoranıyla sanat ve eğlence temalı 5 yıldızlı ultra her şey dahil resort.',
        N'Çomça Mevkii Sok. No:10, Güvercinlik', N'Bodrum', N'Türkiye', N'02525221060', N'info@lujohotel.com', 37.050000, 27.420000, 5, 1, GETDATE());
SET @h = SCOPE_IDENTITY();
INSERT INTO HotelAmenities (HotelId, AmenityId) SELECT @h, Id FROM Amenities WHERE Name IN (N'WiFi',N'Havuz',N'Spa',N'Restoran',N'Su Parkı',N'Özel Plaj',N'Fitness Merkezi',N'Bar',N'Çocuk Kulübü',N'Su Sporları',N'Beach Club');

INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deluxe'), 28000, 4000, 40, 1, 5);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Family'), 35000, 5000, 30, 1, 4);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Balkon');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Deniz Manzaralı Oda'), 33000, 4500, 30, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Balkon',N'Çay/Kahve Makinesi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Bungalow'), 45000, 5500, 20, 1, 3);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Suite'), 70000, 8000, 10, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi');
INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms, IsActive, RoomViewId) VALUES (@h, (SELECT Id FROM RoomTypes WHERE Name=N'Villa'), 160000, 12000, 5, 1, 1);
SET @r=SCOPE_IDENTITY(); INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId) SELECT @r, Id FROM RoomAmenities WHERE Name IN (N'WiFi',N'Klima',N'Minibar',N'Smart TV',N'Kasa',N'Saç Kurutma Makinesi',N'Balkon',N'Çay/Kahve Makinesi',N'Jakuzi',N'Özel Havuz',N'Bornoz & Terlik');
GO

-- =============================================================================
-- 5) HOTEL GORSELLERI - her otele 5 gorsel (havuzdan otomatik, dongusel)
-- =============================================================================
DECLARE @hpool TABLE (rn INT IDENTITY(1,1), base NVARCHAR(200));
INSERT INTO @hpool (base) VALUES
(N'https://images.unsplash.com/photo-1571896349842-33c89424de2d'),(N'https://images.unsplash.com/photo-1596178060810-72f53ce9a65c'),
(N'https://images.unsplash.com/photo-1600619754865-8fe927da0701'),(N'https://images.unsplash.com/photo-1718942899965-4fc10607d805'),
(N'https://images.unsplash.com/photo-1549294413-26f195200c16'),(N'https://images.unsplash.com/photo-1610641818989-c2051b5e2cfd'),
(N'https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9'),(N'https://images.unsplash.com/photo-1742171046853-0961eabdc7d4'),
(N'https://images.unsplash.com/photo-1596178065887-1198b6148b2b'),(N'https://images.unsplash.com/photo-1619631428089-813ef0fca73b'),
(N'https://images.unsplash.com/photo-1569335468888-1d3e4a5a3595'),(N'https://images.unsplash.com/photo-1561501900-3701fa6a0864'),
(N'https://images.unsplash.com/photo-1645447556616-e9d1c52e8037'),(N'https://images.unsplash.com/photo-1586739807089-5f978d168cfa'),
(N'https://images.unsplash.com/photo-1562593017-589f766dea08'),(N'https://images.unsplash.com/photo-1711114377434-5ea6f68ad0c7'),
(N'https://images.unsplash.com/photo-1711114435495-76503f9f3181'),(N'https://images.unsplash.com/photo-1711114378532-bab7d55311f3'),
(N'https://images.unsplash.com/photo-1711114378455-b1f479d94a19'),(N'https://images.unsplash.com/photo-1722409195473-d322e99621e3'),
(N'https://images.unsplash.com/photo-1629129924298-42d89875f115'),(N'https://images.unsplash.com/photo-1747336755360-c4830d819af1'),
(N'https://images.unsplash.com/photo-1747336755438-e822d83e9383'),(N'https://images.unsplash.com/photo-1747336755349-59df86ba5794'),
(N'https://images.unsplash.com/photo-1747336755296-a9e715350b32'),(N'https://images.unsplash.com/photo-1680422273918-5f1d443272a5'),
(N'https://images.unsplash.com/photo-1641150557653-e4c409426e59'),(N'https://images.unsplash.com/photo-1747336755334-c7bd5a5e104d'),
(N'https://images.unsplash.com/photo-1619680939776-4eadd78709a4'),(N'https://images.unsplash.com/photo-1747336755468-2c12efa216c8'),
(N'https://images.unsplash.com/photo-1747336755289-89f02e694606'),(N'https://images.unsplash.com/photo-1631049307264-da0ec9d70304');
DECLARE @hcnt INT = (SELECT COUNT(*) FROM @hpool);

;WITH NH AS (
    SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS seq FROM Hotels
    WHERE Name IN (N'Liberty Lykia', N'Belcekiz Beach Club', N'Mandarin Oriental, Bodrum', N'The Bodrum EDITION', N'Lujo Hotel Bodrum')
)
INSERT INTO HotelImages (HotelId, ImageUrl, IsMain, DisplayOrder)
SELECT nh.Id, hp.base + N'?auto=format&fit=crop&w=1200&q=80', CASE WHEN s.k = 0 THEN 1 ELSE 0 END, s.k
FROM NH nh
CROSS APPLY (VALUES (0),(1),(2),(3),(4)) AS s(k)
JOIN @hpool hp ON hp.rn = (((nh.seq - 1) * 5 + s.k) % @hcnt) + 1;
GO

-- =============================================================================
-- 6) ODA GORSELLERI - her odaya 4 gorsel (havuzdan otomatik, dongusel)
-- =============================================================================
DECLARE @rpool TABLE (rn INT IDENTITY(1,1), base NVARCHAR(200));
INSERT INTO @rpool (base) VALUES
(N'https://images.unsplash.com/photo-1631049421450-348ccd7f8949'),(N'https://images.unsplash.com/photo-1572177215152-32f247303126'),
(N'https://images.unsplash.com/photo-1662841540530-2f04bb3291e8'),(N'https://images.unsplash.com/photo-1630888728969-5e43f55a7a18'),
(N'https://images.unsplash.com/photo-1618773928121-c32242e63f39'),(N'https://images.unsplash.com/photo-1629140727571-9b5c6f6267b4'),
(N'https://images.unsplash.com/photo-1576354302919-96748cb8299e'),(N'https://images.unsplash.com/photo-1631048730670-ff5cd0d08f15'),
(N'https://images.unsplash.com/photo-1621891333819-00c206ec8994'),(N'https://images.unsplash.com/photo-1711059985570-4c32ed12a12c'),
(N'https://images.unsplash.com/photo-1590675560125-0d832b9d719e'),(N'https://images.unsplash.com/photo-1631048835184-3f0ceda91b75'),
(N'https://images.unsplash.com/photo-1777180249046-abf7d640e0d9'),(N'https://images.unsplash.com/photo-1777169794972-12095816073b'),
(N'https://images.unsplash.com/photo-1776763018970-9fdf66bd4666'),(N'https://images.unsplash.com/photo-1761039265583-9489b4246454'),
(N'https://images.unsplash.com/photo-1759223607861-f0ef3e617739'),(N'https://images.unsplash.com/photo-1731336478850-6bce7235e320'),
(N'https://images.unsplash.com/photo-1685592437742-3b56edb46b15'),(N'https://images.unsplash.com/photo-1759223198981-661cadbbff36'),
(N'https://images.unsplash.com/photo-1775866914767-7e4646f2481a'),(N'https://images.unsplash.com/photo-1560703652-f3fd178338b7'),
(N'https://images.unsplash.com/photo-1578898887932-dce23a595ad4'),(N'https://images.unsplash.com/photo-1561049933-c8fbef47b329'),
(N'https://images.unsplash.com/photo-1690935986319-c11e6cae84f7'),(N'https://images.unsplash.com/photo-1678978866819-306ed8608e7f'),
(N'https://images.unsplash.com/photo-1578898886225-c7c894047899'),(N'https://images.unsplash.com/photo-1635428335190-95f20a76cb47'),
(N'https://images.unsplash.com/photo-1595161695996-f746349f4945'),(N'https://images.unsplash.com/photo-1552858725-693709cc17c7'),
(N'https://images.unsplash.com/photo-1611971263575-ab653c31a5a9'),(N'https://images.unsplash.com/photo-1690185389446-82a067939ad6'),
(N'https://images.unsplash.com/photo-1663659506588-5d5f24c3eb4a'),(N'https://images.unsplash.com/photo-1663659508974-4f178e40d8d7'),
(N'https://images.unsplash.com/photo-1519868343531-805e97cbda3e'),(N'https://images.unsplash.com/photo-1641870538417-c83e621d1425'),
(N'https://images.unsplash.com/photo-1646974400439-8472d58bb19e'),(N'https://images.unsplash.com/photo-1677553512940-f79af72efd1b');
DECLARE @rcnt INT = (SELECT COUNT(*) FROM @rpool);

;WITH NR AS (
    SELECT hr.Id, ROW_NUMBER() OVER (ORDER BY hr.Id) AS seq
    FROM HotelRooms hr
    WHERE hr.HotelId IN (SELECT Id FROM Hotels WHERE Name IN (N'Liberty Lykia', N'Belcekiz Beach Club', N'Mandarin Oriental, Bodrum', N'The Bodrum EDITION', N'Lujo Hotel Bodrum'))
)
INSERT INTO RoomImages (HotelRoomId, ImageUrl, IsMain, DisplayOrder)
SELECT nr.Id, rp.base + N'?auto=format&fit=crop&w=900&q=80', CASE WHEN s.k = 0 THEN 1 ELSE 0 END, s.k
FROM NR nr
CROSS APPLY (VALUES (0),(1),(2),(3)) AS s(k)
JOIN @rpool rp ON rp.rn = (((nr.seq - 1) * 4 + s.k) % @rcnt) + 1;
GO

PRINT '5 gercek otel, odalari, 5er otel gorseli ve 4er oda gorseli basariyla eklendi (encoding: UTF-8).';
GO
