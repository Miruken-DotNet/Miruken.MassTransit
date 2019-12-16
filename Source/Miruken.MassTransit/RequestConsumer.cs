namespace Miruken.MassTransit
{
    using System;
    using System.Threading.Tasks;
    using global::MassTransit;
    using Miruken.Api;

    public class RequestConsumer : ContextualConsumer<Api.Request>
    {
        public override async Task Consume(ConsumeContext<Api.Request> context)
        {
            try
            {
                var result = await Context.Send(context.Message.Payload);
                await context.RespondAsync(new Api.Response(result));
            }
            catch (Exception e)
            {
                await context.RespondAsync(new Api.Response(e));
            }
        }
    }
}