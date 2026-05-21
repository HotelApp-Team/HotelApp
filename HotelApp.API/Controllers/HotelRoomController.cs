using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [Route("api/hotels/{hotelId}/rooms")]
    [ApiController]
    public class HotelRoomController : ControllerBase
    {
        private readonly IHotelRoomService _service;
        public HotelRoomController(IHotelRoomService service) => _service = service;

        /// <summary>
        /// Belirtilen otele ait tüm odaları getirir. Her oda için otel adı ve oda tipi adı bilgileri de döndürülür.
        /// </summary>
        /// <param name="hotelId">Odaları listelenecek otelin kimliği.</param>
        /// <returns>Otele ait oda listesi.</returns>
        [HttpGet]
        public async Task<IActionResult> GetHotelRooms(int hotelId)
        {
            var rooms = await _service.GetRoomsByHotelAsync(hotelId);
            return Ok(rooms);
        }

        /// <summary>
        /// Oda ekler. Gerekli bilgiler CreateHotelRoomDto içinde sağlanır. Başarılı bir ekleme işleminden sonra 200 OK döner.
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddRoom(int hotelId, [FromBody] CreateHotelRoomRequestDto request)
        {
            var dto = new CreateHotelRoomDto
            {
                HotelId = hotelId,
                RoomTypeId = request.RoomTypeId,
                PricePerNight = request.PricePerNight,
                ExtraGuestPricePerNight = request.ExtraGuestPricePerNight,
                TotalRooms = request.TotalRooms,
                RoomViewId = request.RoomViewId
            };

            await _service.AddRoomsAsync(dto);
            return Ok();
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchAvailableRooms(int hotelId, [FromBody] SearchAvailableRoomsRequestDto request)
        {
            if (request.CheckIn >= request.CheckOut)
                return BadRequest("CheckIn tarihi CheckOut'tan küçük olmalı.");

            var rooms = await _service.GetAvailableRoomsAsync(
                hotelId,
                request.City,
                request.CheckIn,
                request.CheckOut,
                request.GuestCount);

            return Ok(rooms);
        }

        /// <summary>
        /// Oda günceller. Oda Id'si URL parametresi olarak sağlanır ve güncellenecek bilgiler UpdateHotelRoomDto içinde sağlanır. Başarılı bir güncelleme işleminden sonra 200 OK döner.
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="roomId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{roomId}")]
        public async Task<IActionResult> UpdateRoom(int hotelId, int roomId, [FromBody] UpdateHotelRoomRequestDto request)
        {
            var room = await _service.GetRoomByIdAsync(roomId);
            if (room == null || room.HotelId != hotelId)
                return NotFound();

            var dto = new UpdateHotelRoomDto
            {
                Id = roomId,
                HotelId = hotelId,
                RoomTypeId = request.RoomTypeId,
                PricePerNight = request.PricePerNight,
                ExtraGuestPricePerNight = request.ExtraGuestPricePerNight,
                TotalRooms = request.TotalRooms,
                IsActive = request.IsActive,
                RoomViewId = request.RoomViewId
            };

            await _service.UpdateRoomsAsync(dto);
            return Ok();
        }

        /// <summary>
        /// Oda siler. Oda Id'si URL parametresi olarak sağlanır. Başarılı bir silme işleminden sonra 204 NoContent döner.
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{roomId}")]
        public async Task<IActionResult> Delete(int hotelId, int roomId)
        {
            var room = await _service.GetRoomByIdAsync(roomId);
            if (room == null || room.HotelId != hotelId)
                return NotFound();

            await _service.DeleteRoomsAsync(roomId);
            return NoContent();
        }
    }
}
