using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers
{
    /// <summary>
    /// Otel odalarina ait gorsellerin listelenmesi, eklenmesi,
    /// silinmesi, guncellenmesi ve ana gorsel olarak isaretlenmesi islemlerini yonetir.
    /// </summary>
    [ApiController]
    [Route("api/rooms/{roomId}/images")]
    public class RoomImagesController : ControllerBase
    {

        /// <summary>
        /// RoomImagesController sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="roomImageService">Oda gorseli islemleri icin kullanilan servis.</param>
        private readonly IRoomImageService _roomImageService;

        public RoomImagesController(IRoomImageService roomImageService)
        {
            _roomImageService = roomImageService;
        }

        /// <summary>
        /// Belirtilen odaya ait tum gorselleri getirir.
        /// </summary>
        /// <param name="roomId">Gorselleri listelenecek otel odasinin Id degeri.</param>
        /// <returns>Odaya ait gorsel listesini doner.</returns>
        [HttpGet]
        public async Task<IActionResult> GetByRoomId(int roomId)
        {
            var images = await _roomImageService.GetByRoomIdAsync(roomId);
            return Ok(images);
        }

        /// <summary>
        /// Belirtilen odaya yeni gorsel ekler.
        /// Route uzerinden gelen roomId degeri DTO icindeki HotelRoomId alanina atanir.
        /// </summary>
        /// <param name="roomId">Gorsel eklenecek otel odasinin Id degeri.</param>
        /// <param name="dto">Eklenecek gorsel bilgilerini iceren DTO.</param>
        /// <returns>Olusturulan gorsel kaydini doner.</returns>
        [HttpPost]
        public async Task<IActionResult> Add(int roomId, [FromBody] CreateRoomImageDto dto)
        {
            dto.HotelRoomId = roomId;

            var image = await _roomImageService.AddAsync(dto);
            return CreatedAtAction(nameof(GetByRoomId), new { roomId }, image);
        }

        /// <summary>
        /// Belirtilen gorseli ilgili oda icin ana gorsel olarak isaretler.
        /// </summary>
        /// <param name="roomId">Ana gorseli belirlenecek otel odasinin Id degeri.</param>
        /// <param name="imageId">Ana gorsel olarak isaretlenecek gorselin Id degeri.</param>
        /// <returns>Basarili olursa 204 NoContent doner.</returns>
        [HttpPut("{imageId}/main")]
        public async Task<IActionResult> SetMain(int roomId, int imageId)
        {
            await _roomImageService.SetMainAsync(roomId, imageId);
            return NoContent();
        }

        /// <summary>
        /// Belirtilen oda gorselini siler.
        /// </summary>
        /// <param name="roomId">Gorselin ait oldugu otel odasinin Id degeri.</param>
        /// <param name="imageId">Silinecek gorselin Id degeri.</param>
        /// <returns>Basarili olursa 204 NoContent doner.</returns>
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(int roomId, int imageId)
        {
            await _roomImageService.DeleteAsync(imageId);
            return NoContent();
        }

        /// <summary>
        /// Belirtilen oda gorselini gunceller.
        /// </summary>
        /// <param name="roomId">Gorselin ait oldugu otel odasinin Id degeri.</param>
        /// <param name="imageId">Guncellenecek gorselin Id degeri.</param>
        /// <param name="dto">Guncel gorsel bilgilerini iceren DTO.</param>
        /// <returns>Basarili olursa 204 NoContent doner.</returns>
        [HttpPut("{imageId}")]
        public async Task<IActionResult> Update(int roomId, int imageId, [FromBody] UpdateRoomImageDto dto)
        {
            await _roomImageService.UpdateAsync(roomId, imageId, dto);
            return NoContent();
        }
    }
}
