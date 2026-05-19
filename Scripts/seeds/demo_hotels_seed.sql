-- Optional demo seed data used by public hotel UI contract tests.

IF NOT EXISTS (SELECT 1 FROM Hotels WHERE Name = 'Demo Istanbul Palace')
BEGIN
    INSERT INTO Hotels (Name, Description, Address, City, Country, StarRating, Phone, Email)
    VALUES
    ('Demo Istanbul Palace', 'Demo hotel', 'Demo Address 1', 'Istanbul', 'Turkiye', 5, '02120000001', 'demo01@hotelapp.test'),
    ('Demo Ankara Business', 'Demo hotel', 'Demo Address 2', 'Ankara', 'Turkiye', 4, '03120000001', 'demo02@hotelapp.test'),
    ('Demo Izmir Resort', 'Demo hotel', 'Demo Address 3', 'Izmir', 'Turkiye', 5, '02320000001', 'demo03@hotelapp.test'),
    ('Demo Antalya Beach', 'Demo hotel', 'Demo Address 4', 'Antalya', 'Turkiye', 5, '02420000001', 'demo04@hotelapp.test'),
    ('Demo Bursa City', 'Demo hotel', 'Demo Address 5', 'Bursa', 'Turkiye', 4, '02240000001', 'demo05@hotelapp.test'),
    ('Demo Konya Selcuk', 'Demo hotel', 'Demo Address 6', 'Konya', 'Turkiye', 4, '03320000001', 'demo06@hotelapp.test'),
    ('Demo Kayseri Mountain', 'Demo hotel', 'Demo Address 7', 'Kayseri', 'Turkiye', 4, '03520000001', 'demo07@hotelapp.test'),
    ('Demo Trabzon Sea', 'Demo hotel', 'Demo Address 8', 'Trabzon', 'Turkiye', 5, '04620000001', 'demo08@hotelapp.test'),
    ('Demo Gaziantep Center', 'Demo hotel', 'Demo Address 9', 'Gaziantep', 'Turkiye', 4, '03420000001', 'demo09@hotelapp.test'),
    ('Demo Aydin Coast', 'Demo hotel', 'Demo Address 10', 'Aydin', 'Turkiye', 5, '02560000001', 'demo10@hotelapp.test');
END;
GO

INSERT INTO HotelImages (HotelId, ImageUrl, IsMain, DisplayOrder)
SELECT h.Id, CONCAT('https://example.com/demo-hotel-', h.Id, '.jpg'), 1, 0
FROM Hotels h
WHERE h.Name LIKE 'Demo %'
  AND NOT EXISTS (
      SELECT 1
      FROM HotelImages i
      WHERE i.HotelId = h.Id
  );
GO

IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = 'Demo WiFi')
BEGIN
    INSERT INTO Amenities (Name, Icon)
    VALUES
    ('Demo WiFi', 'wifi'),
    ('Demo Otopark', 'car-front-fill'),
    ('Demo Kahvalti', 'cup-hot');
END;
GO

INSERT INTO HotelAmenities (HotelId, AmenityId)
SELECT h.Id, a.Id
FROM Hotels h
CROSS JOIN Amenities a
WHERE h.Name LIKE 'Demo %'
  AND a.Name IN ('Demo WiFi', 'Demo Otopark')
  AND NOT EXISTS (
      SELECT 1
      FROM HotelAmenities ha
      WHERE ha.HotelId = h.Id
        AND ha.AmenityId = a.Id
  );
GO
