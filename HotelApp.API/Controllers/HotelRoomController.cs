using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelRoomController : ControllerBase
    {
        private readonly IHotelRoomService _service;
        public HotelRoomController(IHotelRoomService service) => _service = service;

        /// <summary>
        /// Tüm otel odalarını getirir. Her oda için otel adı ve oda tipi adı da dahil edilir.
        /// </summary>

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _service.GetAllRoomsAsync();
            return Ok(rooms);
        }

        /// <summary>
        /// Id'ye göre belirli bir otel odasını getirir. Oda bilgileri, otel adı ve oda tipi adı ile birlikte döndürülür.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _service.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        /// <summary>
        /// Belirtilen otele ait tüm odaları getirir. Her oda için otel adı ve oda tipi adı bilgileri de döndürülür.
        /// </summary>
        /// <param name="hotelId">Odaları listelenecek otelin kimliği.</param>
        /// <returns>Otele ait oda listesi.</returns>
        [HttpGet("/api/hotels/{hotelId}/rooms")]
        public async Task<IActionResult> GetHotelRooms(int hotelId)
        {
            var rooms = await _service.GetRoomsByHotelAsync(hotelId);
            return Ok(rooms);
        }

        /// <summary>
        /// Oda tipi Id'sine göre odaya ait tüm odaları getirir. Her oda için otel adı ve oda tipi adı da dahil edilir.
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <returns></returns>

        [HttpGet("room-type/{roomTypeId}")]
        public async Task<IActionResult> GetRoomsByType(int roomTypeId)
        {
            var rooms = await _service.GetRoomsByTypeAsync(roomTypeId);
            return Ok(rooms);
        }

        /// <summary>
        /// Müsait tarih aralığına göre müsait odaları getirir. Her oda için otel adı, oda tipi adı ve müsait oda sayısı da dahil edilir.
        /// </summary>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms(int hotelId, string city, DateTime checkIn, DateTime checkOut, int guestCount)
        {
            if (checkIn >= checkOut)
                return BadRequest("CheckIn tarihi CheckOut'tan küçük olmalı");

            var rooms = await _service.GetAvailableRoomsAsync(hotelId, city, checkIn, checkOut, guestCount);
            return Ok(rooms);
        }


        /// <summary>
        /// Oda ekler. Gerekli bilgiler CreateHotelRoomDto içinde sağlanır. Başarılı bir ekleme işleminden sonra 200 OK döner.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddRoom([FromBody] CreateHotelRoomDto dto)
        {
            await _service.AddRoomsAsync(dto);
            return Ok();
        }

        [HttpPost("/api/hotels/{hotelId}/rooms/search")]
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
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateHotelRoomDto dto)
        {
            dto.Id = id;
            await _service.UpdateRoomsAsync(dto);
            return Ok();
        }

        /// <summary>
        /// Oda siler. Oda Id'si URL parametresi olarak sağlanır. Başarılı bir silme işleminden sonra 204 NoContent döner.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteRoomsAsync(id);
            return NoContent();
        }
    }
}
