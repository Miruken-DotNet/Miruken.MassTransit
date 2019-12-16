namespace IntegrationTests
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Api;
    using Miruken.MassTransit.Api;

    [TestClass]
    public class TestScenarioTest : TestScenario
    {
        [TestMethod]
        public void MirukenCanHandle_DoSomething()
        {
            var handlerCounter         = DoSomethingHandler.Counter;
            var anotherHandlerCounter  = AnotherDoSomethingHandler.Counter;

            appContext.Send(new DoSomething());
            Assert.IsTrue(
                DoSomethingHandler.Counter        > handlerCounter ||
                AnotherDoSomethingHandler.Counter > anotherHandlerCounter
            );
        }

        [TestMethod]
        public async Task MassTransitCanConsume_QueueThis()
        {
            var counter  = QueueThisConsumer.Counter;
            var endpoint = await clientBus.GetSendEndpoint(queueUri);
            await endpoint.Send(new QueueThis());

            //Wonder what a better way might be to wait for the
            //message to be processed
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(QueueThisConsumer.Counter > counter);
        }

        [TestMethod]
        public async Task MassTransitCanConsume_Send()
        {
            var handlerCounter        = DoSomethingHandler.Counter;
            var anotherHandlerCounter = AnotherDoSomethingHandler.Counter;

            var endpoint = await clientBus.GetSendEndpoint(queueUri);
            await endpoint.Send(new Send(new DoSomething{ message = "go do something"}));

            //Wonder what a better way might be to wait for the
            //message to be processed
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(
                DoSomethingHandler.Counter        > handlerCounter ||
                AnotherDoSomethingHandler.Counter > anotherHandlerCounter
            );
        }

        [TestMethod, ExpectedException(typeof(UriFormatException))]
        public void ValidUris()
        {
            new Uri("mt");
        }
    }
}
