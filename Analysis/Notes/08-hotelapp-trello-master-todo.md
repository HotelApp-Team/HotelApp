# HotelApp - Trello Master TODO

Bu dosya, Trello board durumunu yerelde takip etmek ve hem insan hem de yapay zeka araclarinin projede nerede kalindigini hizli anlamasi icin hazirlandi.

## 1. Hizli Durum Ozeti

- Trello board: `Otel Rezervasyon App`
- Snapshot tarihi: `2026-04-06`
- Tamamlanan kart: `1`
- Devam eden kart: `1`
- Sprint Backlog karti: `8`
- Backlog karti: `10`
- Review/PR karti: `0`
- Testing karti: `0`

## 2. Nerde Kaldik?

Trello'ya gore su an durum su:

- Biten son kart: `S1-01: Kickoff Toplantisi + Grup Atamalari`
- Aktif kart: `S1-02: Git Repo + Branch Stratejisi`
- Sprint 1'in teknik kurulum kartlari (`S1-03` ile `S1-10` arasi) henuz `Sprint Backlog` asamasinda duruyor.
- Sprint 2 feature kartlari (`S2-01` ile `S2-10` arasi) henuz `Backlog` asamasinda duruyor.

Kisa yorum:
- Organizasyon basladi.
- Kickoff tamamlandi.
- Repo/branch kurulumu Trello'da aktif gorunuyor.
- Kodlama ve teknik altyapi kartlari Trello tarafinda henuz tamamlanmis olarak isaretlenmemis.

## 3. Onceki Kararlardan Bilinen Proje Notlari

Bu notlar, onceki proje kararlarindan geliyor ve Trello ile birlikte okunmali:

- Kurgu asamasi tamamlandi.
- Kickoff toplantisi yapildi.
- Gorev dagilimi netlestirildi:
  - Grup A -> Alper
  - Grup B -> Hilal
  - Grup C -> Sevde
- Veritabani guncellemeleri yapildi:
  - `Reservations.PaymentMethod` eklendi
  - `Users.Phone` zorunlu hale getirildi
  - `Hotels.Phone` zorunlu hale getirildi
- Hedef branch yapisi su sekilde:
  - `main`
  - `dev`
  - `alper-dev`
  - `hilal-dev`
  - `sevde-dev`

Not:
- Trello durumu ile onceki proje notlari birebir ayni olmayabilir.
- Ozellikle `S1-02: Git Repo + Branch Stratejisi` karti Trello'da halen checklist `0/9` gorunuyor. Yani bazi kararlar alinmis olsa bile Trello checklistleri henuz guncellenmemis olabilir.

## 4. Genel Faz Gorunumu

| Faz | Durum | Aciklama |
|---|---|---|
| Kurgu ve planlama | Tamamlandi | Kickoff, dagilim, kapsam ve genel kurgu tamamlandi |
| Repo ve branch kurulumu | Devam ediyor | Trello'ya gore aktif is `S1-02` |
| Veritabani kurulumu ve SQL nesneleri | Sirada | `S1-03` Sprint Backlog'ta |
| Solution ve Onion Architecture kurulumu | Sirada | `S1-04` Sprint Backlog'ta |
| EF Core Scaffold ve temel konfigurasyon | Sirada | `S1-05` ve `S1-06` Sprint Backlog'ta |
| Auth altyapisi | Sirada | `S1-07` ve `S2-01`/`S2-02` ile ilerleyecek |
| MVC ortak altyapi ve layout | Sirada | `S1-08`, `S1-09`, `S1-10` |
| Feature gelistirme | Bekliyor | Hotel, RoomType, HotelRoom, Email, SP test ve Logger kartlari Backlog'ta |
| Review/PR | Baslamadi | Trello listesi bos |
| Testing | Baslamadi | Trello listesi bos |

## 5. Trello Listeleri ve Kartlar

### Done

#### [x] S1-01: Kickoff Toplantisi + Grup Atamalari
- Durum: Done
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 4/4 tamamlandi
- Alt gorevler:
  - [x] Grup atamalari (A/B/C) belirle
  - [x] Trello board tanitimi
  - [x] Sprint 1 hedeflerini gozden gecir
  - [x] Git branch stratejini anlat

### In Progress

#### [ ] S1-02: Git Repo + Branch Stratejisi
- Durum: In Progress
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 0/9 tamamlandi
- Alt gorevler:
  - [ ] GitHub'da repo olustur
  - [ ] .gitignore dosyasi ekle (Visual Studio sablonu)
  - [ ] main branch'i olustur
  - [ ] develop branch'i olustur
  - [ ] feature/grup-a branch'i olustur
  - [ ] feature/grup-b branch'i olustur
  - [ ] feature/grup-c branch'i olustur
  - [ ] README.md ilk versiyonunu ekle
  - [ ] Takim uyelerini repo'ya collaborator olarak ekle

