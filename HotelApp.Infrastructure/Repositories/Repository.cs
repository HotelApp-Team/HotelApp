using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// Repository, ortak CRUD islemlerinin temel implementasyonunu saglar.
// Amaci, tekrar eden EF Core kodunu tek yerde toplamak ve alt repository'leri sade tutmaktir.
// SOLID: SRP kullanir, OCP icin kalitimla genisletilebilir.
namespace HotelApp.Infrastructure.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly HotelAppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    // Constructor ile hem DbContext'i hem de ilgili entity'nin DbSet'ini hazirliyoruz
    protected Repository(HotelAppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        // 1. Tum kayitlari getir
        // 2. AsNoTracking ile sadece okuma amacli calistir
        return await _dbSet
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        // Verilen primary key ile tek bir kaydi bulur
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
    {
        // Gonderilen kosula uyan ilk kaydi bulur
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        // Kosula uyan herhangi bir kayit var mi kontrol eder
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task AddAsync(T entity)
    {
        // Yeni entity'yi context'e ekler, kaydetme islemi UnitOfWork tarafinda yapilir
        await _dbSet.AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        // Var olan entity'yi guncellenmis olarak isaretler
        _dbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        // Entity'yi silinmek uzere isaretler
        _dbSet.Remove(entity);
    }
}
