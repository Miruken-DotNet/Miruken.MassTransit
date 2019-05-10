using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IntegrationTests.Domain;
using MassTransit;
using MassTransit.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miruken.Callback;
using Miruken.Castle;
using Miruken.Context;
using Miruken.MassTransit;
using Miruken.MassTransit.Api;

namespace IntegrationTests
{
    public abstract class TestScenario
    {
        private IBusControl         bus;

        protected IWindsorContainer container;
        protected Context           appContext;
        protected Uri               queueUri;
        protected IBusControl       clientBus;

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
            appContext = new Context();

            container = new WindsorContainer();
            container.Kernel.AddHandlersFilter(new ContravariantFilter());
            appContext.AddHandlers(new WindsorHandler(container).Infer());
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
                Component.For<IHandler>().Instance(appContext),
                Classes.FromThisAssembly().BasedOn(typeof(IConsumer<>)),
                Classes.FromAssemblyContaining<SendConsumer>().BasedOn(typeof(IConsumer<>))
            );

            var rabbitUri = new Uri("rabbitmq://localhost");
            var queueName = "miruken_masstransit_integration_tests";
            queueUri      = new Uri($"{rabbitUri}{queueName}");

            //configure MassTransit consumer
            container.AddMassTransit(c =>
            {

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
            TaskUtil.Await(() => bus.StartAsync());

            //Create client bus
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

            TaskUtil.Await(() => clientBus.StartAsync());
        }
    }
}
