-- =============================================
-- HotelApp Veritabani Semasi
-- Otel Rezervasyon Sistemi - Final Projesi
-- Tarih: 2026-04-06
-- =============================================

CREATE DATABASE HotelAppDB;
GO
USE HotelAppDB;
GO

-- =============================================
-- 1. HOTELS - Otel bilgileri
-- =============================================
CREATE TABLE Hotels (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(2000),
    Address NVARCHAR(500) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    Country NVARCHAR(100) NOT NULL DEFAULT 'Turkiye',
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(200),
    Latitude DECIMAL(9,6),
    Longitude DECIMAL(9,6),
    AvgRating DECIMAL(3,1),
    StarRating TINYINT NOT NULL CHECK (StarRating BETWEEN 1 AND 5),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL
);
GO

-- =============================================
-- 2. ROOM TYPES - Oda tipleri
-- Ornek: Single, Double, Suite, Family, Deluxe
-- =============================================
CREATE TABLE RoomTypes(
Id INT PRIMARY KEY IDENTITY(1,1),
Name NVARCHAR(100) NOT NULL ,      
Description NVARCHAR(500) NOT NULL,
MaxGuests INT NOT NULL DEFAULT 2,    ---- Max misafir sayısı

CONSTRAINT UQ_RoomTypes_Name UNIQUE(Name),
CONSTRAINT CHK_RoomTypes_MaxGuests CHECK (MaxGuests > 0)
);
GO

-- =============================================
-- 3. HOTEL ROOMS - Hotel <-> RoomType N-N ara tablo
-- Bir otelde birden fazla oda tipi, bir oda tipi birden fazla otelde
-- Fiyat ve oda sayisi bu tabloda (otele ozel)
-- =============================================
CREATE TABLE HotelRooms(
    Id INT PRIMARY KEY IDENTITY(1,1),
    HotelId INT NOT NULL,               
    RoomTypeId INT NOT NULL,            
    PricePerNight DECIMAL(10,2) NOT NULL CHECK (PricePerNight >= 0), 
    TotalRooms INT NOT NULL,        
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT CHK_HotelRooms_Total CHECK (TotalRooms > 0),
    CONSTRAINT UQ_HotelRooms UNIQUE (HotelId, RoomTypeId),

    CONSTRAINT FK_HotelRooms_Hotels
    FOREIGN KEY(HotelId) REFERENCES Hotels(Id),

    CONSTRAINT FK_HotelRooms_RoomTypes
    FOREIGN KEY(RoomTypeId) REFERENCES RoomTypes(Id)
);
GO

-- =============================================
-- 4.USERS - Kullanicilar
-- Kullanici hesap, kimlik dogrulama ve guvenlik bilgilerini tutar.
-- 13 kolon: temel bilgiler + email dogrulama + hesap kilitleme
-- =============================================

CREATE TABLE Users
(
Id INT PRIMARY KEY IDENTITY (1,1),             -- Otomatik artan benzersiz kimlik
FirstName NVARCHAR(100) NOT NULL,              -- Kullanicinin adi
LastName NVARCHAR(100) NOT NULL,               -- Kullanicinin soyadi
Email NVARCHAR(200) NOT NULL,                  -- E-posta adresi (giris icin kullanilir)
PasswordHash NVARCHAR(500) NOT NULL,           -- Sifrenin hash'lenmis hali (duz metin saklanmaz)
Phone NVARCHAR(20) NOT NULL,                   -- Telefon numarasi
IsActive BIT NOT NULL DEFAULT 1,               -- Hesap aktif mi? (soft delete icin)
IsEmailVerified BIT NOT NULL DEFAULT 0,        -- E-posta dogrulandi mi?
EmailVerificationToken NVARCHAR(100) NULL,     -- Dogrulama linki icin GUID token
VerificationTokenExpiry DATETIME2 NULL,        -- Token son gecerlilik tarihi
FailedLoginAttempts INT NOT NULL DEFAULT 0,    -- Basarisiz giris deneme sayaci
LockoutEndTime DATETIME2 NULL,                 -- Hesap kilitleme bitis zamani
CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),-- Hesap olusturulma tarihi

CONSTRAINT UQ_Users_Email UNIQUE (Email)       -- Ayni e-posta tekrar edemez
);
GO

-- =============================================
-- 5.ROLES - Roller
-- Sistemdeki rol tanimlarini tutar (Admin, User).
-- =============================================

CREATE TABLE Roles
(
Id INT PRIMARY KEY IDENTITY(1,1),              -- Otomatik artan benzersiz kimlik
Name NVARCHAR(50) NOT NULL,                    -- Rol adi (orn: Admin, User)

CONSTRAINT UQ_Roles_Name UNIQUE (Name)         -- Ayni rol adi tekrar edemez
);
GO

