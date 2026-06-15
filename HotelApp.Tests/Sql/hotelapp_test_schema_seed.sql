USE [master];
GO

-- QUOTED_IDENTIFIER ON: filtered unique index'lerin (UQ_HotelRooms_NoView, _WithView)
-- olusturulmasi icin gereklidir. sqlcmd default'unda OFF gelir.
SET QUOTED_IDENTIFIER ON;
GO

IF DB_ID(N'$(DatabaseName)') IS NOT NULL
BEGIN
    ALTER DATABASE [$(DatabaseName)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END;
GO

CREATE DATABASE [$(DatabaseName)];
GO

USE [$(DatabaseName)];
GO

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

CREATE TABLE RoomTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    MaxGuests INT NOT NULL DEFAULT 2,
    CONSTRAINT UQ_RoomTypes_Name UNIQUE(Name),
    CONSTRAINT CHK_RoomTypes_MaxGuests CHECK (MaxGuests > 0)
);
GO

-- Oda manzara (RoomView) lookup tablosu - 2026-05-19 migration ile eklendi.
CREATE TABLE RoomViews (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Icon NVARCHAR(50) NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_RoomViews_Name UNIQUE (Name)
);
GO

CREATE TABLE HotelRooms (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HotelId INT NOT NULL,
    RoomTypeId INT NOT NULL,
    PricePerNight DECIMAL(10,2) NOT NULL CHECK (PricePerNight >= 0),
    ExtraGuestPricePerNight DECIMAL(10,2) NOT NULL DEFAULT 0 CHECK (ExtraGuestPricePerNight >= 0),
    TotalRooms INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    -- Manzara FK (NULL'a izin var, mevcut kayit etkilenmesin).
    RoomViewId INT NULL,
    CONSTRAINT CHK_HotelRooms_Total CHECK (TotalRooms > 0),
    CONSTRAINT FK_HotelRooms_Hotels FOREIGN KEY(HotelId) REFERENCES Hotels(Id),
    CONSTRAINT FK_HotelRooms_RoomTypes FOREIGN KEY(RoomTypeId) REFERENCES RoomTypes(Id),
    CONSTRAINT FK_HotelRooms_RoomViews FOREIGN KEY(RoomViewId) REFERENCES RoomViews(Id) ON DELETE SET NULL
);
GO

-- Unique constraint manzara dahil ama NULL-aware: iki filtered index ile bolundu.
-- NULL manzara icin (HotelId, RoomTypeId) tek olsun; NOT NULL manzara icin uclu kombinasyon tek.
CREATE UNIQUE INDEX UQ_HotelRooms_NoView
    ON HotelRooms(HotelId, RoomTypeId)
    WHERE RoomViewId IS NULL;
GO

CREATE UNIQUE INDEX UQ_HotelRooms_WithView
    ON HotelRooms(HotelId, RoomTypeId, RoomViewId)
    WHERE RoomViewId IS NOT NULL;
GO

CREATE TABLE RoomAmenities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Icon NVARCHAR(50),
    CONSTRAINT UQ_RoomAmenities_Name UNIQUE (Name)
);
GO

CREATE TABLE HotelRoomAmenities (
    HotelRoomId INT NOT NULL,
    RoomAmenityId INT NOT NULL,
    CONSTRAINT PK_HotelRoomAmenities PRIMARY KEY (HotelRoomId, RoomAmenityId),
    CONSTRAINT FK_HotelRoomAmenities_HotelRooms FOREIGN KEY (HotelRoomId) REFERENCES HotelRooms(Id),
    CONSTRAINT FK_HotelRoomAmenities_RoomAmenities FOREIGN KEY (RoomAmenityId) REFERENCES RoomAmenities(Id)
);
GO

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsEmailVerified BIT NOT NULL DEFAULT 0,
    EmailVerificationToken NVARCHAR(100) NULL,
    VerificationTokenExpiry DATETIME2 NULL,
    PasswordResetToken NVARCHAR(100) NULL,
    PasswordResetTokenExpiry DATETIME2 NULL,
    PasswordResetRequestedAt DATETIME2 NULL,
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    LockoutEndTime DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);
GO

CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO

