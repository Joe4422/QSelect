using LibQuakePackageManager.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibQuakePackageManager.Databases
{
    class DependenciesToKeyListJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<string, IProviderItem>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.StartArray)
            {
                Dictionary<string, IProviderItem> output = new Dictionary<string, IProviderItem>();
                JToken token = JToken.Load(reader);

                List<string> items = token.ToObject<List<string>>();

                items.ForEach(x => output[x] = null);

                return output;
            }
            else
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is IDictionary<string, IProviderItem> dict)
            {
                JArray array = new JArray(dict.Keys);

                array.WriteTo(writer);
            }
            else
            {
                throw new ArgumentException("value was not IDictionary<string, IProviderItem>.");
            }
        }
    }
}
