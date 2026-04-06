# Onion Mimari - Bağımlılık Yönü ve İş Akışı

Bu not, Onion Architecture içindeki bağımlılık yönü ile gerçek çalışma akışı arasındaki farkı açıklamak için oluşturuldu.

## Soru
`API -> Infrastructure -> Core` yönü ne demek?
API'den sonra doğrudan Infrastructure katmanına gidilmesi güvenlik zafiyeti mi oluşturur?
Yoksa asıl çalışma akışı `API -> Core -> Infrastructure -> Core -> API` gibi mi düşünülmelidir?

## Kısa Cevap
Şu an kurgulanan 3 katmanlı yapı genel olarak doğrudur.

- **Core / Domain**
  - Entity
  - DTO
  - Interface
  - Service
  - Business logic
- **Infrastructure**
  - DbContext
  - Repository
  - UnitOfWork
  - Veritabanı erişimi
- **API**
  - Controller
  - Middleware
  - Authentication
  - Dependency Injection

## 1. Bağımlılık Yönü Ne Demek?
`API -> Infrastructure -> Core` ifadesi çoğu zaman **istek akışını değil**, **katmanların birbirine olan referans / bağımlılık yönünü** anlatır.

Yani mantık şu şekildedir:
- API dış katmandır
- Infrastructure orta katmandır
- Core merkezdeki katmandır
- Dış katmanlar iç katmanları kullanır
- İç katmanlar dış katmanları bilmez

Bu yüzden:
- API, Core'u kullanabilir
- Infrastructure, Core'daki interface ve modelleri kullanabilir
- Core ise dış katmanlara bağımlı değildir

## 2. Gerçek İş Akışı Nasıl Düşünülmeli?
Runtime sırasında işleyişi zihinde şu şekilde kurmak daha doğrudur:

1. Kullanıcı istek atar
2. API Controller isteği alır
3. Controller işi Core içindeki Service'e verir
4. Service business logic'i çalıştırır
5. Gerekirse repository / interface üzerinden veri ister
6. Infrastructure katmanı veritabanı işlemini yapar
7. Sonuç tekrar Service'e döner
8. Sonuç API tarafından response olarak döndürülür

Kısaca gerçek iş akışı şu şekilde düşünülebilir:

`API -> Core -> Infrastructure -> Core -> API`

Bu ifade, bağımlılık şemasından daha çok çalışma mantığını anlatır.

## 3. Güvenlik Zafiyeti Nerede Oluşur?
Sadece API'nin Infrastructure'a referans vermesi tek başına güvenlik zafiyeti oluşturmaz.

Risk daha çok şu durumda ortaya çıkar:
- Controller doğrudan `DbContext` kullanırsa
- Controller doğrudan repository çağırırsa
- Business rule'lar service katmanında değil, rastgele yerlerde dağılırsa
- Core'daki iş kuralları bypass edilirse

Yani sorun şurada değildir:
- "Infrastructure var, o yüzden güvensiz"

Asıl sorun şurada olur:
- "API, business logic'i atlayıp veritabanına direkt dalıyor"

## 4. Doğru Kullanım Şekli
En sağlıklı yaklaşım şöyledir:

- **API**
  - isteği alır
  - doğrulama ve yönlendirme yapar
  - response döndürür
- **Core**
  - iş kurallarını yürütür
  - business logic burada yaşar
- **Infrastructure**
  - veritabanı ve teknik erişim işlemlerini yapar

Pratik akış:
- Controller doğrudan DbContext'e gitmez
- Önce Service'e gider
- Service gerekirse repository interface'lerini kullanır
- Repository implementasyonu Infrastructure katmanında çalışır

## 5. Onion Mimarinin Asıl Fikri
Onion mimaride asıl merkez veritabanı değildir.
Merkezde **iş mantığı** bulunur.

Bu sayede:
- Veritabanı değişse bile business logic bozulmaz
- SQL Server yerine PostgreSQL kullanılsa bile en çok etkilenen yer Infrastructure olur
- Core tarafındaki kurallar büyük ölçüde aynı kalır
- Sistem daha test edilebilir olur

## 6. Bu Projedeki Sonuç
Bu projede kurulan 3 katmanlı yapı referans projeye uygundur.
Genel kurgu doğrudur.

Yanlış olacak senaryo şudur:
- API katmanı business logic'i atlar
- doğrudan DbContext veya repository ile çalışır
- kurallar service katmanında toplanmaz

Doğru senaryo ise şudur:
- API isteği alır
- Core iş mantığını yürütür
- Infrastructure teknik veritabanı işlemini yapar
- sonuç tekrar yukarı döner

## Sonuç Cümlesi
Kurgulanan sistem temelden yanlış değildir.
Tam tersine, mantığı doğru kurulmuştur.
Dikkat edilmesi gereken nokta, Controller'ın doğrudan veritabanına gitmemesi ve business logic'in Core içinde korunmasıdır.
