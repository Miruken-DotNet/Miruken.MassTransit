// ReSharper disable RedundantAssignment
namespace IntegrationTests.Setup
{
    using System;
    using System.Threading.Tasks;
    using Docker.DotNet.Models;
    using Domain;
    using MassTransit;
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using MassTransit.Registration;
    using Microsoft.Extensions.Configuration;
    using Miruken.MassTransit;
    using Miruken.MassTransit.Api;
    using RabbitMQ.Client;
    
    public class RabbitMqSetup : DockerMassTransitSetup
    {
        private const int Port = 5672;
       
        public RabbitMqSetup() 
            : base("rabbitmq", "3-management", Port)
        {
        }

        public override string HostName => "rabbitmq";

        public override Uri CreateQueueUri(string queueName)
        {
            var rabbitUri = new Uri("rabbitmq://localhost");
            return new Uri($"{rabbitUri}/{queueName}");
        }

        public override Action<IServiceCollectionBusConfigurator> Configure(string queueName) =>
            mt => mt.AddBus(sp => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
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
            }));

        public override IBusControl CreateClientBus()
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
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
        }

        protected override Task<bool> TestReady(int externalPort)
        {
            var factory = new ConnectionFactory {
                HostName =                 "localhost",
                Port     =                 externalPort,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(1)
            };

            try
            {
                using var connection = factory.CreateConnection();
                return Task.FromResult(connection.IsOpen);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        protected override CreateContainerParameters ConfigureContainer(
            ConfigurationBuilder configuration,
            ref int              externalPort)
        {
            externalPort = Port;
            return new CreateContainerParameters
            {
                Hostname = HostName
            };
        }
    }
}