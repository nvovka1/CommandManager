using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nvovka.CommandManager.Contract.Models;

namespace Nvovka.CommandManager.Data.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async ValueTask AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public ValueTask UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return ValueTask.CompletedTask;
    }
}