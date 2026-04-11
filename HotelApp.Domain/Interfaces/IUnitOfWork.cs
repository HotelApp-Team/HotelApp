namespace HotelApp.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAuthRepository AuthRepository { get; }

    // Tüm değişiklikleri kaydet
    Task<int> SaveChangesAsync();

    // Transaction başlat
    Task BeginTransactionAsync();

    // Transaction onayla
    Task CommitTransactionAsync();

    // Transaction geri al
    Task RollbackTransactionAsync();

}
