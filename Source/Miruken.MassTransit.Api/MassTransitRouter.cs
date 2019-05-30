namespace Miruken.MassTransit.Api
{
    using System;
    using System.Threading.Tasks;
    using Callback;
    using global::MassTransit;
    using Infrastructure;
    using Miruken.Api;
    using Miruken.Api.Route;

    [Routes("mt")]
    public class MassTransitRouter : Handler
    {
        private readonly IBus _bus;

        public MassTransitRouter(IBus bus)
        {
            _bus = bus;
        }

        [Handles]
        public async Task<object> Route(Routed routed, Command command, IHandler composer)
        {
            Uri endpointUri;
            try
            {
                var uri     = new Uri(routed.Route);
                endpointUri = new Uri(uri.PathAndQuery);
            } catch (UriFormatException)
            {
                return null;
            }

            if (routed.Message.GetType().IsClassOf(typeof(IRequest<>)))
            {
                var client = _bus.CreateRequestClient<Request, Response>(endpointUri, TimeSpan.FromSeconds(30));
                var result = await client.Request(new Request(routed.Message));
                return EnsureSuccessfulResult(result);
            }

            if (command.Many)
            {
                await _bus.Publish(new Publish(routed.Message));
                return null;
            }

            var endpoint = await _bus.GetSendEndpoint(endpointUri);
            await endpoint.Send(new Send(routed.Message));
            return null;
        }

        public object EnsureSuccessfulResult(Response response)
        {
            if (response.Exception != null)
                throw response.Exception;
            return response.Payload;
        }
    }
}
