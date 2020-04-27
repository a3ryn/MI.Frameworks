using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candea.Frameworks.Tests
{

    internal class JsonInstanceConverter : JsonConverter
    {
        private static Dictionary<Type, Func<object>> ctors = new Dictionary<Type, Func<object>>
        {
            [typeof(TestConfig)] = () => new TestConfig()
        };

        public override bool CanWrite => false;

        protected object Create(Type objectType, JObject jObject)
        {
            //var objTypeId = jObject//.Value<int>("concreteAssetTypeId"); //<- avoiding to use hard coded string; also b/c actual prop name is with Caps
            //                    .GetValue(ObjectTypeIdKey, StringComparison.InvariantCultureIgnoreCase)
            //                    .ToObject<string>();

            //var t = Mapper.GetSpecializedType(objTypeId);
            //if (t == null) return Activator.CreateInstance(objectType); //create default if no specialized type specified

            //return objectType.IsAssignableFrom(t)
            //    ? Activator.CreateInstance(t) //specialized type instance
            //    : Activator.CreateInstance(objectType); //base type instance
            return Activator.CreateInstance(objectType);
                //ctors.ContainsKey(objectType)
                //? ctors[objectType]()
                //: Activator.CreateInstance(objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            //var canConvert = Mapper.SpecializedTypes.Any(objectType.IsAssignableFrom);
            //return canConvert;
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType,
          object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            // Load JObject from stream 
            var jObject = JObject.Load(reader);

            // Create target object based on JObject 
            var target = Create(objectType, jObject);

            // Populate the object properties 
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        /// <summary>
        /// Not used because we only have this converter for POST (and PUT eventually).
        /// This would be overridden if we had a custom serializer for GET.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value,
          JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
