using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nvovka.CommandManager.Commands.CommandHandler;
using Nvovka.CommandManager.Contract.Options;
using Nvovka.CommandManager.Contract.Servcies;
using Nvovka.CommandManager.Data;
using Nvovka.CommandManager.Data.Repository;

namespace Nvovka.CommandManager.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Add JWT Bearer Security Definition
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: 'Bearer abc123'"
            });

            // Add Security Requirement
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
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey("MySecretKeyForMyApplicationVeryLong"u8.ToArray()),
                    ValidIssuer = "http://nvovka.com",
                    ValidAudience = "http://localhost:5000",
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                };
            });

        services
             .AddOptions()
             .Configure<RabbitMqConfigOptions>(configuration.GetSection(RabbitMqConfigOptions.SectionName));

        services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
             sqlOptions => sqlOptions.EnableRetryOnFailure()));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IMassTransitBusUriGenerator, MassTransitBusUriGenerator>();

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