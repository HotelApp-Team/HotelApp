# Trello Etiketler

Kaynak:
- `trello etiketler.txt`

Bu dosya, Trello üzerindeki etiketlerin ne işe yaradığını açıklar.

## Etiketler ne işe yarar?
Etiketler kartların üzerinde renkli işaretler olarak görünür.
Bir karta bakınca hızlıca iki şeyi anlamayı sağlar:
- Kimin yapacağı
- Hangi alanda yapılacağı

## 1. Grup Etiketleri - Kim yapacak?
### Grup A (mavi)
Bu iş Kimlik + Yönetim grubunundur.

### Grup B (yeşil)
Bu iş Otel Vitrin grubunundur.

### Grup C (pembe)
Bu iş İşlemler grubunundur.

### Ortak (sarı)
Bu iş 3 kişi birlikte yapılacaktır.

## 2. Katman Etiketleri - Nerede yapılacak?
### DB (mor)
SQL ve veritabanı işi olduğunu gösterir.

### Backend (turuncu)
C# API kodu tarafında yapılacak işi gösterir.

### Frontend (açık mavi)
MVC sayfa, view veya arayüz tarafında yapılacak işi gösterir.

### Kritik (kırmızı)
Bu işin acil olduğunu ve gecikirse başka işleri de tıkayacağını gösterir.

## Örnek Okuma
Bir kartta şu etiketler varsa:
- Grup B
- Backend

Anlamı şudur:
- Bu kart Grup B'nin işidir
- Bu iş backend tarafında yapılacaktır

## Filtre Mantığı
Trello'da filtre özelliği ile:
- sadece Grup A kartları
- sadece Backend kartları
- sadece Kritik kartlar

gibi daraltma yapılabilir.

Bu sayede herkes sadece kendi işlerine veya görmek istediği kartlara odaklanabilir.
