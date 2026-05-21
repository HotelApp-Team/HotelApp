namespace HotelApp.Domain.Interfaces;

// Amaç: Repository erişimini ve veritabanı kaydetme/transaction işlemlerini tek sözleşmede toplar.
// SOLID: ISP ile veri işlemleri için net bir sözleşme sunar; DIP ile servisler somut UnitOfWork sınıfına bağlı kalmaz.

public interface IUnitOfWork : IDisposable
{
    // Repository'lere tek yerden erişim sağlar.
    IAuthRepository AuthRepository { get; }
    IHotelRoomRepository HotelRoomRepository { get; }
    IRoomTypeRepository RoomTypeRepository { get; }
    IRoomViewRepository RoomViewRepository { get; }
    IReservationRepository ReservationRepository { get; }
    IReviewRepository ReviewRepository { get; }
    IUserRepository UserRepository { get; }
    IHotelRepository HotelRepository { get; }
    IHotelImageRepository HotelImageRepository { get; }
    IAmenityRepository AmenityRepository { get; }
    IRoomAmenityRepository RoomAmenityRepository { get; }
    IAdminRepository AdminRepository { get; }

    // Yapılan değişiklikleri veritabanına kaydeder.
    Task<int> SaveChangesAsync();

    // Yeni bir transaction başlatır.
    Task BeginTransactionAsync();

    // Açık transaction'ı onaylar.
    Task CommitTransactionAsync();

    // Açık transaction'ı geri alır.
    Task RollbackTransactionAsync();
}
