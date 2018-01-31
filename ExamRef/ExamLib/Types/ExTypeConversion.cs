using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

namespace ExamRef.ExamLib.Types
{
    public class ExTypeConversion
    {
        // Helper method to list all cultures.
        public static void ListCultures()
        {
            // Get all cultures.
            CultureInfo[] custom = CultureInfo.GetCultures(CultureTypes.AllCultures);
            if (custom.Length == 0)
            {
                Console.WriteLine("Custom cultures:");
                foreach (var culture in custom)
                    Console.WriteLine("   {0} -- {1}", culture.Name, culture.DisplayName);
            }
            Console.WriteLine();
        }

        // Ex 1: Implicit conversion
        private interface IBase0 { }
        private class BaseA : IBase0 { }
        private class DerivedB : BaseA { }

        public void Ex1_Implicit()
        {
            int i = 7;
            double d = i; // allowed

            DerivedB db = new DerivedB();
            BaseA ba = db; // allowed (polymorphism)

            BaseA ba2 = new BaseA();
            IBase0 ib0 = ba2; // allowed (interface polymorphism)

        }

        // Ex 2: Explicit conversion
        public void Ex2_Explicit()
        {
            double d = 10.5;
            int i = (int) d; // cast required (truncation)

            BaseA ba = new DerivedB();
            DerivedB db = (DerivedB) ba;
        }

        // Ex 3: User-defined conversion
        public double Val1 { get; set; }

        // Implicit operator from ExTypeConversion to double.
        public static implicit operator double(ExTypeConversion e)
        {
            return e.Val1;
        }

        // Explicit operator from ExTypeConversion to int.
        public static explicit operator int(ExTypeConversion e)
        {
            return (int) e.Val1;
        }

        public void Ex3_UserDefined()
        {
            ExTypeConversion e = new ExTypeConversion() { Val1 = 21.1 };
            double d = e; // implicit allowed
            int i = (int)e; // explicit allowed

            Console.WriteLine("d: {0}, i: {1}", d, i);
        }

        /**********************************************************************************
         * Parse / Try Parse
         **********************************************************************************/

