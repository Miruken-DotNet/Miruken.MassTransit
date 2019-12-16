#if NETSTANDARD
namespace Miruken.MassTransit
{
    using System;
    using Api;
    using global::MassTransit;
    using global::MassTransit.ExtensionsDependencyInjectionIntegration;
    using global::MassTransit.ExtensionsDependencyInjectionIntegration.Registration;
    using Register;

    public static class RegistrationExtensions
    {
        public static Registration WithMassTransit(this Registration registration,
            Action<IServiceCollectionConfigurator> configure = null)
        {
            if (!registration.CanRegister(typeof(RegistrationExtensions)))
                return registration;

            registration.Services(services =>
            {
                var configurator = new ServiceCollectionConfigurator(services);
                configure?.Invoke(configurator);
            });
            
            return registration
                .Sources(sources => sources.FromAssemblyOf<RequestConsumer>(),
                         sources => sources.FromAssemblyOf<MassTransitRouter>())
                .Select((selector, publicOnly) =>
                    selector.AddClasses(x => x.AssignableTo<IConsumer>(), publicOnly)
                        .AsSelf().WithScopedLifetime());
        }
    }
}
#endif
