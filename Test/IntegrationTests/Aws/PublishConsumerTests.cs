namespace IntegrationTests.Aws
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Api;
    using Miruken.Api.Route;

    [TestClass]
    public class PublishConsumerTests : PublishConsumerScenario
    {
        public PublishConsumerTests() : base(new LocalstackSetup())
        {
            
        }

        [TestMethod]
        public new async Task CanPublishAndRouteToMassTransit()
        {
            var handlerCounter        = DoSomethingHandler.Counter;
            var anotherHandlerCounter = AnotherDoSomethingHandler.Counter;

            await AppContext.Publish(
                new DoSomething { Message = "Everyone gets this" }
                    .RouteTo("mt:topic:miruken-topic"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(DoSomethingHandler.Counter > handlerCounter);
            Assert.IsTrue(AnotherDoSomethingHandler.Counter > anotherHandlerCounter);
        }
        
        [TestMethod]
        public async Task CanPublishAndRouteToMassTransitTopic()
        {
            var handlerCounter        = DoSomethingHandler.Counter;
            var anotherHandlerCounter = AnotherDoSomethingHandler.Counter;

            await AppContext.Send(
                new DoSomething { Message = "Everyone gets this" }
                    .RouteTo("mt:topic:miruken-topic"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(DoSomethingHandler.Counter > handlerCounter ||
                          AnotherDoSomethingHandler.Counter > anotherHandlerCounter);
            Assert.IsTrue(DoSomethingHandler.Counter == handlerCounter ||
                          AnotherDoSomethingHandler.Counter == anotherHandlerCounter);
        }
    }
}