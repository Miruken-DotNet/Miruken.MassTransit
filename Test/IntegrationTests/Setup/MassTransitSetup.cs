namespace IntegrationTests.Setup
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using MassTransit.Registration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class MassTransitSetup : IAsyncDisposable
    {
        public virtual string HostName => "localhost";

        public abstract Uri CreateQueueUri(string queueName);

        public abstract ValueTask Setup(
            ConfigurationBuilder configuration, IServiceCollection services);
        
        public abstract Action<IServiceCollectionBusConfigurator> Configure(string queueName);
        
        public abstract IBusControl CreateClientBus();

        public virtual ValueTask DisposeAsync() => new ValueTask();
    }
}