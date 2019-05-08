using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IntegrationTests.Domain;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miruken.Callback;
using Miruken.Castle;
using Miruken.MassTransit;
using Miruken.MassTransit.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IntegrationTests
{
    public abstract class TestScenario
    {
        protected IWindsorContainer container;
        protected IHandler          handler;
        protected IBusControl       bus;
        protected Uri               queueUri;

        [TestCleanup]
        public virtual void TestCleanup()
        {
            bus.Stop();
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            container = new WindsorContainer();
            container.Kernel.AddHandlersFilter(new ContravariantFilter());
            handler = new WindsorHandler(container).Infer();
            container.Install(
                new FeaturesInstaller(
                    new HandleFeature().AddFilters(
                        new FilterAttribute(typeof(LogFilter<,>)))
                ).Use(
                    Types.FromThisAssembly(),
                    Types.FromAssemblyContaining<MassTransitRouter>(),
                    Types.FromAssemblyContaining<SendConsumer>()
                )
            );
            container.Register(
                Component.For<IHandler>().Instance(handler),
                Classes.FromThisAssembly().BasedOn(typeof(IConsumer<>)),
                Classes.FromAssemblyContaining<SendConsumer>().BasedOn(typeof(IConsumer<>))
            );

            //configure MassTransit
            container.AddMassTransit(c =>
            {
                var rabbitUri = new Uri("rabbitmq://localhost");
                var queueName = "miruken_masstransit_integration_tests";
                queueUri      = new Uri($"{rabbitUri}{queueName}");

                c.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(rabbitUri, h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.UseInMemoryOutbox();
                    cfg.ReceiveEndpoint(host, queueName, ep =>
                    {
                        ep.Consumer<QueueThisConsumer>(context);
                        ep.Consumer<SendConsumer>(context);
                        ep.Consumer<PublishConsumer>(context);
                        ep.Consumer<RequestConsumer>(context);
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
                }));
            });

            bus = container.Kernel.Resolve<IBusControl>();
            bus.Start();
        }
    }

    public class MirukenJsonConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            new JsonSerializer
                {
                    NullValueHandling              = NullValueHandling.Ignore,
                    TypeNameHandling               = TypeNameHandling.Auto,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    ContractResolver               = new CamelCasePropertyNamesContractResolver()
                }
                .Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new JsonSerializer
                {
                    NullValueHandling              = NullValueHandling.Ignore,
                    TypeNameHandling               = TypeNameHandling.Auto,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    ContractResolver               = new CamelCasePropertyNamesContractResolver()
                }.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return !IsMassTransitOrSystemType(objectType);
        }

        private static bool IsMassTransitOrSystemType(Type objectType)
        {
            return objectType.Assembly == typeof(MassTransit.IConsumer).Assembly ||
                   objectType.Assembly.IsDynamic ||
                   objectType.Assembly == typeof(object).Assembly;
        }
    }
}
