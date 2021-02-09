using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Frags.Core.Statistics;

namespace Frags.Database
{
    public class StatisticValueJsonConverter : JsonConverter<StatisticValue>
    {
        public override StatisticValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string input = reader.GetString();
            string[] arr = input.Split(',');

            if (arr.Length != 3)
                throw new JsonException("Array length was not 3.");

            int rank = int.Parse(arr[0]);
            bool isprof = bool.Parse(arr[1]);
            double prof = double.Parse(arr[2]);

            return new StatisticValue(rank, isprof, prof);
        }

        public override void Write(Utf8JsonWriter writer, StatisticValue value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}