namespace IntegrationTests.RabbitMq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PublishConsumerTests : PublishConsumerScenario
{
    public PublishConsumerTests() : base(new RabbitMqSetup())
    {
    }
}