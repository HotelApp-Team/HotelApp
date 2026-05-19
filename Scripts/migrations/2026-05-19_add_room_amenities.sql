-- Room amenities for HotelRoom-level dynamic chips.
-- DB First project: run this script on SQL Server, then scaffold/update EF files if needed.

IF OBJECT_ID('dbo.RoomAmenities', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RoomAmenities
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Icon NVARCHAR(50) NULL,
        CONSTRAINT PK_RoomAmenities PRIMARY KEY (Id),
        CONSTRAINT UQ_RoomAmenities_Name UNIQUE (Name)
    );

    PRINT 'RoomAmenities tablosu olusturuldu.';
END
ELSE
BEGIN
    PRINT 'RoomAmenities tablosu zaten var.';
END;
GO

IF OBJECT_ID('dbo.HotelRoomAmenities', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.HotelRoomAmenities
    (
        HotelRoomId INT NOT NULL,
        RoomAmenityId INT NOT NULL,
        CONSTRAINT PK_HotelRoomAmenities PRIMARY KEY (HotelRoomId, RoomAmenityId),
        CONSTRAINT FK_HotelRoomAmenities_HotelRooms
            FOREIGN KEY (HotelRoomId) REFERENCES dbo.HotelRooms(Id),
        CONSTRAINT FK_HotelRoomAmenities_RoomAmenities
            FOREIGN KEY (RoomAmenityId) REFERENCES dbo.RoomAmenities(Id)
    );

    CREATE INDEX IX_HotelRoomAmenities_RoomAmenityId
        ON dbo.HotelRoomAmenities(RoomAmenityId);

    PRINT 'HotelRoomAmenities tablosu olusturuldu.';
END
ELSE
BEGIN
    PRINT 'HotelRoomAmenities tablosu zaten var.';
END;
GO
