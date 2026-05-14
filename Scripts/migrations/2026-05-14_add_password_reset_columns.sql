-- ============================================================================
-- Migration: 2026-05-14 — Users tablosuna sifre yenileme alanlari ekleniyor
-- ============================================================================
-- Sebep: Faz 2 sifremi unuttum akisi gecici sifre gondermeyecek.
-- Kullaniciya tek kullanimlik reset token linki mail olarak gonderilecek.
--
-- Calistirma: SSMS veya sqlcmd uzerinden HotelAppDB veritabaninda bir kez calistir.
-- Idempotent: birden fazla kez calistirilabilir, mevcut kolon/index varsa hata vermez.
-- ============================================================================

USE HotelAppDB;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

-- Sifre yenileme token'i tutulur. Token kullanildiktan sonra NULL yapilir.
IF COL_LENGTH('dbo.Users', 'PasswordResetToken') IS NULL
BEGIN
    ALTER TABLE dbo.Users
        ADD PasswordResetToken NVARCHAR(100) NULL;
    PRINT 'PasswordResetToken kolonu eklendi.';
END
ELSE
BEGIN
    PRINT 'PasswordResetToken kolonu zaten var, atlandi.';
END
GO

-- Token'in son gecerlilik tarihi tutulur. Faz 2 icin onerilen sure 30 dakikadir.
IF COL_LENGTH('dbo.Users', 'PasswordResetTokenExpiry') IS NULL
BEGIN
    ALTER TABLE dbo.Users
        ADD PasswordResetTokenExpiry DATETIME2 NULL;
    PRINT 'PasswordResetTokenExpiry kolonu eklendi.';
END
ELSE
BEGIN
    PRINT 'PasswordResetTokenExpiry kolonu zaten var, atlandi.';
END
GO

-- Ayni kullaniciya cok sik reset maili gonderilmesini engellemek icin son istek zamani tutulur.
IF COL_LENGTH('dbo.Users', 'PasswordResetRequestedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Users
        ADD PasswordResetRequestedAt DATETIME2 NULL;
    PRINT 'PasswordResetRequestedAt kolonu eklendi.';
END
ELSE
BEGIN
    PRINT 'PasswordResetRequestedAt kolonu zaten var, atlandi.';
END
GO

-- Reset token ile kullanici bulma sorgusunu hizlandirir.
-- Filtreli index sadece token dolu kayitlari kapsar.
IF NOT EXISTS (
    SELECT 1
      FROM sys.indexes
     WHERE name = 'IX_Users_PasswordResetToken'
       AND object_id = OBJECT_ID('dbo.Users')
)
BEGIN
    CREATE INDEX IX_Users_PasswordResetToken
        ON dbo.Users(PasswordResetToken)
        WHERE PasswordResetToken IS NOT NULL;
    PRINT 'IX_Users_PasswordResetToken indexi eklendi.';
END
ELSE
BEGIN
    PRINT 'IX_Users_PasswordResetToken indexi zaten var, atlandi.';
END
GO

PRINT 'Password reset kolon migration tamamlandi.';
GO
