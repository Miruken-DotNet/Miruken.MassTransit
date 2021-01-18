namespace IntegrationTests.Aws
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RabbitMq;

    [TestClass]
    public class SendConsumerTests : SendConsumerScenario
    {
        public SendConsumerTests() : base(new RabbitMqSetup())
        {
        }
    }
}