### Sprint Backlog

#### [ ] S1-03: SQL Script Calistirma (11 Tablo)
- Durum: Sprint Backlog
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 0/18 tamamlandi
- Tablolar:
  - [ ] Users tablosu
  - [ ] Roles tablosu
  - [ ] UserRoles tablosu
  - [ ] Hotels tablosu
  - [ ] HotelImages tablosu
  - [ ] Amenities tablosu
  - [ ] HotelAmenities tablosu
  - [ ] RoomTypes tablosu
  - [ ] HotelRooms tablosu
  - [ ] Reservations tablosu
  - [ ] Reviews tablosu
- DB Nesneleri:
  - [ ] sp_SearchAvailableRooms
  - [ ] sp_MakeReservation
  - [ ] vw_HotelSummary
  - [ ] vw_ReservationDetails
  - [ ] trg_UpdateHotelRating
  - [ ] Index'ler
  - [ ] Seed data (test verileri)

#### [ ] S1-04: Solution Olusturma (Onion Architecture)
- Durum: Sprint Backlog
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 0/7 tamamlandi
- Alt gorevler:
  - [ ] HotelApp.Domain projesi olustur (Class Library)
  - [ ] HotelApp.Infrastructure projesi olustur (Class Library)
  - [ ] HotelApp.API projesi olustur (Web API)
  - [ ] HotelApp.Web projesi olustur (MVC)
  - [ ] Backend katman referanslarini ayarla (Domain <- Infrastructure <- API)
  - [ ] NuGet paketlerini yukle (EF Core, JWT, BCrypt vb.)
  - [ ] Solution build kontrolu

#### [ ] S1-05: EF Core Scaffold (Entity Uretimi)
- Durum: Sprint Backlog
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 0/6 tamamlandi
- Alt gorevler:
  - [ ] Connection string'i appsettings.json'a ekle
  - [ ] Scaffold komutunu calistir
  - [ ] Uretilen entity'leri kontrol et (11 tablo = 11 entity)
  - [ ] DbContext dosyasini incele
  - [ ] Partial class yapisi icin klasor hazirla
  - [ ] Build kontrolu

#### [ ] S1-06: Program.cs Konfigurasyonu
- Durum: Sprint Backlog
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 0/8 tamamlandi
- Alt gorevler:
  - [ ] DbContext DI kaydini ekle
  - [ ] JWT Authentication ayarlarini ekle
  - [ ] CORS policy tanimla (development icin genis izin)
  - [ ] Swagger/OpenAPI konfigurasyonu
  - [ ] appsettings.json'a JWT SecretKey, Issuer, Audience ekle
  - [ ] Middleware sirasini ayarla (CORS, Auth, Swagger)
  - [ ] Build ve calistirma kontrolu
  - [ ] Swagger UI aciliyor mu test et

#### [ ] S1-07: JWT + Auth API (Register/Login)
- Durum: Sprint Backlog
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 0/12 tamamlandi
- Alt gorevler:
  - [ ] IAuthRepository interface (Domain)
  - [ ] AuthRepository implementasyonu (Infrastructure)
  - [ ] JwtService - token uretme metodu (Domain)
  - [ ] AuthService - Register metodu (BCrypt hash)
  - [ ] AuthService - Login metodu (BCrypt verify + token)
  - [ ] RegisterDto, LoginDto, TokenResponseDto (Domain)
  - [ ] AuthController - E01 Register endpoint
  - [ ] AuthController - E02 Login endpoint
  - [ ] Program.cs'e DI kayitlarini ekle
  - [ ] Swagger'da Register test et
  - [ ] Swagger'da Login test et - token donuyor mu?
  - [ ] Token ile [Authorize] endpoint'e erisim testi

#### [ ] S1-08: Layout'lar (_Layout + _AdminLayout)
- Durum: Sprint Backlog
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 0/8 tamamlandi
- Alt gorevler:
  - [ ] MVC projesine Bootstrap 5 CDN ekle
  - [ ] _Layout.cshtml - navbar (logo, menu linkleri)
  - [ ] _Layout.cshtml - footer
  - [ ] _Layout.cshtml - responsive tasarim
  - [ ] _AdminLayout.cshtml - admin sidebar menu
  - [ ] _AdminLayout.cshtml - ust bar
  - [ ] wwwroot/css/site.css temel stiller
  - [ ] Tarayicida layout'larin dogru render oldugunu kontrol et

