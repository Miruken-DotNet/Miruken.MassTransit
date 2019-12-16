using System.Threading.Tasks;
using MassTransit;

namespace IntegrationTests.Domain
{
    public class QueueThisConsumer : IConsumer<QueueThis>
    {
        public static int Counter;
        public Task Consume(ConsumeContext<QueueThis> context)
        {
            Counter++;
            return Task.CompletedTask;
        }
    }
}