using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Nvovka.CommandManager.Contract.Messages;
using Nvovka.CommandManager.Data;
using Nvovka.CommandManager.Worker.Consumers;

namespace Nvovka.CommandManager.Worker.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Should_Consume_MyMessage()
        {
            // Arrange
            await using var provider = new ServiceCollection()
                   .AddMassTransitTestHarness(static x =>
                   {
                       x.SetTestTimeouts(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
                       x.UsingInMemory(
                           (context, configurator) =>
                           {
                               configurator.ReceiveEndpoint(
                               "queue-test",
                               ep =>
                               {
                                   ep.ConfigureConsumer<CreateCommandConsumer>(context);

                                   ep.ConfigureConsumers(context);
                               });
                           });

                       x.AddConsumer<CreateCommandConsumer>();

                       EndpointConvention.Map<ICreateCommandMessage>(new Uri("exchange:queue-test"));
                   })
                   .BuildServiceProvider(true);

            var harness = new InMemoryTestHarness()
            {

            };//provider.GetRequiredService<ITestHarness>();

            await harness.Start();
            try
            {
                // Act
                await harness.Bus.Publish<ICreateCommandMessage>(new CreateCommandMessage("Test", "Test"));

                //   var consumer = harness.GetConsumerHarness<CreateCommandConsumer>();
                // var consumer = harness.Consumer<CreateCommandConsumer>();

                // Assert
                Assert.True(await harness.Consumed.Any<ICreateCommandMessage>());
                // Assert.True(await consumer.Consumed.Any<ICreateCommandMessage>());

            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task MailDataBatchEventConsumer_DoesNotThrow()
        {
            var provider = new ServiceCollection()
                 .AddTransient<IUnitOfWork, UnitOfWork>()
                 .AddScoped<CreateCommandConsumer>()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddConsumer<CreateCommandConsumer>();

                    EndpointConvention.Map<ICreateCommandMessage>(new Uri("exchange:queue-test"));
                    // cfg.AddConsumer<CreateCommandConsumer>();
                    cfg.AddConsumerTestHarness<CreateCommandConsumer>();

                    cfg.UsingInMemory(
                           (context, configurator) =>
                           {
                               configurator.ReceiveEndpoint(
                               "queue-test",
                               ep =>
                               {
                                   ep.ConfigureConsumer<CreateCommandConsumer>(context);

                                   ep.ConfigureConsumers(context);
                               });
                           });
                })
                .BuildServiceProvider(true);

            await provider.StartTestHarness();

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();
            try
            {
                var bus = provider.GetRequiredService<IBus>();
                await bus.Publish<ICreateCommandMessage>(new CreateCommandMessage("Test", "Test"));
                //  IRequestClient<IMailData> client = bus.CreateRequestClient<IMailData>();

                //await client.GetResponse<IMailDataResponse>(new MailData());

                //Assert.That(await harness.Consumed.Any<IMailData>());

                var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<CreateCommandConsumer>>();
                Assert.True(await consumerHarness.Consumed.Any<ICreateCommandMessage>());
            }
            finally
            {
                await harness.Stop();

                await provider.DisposeAsync();
            }
        }
    }
}