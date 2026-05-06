using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using HotelApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelApp.Infrastructure;

// UnitOfWork sınıfının amacı, repository erişimini ve transaction yönetimini tek merkezde toplamaktır.
// Repository nesnelerini üretir, SaveChanges çalıştırır ve transaction başlatma/commit/rollback işlemlerini yönetir.
// SOLID: SRP kullanır; sadece veri erişimi koordinasyonundan sorumludur.
// SOLID: DIP destekler; üst katmanlar somut sınıf yerine IUnitOfWork arayüzüne bağlı kalır.

public class UnitOfWork : IUnitOfWork
{
    private readonly HotelAppDbContext _context;
    private IDbContextTransaction? _transaction;
    private IAuthRepository? _authRepository;
    private IRoomTypeRepository? _roomTypeRepository;
    private IHotelRoomRepository? _hotelRoomRepository;
    private IReviewRepository? _reviewRepository;
    private IReservationRepository? _reservationRepository;
    private IHotelRepository? _hotelRepository;
    private IHotelImageRepository? _hotelImageRepository;
    private IAmenityRepository? _amenityRepository;
    private IUserRepository? _userRepository;
    private IAdminRepository? _adminRepository;

    // Aynı request içinde kullanılacak DbContext'i alır.
    public UnitOfWork(HotelAppDbContext context)
    {
        _context = context;
    }

    // Repository ilk istendiğinde oluşturulur, sonra aynı nesne tekrar kullanılır.
    public IAuthRepository AuthRepository =>
        _authRepository ??= new AuthRepository(_context);

    public IRoomTypeRepository RoomTypeRepository =>
        _roomTypeRepository ??= new RoomTypeRepository(_context);

    public IHotelRoomRepository HotelRoomRepository =>
        _hotelRoomRepository ??= new HotelRoomRepository(_context);

    public IHotelRepository HotelRepository =>
        _hotelRepository ??= new HotelRepository(_context);

    public IHotelImageRepository HotelImageRepository =>
        _hotelImageRepository ??= new HotelImageRepository(_context);

    public IAmenityRepository AmenityRepository =>
        _amenityRepository ??= new AmenityRepository(_context);

    public IUserRepository UserRepository =>
        _userRepository ??= new UserRepository(_context);

    public IReservationRepository ReservationRepository =>
        _reservationRepository ??= new ReservationRepository(_context);

    public IReviewRepository ReviewRepository =>
        _reviewRepository ??= new ReviewRepository(_context);

    public IAdminRepository AdminRepository =>
        _adminRepository ??= new AdminRepository(_context);

    // Yapılan değişiklikleri veritabanına kaydeder.
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // Yeni bir transaction başlatır.
    public async Task BeginTransactionAsync()
    {
        // Aynı anda ikinci transaction başlatmayız; önce açık olan işlem bitmelidir.
        if (_transaction != null)
            throw new InvalidOperationException("Aktif bir transaction zaten var. Önce onu commit veya rollback ile kapatın.");

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    // Açık transaction'ı onaylar ve kapatır.
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    // Hata olursa açık transaction'ı geri alır ve kapatır.
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    // UnitOfWork kapanırken açık transaction varsa temizler.
    public void Dispose()
    {
        // DbContext'i burada kapatmıyoruz; onu DI container request sonunda kapatır.
        // Burada sadece UnitOfWork'ün yönettiği transaction temizlenir.
        _transaction?.Dispose();
    }
}
