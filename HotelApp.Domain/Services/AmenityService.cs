using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    // Olanak işlemlerinin servis katmanındaki iş kurallarını yönetir.
    // Controller bu sınıfı kullanarak doğrudan repository veya veritabanı işlemi yapmadan
    // olanak ekleme, silme, güncelleme ve otel-olanak atama işlemlerini gerçekleştirir.
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;

        // UnitOfWork nesnesi constructor ile alınır.
        // Tüm repository işlemleri bu nesne üzerinden yapılır.
        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Veritabanında kayıtlı tüm olanakları getirir.
        // Gelen Amenity entity listesini, dış katmanlarda kullanılacak AmenityDto listesine dönüştürür.
        public async Task<List<AmenityDto>> GetAllAsync()
        {
            var amenities = await _unitOfWork.AmenityRepository.GetAllAsync();
            return amenities.Select(MapToDto).ToList();
        }

        // Id değerine göre tek bir olanak kaydını getirir.
        // Kayıt bulunamazsa null döndürür; bulunursa DTO formatına çevirir.
        public async Task<AmenityDto?> GetByIdAsync(int id)
        {
            var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(id);

            if (amenity == null)
            {
                return null;
            }

            return MapToDto(amenity);
        }

        // Yeni olanak kaydı oluşturur.
        // Aynı isimde başka bir olanak olup olmadığını kontrol eder.
        // Eğer aynı isimde kayıt varsa tekrar eklemeyi engelleyerek veri tekrarını önler.
        public async Task AddAsync(AmenityCreateDto dto)
        {
            var exists = await _unitOfWork.AmenityRepository
                .ExistsAsync(x => x.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new InvalidOperationException("Bu olanak zaten mevcut");

            var entity = new Amenity
            {
                Name = dto.Name,
                Icon = dto.Icon
            };

            await _unitOfWork.AmenityRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        // Mevcut olanak bilgisini günceller.
        // Önce güncellenecek kaydın var olup olmadığı kontrol edilir.
        // Daha sonra aynı isimde başka bir kayıt olup olmadığına bakılır.
        public async Task UpdateAsync(AmenityUpdateDto dto)
        {
            var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(dto.Id);

            if (amenity == null)
            {
                throw new KeyNotFoundException("Olanak bulunamadı");
            }

            var exists = await _unitOfWork.AmenityRepository
                .ExistsAsync(x => x.Name.ToLower() == dto.Name.ToLower()
                               && x.Id != dto.Id);

            if (exists)
                throw new InvalidOperationException("Bu isimde bir olanak zaten var");


            amenity.Name = dto.Name;
            amenity.Icon = dto.Icon;

            _unitOfWork.AmenityRepository.Update(amenity);
            await _unitOfWork.SaveChangesAsync();
        }

        // Olanak kaydını siler.
        // Silmeden önce kaydın var olup olmadığını kontrol eder.
        // Eğer olanak herhangi bir otele atanmışsa silmeye izin vermez.
        public async Task DeleteAsync(int id)
        {
            var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(id);

            if (amenity == null)
            {
                throw new KeyNotFoundException("Olanak bulunamadı");
            }

            var isUsed = await _unitOfWork.HotelRepository
                .ExistsAsync(h => h.Amenities.Any(a => a.Id == id));


            if (isUsed)
                throw new InvalidOperationException("Bu olanak oteller tarafından kullanılıyor, silinemez");

            _unitOfWork.AmenityRepository.Delete(amenity);
            await _unitOfWork.SaveChangesAsync();
        }

        // Belirtilen otele atanmış olanakları getirir.
        // Otel detay sayfasında seçili olanakları göstermek için kullanılır.
        // Gelen entity kayıtları AmenityDto listesine dönüştürülür.
        public async Task<List<AmenityDto>> GetAmenitiesByHotelIdAsync(int hotelId)
        {
            var amenities = await _unitOfWork.AmenityRepository.GetAmenitiesByHotelIdAsync(hotelId);
            return amenities.Select(MapToDto).ToList();
        }

        // Bir otele birden fazla olanak atar.
        // Önce otelin var olup olmadığını kontrol eder.
        // Sonra gelen amenityIds listesindeki her Id için ilgili olanağı arar.
        public async Task<HotelAmenityAssignResponseDto> AddAmenitiesToHotelAsync(int hotelId, List<int> amenityIds)
        {
            var hotel = await _unitOfWork.HotelRepository
                .GetWithDetailsAsync(hotelId);

            if (hotel == null)
                throw new KeyNotFoundException("Otel bulunamadı");

            int added = 0;
            int notFound = 0;
            var notFoundIds = new List<int>();

            foreach (var amenityId in amenityIds)
            {
                var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(amenityId);

                // Eğer verilen Id'ye ait olanak bulunamazsa sayaç artırılır
                // ve bu Id response içinde NotFoundIds listesine eklenir.
                if (amenity == null)
                {
                    notFound++;
                    notFoundIds.Add(amenityId);
                    continue;
                }

                // Olanak daha önce bu otele eklenmemişse ilişki oluşturulur.
                // Böylece aynı olanak aynı otele tekrar tekrar eklenmez.
                if (!hotel.Amenities.Any(a => a.Id == amenityId))
                {
                    hotel.Amenities.Add(amenity);
                    added++;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return new HotelAmenityAssignResponseDto
            {
                AddedCount = added,
                NotFoundCount = notFound,
                NotFoundIds = notFoundIds
            };
        }

        // Belirtilen otelden seçilen olanağı kaldırır.
        // Önce otel detaylarıyla birlikte getirilir.
        // Sonra kaldırılacak olanağın gerçekten otele ait olup olmadığı kontrol edilir.
        public async Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId)
        {
            var hotel = await _unitOfWork.HotelRepository
                .GetWithDetailsAsync(hotelId);

            if (hotel == null)
                throw new KeyNotFoundException("Otel bulunamadı");

            var amenity = hotel.Amenities.FirstOrDefault(a => a.Id == amenityId);

            if (amenity == null)
                throw new KeyNotFoundException("Olanak bu otele ait değil");

            hotel.Amenities.Remove(amenity);

            await _unitOfWork.SaveChangesAsync();
        }

        // Amenity entity nesnesini AmenityDto nesnesine dönüştürür.
        // Servis dışına entity yerine DTO döndürmek için kullanılır.
        private static AmenityDto MapToDto(Amenity amenity)
        {
            return new AmenityDto
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Icon = amenity.Icon
            };
        }
    }
}