#### [ ] S1-09: ApiService + AuthFilter + AdminFilter
- Durum: Sprint Backlog
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 0/8 tamamlandi
- Alt gorevler:
  - [ ] ApiService sinifi olustur (HttpClient wrapper)
  - [ ] ApiService - GET, POST, PUT, DELETE metodlari
  - [ ] ApiService - JWT token'i header'a ekleme
  - [ ] ApiService - hata yonetimi (try-catch)
  - [ ] Program.cs'e ApiService DI kaydini ekle
  - [ ] AuthFilter - giris yapmamis kullaniciyi Login'e yonlendir
  - [ ] AdminFilter - admin olmayani ana sayfaya yonlendir
  - [ ] appsettings.json'a API base URL ekle

#### [ ] S1-10: Pagination Partial + Build Kontrolu
- Durum: Sprint Backlog
- Sorumlu: Alper, Hilal, Sevde
- Checklist: 0/7 tamamlandi
- Alt gorevler:
  - [ ] _Pagination.cshtml partial view olustur
  - [ ] Sayfa numaralari + onceki/sonraki butonlari
  - [ ] Tum solution clean build kontrolu
  - [ ] Swagger'da tum endpoint'lerin gorunurlugu
  - [ ] MVC projesini calistir - layout dogru mu?
  - [ ] develop branch'e commit + push
  - [ ] Takim uyelerinin pull yapabildigini dogrula

### Backlog

#### [ ] S2-01: Auth Backend (Domain+Infra+App)
- Durum: Backlog
- Sorumlu: Alper
- Checklist: yok

#### [ ] S2-02: AuthController Tamamlama (E03, E42, E43)
- Durum: Backlog
- Sorumlu: Alper
- Checklist: yok

#### [ ] S2-03: EmailService (SMTP/Mailtrap)
- Durum: Backlog
- Sorumlu: Alper
- Checklist: yok

#### [ ] S2-04: Hotel Backend (Domain+Infra+App)
- Durum: Backlog
- Sorumlu: Hilal
- Checklist: yok

#### [ ] S2-05: HotelsController API (E04-E10)
- Durum: Backlog
- Sorumlu: Hilal
- Checklist: yok

#### [ ] S2-06: HotelImages + Amenities API
- Durum: Backlog
- Sorumlu: Hilal
- Checklist: yok

#### [ ] S2-07: RoomType + HotelRoom Backend
- Durum: Backlog
- Sorumlu: Sevde
- Checklist: yok

#### [ ] S2-08: RoomTypes + HotelRooms API (E11-E15, E20-E22)
- Durum: Backlog
- Sorumlu: Sevde
- Checklist: yok

#### [ ] S2-09: SP Test (sp_SearchAvailableRooms)
- Durum: Backlog
- Sorumlu: Sevde
- Checklist: yok

#### [ ] S2-10: Logger Singleton Pattern (Domain+Infra+API)
- Durum: Backlog
- Sorumlu: Alper
- Checklist: 0/11 tamamlandi
- Kapsam:
  - [ ] Logs tablosu (SQL Server: CREATE TABLE, indexler, FK -> Users)
  - [ ] LogEntry.cs entity
  - [ ] LogDto.cs
  - [ ] ILoggerService.cs interface
  - [ ] LogRepository.cs Singleton implementasyonu
  - [ ] HotelAppDbContext'e DbSet<LogEntry> eklenmesi
  - [ ] ExceptionLoggingMiddleware.cs
  - [ ] Program.cs DI ve Singleton kaydi
  - [ ] Controller'lara log cagrilari
  - [ ] Admin log goruntuleme endpoint'leri
  - [ ] Frontend admin log sayfasi

### Review/PR

- Bu listede acik kart yok.

### Testing

- Bu listede acik kart yok.

## 6. Yerel Guncelleme Kurali

Bu dosya her is bitiminde su mantikla guncellenmeli:

1. Trello karti hangi listedeyse burada da ayni duruma cek.
2. Checklist'te biten maddeyi burada da `[x]` yap.
3. Kart sahibi degistiyse `Sorumlu` alanini guncelle.
4. Kart `Done` oldugunda bir ust bolumdeki `Nerde Kaldik?` kismini da yenile.
5. Trello'da yapilip burada islenmeyen bir sey kalmamali.
6. Eger burada yapildi ama Trello'da isaretlenmedi ise `Not` satiri dus.

## 7. Sonraki Net Adim

Trello snapshot'ina gore bir sonraki net operasyonel adim:

- Once `S1-02: Git Repo + Branch Stratejisi` kartini gercek durumunuza gore guncelleyin.
- Ardindan `S1-03`, `S1-04`, `S1-05` ve `S1-06` kartlari ile teknik kurulum akisina gecin.

Bu dosya, Trello board'un yerel markdown aynasi olarak kullanilmak uzere hazirlandi.
