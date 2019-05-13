using System;
using System.Threading.Tasks;
using MassTransit;
using Miruken.Callback;
using Miruken.MassTransit.Api;
using Miruken.Mediate;

namespace Miruken.MassTransit
{
    public class RequestConsumer : IConsumer<Request>
    {
        private readonly IHandler _handler;

        public RequestConsumer(IHandler handler)
        {
            _handler = handler;
        }

        public async Task Consume(ConsumeContext<Request> context)
        {
            try
            {
                var result = await _handler.Send(context.Message.Payload);
                await context.RespondAsync(new Response(result));
            }
            catch (Exception e)
            {
                await context.RespondAsync(new Response(e));
            }
        }
    }
}