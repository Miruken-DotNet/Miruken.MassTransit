namespace Miruken.MassTransit.Api
{
    using System.Linq;
    using global::MassTransit;
    using Newtonsoft.Json;

    public static class ConfigurationExtensions
    {
        private static readonly JsonConverter Json = new MirukenJsonConverter();
        
        public static void UseMirukenJsonSerialization(this IBusFactoryConfigurator configurator)
        {
            configurator.ConfigureJsonSerializer(x =>
            {
                if (!x.Converters.OfType<MirukenJsonConverter>().Any())
                    x.Converters.Insert(0, Json);
                return x;
            });

            configurator.ConfigureJsonDeserializer(x =>
            {
                if (!x.Converters.OfType<MirukenJsonConverter>().Any())
                    x.Converters.Insert(0, Json);
                return x;
            });
        }
    }
}