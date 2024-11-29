using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Nvovka.CommandManager.Api.Extensions;
using Nvovka.CommandManager.Commands.CommandHandler;

namespace Nvovka.CommandManager.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {

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

        services.AddAuthorization()
            .AddOptions(configuration)
            .RegisterSwagger(configuration)
            .RegisterGrpcServcies(configuration)
            .RegisterMassTransit(configuration)
            .AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(GetCommandHandler).Assembly))
            .RegisterSqlServcies(configuration);
    }
}