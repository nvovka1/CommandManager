using MassTransit;
using Microsoft.EntityFrameworkCore;
using Nvovka.CommandManager.Commands.CommandHandler;
using Nvovka.CommandManager.Contract.Options;
using Nvovka.CommandManager.Data;
using Nvovka.CommandManager.Data.Repository;

namespace Nvovka.CommandManager.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
             .AddOptions()
             .Configure<RabbitMqConfigOptions>(configuration.GetSection(RabbitMqConfigOptions.SectionName));

        services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
             sqlOptions => sqlOptions.EnableRetryOnFailure()));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICommandDupperRepository>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            return new CommandDupperRepository(connectionString!);
        });

        services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(GetCommandHandler).Assembly));

        var rabbitOptions = configuration
        .GetSection(RabbitMqConfigOptions.SectionName)
        .Get<RabbitMqConfigOptions>();

        services.AddMassTransit(x =>
        {
            // A Transport
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitOptions.HostName, "/", conf =>
                {
                    conf.Username(rabbitOptions.UserName);
                    conf.Password(rabbitOptions.Password);
                });

                if (!rabbitOptions.HostName.EndsWith("/"))
                {
                    rabbitOptions.HostName += "/";
                }
               
            });

            x.AddConfigureEndpointsCallback((name, cfg) =>
            {
                cfg.UseMessageRetry(r => r.Immediate(2));
            });
        });

        // Dynamically build URI for the endpoint
        ////Uri commandMessageEndpointUri = new Uri("exchange:nvovka-commands");

        ////EndpointConvention.Map<ICreateCommandMessage>(commandMessageEndpointUri);
        ////EndpointConvention.Map<IUpdateCommandStatusMessage>(commandMessageEndpointUri);

        //services.AddMassTransitHostedService();

        //  EndpointConvention.Map<ICreateCommandMessage>(rfpEmailCommandBusUri);
    }
}