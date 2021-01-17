namespace IntegrationTests.RabbitMq
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MassTransitSetupTests : MassTransitSetupScenario
    {
        public MassTransitSetupTests()
            : base(new RabbitMqSetup())
        {
        }
    }
}