namespace IntegrationTests
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Callback;
    using Miruken.Context;
    using Miruken.MassTransit;
    using Miruken.MassTransit.Api;
    using Miruken.Register;
    using Setup;

    public abstract class MassTransitScenario
    {
        private readonly MassTransitSetup _massTransitSetup;
        private IBusControl   _bus;

        protected Context     AppContext;
        protected IBusControl ClientBus;
        protected Uri         QueueUri;
        protected string      RouteString;

        private const string QueueName = "miruken_masstransit_integration_tests";

        protected MassTransitScenario(MassTransitSetup massTransitSetup)
        {
            _massTransitSetup = massTransitSetup
                ?? throw new ArgumentNullException(nameof(massTransitSetup));
        }

        [TestInitialize]
        public async Task TestInitialize()
        {
            var services             = new ServiceCollection();
            var configurationBuilder = new ConfigurationBuilder();

            await _massTransitSetup.Setup(configurationBuilder, services);
            
            var rabbitUri = new Uri("rabbitmq://localhost");
            QueueUri      = new Uri($"{rabbitUri}{QueueName}");
            RouteString   = $"mt:{QueueUri}";

            AppContext = services
                .AddMiruken(configure => configure
                    .PublicSources(s => s.FromAssemblyOf<MassTransitScenario>())
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
        
        [TestCleanup]
        public async Task TestCleanup()
        {
            try
            {
                _bus.Stop();
                ClientBus.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            try
            {
                await _massTransitSetup.DisposeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            AppContext.End();
        }
    }
}

