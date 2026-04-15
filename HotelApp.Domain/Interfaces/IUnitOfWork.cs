namespace HotelApp.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    //Repository'lere erişim
    IAuthRepository AuthRepository { get; }
    IHotelRoomRepository HotelRoomRepository { get; }
    IRoomTypeRepository RoomTypeRepository { get; }
    IUserRepository UserRepository { get; }

    IHotelRepository HotelRepository { get; }

    IHotelImageRepository HotelImageRepository { get; }

    IAmenityRepository AmenityRepository { get; }

    // Tüm değişiklikleri kaydet
    Task<int> SaveChangesAsync();

    // Transaction başlat
    Task BeginTransactionAsync();

    // Transaction onayla
    Task CommitTransactionAsync();

    // Transaction geri al
    Task RollbackTransactionAsync();

}