-- =============================================
-- 6.USERROLES - Kullanici Rolleri (N-N ara tablo)
-- Bir kullanicinin birden fazla rolu olabilir.
-- Composite PK: (UserId, RoleId)
-- =============================================

CREATE TABLE UserRoles
(
UserId INT NOT NULL,                           -- Kullanici kimlik numarasi
RoleId INT NOT NULL,                           -- Rol kimlik numarasi

CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),  -- Birlesik anahtar (ayni kullanici-rol cifti tekrar edemez)
CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserId) REFERENCES Users(Id),  -- Users tablosuna baglanti
CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleId) REFERENCES Roles(Id)   -- Roles tablosuna baglanti
);
GO

-- =============================================
-- 7. RESERVATIONS - Rezervasyonlar
-- =============================================

CREATE TABLE Reservations(
    Id INT PRIMARY KEY IDENTITY(1,1),
    HotelRoomId INT NOT NULL,                            
    UserId INT NOT NULL,                                
    CheckInDate DATE NOT NULL,                           
    CheckOutDate DATE NOT NULL,                          
    GuestCount INT NOT NULL DEFAULT 1  CHECK (GuestCount > 0),      
    TotalPrice DECIMAL(10,2) NOT NULL CHECK (TotalPrice >= 0), 
    PaymentMethod NVARCHAR(50) NOT NULL,                 
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',                        
    CreatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,
    CONSTRAINT CHK_Dates CHECK (CheckOutDate > CheckInDate),
    CONSTRAINT CHK_Status CHECK (Status IN ('Pending','Confirmed','Cancelled','Completed')),

    CONSTRAINT FK_Reservation_Users
    FOREIGN KEY(UserId) REFERENCES Users(Id),

    CONSTRAINT FK_Reservation_HotelRoomId
    FOREIGN KEY(HotelRoomId) REFERENCES HotelRooms(Id)
);
GO

-- =============================================
-- 8. REVIEWS - Degerlendirmeler
-- =============================================

CREATE TABLE Reviews(
Id INT PRIMARY KEY IDENTITY(1,1),
HotelId INT NOT NULL,    --- Hotel Kimlik Numarası
UserId INT NOT NULL,    --- Kullanıcı Kimlik Numarası
Rating TINYINT NOT NULL CHECK (Rating >= 1 AND Rating <= 5), --- Yorum puanları 1 ila 5 arasında
Comment NVARCHAR(1000),                 --- Yorum
CreatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,    --- Oluşturma tarihi

CONSTRAINT UQ_Reviews_UserHotel UNIQUE (UserId, HotelId),

CONSTRAINT FK_Reviews_Hotels
FOREIGN KEY(HotelId) REFERENCES Hotels(Id),

CONSTRAINT FK_Reviews_Users
FOREIGN KEY(UserId) REFERENCES Users(Id)
);
GO


-- =============================================
-- 9. HOTEL IMAGES - Otel gorselleri
-- =============================================
CREATE TABLE HotelImages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HotelId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsMain BIT NOT NULL DEFAULT 0,
    DisplayOrder INT NOT NULL DEFAULT 0,

    CONSTRAINT FK_HotelImages_Hotels
    FOREIGN KEY (HotelId) REFERENCES Hotels(Id)
);
GO


-- =============================================
-- 10. HOTEL AMENITIES - Hotel <-> Amenity N-N ara tablo
-- =============================================
CREATE TABLE Amenities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Icon NVARCHAR(50)
);
GO

-- =============================================
-- 11. AMENITIES - Otel ozellikleri
-- Ornek: WiFi, Havuz, Spa, Otopark, Restoran, Gym
-- =============================================
CREATE TABLE HotelAmenities (
    HotelId INT NOT NULL,
    AmenityId INT NOT NULL,
    PRIMARY KEY (HotelId, AmenityId),
    CONSTRAINT FK_HotelAmenities_Hotels
        FOREIGN KEY (HotelId) REFERENCES Hotels(Id),
    CONSTRAINT FK_HotelAmenities_Amenities
        FOREIGN KEY (AmenityId) REFERENCES Amenities(Id)
);
GO


-- =============================================
-- 12.LOGS - Sistem Kayitlari (Logger Singleton Pattern)
-- Uygulama islem ve hata loglarini tutar.
-- UserId NULL olabilir (anonim islemler icin).
-- FK: ON DELETE SET NULL (kullanici silinirse log kaybi olmaz)
-- =============================================

