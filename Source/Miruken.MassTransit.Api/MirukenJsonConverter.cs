namespace Miruken.MassTransit.Api
{
    using System;
    using Newtonsoft.Json;

    public class MirukenJsonConverter : JsonConverter
    {
        public override void WriteJson(
            JsonWriter     writer, 
            object         value, 
            JsonSerializer serializer)
        {
            new JsonSerializer
            {
                NullValueHandling              = NullValueHandling.Ignore,
                TypeNameHandling               = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            }
            .Serialize(writer, value);
        }

        public override object ReadJson(
            JsonReader     reader,
            Type           objectType,
            object         existingValue,
            JsonSerializer serializer)
        {
            return new JsonSerializer
            {
                NullValueHandling              = NullValueHandling.Ignore,
                TypeNameHandling               = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            }.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return !IsMassTransitOrSystemType(objectType);
        }

        private static bool IsMassTransitOrSystemType(Type objectType)
        {
            return objectType.Assembly == typeof(global::MassTransit.IConsumer).Assembly ||
                   objectType.Assembly.IsDynamic ||
                   objectType.Assembly == typeof(object).Assembly;
        }
    }
}