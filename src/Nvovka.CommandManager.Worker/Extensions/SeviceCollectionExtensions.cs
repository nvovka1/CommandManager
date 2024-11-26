using MassTransit;
using Nvovka.CommandManager.Contract.Options;
using Nvovka.CommandManager.Worker.Consumers;
using Nvovka.CommandManager.Worker.Options;

namespace Nvovka.CommandManager.Worker.Extensions;

public static class SeviceCollectionExtensions
{
    public static IServiceCollection RegisterOptions(this IServiceCollection services, IConfiguration congiguration)
    {
        services
              .AddOptions()
              .Configure<CommandOptions>(congiguration.GetSection(CommandOptions.SectionName))
              .Configure<RabbitMqConfigOptions>(congiguration.GetSection(RabbitMqConfigOptions.SectionName));
        return services;
    }

    public static IBusRegistrationConfigurator RegisterConsumers(this IBusRegistrationConfigurator registretion)
    {
        registretion.AddConsumer<CreateCommandConsumer>();
        registretion.AddConsumer<UpdateCommandStatusConsumer>();

        return registretion;
    }

    public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration congiguration)
    {
        services.RegisterOptions(congiguration);

         ////services.RegisterSagaStateMachine<TaskItemSaga, TaskItemState>();

         ////services.AddSingleton<IDocumentConfiguration, TaskItemStateConfiguration>();

         var commandBusOptions = congiguration
            .GetSection(CommandOptions.SectionName)
            .Get<CommandOptions>();

        var rabbitOptions = congiguration
         .GetSection(RabbitMqConfigOptions.SectionName)
         .Get<RabbitMqConfigOptions>();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            ////x.AddSagaStateMachine<TaskItemSaga, TaskItemState>(configurator =>
            ////{
            ////    configurator.SelectOptions<CommandBusOptions>();
            ////}).MongoDbRepository(r =>
            ////{
            ////    r.Connection = commandBusOptions.MongoDbAddress;
            ////    r.DatabaseName = commandBusOptions.MongoDbOrderState;
            ////    r.CollectionName = "order-state-saga";
            ////});


            x.RegisterConsumers();

            x.UsingRabbitMq((busRegContext, cfg) =>
            {
                cfg.Host(rabbitOptions.HostName, "/", conf =>
                {
                    conf.Username(rabbitOptions.UserName);
                    conf.Password(rabbitOptions.Password);
                });

                cfg.ReceiveEndpoint(commandBusOptions.CommandQueue, configurator =>
                {
                    configurator.PrefetchCount = 1;
                    configurator.ConfigureConsumer<CreateCommandConsumer>(busRegContext);
                    configurator.ConfigureConsumer<UpdateCommandStatusConsumer>(busRegContext);
                });

                ////cfg.ReceiveEndpoint(commandBusOptions.AcceptedOrderQueue, configurator =>
                ////{
                ////    configurator.PrefetchCount = 1;
                ////    configurator.ConfigureConsumer<OrderAcceptedConsumer>(busRegContext);
                ////});


                ////cfg.ReceiveEndpoint(commandBusOptions.SagaQueueName,
                ////    ep =>
                ////    {
                ////        const ushort numberOfConsumers = 8;

                ////        ep.UseMessageRetry(
                ////            r =>
                ////            {
                ////                r.Handle<MongoDbConcurrencyException>();
                ////                r.Handle<MongoException>();
                ////                r.Immediate(3);
                ////            });

                ////        ep.UseInMemoryOutbox();

                ////        ep.StateMachineSaga<OrderState>(busRegContext, c =>
                ////        {
                ////            c.SelectOptions<CommandBusOptions>();
                ////        });
                ////    });
            });

        });
        return services;
    }
}
