using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using HotelApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelApp.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly HotelAppDbContext _context;
    private IDbContextTransaction? _transaction;
    private IAuthRepository? _authRepository;
    private IRoomTypeRepository? _roomTypeRepository;
    private IHotelRoomRepository? _hotelRoomRepository;

    public UnitOfWork(HotelAppDbContext context, IHotelRoomRepository hotelRoomRepository, IRoomTypeRepository roomTypeRepository)
    {
        _context = context;
    }

    // Lazy loading: ilk erisimde olustur, sonrakilerde ayni instance'i don
    public IAuthRepository AuthRepository =>
        _authRepository ??= new AuthRepository(_context);

    public IRoomTypeRepository RoomTypeRepository =>
        _roomTypeRepository ??= new RoomTypeRepository(_context);

    public IHotelRoomRepository HotelRoomRepository =>
        _hotelRoomRepository ??= new HotelRoomRepository(_context);


    /// Tüm değişiklikleri veritabanına kaydeder.
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // Transaction başlatır.
    /// Birden fazla işlemi atomik olarak yapmak için kullanılır.
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// Transaction'ı commit eder. Yani yapılan değişiklikleri kalıcı hale getirir.
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// Transaction'ı geri alır (rollback)
    /// Hata durumunda tüm değişiklikleri iptal etmek için kullanılır.
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// Kaynakları serbest bırakır.
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
