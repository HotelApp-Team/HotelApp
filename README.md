# HotelApp - Otel Rezervasyon Web Uygulamasi

Yazilim kursu final projesi. EtsTur / Hotels.com benzeri otel rezervasyon sistemi.

Kullanicilar sehir ve tarih araligina gore musait oda arayabilir, rezervasyon yapabilir,
konakladiklari otellere puan ve yorum birakabilir. Adminler otel, oda, rezervasyon,
kullanici ve yorumlari tek panelden yonetir; uygulama loglarini izleyebilir.

## Tech Stack

- **Backend:** C# / .NET 8 Web API
- **Frontend:** ASP.NET Core MVC, Bootstrap 5, JavaScript
- **Database:** SQL Server (EF Core - DB First / Scaffold)
- **Auth:** JWT Bearer + BCrypt password hash + rol bazli yetkilendirme (Admin/User)
- **Mimari:** Onion Architecture (Domain - Infrastructure - API - Web)
- **DevOps:** Docker, GitHub
- **Test:** xUnit (contract + SQL Server integration testleri)

## Solution Yapisi (Onion Architecture)

```
HotelApp/
├── HotelApp.Domain          Merkez katman: Entity'ler, DTO'lar, repository/service
│                            interface'leri ve service implementasyonlari (is kurallari).
│                            Hicbir dis teknolojiye bagimli degildir.
├── HotelApp.Infrastructure  EF Core DbContext, repository implementasyonlari,
│                            UnitOfWork, LoggerService. Domain interface'lerini uygular.
├── HotelApp.API             Web API: Controller'lar, JWT authentication, Swagger,
│                            global exception handler, background worker.
├── HotelApp.Web             MVC frontend: Razor view'lar, ApiService (API ile sadece
│                            HTTP uzerinden konusur, proje referansi yoktur),
│                            Auth/Admin filter'lari.
├── HotelApp.Tests           xUnit testleri: sprint contract testleri + DB entegrasyon.
└── Scripts/                 migrations/ (sema degisiklik script'leri) ve seeds/ (ornek veri).
```

Bagimlilik yonu hep ice dogru akar: API -> Infrastructure -> Domain.
Web katmani ise hicbir projeye referans vermez; API ile HTTP uzerinden iletisir.

## Takim

| Uye   | Rol          | Grup   | Sorumluluk                                                |
|-------|--------------|--------|-----------------------------------------------------------|
| Alper | Takim Lideri | Grup A | Kimlik & Yetkilendirme, Admin Yonetimi, Logger Singleton  |
| Hilal | Gelistirici  | Grup B | Otel Listeleme, Arama, Gorsel & Olanak Yonetimi           |
| Sevde | Gelistirici  | Grup C | Oda & Rezervasyon Yonetimi, Degerlendirme Sistemi         |

Her uye kendi alaninin veritabanindan arayuzune kadar tum katmanlarini gelistirdi.

## Branching Stratejisi

- `main`     - Stabil, teslime hazir kod (**teslim branch'i**)
- `dev`      - Integration / test branch
- `alper-dev`, `sevde-dev`, `hilal-dev`, `hilal-fix` - Gelistirici branch'leri

Akis: gelistirici branch -> PR -> code review -> `dev` merge -> `main` merge.
Merge oncesi build kontrolu zorunludur. Final teslim `main` branch'i uzerinden yapilir.

## Kurulum

### 1. Repo'yu klonla
```bash
git clone https://github.com/HotelApp-Team/HotelApp.git
cd HotelApp
```

### 2. SQL Server'i hazirla
Iki secenek var:

**A) Lokal SQL Server / SQL Express:**
- SQL Server (`.\SQLEXPRESS`) kurulu ve calisiyor olmali.

**B) Docker ile SQL Server:**
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<GucluBirSifre>" \
  -p 1433:1433 --name hotelapp-sql -d mcr.microsoft.com/mssql/server:2022-latest
