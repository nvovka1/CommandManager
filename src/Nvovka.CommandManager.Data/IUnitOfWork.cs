using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data.Repository;

namespace Nvovka.CommandManager.Data;

public interface IUnitOfWork: IDisposable
{
    IGenericRepository<T> GetRepository<T>() where T : class, IEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public AppDbContext appDbContext();
}
