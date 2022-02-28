namespace Miruken.MassTransit;

using System.Threading.Tasks;
using global::MassTransit;

public class CallbackConsumer<T> : ContextualConsumer<T>
    where T : class
{
    public override Task Consume(ConsumeContext<T> context)
    {
        throw new System.NotImplementedException();
    }
}