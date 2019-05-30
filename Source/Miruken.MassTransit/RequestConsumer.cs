namespace Miruken.MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Callback;
    using global::MassTransit;
    using Miruken.Api;

    public class RequestConsumer : IConsumer<Api.Request>
    {
        private readonly IHandler _handler;

        public RequestConsumer(IHandler handler)
        {
            _handler = handler;
        }

        public async Task Consume(ConsumeContext<Api.Request> context)
        {
            try
            {
                var result = await _handler.Send(context.Message.Payload);
                await context.RespondAsync(new Api.Response(result));
            }
            catch (Exception e)
            {
                await context.RespondAsync(new Api.Response(e));
            }
        }
    }
}