CREATE TABLE Logs
(
Id INT PRIMARY KEY IDENTITY(1,1),              -- Otomatik artan benzersiz kimlik
[Timestamp] DATETIME2 NOT NULL DEFAULT GETDATE(), -- Olay zamani
[Level] NVARCHAR(20) NOT NULL,                 -- Log seviyesi: 'Info', 'Warning', 'Error'
[Message] NVARCHAR(2000) NOT NULL,             -- Log mesaji (ne oldu?)
UserId INT NULL,                               -- Islemi yapan kullanici (anonim icin NULL)
UserEmail NVARCHAR(200) NULL,                  -- Denormalize email (hizli okuma icin)
Controller NVARCHAR(100) NULL,                 -- Hangi controller (orn: AuthController)
[Action] NVARCHAR(200) NULL,                   -- Hangi endpoint (orn: POST /api/auth/login)
IpAddress NVARCHAR(45) NULL,                   -- Istemci IP adresi (IPv6 destekli)
ExceptionDetail NVARCHAR(4000) NULL,           -- Hata detayi (sadece Error seviyesi)

CONSTRAINT FK_Logs_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL  -- Kullanici silinirse log kaybolmaz, NULL olur
);
GO

-- =============================================
-- INDEXES - Performans icin index tanimlari
-- Sik sorgulanan alanlarda arama hizini arttirir.
-- =============================================

--- Şehire göre otel sorgulama
CREATE INDEX IX_Hotels_City ON  Hotels(City);

--- Otel odaları hızlı bulmak için
CREATE INDEX IX_HotelRooms_HotelId ON HotelRooms(HotelId);
CREATE INDEX IX_HotelRooms_RoomTypeId ON HotelRooms(RoomTypeId);

---Rezervasyonlarda tarih aralığına göre sorgulama
CREATE INDEX IX_Reservations_Dates ON Reservations(CheckInDate,CheckOutDate);
CREATE INDEX IX_Reservations_UserId ON Reservations(UserId);
CREATE INDEX IX_Reservations_HotelRoomId ON Reservations(HotelRoomId);

--- Yoruma göre filtreleme
CREATE INDEX IX_Reviews_HotelId ON Reviews(HotelId);

-- Users: Email ile giris sorgularini hizlandirir
CREATE INDEX IX_Users_Email ON Users(Email);

-- Logs: Zamana gore siralama (en yeni log ustte)
CREATE INDEX IX_Logs_Timestamp ON Logs([Timestamp] DESC);

-- Logs: Seviyeye gore filtreleme (Info/Warning/Error)
CREATE INDEX IX_Logs_Level ON Logs([Level]);

-- Logs: Kullaniciya gore filtreleme
CREATE INDEX IX_Logs_UserId ON Logs(UserId);
GO


--=====================================================================
--- VIEW: Otelin adı, şehir, yıldız, ortalama puan, yorum sayısı
--=====================================================================

CREATE VIEW vw_HotelSummary AS 
SELECT
h.Id,
h.Name,
h.City,
h.StarRating,
ISNULL(AVG(CAST(r.Rating AS DECIMAL(3,1))),0) AS AvgRating,
COUNT(r.Id) AS ReviewCount,
(SELECT MIN(hr.PricePerNight) FROM HotelRooms hr WHERE hr.HotelId= h.Id AND hr.IsActive = 1) AS MinPrice,
h.IsActive
FROM Hotels h
LEFT JOIN Reviews r ON r.HotelId = h.Id
GROUP BY h.Id, h.Name, h.City, h.StarRating, h.IsActive;
GO

--=====================================================================
--- VIEW: Belirli bir tarih aralığı boş olan odalar
--=====================================================================

CREATE VIEW vw_RoomDetails AS
SELECT
hr.Id AS HotelRoomId,
h.Id AS HotelId,
h.Name AS HotelName,
h.City,
rt.Name AS RoomTypeName,
rt.MaxGuests,
hr.PricePerNight,
hr.TotalRooms,
hr.IsActive
FROM HotelRooms hr
INNER JOIN Hotels h ON h.Id = hr.HotelId
INNER JOIN RoomTypes rt ON rt.Id= hr.RoomTypeId
WHERE hr.IsActive = 1 AND h.IsActive= 1;
GO


--=====================================================================
-- STORED PROCEDURE: Şehir ve tarih aralığına göre müsait odalar
--=====================================================================

