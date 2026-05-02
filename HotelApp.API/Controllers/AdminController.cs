using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[Route("api/admin")]
[ApiController]
public class AdminController : Controller
{
    private readonly IHotelService _hotelService;
    private readonly IUserService _userService;
    private readonly IReservationService _reservationService;

}
