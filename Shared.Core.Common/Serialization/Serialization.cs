using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shared.Core.Common.Serialization
{
    using static auxfunc;

    public class StringWriterWithEncoding : StringWriter
    {
        public StringWriterWithEncoding(Encoding e)
        {
            Encoding = e ?? Encoding.UTF8;
        }

        public override Encoding Encoding { get; }
    }

    public static class SerializationExt
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> XmlSerializers =
            new ConcurrentDictionary<Type, XmlSerializer>();

        public static string Serialize<T>(this T obj, Encoding encoding = null)
        {
            var t = typeof (T);
            var ser = GetSerializer<T>(t);

            using (var stream = new StringWriterWithEncoding(encoding))
            {
                ser.Serialize(stream, obj);
                stream.Flush();
                return stream.ToString();
            }
        }

        private static XmlSerializer GetSerializer<T>(Type t)
        {
            XmlSerializer ser;
            if (XmlSerializers.ContainsKey(t))
                ser = XmlSerializers[t];
            else
            {
                ser = new XmlSerializer(t);
                XmlSerializers.TryAdd(t, ser);
            }
            return ser;
        }

        public static T Deserialize<T>(this string xmlData) where T : new()
        {
            var t = typeof(T);
            var ser = GetSerializer<T>(t);
            if (ser == null) return default(T);

            using (var reader = XmlReader.Create(new StringReader(xmlData)))
            {
                return (T) ser.Deserialize(reader);
            }
        }

        public static void WriteToFile(this string data, string path)
        {
            File.WriteAllText(path, data);
        }

        #region  JSON Serialization
        private static JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = appSetting("Json.IncludeTypeMetadata", true) 
                                ? TypeNameHandling.All : TypeNameHandling.None,
        };

        public static string ToJson<T>(this T r, JsonSerializerSettings settings = null) =>
            JsonConvert.SerializeObject(r, settings ?? SerializerSettings);

        public static T FromJson<T>(this string s, JsonSerializerSettings settings = null) =>
            JsonConvert.DeserializeObject<T>(s, settings ?? SerializerSettings);

        /// <summary>
        /// Deserializes a JSON string to an object, given a custom converter
        /// </summary>
        /// <typeparam name="T">The type of the object to be hydrated (can be an interface as well)</typeparam>
        /// <param name="s">The serialized data (json string)</param>
        /// <param name="c">The custom converter.</param>
        /// <returns>An instance of the object reconstructed from the JSON string.</returns>
        public static T FromJson<T>(this string s, JsonConverter c) =>
            JsonConvert.DeserializeObject<T>(s,c);

        #endregion
    }
}
