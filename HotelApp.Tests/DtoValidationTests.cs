using HotelApp.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace HotelApp.Tests;

public class DtoValidationTests
{
    [Fact]
    public void UserRegisterDto_SifreAltiKarakterdenKisaysa_Gecersizdir()
    {
        var dto = new UserRegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@hotelapp.com",
            Password = "12345",
            Phone = "5551112233"
        };

        Assert.False(IsValid(dto));
    }

    [Fact]
    public void UserLoginDto_EmailFormatiYanlissa_Gecersizdir()
    {
        var dto = new UserLoginDto
        {
            Email = "gecersiz-email",
            Password = "123456"
        };

        Assert.False(IsValid(dto));
    }

    [Fact]
    public void HotelCreateDto_ZorunluAlanlarEksikse_Gecersizdir()
    {
        var dto = new HotelCreateDto
        {
            StarRating = 5
        };

        Assert.False(IsValid(dto));
    }

    [Fact]
    public void HotelCreateDto_YildizBesinUzerindeyse_Gecersizdir()
    {
        var dto = CreateValidHotelCreateDto();
        dto.StarRating = 6;

        Assert.False(IsValid(dto));
    }

    [Fact]
    public void HotelCreateDto_TumAlanlarDogruysa_Gecerlidir()
    {
        var dto = CreateValidHotelCreateDto();

        Assert.True(IsValid(dto));
    }

    [Fact]
    public void CreateRoomTypeDto_MaxGuestsBirdenKucukse_Gecersizdir()
    {
        var dto = new CreateRoomTypeDto
        {
            Name = "Standart",
            Description = "Standart oda",
            MaxGuests = 0
        };

        Assert.False(IsValid(dto));
    }

    [Fact]
    public void CreateHotelRoomDto_FiyatSifirsa_Gecersizdir()
    {
        var dto = new CreateHotelRoomDto
        {
            HotelId = 1,
            RoomTypeId = 1,
            PricePerNight = 0,
            TotalRooms = 1
        };

        Assert.False(IsValid(dto));
    }

    [Fact]
    public void AmenityCreateDto_NameBosIse_Gecersizdir()
    {
        var dto = new AmenityCreateDto();

        Assert.False(IsValid(dto));
    }

    [Fact]
    public void HotelImageCreateDto_ImageUrlBosIse_Gecersizdir()
    {
        var dto = new HotelImageCreateDto
        {
            DisplayOrder = 0
        };

        Assert.False(IsValid(dto));
    }

    [Fact]
    public void HotelImageCreateDto_DisplayOrderNegatifse_Gecersizdir()
    {
        var dto = new HotelImageCreateDto
        {
            ImageUrl = "https://example.com/hotel.jpg",
            DisplayOrder = -1
        };

        Assert.False(IsValid(dto));
    }

    [Fact]
    public void HotelImageUpdateDto_GecerliAlanlarla_Gecerlidir()
    {
        var dto = new HotelImageUpdateDto
        {
            Images = new List<HotelImageUpdateItemDto>
            {
                new() { Id = 1, IsMain = true, DisplayOrder = 0 }
            }
        };

        Assert.True(IsValid(dto));
    }

    private static HotelCreateDto CreateValidHotelCreateDto()
    {
        return new HotelCreateDto
        {
            Name = "Test Hotel",
            Address = "Test Address",
            CityId = 1,
            Country = "Turkiye",
            Phone = "5551112233",
            Email = "info@testhotel.com",
            StarRating = 5
        };
    }

    private static bool IsValid(object dto)
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
    }
}
