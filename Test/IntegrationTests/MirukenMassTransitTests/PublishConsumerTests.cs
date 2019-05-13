﻿using System;
using System.Threading.Tasks;
using IntegrationTests.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miruken.Mediate;
using Miruken.Mediate.Route;

namespace IntegrationTests.MirukenMassTransitTests
{
    [TestClass]
    public class PublishConsumerTests : TestScenario
    {
        [TestMethod]
        public async Task CanPublishAndRouteToMassTransit()
        {
            var handlerCounter         = DoSomethingHandler.Counter;
            var anotherHandlerCounter  = AnotherDoSomethingHandler.Counter;

            await appContext.Publish(new DoSomething()
                .RouteTo(routeString));

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(DoSomethingHandler.Counter > handlerCounter);
            Assert.IsTrue(AnotherDoSomethingHandler.Counter > anotherHandlerCounter);
        }
    }
}
