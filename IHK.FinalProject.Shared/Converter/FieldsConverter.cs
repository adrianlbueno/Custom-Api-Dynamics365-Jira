using IHK.FinalProject.Shared.Models.Jira;
using IHK.FinalProject.Shared.Models.Jiras;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Converter
{
    public class FieldsConverter : JsonConverter<Fields>
    {
        private readonly Dictionary<string, Type> _customFieldMapping = new Dictionary<string, Type>()
        {
            { "customfield_10500", typeof(string) },
            { "customfield_10900", typeof(double) }, //Aufwand in PT
            { "customfield_10902", typeof(DateTimeOffset) },
            { "customfield_10904", typeof(string) },
            { "customfield_10940", typeof(Customfield) },
            { "customfield_10941", typeof(Customfield) },
            { "customfield_10942", typeof(Customfield) },
            { "customfield_10943", typeof(int) },
            { "customfield_10944", typeof(int) },
            { "customfield_10945", typeof(int) },
            { "customfield_10946", typeof(int) },
            { "customfield_10930", typeof(List<string>) },
            { "customfield_10931", typeof(List<string>) },
            { "customfield_10932", typeof(List<string>) },
            { "customfield_10933", typeof(List<string>) },
            { "customfield_10934", typeof(List<string>) },
            { "customfield_10935", typeof(List<string>) },
            { "customfield_10938", typeof(Customfield) },
            { "customfield_10939", typeof(Customfield) },
            { "customfield_11100", typeof(int) },
            { "customfield_10005", typeof(string) },
            { "customfield_10920", typeof(Customfield) },
            { "customfield_10921", typeof(Customfield) },
            { "customfield_10922", typeof(Customfield) },
            { "customfield_10928", typeof(List<string>) },
            { "customfield_10929", typeof(List<string>) },
            { "customfield_10915", typeof(Customfield) },
            { "customfield_10916", typeof(Customfield) },
            { "customfield_10917", typeof(Customfield) },
            { "customfield_10918", typeof(Customfield) },
            { "customfield_10919", typeof(Customfield) }
        };

        public override Fields Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!JsonDocument.TryParseValue(ref reader, out JsonDocument jsonDocument))
            {
                return default;
            }

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new StringToDateTimeOffsetConverter());

            Fields fields = JsonSerializer.Deserialize<Fields>(jsonDocument.RootElement.ToString(), jsonSerializerOptions);

            foreach (JsonProperty jsonProperty in jsonDocument.RootElement.EnumerateObject())
            {
                if (!jsonProperty.Name.StartsWith("customfield_", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!this._customFieldMapping.ContainsKey(jsonProperty.Name))
                {
                    continue;
                }

                if (!jsonDocument.RootElement.TryGetProperty(jsonProperty.Name, out JsonElement element))
                {
                    continue;
                }

                this.AddCustomField(fields, jsonProperty, element.ValueKind, jsonSerializerOptions);
            }

            return fields;
        }

        public override void Write(Utf8JsonWriter writer, Fields value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private void AddCustomField(Fields fields, JsonProperty property, JsonValueKind valueKind, JsonSerializerOptions jsonSerializerOptions)
        {
            this._customFieldMapping.TryGetValue(property.Name, out Type type);

            switch (valueKind)
            {
                case JsonValueKind.Object:
                    object deserializedValue = JsonSerializer.Deserialize(property.Value.GetRawText(), type, jsonSerializerOptions);
                    fields.CustomFields.Add(property.Name.ToString(), deserializedValue);
                    break;
                case JsonValueKind.Array:
                    object deserializedArray = JsonSerializer.Deserialize(property.Value.GetRawText(), type, jsonSerializerOptions);
                    fields.CustomFields.Add(property.Name.ToString().ToLower(), deserializedArray);
                    break;
                case JsonValueKind.String:
                    if (type == typeof(DateTimeOffset))
                    {
                        fields.CustomFields.Add(property.Name, property.Value.GetDateTimeOffset());
                        break;
                    }

                    fields.CustomFields.Add(property.Name, property.Value.GetString());
                    break;
                case JsonValueKind.Number:
                    double doubleValue = property.Value.GetDouble();

                    if (type == typeof(int))
                    {
                        fields.CustomFields.Add(property.Name, Convert.ToInt32(doubleValue));
                        break;
                    }

                    fields.CustomFields.Add(property.Name, doubleValue);
                    break;
            }
        }
    }
}