CREATE PROCEDURE sp_SearchAvailableRooms
@City NVARCHAR(100),
@CheckInDate DATE,
@CheckOutDate DATE,
@GuestCount INT = 1
AS
BEGIN
SET NOCOUNT ON;
SELECT
rd.HotelRoomId,
rd.HotelId,
rd.HotelName,
rd.City,
rd.RoomTypeName,
rd.MaxGuests,
rd.PricePerNight,
rd.TotalRooms,
ISNULL(booked.ReservedCount, 0) AS ReservedCount,
rd.TotalRooms - ISNULL(booked.ReservedCount, 0) AS AvailableRooms
FROM vw_RoomDetails rd
LEFT JOIN (
SELECT
HotelRoomId,
Count(*) AS ReservedCount
FROM Reservations 
WHERE Status IN('Pending', 'Confirmed')
AND CheckInDate < @CheckOutDate
AND CheckOutDate > @CheckInDate
GROUP BY HotelRoomId 
) booked ON booked.HotelRoomId = rd.HotelRoomId
WHERE rd.City = @City
AND rd.MaxGuests >= @GuestCount
AND (rd.TotalRooms - ISNULL (booked.ReservedCount, 0)) > 0
ORDER BY rd.PricePerNight;
END;
GO



--=====================================================================
-- STORED PROCEDURE: Rezervasyon Oluşturur (müsaitlik durumu ile)
--=====================================================================

CREATE PROCEDURE sp_MakeReservation
@UserId INT,
@HotelRoomId INT,
@CheckInDate DATE,
@CheckOutDate DATE,
@GuestCount INT,
@PaymentMethod NVARCHAR(50),
@ReservationId INT OUTPUT,
@ResultMessage NVARCHAR(200) OUTPUT
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRANSACTION;
--- 1.Oda bilgisini al
DECLARE @PricePerNight DECIMAL(10,2), @TotalRooms INT, @MaxGuests INT;
SELECT
@PricePerNight = hr.PricePerNight,
@TotalRooms = hr.TotalRooms,
@MaxGuests = rt.MaxGuests
FROM HotelRooms hr
INNER JOIN RoomTypes rt ON rt.Id =hr.RoomTypeId
WHERE hr.Id = @HotelRoomId AND hr.IsActive = 1;

IF @PricePerNight IS NULL
BEGIN
SET @ResultMessage = 'Oda bulunamad� veya aktif de�il.';
SET @ReservationId = 0;
ROLLBACK;
RETURN;
END
--- 2.Misafir sayısı kontrolü
IF @GuestCount > @MaxGuests
BEGIN 
SET @ResultMessage = 'Misafir say�s� oda kapastesini a��yor';
SET @ReservationId = 0;
ROLLBACK;
RETURN;
END
--- 3.Müsaitlik kontrolü
DECLARE @ReservedCount INT;
SELECT @ReservedCount = Count(*)
FROM Reservations
WHERE HotelRoomId = @HotelRoomId
AND Status IN ('Pending', 'Confirmed')
AND CheckInDate < @CheckOutDate
AND CheckOutDate > @CheckInDate;

IF (@TotalRooms - @ReservedCount) <= 0
BEGIN
SET @ResultMessage = 'Bu tarih aral���nda musait oda yok';
SET @ReservationId = 0;
ROLLBACK;
RETURN;
END
--- 4.Toplam fiyat hesaplama
DECLARE @Nights INT = DATEDIFF(DAY, @CheckInDate , @CheckOutDate);
DECLARE @TotalPrice DECIMAL(10,2) = @Nights * @PricePerNight;
--- 5.Rezervasyon oluşturma
INSERT INTO Reservations (UserId, HotelRoomId, CheckInDate, CheckOutDate, GuestCount, TotalPrice, PaymentMethod, Status)
VALUES (@UserId, @HotelRoomId, @CheckInDate, @CheckOutDate, @GuestCount, @TotalPrice, @PaymentMethod, 'Confirmed');
SET @ReservationId = SCOPE_IDENTITY();
SET @ResultMessage = 'Rezervasyon ba�ar�yla olu�turuldu.';
COMMIT;
END;
GO

--=====================================================================
-- TRIGGER: Review eklendiginde/guncellendiginde otelin ortalama puanini
-- Hotels tablosunda tutmak icin (performans optimizasyonu)
-- AvgRating kolonu Hotels CREATE TABLE icerisinde tanimli
--=====================================================================
CREATE TRIGGER trg_UpdateHotelRating
ON Reviews
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
SET NOCOUNT ON;
--- Etkilenen otel ID'lerini bul 
DECLARE @AffectedHotels TABLE(HotelId INT);
INSERT INTO @AffectedHotels (HotelId)
SELECT DISTINCT HotelId FROM inserted
UNION
SELECT DISTINCT HotelId FROM deleted;
--- Ortalama puanı güncelleme
UPDATE h
SET h.AvgRating = ISNULL(
(SELECT AVG(CAST(R.Rating AS DECIMAL(3,1)))
FROM Reviews r
WHERE r.HotelId = h.Id), 0)
FROM Hotels h
INNER JOIN @AffectedHotels ah ON ah.HotelId = h.Id;
END;
GO


