using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IHK.FinalProject.Shared.Converter
{
    /// <summary>
    /// Wrote by René Duesmann
    /// </summary>
    public class StringToDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        #region Properties

        /// <summary>
        /// Formats that can be converted.
        /// </summary>
        public string[] Formats { get; } = new[]
        {
            "yyyy-MM-dd HH:mm:ss"
        };

        #endregion //Properties

        /// <summary>
        /// Read the given value and try to convert it to a <see cref="DateTimeOffset"/> value.
        /// </summary>
        /// <param name="reader">Reader that contains the value.</param>
        /// <param name="typeToConvert">Source type.</param>
        /// <param name="options">Optional serialzation options.</param>
        /// <returns>Converted value.</returns>
        public override DateTimeOffset Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString().ToLower().Replace("utc", string.Empty);

            if (DateTimeOffset.TryParse(value, out DateTimeOffset result))
            {
                return result;
            }

            IFormatProvider provider = CultureInfo.InvariantCulture.DateTimeFormat;

            return DateTimeOffset.TryParseExact(value, this.Formats, provider,
                DateTimeStyles.AllowWhiteSpaces, out result)
                ? result
                : default;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
