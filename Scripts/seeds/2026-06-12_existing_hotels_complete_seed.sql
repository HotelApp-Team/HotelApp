USE HotelAppDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
GO

/* ============================================================
   1. MANZARALAR
   ============================================================ */

INSERT INTO RoomViews (Name, Icon, DisplayOrder, IsActive)
SELECT v.Name, v.Icon, v.DisplayOrder, 1
FROM (VALUES
    (N'Şehir Manzaralı', N'bi-buildings', 2),
    (N'Dağ Manzaralı',   N'bi-triangle',  3),
    (N'Deniz Manzaralı', N'bi-water',     1),
    (N'Bahçe Manzaralı', N'bi-flower2',   5)
) v(Name, Icon, DisplayOrder)
WHERE NOT EXISTS (
    SELECT 1 FROM RoomViews x WHERE x.Name = v.Name
);
GO

/* ============================================================
   2. OTEL OLANAKLARI
   ============================================================ */

INSERT INTO Amenities (Name, Icon)
SELECT v.Name, v.Icon
FROM (VALUES
    (N'WiFi', N'wifi'),
    (N'Havuz', N'water'),
    (N'Kapalı Havuz', N'water'),
    (N'Restoran', N'egg-fried'),
    (N'Bar', N'cup-straw'),
    (N'Spa', N'flower1'),
    (N'Hamam', N'droplet'),
    (N'Sauna', N'thermometer-half'),
    (N'Fitness', N'activity'),
    (N'Toplantı Salonu', N'briefcase'),
    (N'Konferans Salonu', N'briefcase'),
    (N'Otopark', N'p-square'),
    (N'Vale Servisi', N'car-front'),
    (N'Kuru Temizleme', N'washing-machine'),
    (N'24 Saat Resepsiyon', N'clock'),
    (N'Kütüphane', N'book'),
    (N'Teras', N'sun'),
    (N'Oda Servisi', N'bell'),
    (N'Kahvaltı', N'cup-hot'),
    (N'Jakuzi', N'droplet-half'),
    (N'Evcil Hayvan Dostu', N'heart'),
    (N'Denize Sıfır', N'water'),
    (N'Özel Plaj', N'umbrella'),
    (N'Termal', N'thermometer'),
    (N'Buhar Odası', N'cloud'),
    (N'Tenis Kortu', N'trophy')
) v(Name, Icon)
WHERE NOT EXISTS (
    SELECT 1 FROM Amenities x WHERE x.Name = v.Name
);
GO

/* ============================================================
   3. ODA OLANAKLARI
   ============================================================ */

INSERT INTO RoomAmenities (Name, Icon)
SELECT v.Name, v.Icon
FROM (VALUES
    (N'WiFi', N'wifi'),
    (N'Klima', N'snow'),
    (N'Minibar', N'cup-straw'),
    (N'LCD TV', N'tv'),
    (N'Kasa', N'safe'),
    (N'Saç Kurutma Makinesi', N'wind'),
    (N'Balkon veya Teras', N'door-open'),
    (N'Buklet Banyo Ürünleri', N'basket'),
    (N'Havlu', N'layers'),
    (N'Duş veya Küvet', N'droplet'),
    (N'Uyandırma Servisi', N'alarm'),
    (N'Telefon', N'telephone'),
    (N'Su Isıtıcısı', N'cup-hot'),
    (N'Elektronik Anahtar Sistemi', N'key'),
    (N'Gardırop', N'archive'),
    (N'Ücretsiz Şişe Su', N'droplet'),
    (N'Oturma Alanı', N'chair'),
    (N'Kahve Makinesi', N'cup-hot'),
    (N'Bornoz', N'shirt'),
    (N'Terlik', N'person-standing'),
    (N'Yastık Menüsü', N'layers'),
    (N'Executive Lounge Girişi', N'star'),
    (N'Balkonda Jakuzi', N'droplet-half'),
    (N'2 Yatak Odası', N'house'),
    (N'Oturma Odası', N'chair'),
    (N'Buzdolabı', N'box')
) v(Name, Icon)
WHERE NOT EXISTS (
    SELECT 1 FROM RoomAmenities x WHERE x.Name = v.Name
);
GO

/* ============================================================
   4. ODA TİPLERİ
   ============================================================ */

INSERT INTO RoomTypes (Name, Description, MaxGuests)
SELECT v.Name, v.Description, v.MaxGuests
FROM (VALUES
    (N'Dedeman - Superior Room', N'Dedeman Konya superior oda, 32 m2', 2),
    (N'Dedeman - Superior Twin Room', N'İki ayrı yataklı superior oda, 32 m2', 2),
    (N'Dedeman - Deluxe Room', N'Dedeman Konya deluxe oda, 38 m2', 3),
    (N'Dedeman - Family Room', N'Dedeman Konya aile odası, 57 m2', 4),
    (N'Dedeman - Junior Suite', N'Dedeman Konya junior suite, 60 m2', 3),

    (N'Divan Ankara - Superior Room', N'Divan Ankara superior oda, 29 m2', 2),
    (N'Divan Ankara - Executive Room', N'Divan Ankara executive oda, 29 m2', 2),
    (N'Divan Ankara - Deluxe Room', N'Divan Ankara deluxe oda, 35 m2', 3),
    (N'Divan Ankara - Junior Suite', N'Divan Ankara junior suite, 60 m2', 3),
    (N'Divan Ankara - Executive Suite', N'Divan Ankara executive suite, 100 m2', 4),

    (N'Gaziantep - Queen Guest Room', N'Queen guest room', 2),
    (N'Gaziantep - King Guest Room', N'King guest room', 2),
    (N'Gaziantep - King One Bedroom Suite', N'King one bedroom suite', 4),

    (N'Bursa Hilton - Queen Guest Room', N'Queen guest room, 30 m2', 2),
    (N'Bursa Hilton - Twin Guest Room', N'Twin guest room, 30 m2', 2),
    (N'Bursa Hilton - Executive Room', N'Executive oda, 30 m2', 2),

    (N'Novotel - Modern Room', N'Novotel Kayseri modern oda', 2),
    (N'Novotel - Family Room', N'Novotel Kayseri aile odası', 4),
    (N'Novotel - Suite Room', N'Ayrı oturma alanlı suite oda', 3),

    (N'Trabzon - Deluxe City View Room', N'Şehir manzaralı deluxe oda, 27 m2', 2),
    (N'Trabzon - Deluxe Sea View Room', N'Deniz manzaralı deluxe oda, 27 m2', 2),
    (N'Trabzon - Corner Suite', N'Corner suite, 37 m2', 3),
    (N'Trabzon - Business Room', N'Business room, 46 m2', 3),
    (N'Trabzon - Family Connect Room', N'Family connect room, 54 m2', 4),
    (N'Trabzon - Suite', N'Ramada Plaza Trabzon suite, 54 m2', 4),

    (N'Kusadasi - Resort Room', N'Balkon veya teraslı resort oda', 2),
    (N'Kusadasi - Family Room', N'Aile konaklamasına uygun oda', 4),
    (N'Kusadasi - Suite Room', N'Oturma alanlı suite oda', 4),

    (N'Swissotel - Bosphorus View Room', N'Boğaz manzaralı oda', 2),
    (N'Swissotel - Residence Suite', N'Swissotel Living residence suite', 4),
    (N'Swissotel - Family Residence', N'Geniş aile residence dairesi', 5),

    (N'Wyndham Izmir - Club Room', N'Kara manzaralı club room, 27 m2', 2),
    (N'Wyndham Izmir - Standard Sea View Room', N'Deniz manzaralı oda, 27 m2', 3),
    (N'Wyndham Izmir - Standard Triple Room', N'Üç kişilik oda, 27 m2', 3),
    (N'Wyndham Izmir - Standard Single Room', N'Tek kişilik oda, 27 m2', 1),

    (N'Yazz - Deluxe Room', N'Bahçe manzaralı deluxe oda, 36 m2', 2),
    (N'Yazz - Grand Deluxe Room', N'Grand deluxe oda, 50 m2', 2),
    (N'Yazz - Executive Suite', N'Executive suite, 55 m2', 3),
    (N'Yazz - King Suite', N'King suite, 63 m2', 3),
    (N'Yazz - Bungalow', N'Yazz Collective bungalov, 45 m2', 3),
    (N'Yazz - Villa', N'Yazz Collective villa, 45 m2', 3),
    (N'Yazz - Connecting Room', N'Bağlantılı oda, 90 m2', 4)
) v(Name, Description, MaxGuests)
WHERE NOT EXISTS (
    SELECT 1 FROM RoomTypes x WHERE x.Name = v.Name
);
GO

