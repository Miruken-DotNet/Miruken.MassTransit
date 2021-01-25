namespace IntegrationTests.Aws
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using MassTransit;
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
        public async Task CanSendAndRouteToMassTransitTopic()
        {
            var handlerCounter        = DoSomethingHandler.Counter;
            var anotherHandlerCounter = AnotherDoSomethingHandler.Counter;

            await AppContext.Send(
                new DoSomething { Message = "Some gets this" }
                    .RouteTo("mt:topic:miruken-topic"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(DoSomethingHandler.Counter > handlerCounter ||
                          AnotherDoSomethingHandler.Counter > anotherHandlerCounter);
            Assert.IsTrue(DoSomethingHandler.Counter == handlerCounter ||
                          AnotherDoSomethingHandler.Counter == anotherHandlerCounter);
        }
        
        [TestMethod]
        public async Task CanSendToMassTransitQueueDirectly()
        {
            var counter  = QueueThisConsumer.Counter;
            var endpoint = await ClientBus.GetSendEndpoint(new Uri("queue:miruken_masstransit_integration_tests"));
            await endpoint.Send(new QueueThis());
            
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(QueueThisConsumer.Counter > counter);
        }
        
        [TestMethod]
        public async Task CanSendToMassTransitTopicDirectly()
        {
            var counter  = QueueThisConsumer.Counter;
            var endpoint = await ClientBus.GetSendEndpoint(new Uri("topic:miruken-topic"));
            await endpoint.Send(new QueueThis());
            
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(QueueThisConsumer.Counter > counter);
        }
    }
}