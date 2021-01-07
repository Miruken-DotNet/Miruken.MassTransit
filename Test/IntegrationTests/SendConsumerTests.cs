namespace IntegrationTests
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Api;
    using Miruken.Api.Route;
    using Setup;

    [TestClass]
    public class SendConsumerTests : MassTransitScenario
    {
        public SendConsumerTests()
            : base(new RabbitMqSetup())
        {
        }
        
        [TestMethod]
        public async Task CanSendCommandAndRouteToMassTransit()
        {
            var handlerCounter         = DoSomethingHandler.Counter;
            var anotherHandlerCounter  = AnotherDoSomethingHandler.Counter;

            await AppContext.Send(new DoSomething()
                .RouteTo(RouteString));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(
                DoSomethingHandler.Counter        > handlerCounter ||
                AnotherDoSomethingHandler.Counter > anotherHandlerCounter
            );
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task WhenUriIsInvalid()
        {
            await AppContext.Send(new DoSomething()
                .RouteTo("mt"));
        }

        [TestMethod]
        public async Task CanSendRequestAndRouteToMassTransitAndGetResponse()
        {
            const string message = "I need something";
            var handlerCounter = GiveMeSomethingHandler.Counter;

            var result = await AppContext.Send(new GiveMeSomething { Message = message }
                .RouteTo(RouteString));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(GiveMeSomethingHandler.Counter > handlerCounter);
            Assert.AreEqual(message, result.Message);
        }

        [TestMethod]
        public async Task WhenRequestThrowsException()
        {
            try
            {
                await AppContext.Send(new ThrowExceptionRequest
                    {
                        Message = "I expect you to throw."
                    }
                    .RouteTo(RouteString));
                
                throw new Exception("Should never get here if test passes");
            }
            catch (TestException)
            {
                //Expected TestException was thrown
            }
        }
    }
}

