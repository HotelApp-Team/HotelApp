using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    // Otel görselleriyle ilgili iş kurallarını yöneten servis sınıfıdır.
    // Görsel ekleme, silme, güncelleme ve ana görsel seçme işlemlerini repository katmanı üzerinden yapar.
    // Controller bu sınıfı kullanarak doğrudan veritabanı işlemi yapmadan görsel yönetimini gerçekleştirir.
    public class HotelImageService : IHotelImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        // UnitOfWork nesnesi constructor ile alınır.
        // Tüm repository işlemleri bu nesne üzerinden yapılır.
        public HotelImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Belirtilen otele ait tüm görselleri repository'den getirir.
        // Gelen entity kayıtlarını API veya web katmanında kullanılacak HotelImageDto listesine dönüştürür.
        public async Task<List<HotelImageDto>> GetByHotelIdAsync(int hotelId)
        {
            var images = await _unitOfWork.HotelImageRepository.GetByHotelIdAsync(hotelId);

            return images.Select(x => new HotelImageDto
            {
                Id = x.Id,
                ImageUrl = x.ImageUrl,
                IsMain = x.IsMain,
                DisplayOrder = x.DisplayOrder
            }).ToList();
        }

        // Id değerine göre tek bir görsel kaydını getirir.
        // Kayıt bulunamazsa null döner, bulunursa DTO'ya çevrilerek döndürülür.
        public async Task<HotelImageDto?> GetByIdAsync(int id)
        {
            var image = await _unitOfWork.HotelImageRepository.GetByIdAsync(id);

            if (image == null)
            {
                return null;
            }

            return new HotelImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                IsMain = image.IsMain,
                DisplayOrder = image.DisplayOrder
            };
        }

        // Belirtilen otelin ana görselini getirir.
        // Ana görsel otel kartlarında kapak fotoğrafı olarak kullanılabilir.
        // Eğer ana görsel tanımlı değilse null döndürür.
        public async Task<HotelImageDto?> GetMainImageAsync(int hotelId)
        {
            var image = await _unitOfWork.HotelImageRepository.GetMainImageAsync(hotelId);

            if (image == null)
            {
                return null;
            }

            return new HotelImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                IsMain = image.IsMain,
                DisplayOrder = image.DisplayOrder
            };
        }

        // Belirtilen otele yeni görsel ekler.
        // Önce otelin gerçekten var olup olmadığını kontrol eder.
        // Otel bulunamazsa hatalı kayıt oluşmaması için işlem durdurulur.
        public async Task AddAsync(int hotelId, HotelImageCreateDto dto)
        {
            var hotelExists = await _unitOfWork.HotelRepository
                .ExistsAsync(x => x.Id == hotelId);

            if (!hotelExists)
                throw new KeyNotFoundException("Otel bulunamadı");

            var existingImages = await _unitOfWork.HotelImageRepository.GetTrackedByHotelIdAsync(hotelId);
            var isMain = existingImages.Count == 0 || dto.IsMain;

            var entity = new HotelImage
            {
                HotelId = hotelId,
                ImageUrl = dto.ImageUrl,
                IsMain = isMain,
                DisplayOrder = dto.DisplayOrder
            };

            await _unitOfWork.HotelImageRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Eklenen görsel ana görsel olarak seçildiyse,
            // aynı otele ait diğer görsellerin IsMain değerleri düzenlenir.
            // Böylece bir otelde aynı anda yalnızca bir ana görsel kalır.
            if (isMain && existingImages.Count > 0)
            {
                await _unitOfWork.HotelImageRepository
                    .SetMainImageAsync(hotelId, entity.Id);

                await _unitOfWork.SaveChangesAsync();
            }
        }

        // Gönderilen görselleri doğrular ve sıralama ile ana görsel ayarlarını tek SaveChanges ile günceller.
        public async Task UpdateAsync(int hotelId, HotelImageUpdateDto dto)
        {
            var images = await _unitOfWork.HotelImageRepository.GetTrackedByHotelIdAsync(hotelId);
            var requestedImages = dto.Images ?? new List<HotelImageUpdateItemDto>();

            if (requestedImages.Count(x => x.IsMain) != 1)
                throw new ArgumentException("Tam olarak bir ana görsel seçilmelidir.");

            if (requestedImages.Any(x => x.DisplayOrder < 0))
                throw new ArgumentException("Görsel sırası negatif olamaz.");

            var requestedIds = requestedImages.Select(x => x.Id).ToList();
            if (requestedIds.Distinct().Count() != requestedIds.Count
                || images.Count != requestedImages.Count
                || !images.Select(x => x.Id).OrderBy(x => x).SequenceEqual(requestedIds.OrderBy(x => x)))
            {
                throw new ArgumentException("Gönderilen görseller otelin mevcut görselleriyle eşleşmiyor.");
            }

            var updatesById = requestedImages.ToDictionary(x => x.Id);
            foreach (var image in images)
            {
                var update = updatesById[image.Id];
                image.DisplayOrder = update.DisplayOrder;
                image.IsMain = update.IsMain;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        // Belirtilen görsel kaydını siler.
        // Silmeden önce görselin var olup olmadığını ve doğru otele ait olup olmadığını kontrol eder.
        // Bu kontrol başka otele ait görselin yanlışlıkla silinmesini engeller.
        public async Task DeleteAsync(int hotelId, int imageId)
        {
            var images = await _unitOfWork.HotelImageRepository.GetTrackedByHotelIdAsync(hotelId);
            var image = images.FirstOrDefault(x => x.Id == imageId);

            if (image == null)
                throw new KeyNotFoundException("Resim bulunamadı");

            _unitOfWork.HotelImageRepository.Delete(image);

            if (image.IsMain)
            {
                var nextMain = images
                    .Where(x => x.Id != imageId)
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.Id)
                    .FirstOrDefault();

                if (nextMain != null)
                    nextMain.IsMain = true;
            }

            await _unitOfWork.SaveChangesAsync();
        }

    }
}
