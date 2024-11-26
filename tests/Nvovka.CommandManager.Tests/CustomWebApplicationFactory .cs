using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Nvovka.CommandManager.Api;
using Nvovka.CommandManager.Worker.Extensions;

namespace Nvovka.CommandManager.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly InMemoryTestHarness TestHarness;
    private TimeSpan DefaultHarnessTestTimeout = TimeSpan.FromMilliseconds(100);
    public CustomWebApplicationFactory()
    {
        TestHarness = new InMemoryTestHarness
        {
            TestTimeout = DefaultHarnessTestTimeout,
            TestInactivityTimeout = Timeout.InfiniteTimeSpan
        };
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(static services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IServiceProvider));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<IServiceProviderFactory<ContainerBuilder>, AutofacServiceProviderFactory>();

            var containerBuilder = new ContainerBuilder();
     
            services.AddMassTransit(x =>
            {
                x.RegisterConsumers();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });
            var factory = new AutofacServiceProviderFactory();
            var container = factory.CreateBuilder(services);

            factory.CreateServiceProvider(container);
        });
    }
}
