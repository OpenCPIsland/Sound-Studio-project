using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace JsonDotNet.Extras.CustomConverters
{
	public class Vector4Converter : JsonConverter
	{
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JToken jToken = JToken.FromObject(value);
			if (jToken.Type != JTokenType.Object)
			{
				jToken.WriteTo(writer);
				return;
			}
			JObject jObject = (JObject)jToken;
			IList<string> content = Enumerable.ToList<string>(Enumerable.Select<JProperty, string>(Enumerable.Where<JProperty>(jObject.Properties(), (Func<JProperty, bool>)((JProperty p) => p.Name == "w" || p.Name == "x" || p.Name == "y" || p.Name == "z")), (Func<JProperty, string>)((JProperty p) => p.Name)));
			jObject.AddFirst(new JProperty("Keys", new JArray(content)));
			jObject.WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Vector4);
		}
	}
}
