using Miruken.Mediate.Api;

namespace Miruken.MassTransit.Api
{
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