using MassTransit;
using Microsoft.EntityFrameworkCore;
using Nvovka.CommandManager.Commands.CommandHandler;
using Nvovka.CommandManager.Contract.Messages;
using Nvovka.CommandManager.Contract.Options;
using Nvovka.CommandManager.Data;

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

                // Dynamically build URI for the endpoint
                Uri commandMessageEndpointUri = new Uri("queue:nvovka-commands");

                EndpointConvention.Map<ICreateCommandMessage>(commandMessageEndpointUri);
                EndpointConvention.Map<IUpdateCommandStatusMessage>(commandMessageEndpointUri);
            });

            x.AddConfigureEndpointsCallback((name, cfg) =>
            {
                cfg.UseMessageRetry(r => r.Immediate(2));
            });

        });

       // services.AddMassTransitHostedService();

      //  EndpointConvention.Map<ICreateCommandMessage>(rfpEmailCommandBusUri);
    }
}