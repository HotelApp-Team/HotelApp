using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    public class HotelImageService : IHotelImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HotelImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

            // main image logic
            if (dto.IsMain)
            {
                await _unitOfWork.HotelImageRepository
                    .SetMainImageAsync(hotelId, entity.Id);

                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(HotelImageUpdateDto dto)
        {
            var image = await _unitOfWork.HotelImageRepository.GetByIdAsync(dto.Id);

            if (image == null)
            {
                throw new Exception("Resim bulunamadı");
            }

            image.ImageUrl = dto.ImageUrl;
            image.IsMain = dto.IsMain;
            image.DisplayOrder = dto.DisplayOrder;

            _unitOfWork.HotelImageRepository.Update(image);

            if (dto.IsMain)
            {
                await _unitOfWork.HotelImageRepository.SetMainImageAsync(image.HotelId, image.Id);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int hotelId, int imageId)
        {
            var image = await _unitOfWork.HotelImageRepository.GetByIdAsync(imageId);

            if (image == null)
                throw new KeyNotFoundException("Resim bulunamadı");

            if (image.HotelId != hotelId)
                throw new InvalidOperationException("Bu resim bu otele ait değil");

            _unitOfWork.HotelImageRepository.Delete(image);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetMainImageAsync(int hotelId, int imageId)
        {
            await _unitOfWork.HotelImageRepository.SetMainImageAsync(hotelId, imageId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
