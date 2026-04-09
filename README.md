# HotelApp - Otel Rezervasyon Web Uygulamasi

Yazilim kursu final projesi. EtsTur / Hotels.com benzeri otel rezervasyon sistemi.

## Tech Stack

- **Backend:** C# / .NET 8 Web API
- **Frontend:** ASP.NET Core MVC, Bootstrap 5
- **Database:** SQL Server (EF Core - DB First / Scaffold)
- **Mimari:** Onion Architecture (Domain - Infrastructure - API - Web)
- **DevOps:** Docker, GitHub

## Solution Yapisi

```
HotelApp/
├── HotelApp.Domain          (Entity'ler, interface'ler)
├── HotelApp.Infrastructure  (EF Core, Repository implementasyonlari)
├── HotelApp.API             (Web API, JWT Authentication, Swagger)
└── HotelApp.Web             (MVC frontend)
```

## Takim

| Uye   | Rol           | Grup   | Sorumluluk                                                                |
|-------|---------------|--------|---------------------------------------------------------------------------|
| Alper | Takim Lideri  | Grup A | Kimlik & Yetkilendirme, Admin Yonetimi, Logger Singleton                  |
| Hilal | Gelistirici   | Grup B | Otel Listeleme, Arama, Gorsel & Olanak Yonetimi                           |
| Sevde | Gelistirici   | Grup C | Oda & Rezervasyon Yonetimi, Degerlendirme Sistemi                         |

## Branching Stratejisi

- `main`     - Stabil, deploy-ready kod
- `dev`      - Integration / test branch
- `alper-dev`, `sevde-dev`, `hilal-dev` - Gelistirici branch'leri

Akis: dev branch -> PR -> code review -> dev merge -> main merge

## Kurulum

### 1. Repo'yu klonla
```bash
git clone https://github.com/alprclbi/HotelApp.git
cd HotelApp
```

### 2. Kendi branch'ine gec
```bash
git checkout sevde-dev   # veya hilal-dev
```

### 3. SQL Server
- SQL Server (.\SQLEXPRESS) kurulu ve calisiyor olmali
- `HotelAppDB` veritabani olusturulmus olmali (SQL script takim liderinden gelecek)

### 4. Build ve calistir
- VS'de Solution'i ac, Build -> Build Solution (Ctrl+Shift+B)
- NuGet paketleri otomatik restore edilir
- HotelApp.API'yi startup olarak sec, F5 ile calistir
- Swagger UI acilirsa kurulum basarili

> **Not:** Port cakismasi durumunda (ornegin 7274 portu baska uygulama tarafindan kullaniliyorsa) ilgili projenin `Properties/launchSettings.json` dosyasindaki `applicationUrl` satirindaki port numarasini degistir. Web projesinin portunu degistirirsen `HotelApp.Web/appsettings.json` icindeki `ApiSettings:BaseUrl` portunu da ayni sekilde guncelle.
