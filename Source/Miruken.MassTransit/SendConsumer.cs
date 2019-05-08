using System.Threading.Tasks;
using MassTransit;
using Miruken.Callback;
using Miruken.MassTransit.Api;
using Miruken.Mediate;

namespace Miruken.MassTransit
{
    public class SendConsumer : IConsumer<Send>
    {
        private readonly IHandler _handler;

        public SendConsumer(IHandler handler)
        {
            _handler = handler;
        }

        public async Task Consume(ConsumeContext<Send> context)
        {
            var result = await _handler.Send(context.Message.Payload);
        }
    }
}
