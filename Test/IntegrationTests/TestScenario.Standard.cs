namespace IntegrationTests
{
    using System;
    using Domain;
    using MassTransit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Callback;
    using Miruken.Context;
    using Miruken.MassTransit;
    using Miruken.MassTransit.Api;
    using Miruken.Register;

    public abstract class TestScenario
    {
        private IBusControl   bus;

        protected Context     appContext;
        protected IBusControl clientBus;
        protected Uri         queueUri;
        protected string      routeString;

        private const string queueName = "miruken_masstransit_integration_tests";

        [TestCleanup]
        public virtual void TestCleanup()
        {
            bus.Stop();
            clientBus.Stop();
            appContext.End();
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            var rabbitUri = new Uri("rabbitmq://localhost");
            queueUri      = new Uri($"{rabbitUri}{queueName}");
            routeString   = $"mt:{queueUri}";

            appContext = new ServiceCollection()
                .AddMiruken(configure => configure
                    .PublicSources(s => s.FromAssemblyOf<TestScenario>())
                    .WithMassTransit(mt =>
                         mt.AddBus(sp => Bus.Factory.CreateUsingRabbitMq(cfg =>
                         {
                             cfg.Host(rabbitUri, h =>
                             {
                                 h.Username("guest");
                                 h.Password("guest");
                             });

                             cfg.UseInMemoryOutbox();

                             cfg.ReceiveEndpoint(queueName, ep =>
                             {
                                 ep.Consumer<QueueThisConsumer>(sp);
                                 ep.Consumer<SendConsumer>(sp);
                                 ep.Consumer<PublishConsumer>(sp);
                                 ep.Consumer<RequestConsumer>(sp);
                             });

                             cfg.ConfigureJsonSerializer(x =>
                             {
                                 x.Converters.Insert(0, new MirukenJsonConverter());
                                 return x;
                             });

                             cfg.ConfigureJsonDeserializer(x =>
                             {
                                 x.Converters.Insert(0, new MirukenJsonConverter());
                                 return x;
                             });
                         })))
                ).Build();

            bus = appContext.Resolve<IBusControl>();
            bus.Start();

            clientBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(rabbitUri, h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureJsonSerializer(x =>
                {
                    x.Converters.Insert(0, new MirukenJsonConverter());
                    return x;
                });

                cfg.ConfigureJsonDeserializer(x =>
                {
                    x.Converters.Insert(0, new MirukenJsonConverter());
                    return x;
                });
            });

            clientBus.Start();
        }
    }
}

