using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Nvovka.CommandManager.Contract.Messages;
using Nvovka.CommandManager.Worker;
using Nvovka.CommandManager.Worker.Consumers;

namespace Nvovka.CommandManager.Tests;

public class TestsFixture : BaseStartupFixture<Startup>, IAsyncLifetime
{

    private TimeSpan DefaultHarnessTestTimeout = TimeSpan.FromMinutes(1);

   // public readonly InMemoryTestHarness TestHarness;

    public TestsFixture()
    {
        // TestHarness = base.Services.GetTestHarness();
        TestHarness = new InMemoryTestHarness
        {
            TestTimeout = DefaultHarnessTestTimeout,
            TestInactivityTimeout = Timeout.InfiniteTimeSpan
        };

        ////TestHarness.OnConfigureInMemoryBus += configurator =>
        ////    {
        ////        configurator.ReceiveEndpoint(TestHarness.InputQueueName, endpoint =>
        ////        {
        ////            endpoint.Consumer<CreateCommandConsumer>();
        ////        });
        ////    };
         //// CreateCommandConsumer = TestHarness.Consumer<CreateCommandConsumer>();
        ////CreateCommandConsumer = TestHarness.Consumer(() => Services.GetRequiredService<CreateCommandConsumer>());
        EndpointConvention.Map<ICreateCommandMessage>(TestHarness.BaseAddress);
    }


    public ConsumerTestHarness<CreateCommandConsumer> CreateCommandConsumer { get; }

    public async Task InitializeAsync()
    {
        await TestHarness.Start();
        // ConfigureMock();
    }

    ////protected override void configureservices(iservicecollection services)
    ////{

    ////    services.addtransient(_ => testharness.bus);
    ////    services.addtransient(_ => testharness.buscontrol);
    ////    services.addtransient<ipublishendpoint>(_ => testharness.bus);
    ////    services.addsingleton(_ => mock.of<ibusdepot>());
    ////    services.addscoped<createcommandconsumer>();
    ////    services.addmasstransittestharness();
    ////    //// var mock = new mock<ireceiveendpointconfigurator>();
    ////    ////// mock.setup(m => m.(it.isany<string>())).returns(testharness.baseaddress);

    ////    //// services.addsingleton(mock.object);
    ////}

    async Task IAsyncLifetime.DisposeAsync()
    {
        await TestHarness.Stop();
        TestHarness.Dispose();
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        return Host
            .CreateDefaultBuilder(null)
            .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseContentRoot(".");
        builder.UseEnvironment("Testing");
    }

    protected override void ConfigureServices(IServiceCollection services)
    {

        services.AddTransient(_ => TestHarness.Bus);
        services.AddTransient(_ => TestHarness.BusControl);
        services.AddTransient<IPublishEndpoint>(_ => TestHarness.Bus);
        services.AddSingleton(_ => Mock.Of<IBusDepot>());
        services.AddScoped<CreateCommandConsumer>();
        services.AddMassTransitTestHarness();
        //// var mock = new mock<ireceiveendpointconfigurator>();
        ////// mock.setup(m => m.(it.isany<string>())).returns(testharness.baseaddress);

         ////services.AddSingleton(mock.object);
    }
}