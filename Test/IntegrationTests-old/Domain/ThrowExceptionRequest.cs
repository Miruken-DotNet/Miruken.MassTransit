namespace IntegrationTests.Domain
{
    using Miruken.Api;

    public class ThrowExceptionRequest : IRequest<ThrowExceptionResponse>
    {
        public string Message { get; set; }
    }

    public class ThrowExceptionResponse
    {
    }
}