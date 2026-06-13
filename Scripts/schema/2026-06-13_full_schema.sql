/* ============================================================
   HotelAppDB - TAM SEMA (schema-only, veri yok)
   Kaynak: .\SQLEXPRESS  ->  HotelAppDB (canli veritabani)
   Uretildi: 2026-06-13 18:31
   Sira: Tablolar -> Foreign Key -> View -> Stored Procedure -> Trigger
   Not: Hedef DB'yi onceden olusturup secin (USE [HotelAppDB]).
   ============================================================ */

-- ============================================================
-- TABLOLAR (PK, default, check, index dahil)
-- ============================================================

-- Tablo: dbo.Amenities
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Amenities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
	[Icon] [nvarchar](50) COLLATE Turkish_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Tablo: dbo.HotelAmenities
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HotelAmenities](
	[HotelId] [int] NOT NULL,
	[AmenityId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[HotelId] ASC,
	[AmenityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Tablo: dbo.HotelImages
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HotelImages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HotelId] [int] NOT NULL,
	[ImageUrl] [nvarchar](500) COLLATE Turkish_CI_AS NOT NULL,
	[IsMain] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[HotelImages] ADD  DEFAULT ((0)) FOR [IsMain]
GO
ALTER TABLE [dbo].[HotelImages] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO

-- Tablo: dbo.HotelRoomAmenities
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HotelRoomAmenities](
	[HotelRoomId] [int] NOT NULL,
	[RoomAmenityId] [int] NOT NULL,
 CONSTRAINT [PK_HotelRoomAmenities] PRIMARY KEY CLUSTERED 
(
	[HotelRoomId] ASC,
	[RoomAmenityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IX_HotelRoomAmenities_RoomAmenityId] ON [dbo].[HotelRoomAmenities]
(
	[RoomAmenityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- Tablo: dbo.HotelRooms
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HotelRooms](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HotelId] [int] NOT NULL,
	[RoomTypeId] [int] NOT NULL,
	[PricePerNight] [decimal](10, 2) NOT NULL,
	[TotalRooms] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ExtraGuestPricePerNight] [decimal](10, 2) NOT NULL,
	[RoomViewId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IX_HotelRooms_HotelId] ON [dbo].[HotelRooms]
(
	[HotelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_HotelRooms_RoomTypeId] ON [dbo].[HotelRooms]
(
	[RoomTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_HotelRooms_RoomViewId] ON [dbo].[HotelRooms]
(
	[RoomViewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_HotelRooms_NoView] ON [dbo].[HotelRooms]
(
	[HotelId] ASC,
	[RoomTypeId] ASC
)
WHERE ([RoomViewId] IS NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_HotelRooms_WithView] ON [dbo].[HotelRooms]
(
	[HotelId] ASC,
	[RoomTypeId] ASC,
	[RoomViewId] ASC
)
WHERE ([RoomViewId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[HotelRooms] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[HotelRooms] ADD  CONSTRAINT [DF_HotelRooms_ExtraGuestPricePerNight]  DEFAULT ((0)) FOR [ExtraGuestPricePerNight]
GO
ALTER TABLE [dbo].[HotelRooms]  WITH CHECK ADD  CONSTRAINT [CHK_HotelRooms_Total] CHECK  (([TotalRooms]>(0)))
GO
ALTER TABLE [dbo].[HotelRooms] CHECK CONSTRAINT [CHK_HotelRooms_Total]
GO
ALTER TABLE [dbo].[HotelRooms]  WITH CHECK ADD CHECK  (([PricePerNight]>=(0)))
GO

-- Tablo: dbo.Hotels
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hotels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) COLLATE Turkish_CI_AS NOT NULL,
	[Description] [nvarchar](2000) COLLATE Turkish_CI_AS NULL,
	[Address] [nvarchar](500) COLLATE Turkish_CI_AS NOT NULL,
	[City] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
	[Country] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
	[Phone] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL,
	[Email] [nvarchar](200) COLLATE Turkish_CI_AS NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[AvgRating] [decimal](3, 1) NULL,
	[StarRating] [tinyint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
CREATE NONCLUSTERED INDEX [IX_Hotels_City] ON [dbo].[Hotels]
(
	[City] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Hotels] ADD  DEFAULT ('Turkiye') FOR [Country]
GO
ALTER TABLE [dbo].[Hotels] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Hotels] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Hotels]  WITH CHECK ADD CHECK  (([StarRating]>=(1) AND [StarRating]<=(5)))
GO

-- Tablo: dbo.Logs
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[Level] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL,
	[Message] [nvarchar](2000) COLLATE Turkish_CI_AS NOT NULL,
	[UserId] [int] NULL,
	[UserEmail] [nvarchar](200) COLLATE Turkish_CI_AS NULL,
	[Controller] [nvarchar](100) COLLATE Turkish_CI_AS NULL,
	[Action] [nvarchar](200) COLLATE Turkish_CI_AS NULL,
	[IpAddress] [nvarchar](45) COLLATE Turkish_CI_AS NULL,
	[ExceptionDetail] [nvarchar](4000) COLLATE Turkish_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
CREATE NONCLUSTERED INDEX [IX_Logs_Level] ON [dbo].[Logs]
(
	[Level] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Logs_Timestamp] ON [dbo].[Logs]
(
	[Timestamp] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Logs_UserId] ON [dbo].[Logs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Logs] ADD  DEFAULT (getdate()) FOR [Timestamp]
GO

-- Tablo: dbo.Reservations
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reservations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HotelRoomId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[CheckInDate] [date] NOT NULL,
	[CheckOutDate] [date] NOT NULL,
	[GuestCount] [int] NOT NULL,
	[TotalPrice] [decimal](10, 2) NOT NULL,
	[PaymentMethod] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
	[Status] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IX_Reservations_Dates] ON [dbo].[Reservations]
(
	[CheckInDate] ASC,
	[CheckOutDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Reservations_HotelRoomId] ON [dbo].[Reservations]
(
	[HotelRoomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Reservations_UserId] ON [dbo].[Reservations]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Reservations] ADD  DEFAULT ((1)) FOR [GuestCount]
GO
ALTER TABLE [dbo].[Reservations] ADD  CONSTRAINT [DF_Reservations_Status]  DEFAULT ('Confirmed') FOR [Status]
GO
ALTER TABLE [dbo].[Reservations] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Reservations]  WITH CHECK ADD  CONSTRAINT [CHK_Dates] CHECK  (([CheckOutDate]>[CheckInDate]))
GO
ALTER TABLE [dbo].[Reservations] CHECK CONSTRAINT [CHK_Dates]
GO
ALTER TABLE [dbo].[Reservations]  WITH CHECK ADD  CONSTRAINT [CHK_Status] CHECK  (([Status]='Completed' OR [Status]='Cancelled' OR [Status]='Confirmed'))
GO
ALTER TABLE [dbo].[Reservations] CHECK CONSTRAINT [CHK_Status]
GO
ALTER TABLE [dbo].[Reservations]  WITH CHECK ADD CHECK  (([GuestCount]>(0)))
GO
ALTER TABLE [dbo].[Reservations]  WITH CHECK ADD CHECK  (([TotalPrice]>=(0)))
GO

-- Tablo: dbo.Reviews
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HotelId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Rating] [tinyint] NOT NULL,
	[Comment] [nvarchar](1000) COLLATE Turkish_CI_AS NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Reviews_UserHotel] UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[HotelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IX_Reviews_HotelId] ON [dbo].[Reviews]
(
	[HotelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Reviews] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO

-- Tablo: dbo.Roles
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Roles_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Tablo: dbo.RoomAmenities
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomAmenities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
	[Icon] [nvarchar](50) COLLATE Turkish_CI_AS NULL,
 CONSTRAINT [PK_RoomAmenities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_RoomAmenities_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Tablo: dbo.RoomImages
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomImages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HotelRoomId] [int] NOT NULL,
	[ImageUrl] [nvarchar](500) COLLATE Turkish_CI_AS NOT NULL,
	[IsMain] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_RoomImages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IX_RoomImages_HotelRoomId] ON [dbo].[RoomImages]
(
	[HotelRoomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[RoomImages] ADD  CONSTRAINT [DF_RoomImages_IsMain]  DEFAULT ((0)) FOR [IsMain]
GO
ALTER TABLE [dbo].[RoomImages] ADD  CONSTRAINT [DF_RoomImages_DisplayOrder]  DEFAULT ((0)) FOR [DisplayOrder]
GO

-- Tablo: dbo.RoomTypes
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
	[Description] [nvarchar](500) COLLATE Turkish_CI_AS NOT NULL,
	[MaxGuests] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_RoomTypes_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[RoomTypes] ADD  DEFAULT ((2)) FOR [MaxGuests]
GO
ALTER TABLE [dbo].[RoomTypes]  WITH CHECK ADD  CONSTRAINT [CHK_RoomTypes_MaxGuests] CHECK  (([MaxGuests]>(0)))
GO
ALTER TABLE [dbo].[RoomTypes] CHECK CONSTRAINT [CHK_RoomTypes_MaxGuests]
GO

-- Tablo: dbo.RoomViews
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoomViews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) COLLATE Turkish_CI_AS NOT NULL,
	[Icon] [nvarchar](50) COLLATE Turkish_CI_AS NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_RoomViews] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_RoomViews_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[RoomViews] ADD  CONSTRAINT [DF_RoomViews_DisplayOrder]  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[RoomViews] ADD  CONSTRAINT [DF_RoomViews_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

-- Tablo: dbo.UserRoles
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Tablo: dbo.Users
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
	[LastName] [nvarchar](100) COLLATE Turkish_CI_AS NOT NULL,
	[Email] [nvarchar](200) COLLATE Turkish_CI_AS NOT NULL,
	[PasswordHash] [nvarchar](500) COLLATE Turkish_CI_AS NOT NULL,
	[Phone] [nvarchar](20) COLLATE Turkish_CI_AS NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsEmailVerified] [bit] NOT NULL,
	[EmailVerificationToken] [nvarchar](100) COLLATE Turkish_CI_AS NULL,
	[VerificationTokenExpiry] [datetime2](7) NULL,
	[FailedLoginAttempts] [int] NOT NULL,
	[LockoutEndTime] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[PasswordResetToken] [nvarchar](100) COLLATE Turkish_CI_AS NULL,
	[PasswordResetTokenExpiry] [datetime2](7) NULL,
	[PasswordResetRequestedAt] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
CREATE NONCLUSTERED INDEX [IX_Users_PasswordResetToken] ON [dbo].[Users]
(
	[PasswordResetToken] ASC
)
WHERE ([PasswordResetToken] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [IsEmailVerified]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [FailedLoginAttempts]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO

-- ============================================================
-- FOREIGN KEY (iliskiler)
-- ============================================================
ALTER TABLE [dbo].[HotelAmenities]  WITH CHECK ADD  CONSTRAINT [FK_HotelAmenities_Amenities] FOREIGN KEY([AmenityId])
REFERENCES [dbo].[Amenities] ([Id])
GO
ALTER TABLE [dbo].[HotelAmenities] CHECK CONSTRAINT [FK_HotelAmenities_Amenities]
GO
ALTER TABLE [dbo].[HotelAmenities]  WITH CHECK ADD  CONSTRAINT [FK_HotelAmenities_Hotels] FOREIGN KEY([HotelId])
REFERENCES [dbo].[Hotels] ([Id])
GO
ALTER TABLE [dbo].[HotelAmenities] CHECK CONSTRAINT [FK_HotelAmenities_Hotels]
GO
ALTER TABLE [dbo].[HotelImages]  WITH CHECK ADD  CONSTRAINT [FK_HotelImages_Hotels] FOREIGN KEY([HotelId])
REFERENCES [dbo].[Hotels] ([Id])
GO
ALTER TABLE [dbo].[HotelImages] CHECK CONSTRAINT [FK_HotelImages_Hotels]
GO
ALTER TABLE [dbo].[HotelRoomAmenities]  WITH CHECK ADD  CONSTRAINT [FK_HotelRoomAmenities_HotelRooms] FOREIGN KEY([HotelRoomId])
REFERENCES [dbo].[HotelRooms] ([Id])
GO
ALTER TABLE [dbo].[HotelRoomAmenities] CHECK CONSTRAINT [FK_HotelRoomAmenities_HotelRooms]
GO
ALTER TABLE [dbo].[HotelRoomAmenities]  WITH CHECK ADD  CONSTRAINT [FK_HotelRoomAmenities_RoomAmenities] FOREIGN KEY([RoomAmenityId])
REFERENCES [dbo].[RoomAmenities] ([Id])
GO
ALTER TABLE [dbo].[HotelRoomAmenities] CHECK CONSTRAINT [FK_HotelRoomAmenities_RoomAmenities]
GO
ALTER TABLE [dbo].[HotelRooms]  WITH CHECK ADD  CONSTRAINT [FK_HotelRooms_Hotels] FOREIGN KEY([HotelId])
REFERENCES [dbo].[Hotels] ([Id])
GO
ALTER TABLE [dbo].[HotelRooms] CHECK CONSTRAINT [FK_HotelRooms_Hotels]
GO
ALTER TABLE [dbo].[HotelRooms]  WITH CHECK ADD  CONSTRAINT [FK_HotelRooms_RoomTypes] FOREIGN KEY([RoomTypeId])
REFERENCES [dbo].[RoomTypes] ([Id])
GO
ALTER TABLE [dbo].[HotelRooms] CHECK CONSTRAINT [FK_HotelRooms_RoomTypes]
GO
ALTER TABLE [dbo].[HotelRooms]  WITH CHECK ADD  CONSTRAINT [FK_HotelRooms_RoomViews] FOREIGN KEY([RoomViewId])
REFERENCES [dbo].[RoomViews] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[HotelRooms] CHECK CONSTRAINT [FK_HotelRooms_RoomViews]
GO
ALTER TABLE [dbo].[Logs]  WITH CHECK ADD  CONSTRAINT [FK_Logs_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Logs] CHECK CONSTRAINT [FK_Logs_User]
GO
ALTER TABLE [dbo].[Reservations]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_HotelRoomId] FOREIGN KEY([HotelRoomId])
REFERENCES [dbo].[HotelRooms] ([Id])
GO
ALTER TABLE [dbo].[Reservations] CHECK CONSTRAINT [FK_Reservation_HotelRoomId]
GO
ALTER TABLE [dbo].[Reservations]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Reservations] CHECK CONSTRAINT [FK_Reservation_Users]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK_Reviews_Hotels] FOREIGN KEY([HotelId])
REFERENCES [dbo].[Hotels] ([Id])
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK_Reviews_Hotels]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK_Reviews_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK_Reviews_Users]
GO
ALTER TABLE [dbo].[RoomImages]  WITH CHECK ADD  CONSTRAINT [FK_RoomImages_HotelRooms] FOREIGN KEY([HotelRoomId])
REFERENCES [dbo].[HotelRooms] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoomImages] CHECK CONSTRAINT [FK_RoomImages_HotelRooms]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Role]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_User]
GO

