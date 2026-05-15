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
--
-- Calistirma: SSMS uzerinden HotelAppDB veritabaninda bir kez calistir.
-- Idempotent: birden fazla kez calistirilabilir, hata vermez.
--
-- Adim sirasi onemli:
--   1) HotelRooms.ExtraGuestPricePerNight kolonu eklenir (SP'nin bu kolona
--      bagimliligi var, kolon olmadan SP olusturulamaz).
--   2) Eski CHK_Status constraint'i kaldirilir.
--   3) Yeni CHK_Status constraint'i eklenir.
--   4) sp_MakeReservation CREATE OR ALTER ile guncellenir.
-- ============================================================================

USE HotelAppDB;
GO

SET NOCOUNT ON;
GO

-- 1) HotelRooms.ExtraGuestPricePerNight kolonu (SP'den ONCE olusturulmali).
IF COL_LENGTH('dbo.HotelRooms', 'ExtraGuestPricePerNight') IS NULL
BEGIN
    ALTER TABLE dbo.HotelRooms
        ADD ExtraGuestPricePerNight DECIMAL(10,2) NULL
        CONSTRAINT DF_HotelRooms_ExtraGuestPricePerNight DEFAULT 0;
    PRINT 'HotelRooms.ExtraGuestPricePerNight kolonu eklendi.';
END
ELSE
BEGIN
    PRINT 'HotelRooms.ExtraGuestPricePerNight zaten var, atlandi.';
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

PRINT 'Migration 2026-05-13 tamamlandi.';
GO
