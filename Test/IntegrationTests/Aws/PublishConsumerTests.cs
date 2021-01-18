namespace IntegrationTests.Aws
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RabbitMq;

    [TestClass]
    public class PublishConsumerTests : PublishConsumerScenario
    {
        public PublishConsumerTests() : base(new RabbitMqSetup())
        {
        }
    }
}