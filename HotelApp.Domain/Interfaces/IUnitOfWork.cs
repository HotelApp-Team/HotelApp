namespace HotelApp.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    //Repository'lere erişim
    IAuthRepository AuthRepository { get; }
    IUserRepository UserRepository { get; }

    // Tüm değişiklikleri kaydet
    Task<int> SaveChangesAsync();

    // Transaction başlat
    Task BeginTransactionAsync();

    // Transaction onayla
    Task CommitTransactionAsync();

    // Transaction geri al
    Task RollbackTransactionAsync();

}
