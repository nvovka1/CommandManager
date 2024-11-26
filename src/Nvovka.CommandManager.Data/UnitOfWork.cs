using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data.Repository;

namespace Nvovka.CommandManager.Data;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> GetRepository<T>() where T : class, IEntity
    {
        // Check if a repository of the given type already exists
        if (!_repositories.TryGetValue(typeof(T), out var repository))
        {
            // Create a new instance if it doesn't exist
            repository = new GenericRepository<T>(_context);
            _repositories[typeof(T)] = repository;
        }

        return (IGenericRepository<T>)repository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
