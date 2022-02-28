namespace Miruken.MassTransit.Api;

using Miruken.Api;

public record Request(object Payload) : IRequest<Response>;