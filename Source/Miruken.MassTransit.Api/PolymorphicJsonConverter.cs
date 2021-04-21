namespace Miruken.MassTransit.Api
{
    using System;
    using Newtonsoft.Json;

   public class PolymorphicJsonConverter : JsonConverter
    {
        public override void WriteJson(
            JsonWriter     writer, 
            object         value, 
            JsonSerializer serializer)
        {
           Copy(serializer).Serialize(writer, value);
        }

        public override object ReadJson(
            JsonReader     reader,
            Type           objectType,
            object         existingValue,
            JsonSerializer serializer)
        {
            return Copy(serializer).Deserialize(reader, objectType);
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
        
        private static JsonSerializer Copy(JsonSerializer serializer)
        {
            var copy = new JsonSerializer
            {
                Context                        = serializer.Context,
                Culture                        = serializer.Culture,
                ContractResolver               = serializer.ContractResolver,
                ConstructorHandling            = serializer.ConstructorHandling,
                CheckAdditionalContent         = serializer.CheckAdditionalContent,
                DateFormatHandling             = serializer.DateFormatHandling,
                DateFormatString               = serializer.DateFormatString,
                DateParseHandling              = serializer.DateParseHandling,
                DateTimeZoneHandling           = serializer.DateTimeZoneHandling,
                DefaultValueHandling           = serializer.DefaultValueHandling,
                EqualityComparer               = serializer.EqualityComparer,
                FloatFormatHandling            = serializer.FloatFormatHandling,
                Formatting                     = serializer.Formatting,
                FloatParseHandling             = serializer.FloatParseHandling,
                MaxDepth                       = serializer.MaxDepth,
                MetadataPropertyHandling       = serializer.MetadataPropertyHandling,
                MissingMemberHandling          = serializer.MissingMemberHandling,
                NullValueHandling              = NullValueHandling.Ignore,
                ObjectCreationHandling         = serializer.ObjectCreationHandling,
                PreserveReferencesHandling     = serializer.PreserveReferencesHandling,
                ReferenceResolver              = serializer.ReferenceResolver,
                ReferenceLoopHandling          = serializer.ReferenceLoopHandling,
                SerializationBinder            = serializer.SerializationBinder,
                StringEscapeHandling           = serializer.StringEscapeHandling,
                TraceWriter                    = serializer.TraceWriter,
                TypeNameHandling               = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
            
            foreach (var converter in serializer.Converters)
            {
                if (converter is not PolymorphicJsonConverter)
                    copy.Converters.Add(converter);
            }
            
            return copy;
        }
    }
}