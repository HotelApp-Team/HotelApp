namespace HotelApp.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    //Repository'lere erişim
    IAuthRepository AuthRepository { get; }
<<<<<<< HEAD

    IHotelRoomRepository HotelRoomRepository { get; }
    IRoomTypeRepository RoomTypeRepository { get; }

=======
    IHotelRoomRepository HotelRoomRepository { get; }
    IRoomTypeRepository RoomTypeRepository { get; }
>>>>>>> e41468883b5cb46972dbb50084b11aad5efe789f
    IUserRepository UserRepository { get; }

    IHotelRepository HotelRepository { get; }

    IHotelImageRepository HotelImageRepository { get; }

    IAmenityRepository AmenityRepository { get; }
<<<<<<< HEAD

=======
>>>>>>> e41468883b5cb46972dbb50084b11aad5efe789f

    // Tüm değişiklikleri kaydet
    Task<int> SaveChangesAsync();

    // Transaction başlat
    Task BeginTransactionAsync();

    // Transaction onayla
    Task CommitTransactionAsync();

    // Transaction geri al
    Task RollbackTransactionAsync();

}
