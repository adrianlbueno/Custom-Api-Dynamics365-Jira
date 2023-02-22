using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace System
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static T ParseXML<T>(this string @this) where T : class
        {
            var reader = XmlReader.Create(@this.Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }

        public static List<IFormatProvider> SupportedFormats = new List<IFormatProvider>
        {
            CultureInfo.CreateSpecificCulture("en-US"),
            CultureInfo.CreateSpecificCulture("de-DE")
        };

        public static IFormatProvider DefaultFormat = SupportedFormats[0];

        public static bool EqualsAny(this string source, StringComparison comp = StringComparison.CurrentCulture, params string[] toCheck)
        {
            if (String.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source), $"{nameof(source)} must not be empty.");

            return toCheck.Any(v => source.Equals(v, comp));
        }

        public static string Replace(this String source, Dictionary<String, String> replacements, StringComparison stringComparison = StringComparison.Ordinal)
        {
            string working = String.Copy(source);
            foreach (var replacement in replacements)
            {
                working = working.Replace(replacement.Key, replacement.Value, stringComparison);
            }
            return working;
        }

        public static string Replace(this String source, string oldValue, string newValue, StringComparison stringComparison)
        {
            string working = String.Copy(source);
            int index = working.IndexOf(oldValue, stringComparison);
            while (index != -1)
            {
                working = working.Remove(index, oldValue.Length);
                working = working.Insert(index, newValue);
                index = index + newValue.Length;
                index = working.IndexOf(oldValue, index, stringComparison);
            }
            return working;
        }

        /// <summary>
        /// Checks if a string contains another with respecting the passed stringcomparison
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        ///  Checks if a string contains another with ignoring the case
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            return source.Contains(toCheck, StringComparison.OrdinalIgnoreCase);
        }

        public enum SubStringOptions
        {
            FromFirst,
            FromLast,
            ToFirst,
            ToLast
        }

        public static string FirstCharToUpper(this String input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static String Trim(this String source, int length)
        {
            return source?.Substring(0, Math.Min(source.Length, length > 0 ? length : 0));
        }

        public static String TrimFromStart(this String source, int length)
        {
            if (source.Length <= length) return source;

            return source?.Substring(source.Length - length, length);
        }

        public static String Substring(this String source, String from, SubStringOptions options = SubStringOptions.ToFirst)
        {
            if (!String.IsNullOrWhiteSpace(source) && !String.IsNullOrWhiteSpace(from) && source.Contains(from))
            {
                switch (options)
                {
                    case SubStringOptions.FromFirst:
                        return source.Substring(source.IndexOf(from) + from.Length);
                    case SubStringOptions.FromLast:
                        return source.Substring(source.LastIndexOf(from) + from.Length);
                    case SubStringOptions.ToFirst:
                        return source.Substring(0, source.IndexOf(from));
                    case SubStringOptions.ToLast:
                        return source.Substring(0, source.LastIndexOf(from));
                    default:
                        break;
                }
            }
            return source;
        }

        /// <summary>
        /// Returns the content of a string that is between two other strings
        /// </summary>
        /// <param name="source"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static String Substring(this String source, String from, String to)
        {
            int fromIndex = 0;
            int toIndex = 0;
            if (source == null || !(0 <= (fromIndex = source.IndexOf(from) + from.Length) && (toIndex = (source.IndexOf(to) - fromIndex)) > 0)) // both strings are contained and from is before to
            {
                return String.Empty;
            }
            return source.Substring(fromIndex, toIndex);
        }

        public static DateTime ParseToDateTime(this String str, NumberStyles numberstyle = NumberStyles.Number, IFormatProvider format = null)
        {
            String objectString = str?.ToString() ?? "";
            DateTime result = DateTime.MinValue;

            foreach (var supportedFormat in SupportedFormats)
            {
                if (DateTime.TryParse(objectString, supportedFormat, DateTimeStyles.None, out result))
                {
                    break;
                }
            }

            if (result == DateTime.MinValue)
            {
                throw new NotSupportedException($"Unable to parse \"{objectString}\" into a valid datetime.");
            }

            return result;
        }

        public static Money ParseToMoney(this String str, NumberStyles numberstyle = NumberStyles.Number, IFormatProvider format = null)
        {
            return new Money(str.ParseToDecimal(numberstyle, format));
        }

        public static Decimal ParseToDecimal(this Object str, NumberStyles numberstyle = NumberStyles.Number, IFormatProvider format = null)
        {
            format = format ?? DefaultFormat;
            String objectString = (str as IConvertible)?.ToString(format) ?? "";
            decimal value = 0.0M;
            Decimal.TryParse(objectString, numberstyle, format, out value);
            return value;
        }



        /// <summary>
        /// Generates String representation of the most used crm objects
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String ToDetailedString(this Object CrmObject)
        {
            switch (CrmObject)
            {
                case null:
                    return "NULL";
                case IConvertible icv:
                    return icv.ToString(DefaultFormat);
                case EntityReference er:
                    return $"[{er?.Name} ({er?.LogicalName} :: {er?.Id})]";
                case Money mn:
                    return $"{mn?.Value.ToString(DefaultFormat)}";
                case OptionSetValue osv:
                    return $"{osv.Value}";
                case AliasedValue av:
                    return $"{av.Value.ToDetailedString()}";
            }

            return CrmObject?.ToString() ?? "";
        }
    }
}