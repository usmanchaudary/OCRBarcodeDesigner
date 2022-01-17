using System;
using System.Collections.Generic;
using System.Text;

namespace OCRBarcodeDesigner
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class OcrResult
    {
        [JsonProperty("ParsedResults")]
        public List<ParsedResult> ParsedResults { get; set; }

        [JsonProperty("OCRExitCode")]
        public long OcrExitCode { get; set; }

        [JsonProperty("IsErroredOnProcessing")]
        public bool IsErroredOnProcessing { get; set; }

        [JsonProperty("ProcessingTimeInMilliseconds")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ProcessingTimeInMilliseconds { get; set; }

        [JsonProperty("SearchablePDFURL")]
        public string SearchablePdfurl { get; set; }
    }

    public partial class ParsedResult
    {
        [JsonProperty("TextOverlay")]
        public TextOverlay TextOverlay { get; set; }

        [JsonProperty("TextOrientation")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TextOrientation { get; set; }

        [JsonProperty("FileParseExitCode")]
        public long FileParseExitCode { get; set; }

        [JsonProperty("ParsedText")]
        public string ParsedText { get; set; }

        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("ErrorDetails")]
        public string ErrorDetails { get; set; }
    }

    public partial class TextOverlay
    {
        [JsonProperty("Lines")]
        public List<object> Lines { get; set; }

        [JsonProperty("HasOverlay")]
        public bool HasOverlay { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }
    }

    public partial class OcrResult
    {
        public static OcrResult FromJson(string json) => JsonConvert.DeserializeObject<OcrResult>(json, OCRBarcodeDesigner.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
