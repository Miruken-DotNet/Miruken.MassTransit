namespace IntegrationTests.RabbitMq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SendConsumerTests : SendConsumerScenario
{
    public SendConsumerTests() : base(new RabbitMqSetup())
    {
    }
}