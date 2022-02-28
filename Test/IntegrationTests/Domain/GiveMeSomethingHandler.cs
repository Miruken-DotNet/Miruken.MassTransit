using System.Threading.Tasks;
using Miruken.Callback;

namespace IntegrationTests.Domain;

public class GiveMeSomethingHandler : Handler
{
    public static int Counter;

    [Handles]
    public Task<GiveMeSomethingResult> GiveMeSomething(GiveMeSomething request)
    {
        Counter++;
        return Task.FromResult(new GiveMeSomethingResult
        {
            Message = request.Message
        });
    }
}