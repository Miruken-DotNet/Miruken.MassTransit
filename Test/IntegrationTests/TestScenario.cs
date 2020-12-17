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
        private IBusControl   _bus;

        protected Context     AppContext;
        protected IBusControl ClientBus;
        protected Uri         QueueUri;
        protected string      RouteString;

        private const string QueueName = "miruken_masstransit_integration_tests";

        [TestCleanup]
        public virtual void TestCleanup()
        {
            _bus.Stop();
            ClientBus.Stop();
            AppContext.End();
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            var rabbitUri = new Uri("rabbitmq://localhost");
            QueueUri      = new Uri($"{rabbitUri}{QueueName}");
            RouteString   = $"mt:{QueueUri}";

            AppContext = new ServiceCollection()
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

                             cfg.ReceiveEndpoint(QueueName, ep =>
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

            _bus = AppContext.Resolve<IBusControl>();
            _bus.Start();

            ClientBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
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

            ClientBus.Start();
        }
    }
}

