using System.Threading.Tasks;
using Miruken.Callback;

namespace IntegrationTests.Domain
{
    public class DoSomethingHandler : Handler
    {
        public static int Counter;

        [Handles]
        public Task DoSomething(DoSomething request)
        {
            Counter++;
            return Task.CompletedTask;
        }
    }

    public class AnotherDoSomethingHandler : Handler
    {
        public static int Counter;

        [Handles]
        public Task DoSomething(DoSomething request)
        {
            Counter++;
            return Task.CompletedTask;
        }
    }
}