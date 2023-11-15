using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OVR_Dash_Manager.Functions
{
    public static class JsonFunctions
    {
        // Default settings for JSON serialization and deserialization
        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            MaxDepth = 99999,
            NullValueHandling = NullValueHandling.Ignore,
            Error = (s, e) => { e.ErrorContext.Handled = true; },
            Formatting = Formatting.None
        };

        // Method to serialize an object to a JSON string
        public static string SerializeClass(object Class, JsonSerializerSettings Settings = null)
        {
            var Serialized = string.Empty;

            Settings ??= JsonSettings;

            try
            {
                Serialized = JsonConvert.SerializeObject(Class, Settings);
            }
            catch (Exception)
            {
                // Handle exception if needed
            }

            return Serialized;
        }

        // Method to deserialize a JSON string to an object
        public static object DeserializeClass(string Data, Type DataType, JsonSerializerSettings Settings = null)
        {
            object Deserialized = null;

            Settings ??= JsonSettings;

            try
            {
                Deserialized = JsonConvert.DeserializeObject(Data, DataType, Settings);
            }
            catch (Exception)
            {
                // Handle exception if needed
            }

            return Deserialized;
        }

        // Generic method to deserialize a JSON string to a specific type of object
        public static T DeserializeClass<T>(string Data, JsonSerializerSettings Settings = null, params string[] IgnoreFields)
        {
            T Deserialized;

            Settings ??= JsonSettings;

            if (IgnoreFields.Length > 0)
            {
                var jsonResolver = new IgnorableSerializerContractResolver();
                jsonResolver.Ignore(typeof(T), IgnoreFields);
                var NewSettings = DeepCopySettings(Settings);
                NewSettings.ContractResolver = jsonResolver;
                Settings = NewSettings;  // Update Settings to use the new contract resolver
            }

            try
            {
                Deserialized = JsonConvert.DeserializeObject<T>(Data, Settings);
            }
            catch (Exception)
            {
                Deserialized = default;
            }

            return Deserialized;
        }

        // Method to create a deep copy of JsonSerializerSettings
        public static JsonSerializerSettings DeepCopySettings(JsonSerializerSettings serializer)
        {
            var copiedSerializer = new JsonSerializerSettings
            {
                Context = serializer.Context,
                Culture = serializer.Culture,
                ContractResolver = serializer.ContractResolver,
                Converters = serializer.Converters,
                ConstructorHandling = serializer.ConstructorHandling,
                CheckAdditionalContent = serializer.CheckAdditionalContent,
                DateFormatHandling = serializer.DateFormatHandling,
                DateFormatString = serializer.DateFormatString,
                DateParseHandling = serializer.DateParseHandling,
                DateTimeZoneHandling = serializer.DateTimeZoneHandling,
                DefaultValueHandling = serializer.DefaultValueHandling,
                EqualityComparer = serializer.EqualityComparer,
                FloatFormatHandling = serializer.FloatFormatHandling,
                Formatting = serializer.Formatting,
                FloatParseHandling = serializer.FloatParseHandling,
                MaxDepth = serializer.MaxDepth,
                MetadataPropertyHandling = serializer.MetadataPropertyHandling,
                MissingMemberHandling = serializer.MissingMemberHandling,
                NullValueHandling = serializer.NullValueHandling,
                ObjectCreationHandling = serializer.ObjectCreationHandling,
                PreserveReferencesHandling = serializer.PreserveReferencesHandling,
                ReferenceLoopHandling = serializer.ReferenceLoopHandling,
                StringEscapeHandling = serializer.StringEscapeHandling,
                TraceWriter = serializer.TraceWriter,
                TypeNameHandling = serializer.TypeNameHandling,
                SerializationBinder = serializer.SerializationBinder,
                TypeNameAssemblyFormatHandling = serializer.TypeNameAssemblyFormatHandling
            };

            return copiedSerializer;
        }
    }

    // Custom contract resolver to ignore specified properties during serialization/deserialization
    public class IgnorableSerializerContractResolver : DefaultContractResolver
    {
        protected readonly Dictionary<Type, HashSet<string>> Ignores;

        public IgnorableSerializerContractResolver()
        {
            Ignores = new Dictionary<Type, HashSet<string>>();
        }

        // Explicitly ignore the given property(s) for the given type
        public void Ignore(Type type, params string[] propertyName)
        {
            // start bucket if DNE
            if (!Ignores.ContainsKey(type)) Ignores[type] = new HashSet<string>();

            foreach (var prop in propertyName)
                Ignores[type].Add(prop);
        }

        // Is the given property for the given type ignored?
        public bool IsIgnored(Type type, string propertyName)
        {
            if (!Ignores.ContainsKey(type)) return false;

            // if no properties provided, ignore the type entirely
            if (Ignores[type].Count == 0) return true;

            return Ignores[type].Contains(propertyName);
        }

        // The decision logic goes here
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = instance => false;
            }

            return property;
        }
    }
}