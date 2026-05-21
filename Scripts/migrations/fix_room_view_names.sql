-- Tek seferlik fix: mevcut RoomViews isimlerini Turkce karakterli hale getir.
-- Migration 2026-05-19 ilk calistigi zaman Turkce karakter yoktu, sonradan duzeltildi.
-- Bu script idempotent (tekrar calistirilabilir).
USE HotelAppDB;
GO

UPDATE dbo.RoomViews SET Name = N'Deniz Manzaralı' WHERE Name = N'Deniz Manzarali';
UPDATE dbo.RoomViews SET Name = N'Şehir Manzaralı' WHERE Name = N'Sehir Manzarali';
UPDATE dbo.RoomViews SET Name = N'Dağ Manzaralı'   WHERE Name = N'Dag Manzarali';
UPDATE dbo.RoomViews SET Name = N'Havuz Manzaralı' WHERE Name = N'Havuz Manzarali';
UPDATE dbo.RoomViews SET Name = N'Bahçe Manzaralı' WHERE Name = N'Bahce Manzarali';

PRINT 'RoomViews isimleri Turkce karakterli hale guncellendi.';
GO

SELECT Id, Name, Icon, DisplayOrder FROM dbo.RoomViews ORDER BY DisplayOrder;
GO
