using System.Linq.Expressions;

// IRepository, tum entity'ler icin ortak CRUD sozlesmesini tanimlar.
// Amaci, tekrar eden veri erisim kurallarini ortak bir yapida toplamaktir.
namespace HotelApp.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
