using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Miruken.MassTransit.Api
{
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
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                ContractResolver               = new CamelCasePropertyNamesContractResolver()
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
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                ContractResolver               = new CamelCasePropertyNamesContractResolver()
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