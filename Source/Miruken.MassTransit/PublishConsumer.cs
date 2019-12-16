namespace Miruken.MassTransit
{
    using System.Threading.Tasks;
    using Api;
    using global::MassTransit;
    using Miruken.Api;

    public class PublishConsumer : ContextualConsumer<Publish>
    {
        public override async Task Consume(ConsumeContext<Publish> context)
        {
            await Context.Publish(context.Message.Payload);
        }
    }
}