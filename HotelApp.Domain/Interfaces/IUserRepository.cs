using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

// IUserRepository arayüzünün amacı, kullanıcı profil işlemleri için veri erişim sözleşmesi tanımlamaktır.
// Kullanıcı verisini repository pattern üzerinden okumak ve genişletmek için ayrı bir sözleşme sağlar.
// SOLID: ISP kullanır; kullanıcıya özel repository ihtiyacını ayrı tutar.
// SOLID: DIP sağlar; servis katmanı somut UserRepository yerine bu arayüze bağlı kalır.

public interface IUserRepository : IRepository<User>
{
}