/* ============================================================
   5. OTELLER
   ============================================================ */

MERGE Hotels AS target
USING (VALUES
    (N'Dedeman Konya', N'Selçuklu’da yer alan Dedeman Konya; restoran, toplantı salonları, hamam, fitness, havuz ve SPA olanakları sunar.', N'Selçuklu, Konya Şehir Merkezi, Konya', N'Konya', N'Türkiye', N'+90 332 221 66 00', NULL, NULL, NULL, 8.9, 5),

    (N'Divan Ankara', N'Çankaya’da hizmet veren Divan Ankara; restoran, bar, SPA ve etkinlik alanları sunar.', N'Çankaya, Ankara', N'Ankara', N'Türkiye', N'+90 312 409 30 00', NULL, NULL, NULL, 9.3, 5),

    (N'DoubleTree by Hilton Gaziantep', N'Konferans alanı, vale, kütüphane ve fitness olanakları sunan şehir oteli.', N'Gaziantep Şehir Merkezi, Gaziantep', N'Gaziantep', N'Türkiye', N'+90 342 999 41 11', N'reservation.gaziantep@hilton.com', NULL, NULL, 7.2, 4),

    (N'Hilton Bursa Convention Center', N'Kapalı havuz, sauna, fitness ve restoran olanakları sunan şehir oteli.', N'Osmangazi, Bursa', N'Bursa', N'Türkiye', N'+90 224 500 05 05', N'bursa.sales@hilton.com', NULL, NULL, 9.2, 5),

    (N'Novotel Kayseri', N'Erciyes Dağı manzaralı odaları, restoranı, barı, fitness alanı ve terası bulunan şehir oteli.', N'Yenipervane Mah, Kocasinan Blv. 161-1, Kayseri', N'Kayseri', N'Türkiye', N'+90 352 207 30 00', N'info@novotelkayseri.com', NULL, NULL, 8.7, 4),

    (N'Ramada Plaza Trabzon', N'Denize sıfır konumu, havuzları, SPA, fitness ve restoranlarıyla hizmet veren resort otel.', N'Ortahisar, Trabzon', N'Trabzon', N'Türkiye', N'+90 462 335 30 30', N'info@ramadaplazatrabzon.com', NULL, NULL, 8.8, 5),

    (N'Ramada Resort Kusadasi', N'Balkon ve teraslı odaları, havuzu, SPA alanı ve özel plajıyla hizmet veren resort.', N'Türkmen Mahallesi Selçuk Bulvarı No:9/3 Kuşadası', N'Aydın', N'Türkiye', N'+90 256 420 07 00', NULL, NULL, NULL, NULL, 5),

    (N'Swissotel The Bosphorus', N'Boğaz manzarası, SPA, havuzlar, fitness, restoran ve tenis kortları sunan lüks otel.', N'Vişnezade, Acısu Sk. No:19, İstanbul', N'İstanbul', N'Türkiye', N'+90 212 326 11 00', N'istanbul@swissotel.com', NULL, NULL, 9.4, 5),

    (N'Wyndham Grand Izmir Ozdilek', N'Termal wellness merkezi, havuz, jakuzi, sauna, hamam ve fitness olanakları sunar.', N'Balçova, İzmir', N'İzmir', N'Türkiye', N'+90 232 292 13 00', NULL, NULL, NULL, 9.4, 5),

    (N'Yazz Collective Fethiye', N'Doğayla iç içe, denize sıfır konumda modern odalar, bungalov ve villalar sunar.', N'Fethiye, Muğla', N'Fethiye', N'Türkiye', N'+90 530 972 92 99', N'reservation@yazzcollective.com', NULL, NULL, 9.1, 5)
) AS source
(Name, Description, Address, City, Country, Phone, Email,
 Latitude, Longitude, AvgRating, StarRating)
ON target.Name = source.Name

WHEN MATCHED THEN UPDATE SET
    target.Description = source.Description,
    target.Address = source.Address,
    target.City = source.City,
    target.Country = source.Country,
    target.Phone = source.Phone,
    target.Email = source.Email,
    target.Latitude = source.Latitude,
    target.Longitude = source.Longitude,
    target.StarRating = source.StarRating,
    target.IsActive = 1,
    target.UpdatedAt = GETDATE()

WHEN NOT MATCHED THEN INSERT
(Name, Description, Address, City, Country, Phone, Email,
 Latitude, Longitude, AvgRating, StarRating, IsActive, CreatedAt)
VALUES
(source.Name, source.Description, source.Address, source.City,
 source.Country, source.Phone, source.Email, source.Latitude,
 source.Longitude, source.AvgRating, source.StarRating, 1, GETDATE());
GO

/* ============================================================
   6. ODALAR
   ============================================================ */

