-- Migration: sp_MakeReservation kapasite kontrolunu eszamanli isteklere karsi guvenli hale getirir.
-- HotelRooms satiri transaction boyunca kilitlenir; ayni oda stogu icin rezervasyonlar sirali islenir.

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