```
Docker kullaniyorsan connection string'te `Server=localhost,1433;User Id=sa;Password=<GucluBirSifre>;` kullan.

### 3. Veritabanini olustur
- `HotelAppDB` adinda bos bir veritabani olustur.
- Sema script'ini calistir, ardindan `Scripts/migrations/` altindaki script'leri **tarih sirasiyla** uygula.
- Ornek veri icin `Scripts/seeds/` altindaki seed script'lerini calistir.
- Uygulama ilk acilista `Admin` ve `User` rollerini otomatik ekler (Program.cs role seed).

### 4. Connection string'i ayarla
`HotelApp.API/appsettings.json` (veya `appsettings.Development.json`) icinde:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=HotelAppDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
> Gizli bilgiler (gercek connection string, mail hesabi, JWT secret) repoya commit edilmez;
> lokal `appsettings.Development.json` veya user-secrets kullanilir.

### 5. Build ve calistir
```bash
dotnet restore
dotnet build
```
- Visual Studio'da once `HotelApp.API`'yi calistir (Swagger acilir: https://localhost:7274/swagger).
- Sonra `HotelApp.Web`'i calistir (ana sayfa: https://localhost:7050).
- Iki projeyi birlikte baslatmak icin solution'da multiple startup da secilebilir.

> **Port notu:** Port cakismasi durumunda ilgili projenin `Properties/launchSettings.json`
> dosyasindaki `applicationUrl` portunu degistir. API portunu degistirirsen
> `HotelApp.Web/appsettings.json` icindeki `ApiSettings:BaseUrl` degerini de guncelle.

## Swagger ve API Testi (JWT)

- API calisirken Swagger UI: `https://localhost:7274/swagger`
- Korumali endpoint'leri test etmek icin:
  1. `POST /api/auth/register` ile kullanici olustur (veya mevcut demo hesabiyla devam et).
  2. `POST /api/auth/login` ile giris yap; cevaptaki JWT token'i kopyala.
  3. Swagger'da sag ustteki **Authorize** butonuna token'i yapistir (`Bearer` on eki olmadan).
  4. Artik `[Authorize]` gerektiren endpoint'ler test edilebilir; Admin endpoint'leri icin Admin rollu hesap gerekir.

## Demo Hesaplari

> Guvenlik notu: Bu repo gercek credential icermez. Demo hesaplari her ortamda yerel olarak olusturulur.

- **User:** Kayit ol ekranindan yeni hesap olustur (e-posta dogrulama AuthSettings:UseEmailVerification ile kapatilabilir).
- **Admin:** Olusturdugun kullaniciya DB'de Admin rolu ata:
```sql
INSERT INTO UserRoles (UserId, RoleId)
SELECT u.Id, r.Id FROM Users u CROSS JOIN Roles r
WHERE u.Email = 'demo@ornek.com' AND r.Name = 'Admin';
```

## Demo Akisi

1. **Kayit / Giris:** Yeni kullanici kaydi, giris (JWT token uretilir).
2. **Arama:** Sehir + tarih araligi + misafir sayisi ile musait oda arama (sp_SearchAvailableRooms).
3. **Otel Detayi:** Gorseller, olanaklar, oda tipleri, yorumlar ve ortalama puan.
4. **Rezervasyon:** Tarih ve oda secimi, fiyat hesabi (gece sayisi + ekstra misafir), rezervasyon olusturma (sp_MakeReservation - eszamanlilik guvenli).
5. **Rezervasyonlarim:** Listeleme ve iptal.
6. **Yorum:** Sadece konaklamasi tamamlanmis kullanici puan + yorum birakabilir; otel ortalama puani trigger ile guncellenir.
7. **Admin Panel:** Dashboard istatistikleri, otel/oda tipi/olanak/rezervasyon/kullanici/yorum yonetimi, log izleme.

## Sayfalar

**Public (giris gerektirmez):**
- Ana sayfa (sehir kartlari, one cikan oteller)
- Otel arama ve listeleme
- Otel detay (odalar, olanaklar, yorumlar)
- Hakkimizda / Iletisim / SSS
- Kayit, giris, sifre sifirlama

**User (giris gerekir):**
- Rezervasyon olusturma (Booking)
- Rezervasyonlarim (listeleme, detay, iptal)
- Yorum ve puan verme
- Profil yonetimi (bilgi guncelleme, sifre degistirme)

**Admin (Admin rolu gerekir):**
- Admin Dashboard (istatistikler)
- Otel yonetimi, Oda tipi yonetimi, Olanak (amenity) yonetimi
- Rezervasyon yonetimi, Kullanici yonetimi, Yorum yonetimi
- Log izleme ekrani

## Ekran Goruntuleri

Ekran goruntuleri `docs/screenshots/` klasorunde tutulur
(ana sayfa, arama, otel detay, rezervasyon, admin dashboard).

## Testler

```bash
dotnet test
```
- Contract testleri (sprint bazli) dogrudan calisir.
- `HotelApp.Tests/Database/` altindaki entegrasyon testleri calisan bir SQL Server ister
  (test sema script'i: `HotelApp.Tests/Sql/hotelapp_test_schema_seed.sql`).

## Bilinen Kisitlar (MVP Disi)

Asagidakiler bilincli olarak kapsam disinda birakildi:

- **Odeme entegrasyonu yok** - odeme yontemi sadece kayit olarak tutulur, gercek tahsilat yapilmaz.
- **Harita entegrasyonu yok** - otel koordinatlari DB'de tutulur ama harita gosterimi yoktur.
- **Coklu dil destegi yok** - arayuz Turkce'dir.
- **Anlik bildirim yok** - e-posta disinda push/realtime bildirim bulunmaz.
