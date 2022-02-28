using System.Threading.Tasks;
using MassTransit;

namespace IntegrationTests.Domain;

using Miruken.MassTransit;

public class QueueThisConsumer : ContextualConsumer<QueueThis>
{
    public static int Counter;

    public override Task Consume(ConsumeContext<QueueThis> context)
    {
        Counter++;
        return Task.CompletedTask;
    }
}