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

            var entity = new HotelImage
            {
                HotelId = hotelId,
                ImageUrl = dto.ImageUrl,
                IsMain = dto.IsMain,
                DisplayOrder = dto.DisplayOrder
            };

            await _unitOfWork.HotelImageRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Eklenen görsel ana görsel olarak seçildiyse,
            // aynı otele ait diğer görsellerin IsMain değerleri düzenlenir.
            // Böylece bir otelde aynı anda yalnızca bir ana görsel kalır.
            if (dto.IsMain)
            {
                await _unitOfWork.HotelImageRepository
                    .SetMainImageAsync(hotelId, entity.Id);

                await _unitOfWork.SaveChangesAsync();
            }
        }

        // Belirtilen otele ait mevcut görseli günceller.
        // Önce görselin varlığı kontrol edilir.
        // Daha sonra görselin gerçekten ilgili otele ait olup olmadığı doğrulanır.
        public async Task UpdateAsync(int hotelId, HotelImageUpdateDto dto)
        {
            var image = await _unitOfWork.HotelImageRepository.GetByIdAsync(dto.Id);

            if (image == null)
                throw new KeyNotFoundException("Resim bulunamadı");

            if (image.HotelId != hotelId)
                throw new KeyNotFoundException("Resim bu otele ait değil");

            image.ImageUrl = dto.ImageUrl;
            image.IsMain = dto.IsMain;
            image.DisplayOrder = dto.DisplayOrder;

            _unitOfWork.HotelImageRepository.Update(image);

            if (dto.IsMain)
            {
                await _unitOfWork.HotelImageRepository
                    .SetMainImageAsync(hotelId, image.Id);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        // Belirtilen görsel kaydını siler.
        // Silmeden önce görselin var olup olmadığını ve doğru otele ait olup olmadığını kontrol eder.
        // Bu kontrol başka otele ait görselin yanlışlıkla silinmesini engeller.
        public async Task DeleteAsync(int hotelId, int imageId)
        {
            var image = await _unitOfWork.HotelImageRepository.GetByIdAsync(imageId);

            if (image == null)
                throw new KeyNotFoundException("Resim bulunamadı");

            if (image.HotelId != hotelId)
                throw new KeyNotFoundException("Resim bu otele ait değil");

            _unitOfWork.HotelImageRepository.Delete(image);

            await _unitOfWork.SaveChangesAsync();
        }

        // Belirtilen görseli otelin ana görseli yapar.
        // Repository bu işlem sırasında aynı otele ait diğer ana görselleri pasifleştirir.
        // Son değişiklikler SaveChangesAsync ile veritabanına kaydedilir.
        public async Task SetMainImageAsync(int hotelId, int imageId)
        {
            await _unitOfWork.HotelImageRepository.SetMainImageAsync(hotelId, imageId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