MERGE HotelRooms AS target
USING (
    SELECT h.Id HotelId, rt.Id RoomTypeId, rv.Id RoomViewId,
           v.Price, v.ExtraPrice, v.TotalRooms
    FROM (VALUES
        (N'Dedeman Konya',N'Dedeman - Superior Room',N'Şehir Manzaralı',5200,700,72),
        (N'Dedeman Konya',N'Dedeman - Superior Twin Room',N'Şehir Manzaralı',5400,700,60),
        (N'Dedeman Konya',N'Dedeman - Deluxe Room',N'Şehir Manzaralı',6500,900,36),
        (N'Dedeman Konya',N'Dedeman - Family Room',N'Şehir Manzaralı',7900,1200,18),
        (N'Dedeman Konya',N'Dedeman - Junior Suite',N'Şehir Manzaralı',9800,1500,10),

        (N'Divan Ankara',N'Divan Ankara - Superior Room',N'Şehir Manzaralı',7200,1000,70),
        (N'Divan Ankara',N'Divan Ankara - Executive Room',N'Şehir Manzaralı',8500,1200,45),
        (N'Divan Ankara',N'Divan Ankara - Deluxe Room',N'Şehir Manzaralı',9400,1300,30),
        (N'Divan Ankara',N'Divan Ankara - Junior Suite',N'Şehir Manzaralı',13500,1800,12),
        (N'Divan Ankara',N'Divan Ankara - Executive Suite',N'Şehir Manzaralı',22000,2500,4),

        (N'DoubleTree by Hilton Gaziantep',N'Gaziantep - Queen Guest Room',N'Şehir Manzaralı',4300,650,45),
        (N'DoubleTree by Hilton Gaziantep',N'Gaziantep - King Guest Room',N'Şehir Manzaralı',4600,650,35),
        (N'DoubleTree by Hilton Gaziantep',N'Gaziantep - King One Bedroom Suite',N'Şehir Manzaralı',7600,1100,8),

        (N'Hilton Bursa Convention Center',N'Bursa Hilton - Queen Guest Room',N'Şehir Manzaralı',6800,900,65),
        (N'Hilton Bursa Convention Center',N'Bursa Hilton - Twin Guest Room',N'Şehir Manzaralı',7000,900,50),
        (N'Hilton Bursa Convention Center',N'Bursa Hilton - Executive Room',N'Şehir Manzaralı',9200,1300,28),

        (N'Novotel Kayseri',N'Novotel - Modern Room',N'Dağ Manzaralı',3600,500,60),
        (N'Novotel Kayseri',N'Novotel - Family Room',N'Dağ Manzaralı',4800,700,20),
        (N'Novotel Kayseri',N'Novotel - Suite Room',N'Dağ Manzaralı',6200,900,8),

        (N'Ramada Plaza Trabzon',N'Trabzon - Deluxe City View Room',N'Şehir Manzaralı',6200,900,80),
        (N'Ramada Plaza Trabzon',N'Trabzon - Deluxe Sea View Room',N'Deniz Manzaralı',7200,1000,90),
        (N'Ramada Plaza Trabzon',N'Trabzon - Corner Suite',N'Deniz Manzaralı',9800,1400,25),
        (N'Ramada Plaza Trabzon',N'Trabzon - Business Room',N'Deniz Manzaralı',11200,1600,18),
        (N'Ramada Plaza Trabzon',N'Trabzon - Family Connect Room',N'Deniz Manzaralı',13500,1900,16),
        (N'Ramada Plaza Trabzon',N'Trabzon - Suite',N'Deniz Manzaralı',15500,2200,12),

        (N'Ramada Resort Kusadasi',N'Kusadasi - Resort Room',N'Deniz Manzaralı',8200,1200,90),
        (N'Ramada Resort Kusadasi',N'Kusadasi - Family Room',N'Deniz Manzaralı',11800,1700,35),
        (N'Ramada Resort Kusadasi',N'Kusadasi - Suite Room',N'Deniz Manzaralı',14500,2100,18),

        (N'Swissotel The Bosphorus',N'Swissotel - Bosphorus View Room',N'Deniz Manzaralı',16500,2500,120),
        (N'Swissotel The Bosphorus',N'Swissotel - Residence Suite',N'Deniz Manzaralı',28000,4000,45),
        (N'Swissotel The Bosphorus',N'Swissotel - Family Residence',N'Deniz Manzaralı',42000,6000,18),

        (N'Wyndham Grand Izmir Ozdilek',N'Wyndham Izmir - Club Room',N'Şehir Manzaralı',7800,1100,45),
        (N'Wyndham Grand Izmir Ozdilek',N'Wyndham Izmir - Standard Sea View Room',N'Deniz Manzaralı',8500,1200,80),
        (N'Wyndham Grand Izmir Ozdilek',N'Wyndham Izmir - Standard Triple Room',N'Deniz Manzaralı',9600,1400,30),
        (N'Wyndham Grand Izmir Ozdilek',N'Wyndham Izmir - Standard Single Room',N'Deniz Manzaralı',6900,0,25),

        (N'Yazz Collective Fethiye',N'Yazz - Deluxe Room',N'Bahçe Manzaralı',9900,1200,12),
        (N'Yazz Collective Fethiye',N'Yazz - Grand Deluxe Room',N'Bahçe Manzaralı',12900,1500,10),
        (N'Yazz Collective Fethiye',N'Yazz - Executive Suite',N'Bahçe Manzaralı',15900,1900,6),
        (N'Yazz Collective Fethiye',N'Yazz - King Suite',N'Bahçe Manzaralı',18900,2300,4),
        (N'Yazz Collective Fethiye',N'Yazz - Bungalow',N'Bahçe Manzaralı',14900,1800,8),
        (N'Yazz Collective Fethiye',N'Yazz - Villa',N'Bahçe Manzaralı',17500,2200,5),
        (N'Yazz Collective Fethiye',N'Yazz - Connecting Room',N'Bahçe Manzaralı',24500,3000,3)
    ) v(HotelName,RoomTypeName,ViewName,Price,ExtraPrice,TotalRooms)
    JOIN Hotels h ON h.Name = v.HotelName
    JOIN RoomTypes rt ON rt.Name = v.RoomTypeName
    JOIN RoomViews rv ON rv.Name = v.ViewName
) source
ON target.HotelId = source.HotelId
AND target.RoomTypeId = source.RoomTypeId
AND target.RoomViewId = source.RoomViewId

WHEN MATCHED THEN UPDATE SET
    target.PricePerNight = source.Price,
    target.ExtraGuestPricePerNight = source.ExtraPrice,
    target.TotalRooms = source.TotalRooms,
    target.IsActive = 1

WHEN NOT MATCHED THEN INSERT
(HotelId,RoomTypeId,PricePerNight,ExtraGuestPricePerNight,
 TotalRooms,IsActive,RoomViewId)
VALUES
(source.HotelId,source.RoomTypeId,source.Price,source.ExtraPrice,
 source.TotalRooms,1,source.RoomViewId);
GO

/* ============================================================
   7. ODA FOTOĞRAFLARI
   ============================================================ */

