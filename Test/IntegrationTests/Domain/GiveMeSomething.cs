using Miruken.Mediate.Api;

namespace IntegrationTests.Domain
{
    public class GiveMeSomething : IRequest<GiveMeSomethingResult>
    {
        public string Message { get; set; }
    }

    public class GiveMeSomethingResult
    {
        public string Message { get; set; }
    }
}