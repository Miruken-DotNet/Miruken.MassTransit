namespace Miruken.MassTransit
{
    using System.Threading.Tasks;
    using Api;
    using Callback;
    using global::MassTransit;
    using Miruken.Api;

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