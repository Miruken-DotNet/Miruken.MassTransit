namespace IntegrationTests.RabbitMq
{
    public class SendConsumerTests : SendConsumerScenario
    {
        public SendConsumerTests() : base(new RabbitMqSetup())
        {
        }
    }
}