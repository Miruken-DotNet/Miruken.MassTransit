namespace IntegrationTests.Aws;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SendConsumerTests : SendConsumerScenario
{
    public SendConsumerTests() : base(new LocalstackSetup())
    {
    }
}