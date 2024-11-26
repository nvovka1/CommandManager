using Nvovka.CommandManager.Contract.Models;

namespace Nvovka.CommandManager.Data.Repository;

public interface IGenericRepository<T> where T : IEntity
{
    Task<T> GetByIdAsync(int id);
    Task<IReadOnlyCollection<T>> GetAllAsync();
    ValueTask AddAsync(T entity);
    ValueTask UpdateAsync(T entity);
    ValueTask DeleteAsync(T entity);
}
