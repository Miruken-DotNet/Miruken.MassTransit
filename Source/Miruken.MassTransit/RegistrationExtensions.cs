namespace Miruken.MassTransit
{
    using System;
    using Api;
    using global::MassTransit;
    using global::MassTransit.ExtensionsDependencyInjectionIntegration;
    using Register;

    public static class RegistrationExtensions
    {
        public static Registration WithMassTransit(this Registration registration,
            Action<IServiceCollectionBusConfigurator> configure = null)
        {
            if (!registration.CanRegister(typeof(RegistrationExtensions)))
                return registration;

            if (configure != null)
                registration.Services(services => services.AddMassTransit(configure));

            return registration
                .Sources(sources => sources.FromAssemblyOf<RequestConsumer>(),
                         sources => sources.FromAssemblyOf<MassTransitRouter>())
                .Select((selector, publicOnly) =>
                    selector.AddClasses(x => x.AssignableTo<IConsumer>(), publicOnly)
                        .AsSelf().WithScopedLifetime());
        }
    }
}

