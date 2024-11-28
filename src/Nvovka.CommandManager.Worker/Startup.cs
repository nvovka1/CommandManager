using Microsoft.EntityFrameworkCore;
using Nvovka.CommandManager.Contract.Servcies;
using Nvovka.CommandManager.Data;
using Nvovka.CommandManager.Worker.Extensions;

namespace Nvovka.CommandManager.Worker;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IMassTransitBusUriGenerator, MassTransitBusUriGenerator>();
        services.AddMassTransitServices(configuration);

        services.AddDbContext<AppDbContext>(options =>
              options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
    }
}
