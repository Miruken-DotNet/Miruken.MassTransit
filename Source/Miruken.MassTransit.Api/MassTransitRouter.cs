namespace Miruken.MassTransit.Api;

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
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    [Handles]
    public async Task<object> Route(Routed routed, Command command)
    {
        Uri endpointUri;
        try
        {
            var uri     = new Uri(routed.Route);
            endpointUri = new Uri(uri.PathAndQuery);
        }
        catch (UriFormatException)
        {
            return null;
        }

        if (routed.Message.GetType().IsClassOf(typeof(IRequest<>)))
        {
            var client = _bus.CreateRequestClient<Request>(endpointUri, TimeSpan.FromSeconds(30));
            var result = await client.GetResponse<Response, Failure>(new Request(routed.Message));
            if (result.Is(out Response<Response> response))
                return response.Message.Payload;

            if (result.Is(out Response<Failure> failure))
                throw failure.Message.Exception;

            throw new InvalidOperationException("Unable to determine the response.");
        }

        if (command.Many)
        {
            var publish = new Publish(routed.Message);
            if (endpointUri.Scheme == "topic")
            {
                var topicEndpoint = await _bus.GetSendEndpoint(endpointUri);
                await topicEndpoint.Send(publish);
            }
            else
            {
                await _bus.Publish(publish);
            }
            return null;
        }

        var endpoint = await _bus.GetSendEndpoint(endpointUri);
        await endpoint.Send(new Send(routed.Message));
        return null;
    }
}