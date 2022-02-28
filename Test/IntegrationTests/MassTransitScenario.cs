namespace IntegrationTests;

using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miruken.Callback;
using Miruken.Context;
using Miruken.MassTransit;
using Miruken.Register;
using Setup;

public abstract class MassTransitScenario
{
    private readonly MassTransitSetup _massTransitSetup;
    private IBusControl   _bus;

    protected Context     AppContext;
    protected IBusControl ClientBus;
    protected Uri         QueueUri;
    protected string      RouteString;

    private const string QueueName = "miruken_masstransit_integration_tests";

    protected MassTransitScenario(MassTransitSetup massTransitSetup)
    {
        _massTransitSetup = massTransitSetup
                            ?? throw new ArgumentNullException(nameof(massTransitSetup));
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        var services             = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder();

        await _massTransitSetup.Setup(configurationBuilder, services);
        var configuration = configurationBuilder.Build();
            
        QueueUri    = _massTransitSetup.CreateQueueUri(QueueName);
        RouteString = $"mt:{QueueUri}";

        AppContext = services
            .AddSingleton<IConfiguration>(configuration)
            .AddMiruken(configure => configure
                .PublicSources(s => s.FromAssemblyOf<MassTransitScenario>())
                .WithMassTransit(_massTransitSetup.Configure(QueueName))
            ).Build();

        _bus = AppContext.Resolve<IBusControl>();
        await _bus.StartAsync();

        ClientBus = _massTransitSetup.CreateClientBus();
        await ClientBus.StartAsync();
    }
        
    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            _bus?.Stop();
            ClientBus?.Stop();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
            
        try
        {
            await _massTransitSetup.DisposeAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        AppContext?.End();
    }
}