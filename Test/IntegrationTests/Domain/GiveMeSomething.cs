namespace IntegrationTests.Domain;

using Miruken.Api;

public class GiveMeSomething : IRequest<GiveMeSomethingResult>
{
    public string Message { get; set; }
}

public class GiveMeSomethingResult
{
    public string Message { get; set; }
}