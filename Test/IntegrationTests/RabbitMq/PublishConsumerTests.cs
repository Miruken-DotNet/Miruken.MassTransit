namespace IntegrationTests.RabbitMq
{
    public class PublishConsumerTests : PublishConsumerScenario
    {
        public PublishConsumerTests() : base(new RabbitMqSetup())
        {
        }
    }
}