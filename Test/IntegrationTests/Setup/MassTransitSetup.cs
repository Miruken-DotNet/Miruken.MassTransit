namespace IntegrationTests.Setup
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class MassTransitSetup : IAsyncDisposable
    {
        public abstract ValueTask Setup(
            ConfigurationBuilder configuration,
            IServiceCollection   services);

        public virtual string HostName => "localhost";
        
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }
}