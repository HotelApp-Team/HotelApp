# View, Stored Procedure, Trigger ve Index

Kaynak:
- `View, SP.txt`

Bu dosya, veritabanı tarafında sık geçen dört temel kavramı sade şekilde açıklar.

## 1. View (Görünüm)
View, sık kullanılan karmaşık `SELECT` sorgularına isim verilmiş halidir.
Tablo gibi sorgulanır ama kendi başına veri saklamaz.

### Örnek
`vw_HotelSummary`
- Hotels ve Reviews tablolarını birleştirir
- Ortalama puan gibi özet bilgi üretebilir

### Avantajı
- Uzun sorguyu tekrar tekrar yazmazsın
- Kod daha okunur olur

### Önemli not
View veri tutmaz.
Her çağrıldığında sorgu yeniden çalışır.

## 2. Stored Procedure (SP - Saklı Yordam)
Stored Procedure, veritabanına kaydedilen ve parametre alıp işlem yapabilen SQL bloklarıdır.
Fonksiyon gibi düşünülebilir.

### Örnekler
#### `sp_SearchAvailableRooms`
- şehir ve tarih alır
- müsait odaları döner

#### `sp_MakeReservation`
- müsaitlik kontrolü yapar
- fiyat hesaplar
- rezervasyon oluşturur
- tüm işlemi tek çağrıda yapar

### Kullanım mantığı
Uygulama tarafından örneğin şu şekilde çağrılabilir:
`EXEC sp_MakeReservation @UserId=5, @HotelRoomId=3, ...`

## 3. Trigger (Tetikleyici)
Trigger, bir tabloda `INSERT`, `UPDATE` veya `DELETE` olduğunda otomatik çalışan veritabanı kodudur.
Sen elle çağırmazsın, veritabanı kendisi tetikler.

### Örnek
`trg_UpdateHotelRating`
- Reviews tablosuna yeni yorum gelince
- Hotels tablosundaki ortalama puanı otomatik günceller

Mantık:
- sen sadece yorumu eklersin
- puan güncellemesini trigger yapar

## 4. Index (Dizin)
Index, veritabanında sık aranan kolonlar için oluşturulan hızlandırıcı yapıdır.
Kitabın arkasındaki dizin gibi düşünülebilir.

### Örnek
`IX_Hotels_City`
- şehre göre yapılan aramaları hızlandırır

### Avantajı
- sorgular daha hızlı çalışır

### Dezavantajı
- `INSERT` ve `UPDATE` işlemleri biraz yavaşlayabilir
- çünkü index de güncellenmek zorundadır

## Kısa Özet
- **View** = isim verilmiş sorgu
- **Stored Procedure** = parametreli işlem bloğu
- **Trigger** = otomatik çalışan veritabanı kodu
- **Index** = sorgu hızlandırıcı yapı
