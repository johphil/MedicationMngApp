using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MedicationMngApp.Utils
{
    public static class Converters
    {
        //Date Converters
        public static string ToDateWithTime(this DateTime value)
        {
            return value == DateTime.MinValue ? "" : value.ToString("MMMM dd, yyyy hh:mm tt");
        }
        public static string ToDateWithDayAndTime(this DateTime value)
        {
            return value == DateTime.MinValue ? "" : value.ToString("dddd, MMMM dd, yyyy hh:mm tt");
        }
        public static string ToDateOnly(this DateTime value)
        {
            return value == DateTime.MinValue ? "" : value.ToString("MMMM dd, yyyy");
        }
        public static string ToTimeOnly(this DateTime value)
        {
            return value == DateTime.MinValue ? "" : value.ToString("hh:mm tt");
        }
        public static DateTime ToDateTime(this string value)
        {
            DateTime val;
            return DateTime.TryParse(value, out val) ? val : DateTime.MinValue;
        }
    }
    public class TimespanConverter : Newtonsoft.Json.JsonConverter<TimeSpan>
    {
        /// <summary>
        /// Format: Days.Hours:Minutes:Seconds:Milliseconds
        /// </summary>
        public const string TimeSpanFormatString = @"d\.hh\:mm\:ss\:FFF";

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            var timespanFormatted = value.ToString(TimeSpanFormatString);
            writer.WriteValue(timespanFormatted);
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            TimeSpan parsedTimeSpan;
            TimeSpan.TryParseExact((string)reader.Value, TimeSpanFormatString, null, out parsedTimeSpan);
            return parsedTimeSpan;
        }
    }
}
