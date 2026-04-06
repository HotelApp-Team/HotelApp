# Final Proje ve Analiz

Kaynaklar:
- `final proje.txt`
- `final proje analizi.txt`

Bu dosya, iki kaynağın tek yerde toplanmış ve daha düzenli hale getirilmiş sürümüdür.

## 1. Teknik Altyapım
Şu ana kadar öğrendiğim veya kullandığım ana teknolojiler:
- C#
- .NET Core
- EF Core
- MSSQL
- HTML
- CSS
- JavaScript
- Bootstrap
- Docker
- Postman
- GitHub
- REST API
- Web API

## 2. Kullandığım Araçlar
- Visual Studio 2026
- Visual Studio 2022
- SQL Server Management Studio 2022
- Docker
- Postman

## 3. Öğrendiğim Konular
- SOLID prensipleri
- LINQ / Lambda sorguları
- OOP
- Design Pattern
- HTML / CSS / JS / Bootstrap
- Döngüler
- Methodlar
- Inheritance
- Polymorphism
- CRUD
- Onion Architecture

## 4. Referans Proje Bilgisi
Kursta verilen referans backend projesi:
- https://github.com/alprclbi/examples/tree/main/Author_Books

Bu referans proje backend yapısını anlamak için kullanılacak.
DB ve Docker tarafında ek kurgular proje içinde ayrıca yapılacak.

### Referans proje yapısı
- `StudentDemo.Core`
  - Entity
  - DTO
  - Interface
  - Service
- `StudentDemo.Data`
  - AppDbContext
  - Repository pattern
  - UnitOfWork
  - Migrations
- `StudentDemo.API`
  - Controller'lar
  - Dockerfile
- Docker Compose ile deployment

## 5. Final Proje Özeti
### Takım
- Alper
- Sevde
- Hilal

### Grup atamaları
- Grup A: Alper
- Grup B: Hilal
- Grup C: Sevde

### Rolüm
- Team lead benim
- Proje planlama
- Görev dağılımı
- GitHub repo oluşturma
- Herkese ayrı branch açma
- Branch'leri kontrol etme
- Son birleştirmeyi yönetme

### Proje konusu
Ets Tur, Hotels, Tatil Budur benzeri bir otel rezervasyon web uygulaması.

### Ana kapsam
- Database kurgusu
- Web API
- Frontend

## 6. Fonksiyonel Gereksinimler
- Otel CRUD işlemleri
- Otel ile odaların ilişkilendirilmesi
- Oda listesinin ortak tutulması
- N-N ilişki kurulması
- Oteller, otel özellikleri, görseller ve odalar
- Oda müsaitlik kontrolü
- Rezervasyon sistemi
- Üyelik sistemi
- Sadece kayıtlı müşterinin rezervasyon yapabilmesi
- Admin tarafında müşteri listeleri
- Dashboard ve raporlama
- Müşteri segmentasyonu
- Çıkış sonrası puan ve yorum sistemi

### Oda ve ilişki mantığı
Önemli not:
- Oda kayıtları tekrar tekrar açılmamalı
- Oda tipleri ortak tutulmalı
- Otel ile oda ilişkisi ara yapı üzerinden kurulmalı
- Bu yüzden N-N ilişki mantığı doğru kurgulanmalı

### Rezervasyon mantığı
- Kullanıcı rezervasyon öncesinde hangi odanın dolu / boş olduğunu görmeli
- Oda tipine göre müsaitlik kontrolü yapılmalı
- Örneğin Deluxe oda doluysa sistem bunu doğru göstermeli

### Üyelik ve yönetim
- Sadece kayıtlı kullanıcı rezervasyon yapabilmeli
- Admin tarafında kullanıcı listesi görülebilmeli
- Kullanıcı davranışına göre segment mantığı düşünülebilir
  - Premium müşteri
  - Orta müşteri
  - Düşük müşteri

### Değerlendirme sistemi
- Müşterinin giriş / çıkış tarihi olacak
- Çıkış sonrası oteli puanlayabilecek
- Yorum bırakabilecek

## 7. Proje Yönetimi ve Takip Şekli
- Trello kullanılacak
- Hoca da yetkili olacak ve süreci takip edecek
- Önce proje analizi yapılacak
- Sonra proje planı çıkarılacak
- Sonra iş paketleri zamana göre bölünecek
- Süreç boyunca kim ne yapıyor net takip edilecek
- Bitişten sonra demo yapılacak

### Görev dağılımı prensibi
- Herkese her katmanda eşit görev dağılımı yapılacak
- Kimse sadece frontend ya da sadece backend tarafında kalmayacak
- Herkes tüm yapıyı öğrenerek ilerleyecek

## 8. Git ve Branch Planı
- GitHub repo olacak
- Takım erişimi olacak
- Main branch korunacak
- Main üzerinde doğrudan geliştirme yapılmayacak
- dev, alper-dev, hilal-dev, sevde-dev branch'leri açılacak
- Herkes kendi branch'inde geliştirme yapacak, tamamlanan işler önce dev branch'inde birleştirilecek
- Sonra kontrollü şekilde birleştirme yapılacak

## 8.1 Güncel Durum
- Kurgu aşaması tamamlandı
- Kickoff toplantısı yapıldı
- Görev paylaşımı netleşti
- Veritabanında Reservations.PaymentMethod eklendi
- Users.Phone ve Hotels.Phone alanları zorunlu hale getirildi

## 9. Teknik Hedefler
- Proje Docker'da yayınlanacak
- Domain alınacak
- Backend katmanlı mimari ile kurulacak
- Frontend daha güçlü ve gelişmiş görünecek şekilde tasarlanmak isteniyor
- DB dinamik bir yapıda olacak

## 10. Risk Notu
Proje başarısız olursa lider olarak ben de doğrudan etkilenirim.
Bu yüzden proje planının sağlam, kontrollü ve sürdürülebilir olması gerekiyor.

## 11. HotelApp Ajanından Beklenti
HotelApp ajanı:
- uzman yazılımcı gibi davranmalı
- tecrübeli olmalı
- öğretici olmalı
- mentor gibi yol göstermeli
- proje kurgusunda yardımcı olmalı
- teknik liderlik desteği vermeli

## 12. Hazırlık Analizi
### Hazır olduğum konular
- C# ve OOP temelleri
- SOLID ve Design Pattern bilgisi
- MVC ve CRUD deneyimi
- EF Core + DB First (Scaffold)
- Onion Architecture referans bilgisi
- Docker deneyimi

### Geliştirilmesi gereken konular
- Veritabanı tasarımı
- Authentication / Authorization
- N-N ilişki yönetimi
- Stored Procedure, View, Trigger kullanımı
- Admin Dashboard
- Frontend ile API entegrasyonu

## 13. Önerilen Başlangıç Sırası
1. SQL Server'da veritabanı şemasını tasarla
2. Solution yapısını kur
3. Scaffold ile entity üret
4. Repository + Service katmanlarını kur
5. API controller'larını yaz
6. Authentication ekle
7. Frontend geliştir
8. Docker entegrasyonunu tamamla

## 14. Netleştirilecek Nokta
Hocanın "iş ... açıyorsunuz, oradan da ben takip edeceğim" dediği bölüm tam anlaşılmamış.
Bu ifade muhtemelen issue, task, PR veya proje takip ekranlarından birini işaret ediyor olabilir.
Daha sonra netleştirilmesi faydalı olur.
