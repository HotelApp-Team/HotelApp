-- ============================================================================
-- Migration: 2026-05-12 — Reservations.Status'den 'Pending' kaldiriliyor
-- ============================================================================
-- Sebep: Pending durumu kodda hicbir zaman yazilmiyordu (sp_MakeReservation
-- direkt 'Confirmed' ekliyor, baska bir akis Pending'e dusurmuyor). Olu kanca
-- olarak duran bu durum, ogrenci projesi kapsamindaki state machine'i
-- gereksiz karmasiklastiriyordu. Yeni durum kumesi: Confirmed, Cancelled, Completed.
--
-- Calistirma: SSMS uzerinden HotelAppDB veritabaninda bir kez calistir.
-- Idempotent: birden fazla kez calistirilabilir, hata vermez.
-- ============================================================================

USE HotelAppDB;
GO


SET NOCOUNT ON;

-- 1) Mevcut 'Pending' kayitlarini 'Confirmed'a tasi. Hicbir kayit yoksa 0 row etkilenir.
-- Sema prefix dosyanin geri kalaniyla tutarli olsun diye dbo. ile yazildi (calistiran kullanicinin
-- default schema'si dbo olmayabilir, bu durumda prefix'siz ad yanlis tabloyu hedefleyebilir).
UPDATE dbo.Reservations
   SET Status = 'Confirmed'
 WHERE Status = 'Pending';
PRINT CONCAT('Pending -> Confirmed guncellenen kayit sayisi: ', @@ROWCOUNT);
GO

-- 2) Eski CHECK constraint'i kaldir (varsa)
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

-- 3) Yeni CHECK constraint'i ekle (Pending haric)
IF NOT EXISTS (
    SELECT 1
      FROM sys.check_constraints
     WHERE name = 'CHK_Status'
       AND parent_object_id = OBJECT_ID('dbo.Reservations')
)
BEGIN
    ALTER TABLE dbo.Reservations
        ADD CONSTRAINT CHK_Status CHECK (Status IN ('Confirmed','Cancelled','Completed'));
    PRINT 'Yeni CHK_Status constraint eklendi (Confirmed, Cancelled, Completed).';
END
GO

-- 4) Status kolonu icin eski DEFAULT constraint'i bul ve kaldir.
-- Inline DEFAULT 'Pending' ile yaratilan constraint'in adi sistem tarafindan uretilir
-- (orn. DF__Reservati__Status__XXXXX). Adi bilmedigimiz icin sys.default_constraints'ten ariyoruz.
DECLARE @DefaultName SYSNAME;

SELECT @DefaultName = dc.name
  FROM sys.default_constraints dc
  INNER JOIN sys.columns c
       ON c.default_object_id = dc.object_id
 WHERE dc.parent_object_id = OBJECT_ID('dbo.Reservations')
   AND c.name = 'Status';

IF @DefaultName IS NOT NULL
BEGIN
    DECLARE @Sql NVARCHAR(MAX) = N'ALTER TABLE dbo.Reservations DROP CONSTRAINT ' + QUOTENAME(@DefaultName) + N';';
    EXEC sp_executesql @Sql;
    PRINT CONCAT('Eski default constraint kaldirildi: ', @DefaultName);
END
ELSE
BEGIN
    PRINT 'Status kolonu icin default constraint bulunmadi, atlandi.';
END
GO

-- 5) Yeni default'u ekle: yeni rezervasyon insert'leri SP'de Status veriyor zaten,
-- ama herhangi bir manuel insert yapildiginda Confirmed varsayilani devreye girsin.
IF NOT EXISTS (
    SELECT 1
      FROM sys.default_constraints dc
      INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
     WHERE dc.parent_object_id = OBJECT_ID('dbo.Reservations')
       AND c.name = 'Status'
)
BEGIN
    ALTER TABLE dbo.Reservations
        ADD CONSTRAINT DF_Reservations_Status DEFAULT 'Confirmed' FOR Status;
    PRINT 'Yeni default eklendi: DF_Reservations_Status = ''Confirmed''.';
END
GO

PRINT 'Migration tamamlandi.';
GO

