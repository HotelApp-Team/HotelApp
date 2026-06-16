using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    // Otel işlemlerinin servis katmanındaki iş kurallarını yönetir.
    // Repository ile controller arasında köprü görevi görür.
    public class HotelService : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;

        // UnitOfWork nesnesi constructor ile alınır.
        // Tüm repository işlemleri bu nesne üzerinden yapılır.
        public HotelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Tüm otelleri veritabanından getirir.
        // Gelen otel kayıtlarını liste DTO'suna dönüştürür.
        public async Task<IEnumerable<HotelListDto>> GetAllAsync()
        {
            var hotels = await _unitOfWork.HotelRepository.GetAllAsync();
            return hotels.Select(MapToListDto);
        }

        // Id değerine göre otel detayını getirir.
        // Otel bulunamazsa veya pasifse null döndürür.
        public async Task<HotelDetailDto?> GetByIdAsync(int id)
        {
            var hotel = await _unitOfWork.HotelRepository.GetWithDetailsAsync(id);

            if (hotel == null || !hotel.IsActive)
            {
                return null;
            }

            return new HotelDetailDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                CityId = hotel.CityId,
                City = hotel.City.Name,
                Country = hotel.Country,
                Phone = hotel.Phone,
                Email = hotel.Email,
                Latitude = hotel.Latitude,
                Longitude = hotel.Longitude,
                StarRating = hotel.StarRating,
                AvgRating = hotel.AvgRating,
                IsActive = hotel.IsActive,
                Amenities = hotel.Amenities.Select(a => new AmenityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Icon = a.Icon
                }).ToList()
            };
        }

        // Şehir adına göre otelleri getirir.
        // Sonucu liste DTO'suna dönüştürür.
        public async Task<IEnumerable<HotelListDto>> GetByCityAsync(string city)
        {
            var hotels = await _unitOfWork.HotelRepository.GetByCityAsync(city);
            return hotels.Select(MapToListDto);
        }

        // Sadece aktif durumdaki otelleri getirir.
        // Pasif oteller listeye dahil edilmez.
        public async Task<IEnumerable<HotelListDto>> GetActiveAsync()
        {
            var hotels = await _unitOfWork.HotelRepository.GetActiveAsync();
            return hotels.Select(MapToListDto);
        }

        // Yeni otel kaydı oluşturur.
        // Otel varsayılan olarak aktif şekilde kaydedilir.
        public async Task<int> CreateAsync(HotelCreateDto dto)
        {
            var hotel = new Hotel
            {
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                CityId = dto.CityId,
                Country = dto.Country,
                Phone = dto.Phone,
                Email = dto.Email,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                StarRating = dto.StarRating,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.HotelRepository.AddAsync(hotel);
            await _unitOfWork.SaveChangesAsync();

            return hotel.Id;
        }

        // Mevcut otel kaydını günceller.
        // Otel bulunamazsa KeyNotFoundException fırlatır.
        public async Task UpdateAsync(HotelUpdateDto dto)
        {
            var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(dto.Id);

            if (hotel == null)
            {
                throw new KeyNotFoundException("Otel bulunamadı");
            }

            hotel.Name = dto.Name;
            hotel.Description = dto.Description;
            hotel.Address = dto.Address;
            hotel.CityId = dto.CityId;
            hotel.Country = dto.Country;
            hotel.Phone = dto.Phone;
            hotel.Email = dto.Email;
            hotel.Latitude = dto.Latitude;
            hotel.Longitude = dto.Longitude;
            hotel.StarRating = dto.StarRating;
            hotel.IsActive = dto.IsActive;
            hotel.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.HotelRepository.Update(hotel);
            await _unitOfWork.SaveChangesAsync();
        }

        // Oteli veritabanından fiziksel olarak silmez.
        // IsActive alanını false yaparak pasif duruma alır.
        // Oteli kalici olarak siler (otel + bagli oda, gorsel, olanak, yorum).
        // Yayinda/pasif olmasi fark etmez; pasife alma icin ayri IsActive akisi vardir.
        // Rezervasyon gecmisi olan otel silinmez; once kullaniciya bilgi verilir.
        public async Task DeleteAsync(int id)
        {
            var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(id);

            if (hotel == null)
                throw new KeyNotFoundException("Otel bulunamadı");

            if (await _unitOfWork.HotelRepository.HasReservationsAsync(id))
                throw new InvalidOperationException(
                    "Bu otele ait rezervasyon kayıtları olduğu için otel silinemez. Dilerseniz oteli pasife alabilirsiniz.");

            await _unitOfWork.HotelRepository.HardDeleteAsync(id);
        }

        // Hotel entity nesnesini HotelListDto nesnesine dönüştürür.
        // Ana görseli ve aktif odalar arasındaki en düşük fiyatı hesaplar.
        private static HotelListDto MapToListDto(Hotel hotel)
        {
            return new HotelListDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                City = hotel.City.Name,
                Country = hotel.Country,
                StarRating = hotel.StarRating,
                AvgRating = hotel.AvgRating,
                IsActive = hotel.IsActive,
                MainImageUrl = hotel.HotelImages.FirstOrDefault(x => x.IsMain)?.ImageUrl,
                MinPrice = hotel.HotelRooms
                .Where(x => x.IsActive)
                .Select(x => (decimal?)x.PricePerNight)
                .Min()
            };
        }

        // Aktif otellerin bulunduğu şehirleri getirir.
        public async Task<List<string>> GetCitiesAsync()
        {
            return await _unitOfWork.HotelRepository.GetActiveCitiesAsync();
        }

        // Anasayfa hero alaninda gosterilecek otel ve sehir sayilarini UnitOfWork uzerinden hazirlar.
        public async Task<HeroStatsDto> GetHeroStatsAsync()
        {
            var hotelCount = await _unitOfWork.HotelRepository.GetActiveHotelCountAsync();
            var cityCount = await _unitOfWork.HotelRepository.GetActiveCityCountAsync();

            // Repository'den gelen ham sayilari API response DTO'suna mapler.
            return new HeroStatsDto
            {
                HotelCount = hotelCount,
                CityCount = cityCount
            };
        }

        // Anasayfa şehir kartları için repository'den gelen istatistikleri ekrana uygun DTO'ya mapler.
        public async Task<List<CityCardDto>> GetFeaturedCitiesAsync(int count)
        {
            var safeCount = Math.Clamp(count, 1, 12);
            var cities = await _unitOfWork.HotelRepository.GetFeaturedCityStatsAsync(safeCount);

            // Repository sorgu sonucuna sadece vitrin karti siralama bilgisini ekler.
            return cities
                .Select((city, index) => new CityCardDto
                {
                    Name = city.Name,
                    HotelCount = city.HotelCount,
                    IsBig = index == 0
                })
                .ToList();
        }

        // Kullanıcının otel arama kriterlerine göre müsait otelleri getirir.
        // Sayfalama, tarih, misafir sayısı ve şehir kontrollerini yapar.
        public async Task<HotelSearchResponseDto> SearchAsync(HotelSearchRequestDto dto, int page, int pageSize)
        {
            ValidatePaging(page, pageSize);

            if (dto.CheckIn >= dto.CheckOut)
                throw new ArgumentException("CheckIn tarihi CheckOut tarihinden küçük olmalıdır.");

            if (dto.GuestCount < 1)
                throw new ArgumentException("GuestCount en az 1 olmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.City))
                throw new ArgumentException("Şehir bilgisi zorunludur.");

            var (data, totalCount) = await _unitOfWork.HotelRepository.SearchAvailableHotelsPagedAsync(
                dto.City,
                dto.CheckIn,
                dto.CheckOut,
                dto.GuestCount,
                dto.StarRating,
                dto.MinRating,
                page,
                pageSize);

            return new HotelSearchResponseDto
            {
                TotalCount = totalCount,
                Hotels = data
            };
        }

        // Admin otel listesinde filtreleme, sıralama ve sayfalama işlemini yapar.
        // Repository'den gelen kayıtları HotelListDto listesine dönüştürür.
        public async Task<PagedResponseDto<HotelListDto>> GetAllFilteredAsync(string? city, byte? star, bool? isActive, int? hotelId, string? sortBy, string? sortOrder, int page, int pageSize)
        {
            var (data, totalCount) =
                await _unitOfWork.HotelRepository.GetPagedFilteredAsync(
                    city, star, isActive, hotelId, sortBy, sortOrder, page, pageSize);


            var mapped = data.Select(MapToListDto).ToList();

            return new PagedResponseDto<HotelListDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = mapped
            };
        }

        // Admin otel listesi filtreleri icin sehir ve otel seceneklerini hazirlar.
        // Aktif/pasif tum oteller dahil edilir; sehirler tekil ve sirali doner.
        public async Task<HotelFilterOptionsDto> GetFilterOptionsAsync()
        {
            var hotels = await _unitOfWork.HotelRepository.GetAllAsync();

            var hotelItems = hotels
                .OrderBy(x => x.Name)
                .Select(x => new HotelFilterOptionItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    City = x.City.Name
                })
                .ToList();

            var cities = hotelItems
                .Select(x => x.City)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            return new HotelFilterOptionsDto
            {
                Cities = cities,
                Hotels = hotelItems
            };
        }

        // Navbar üzerinden yapılan otel veya şehir aramasını yönetir.
        // Sayfalama ve boş arama kontrollerini gerçekleştirir.
        public async Task<PagedResponseDto<HotelListDto>> SearchCatalogAsync(string searchTerm, int page, int pageSize)
        {
            // Sayfa numarası ve sayfa boyutunu kontrol eder.
            ValidatePaging(page, pageSize);

            // Boş bir arama ifadesinin repository katmanına gitmesini engeller.
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Otel veya şehir bilgisi zorunludur.");


            // Repository üzerinden eşleşen aktif otelleri getirir.
            var (hotels, totalCount) = await _unitOfWork.HotelRepository.SearchCatalogPagedAsync(
                        searchTerm,
                        page,
                        pageSize);

            // Hotel nesnelerini kullanıcıya gösterilecek DTO modeline dönüştürür.
            return new PagedResponseDto<HotelListDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = hotels.Select(MapToListDto).ToList()
            };
        }

        // Sayfa numarası ve sayfa boyutunun geçerli aralıkta olup olmadığını kontrol eder.
        private void ValidatePaging(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentException("Page 1 veya daha büyük olmalıdır.");

            if (pageSize < 1)
                throw new ArgumentException("PageSize 1 veya daha büyük olmalıdır.");

            if (pageSize > 100)
                throw new ArgumentException("PageSize en fazla 100 olabilir.");
        }
    }
}
