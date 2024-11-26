using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nvovka.CommandManager.Contract.Interfaces;

namespace Nvovka.CommandManager.Data;

public class Interceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var dateTimeChanges = eventData.Context.ChangeTracker.Entries()
            .Where(x => x.Entity is IHasDateTime);
        var dateTimeNow = DateTime.UtcNow;
        foreach (var entry in dateTimeChanges)
        {
            if (entry.Entity is IHasDateTime entity)
            {
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = dateTimeNow;
                    entity.ModifiedDate = dateTimeNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.CreatedDate = entity.CreatedDate;
                    entity.ModifiedDate = dateTimeNow;
                }
            }
        }


        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
