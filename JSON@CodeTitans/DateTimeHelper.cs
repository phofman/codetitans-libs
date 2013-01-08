#region License
/*
    Copyright (c) 2010, Paweł Hofman (CodeTitans)
    All Rights Reserved.

    Licensed under the Apache License version 2.0.
    For more information please visit:

    http://codetitans.codeplex.com/license
        or
    http://www.apache.org/licenses/


    For latest source code, documentation, samples
    and more information please visit:

    http://codetitans.codeplex.com/
*/
#endregion

using System;
using System.Globalization;
using CodeTitans.JSon;

namespace CodeTitans.Helpers
{
    static class DateTimeHelper
    {
        /// <summary>
        /// Number of tics since 1970-01-01. Required for DateTime -- Unix epoch conversions.
        /// </summary>
        internal static readonly long TicksAt1970 = 621355968000000000L; //new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        /// <summary>
        /// Parses string in following formats to DateTime representation:
        ///  - @decimal@ - 1970-01-01 + decimal (millisec) - UTC!
        ///  - \/Date(decimal)\/ - 1970-01-01 + decimal (millisec) - UTC!
        ///  - \/Date(yyyy,MM,dd[,hh,mm,ss,mmm])\/ - direct params - UTC!
        ///  - decimal - 1970-01-01 + decimal (millisec/sec) - UTC!
        ///  - ISO format - with time zones support
        /// </summary>
        public static DateTime ParseDateTime(string text, IFormatProvider provider, DateTimeStyles styles, JSonDateTimeKind kind)
        {
            if (!String.IsNullOrEmpty(text))
            {
                var date = text.Replace(" ", String.Empty).Replace("\t", String.Empty);

                if (date.Length > 2)
                {
                    if (date[0] == '@' && date[date.Length - 1] == '@')
                        return ParseDateTimeAsDecimal(date.Substring(1, date.Length - 2));

                    if (date.StartsWith("\\/Date(", StringComparison.OrdinalIgnoreCase) && date.EndsWith(")\\/"))
                        return ParseDateTimeAsArrayOfDecimals(date.Substring(7, date.Length - 10));

                    if (date.StartsWith("/Date(", StringComparison.OrdinalIgnoreCase) && date.EndsWith(")/"))
                        return ParseDateTimeAsArrayOfDecimals(date.Substring(6, date.Length - 8));
                }
            }

            // try to parse date as a pure number:
            long dateValue;
            if (NumericHelper.TryParseInt64(text, out dateValue))
                return ToDateTime(dateValue, kind);

            // always try to parse as ISO format, to get the result or throw a standard exception, when non-matching format given:
            return DateTime.Parse(text, provider, styles);
        }

        private static DateTime ParseDateTimeAsArrayOfDecimals(string text)
        {
            if (String.IsNullOrEmpty(text))
                return DateTime.Now;

            // has only one decimal as parameter?
            if (text.IndexOf(',') == -1)
                return ParseDateTimeAsDecimal(text);

            var args = text.Split(',');

            if (args == null || args.Length < 3)
                throw new FormatException(String.Concat("Too few parameters passed for the DateTime construction (\"", text, "\")"));
            if (args.Length > 7)
                throw new FormatException(String.Concat("Too many parameters passed for the DateTime construction (\"", text, "\")"));

            int year;
            int month;
            int day;
            int hour = 0;
            int minute = 0;
            int second = 0;
            int millisecond = 0;

            if (!NumericHelper.TryParseInt32(args[0], out year))
                throw new FormatException(String.Concat("Invalid year (\"", args[0], "\")"));
            if (!NumericHelper.TryParseInt32(args[1], out month))
                throw new FormatException(String.Concat("Invalid month (\"", args[1], "\")"));
            if (!NumericHelper.TryParseInt32(args[2], out day))
                throw new FormatException(String.Concat("Invalid day (\"", args[2], "\")"));

            if (args.Length > 3 && !String.IsNullOrEmpty(args[3]) && !NumericHelper.TryParseInt32(args[3], out hour))
                throw new FormatException(String.Concat("Invalid hour (\"", args[3], "\")"));
            if (args.Length > 4 && !String.IsNullOrEmpty(args[4]) && !NumericHelper.TryParseInt32(args[4], out minute))
                throw new FormatException(String.Concat("Invalid minute (\"", args[4], "\")"));
            if (args.Length > 5 && !String.IsNullOrEmpty(args[5]) && !NumericHelper.TryParseInt32(args[5], out second))
                throw new FormatException(String.Concat("Invalid second (\"", args[5], "\")"));
            if (args.Length > 6 && !String.IsNullOrEmpty(args[6]) && !NumericHelper.TryParseInt32(args[6], out millisecond))
                throw new FormatException(String.Concat("Invalid millisecond (\"", args[6], "\")"));

            return new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc).ToLocalTime();
        }

        private static DateTime ParseDateTimeAsDecimal(string text)
        {
            long value;

            // try to parse the milliseconds building the date-time:
            if (NumericHelper.TryParseInt64(text, out value))
                return new DateTime(TicksAt1970 + (value * TimeSpan.TicksPerMillisecond), DateTimeKind.Utc).ToLocalTime();

            throw new FormatException(String.Concat("Invalid DateTime specified as \"", text, "\" milliseconds"));
        }

        /// <summary>
        /// Parses several string formats into a DateTime object.
        /// Check other overloaded methods for more detailed description.
        /// </summary>
        public static DateTime ParseDateTime(string text, JSonDateTimeKind kind)
        {
            return ParseDateTime(text, CultureInfo.InvariantCulture, DateTimeStyles.None, kind);
        }

        /// <summary>
        /// Converts given decimal value into a DateTime object.
        /// </summary>
        public static DateTime ToDateTime(Int64 value, JSonDateTimeKind kind)
        {
            switch(kind)
            {
                case JSonDateTimeKind.Default:
                case JSonDateTimeKind.UnixEpochMilliseconds:
                    return new DateTime(TicksAt1970 + (value * TimeSpan.TicksPerMillisecond), DateTimeKind.Utc).ToLocalTime();
                case JSonDateTimeKind.UnixEpochSeconds:
                    return new DateTime(TicksAt1970 + (value * TimeSpan.TicksPerSecond), DateTimeKind.Utc).ToLocalTime();
                case JSonDateTimeKind.Ticks:
                    return new DateTime(value, DateTimeKind.Utc).ToLocalTime();

                default:
                    throw new FormatException(string.Concat("Invalid date conversion kind (", kind, ")"));
            }
        }

        public static Int64 ToNumber(DateTime date, JSonDateTimeKind kind)
        {
            var dateAsUniversal = date.ToUniversalTime();
            long ticks = dateAsUniversal.Ticks;

            switch(kind)
            {
                case JSonDateTimeKind.Default:
                case JSonDateTimeKind.UnixEpochMilliseconds:
                    if (ticks < TicksAt1970)
                        throw new ArgumentOutOfRangeException("date");
                    return (ticks - TicksAt1970) / TimeSpan.TicksPerMillisecond;
                case JSonDateTimeKind.UnixEpochSeconds:
                    if (ticks < TicksAt1970)
                        throw new ArgumentOutOfRangeException("date");
                    return (ticks - TicksAt1970) / TimeSpan.TicksPerSecond;
                case JSonDateTimeKind.Ticks:
                    return ticks;

                default:
                    throw new FormatException(string.Concat("Invalid date conversion kind (", kind, ")"));
            }
        }
    }
}
