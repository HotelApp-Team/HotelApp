using HotelApp.Domain.DTOs;
using HotelApp.Domain.Interfaces;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _service;
        public RoomTypeController(IRoomTypeService service) => _service = service;

        /// <summary>
        /// Tüm oda tiplerini getirir. Her oda tipi için, odaya ait otel odası sayısı da dahil edilir.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllRoomTypesAsync();
            return Ok(data);
        }

        /// <summary>
        /// Id'ye göre oda tipi getirir. Oda tipi bulunamazsa NotFound döner.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var roomType = await _service.GetRoomTypeByIdAsync(id);
            if (roomType == null) return NotFound();
            return Ok(roomType);
        }

        /// <summary>
        /// İsime göre oda tipi getirir. Oda tipi bulunamazsa NotFound döner.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        [HttpGet("by-name")]
        public async Task<IActionResult> GetByName(string name)
        {
            var roomType = await _service.GetRoomTypeByNameAsync(name);
            if (roomType == null) return NotFound();
            return Ok(roomType);
        }

        /// <summary>
        /// Max Guests sayısına göre oda tiplerini getirir. Oda tipi bulunamazsa NotFound döner.
        /// </summary>
        /// <param name="maxGuests"></param>
        /// <returns></returns>

        [HttpGet("by-max-guests")]
        public async Task<IActionResult> GetByMaxGuests(int maxGuests)
        {
            var roomTypes = await _service.GetRoomTypesByMaxGuestsAsync(maxGuests);
            return Ok(roomTypes);
        }

        /// <summary>
        /// Oda tipi oluşturur. Oluşturulan oda tipi bilgisi döndürülmez, sadece başarılı mesajı döner.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoomTypeDto dto)
        {
            await _service.AddRoomTypeAsync(dto);
            return Ok("Oda tipi oluşturuldu.");
        }

        /// <summary>
        /// Oda tipi günceller. Güncellenen oda tipi bilgisi döndürülmez, sadece başarılı mesajı döner.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateRoomTypeDto dto)
        {
            await _service.UpdateRoomTypeAsync(dto);
            return Ok("Oda tipi güncellendi");

        }

        /// <summary>
        /// Oda tipi siler. Silinen oda tipi bilgisi döndürülmez, sadece başarılı mesajı döner.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteRoomTypeAsync(id);
            return Ok("Oda tipi silindi");
        }
    }
}
