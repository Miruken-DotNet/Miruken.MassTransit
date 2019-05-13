using Miruken.Mediate.Api;

namespace IntegrationTests.Domain
{
    public class ThrowExceptionRequest : IRequest<ThrowExceptionResponse>
    {
        public string Message { get; set; }
    }

    public class ThrowExceptionResponse
    {
    }
}