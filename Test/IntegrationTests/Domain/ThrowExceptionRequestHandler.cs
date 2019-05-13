using System;
using System.Threading.Tasks;
using Miruken.Callback;

namespace IntegrationTests.Domain
{
    public class ThrowExceptionRequestHandler : Handler
    {
        public static int Counter;

        [Handles]
        public Task ThrowExceptionRequest(ThrowExceptionRequest request)
        {
            Counter++;
            throw new TestException(request.Message);
        }
    }
}