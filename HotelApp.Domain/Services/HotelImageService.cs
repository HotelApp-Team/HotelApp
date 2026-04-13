using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (image == null) return null;

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

            if (image == null) return null;

            return new HotelImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                IsMain = image.IsMain,
                DisplayOrder = image.DisplayOrder
            };
        }

        public async Task AddAsync(HotelImageCreateDto dto)
        {
            var entity = new HotelImage
            {
                HotelId = dto.HotelId,
                ImageUrl = dto.ImageUrl,
                IsMain = dto.IsMain,
                DisplayOrder = dto.DisplayOrder
            };

            await _unitOfWork.HotelImageRepository.AddAsync(entity);

            // Eğer main olarak ekleniyorsa diğerlerini kapat
            if (dto.IsMain)
            {
                await _unitOfWork.HotelImageRepository.SetMainImageAsync(dto.HotelId, entity.Id);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(HotelImageUpdateDto dto)
        {
            var image = await _unitOfWork.HotelImageRepository.GetByIdAsync(dto.Id);

            if (image == null)
                throw new Exception("Resim bulunamadı");

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

        public async Task DeleteAsync(int id)
        {
            var image = await _unitOfWork.HotelImageRepository.GetByIdAsync(id);

            if (image == null)
                throw new Exception("Resim bulunamadı");

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
