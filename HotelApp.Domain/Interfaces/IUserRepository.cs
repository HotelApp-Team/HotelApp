using HotelApp.Domain.Entities;

// IUserRepository, kullaniciya ait veri erisim sozlesmesini tanimlar.
// Amaci, user islemlerinde gerekli ozel sorgular icin abstraction saglamaktir.
// SOLID: DIP ve ISP kullanir.
namespace HotelApp.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
}
