using System.Threading.Tasks;
using MassTransit;
using Miruken.Callback;
using Miruken.MassTransit.Api;
using Miruken.Mediate;

namespace Miruken.MassTransit
{
    public class PublishConsumer : IConsumer<Publish>
    {
        private readonly IHandler _handler;

        public PublishConsumer(IHandler handler)
        {
            _handler = handler;
        }

        public async Task Consume(ConsumeContext<Publish> context)
        {
            await _handler.Publish(context.Message.Payload);
        }
    }
}