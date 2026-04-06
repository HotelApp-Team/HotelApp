# HotelApp - Proje Kurgusu Yapilacaklar

## Tamamlanan
- [x] DB Kurgusu (11 tablo, FK, SP, View, Trigger, Index)
- [x] DB Diagram (ER Diagram - interaktif HTML)
- [x] DB Detayli Rapor (tablo aciklamalari, kolon detaylari)
- [x] Solution Yapisi (Onion Architecture, dosya agaci, katman referanslari)
- [x] API Endpoint Listesi (8 controller, 41 endpoint, detayli rapor)

## Yapilacak
- [x] Frontend Sayfa Plani (MVC - 19 sayfa, 12 controller, sayfa-API esleme, akis haritasi)
- [x] Authentication/Authorization Akisi (JWT flow, email dogrulama, beni hatirla, hesap kilitleme)
- [x] Risk ve Bagimliliklar (capraz bagimliliklar, kritik yol, teknik/takim/kapsam riskleri, 4 fazli plan)

## Kurgu Tamamlandiktan Sonra (Organizasyon)
- [x] Gorev Dagilimi (3 dikey ozellik grubu, katman bazli esit dagilim, birlestirme sureci)
- [x] Sprint/Milestone Plani (6 sprint, 06 Nisan - 15 Mayis, haftalik hedefler, milestone kriterleri)
- [x] Trello Board Yapisi (6 liste, 8 etiket, Sprint 1: 10 kart detayli, Sprint 2: 9 kart etiketli)
- [ ] Git Repo Baslangic (solution olusturma, main + dev + alper-dev + hilal-dev + sevde-dev branch'leri, ilk commit)

## Design Pattern Eklemeleri
- [ ] Logger Singleton Pattern
  - [ ] Logs tablosu (SQL Server: CREATE TABLE, 3 Index, FK -> Users)
  - [ ] LogEntry.cs entity (Domain/Entities)
  - [ ] LogDto.cs (Domain/DTOs)
  - [ ] ILoggerService.cs interface (Domain/Services)
  - [ ] LogRepository.cs Singleton implementasyonu (Infrastructure/Repositories)
  - [ ] HotelAppDbContext'e DbSet<LogEntry> eklenmesi
  - [ ] ExceptionLoggingMiddleware.cs (API/Middleware)
  - [ ] Program.cs: Singleton baslatma + DI kaydi
  - [ ] Controller'lara log cagrilari (Auth, Hotels, Reservations, Reviews, Admin)
  - [ ] Admin log goruntuleme endpoint'leri (GET /api/admin/logs, GET /api/admin/logs/{id})
  - [ ] Frontend: Admin log sayfasi

## Guncel Durum Notu
- Kickoff toplantisi yapildi.
- Kurgu asamasi tamamlandi.
- Grup A -> Alper
- Grup B -> Hilal
- Grup C -> Sevde
- Veritabani guncellemeleri: Reservations.PaymentMethod eklendi, Users.Phone ve Hotels.Phone zorunlu hale getirildi.