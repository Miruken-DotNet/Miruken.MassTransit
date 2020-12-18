// ReSharper disable RedundantAssignment
namespace IntegrationTests.Setup
{
    using System;
    using System.Threading.Tasks;
    using Docker.DotNet.Models;
    using Microsoft.Extensions.Configuration;
    using RabbitMQ.Client;
    
    public class RabbitMqSetup : DockerMassTransitSetup
    {
        private const int Port = 5672;
        
        public RabbitMqSetup() 
            : base("rabbitmq", "3-management", Port)
        {
        }

        public override string HostName => "rabbitmq";
        
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