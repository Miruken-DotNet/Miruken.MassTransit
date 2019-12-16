namespace Miruken.MassTransit
{
    using System.Threading.Tasks;
    using Api;
    using global::MassTransit;
    using Miruken.Api;

    public class SendConsumer : ContextualConsumer<Send>
    {
        public override async Task Consume(ConsumeContext<Send> context)
        {
            await Context.Send(context.Message.Payload);
        }
    }
}
