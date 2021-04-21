namespace Miruken.MassTransit
{
    using System;
    using global::MassTransit;

    public static class ReceiveEndpointExtensions
    {
        public static IReceiveEndpointConfigurator AddMirukenConsumers(
            this IReceiveEndpointConfigurator configurator,
            IServiceProvider                  provider)
        {
            configurator.Consumer<SendConsumer>(provider);
            configurator.Consumer<PublishConsumer>(provider);
            configurator.Consumer<RequestConsumer>(provider);
            return configurator;
        }
    }
}