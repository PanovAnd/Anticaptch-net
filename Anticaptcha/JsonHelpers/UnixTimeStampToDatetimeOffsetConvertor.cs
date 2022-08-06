using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Anticaptcha.JsonHelpers{
    internal class UnixTimeStampToDatetimeOffsetConvertor : JsonConverter<DateTimeOffset>{
        public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer){
            if (value < _unixTimeStartOfRange) throw new AggregateException();
            writer.WriteValue((value.ToUniversalTime() - _unixTimeStartOfRange).TotalSeconds);
        }

        private static readonly DateTimeOffset _unixTimeStartOfRange = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset existingValue, bool hasExistingValue, JsonSerializer serializer){
            if (objectType != typeof(DateTimeOffset)) throw new AggregateException();
            if (reader.ValueType != typeof(long)) throw new AggregateException();
            if (reader.Value is null) throw new AggregateException();
            if (!(reader.Value is long unixStamp)) throw new AggregateException();

            return _unixTimeStartOfRange.AddSeconds(unixStamp);
        }
    }
}