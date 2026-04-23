using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using HotelApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

// UnitOfWork, repository'leri tek merkezden yonetir ve SaveChanges/transaction kontrolu saglar.
// Amaci, ayni is akisi icindeki veri degisikliklerini birlikte yonetmektir.
// SOLID: SRP kullanir, DIP icin IUnitOfWork interface'ini uygular.
namespace HotelApp.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly HotelAppDbContext _context;
    private IDbContextTransaction? _transaction;
    private IAuthRepository? _authRepository;
    private IHotelRepository? _hotelRepository;
    private IHotelImageRepository? _hotelImageRepository;
    private IAmenityRepository? _amenityRepository;
    private IUserRepository? _userRepository;

    public UnitOfWork(HotelAppDbContext context)
    {
        _context = context;
    }

    // Lazy loading: ilk erisimde olustur, sonrakilerde ayni instance'i don
    public IAuthRepository AuthRepository =>
        _authRepository ??= new AuthRepository(_context);

    public IHotelRepository HotelRepository =>
        _hotelRepository ??= new HotelRepository(_context);

    public IHotelImageRepository HotelImageRepository =>
        _hotelImageRepository ??= new HotelImageRepository(_context);

    public IAmenityRepository AmenityRepository =>
        _amenityRepository ??= new AmenityRepository(_context);

    public IUserRepository UserRepository =>
        _userRepository ??= new UserRepository(_context);


    /// Tüm değişiklikleri veritabanına kaydeder.
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // Transaction başlatır.
    /// Birden fazla işlemi atomik olarak yapmak için kullanılır.
    public async Task BeginTransactionAsync()
    {
        // Zaten acik bir transaction varsa ustune yazmayiz: onceki transaction
        // dispose edilmeden kaybolacagi icin resource leak olusur ve ic ice
        // transaction gibi beklenmedik davranislara yol acar.
        // Once mevcut transaction commit veya rollback ile kapatilmali.
        if (_transaction != null)
            throw new InvalidOperationException("Aktif bir transaction zaten var. Once onu commit veya rollback ile kapatin.");

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
        // DbContext'i burada dispose ETMIYORUZ. Program.cs'te AddDbContext ile
        // scoped kaydedildigi icin DI container istek bitince context'i kendisi dispose eder.
        // Burada tekrar dispose cagirmak double-dispose hatasina yol acabilir.
        // UnitOfWork sadece kendi olusturdugu kaynagi (transaction) temizler.
        _transaction?.Dispose();
    }
}
