using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Nvovka.CommandManager.Api.Services;
using Nvovka.CommandManager.Contract.Options;
using Nvovka.CommandManager.Contract.Servcies;
using Nvovka.CommandManager.Data.Repository;
using Nvovka.CommandManager.Data;

namespace Nvovka.CommandManager.Api.Extensions;

public static class ServcieCollectionExtendsions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions()
            .Configure<BasicLoginOption>(configuration.GetSection(BasicLoginOption.SectionName))
            .Configure<RabbitMqConfigOptions>(configuration.GetSection(RabbitMqConfigOptions.SectionName));

        return services;
    }
    public static IServiceCollection RegisterMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMassTransitBusUriGenerator, MassTransitBusUriGenerator>();
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
        return services;
    }

    public static IServiceCollection RegisterGrpcServcies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IGrpcLoginService, GrpcLoginService>();
        return services;
    }

    public static IServiceCollection RegisterSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: 'Bearer abc123'"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {} // No specific scopes
                        }
                    });
        });
        return services;
    }

    public static IServiceCollection RegisterSqlServcies(this IServiceCollection services, IConfiguration configuration)
    {
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
        return services;
    }
}
