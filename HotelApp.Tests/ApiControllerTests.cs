using HotelApp.API.Controllers;
using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;

namespace HotelApp.Tests;

public class ApiControllerTests
{
    [Fact]
    public async Task Hotels_GetAll_PageBirdenKucukse_BadRequestDoner()
    {
        var controller = new HotelsController(new FakeHotelService(), new FakeLoggerService());

        var result = await controller.GetAll(city: null, star: null, page: 0);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Hotels_GetAll_PageSizeYuzdenBuyukse_BadRequestDoner()
    {
        var controller = new HotelsController(new FakeHotelService(), new FakeLoggerService());

        var result = await controller.GetAll(city: null, star: null, pageSize: 101);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Hotels_GetById_OtelYoksa_NotFoundDoner()
    {
        var controller = new HotelsController(new FakeHotelService { HotelById = null }, new FakeLoggerService());

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Hotels_Search_ServiceArgumentExceptionFirlatirsa_BadRequestDoner()
    {
        var controller = new HotelsController(new FakeHotelService
        {
            SearchException = new ArgumentException("Gecersiz arama kriteri")
        }, new FakeLoggerService());

        var result = await controller.Search(new HotelSearchRequestDto(), page: 1, pageSize: 10);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Amenities_GetAll_OkDoner()
    {
        var controller = new AmenitiesController(new FakeAmenityService
        {
            Amenities = new List<AmenityDto>
            {
                new() { Id = 1, Name = "Wi-Fi", Icon = "wifi" }
            }
        });

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<List<AmenityDto>>(ok.Value);
        Assert.Single(data);
    }

    [Fact]
    public async Task Amenities_Create_ModelStateGecersizse_BadRequestDoner()
    {
        var controller = new AmenitiesController(new FakeAmenityService());
        controller.ModelState.AddModelError("Name", "Olanak adi zorunludur");

        var result = await controller.Create(new AmenityCreateDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Amenities_Update_UrlIdBodyIdUyusmazsa_BadRequestDoner()
    {
        var controller = new AmenitiesController(new FakeAmenityService());

        var result = await controller.Update(1, new AmenityUpdateDto { Id = 2, Name = "Spa" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task HotelImages_GetImages_OkVeOtelGorselleriniDoner()
    {
        var controller = new HotelImagesController(new FakeHotelImageService
        {
            Images = new List<HotelImageDto>
            {
                new() { Id = 1, ImageUrl = "https://example.com/hotel-1.jpg", IsMain = true, DisplayOrder = 0 }
            }
        });

        var result = await controller.GetImages(hotelId: 1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<List<HotelImageDto>>(ok.Value);
        Assert.Single(data);
    }

    [Fact]
    public async Task HotelImages_AddImage_Basariliysa_CreatedDoner()
    {
        var service = new FakeHotelImageService();
        var controller = new HotelImagesController(service);

        var result = await controller.AddImage(1, new HotelImageCreateDto
        {
            ImageUrl = "https://example.com/hotel-1.jpg",
            IsMain = true,
            DisplayOrder = 0
        });

        var status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(201, status.StatusCode);
        Assert.Equal(1, service.LastAddedHotelId);
    }

    [Fact]
    public async Task HotelImages_Update_UrlIdBodyIdUyusmazsa_BadRequestDoner()
    {
        var controller = new HotelImagesController(new FakeHotelImageService());

        var result = await controller.UpdateImage(1, imageId: 5, new HotelImageUpdateDto
        {
            Id = 6,
            ImageUrl = "https://example.com/hotel-1.jpg",
            IsMain = false,
            DisplayOrder = 1
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task HotelAmenities_AddAmenities_ModelStateGecersizse_BadRequestDoner()
    {
        var controller = new HotelAmenitiesController(new FakeAmenityService());
        controller.ModelState.AddModelError("AmenityIds", "AmenityId listesi zorunludur");

        var result = await controller.AddAmenities(1, new HotelAmenityAssignDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task HotelAmenities_AddAmenities_Basariliysa_SonucuOkDoner()
    {
        var controller = new HotelAmenitiesController(new FakeAmenityService
        {
            AddAmenitiesResponse = new HotelAmenityAssignResponseDto
            {
                AddedCount = 2,
                NotFoundCount = 0
            }
        });

        var result = await controller.AddAmenities(1, new HotelAmenityAssignDto
        {
            AmenityIds = new List<int> { 1, 2 }
        });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<HotelAmenityAssignResponseDto>(ok.Value);
        Assert.Equal(2, response.AddedCount);
    }

    [Fact]
    public async Task HotelAmenities_RemoveAmenity_Basariliysa_NoContentDoner()
    {
        var controller = new HotelAmenitiesController(new FakeAmenityService());

        var result = await controller.RemoveAmenity(hotelId: 1, amenityId: 2);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RoomTypes_GetById_OdaTipiYoksa_NotFoundDoner()
    {
        var controller = new RoomTypeController(new FakeRoomTypeService { RoomTypeById = null });

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RoomTypes_GetByName_OdaTipiYoksa_NotFoundDoner()
    {
        var controller = new RoomTypeController(new FakeRoomTypeService { RoomTypeByName = null });

        var result = await controller.GetByName("olmayan-tip");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task HotelRooms_Search_CheckInCheckOuttanBuyukVeyaEsitse_BadRequestDoner()
    {
        var controller = new HotelRoomController(new FakeHotelRoomService());
        var request = new SearchAvailableRoomsRequestDto
        {
            City = "Istanbul",
            CheckIn = new DateTime(2026, 5, 10),
            CheckOut = new DateTime(2026, 5, 10),
            GuestCount = 2
        };

        var result = await controller.SearchAvailableRooms(1, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task HotelRooms_Update_OdaBaskaOteleAitse_NotFoundDoner()
    {
        var controller = new HotelRoomController(new FakeHotelRoomService
        {
            RoomById = new DetailHotelRoomDto { Id = 5, HotelId = 2 }
        });

        var result = await controller.UpdateRoom(
            hotelId: 1,
            roomId: 5,
            request: new UpdateHotelRoomRequestDto
            {
                RoomTypeId = 1,
                PricePerNight = 1000,
                TotalRooms = 3,
                IsActive = true
            });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task HotelRooms_Delete_OdaYoksa_NotFoundDoner()
    {
        var controller = new HotelRoomController(new FakeHotelRoomService { RoomById = null });

        var result = await controller.Delete(hotelId: 1, roomId: 999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Auth_GetProfile_TokendaUserIdYoksa_UnauthorizedDoner()
    {
        var controller = CreateAuthController();

        var result = await controller.GetProfile();

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Auth_UpdateProfile_TokendaUserIdYoksa_UnauthorizedDoner()
    {
        var controller = CreateAuthController();

        var result = await controller.UpdateProfile(new ProfileDto
        {
            FirstName = "Test",
            LastName = "User",
            Phone = "5551112233"
        });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public void AdminEndpointleri_AdminRoleAuthorizeIleKorunur()
    {
        AssertAdminOnly<HotelsController>(nameof(HotelsController.Create), typeof(HotelCreateDto));
        AssertAdminOnly<HotelsController>(nameof(HotelsController.Update), typeof(int), typeof(HotelUpdateDto));
        AssertAdminOnly<HotelsController>(nameof(HotelsController.Delete), typeof(int));

        AssertAdminOnly<AmenitiesController>(nameof(AmenitiesController.Create), typeof(AmenityCreateDto));
        AssertAdminOnly<AmenitiesController>(nameof(AmenitiesController.Update), typeof(int), typeof(AmenityUpdateDto));
        AssertAdminOnly<AmenitiesController>(nameof(AmenitiesController.Delete), typeof(int));

        AssertAdminOnly<HotelImagesController>(nameof(HotelImagesController.AddImage), typeof(int), typeof(HotelImageCreateDto));
        AssertAdminOnly<HotelImagesController>(nameof(HotelImagesController.UpdateImage), typeof(int), typeof(int), typeof(HotelImageUpdateDto));
        AssertAdminOnly<HotelImagesController>(nameof(HotelImagesController.DeleteImage), typeof(int), typeof(int));

        AssertAdminOnly<HotelAmenitiesController>(nameof(HotelAmenitiesController.AddAmenities), typeof(int), typeof(HotelAmenityAssignDto));
        AssertAdminOnly<HotelAmenitiesController>(nameof(HotelAmenitiesController.RemoveAmenity), typeof(int), typeof(int));

        AssertAdminOnly<RoomTypeController>(nameof(RoomTypeController.Create), typeof(CreateRoomTypeDto));
        AssertAdminOnly<RoomTypeController>(nameof(RoomTypeController.Update), typeof(int), typeof(UpdateRoomTypeRequestDto));
        AssertAdminOnly<RoomTypeController>(nameof(RoomTypeController.Delete), typeof(int));

        AssertAdminOnly<HotelRoomController>(nameof(HotelRoomController.AddRoom), typeof(int), typeof(CreateHotelRoomRequestDto));
        AssertAdminOnly<HotelRoomController>(nameof(HotelRoomController.UpdateRoom), typeof(int), typeof(int), typeof(UpdateHotelRoomRequestDto));
        AssertAdminOnly<HotelRoomController>(nameof(HotelRoomController.Delete), typeof(int), typeof(int));
    }

    [Fact]
    public void AuthKorumaliEndpointleri_AuthorizeIleKorunur()
    {
        AssertHasAuthorize<AuthController>(nameof(AuthController.GetProfile));
        AssertHasAuthorize<AuthController>(nameof(AuthController.UpdateProfile), typeof(ProfileDto));
    }

    [Fact]
    public void EmailDogrulamaEndpointleri_AnonymousErisimeAciktir()
    {
        AssertHasAllowAnonymous<AuthController>(nameof(AuthController.VerifyEmail), typeof(string));
        AssertHasAllowAnonymous<AuthController>(nameof(AuthController.ResendVerification), typeof(ResendVerificationDto));
    }

    private static AuthController CreateAuthController(ClaimsPrincipal? user = null)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = user ?? new ClaimsPrincipal(new ClaimsIdentity());

        return new AuthController(new FakeAuthService(), new FakeUserService(), new FakeLoggerService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private static void AssertAdminOnly<TController>(string methodName, params Type[] parameterTypes)
    {
        var authorize = GetRequiredMethod<TController>(methodName, parameterTypes)
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal("Admin", authorize!.Roles);
    }

    private static void AssertHasAuthorize<TController>(string methodName, params Type[] parameterTypes)
    {
        var authorize = GetRequiredMethod<TController>(methodName, parameterTypes)
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
    }

    private static void AssertHasAllowAnonymous<TController>(string methodName, params Type[] parameterTypes)
    {
        var allowAnonymous = GetRequiredMethod<TController>(methodName, parameterTypes)
            .GetCustomAttribute<AllowAnonymousAttribute>();

        Assert.NotNull(allowAnonymous);
    }

    private static MethodInfo GetRequiredMethod<TController>(string methodName, params Type[] parameterTypes)
    {
        var method = typeof(TController).GetMethod(methodName, parameterTypes);
        Assert.NotNull(method);
        return method!;
    }
}

internal class FakeHotelService : IHotelService
{
    public HotelDetailDto? HotelById { get; set; }
    public ArgumentException? SearchException { get; set; }
    public PagedResponseDto<HotelListDto> FilteredHotels { get; set; } = new();
    public HotelSearchResponseDto SearchResult { get; set; } = new();

    public Task<IEnumerable<HotelListDto>> GetAllAsync() => Task.FromResult(Enumerable.Empty<HotelListDto>());
    public Task<HotelDetailDto?> GetByIdAsync(int id) => Task.FromResult(HotelById);
    public Task<IEnumerable<HotelListDto>> GetByCityAsync(string city) => Task.FromResult(Enumerable.Empty<HotelListDto>());
    public Task<IEnumerable<HotelListDto>> GetActiveAsync() => Task.FromResult(Enumerable.Empty<HotelListDto>());
    public Task CreateAsync(HotelCreateDto dto) => Task.CompletedTask;
    public Task UpdateAsync(HotelUpdateDto dto) => Task.CompletedTask;
    public Task DeleteAsync(int id) => Task.CompletedTask;
    public Task<List<string>> GetCitiesAsync() => Task.FromResult(new List<string>());
    public Task<List<CityCardDto>> GetFeaturedCitiesAsync(int count) => Task.FromResult(new List<CityCardDto>());

    public Task<HotelSearchResponseDto> SearchAsync(HotelSearchRequestDto dto, int page, int pageSize)
    {
        if (SearchException != null)
            throw SearchException;

        return Task.FromResult(SearchResult);
    }

    public Task<PagedResponseDto<HotelListDto>> GetAllFilteredAsync(string? city, byte? star, string? sortBy, string? sortOrder, int page, int pageSize)
        => Task.FromResult(FilteredHotels);
}

internal class FakeAmenityService : IAmenityService
{
    public List<AmenityDto> Amenities { get; set; } = new();
    public HotelAmenityAssignResponseDto AddAmenitiesResponse { get; set; } = new();
    public Task<List<AmenityDto>> GetAllAsync() => Task.FromResult(Amenities);
    public Task<AmenityDto?> GetByIdAsync(int id) => Task.FromResult<AmenityDto?>(null);
    public Task AddAsync(AmenityCreateDto dto) => Task.CompletedTask;
    public Task UpdateAsync(AmenityUpdateDto dto) => Task.CompletedTask;
    public Task DeleteAsync(int id) => Task.CompletedTask;
    public Task<List<AmenityDto>> GetAmenitiesByHotelIdAsync(int hotelId) => Task.FromResult(new List<AmenityDto>());
    public Task<HotelAmenityAssignResponseDto> AddAmenitiesToHotelAsync(int hotelId, List<int> amenityIds) => Task.FromResult(AddAmenitiesResponse);
    public Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId) => Task.CompletedTask;
}

internal class FakeHotelImageService : IHotelImageService
{
    public List<HotelImageDto> Images { get; set; } = new();
    public int? LastAddedHotelId { get; private set; }

    public Task<List<HotelImageDto>> GetByHotelIdAsync(int hotelId) => Task.FromResult(Images);
    public Task<HotelImageDto?> GetByIdAsync(int id) => Task.FromResult<HotelImageDto?>(null);
    public Task<HotelImageDto?> GetMainImageAsync(int hotelId) => Task.FromResult<HotelImageDto?>(null);

    public Task AddAsync(int hotelId, HotelImageCreateDto dto)
    {
        LastAddedHotelId = hotelId;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(int hotelId, HotelImageUpdateDto dto) => Task.CompletedTask;
    public Task DeleteAsync(int hotelId, int imageId) => Task.CompletedTask;
    public Task SetMainImageAsync(int hotelId, int imageId) => Task.CompletedTask;
}

internal class FakeRoomTypeService : IRoomTypeService
{
    public RoomTypeDto? RoomTypeById { get; set; }
    public RoomTypeDto? RoomTypeByName { get; set; }

    public Task<IEnumerable<RoomTypeDto>> GetAllRoomTypesAsync() => Task.FromResult(Enumerable.Empty<RoomTypeDto>());
    public Task<RoomTypeDto?> GetRoomTypeByIdAsync(int id) => Task.FromResult(RoomTypeById);
    public Task<RoomTypeDto?> GetRoomTypeByNameAsync(string name) => Task.FromResult(RoomTypeByName);
    public Task<IEnumerable<RoomTypeDto>> GetRoomTypesByMaxGuestsAsync(int maxGuests) => Task.FromResult(Enumerable.Empty<RoomTypeDto>());
    public Task AddRoomTypeAsync(CreateRoomTypeDto dto) => Task.CompletedTask;
    public Task UpdateRoomTypeAsync(UpdateRoomTypeDto dto) => Task.CompletedTask;
    public Task DeleteRoomTypeAsync(int id) => Task.CompletedTask;
}

internal class FakeHotelRoomService : IHotelRoomService
{
    public DetailHotelRoomDto? RoomById { get; set; }

    public Task<IEnumerable<HotelRoomDto>> GetAllRoomsAsync() => Task.FromResult(Enumerable.Empty<HotelRoomDto>());
    public Task<DetailHotelRoomDto?> GetRoomByIdAsync(int id) => Task.FromResult(RoomById);
    public Task<IEnumerable<HotelRoomDto>> GetRoomsByHotelAsync(int hotelId) => Task.FromResult(Enumerable.Empty<HotelRoomDto>());
    public Task<IEnumerable<HotelRoomDto>> GetRoomsByTypeAsync(int roomTypeId) => Task.FromResult(Enumerable.Empty<HotelRoomDto>());
    public Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(int hotelId, string city, DateTime checkIn, DateTime checkOut, int guestCount) => Task.FromResult(Enumerable.Empty<AvailableRoomDto>());
    public Task AddRoomsAsync(CreateHotelRoomDto dto) => Task.CompletedTask;
    public Task UpdateRoomsAsync(UpdateHotelRoomDto dto) => Task.CompletedTask;
    public Task DeleteRoomsAsync(int id) => Task.CompletedTask;
}

internal class FakeAuthService : IAuthService
{
    public Task<RegisterResponseDto> RegisterAsync(UserRegisterDto registerRequest) => Task.FromResult(new RegisterResponseDto());
    public Task<LoginResponseDto> LoginAsync(UserLoginDto loginRequest) => Task.FromResult(new LoginResponseDto
    {
        Token = "token",
        Email = loginRequest.Email,
        FullName = "Test User",
        ExpiresAt = DateTime.UtcNow.AddMinutes(30)
    });
    public Task<LoginResponseDto> VerifyEmailAsync(string token) => Task.FromResult(new LoginResponseDto
    {
        Token = "token",
        Email = "test@hotelapp.com",
        FullName = "Test User",
        ExpiresAt = DateTime.UtcNow.AddMinutes(30)
    });
    public Task<ResendVerificationResultDto> ResendVerificationAsync(ResendVerificationDto dto) => Task.FromResult(new ResendVerificationResultDto());
}

internal class FakeUserService : IUserService
{
    public Task<UserDto> GetProfileAsync(int userId) => Task.FromResult(new UserDto { Id = userId, Email = "test@hotelapp.com" });
    public Task<UserDto> UpdateProfileAsync(int userId, ProfileDto profileDto) => Task.FromResult(new UserDto
    {
        Id = userId,
        FirstName = profileDto.FirstName,
        LastName = profileDto.LastName,
        Phone = profileDto.Phone,
        Email = "test@hotelapp.com"
    });

    public Task ChangePasswordAsync(int userId, UserChangePasswordDto changePasswordDto) => Task.CompletedTask;
}

internal class FakeLoggerService : ILoggerService
{
    public Task LogInfoAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null) => Task.CompletedTask;
    public Task LogWarningAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null) => Task.CompletedTask;
    public Task LogErrorAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null, string? exceptionDetail = null) => Task.CompletedTask;
    public Task<IEnumerable<LogDto>> GetLogsAsync(LogFilterDto filter) => Task.FromResult(Enumerable.Empty<LogDto>());
    public Task<int> GetLogCountAsync(LogFilterDto filter) => Task.FromResult(0);
    public Task<LogDto?> GetLogByIdAsync(int id) => Task.FromResult<LogDto?>(null);
}
