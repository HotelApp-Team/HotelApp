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

Yakinda eklenecek.
