-- ============================================================================
-- Migration: 2026-05-27 — RoomImages tablosu ekleme
-- ============================================================================
-- Sebep:
--   * HotelRooms icin birden fazla oda gorseli tutulmasi gerekiyor.
--   * Her gorsel icin ana gorsel bilgisi IsMain kolonu ile saklanacak.
--   * Gorsellerin siralamasi DisplayOrder kolonu ile kontrol edilecek.
--   * HotelRoom silindiginde ilgili RoomImages kayitlari da otomatik silinmeli.
--
-- Calistirma: SSMS uzerinden HotelAppDB veritabaninda bir kez calistir.
-- Idempotent: birden fazla kez calistirilabilir, tablo varsa tekrar olusturmaz.
--
-- Adim sirasi:
--   1) RoomImages tablosu yoksa olusturulur.
--   2) HotelRooms tablosuna FK baglantisi kurulur.
--   3) HotelRoomId uzerine index eklenir.
-- ============================================================================


IF OBJECT_ID('dbo.RoomImages', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RoomImages
    (
        Id INT IDENTITY(1,1) NOT NULL,
        HotelRoomId INT NOT NULL,
        ImageUrl NVARCHAR(500) NOT NULL,
        IsMain BIT NOT NULL CONSTRAINT DF_RoomImages_IsMain DEFAULT 0,
        DisplayOrder INT NOT NULL CONSTRAINT DF_RoomImages_DisplayOrder DEFAULT 0,

        CONSTRAINT PK_RoomImages PRIMARY KEY (Id),

        CONSTRAINT FK_RoomImages_HotelRooms
            FOREIGN KEY (HotelRoomId)
            REFERENCES dbo.HotelRooms(Id)
            ON DELETE CASCADE
    );

    CREATE INDEX IX_RoomImages_HotelRoomId
        ON dbo.RoomImages(HotelRoomId);

    PRINT 'RoomImages tablosu olusturuldu.';
END
ELSE
BEGIN
    PRINT 'RoomImages tablosu zaten var.';
END;
GO