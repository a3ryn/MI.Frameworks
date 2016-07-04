﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shared.Core.Common.Serialization
{
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
            //TODO - perhaps read from some configuration file
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All,
        };

        public static string ToJson<T>(this T r) =>
            JsonConvert.SerializeObject(r, SerializerSettings);

        public static T FromJson<T>(this string s) =>
            JsonConvert.DeserializeObject<T>(s, SerializerSettings);

        #endregion
    }
}