INSERT INTO Hotels 
(Name, Description, Address, City, Country, StarRating, Phone, Email)
VALUES
('Swissotel The Bosphorus', 'Bogaz manzarali luks otel', 'Visnezade Mah. Acisu Sok.', 'Istanbul', 'Turkiye', 5, '02123263200', 'info@swissotel.com'),

('Maxx Royal Belek Golf Resort', 'Ultra luks tatil koyu', 'Iskele Mevkii', 'Antalya', 'Turkiye', 5, '02427101000', 'info@maxxroyal.com'),

('Divan Ankara', 'Is oteli konseptinde', 'Guniz Sok.', 'Ankara', 'Turkiye', 5, '03124666000', 'info@divan.com'),

('Wyndham Grand Izmir Ozdilek', 'Deniz manzarali sehir oteli', 'Inciralti Cad.', 'Izmir', 'Turkiye', 5, '02322921000', 'info@wyndham.com'),

('Hilton Bursa Convention Center', 'Modern sehir oteli', 'Istanbul Cad.', 'Bursa', 'Turkiye', 5, '02242110000', 'info@hilton.com'),

('Ramada Plaza Trabzon', 'Denize yakin konforlu otel', 'Yalincak Mah.', 'Trabzon', 'Turkiye', 5, '04623350000', 'info@ramada.com'),

('Dedeman Konya', 'Merkezi konumda otel', 'Beysehir Yolu', 'Konya', 'Turkiye', 5, '03322210000', 'info@dedeman.com'),

('DoubleTree by Hilton Gaziantep', 'Sehir merkezinde luks otel', 'Incilipinar Mah.', 'Gaziantep', 'Turkiye', 5, '03422110000', 'info@hilton.com'),

('Novotel Kayseri', 'Erciyes manzarali modern otel', 'Kocasinan Bulvari', 'Kayseri', 'Turkiye', 4, '03522220000', 'info@novotel.com'),

('Ramada Resort Kusadasi', 'Tatil koyu konsepti', 'Kusadasi', 'Aydin', 'Turkiye', 5, '02566180000', 'info@ramada.com');


INSERT INTO RoomTypes (Name, Description, MaxGuests )
VALUES
('Single', 'Tek kisilik oda', 1),
('Double', 'Iki kisilik oda', 2),
('Suite', 'Genis ve luks oda', 4),
('Family', 'Aile odasi', 5),
('Deluxe', 'Konforlu deluxe oda', 3),
('Business', 'Is seyahatine uygun oda', 2),
('Kral', 'En luks kral dairesi', 6);

DECLARE @hotelId INT = 1;

WHILE @hotelId <= 10
BEGIN
    INSERT INTO HotelRooms (HotelId, RoomTypeId, PricePerNight, TotalRooms)
    VALUES
    (@hotelId, 1, 800, 10),   -- Single
    (@hotelId, 2, 1200, 10),  -- Double
    (@hotelId, 3, 2500, 5),   -- Suite
    (@hotelId, 4, 2000, 5),   -- Family
    (@hotelId, 5, 1800, 6),   -- Deluxe
    (@hotelId, 6, 1500, 7),   -- Business
    (@hotelId, 7, 5000, 2);   -- Kral

    SET @hotelId = @hotelId + 1;
END;
GO

INSERT INTO Amenities (Name, Icon)
VALUES
('WiFi', 'wifi'),
('Havuz', 'pool'),
('Spa', 'spa'),
('Otopark', 'car'),
('Restoran', 'food');

INSERT INTO Roles (Name) VALUES ('Admin'), ('User');

-- Admin
INSERT INTO Users 
(FirstName, LastName, Email, PasswordHash, Phone, IsActive, IsEmailVerified)
VALUES
('Sevde', 'Admin', 'furuncusvd@outlook.com', 'ADM�N1234.!', '5551111111', 1, 1);

-- Normal User
INSERT INTO Users 
(FirstName, LastName, Email, PasswordHash, Phone, IsActive, IsEmailVerified)
VALUES
('Test', 'User', 'user@test.com', '1234', '5552222222', 1, 1);

INSERT INTO UserRoles (UserId, RoleId)
VALUES
(1, 1),  -- User 1 = Admin
(2, 2);  -- User 2 = User
