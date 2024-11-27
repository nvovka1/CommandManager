using MassTransit.Testing;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nvovka.CommandManager.Data;
using Nvovka.CommandManager.Worker.Consumers;
using Nvovka.CommandManager.Contract.Messages;

namespace Nvovka.CommandManager.Worker.Tests;

public abstract class TestsBase
{
    private readonly IServiceProvider _serviceProvider;
    protected static readonly TimeSpan DefaultHarnessTestTimeout = TimeSpan.FromSeconds(10);
    protected Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
    protected TestsBase()
    {
        var services = new ServiceCollection();

        // ReSharper disable once VirtualMemberCallInConstructor
        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<CreateCommandConsumer>();
      //  services.AddScoped<UpdateCommandStatusConsumer>();
        services.AddSingleton(UnitOfWorkMock.Object);
    }

    protected T Resolve<T>()
    {
        return _serviceProvider.GetService<T>();
    }

    ////protected (InMemoryTestHarness Harness, ConsumerTestHarness<IConsumer<TMessage>> Consumer) GetDefaultHarnessAndConsumer<TMessage>()
    ////  where TMessage : class
    ////{
    ////    var harness = new InMemoryTestHarness
    ////    {
    ////        TestTimeout = DefaultHarnessTestTimeout,
    ////        TestInactivityTimeout = Timeout.InfiniteTimeSpan
    ////    };

    ////    harness.OnConfigureInMemoryBus += configurator =>
    ////    {
    ////        //configurator.UseNewtonsoftJsonSerializer();
    ////        EndpointConvention.Map<ICreateCommandMessage>(harness.InputQueueAddress);
    ////    };

    ////   var cons = harness.Consumer<IConsumer<TMessage>>(Resolve<IConsumer<TMessage>>);
    ////    //ConsumerTestHarness<IConsumer<TMessage>> consumer = harness.Consumer(Resolve<IConsumer<TMessage>>);

    ////    return (harness, cons);
    ////}
}
