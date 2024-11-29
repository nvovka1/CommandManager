using Microsoft.Extensions.Configuration;
using Nvovka.CommandManager.Authentication;
using Nvovka.CommandManager.Authentication.MiddleWare;
using Nvovka.CommandManager.Authentication.Services;
using Nvovka.CommandManager.Contract.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<BasicAuthInterceptor>();
});
builder.Services.AddOptions()
                .Configure<BasicLoginOption>(builder.Configuration
                .GetSection(BasicLoginOption.SectionName));

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
var app = builder.Build();
//app.UseMiddleware<BasicAuthMiddleware>();
app.MapGrpcService<GreeterService>();
app.MapGrpcService<LoginService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
