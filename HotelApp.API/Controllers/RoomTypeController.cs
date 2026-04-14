using HotelApp.Domain.DTOs;
using HotelApp.Domain.Interfaces;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : Controller
    {
        private readonly IRoomTypeService _service;
        public RoomTypeController(IRoomTypeService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllRoomTypesAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var roomType = await _service.GetRoomTypeByIdAsync(id);
            if (roomType == null) return NotFound();
            return Ok(roomType);
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetByName(string name)
        {
            var roomType = await _service.GetRoomTypeByNameAsync(name);
            if (roomType == null) return NotFound();
            return Ok(roomType);
        }

        [HttpGet("by-max-guests")]
        public async Task<IActionResult> GetByMaxGuests(int maxGuests)
        {
            var roomTypes = await _service.GetRoomTypeByMaxGuestsAsync(maxGuests);
            return Ok(roomTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomTypeDto dto)
        {
            await _service.AddRoomTypeAsync(dto);
            return Ok("Oda tipi oluşturuldu.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RoomTypeDto dto)
        {
            await _service.UpdateRoomTypeAsync(dto);
            return Ok("Oda tipi güncellendi");

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteRoomTypeAsync(id);
            return Ok("Oda tipi silindi");
        }
    }
}
