using Microsoft.EntityFrameworkCore;
using Nvovka.CommandManager.Contract.Models;

namespace Nvovka.CommandManager.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Parameterless constructor for EF Core tools
    ////public AppDbContext()
    ////{
    ////}
    public DbSet<CommandItem> CommandItems { get; set; }
    public DbSet<CommandReferenceItem> CommandReferenceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<CommandItem>()
       .HasMany(ci => ci.CommandReferenceItems)
       .WithOne()
       .HasForeignKey(cri => cri.CommandItemId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(new Interceptor());
    }
}
