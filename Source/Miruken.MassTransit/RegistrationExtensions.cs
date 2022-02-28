namespace Miruken.MassTransit;

using System;
using System.Collections.Generic;
using System.Linq;
using Api;
using Callback;
using Callback.Policy;
using Callback.Policy.Bindings;
using global::MassTransit;
using global::MassTransit.ExtensionsDependencyInjectionIntegration;
using Register;

public delegate Action<IServiceCollectionBusConfigurator>
    MassTransitConfigurationBuilder(IHandlerDescriptorFactory factory);
    
public static class RegistrationExtensions
{
    public static Registration WithMassTransit(
        this Registration               registration,
        MassTransitConfigurationBuilder builder = null)
    {
        if (!registration.CanRegister(typeof(RegistrationExtensions)))
            return registration;

        if (builder != null)
        {
            registration.AfterServices((services, _, factory) =>
                services.AddMassTransit(builder(factory)));
        }

        return registration
            .Sources(sources => sources.FromAssemblyOf<RequestConsumer>(),
                sources => sources.FromAssemblyOf<MassTransitRouter>())
            .Select((selector, publicOnly) =>
                selector.AddClasses(x => x.AssignableTo<IConsumer>(), publicOnly)
                    .AsSelf().WithScopedLifetime());
    }

    public static IEnumerable<IGrouping<string, Type>> AddBindings(
        this IRegistrationConfigurator  registration,
        IHandlerDescriptorFactory       handlerDescriptorFactory,
        Func<PolicyMemberBinding, bool> filter = null)
    {
        var bindings = handlerDescriptorFactory.GetPolicyMembers(Handles.Policy);
        if (filter != null)
            bindings = bindings.Where(filter);
        return registration.AddBindings(bindings);
    }
        
    public static IEnumerable<IGrouping<string, Type>> AddBindings(
        this IRegistrationConfigurator   registration,
        IEnumerable<PolicyMemberBinding> bindings) => 
        from  callbackType in bindings
            .Where(b => b.Dispatcher is not GenericMethodDispatch)
            .Select(b => b.Key as Type)
            .Where(callbackType => callbackType != null)
            .Distinct()
        let   consumer = AddConsumer(registration, callbackType)
        group consumer.Consumer by consumer.Group;

    private static (Type Consumer, string Group) AddConsumer(
        IRegistrationConfigurator registration,
        Type                      callbackType)
    {
        var consumer = typeof(CallbackConsumer<>).MakeGenericType(callbackType);
        registration.AddConsumer(consumer);
        return (consumer, "");
    }
}