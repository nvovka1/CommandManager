using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Nvovka.CommandManager.Worker.Consumers;
using Nvovka.CommandManager.Worker.Extensions;

namespace Nvovka.CommandManager.Tests;

public class TestServerFixture : IAsyncLifetime
{
    public WebApplicationFactory<Program> Factory { get; private set; }
    public HttpClient Client { get; private set; }

    public readonly InMemoryTestHarness TestHarness;

    public TestServerFixture()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
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
            });

        Client = Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await TestHarness.Start();
        // Optional: Initialize anything else, such as seeding the test database
    }

    public async Task DisposeAsync()
    {
        await TestHarness.Stop();
        TestHarness.Dispose();
        Client.Dispose();
        Factory.Dispose();

    }

    private void ConfigureServices(IServiceCollection services)
    {

        services.AddTransient(_ => TestHarness.Bus);
        services.AddTransient(_ => TestHarness.BusControl);
        services.AddTransient<IPublishEndpoint>(_ => TestHarness.Bus);
        services.AddSingleton(_ => Mock.Of<IBusDepot>());
        services.AddScoped<CreateCommandConsumer>();
      //  services.AddMassTransitTestHarness();
        //// var mock = new mock<ireceiveendpointconfigurator>();
        ////// mock.setup(m => m.(it.isany<string>())).returns(testharness.baseaddress);

        ////services.AddSingleton(mock.object);
    }
}
