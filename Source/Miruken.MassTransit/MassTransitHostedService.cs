#if NETSTANDARD
namespace Miruken.MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;
    using global::MassTransit;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public class MassTransitHostedService : IHostedService
    {
        private readonly IBusControl _bus;
        private readonly ILogger _logger;

        public MassTransitHostedService(IBusControl bus)
            : this(bus, NullLogger.Instance)
        {
        }

        public MassTransitHostedService(IBusControl bus, ILogger logger)
        {
            _bus    = bus;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting MassTransit Hosted Service");

            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping MassTransit Hosted Service");

            return _bus.StopAsync(cancellationToken);
        }
    }
}
#endif
