using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelRoomController : Controller
    {
        private readonly IHotelRoomService _service;
        public HotelRoomController(IHotelRoomService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _service.GetAllRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _service.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetRoomsByHotel(int hotelId)
        {
            var rooms = await _service.GetRoomsByHotelAsync(hotelId);
            return Ok(rooms);
        }

        [HttpGet("roomType/{roomTypeId}")]
        public async Task<IActionResult> GetRoomsByType(int roomTypeId)
        {
            var rooms = await _service.GetRoomsByTypeAsync(roomTypeId);
            return Ok(rooms);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
        {
            var rooms = await _service.GetAvailableRoomsAsync(checkIn, checkOut);
            return Ok(rooms);
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom([FromBody] CreateHotelRoomDto dto)
        {
            await _service.AddRoomsAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoom([FromBody] UpdateHotelRoomDto dto)
        {
            await _service.UpdateRoomsAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            await _service.DeleteRoomsAsync(id);
            return Ok();
        }
    }
}
