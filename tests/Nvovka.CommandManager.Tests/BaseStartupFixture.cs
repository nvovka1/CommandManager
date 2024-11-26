using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Nvovka.CommandManager.Worker.Extensions;
using Xunit.Abstractions;

namespace Nvovka.CommandManager.Tests;

public abstract class BaseStartupFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    public InMemoryTestHarness TestHarness;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddMassTransitTestHarness(cfg =>
            {
                // cfg.AddDelayedMessageScheduler();

                cfg.RegisterConsumers();

                cfg.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });

                cfg.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            });

            this.ConfigureServices(services);
        });
    }

    public void Configure(ITestOutputHelper testHelper)
    {
        var app = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            this.ConfigureWebHost(builder);
        });

       ////  TestHarness = app.Services.GetTestHarness();
    }


    protected abstract void ConfigureServices(IServiceCollection services);
}