        // Ex 4: Conversion with helper classes
        public void Ex4_HelperClasses()
        {
            int value;
            try
            {
                value = Convert.ToInt32("21!"); 
            }
            catch (FormatException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            finally
            {
                value = 21;
            }

            try
            {
                value = Int32.Parse("7");
                Console.WriteLine("value: {0}", value);
            }
            catch (FormatException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            if (Int32.TryParse("22", out value))
            {
                Console.WriteLine("TryParse value: {0}", value);
            }
        }

        // Ex 5: Use globalization classes for i18n parsing / try parsing.
        public void Ex5_i18NParse()
        {
            CultureInfo english = new CultureInfo("en-US");
            CultureInfo french = new CultureInfo("fr-FR");

            string value = "€19,95";
            decimal d = decimal.Parse(value, NumberStyles.Currency, french);
            Console.WriteLine(d.ToString(english)); // Displays 19.95
        }

        /**********************************************************************************
         * Date/Time Parse / Try Parse
         * MST is -7 offset from UTC after Oct/15 (no DST).
         **********************************************************************************/

        // Ex 6: Converts date strings that contain time zone information to the time in the local time zone
        public void Ex6_ConvertTimeZone()
        {
            string[] dateStrings = {
                                       "2008-05-01T07:34:42-5:00", // Offset from UTC
                                        "2008-05-01 7:34:42Z", // Zulu-time = UTC
                                        "Thu, 01 May 2008 07:34:42 GMT" // GMT (Greenich Mean Time) = UTC
                                   };
            
            try
            {
                foreach (string ds in dateStrings)
                {
                    DateTime cd = DateTime.Parse(ds);
                    Console.WriteLine("Converted {0} to {1} time {2}", ds, cd.Kind, cd);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            catch (FormatException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }

        // Ex 7: Preserve the value of a date and time's Kind property during a formatting and parsing 
        // operation by using the DateTimeStyles.RoundtripKind flag
        public void Ex7_Roundtrip()
        {
            string[] fmtDates = { 
                                    "2008-09-15T09:30:41.7752486-07:00", // Offset from UTC (MST)
                                    "2008-09-15T09:30:41.7752486Z",  // Zulu-time = UTC
                                    "2008-09-15T09:30:41.7752486",  
                                    "2008-09-15T09:30:41.7752486-04:00", // Offset from UTC
                                    "Mon, 15 Sep 2008 09:30:41 GMT"  // GMT (Greenich Mean Time) = UTC
                                };

            try
            {
                foreach (string fd in fmtDates)
                {
                    Console.WriteLine(fd);
                    
                    DateTime rtDate = DateTime.Parse(fd, null, DateTimeStyles.RoundtripKind);
                    Console.WriteLine("   With RoundtripKind flag: date: {0} kind: {1}", rtDate, rtDate.Kind);

                    DateTime noRtDate = DateTime.Parse(fd, null, DateTimeStyles.None);
                    Console.WriteLine("   Without RoundtripKind flag: date: {0} kind: {1}", noRtDate, noRtDate.Kind);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            catch (FormatException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }

        // Ex 8: Override local culture when parsing a date. 
        public void Ex8_DateI18N()
        {
            // Assume the ci culture is en-US. 
            // The date is February 16, 2008, 12 hours, 15 minutes and 12 seconds.

            // Use standard en-US date and time value
            DateTime dateValue;
            string dateString = "2/16/2008 12:15:12 PM";

            try
            {
                dateValue = DateTime.Parse(dateString);
                Console.WriteLine("'{0}' converted to {1}, Kind: {2}.", dateString, dateValue, dateValue.Kind);
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert '{0}'.", dateString);
            }

            // Reverse month and day to conform to the fr-FR culture.
            // The date is February 16, 2008, 12 hours, 15 minutes and 12 seconds.
            dateString = "16/02/2008 12:15:12";
            
            try
            {
                dateValue = DateTime.Parse(dateString);
                Console.WriteLine("'{0}' converted to {1}.", dateString, dateValue);
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert '{0}'.", dateString);
            }

            // Call another overload of Parse to successfully convert string
            // formatted according to conventions of fr-FR culture.      
            try
            {
                dateValue = DateTime.Parse(dateString, new CultureInfo("fr-FR", false));
                Console.WriteLine("'{0}' converted to {1}.", dateString, dateValue);
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert '{0}'.", dateString);
            }

            // Parse string with date but no time component.
            dateString = "2/16/2008";
            try
            {
                dateValue = DateTime.Parse(dateString);
                Console.WriteLine("'{0}' converted to {1}.", dateString, dateValue);
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert '{0}'.", dateString);
            }   
        }

        // Ex 9: Parse using different locales
        public void Ex9_DateLocales()
        {
            // Define cultures to be used to parse dates.
            CultureInfo[] cultures = {
                                            CultureInfo.CreateSpecificCulture("en-US"), 
                                            CultureInfo.CreateSpecificCulture("fr-FR"), 
                                            CultureInfo.CreateSpecificCulture("de-DE")
                                     };

            // Define string representations of a date to be parsed.
            string[] dateStrings = {
                                        "01/10/2009 7:34 PM", 
                                        "10.01.2009 19:34", 
                                        "10-1-2009 19:34" };

            // Parse dates using each culture.
            foreach (CultureInfo culture in cultures)
            {
                DateTime dateValue;
                Console.WriteLine("Attempted conversions using {0} culture.", culture.Name);

                foreach (string dateString in dateStrings)
                {
                    try
                    {
                        dateValue = DateTime.Parse(dateString, culture);
                        Console.WriteLine("   Converted '{0}' to {1}.", dateString, dateValue.ToString("f", culture));
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("   Unable to convert '{0}' for culture {1}.", dateString, culture.Name);
                    }
                }

                Console.WriteLine();
            }
        }

        // Ex 10: Uses DateTimeStyles to specify formatting options.
        public void Ex10_DateStyles()
        {
            // Parse a date and time with no styles.
            string dateString = "03/01/2009 10:00 AM";
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            DateTime result;
            
            try
            {
                result = DateTime.Parse(dateString, culture, styles);
                Console.WriteLine("{0} converted to {1} {2}.", dateString, result, result.Kind.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert {0} to a date and time.", dateString);
            }

            // Parse the same date and time with the AssumeLocal style.
            styles = DateTimeStyles.AssumeLocal;
            try
            {
                result = DateTime.Parse(dateString, culture, styles);
                Console.WriteLine("{0} converted to {1} {2}.", dateString, result, result.Kind.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert {0} to a date and time.", dateString);
            }

            // Parse a date and time that is assumed to be local.
            // This time is five hours behind UTC. The local system's time zone is eight (MST: -7) hours behind UTC.
            dateString = "2009/03/01T10:00:00-5:00";
            styles = DateTimeStyles.AssumeLocal;
            try
            {
                result = DateTime.Parse(dateString, culture, styles);
                Console.WriteLine("{0} converted to {1} {2}.", dateString, result, result.Kind.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert {0} to a date and time.", dateString);
            }

            // Attempt to convert a string in improper ISO 8601 format.
            dateString = "03/01/2009T10:00:00-5:00";
            try
            {
                result = DateTime.Parse(dateString, culture, styles);
                Console.WriteLine("{0} converted to {1} {2}.", dateString, result, result.Kind.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert {0} to a date and time.", dateString);
            }

            // Assume a date and time string formatted for the fr-FR culture is the local 
            // time and convert it to UTC.
            dateString = "2008-03-01 10:00"; // local -7
            culture = CultureInfo.CreateSpecificCulture("fr-FR");
            styles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal; // adjust to UTC + 7
            try
            {
                result = DateTime.Parse(dateString, culture, styles);
                Console.WriteLine("{0} converted to {1} {2}.", dateString, result, result.Kind.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert {0} to a date and time.", dateString);
            }
        }

        // Ex 11: Use TryParse
        public void Ex11_TryParseDate()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB"); // Great Britain

            DateTime date1 = new DateTime(2013, 6, 1, 12, 32, 30);
            List<string> badFormats = new List<String>();

            Console.WriteLine("{0,-37} {1,-19}\n", "Date String", "Date");
            foreach (var dateString in date1.GetDateTimeFormats())
            {
                DateTime parsedDate;
                if (DateTime.TryParse(dateString, out parsedDate))
                    Console.WriteLine("{0,-37} {1,-19}", dateString, DateTime.Parse(dateString));
                else
                    badFormats.Add(dateString);
            }

            // Display strings that could not be parsed.
            if (badFormats.Count > 0)
            {
                Console.WriteLine("\nStrings that could not be parsed: ");
                foreach (var badFormat in badFormats)
                    Console.WriteLine("   {0}", badFormat);
            }
        }

        // Ex 12: Use TryParseExact to convert a date and time string that must match a particular format.
        public void Ex11_TryParseExactDate()
        {
            string[] formats = { "yyyyMMdd", "HHmmss" };
            string[] dateStrings = { 
                                        "20130816", "20131608", "  20130816   ", 
                                        "115216", "521116", "  115216  " 
                                   };
            DateTime parsedDate;

            foreach (var dateString in dateStrings)
            {
                if (DateTime.TryParseExact(dateString, formats, null,
                                           DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal,
                                           out parsedDate))
                {
                    Console.WriteLine("{0} --> {1:g}", dateString, parsedDate);
                }
                else
                {
                    Console.WriteLine("Cannot convert {0}", dateString);
                }
            }
        }

        // Ex 13: Save dates via invariant culture.
        public void Ex12_InvariantCultureDate()
        {
            DateTime[] dates = { 
                                   new DateTime(2014, 6, 14, 6, 32, 0), 
                                   new DateTime(2014, 7, 10, 23, 49, 0),  
                                   new DateTime(2015, 1, 10, 1, 16, 0), 
                                   new DateTime(2014, 12, 20, 21, 45, 0), 
                                   new DateTime(2014, 6, 2, 15, 14, 0) 
                               };

            Console.WriteLine("Current Time Zone: {0}", TimeZoneInfo.Local.DisplayName);
            Console.WriteLine("The dates on an {0} system:", Thread.CurrentThread.CurrentCulture.Name);

            for (int ctr = 0; ctr < dates.Length; ctr++)
            {
                Console.WriteLine(dates[ctr].ToString("f"));
                Console.WriteLine("To UTC: {0}", dates[ctr].ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
            }
        }

        // Ex 13: DateTimeOffset structure should be used over the DateTime structure.
        public void Ex13_DateTimeOffset()
        {
            DateTime thisDate = new DateTime(2007, 3, 10, 0, 0, 0);
            DateTime dstDate = new DateTime(2007, 6, 10, 0, 0, 0);
            DateTimeOffset thisTime;

            thisTime = new DateTimeOffset(dstDate, new TimeSpan(-7, 0, 0));
            ShowPossibleTimeZones(thisTime);

            thisTime = new DateTimeOffset(thisDate, new TimeSpan(-6, 0, 0));
            ShowPossibleTimeZones(thisTime);

            thisTime = new DateTimeOffset(thisDate, new TimeSpan(+1, 0, 0));
            ShowPossibleTimeZones(thisTime);
        }

        private void ShowPossibleTimeZones(DateTimeOffset offsetTime)
        {
            TimeSpan offset = offsetTime.Offset;
            ReadOnlyCollection<TimeZoneInfo> timeZones;

            Console.WriteLine("{0} could belong to the following time zones:", offsetTime.ToString());
            // Get all time zones defined on local system
            timeZones = TimeZoneInfo.GetSystemTimeZones();
            // Iterate time zones 
            foreach (TimeZoneInfo timeZone in timeZones)
            {
                // Compare offset with offset for that date in that time zone
                if (timeZone.GetUtcOffset(offsetTime.DateTime).Equals(offset))
                    Console.WriteLine("   {0}", timeZone.DisplayName);
            }
            Console.WriteLine();
        }

        // Ex 14: Convert from UTC and local timezones.
        public void Ex14_ConvertUTCLocal()
        {
            // Create Pacific Standard Time value and TimeZoneInfo object      
            DateTime estTime = new DateTime(2015, 10, 31, 00, 00, 00); // last day of DST (UTC -6)
            string timeZoneName = "Eastern Standard Time";
            try
            {
                TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);

                // Convert EST to local time
                DateTime localTime = TimeZoneInfo.ConvertTime(estTime, est, TimeZoneInfo.Local);
                Console.WriteLine("At {0} / {1}, the local time is {2} / {3}.",
                        estTime,
                        est,
                        localTime,
                        TimeZoneInfo.Local.IsDaylightSavingTime(localTime) ?
                                  TimeZoneInfo.Local.DaylightName :
                                  TimeZoneInfo.Local.StandardName);

                // Convert EST to UTC
                DateTime utcTime = TimeZoneInfo.ConvertTime(estTime, est, TimeZoneInfo.Utc);
                Console.WriteLine("At {0} / {1}, the UTC time is {2} / {3}.",
                        estTime,
                        est,
                        utcTime,
                        TimeZoneInfo.Utc.StandardName);
            }
            catch (TimeZoneNotFoundException)
            {
                Console.WriteLine("The {0} zone cannot be found in the registry.", timeZoneName);
            }
            catch (InvalidTimeZoneException)
            {
                Console.WriteLine("The registry contains invalid data for the {0} zone.", timeZoneName);
            }
        }
    }
}
