-- ============================================================================
-- Migration: 2026-05-19 — Oda Manzara (RoomView) ozelligi
-- ============================================================================
-- Sebep:
--   * Hotel/Details sayfasinda oda kartlarinda manzara bilgisi eksikti.
--   * Admin oda eklerken/duzenlerken manzara secebilmeli (Deniz, Sehir vb.).
--   * "Suit oda 10 adet ama hepsi ayni manzaraya bakmiyor" senaryosu:
--     Admin ayni RoomType'tan farkli manzaralarla AYRI HotelRoom kayitlari
--     olusturacak (Suit-Deniz TotalRooms=5, Suit-Sehir TotalRooms=3).
--     Bu yuzden mevcut UQ_HotelRooms(HotelId,RoomTypeId) yetersiz; manzara
--     da unique key'in parcasi olmali.
--
-- Calistirma: SSMS uzerinden HotelAppDB veritabaninda bir kez calistir.
-- Idempotent: birden fazla kez calistirilabilir, hata vermez.
--
-- Adim sirasi onemli:
--   1) RoomViews lookup tablosu olusturulur.
--   2) Manzara turleri seed edilir (Deniz, Sehir, Dag, Havuz, Bahce, Yok).
--   3) HotelRooms.RoomViewId kolonu eklenir (NULL, eski kayitlar etkilenmesin).
--   4) FK constraint eklenir (ON DELETE SET NULL guvenligi icin).
--   5) IX_HotelRooms_RoomViewId index'i eklenir (JOIN performansi).
--   6) UQ_HotelRooms(HotelId,RoomTypeId) DROP edilip yerine
--      UQ_HotelRooms_HotelType_View(HotelId,RoomTypeId,RoomViewId) eklenir.
--   7) vw_RoomDetails view'i CREATE OR ALTER (RoomViewName/Icon eklenir).
--   8) sp_SearchAvailableRooms SP'si CREATE OR ALTER (yeni kolonlar SELECT'te).
-- ============================================================================

USE HotelAppDB;
GO

SET NOCOUNT ON;
-- QUOTED_IDENTIFIER ON gereklidir, aksi halde filtered index CREATE statement'i
-- "incorrect SET options" hatasi verir. sqlcmd default'unda kapali geliyor.
SET QUOTED_IDENTIFIER ON;
GO

-- 1) RoomViews tablosu (lookup).
IF OBJECT_ID(N'dbo.RoomViews', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RoomViews
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(50) NOT NULL,
        Icon NVARCHAR(50) NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_RoomViews_DisplayOrder DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_RoomViews_IsActive DEFAULT 1,
        CONSTRAINT PK_RoomViews PRIMARY KEY (Id),
        CONSTRAINT UQ_RoomViews_Name UNIQUE (Name)
    );
    PRINT 'RoomViews tablosu olusturuldu.';
END
ELSE
BEGIN
    PRINT 'RoomViews tablosu zaten var, atlandi.';
END
GO

-- 2) Manzara turleri seed (idempotent).
--    DisplayOrder ile UI siralamasi (Deniz once, Manzara Yok en sonda).
--    Isimler Turkce karakterli (NVARCHAR oldugu icin sorun yok).
IF NOT EXISTS (SELECT 1 FROM dbo.RoomViews WHERE Name = N'Deniz Manzaralı')
    INSERT INTO dbo.RoomViews (Name, Icon, DisplayOrder, IsActive)
    VALUES (N'Deniz Manzaralı', N'bi-water', 1, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.RoomViews WHERE Name = N'Şehir Manzaralı')
    INSERT INTO dbo.RoomViews (Name, Icon, DisplayOrder, IsActive)
    VALUES (N'Şehir Manzaralı', N'bi-buildings', 2, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.RoomViews WHERE Name = N'Dağ Manzaralı')
    INSERT INTO dbo.RoomViews (Name, Icon, DisplayOrder, IsActive)
    VALUES (N'Dağ Manzaralı', N'bi-tree', 3, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.RoomViews WHERE Name = N'Havuz Manzaralı')
    INSERT INTO dbo.RoomViews (Name, Icon, DisplayOrder, IsActive)
    VALUES (N'Havuz Manzaralı', N'bi-droplet', 4, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.RoomViews WHERE Name = N'Bahçe Manzaralı')
    INSERT INTO dbo.RoomViews (Name, Icon, DisplayOrder, IsActive)
    VALUES (N'Bahçe Manzaralı', N'bi-flower2', 5, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.RoomViews WHERE Name = N'Manzara Yok')
    INSERT INTO dbo.RoomViews (Name, Icon, DisplayOrder, IsActive)
    VALUES (N'Manzara Yok', N'bi-dash-circle', 99, 1);

PRINT 'RoomViews seed tamamlandi.';
GO

-- 3) HotelRooms tablosuna RoomViewId kolonu ekle.
--    NULL secimi bilincli: mevcut kayitlarda manzara verisi yok, NOT NULL
--    olursa migration kirilir. _RoomCard chip'i NULL kontrolu yapiyor.
IF COL_LENGTH('dbo.HotelRooms', 'RoomViewId') IS NULL
BEGIN
    ALTER TABLE dbo.HotelRooms
        ADD RoomViewId INT NULL;
    PRINT 'HotelRooms.RoomViewId kolonu eklendi (NULL).';
END
ELSE
BEGIN
    PRINT 'HotelRooms.RoomViewId zaten var, atlandi.';
END
GO