CREATE TABLE Reservations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HotelRoomId INT NOT NULL,
    UserId INT NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    GuestCount INT NOT NULL DEFAULT 1 CHECK (GuestCount > 0),
    TotalPrice DECIMAL(10,2) NOT NULL CHECK (TotalPrice >= 0),
    PaymentMethod NVARCHAR(50) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    CreatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,
    CONSTRAINT CHK_Dates CHECK (CheckOutDate > CheckInDate),
    CONSTRAINT CHK_Status CHECK (Status IN ('Confirmed','Cancelled','Completed')),
    CONSTRAINT FK_Reservation_Users FOREIGN KEY(UserId) REFERENCES Users(Id),
    CONSTRAINT FK_Reservation_HotelRoomId FOREIGN KEY(HotelRoomId) REFERENCES HotelRooms(Id)
);
GO

CREATE TABLE Reviews (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HotelId INT NOT NULL,
    UserId INT NOT NULL,
    Rating TINYINT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(1000),
    CreatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,
    IsApproved BIT NOT NULL DEFAULT 0,
    ApprovedAt DATETIME2 NULL,
    CONSTRAINT UQ_Reviews_UserHotel UNIQUE (UserId, HotelId),
    CONSTRAINT FK_Reviews_Hotels FOREIGN KEY(HotelId) REFERENCES Hotels(Id),
    CONSTRAINT FK_Reviews_Users FOREIGN KEY(UserId) REFERENCES Users(Id)
);
GO

CREATE TABLE HotelImages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HotelId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsMain BIT NOT NULL DEFAULT 0,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_HotelImages_Hotels FOREIGN KEY (HotelId) REFERENCES Hotels(Id)
);
GO

CREATE TABLE RoomImages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HotelRoomId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsMain BIT NOT NULL DEFAULT 0,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_RoomImages_HotelRooms FOREIGN KEY (HotelRoomId) REFERENCES HotelRooms(Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_RoomImages_HotelRoomId ON RoomImages(HotelRoomId);
GO

CREATE TABLE Amenities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Icon NVARCHAR(50),
    CONSTRAINT UQ_Amenities_Name UNIQUE (Name)
);
GO

CREATE TABLE HotelAmenities (
    HotelId INT NOT NULL,
    AmenityId INT NOT NULL,
    CONSTRAINT PK_HotelAmenities PRIMARY KEY (HotelId, AmenityId),
    CONSTRAINT FK_HotelAmenities_Hotels FOREIGN KEY (HotelId) REFERENCES Hotels(Id),
    CONSTRAINT FK_HotelAmenities_Amenities FOREIGN KEY (AmenityId) REFERENCES Amenities(Id)
);
GO

CREATE TABLE Logs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [Level] NVARCHAR(20) NOT NULL,
    [Message] NVARCHAR(2000) NOT NULL,
    UserId INT NULL,
    UserEmail NVARCHAR(200) NULL,
    Controller NVARCHAR(100) NULL,
    [Action] NVARCHAR(200) NULL,
    IpAddress NVARCHAR(45) NULL,
    ExceptionDetail NVARCHAR(4000) NULL,
    CONSTRAINT FK_Logs_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL
);
GO

CREATE INDEX IX_Hotels_City ON Hotels(City);
CREATE INDEX IX_HotelRooms_HotelId ON HotelRooms(HotelId);
CREATE INDEX IX_HotelRooms_RoomTypeId ON HotelRooms(RoomTypeId);
CREATE INDEX IX_HotelRooms_RoomViewId ON HotelRooms(RoomViewId);
CREATE INDEX IX_HotelRoomAmenities_RoomAmenityId ON HotelRoomAmenities(RoomAmenityId);
CREATE INDEX IX_Reservations_Dates ON Reservations(CheckInDate, CheckOutDate);
CREATE INDEX IX_Reservations_UserId ON Reservations(UserId);
CREATE INDEX IX_Reservations_HotelRoomId ON Reservations(HotelRoomId);
CREATE INDEX IX_Reviews_HotelId ON Reviews(HotelId);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_PasswordResetToken ON Users(PasswordResetToken) WHERE PasswordResetToken IS NOT NULL;
CREATE INDEX IX_Logs_Timestamp ON Logs([Timestamp] DESC);
CREATE INDEX IX_Logs_Level ON Logs([Level]);
CREATE INDEX IX_Logs_UserId ON Logs(UserId);
GO

