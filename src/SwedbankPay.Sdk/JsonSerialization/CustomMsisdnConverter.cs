﻿using System;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SwedbankPay.Sdk.JsonSerialization
{
    public class CustomMsisdnConverter : JsonConverter
    {
        private readonly Type[] types;


        public CustomMsisdnConverter(params Type[] types)
        {
            this.types = types;
        }


        public override bool CanRead => true;


        public override bool CanConvert(Type objectType)
        {
            return this.types.Any(t => t == objectType);
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var jo = JObject.Load(reader);
                var msisdn = jo.Values().FirstOrDefault()?.ToString();
                return new Msisdn(msisdn);
            }

            return new Msisdn(reader.Value.ToString());
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object)
                t.WriteTo(writer);
            else
                writer.WriteValue(value.ToString());
        }
    }
}