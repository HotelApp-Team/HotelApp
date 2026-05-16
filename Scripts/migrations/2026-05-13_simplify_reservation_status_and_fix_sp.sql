-- ============================================================================
-- Migration: 2026-05-13 — Reservations status sadelestirme + sp_MakeReservation
-- revize + HotelRooms.ExtraGuestPricePerNight kolonu
-- ============================================================================
-- Sebep:
--   * sp_MakeReservation icinde MaxGuests, GuestCount<=0, gecmis tarih ve
--     CheckOutDate<=CheckInDate validasyonlari eksikti.
--   * Pricing modelinde ek misafir farki desteklenmiyordu (oda basi sabit fiyat
--     yerine misafir basi katki: HotelRooms.ExtraGuestPricePerNight).
--   * Pending status 2026-05-12 migration'inda kalktigi icin SP'deki "Status
--     IN ('Pending','Confirmed')" referansi gereksizdi.
--   * Yeni ExtraGuestPricePerNight kolonu vw_RoomDetails view'inde ve
--     sp_SearchAvailableRooms SP'sinde de SELECT edilmeli (yoksa EF Core
--     FromSql sirasinda "kolon bulunamadi" hatasi atar).
--
-- Calistirma: SSMS uzerinden HotelAppDB veritabaninda bir kez calistir.
-- Idempotent: birden fazla kez calistirilabilir, hata vermez.
--
-- Adim sirasi onemli:
--   1) HotelRooms.ExtraGuestPricePerNight kolonu eklenir (SP/view'lerin bu
--      kolona bagimliligi var, kolon olmadan SELECT ifadeleri hata verir).
--   2) Eski CHK_Status constraint'i kaldirilir.
--   3) Yeni CHK_Status constraint'i eklenir.
--   4) sp_MakeReservation CREATE OR ALTER ile guncellenir.
--   5) vw_RoomDetails view'i CREATE OR ALTER ile guncellenir.
--   6) sp_SearchAvailableRooms CREATE OR ALTER ile guncellenir.
-- ============================================================================

USE HotelAppDB;
GO

SET NOCOUNT ON;
GO

-- 1) HotelRooms.ExtraGuestPricePerNight kolonu (SP/view'lerden ONCE olusturulmali).
--    Entity tarafinda non-nullable 'decimal' oldugu icin DB'de NOT NULL olmali; aksi
--    halde EF Core eski NULL satirlari okurken SqlNullValueException firlatir.
IF COL_LENGTH('dbo.HotelRooms', 'ExtraGuestPricePerNight') IS NULL
BEGIN
    -- Yeni kolon: NOT NULL + DEFAULT 0 (mevcut satirlara 0 set edilir).
    ALTER TABLE dbo.HotelRooms
        ADD ExtraGuestPricePerNight DECIMAL(10,2) NOT NULL
        CONSTRAINT DF_HotelRooms_ExtraGuestPricePerNight DEFAULT 0;
    PRINT 'HotelRooms.ExtraGuestPricePerNight kolonu eklendi (NOT NULL DEFAULT 0).';
END
ELSE
BEGIN
    -- Kolon var; nullable ise NULL satirlari 0'a doldur ve NOT NULL'a cevir.
    UPDATE dbo.HotelRooms
    SET ExtraGuestPricePerNight = 0
    WHERE ExtraGuestPricePerNight IS NULL;

    IF EXISTS (
        SELECT 1
        FROM sys.columns
        WHERE object_id = OBJECT_ID('dbo.HotelRooms')
          AND name = 'ExtraGuestPricePerNight'
          AND is_nullable = 1
    )
    BEGIN
        ALTER TABLE dbo.HotelRooms
            ALTER COLUMN ExtraGuestPricePerNight DECIMAL(10,2) NOT NULL;
        PRINT 'ExtraGuestPricePerNight nullable->NOT NULL yapildi, NULL satirlar 0 ile dolduruldu.';
    END
    ELSE
    BEGIN
        PRINT 'HotelRooms.ExtraGuestPricePerNight zaten var ve NOT NULL.';
    END

    -- DEFAULT constraint yoksa ekle (idempotent).
    IF NOT EXISTS (
        SELECT 1
        FROM sys.default_constraints dc
        INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
        WHERE dc.parent_object_id = OBJECT_ID('dbo.HotelRooms')
          AND c.name = 'ExtraGuestPricePerNight'
    )
    BEGIN
        ALTER TABLE dbo.HotelRooms
            ADD CONSTRAINT DF_HotelRooms_ExtraGuestPricePerNight DEFAULT 0 FOR ExtraGuestPricePerNight;
        PRINT 'DF_HotelRooms_ExtraGuestPricePerNight default constraint eklendi.';
    END
END
GO

-- 2) Eski CHK_Status constraint'i kaldir.
IF EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CHK_Status'
      AND parent_object_id = OBJECT_ID('dbo.Reservations')
)
BEGIN
    ALTER TABLE dbo.Reservations DROP CONSTRAINT CHK_Status;
    PRINT 'Eski CHK_Status constraint kaldirildi.';
END
ELSE
BEGIN
    PRINT 'CHK_Status zaten yok, atlandi.';
END
GO

-- 3) Yeni CHK_Status constraint'i ekle.
IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CHK_Status'
      AND parent_object_id = OBJECT_ID('dbo.Reservations')
)
BEGIN
    ALTER TABLE dbo.Reservations
        ADD CONSTRAINT CHK_Status
        CHECK (Status IN ('Confirmed','Cancelled','Completed'));
    PRINT 'Yeni CHK_Status constraint eklendi: Confirmed, Cancelled, Completed.';
END
GO

-- 4) sp_MakeReservation prosedurunu guncelle.
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
        FROM HotelRooms hr
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

        IF (@TotalRooms - @ReservedCount) <= 0
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

-- 5) vw_RoomDetails view'ini guncelle (ExtraGuestPricePerNight SELECT'e eklendi).
GO
CREATE OR ALTER VIEW vw_RoomDetails AS
SELECT
    hr.Id AS HotelRoomId,
    h.Id AS HotelId,
    h.Name AS HotelName,
    h.City,
    rt.Name AS RoomTypeName,
    rt.MaxGuests,
    hr.PricePerNight,
    hr.ExtraGuestPricePerNight,
    hr.TotalRooms,
    hr.IsActive
FROM HotelRooms hr
INNER JOIN Hotels h ON h.Id = hr.HotelId
INNER JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
WHERE hr.IsActive = 1 AND h.IsActive = 1;
GO

PRINT 'vw_RoomDetails guncellendi.';
GO

-- 6) sp_SearchAvailableRooms SP'sini guncelle (ExtraGuestPricePerNight SELECT'e eklendi).
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

PRINT 'sp_SearchAvailableRooms guncellendi.';
GO

PRINT 'Migration 2026-05-13 tamamlandi.';
GO