CREATE VIEW vw_HotelSummary AS
SELECT
    h.Id,
    h.Name,
    h.City,
    h.StarRating,
    ISNULL(AVG(CAST(r.Rating AS DECIMAL(3,1))), 0) AS AvgRating,
    COUNT(r.Id) AS ReviewCount,
    (SELECT MIN(hr.PricePerNight) FROM HotelRooms hr WHERE hr.HotelId = h.Id AND hr.IsActive = 1) AS MinPrice,
    h.IsActive
FROM Hotels h
LEFT JOIN Reviews r ON r.HotelId = h.Id
GROUP BY h.Id, h.Name, h.City, h.StarRating, h.IsActive;
GO

CREATE VIEW vw_RoomDetails AS
SELECT
    hr.Id AS HotelRoomId,
    h.Id AS HotelId,
    h.Name AS HotelName,
    hi.ImageUrl AS MainImageUrl,
    h.City,
    rt.Name AS RoomTypeName,
    rt.MaxGuests,
    hr.PricePerNight,
    hr.ExtraGuestPricePerNight,
    hr.TotalRooms,
    hr.IsActive,
    -- Manzara kolonlari (LEFT JOIN cunku RoomViewId NULL olabilir).
    hr.RoomViewId,
    rv.Name AS RoomViewName,
    rv.Icon AS RoomViewIcon
FROM HotelRooms hr
INNER JOIN Hotels h ON h.Id = hr.HotelId
INNER JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
LEFT JOIN RoomViews rv ON rv.Id = hr.RoomViewId
WHERE hr.IsActive = 1 AND h.IsActive = 1;
GO

CREATE VIEW vw_ReservationDetails AS
SELECT
    r.Id AS ReservationId,
    r.CheckInDate,
    r.CheckOutDate,
    r.GuestCount,
    r.TotalPrice,
    r.PaymentMethod,
    r.Status,
    r.CreatedAt,
    u.Id AS UserId,
    u.FirstName,
    u.LastName,
    u.Email,
    h.Id AS HotelId,
    h.Name AS HotelName,
    h.City,
    h.Country,
    rt.Id AS RoomTypeId,
    rt.Name AS RoomTypeName,
    rt.MaxGuests,
    hr.PricePerNight
FROM Reservations r
INNER JOIN Users u ON r.UserId = u.Id
INNER JOIN HotelRooms hr ON r.HotelRoomId = hr.Id
INNER JOIN Hotels h ON hr.HotelId = h.Id
INNER JOIN RoomTypes rt ON hr.RoomTypeId = rt.Id
OUTER APPLY (
    SELECT TOP (1) himg.ImageUrl
    FROM HotelImages himg
    WHERE himg.HotelId = h.Id
    ORDER BY himg.IsMain DESC, himg.DisplayOrder, himg.Id
) hi;
GO

CREATE OR ALTER PROCEDURE sp_SearchAvailableRooms
    @HotelId INT = NULL,
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
        rd.ExtraGuestPricePerNight,
        rd.TotalRooms,
        rd.RoomViewName,
        rd.RoomViewIcon,
        ISNULL(booked.ReservedCount, 0) AS ReservedCount,
        rd.TotalRooms - ISNULL(booked.ReservedCount, 0) AS AvailableRooms
    FROM vw_RoomDetails rd
    LEFT JOIN (
        SELECT HotelRoomId, COUNT(*) AS ReservedCount
        FROM Reservations
        WHERE Status = 'Confirmed'
          AND CheckInDate < @CheckOutDate
          AND CheckOutDate > @CheckInDate
        GROUP BY HotelRoomId
    ) booked ON booked.HotelRoomId = rd.HotelRoomId
    WHERE rd.City = @City
      AND rd.MaxGuests >= @GuestCount
      AND (@HotelId IS NULL OR rd.HotelId = @HotelId)
      AND (rd.TotalRooms - ISNULL(booked.ReservedCount, 0)) > 0
    ORDER BY rd.PricePerNight;
