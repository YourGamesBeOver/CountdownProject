using System;
using System.Globalization;
using Windows.UI;
using Newtonsoft.Json;

namespace Countdown.Networking.Serialization {
    public class ColorJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var c = (Color) value;
            writer.WriteValue($"{c.R:X2}{c.G:X2}{c.B:X2}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var c = new Color();
            var val = reader.Value.ToString();
            c.R = byte.Parse(val.Substring(0, 2), NumberStyles.HexNumber);
            c.G = byte.Parse(val.Substring(2, 2), NumberStyles.HexNumber);
            c.B = byte.Parse(val.Substring(4, 2), NumberStyles.HexNumber);
            return c;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }
}
