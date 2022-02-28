namespace Miruken.MassTransit;

using System;
using System.Collections.Generic;
using System.Linq;
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

    public static IReceiveConfigurator<T> CreateReceiverEndpoints<T>(
        this IReceiveConfigurator<T>         configurator,
        IRegistration                        registration,
        string                               baseQueueName,
        IEnumerable<IGrouping<string, Type>> consumers,
        Action<string, T>                    configureEndpoint = null)
        where T : IReceiveEndpointConfigurator
    {
        foreach (var consumerGroup in consumers)
        {
            var group = consumerGroup.Key;
            var queueName = group.Length > 0
                ? $"{baseQueueName}-{group}"
                : baseQueueName;
                
            configurator.ReceiveEndpoint(NormalizeQueueName(queueName), ep =>
            {
                foreach (var commandConsumer in consumerGroup)
                    ep.ConfigureConsumer(registration, commandConsumer);
                    
                configureEndpoint?.Invoke(group, ep);
            });
        }
        return configurator;
    }
        
    private static string NormalizeQueueName(string queueName) =>
        queueName.Replace(".", "_").Replace(" ", "_");
}