END;
GO

CREATE OR ALTER PROCEDURE sp_MakeReservation
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
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE
            @PricePerNight DECIMAL(10,2),
            @ExtraGuestPricePerNight DECIMAL(10,2),
            @TotalRooms INT,
            @MaxGuests INT;

        SELECT
            @PricePerNight = hr.PricePerNight,
            @ExtraGuestPricePerNight = hr.ExtraGuestPricePerNight,
            @TotalRooms = hr.TotalRooms,
            @MaxGuests = rt.MaxGuests
        FROM HotelRooms hr WITH (UPDLOCK, HOLDLOCK)
        INNER JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
        WHERE hr.Id = @HotelRoomId
          AND hr.IsActive = 1;

        IF @PricePerNight IS NULL
        BEGIN
            SET @ResultMessage = N'Oda bulunamadı veya aktif değil.';
            SET @ReservationId = 0;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @GuestCount <= 0
        BEGIN
            SET @ResultMessage = N'Misafir sayısı 1 veya daha fazla olmalıdır.';
            SET @ReservationId = 0;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @GuestCount > @MaxGuests
        BEGIN
            SET @ResultMessage = N'Misafir sayısı oda kapasitesini aşıyor.';
            SET @ReservationId = 0;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @CheckInDate < CAST(GETDATE() AS DATE)
        BEGIN
            SET @ResultMessage = N'Giriş tarihi bugünden eski olamaz.';
            SET @ReservationId = 0;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @CheckOutDate <= @CheckInDate
        BEGIN
            SET @ResultMessage = N'Çıkış tarihi giriş tarihinden sonra olmalıdır.';
            SET @ReservationId = 0;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        DECLARE @ReservedCount INT;

        SELECT @ReservedCount = COUNT(*)
        FROM Reservations
        WHERE HotelRoomId = @HotelRoomId
          AND Status = 'Confirmed'
          AND CheckInDate < @CheckOutDate
          AND CheckOutDate > @CheckInDate;

        IF @ReservedCount >= @TotalRooms
        BEGIN
            SET @ResultMessage = N'Bu tarih aralığında müsait oda yok.';
            SET @ReservationId = 0;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        DECLARE @Nights INT = DATEDIFF(DAY, @CheckInDate, @CheckOutDate);

        DECLARE @BillableExtraGuests INT =
            CASE WHEN @GuestCount > 1 THEN @GuestCount - 1 ELSE 0 END;

        DECLARE @TotalPrice DECIMAL(10,2) =
            (@Nights * @PricePerNight)
            + (@Nights * @ExtraGuestPricePerNight * @BillableExtraGuests);

        INSERT INTO Reservations
            (UserId, HotelRoomId, CheckInDate, CheckOutDate, GuestCount, TotalPrice, PaymentMethod, Status)
        VALUES
            (@UserId, @HotelRoomId, @CheckInDate, @CheckOutDate, @GuestCount, @TotalPrice, @PaymentMethod, 'Confirmed');

        SET @ReservationId = SCOPE_IDENTITY();
        SET @ResultMessage = N'Rezervasyon başarıyla oluşturuldu.';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SET @ReservationId = 0;
        SET @ResultMessage = ERROR_MESSAGE();
    END CATCH;
END;
GO


CREATE TRIGGER trg_UpdateHotelRating
ON Reviews
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AffectedHotels TABLE(HotelId INT);

    INSERT INTO @AffectedHotels (HotelId)
    SELECT DISTINCT HotelId FROM inserted
    UNION
    SELECT DISTINCT HotelId FROM deleted;

    UPDATE h
    SET h.AvgRating = ISNULL((
        SELECT AVG(CAST(r.Rating AS DECIMAL(3,1)))
        FROM Reviews r
        WHERE r.HotelId = h.Id
          AND r.IsApproved = 1
    ), 0)
    FROM Hotels h
    INNER JOIN @AffectedHotels ah ON ah.HotelId = h.Id;
END;
GO