-- 4) FK constraint: RoomViews silinirse HotelRoom silinmesin, NULL'a duser.
--    Seed-only liste oldugu icin pratikte silme olmayacak ama SET NULL guvenli.
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_HotelRooms_RoomViews'
)
BEGIN
    ALTER TABLE dbo.HotelRooms
        ADD CONSTRAINT FK_HotelRooms_RoomViews
        FOREIGN KEY (RoomViewId) REFERENCES dbo.RoomViews(Id)
        ON DELETE SET NULL;
    PRINT 'FK_HotelRooms_RoomViews eklendi.';
END
ELSE
BEGIN
    PRINT 'FK_HotelRooms_RoomViews zaten var, atlandi.';
END
GO

-- 5) Index (JOIN performansi icin).
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_HotelRooms_RoomViewId'
      AND object_id = OBJECT_ID('dbo.HotelRooms')
)
BEGIN
    CREATE INDEX IX_HotelRooms_RoomViewId ON dbo.HotelRooms(RoomViewId);
    PRINT 'IX_HotelRooms_RoomViewId index eklendi.';
END
GO

-- 6) Mevcut UQ_HotelRooms(HotelId,RoomTypeId) constraint/index'ini DROP edip
--    yeni UQ_HotelRooms_HotelType_View(HotelId,RoomTypeId,RoomViewId) ekle.
--    Sebep: Admin ayni otelde "Suit-Deniz" + "Suit-Sehir" girebilmeli.
--    NULL'lar SQL Server'da unique INDEX'te esit sayilir, yani bir otelde
--    "Suit + NULL manzara" tek bir kez olur — bu istedigimiz davranis.
--    UQ_HotelRooms hem UNIQUE CONSTRAINT hem standalone INDEX olabilir;
--    ikisini de ayri ayri dener (DB First'te genelde CONSTRAINT olarak gelir).
IF EXISTS (
    SELECT 1 FROM sys.objects
    WHERE name = 'UQ_HotelRooms'
      AND parent_object_id = OBJECT_ID('dbo.HotelRooms')
      AND type = 'UQ'
)
BEGIN
    ALTER TABLE dbo.HotelRooms DROP CONSTRAINT UQ_HotelRooms;
    PRINT 'Eski UQ_HotelRooms unique constraint DROP edildi.';
END
GO

IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UQ_HotelRooms'
      AND object_id = OBJECT_ID('dbo.HotelRooms')
)
BEGIN
    DROP INDEX UQ_HotelRooms ON dbo.HotelRooms;
    PRINT 'Eski UQ_HotelRooms standalone index DROP edildi.';
END
GO

-- Eski UQ_HotelRooms_HotelType_View tek index'ti; SQL Server'da NULL'lar unique
-- index'te esit sayilmaz, yani "Suite + NULL manzara" iki kez kayit edilebilirdi.
-- Bu yuzden iki filtered index'e bolduk: NULL kayitlar icin (HotelId, RoomTypeId)
-- unique; NOT NULL kayitlar icin (HotelId, RoomTypeId, RoomViewId) unique.
IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UQ_HotelRooms_HotelType_View'
      AND object_id = OBJECT_ID('dbo.HotelRooms')
)
BEGIN
    DROP INDEX UQ_HotelRooms_HotelType_View ON dbo.HotelRooms;
    PRINT 'Eski UQ_HotelRooms_HotelType_View kaldirildi (filtered index'lerle degistiriliyor).';
END
GO

-- NULL manzara icin: ayni otel + oda tipi + NULL manzara tek kez olsun.
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UQ_HotelRooms_NoView'
      AND object_id = OBJECT_ID('dbo.HotelRooms')
)
BEGIN
    CREATE UNIQUE INDEX UQ_HotelRooms_NoView
        ON dbo.HotelRooms(HotelId, RoomTypeId)
        WHERE RoomViewId IS NULL;
    PRINT 'UQ_HotelRooms_NoView filtered unique index eklendi.';
END
GO

-- NOT NULL manzara icin: ayni otel + oda tipi + ayni manzara tek kez olsun
-- (Suite-Deniz / Suite-Sehir gibi farkli manzaralar ayri kayitlar olarak yasar).
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UQ_HotelRooms_WithView'
      AND object_id = OBJECT_ID('dbo.HotelRooms')
)
BEGIN
    CREATE UNIQUE INDEX UQ_HotelRooms_WithView
        ON dbo.HotelRooms(HotelId, RoomTypeId, RoomViewId)
        WHERE RoomViewId IS NOT NULL;
    PRINT 'UQ_HotelRooms_WithView filtered unique index eklendi.';
END
GO

-- 7) vw_RoomDetails view'ini CREATE OR ALTER.
--    RoomViewId, RoomViewName, RoomViewIcon eklendi.
--    LEFT JOIN: RoomViewId NULL olabilir.
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
    hr.IsActive,
    hr.RoomViewId,
    rv.Name AS RoomViewName,
    rv.Icon AS RoomViewIcon
FROM HotelRooms hr
INNER JOIN Hotels h ON h.Id = hr.HotelId
INNER JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
LEFT JOIN RoomViews rv ON rv.Id = hr.RoomViewId
WHERE hr.IsActive = 1 AND h.IsActive = 1;
GO

PRINT 'vw_RoomDetails guncellendi (RoomViewId/Name/Icon eklendi).';
GO

-- 8) sp_SearchAvailableRooms SP'sini CREATE OR ALTER.
--    SELECT listesine RoomViewName, RoomViewIcon eklendi.
--    EF Core SqlQueryRaw<AvailableRoomDto> kolon eslestirmesi yaptigi icin
--    DTO'ya alan eklendiyse SP de eklenmek zorunda — yoksa runtime hatasi.
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

PRINT 'sp_SearchAvailableRooms guncellendi (RoomViewName/Icon eklendi).';
GO

PRINT 'Migration 2026-05-19 tamamlandi.';
GO
