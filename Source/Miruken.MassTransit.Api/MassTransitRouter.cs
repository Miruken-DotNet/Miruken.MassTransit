using System;
using System.Threading.Tasks;
using MassTransit;
using Miruken.Callback;
using Miruken.Infrastructure;
using Miruken.Mediate.Api;
using Miruken.Mediate.Route;

namespace Miruken.MassTransit.Api
{
    [Routes("rabbitmq")]
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
            var uri = new Uri(routed.Route);
            if (routed.Message.GetType().IsClassOf(typeof(IRequest<>)))
            {
                try
                {
                    var client = _bus.CreateRequestClient<Request, Response>(uri, TimeSpan.FromSeconds(30));
                    var result = await client.Request(new Request(routed.Message));
                    return result.Payload;
                }
                catch (Exception e)
                {
                    var a = 1;
                    throw;
                }
            }

            if (command.Many)
            {
                await _bus.Publish(new Publish(routed.Message));
                return null;
            }

            var endpoint = await _bus.GetSendEndpoint(uri);
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
