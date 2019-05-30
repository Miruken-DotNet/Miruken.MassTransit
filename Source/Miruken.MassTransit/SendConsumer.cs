namespace Miruken.MassTransit
{
    using System.Threading.Tasks;
    using Api;
    using Callback;
    using global::MassTransit;
    using Miruken.Api;

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
