namespace IntegrationTests
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Api;
    using Miruken.Api.Route;
    using Setup;
    
    public abstract class PublishConsumerScenario : MassTransitScenario
    {
        protected PublishConsumerScenario(MassTransitSetup setup)
            : base(setup)
        {
        }
        
        [TestMethod]
        public async Task CanPublishAndRouteToMassTransit()
        {
            var handlerCounter         = DoSomethingHandler.Counter;
            var anotherHandlerCounter  = AnotherDoSomethingHandler.Counter;

            await AppContext.Publish(new DoSomething()
                .RouteTo(RouteString));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(DoSomethingHandler.Counter > handlerCounter);
            Assert.IsTrue(AnotherDoSomethingHandler.Counter > anotherHandlerCounter);
        }
    }
}