-- ============================================================
-- VIEW'LAR
-- ============================================================

-- View: dbo.vw_HotelSummary
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--=====================================================================
--- VIEW: Otelin ad�, �ehir , y�ld�z, ortalama puan, yorum say�s�
--=====================================================================

CREATE VIEW vw_HotelSummary AS 
SELECT
h.Id,
h.Name,
h.City,
h.StarRating,
ISNULL(AVG(CAST(r.Rating AS DECIMAL(3,1))),0) AS AvgRating,
COUNT(r.Id) AS ReviewCount,
(SELECT MIN(hr.PricePerNight) FROM HotelRooms hr WHERE hr.HotelId= h.Id AND hr.IsActive = 1) AS MinPrice,
h.IsActive
FROM Hotels h
LEFT JOIN Reviews r ON r.HotelId = h.Id
GROUP BY h.Id, h.Name, h.City, h.StarRating, h.IsActive;

GO

-- View: dbo.vw_ReservationDetails
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW vw_ReservationDetails
AS
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
INNER JOIN RoomTypes rt ON hr.RoomTypeId = rt.Id;
GO

-- View: dbo.vw_RoomDetails
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   VIEW vw_RoomDetails AS
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

-- ============================================================
-- STORED PROCEDURE'LER
-- ============================================================

