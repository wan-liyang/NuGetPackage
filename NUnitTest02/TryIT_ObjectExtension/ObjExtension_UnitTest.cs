//using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using TryIT.ObjectExtension;

namespace NUnitTest02.TryIT_ObjectExtension
{
    class ObjExtension_UnitTest
    {
        [Test]
        public void GetJsonValue()
        {
            string json = "";
            var result = json.GetJsonValue<string>("a");
            Assert.IsNull(result);

            json = "a";
            result = json.GetJsonValue<string>("");
            Assert.IsNull(result);

            json = "{'key1':'value1','key2':'value2','key3':'value3'}";
            result = json.GetJsonValue<string>("key1");
            Assert.That(result, Is.EqualTo("value1"));

            result = json.GetJsonValue<string>("key2");
            Assert.That(result, Is.EqualTo("value2"));

            result = json.GetJsonValue<string>("key3");
            Assert.That(result, Is.EqualTo("value3"));


            json = "{'key1':'value1','key2':[{'key1':'value2-1','key2-2':'value2','key3':123},{'key4':'value2-4','key5':'value2-5','key6':456}],'key3':'value3'}";
            result = json.GetJsonValue<string>("key2[0]:key1");
            Assert.That(result, Is.EqualTo("value2-1"));

            result = json.GetJsonValue<string>("key2[1]:key4");
            Assert.That(result, Is.EqualTo("value2-4"));

            int result_int = json.GetJsonValue<int>("key2[0]:key3");
            Assert.That(result_int, Is.EqualTo(123));

            result_int = json.GetJsonValue<int>("key2[0]:key3");
            Assert.That(result_int, Is.EqualTo(123));

            result = json.GetJsonValue<string>("key2[0]:key4");
            Assert.That(result, Is.EqualTo(null));



            // ignore case
            result_int = json.GetJsonValue<int>("KEY[0]:Key3");
            Assert.That(result_int, Is.EqualTo(0));
            result_int = json.GetJsonValue<int>("KEY2[0]:Key3");
            Assert.That(result_int, Is.EqualTo(123));
        }

        [Test]
        public void GetJsonValue2()
        {
            string json = @"{
    ""Data"": {
        ""StatusCode"": ""400"",
        ""Message"": [
            {
                ""Status"": ""False"",
                ""Description"": ""Invalid Data. ""
            }
        ]
    },
    ""Data2"": ""other data""
}";

            var result = json.GetJsonValue<string>("data");

            Assert.IsTrue(result.Contains("Status"));
        }


        [Test]
        public void CustomizeFormatTest()
        {
            string json = @"{
  ""status"": ""Healthy"",
  ""duration"": ""00:00:04.004"",
  ""timestamp"": ""2025-08-22 12:17:14.000 +0000"",
}";

            var obj = json.JsonToObject<Response>();

            Assert.That(obj.timestamp.Hour, Is.EqualTo(20));
        }


        class ListItem
        {
            public string Title { get; set; }
            public string RequestBody { get; set; }
            public string ResponseBody { get; set; }
        }

        class Response
        {
            public string status { get; set; }
            public string duration { get; set; }

            //[JsonConverter(typeof(CustomDateTimeConverter))]
            public DateTime timestamp { get; set; }
        }
    }


    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("Non-nullable DateTime cannot be null");

            string? input = reader.GetString();
            if (string.IsNullOrWhiteSpace(input))
                throw new JsonException("Invalid DateTime string");

            string normalized = Regex.Replace(input, @"([+-]\d{2})(\d{2})$", "$1:$2");
            return DateTimeOffset.Parse(normalized, CultureInfo.InvariantCulture).LocalDateTime;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("o"));
        }
    }
}
