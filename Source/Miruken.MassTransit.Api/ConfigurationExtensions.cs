namespace Miruken.MassTransit.Api;

using System.Linq;
using global::MassTransit;
using Newtonsoft.Json;

public static class ConfigurationExtensions
{
    public static void UsePolymorphicJsonSerialization(this IBusFactoryConfigurator configurator)
    {
        var polymorphicJson = new PolymorphicJsonConverter();
        JsonSerializerSettings UsePolymorphicJson(JsonSerializerSettings settings)
        {
            if (!settings.Converters.OfType<PolymorphicJsonConverter>().Any())
                settings.Converters.Insert(0, polymorphicJson);
            return settings;
        }
            
        configurator.ConfigureJsonSerializer(UsePolymorphicJson);
        configurator.ConfigureJsonDeserializer(UsePolymorphicJson);
    }
}