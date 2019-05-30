namespace Miruken.MassTransit.Api
{
    using Miruken.Api;

    public class Request : IRequest<Response>
    {
        public Request()
        {
        }

        public Request(object payload)
        {
            Payload = payload;
        }

        public object Payload { get; set; }
    }
}