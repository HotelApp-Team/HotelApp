# Trello Kullanım Akışı

Kaynak:
- `trello kullanım.txt`

Bu dosya, sprint başladığında ve günlük çalışmada Trello'nun nasıl kullanılacağını açıklar.

## 1. Sprint Başlangıcı
Sprint başında, o haftaya ait kartlar önce Backlog'da durur.
Sprint başladığında yapılacak işlem:
- o sprint'in kartlarını Backlog'dan al
- Sprint Backlog kolonuna taşı

Böylece herkes o haftanın hedefini net şekilde görür.

## 2. Günlük Çalışma Örneği
Örnek:
Alper, `S1-03: SQL Script` işine başlayacak.

Yapılacak akış:
1. Kartı Sprint Backlog'dan `In Progress` kolonuna taşı
2. Böylece herkes bu işin aktif olduğunu görür
3. Kart içindeki checklist maddelerini işaretleyerek ilerleme takibi yap

Örnek checklist ilerleyişi:
- Users tablosu
- Roles tablosu
- Hotels tablosu
- vb.

Checklist ilerledikçe kart üzerinde yüzde olarak ilerleme görünür.

## 3. İş Bitince Ne Olur?
Örnek akış:
1. Kod Git'e push edilir
2. PR açılır
3. Kart `In Progress` kolonundan `Review / PR` kolonuna taşınır
4. Başka bir ekip üyesi veya lider inceleme yapar
5. Onaylanınca kart `Testing` kolonuna geçer
6. Test başarılıysa kart `Done` kolonuna taşınır

## 4. Board'a Bakınca Ne Anlaşılır?
Board'a tek bakışta şunlar anlaşılır:
- hangi işler sırada bekliyor
- hangi işler aktif devam ediyor
- hangileri review aşamasında
- hangileri testte
- hangileri tamamlandı

## 5. Önemli Kural
`In Progress` kolonunda kişi başı en fazla 2 kart olmalı.

Sebep:
- aynı anda çok iş başlamak verimi düşürür
- az sayıda işe odaklanıp bitirmek daha doğrudur

Kısacası:
- çok işe başlamak değil
- başlanan işi bitirmek önemli
