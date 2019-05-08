using System;
using System.Threading.Tasks;
using IntegrationTests.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miruken.Mediate;
using Miruken.Mediate.Route;

namespace IntegrationTests.MirukenMassTransitTests
{
    [TestClass]
    public class SendConsumerTests : TestScenario
    {
        [TestMethod]
        public async Task CanSendCommandAndRouteToMassTransit()
        {
            var handlerCounter         = DoSomethingHandler.Counter;
            var anotherHandlerCounter  = AnotherDoSomethingHandler.Counter;

            var uri = queueUri.ToString();
            await handler.Send(new DoSomething()
                .RouteTo(uri));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(
                DoSomethingHandler.Counter        > handlerCounter ||
                AnotherDoSomethingHandler.Counter > anotherHandlerCounter
            );
        }

        [TestMethod]
        public async Task CanSendRequestAndRouteToMassTransitAndGetResponse()
        {
            var message        = "I need something";
            var handlerCounter = GiveMeSomethingHandler.Counter;

            var uri = queueUri.ToString();
            var result = await handler.Send(new GiveMeSomething { Message = message }
                .RouteTo(uri));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(GiveMeSomethingHandler.Counter > handlerCounter);
            Assert.AreEqual(message, result.Message);
        }
    }
}
