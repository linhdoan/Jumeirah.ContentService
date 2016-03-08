using EPiServer.Find.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms.Json
{
    public class FindUrlConverter : FindBaseJsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Url url = existingValue as Url;
            string str = (string)null;
            while (reader.Read())
            {
                if (reader.TokenType.Equals((object)JsonToken.PropertyName))
                    str = reader.Value as string;
                if (reader.TokenType.Equals((object)JsonToken.String) && str.Equals(TypeSuffix.GetSuffixedFieldName("OriginalString", typeof(string))))
                    return (object)new Url(reader.Value.ToString());
            }
            return (object)url;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Url).IsAssignableFrom(objectType);
        }
    }
}