INSERT INTO RoomImages (HotelRoomId, ImageUrl, IsMain, DisplayOrder)
SELECT hr.Id, v.Url, 1, 0
FROM (VALUES
(N'Dedeman Konya',N'Dedeman - Superior Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041139167_RHN00141_1.jpg'),
(N'Dedeman Konya',N'Dedeman - Superior Twin Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041136440_RHN00182_1.jpg'),
(N'Dedeman Konya',N'Dedeman - Deluxe Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041129586_RHN00040_1.jpg'),
(N'Dedeman Konya',N'Dedeman - Family Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041127085_RHN00112_1.jpg'),
(N'Dedeman Konya',N'Dedeman - Junior Suite',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Dedeman_Konya_Hotel_Convention_Center_202605041135229_ECW07680_1.jpg'),

(N'Divan Ankara',N'Divan Ankara - Superior Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9U3VwZXJpb3ItS2luZy1BMg_20220429142311.jpg'),
(N'Divan Ankara',N'Divan Ankara - Executive Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9RXhlY3V0aXZlLVJvb20tS2luZy1BMQ_20220510155731.jpg'),
(N'Divan Ankara',N'Divan Ankara - Deluxe Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9RGVsdXhlLVJvb20tQTI_20220429151127.jpg'),
(N'Divan Ankara',N'Divan Ankara - Junior Suite',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9SnVuaW9yLVN1aXQtTGl2aW5nLVJvb20_20220429144326.jpg'),
(N'Divan Ankara',N'Divan Ankara - Executive Suite',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9RXhlY3V0aXZlLVN1aXQtTGl2aW5nLVJvb20tQTE_20220510153643.jpg'),

(N'DoubleTree by Hilton Gaziantep',N'Gaziantep - Queen Guest Room',N'https://www.hilton.com/im/en/GZTDTDI/7793837/gztdt-queen-guest-room1.jpg?ch=3000&cw=4500&gravity=NorthWest&impolicy=crop&rh=427&rw=640&xposition=0&yposition=0'),
(N'DoubleTree by Hilton Gaziantep',N'Gaziantep - King Guest Room',N'https://www.hilton.com/im/en/GZTDTDI/7793861/king-guest-room.jpg?ch=2857&cw=4286&gravity=NorthWest&impolicy=crop&rh=427&rw=640&xposition=0&yposition=71'),
(N'DoubleTree by Hilton Gaziantep',N'Gaziantep - King One Bedroom Suite',N'https://www.hilton.com/im/en/GZTDTDI/7793819/gztdt-one-bedroom-suite-bedroom.jpg?ch=2893&cw=4340&gravity=NorthWest&impolicy=crop&rh=427&rw=640&xposition=0&yposition=53'),

(N'Hilton Bursa Convention Center',N'Bursa Hilton - Queen Guest Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Hilton_Bursa_Convention_Center_Spa_202601131324149_Queen_standard_room_a.jpg'),
(N'Hilton Bursa Convention Center',N'Bursa Hilton - Twin Guest Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Hilton_Bursa_Convention_Center_Spa_202601131334229__M3A1123_1s.jpg'),
(N'Hilton Bursa Convention Center',N'Bursa Hilton - Executive Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Hilton_Bursa_Convention_Center_Spa_202601131336268_King_Executive_room_1_.jpg'),

(N'Novotel Kayseri', N'Novotel - Modern Room', N'https://www.ahstatic.com/photos/6904_rotwb_00_p_1024x768.jpg'),
(N'Novotel Kayseri', N'Novotel - Family Room', N'https://www.ahstatic.com/photos/6904_roqsb_00_p_1024x768.jpg'),
(N'Novotel Kayseri', N'Novotel - Suite Room', N'https://www.ahstatic.com/photos/6904_roscj_00_p_1024x768.jpg'),

(N'Ramada Plaza Trabzon',N'Trabzon - Deluxe City View Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/z5zu46hligngue0fxajd.jpg'),
(N'Ramada Plaza Trabzon',N'Trabzon - Deluxe Sea View Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Ramada_Plaza_by_Wyndham_Trabzon_202408231239159__DSC1457.jpg'),
(N'Ramada Plaza Trabzon',N'Trabzon - Corner Suite',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/dmcbwgyjhhichxgsnhcj.jpg'),
(N'Ramada Plaza Trabzon',N'Trabzon - Business Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/qmuple4pbojf7qgcbddk.jpg'),
(N'Ramada Plaza Trabzon',N'Trabzon - Family Connect Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Ramada_Plaza_by_Wyndham_Trabzon_202408231241499__DSC1438.jpg'),
(N'Ramada Plaza Trabzon',N'Trabzon - Suite',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Ramada_Plaza_by_Wyndham_Trabzon_202408231249112__DSC1460.jpg'),

(N'Ramada Resort Kusadasi',N'Kusadasi - Resort Room',N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/54018/l/Ramada-Resort-Kusadasi---Golf-Oda-147490.jpg'),
(N'Ramada Resort Kusadasi',N'Kusadasi - Family Room',N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/54018/l/Ramada-Resort-Kusadasi---Golf-Oda-147691.jpg'),
(N'Ramada Resort Kusadasi',N'Kusadasi - Suite Room',N'https://images.etstur.com/imgproxy/files/images/hotelImages/TR/54018/l/Ramada-Resort-Kusadasi---Golf-Oda-147691.jpg'),

(N'Swissotel The Bosphorus',N'Swissotel - Bosphorus View Room',N'https://dq5r178u4t83b.cloudfront.net/wp-content/uploads/sites/149/2021/06/29113752/SWISS-ADVANTAGE-ROOM-BOSPHORUS-Copy-1024x683.jpg'),
(N'Swissotel The Bosphorus',N'Swissotel - Residence Suite',N'https://bos.swissotel.com/assets/0/92/2119/2932/2971/2973/6442451685/13b6df09-a8af-4230-b8a9-720e3402a3e7.jpg'),
(N'Swissotel The Bosphorus',N'Swissotel - Family Residence',N'https://dq5r178u4t83b.cloudfront.net/wp-content/uploads/sites/149/2025/10/20182623/HOP31221-1024x768.jpg'),

(N'Wyndham Grand Izmir Ozdilek',N'Wyndham Izmir - Club Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9Q2x1Yi1PZGEtRnJlbmNoLUthcmE_20230824154904.jpg'),
(N'Wyndham Grand Izmir Ozdilek',N'Wyndham Izmir - Standard Sea View Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9U3RhbmRhcmQtLSYtQ2x1Yi1Sb29tLUZyZW5jaC1TZWEtVmlldw_20211216164217.jpg'),
(N'Wyndham Grand Izmir Ozdilek',N'Wyndham Izmir - Standard Triple Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9U3RhbmRhcmQtLSYtQ2x1Yi1Sb29tLUZyZW5jaC1TZWEtVmlldw_20211216164217.jpg'),
(N'Wyndham Grand Izmir Ozdilek',N'Wyndham Izmir - Standard Single Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/ZmlsZU5hbWU9Q2x1Yi1PZGEtRnJlbmNoLUthcmE_20230824154904.jpg'),

(N'Yazz Collective Fethiye',N'Yazz - Deluxe Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202511271147168_WhatsApp_Image_2025_11_27_at_11_46_22.jpeg'),
(N'Yazz Collective Fethiye',N'Yazz - Grand Deluxe Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202502181420092__LNS0152_HDR_min.jpg'),
(N'Yazz Collective Fethiye',N'Yazz - Executive Suite',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202502181500164_yazz1x_2_11zon.jpg'),
(N'Yazz Collective Fethiye',N'Yazz - King Suite',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202507261519074_Ekran_g_r_nt_s_2025_07_12_175011.png'),
(N'Yazz Collective Fethiye',N'Yazz - Bungalow',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202507261511235_WhatsApp_Image_2025_07_12_at_17_17_26.jpeg'),
(N'Yazz Collective Fethiye',N'Yazz - Villa',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202502181500155_yazza_8_1_11zon.jpg'),
(N'Yazz Collective Fethiye',N'Yazz - Connecting Room',N'https://images.odamax.com/imgproxy/img/800x600/odamax/image/upload/Yazz_Collective_202507261520537_WhatsApp_Image_2025_07_12_at_17_18_08.jpeg')
) v(HotelName, RoomTypeName, Url)
JOIN Hotels h ON h.Name = v.HotelName
JOIN RoomTypes rt ON rt.Name = v.RoomTypeName
JOIN HotelRooms hr
  ON hr.HotelId = h.Id
 AND hr.RoomTypeId = rt.Id
WHERE NOT EXISTS (
    SELECT 1
    FROM RoomImages x
    WHERE x.HotelRoomId = hr.Id
      AND x.ImageUrl = v.Url
);
GO

/* ============================================================
   8. OTEL KAPAK VE GALERİ FOTOĞRAFLARI
   İlk beş farklı oda fotoğrafı otel galerisine eklenir.
   ============================================================ */

;WITH AvailableImages AS
(
    SELECT DISTINCT h.Id AS HotelId, ri.ImageUrl
    FROM Hotels h
    JOIN HotelRooms hr ON hr.HotelId = h.Id
    JOIN RoomImages ri ON ri.HotelRoomId = hr.Id
    WHERE h.Name IN (
        N'Dedeman Konya',
        N'Divan Ankara',
        N'DoubleTree by Hilton Gaziantep',
        N'Hilton Bursa Convention Center',
        N'Novotel Kayseri',
        N'Ramada Plaza Trabzon',
        N'Ramada Resort Kusadasi',
        N'Swissotel The Bosphorus',
        N'Wyndham Grand Izmir Ozdilek',
        N'Yazz Collective Fethiye'
    )
),
OrderedImages AS
(
    SELECT HotelId, ImageUrl,
           ROW_NUMBER() OVER (
               PARTITION BY HotelId ORDER BY ImageUrl
           ) AS RowNumber
    FROM AvailableImages
)
INSERT INTO HotelImages (HotelId, ImageUrl, IsMain, DisplayOrder)
SELECT
    x.HotelId,
    x.ImageUrl,
    CASE
        WHEN x.RowNumber = 1
         AND NOT EXISTS (
             SELECT 1 FROM HotelImages m
             WHERE m.HotelId = x.HotelId AND m.IsMain = 1
         )
        THEN 1 ELSE 0
    END,
    x.RowNumber - 1
FROM OrderedImages x
WHERE x.RowNumber <= 5
AND NOT EXISTS (
    SELECT 1
    FROM HotelImages existing
    WHERE existing.HotelId = x.HotelId
      AND existing.ImageUrl = x.ImageUrl
);
GO

/* ============================================================
   9. OTEL OLANAKLARI
   ============================================================ */

INSERT INTO HotelAmenities (HotelId, AmenityId)
SELECT h.Id, a.Id
FROM Hotels h
JOIN (VALUES
    (N'Dedeman Konya', N'WiFi'),
    (N'Dedeman Konya', N'Havuz'),
    (N'Dedeman Konya', N'Kapalı Havuz'),
    (N'Dedeman Konya', N'Restoran'),
    (N'Dedeman Konya', N'Hamam'),
    (N'Dedeman Konya', N'Sauna'),
    (N'Dedeman Konya', N'Spa'),
    (N'Dedeman Konya', N'Fitness'),
    (N'Dedeman Konya', N'Toplantı Salonu'),
    (N'Dedeman Konya', N'Otopark'),

    (N'Divan Ankara', N'WiFi'),
    (N'Divan Ankara', N'Restoran'),
    (N'Divan Ankara', N'Bar'),
    (N'Divan Ankara', N'Spa'),
    (N'Divan Ankara', N'Fitness'),
    (N'Divan Ankara', N'Toplantı Salonu'),

    (N'DoubleTree by Hilton Gaziantep', N'Fitness'),
    (N'DoubleTree by Hilton Gaziantep', N'Konferans Salonu'),
    (N'DoubleTree by Hilton Gaziantep', N'Kuru Temizleme'),
    (N'DoubleTree by Hilton Gaziantep', N'Vale Servisi'),
    (N'DoubleTree by Hilton Gaziantep', N'Kütüphane'),
    (N'DoubleTree by Hilton Gaziantep', N'24 Saat Resepsiyon'),

    (N'Hilton Bursa Convention Center', N'WiFi'),
    (N'Hilton Bursa Convention Center', N'Restoran'),
    (N'Hilton Bursa Convention Center', N'Spa'),
    (N'Hilton Bursa Convention Center', N'Kapalı Havuz'),
    (N'Hilton Bursa Convention Center', N'Sauna'),
    (N'Hilton Bursa Convention Center', N'Fitness'),
    (N'Hilton Bursa Convention Center', N'Toplantı Salonu'),

    (N'Novotel Kayseri', N'WiFi'),
    (N'Novotel Kayseri', N'Restoran'),
    (N'Novotel Kayseri', N'Bar'),
    (N'Novotel Kayseri', N'Fitness'),
    (N'Novotel Kayseri', N'Teras'),
    (N'Novotel Kayseri', N'Konferans Salonu'),
    (N'Novotel Kayseri', N'Kahvaltı'),
    (N'Novotel Kayseri', N'Oda Servisi'),

    (N'Ramada Plaza Trabzon', N'WiFi'),
    (N'Ramada Plaza Trabzon', N'Havuz'),
    (N'Ramada Plaza Trabzon', N'Özel Plaj'),
    (N'Ramada Plaza Trabzon', N'Denize Sıfır'),
    (N'Ramada Plaza Trabzon', N'Spa'),
    (N'Ramada Plaza Trabzon', N'Fitness'),
    (N'Ramada Plaza Trabzon', N'Toplantı Salonu'),
    (N'Ramada Plaza Trabzon', N'Restoran'),
    (N'Ramada Plaza Trabzon', N'Bar'),
    (N'Ramada Plaza Trabzon', N'Evcil Hayvan Dostu'),

    (N'Ramada Resort Kusadasi', N'WiFi'),
    (N'Ramada Resort Kusadasi', N'Havuz'),
    (N'Ramada Resort Kusadasi', N'Restoran'),
    (N'Ramada Resort Kusadasi', N'Bar'),
    (N'Ramada Resort Kusadasi', N'Spa'),
    (N'Ramada Resort Kusadasi', N'Fitness'),
    (N'Ramada Resort Kusadasi', N'Özel Plaj'),

    (N'Swissotel The Bosphorus', N'WiFi'),
    (N'Swissotel The Bosphorus', N'Havuz'),
    (N'Swissotel The Bosphorus', N'Kapalı Havuz'),
    (N'Swissotel The Bosphorus', N'Spa'),
    (N'Swissotel The Bosphorus', N'Restoran'),
    (N'Swissotel The Bosphorus', N'Bar'),
    (N'Swissotel The Bosphorus', N'Fitness'),
    (N'Swissotel The Bosphorus', N'Tenis Kortu'),
    (N'Swissotel The Bosphorus', N'Hamam'),
    (N'Swissotel The Bosphorus', N'Sauna'),
    (N'Swissotel The Bosphorus', N'Jakuzi'),

    (N'Wyndham Grand Izmir Ozdilek', N'WiFi'),
    (N'Wyndham Grand Izmir Ozdilek', N'Havuz'),
    (N'Wyndham Grand Izmir Ozdilek', N'Kapalı Havuz'),
    (N'Wyndham Grand Izmir Ozdilek', N'Termal'),
    (N'Wyndham Grand Izmir Ozdilek', N'Spa'),
    (N'Wyndham Grand Izmir Ozdilek', N'Sauna'),
    (N'Wyndham Grand Izmir Ozdilek', N'Hamam'),
    (N'Wyndham Grand Izmir Ozdilek', N'Jakuzi'),
    (N'Wyndham Grand Izmir Ozdilek', N'Fitness'),
    (N'Wyndham Grand Izmir Ozdilek', N'Toplantı Salonu'),
    (N'Wyndham Grand Izmir Ozdilek', N'Restoran'),

    (N'Yazz Collective Fethiye', N'WiFi'),
    (N'Yazz Collective Fethiye', N'Restoran'),
    (N'Yazz Collective Fethiye', N'Bar'),
    (N'Yazz Collective Fethiye', N'Denize Sıfır'),
    (N'Yazz Collective Fethiye', N'Özel Plaj'),
    (N'Yazz Collective Fethiye', N'Oda Servisi')
) v(HotelName, AmenityName)
    ON v.HotelName = h.Name
JOIN Amenities a
    ON a.Name = v.AmenityName
WHERE NOT EXISTS (
    SELECT 1
    FROM HotelAmenities existing
    WHERE existing.HotelId = h.Id
      AND existing.AmenityId = a.Id
);
GO

/* ============================================================
   10. TÜM ODALAR İÇİN TEMEL OLANAKLAR
   ============================================================ */

INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId)
SELECT hr.Id, ra.Id
FROM HotelRooms hr
JOIN Hotels h ON h.Id = hr.HotelId
JOIN RoomAmenities ra ON ra.Name IN (
    N'WiFi',
    N'Klima',
    N'Minibar',
    N'LCD TV',
    N'Kasa',
    N'Saç Kurutma Makinesi',
    N'Balkon veya Teras',
    N'Buklet Banyo Ürünleri',
    N'Havlu',
    N'Duş veya Küvet',
    N'Uyandırma Servisi',
    N'Telefon',
    N'Su Isıtıcısı',
    N'Elektronik Anahtar Sistemi',
    N'Gardırop',
    N'Ücretsiz Şişe Su'
)
WHERE h.Name IN (
    N'Dedeman Konya',
    N'Divan Ankara',
    N'DoubleTree by Hilton Gaziantep',
    N'Hilton Bursa Convention Center',
    N'Novotel Kayseri',
    N'Ramada Plaza Trabzon',
    N'Ramada Resort Kusadasi',
    N'Swissotel The Bosphorus',
    N'Wyndham Grand Izmir Ozdilek',
    N'Yazz Collective Fethiye'
)
AND NOT EXISTS (
    SELECT 1
    FROM HotelRoomAmenities existing
    WHERE existing.HotelRoomId = hr.Id
      AND existing.RoomAmenityId = ra.Id
);
GO

/* ============================================================
   11. SUITE, FAMILY, EXECUTIVE VE VİLLA OLANAKLARI
   ============================================================ */

INSERT INTO HotelRoomAmenities (HotelRoomId, RoomAmenityId)
SELECT hr.Id, ra.Id
FROM HotelRooms hr
JOIN Hotels h ON h.Id = hr.HotelId
JOIN RoomTypes rt ON rt.Id = hr.RoomTypeId
JOIN RoomAmenities ra ON
(
       (
           ra.Name IN (
               N'Oturma Alanı',
               N'Oturma Odası',
               N'Kahve Makinesi',
               N'Bornoz',
               N'Terlik',
               N'Yastık Menüsü'
           )
           AND (
               rt.Name LIKE N'%Suite%'
               OR rt.Name LIKE N'%Süit%'
           )
       )
    OR (
           ra.Name = N'2 Yatak Odası'
           AND (
               rt.Name LIKE N'%Family%'
               OR rt.Name LIKE N'%Connecting%'
               OR rt.Name LIKE N'%Residence%'
           )
       )
    OR (
           ra.Name = N'Executive Lounge Girişi'
           AND rt.Name LIKE N'%Executive%'
       )
    OR (
           ra.Name = N'Balkonda Jakuzi'
           AND (
               rt.Name LIKE N'%Villa%'
               OR rt.Name LIKE N'%King Suite%'
           )
       )
    OR (
           ra.Name = N'Buzdolabı'
           AND (
               rt.Name LIKE N'%Residence%'
               OR rt.Name LIKE N'%Villa%'
               OR rt.Name LIKE N'%Bungalow%'
           )
       )
)
WHERE h.Name IN (
    N'Dedeman Konya',
    N'Divan Ankara',
    N'DoubleTree by Hilton Gaziantep',
    N'Hilton Bursa Convention Center',
    N'Novotel Kayseri',
    N'Ramada Plaza Trabzon',
    N'Ramada Resort Kusadasi',
    N'Swissotel The Bosphorus',
    N'Wyndham Grand Izmir Ozdilek',
    N'Yazz Collective Fethiye'
)
AND NOT EXISTS (
    SELECT 1
    FROM HotelRoomAmenities existing
    WHERE existing.HotelRoomId = hr.Id
      AND existing.RoomAmenityId = ra.Id
);
GO

/* ============================================================
   12. YORUM KULLANICILARI
   ============================================================ */

DECLARE @ReviewSeed TABLE
(
    SortNo INT,
    HotelName NVARCHAR(200),
    UserEmail NVARCHAR(200),
    Rating INT,
    Comment NVARCHAR(1000)
);

INSERT INTO @ReviewSeed
(
    SortNo,
    HotelName,
    UserEmail,
    Rating,
    Comment
)
VALUES
(7, N'Dedeman Konya',
 N'hotel.review.007@hotelapp.local', 5,
 N'Kahvaltı, spor alanları, temizlik ve personel ilgisi sürekli aynı kalitede bulunmuş.'),

(8, N'Dedeman Konya',
 N'hotel.review.008@hotelapp.local', 5,
 N'Karşılama, uğurlama, oda hijyeni, yemekler, havuz ve merkezi konum çok olumlu değerlendirilmiş.'),

(9, N'Dedeman Konya',
 N'hotel.review.009@hotelapp.local', 4,
 N'Otel temizliği ve çalışan ilgisi beğenilmiş; kahvaltı çeşitliliği yeterli ama çok geniş bulunmamış.'),

(10, N'Divan Ankara',
 N'hotel.review.010@hotelapp.local', 5,
 N'Oda temizliği, kalite, hizmet ve personel karşılaması nedeniyle otel tavsiye edilmiş.'),

(11, N'Divan Ankara',
 N'hotel.review.011@hotelapp.local', 5,
 N'Geniş ve temiz oda, zengin kahvaltı, ilgili personel ve geniş otopark olumlu bulunmuş.'),

(12, N'Divan Ankara',
 N'hotel.review.012@hotelapp.local', 5,
 N'Odanın temizliği ve kullanışlılığı beğenilmiş; yaşanan klima sorununa hızlı çözüm üretilmiş.'),

(16, N'DoubleTree by Hilton Gaziantep',
 N'hotel.review.016@hotelapp.local', 4,
 N'Şehir merkezi konumu ve personel ilgisi beğenilmiş; iş seyahati için pratik bulunmuş.'),

(17, N'DoubleTree by Hilton Gaziantep',
 N'hotel.review.017@hotelapp.local', 4,
 N'Oda temizliği, kahvaltı ve vale hizmeti olumlu değerlendirilmiş.'),

(18, N'DoubleTree by Hilton Gaziantep',
 N'hotel.review.018@hotelapp.local', 3,
 N'Genel konaklama yeterli bulunmuş; bazı hizmetlerde yenileme ve hız beklentisi dile getirilmiş.'),

(19, N'Hilton Bursa Convention Center',
 N'hotel.review.019@hotelapp.local', 5,
 N'Konum, oda kalitesi, kahvaltı ve personel ilgisi çok başarılı bulunmuş.'),

(20, N'Hilton Bursa Convention Center',
 N'hotel.review.020@hotelapp.local', 5,
 N'Spa, havuz ve temizlik standartları beğenilmiş; iş ve aile konaklaması için uygun görülmüş.'),

(21, N'Hilton Bursa Convention Center',
 N'hotel.review.021@hotelapp.local', 4,
 N'Otel genel olarak kaliteli bulunmuş; yoğun dönemlerde servis hızının artması beklenmiş.'),

(43, N'Novotel Kayseri',
 N'hotel.review.043@hotelapp.local', 4,
 N'Merkezi konum, temiz oda ve pratik konaklama imkanı olumlu bulunmuş.'),

(44, N'Novotel Kayseri',
 N'hotel.review.044@hotelapp.local', 4,
 N'Kahvaltı ve personel ilgisi beğenilmiş; iş seyahatleri için uygun görülmüş.'),

(45, N'Novotel Kayseri',
 N'hotel.review.045@hotelapp.local', 4,
 N'Oda konforu yeterli bulunmuş; fiyat performans dengesi olumlu değerlendirilmiş.'),

(46, N'Ramada Plaza Trabzon',
 N'hotel.review.046@hotelapp.local', 5,
 N'Denize sıfır konum, manzara, oda temizliği ve kahvaltı başarılı bulunmuş.'),

(47, N'Ramada Plaza Trabzon',
 N'hotel.review.047@hotelapp.local', 4,
 N'Havuz, spa ve genel tesis olanakları beğenilmiş; yoğunlukta servis hızı değişebilmiş.'),

(48, N'Ramada Plaza Trabzon',
 N'hotel.review.048@hotelapp.local', 5,
 N'Personel ilgisi ve aile konaklamasına uygun atmosfer olumlu değerlendirilmiş.'),

(49, N'Ramada Resort Kusadasi',
 N'hotel.review.049@hotelapp.local', 4,
 N'Oda büyüklüğü, havuz alanları ve resort imkanları beğenilmiş.'),

(50, N'Ramada Resort Kusadasi',
 N'hotel.review.050@hotelapp.local', 4,
 N'Aile konaklaması için rahat bulunmuş; personel ilgisi ve temizlik olumlu değerlendirilmiş.'),

(51, N'Ramada Resort Kusadasi',
 N'hotel.review.051@hotelapp.local', 4,
 N'Genel tesis olanakları yeterli görülmüş; plaj erişimi ve servis beklentileri değişebilmiş.'),

(61, N'Swissotel The Bosphorus',
 N'hotel.review.061@hotelapp.local', 5,
 N'Boğaz manzarası, oda kalitesi, spa ve merkezi konum çok başarılı bulunmuş.'),

(62, N'Swissotel The Bosphorus',
 N'hotel.review.062@hotelapp.local', 5,
 N'Personel ilgisi, temizlik ve kahvaltı kalitesi üst düzey değerlendirilmiş.'),

(63, N'Swissotel The Bosphorus',
 N'hotel.review.063@hotelapp.local', 5,
 N'Özel gün ve şehir tatili için konforlu, prestijli ve huzurlu bir otel olarak görülmüş.'),

(67, N'Wyndham Grand Izmir Ozdilek',
 N'hotel.review.067@hotelapp.local', 5,
 N'Deniz manzarası, termal alan, havuzlar ve oda temizliği çok başarılı bulunmuş.'),

(68, N'Wyndham Grand Izmir Ozdilek',
 N'hotel.review.068@hotelapp.local', 5,
 N'Personel ilgisi, kahvaltı ve spa olanakları olumlu değerlendirilmiş.'),

(69, N'Wyndham Grand Izmir Ozdilek',
 N'hotel.review.069@hotelapp.local', 4,
 N'Konum ve hizmet kalitesi beğenilmiş; bazı misafirler şehir merkezine mesafeyi not etmiş.'),

(73, N'Yazz Collective Fethiye',
 N'hotel.review.073@hotelapp.local', 5,
 N'Doğayla iç içe atmosfer, deniz, oda tasarımı ve servis kalitesi çok beğenilmiş.'),

(74, N'Yazz Collective Fethiye',
 N'hotel.review.074@hotelapp.local', 5,
 N'Sakinlik, restoran deneyimi ve özel plaj hissi olumlu değerlendirilmiş.'),

(75, N'Yazz Collective Fethiye',
 N'hotel.review.075@hotelapp.local', 4,
 N'Tesis konsepti çok özel bulunmuş; ulaşım ve fiyat seviyesi beklentiye göre değişebilmiş.');


INSERT INTO Users
(
    FirstName,
    LastName,
    Email,
    PasswordHash,
    Phone,
    IsActive,
    IsEmailVerified,
    CreatedAt,
    FailedLoginAttempts
)
SELECT
    N'Misafir',
    N'Yorum ' + RIGHT(N'000' + CONVERT(NVARCHAR(10), r.SortNo), 3),
    r.UserEmail,
    N'hotel-seed-password-hash',
    N'5550000000',
    1,
    1,
    GETDATE(),
    0
FROM @ReviewSeed r
WHERE NOT EXISTS (
    SELECT 1
    FROM Users existing
    WHERE existing.Email = r.UserEmail
);


/* ============================================================
   13. YORUMLAR
   ============================================================ */

INSERT INTO Reviews
(
    HotelId,
    UserId,
    Rating,
    Comment,
    CreatedAt
)
SELECT
    h.Id,
    u.Id,
    r.Rating,
    r.Comment,
    DATEADD(DAY, -r.SortNo, GETDATE())
FROM @ReviewSeed r
JOIN Hotels h ON h.Name = r.HotelName
JOIN Users u ON u.Email = r.UserEmail
WHERE NOT EXISTS (
    SELECT 1
    FROM Reviews existing
    WHERE existing.HotelId = h.Id
      AND existing.UserId = u.Id
);
GO

/* ============================================================
   14. SONUÇ KONTROLÜ
   ============================================================ */

SELECT
    h.Name,
    COUNT(DISTINCT hr.Id) AS OdaSayisi,
    COUNT(DISTINCT hi.Id) AS OtelFotografi,
    COUNT(DISTINCT ri.Id) AS OdaFotografi,
    COUNT(DISTINCT ha.AmenityId) AS OtelOlanagi,
    COUNT(DISTINCT hra.RoomAmenityId) AS OdaOlanagi,
    COUNT(DISTINCT rv.Id) AS YorumSayisi
FROM Hotels h
LEFT JOIN HotelRooms hr
    ON hr.HotelId = h.Id
LEFT JOIN HotelImages hi
    ON hi.HotelId = h.Id
LEFT JOIN RoomImages ri
    ON ri.HotelRoomId = hr.Id
LEFT JOIN HotelAmenities ha
    ON ha.HotelId = h.Id
LEFT JOIN HotelRoomAmenities hra
    ON hra.HotelRoomId = hr.Id
LEFT JOIN Reviews rv
    ON rv.HotelId = h.Id
WHERE h.Name IN (
    N'Dedeman Konya',
    N'Divan Ankara',
    N'DoubleTree by Hilton Gaziantep',
    N'Hilton Bursa Convention Center',
    N'Novotel Kayseri',
    N'Ramada Plaza Trabzon',
    N'Ramada Resort Kusadasi',
    N'Swissotel The Bosphorus',
    N'Wyndham Grand Izmir Ozdilek',
    N'Yazz Collective Fethiye'
)
GROUP BY h.Name
ORDER BY h.Name;
GO

/* ============================================================
   15. GERÇEK OTEL FOTOĞRAFLARI
   Mevcut fotoğrafları silmez.
   ============================================================ */

IF @@TRANCOUNT = 0
    BEGIN TRANSACTION;

DECLARE @Images TABLE
(
    HotelName NVARCHAR(200),
    ImageUrl NVARCHAR(500),
    IsMain BIT,
    DisplayOrder INT
);

INSERT INTO @Images
(
    HotelName,
    ImageUrl,
    IsMain,
    DisplayOrder
)
VALUES
-- Dedeman Konya
(N'Dedeman Konya', N'https://www.dedeman.com/assets/upload/fotograflar/dis-mekan-2_9.jpg', 1, 0),
(N'Dedeman Konya', N'https://www.dedeman.com/assets/upload/fotograflar/dis-mekan-5.jpg', 0, 1),
(N'Dedeman Konya', N'https://www.dedeman.com/assets/upload/fotograflar/dis-mekan-7.jpg', 0, 2),
(N'Dedeman Konya', N'https://www.dedeman.com/assets/upload/fotograflar/dis-mekan-9_1.jpg', 0, 3),
(N'Dedeman Konya', N'https://www.dedeman.com/assets/upload/fotograflar/havuz_1---kopya.jpg', 0, 4),

-- Divan Ankara
(N'Divan Ankara', N'https://bestloved.com/wp-content/uploads/2024/10/60a6feefa0a8f3cf26049d619f015c36.jpg', 1, 0),
(N'Divan Ankara', N'https://bestloved.com/wp-content/uploads/2024/10/b874c895b4e6e4b3a24f8e5a6616a93f.jpg', 0, 1),
(N'Divan Ankara', N'https://bestloved.com/wp-content/uploads/2024/10/058fbe7cf0993bb97fcde72b1aefdb50.jpg', 0, 2),
(N'Divan Ankara', N'https://bestloved.com/wp-content/uploads/2024/10/ea3825c8489cdfd14095c3c32574300c.jpg', 0, 3),
(N'Divan Ankara', N'https://bestloved.com/wp-content/uploads/2024/10/b922b93dbe8c74774a7cf51c7a3b958b.jpg', 0, 4),

-- DoubleTree by Hilton Gaziantep
(N'DoubleTree by Hilton Gaziantep', N'https://images.trvl-media.com/lodging/9000000/8730000/8724700/8724686/5a1a5d70.jpg?impolicy=resizecrop&ra=fit&rw=1200', 1, 0),
(N'DoubleTree by Hilton Gaziantep', N'https://images.trvl-media.com/lodging/9000000/8730000/8724700/8724686/2d8dfd7a.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 1),
(N'DoubleTree by Hilton Gaziantep', N'https://images.trvl-media.com/lodging/9000000/8730000/8724700/8724686/f7f041d7.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 2),
(N'DoubleTree by Hilton Gaziantep', N'https://images.trvl-media.com/lodging/9000000/8730000/8724700/8724686/e7f74405.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 3),
(N'DoubleTree by Hilton Gaziantep', N'https://images.trvl-media.com/lodging/9000000/8730000/8724700/8724686/8aa3f165.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 4),

-- Hilton Bursa Convention Center
(N'Hilton Bursa Convention Center', N'https://images.trvl-media.com/lodging/5000000/4450000/4447800/4447755/6edf56c0.jpg?impolicy=resizecrop&ra=fit&rw=1200', 1, 0),
(N'Hilton Bursa Convention Center', N'https://images.trvl-media.com/lodging/5000000/4450000/4447800/4447755/74e1a2ca.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 1),
(N'Hilton Bursa Convention Center', N'https://images.trvl-media.com/lodging/5000000/4450000/4447800/4447755/44657353.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 2),
(N'Hilton Bursa Convention Center', N'https://images.trvl-media.com/lodging/5000000/4450000/4447800/4447755/86ac1856.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 3),
(N'Hilton Bursa Convention Center', N'https://images.trvl-media.com/lodging/5000000/4450000/4447800/4447755/6371090b.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 4),

-- Novotel Kayseri
(N'Novotel Kayseri', N'https://images.trvl-media.com/lodging/4000000/3570000/3566000/3565950/e4fbb1eb.jpg?impolicy=resizecrop&ra=fit&rw=1200', 1, 0),
(N'Novotel Kayseri', N'https://images.trvl-media.com/lodging/4000000/3570000/3566000/3565950/574bd24e.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 1),
(N'Novotel Kayseri', N'https://images.trvl-media.com/lodging/4000000/3570000/3566000/3565950/944eca2b.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 2),
(N'Novotel Kayseri', N'https://images.trvl-media.com/lodging/4000000/3570000/3566000/3565950/89a523f2.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 3),
(N'Novotel Kayseri', N'https://images.trvl-media.com/lodging/4000000/3570000/3566000/3565950/fbcea38d.jpg?impolicy=resizecrop&ra=fit&rw=1200', 0, 4),

-- Ramada Plaza Trabzon
(N'Ramada Plaza Trabzon', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/trabzon/47688/47688_exterior_1.jpg', 1, 0),
(N'Ramada Plaza Trabzon', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/trabzon/47688/47688_lobby_1.jpg', 0, 1),
(N'Ramada Plaza Trabzon', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/trabzon/47688/47688_Blue_Restaurant_1.jpg', 0, 2),
(N'Ramada Plaza Trabzon', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/trabzon/47688/47688_outdoor_pool_1.jpg', 0, 3),
(N'Ramada Plaza Trabzon', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/trabzon/47688/47688_Garden_view_1.jpg', 0, 4),

-- Ramada Resort Kusadasi
(N'Ramada Resort Kusadasi', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/kusadasi/aydin/46609/46609_exterior_view_1.jpg', 1, 0),
(N'Ramada Resort Kusadasi', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/kusadasi/aydin/46609/46609_exterior_view_6.jpg', 0, 1),
(N'Ramada Resort Kusadasi', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/kusadasi/aydin/46609/46609_lobby_view_5.jpg', 0, 2),
(N'Ramada Resort Kusadasi', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/kusadasi/aydin/46609/46609_restaurant_5.jpg', 0, 3),
(N'Ramada Resort Kusadasi', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/ra/tr/others/kusadasi/aydin/46609/46609_pool_view_3.jpg', 0, 4),

-- Swissotel The Bosphorus
(N'Swissotel The Bosphorus', N'https://teneohg.com/wp-content/uploads/2019/07/SwissotelTheBosphorusIstanbul_Hero1.jpg', 1, 0),
(N'Swissotel The Bosphorus', N'https://teneohg.com/wp-content/uploads/2019/07/SwissotelTheBosphorusIstanbul_Hero6.jpg', 0, 1),
(N'Swissotel The Bosphorus', N'https://teneohg.com/wp-content/uploads/2019/07/SwissotelTheBosphorusIstanbul_Hero7.jpg', 0, 2),
(N'Swissotel The Bosphorus', N'https://teneohg.com/wp-content/uploads/2019/07/SwissotelTheBosphorusIstanbul_Hero4.jpg', 0, 3),
(N'Swissotel The Bosphorus', N'https://teneohg.com/wp-content/uploads/2019/07/SwissotelTheBosphorusIstanbul_Hero5.jpg', 0, 4),

-- Wyndham Grand Izmir Ozdilek
(N'Wyndham Grand Izmir Ozdilek', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/gr/tr/others/izmir/47728/47728_exterior_view_5.jpg', 1, 0),
(N'Wyndham Grand Izmir Ozdilek', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/gr/tr/others/izmir/47728/47728_exterior_view_3.jpg', 0, 1),
(N'Wyndham Grand Izmir Ozdilek', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/gr/tr/others/izmir/47728/47728_lobby_view_1.jpg', 0, 2),
(N'Wyndham Grand Izmir Ozdilek', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/gr/tr/others/izmir/47728/47728_pool_view_1.jpg', 0, 3),
(N'Wyndham Grand Izmir Ozdilek', N'https://www.wyndhamhotels.com/content/dam/property-images/en-us/gr/tr/others/izmir/47728/47728_restaurant_brasserie_1.jpg', 0, 4),

-- Yazz Collective Fethiye
(N'Yazz Collective Fethiye', N'https://yazzcollective.com/wp-content/uploads/2025/02/f9b833b9thumbnail.jpeg', 1, 0),
(N'Yazz Collective Fethiye', N'https://lirp.cdn-website.com/ce11be21/import/clib/yazzcollective_com/dms3rep/multi/opt/csg10-960x680-1920w.jpeg', 0, 1),
(N'Yazz Collective Fethiye', N'https://lirp.cdn-website.com/ce11be21/import/clib/yazzcollective_com/dms3rep/multi/opt/restaurant_evening-683x1024-683x1024-1920w.jpeg', 0, 2),
(N'Yazz Collective Fethiye', N'https://lirp.cdn-website.com/ce11be21/dms3rep/multi/opt/Beach%2BSide%2Bpanaromic-1920w.jpeg', 0, 3),
(N'Yazz Collective Fethiye', N'https://lirp.cdn-website.com/ce11be21/dms3rep/multi/opt/7%2Bkopyas%C4%B1-1920w.jpg', 0, 4);

IF EXISTS
(
    SELECT 1
    FROM @Images i
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM Hotels h
        WHERE h.Name = i.HotelName
    )
)
BEGIN
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW 50000,
          N'Fotoğraf listesindeki otellerden biri bulunamadı.',
          1;
END;

-- Mevcut kapak işaretlerini kaldırır, kayıtları silmez.
UPDATE hi
SET hi.IsMain = 0
FROM HotelImages hi
JOIN Hotels h ON h.Id = hi.HotelId
WHERE h.Name IN
(
    SELECT DISTINCT HotelName
    FROM @Images
);

-- Daha önce eklenmiş URL kayıtlarını günceller.
UPDATE hi
SET
    hi.IsMain = i.IsMain,
    hi.DisplayOrder = i.DisplayOrder
FROM HotelImages hi
JOIN Hotels h ON h.Id = hi.HotelId
JOIN @Images i
    ON i.HotelName = h.Name
   AND i.ImageUrl = hi.ImageUrl;

-- Yalnızca eksik fotoğrafları ekler.
INSERT INTO HotelImages
(
    HotelId,
    ImageUrl,
    IsMain,
    DisplayOrder
)
SELECT
    h.Id,
    i.ImageUrl,
    i.IsMain,
    i.DisplayOrder
FROM @Images i
JOIN Hotels h ON h.Name = i.HotelName
WHERE NOT EXISTS
(
    SELECT 1
    FROM HotelImages hi
    WHERE hi.HotelId = h.Id
      AND hi.ImageUrl = i.ImageUrl
);

IF @@TRANCOUNT > 0
    COMMIT TRANSACTION;
GO

/* ============================================================
   16. FOTOĞRAF KONTROLÜ
   ============================================================ */

SELECT
    h.Name AS OtelAdi,
    COUNT(hi.Id) AS ToplamFotograf,
    SUM(CASE WHEN hi.IsMain = 1 THEN 1 ELSE 0 END) AS KapakFotografi
FROM Hotels h
LEFT JOIN HotelImages hi ON hi.HotelId = h.Id
WHERE h.Name IN
(
    N'Dedeman Konya',
    N'Divan Ankara',
    N'DoubleTree by Hilton Gaziantep',
    N'Hilton Bursa Convention Center',
    N'Novotel Kayseri',
    N'Ramada Plaza Trabzon',
    N'Ramada Resort Kusadasi',
    N'Swissotel The Bosphorus',
    N'Wyndham Grand Izmir Ozdilek',
    N'Yazz Collective Fethiye'
)
GROUP BY h.Name
ORDER BY h.Name;
GO