INSERT INTO Hotels (Name, Description, Address, City, Country, StarRating, Phone, Email)
VALUES
('CI Istanbul Hotel', 'Test hotel in Istanbul', 'Test Address 1', 'Istanbul', 'Turkiye', 5, '02120000001', 'istanbul@testhotel.com'),
('CI Antalya Resort', 'Test resort in Antalya', 'Test Address 2', 'Antalya', 'Turkiye', 4, '02420000001', 'antalya@testhotel.com');
GO

INSERT INTO RoomTypes (Name, Description, MaxGuests)
VALUES
('Single', 'Single test room', 1),
('Double', 'Double test room', 2),
('Suite', 'Suite test room', 3),
('Family', 'Family test room', 4),
('Deluxe', 'Deluxe test room', 3),
('Business', 'Business test room', 2),
('Kral', 'Kral test room', 4);
GO

-- RoomViews seed (production seed ile ayni - 6 manzara).
INSERT INTO RoomViews (Name, Icon, DisplayOrder, IsActive)
VALUES
(N'Deniz Manzaralı', N'bi-water', 1, 1),
(N'Şehir Manzaralı', N'bi-buildings', 2, 1),
(N'Dağ Manzaralı', N'bi-tree', 3, 1),
(N'Havuz Manzaralı', N'bi-droplet', 4, 1),
(N'Bahçe Manzaralı', N'bi-flower2', 5, 1),
(N'Manzara Yok', N'bi-dash-circle', 99, 1);
GO

DECLARE @hotelId INT;

SELECT @hotelId = Id
FROM Hotels
WHERE Name = 'CI Istanbul Hotel';

INSERT INTO HotelRooms
    (HotelId, RoomTypeId, PricePerNight, ExtraGuestPricePerNight, TotalRooms)
VALUES
    (@hotelId, 1, 800, 0, 10),
    (@hotelId, 2, 1200, 250, 10),
    (@hotelId, 3, 2500, 500, 5),
    (@hotelId, 4, 2000, 350, 5),
    (@hotelId, 5, 1800, 300, 6),
    (@hotelId, 6, 1500, 300, 7),
    (@hotelId, 7, 5000, 750, 2);
GO

INSERT INTO RoomAmenities (Name, Icon)
VALUES
('Wi-Fi', 'wifi'),
('Klima', 'snow'),
('Kahvaltı', 'cup-hot'),
('Smart TV', 'tv');
GO

DECLARE @doubleRoomId INT;
DECLARE @suiteRoomId INT;

SELECT @doubleRoomId = hr.Id
FROM HotelRooms hr
INNER JOIN Hotels h ON h.Id = hr.HotelId
INNER JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
WHERE h.Name = 'CI Istanbul Hotel' AND rt.Name = 'Double';

SELECT @suiteRoomId = hr.Id
FROM HotelRooms hr
INNER JOIN Hotels h ON h.Id = hr.HotelId
INNER JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
WHERE h.Name = 'CI Istanbul Hotel' AND rt.Name = 'Suite';

INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId)
VALUES
(@doubleRoomId, 1),
(@doubleRoomId, 2),
(@suiteRoomId, 1),
(@suiteRoomId, 2),
(@suiteRoomId, 4);
GO

INSERT INTO HotelImages (HotelId, ImageUrl, IsMain, DisplayOrder)
VALUES
(1, 'https://example.com/ci-istanbul-main.jpg', 1, 0),
(1, 'https://example.com/ci-istanbul-room.jpg', 0, 1),
(2, 'https://example.com/ci-antalya-main.jpg', 1, 0);
GO

INSERT INTO Amenities (Name, Icon)
VALUES
('WiFi', 'wifi'),
('Pool', 'droplet-fill'),
('Parking', 'car-front-fill');
GO

INSERT INTO HotelAmenities (HotelId, AmenityId)
VALUES
(1, 1),
(1, 2),
(2, 1),
(2, 3);
GO

INSERT INTO Roles (Name)
VALUES ('Admin'), ('User');
GO

INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Phone, IsActive, IsEmailVerified)
VALUES
('CI', 'Admin', 'admin@hotelapp.test', 'test-hash', '5551111111', 1, 1),
('CI', 'User', 'user@hotelapp.test', 'test-hash', '5552222222', 1, 1);
GO

INSERT INTO UserRoles (UserId, RoleId)
VALUES
(1, 1),
(2, 2);
GO

