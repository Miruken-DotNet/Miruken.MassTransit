namespace IntegrationTests.MirukenMassTransitTests
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Api;
    using Miruken.Api.Route;

    [TestClass]
    public class SendConsumerTests : TestScenario
    {
        [TestMethod]
        public async Task CanSendCommandAndRouteToMassTransit()
        {
            var handlerCounter         = DoSomethingHandler.Counter;
            var anotherHandlerCounter  = AnotherDoSomethingHandler.Counter;

            await appContext.Send(new DoSomething()
                .RouteTo(routeString));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(
                DoSomethingHandler.Counter        > handlerCounter ||
                AnotherDoSomethingHandler.Counter > anotherHandlerCounter
            );
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task WhenUriIsInvalid()
        {
            var handlerCounter         = DoSomethingHandler.Counter;
            var anotherHandlerCounter  = AnotherDoSomethingHandler.Counter;

            await appContext.Send(new DoSomething()
                .RouteTo("mt"));
        }

        [TestMethod]
        public async Task CanSendRequestAndRouteToMassTransitAndGetResponse()
        {
            var message        = "I need something";
            var handlerCounter = GiveMeSomethingHandler.Counter;

            var result = await appContext.Send(new GiveMeSomething { Message = message }
                .RouteTo(routeString));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(GiveMeSomethingHandler.Counter > handlerCounter);
            Assert.AreEqual(message, result.Message);
        }

        [TestMethod]
        public async Task WhenRequestThrowsException()
        {
            try
            {
                await appContext.Send(new ThrowExceptionRequest
                    {
                        Message = "I expect you to throw."
                    }
                    .RouteTo(routeString));
            }
            catch (TestException)
            {
                //Expected TestException was thrown
                return;
            }

            throw new Exception("Should never get here if test passes");
        }
    }
}
