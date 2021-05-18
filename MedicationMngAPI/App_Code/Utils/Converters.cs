using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Converters
/// </summary>
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