-- Procedure: dbo.sp_MakeReservation
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
-- Migration: sp_MakeReservation kapasite kontrolunu eszamanli isteklere karsi guvenli hale getirir.
-- HotelRooms satiri transaction boyunca kilitlenir; ayni oda stogu icin rezervasyonlar sirali islenir.

CREATE   PROCEDURE sp_MakeReservation
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

-- Procedure: dbo.sp_SearchAvailableRooms
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 8) sp_SearchAvailableRooms SP'sini CREATE OR ALTER.
--    SELECT listesine RoomViewName, RoomViewIcon eklendi.
--    EF Core SqlQueryRaw<AvailableRoomDto> kolon eslestirmesi yaptigi icin
--    DTO'ya alan eklendiyse SP de eklenmek zorunda â€” yoksa runtime hatasi.
CREATE   PROCEDURE sp_SearchAvailableRooms
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

-- ============================================================
-- TRIGGER'LAR
-- ============================================================

-- Trigger: trg_UpdateHotelRating ON dbo.Reviews
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--=====================================================================
-- TRIGGER: Review eklendiginde/guncellendiginde otelin ortalama puanini
-- Hotels tablosunda tutmak icin (performans optimizasyonu)
-- AvgRating kolonu Hotels CREATE TABLE icerisinde tanimli
--=====================================================================
CREATE TRIGGER trg_UpdateHotelRating
ON Reviews
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
SET NOCOUNT ON;
--- Etkilenen otel ID'lerini bul 
DECLARE @AffectedHotels TABLE(HotelId INT);
INSERT INTO @AffectedHotels (HotelId)
SELECT DISTINCT HotelId FROM inserted
UNION
SELECT DISTINCT HotelId FROM deleted;
--- Ortalama puan� g��ncelleme
UPDATE h
SET h.AvgRating = ISNULL(
(SELECT AVG(CAST(R.Rating AS DECIMAL(3,1)))
FROM Reviews r
WHERE r.HotelId = h.Id), 0)
FROM Hotels h
INNER JOIN @AffectedHotels ah ON ah.HotelId = h.Id;
END;

GO
ALTER TABLE [dbo].[Reviews] ENABLE TRIGGER [trg_UpdateHotelRating]
GO
