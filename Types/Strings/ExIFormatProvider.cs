using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ExamRef.ExamLib.Types.Strings
{
    // Ex 1: Implement IFormatProvider to format a long into accounting format.
    public class ExIFormatProvider 
    {
        public void Ex1_IFormatProvider()
        {
            long acctNumber = 104254567890;
            double balance = 16.34;
            DayOfWeek wday = DayOfWeek.Monday;

            string output = String.Format("On {2}, the balance of account {0:G} was {1:C2}.",
                                    acctNumber, balance, wday);
            Console.WriteLine(output);

            // IFormatProvider must be provided to override default formatting above.
            wday = DayOfWeek.Tuesday;
            output = String.Format(new AcctNumberFormat(), 
                                    "On {2}, the balance of account {0:H} was {1:C2}.", 
                                    acctNumber, balance, wday);
            Console.WriteLine(output);

            wday = DayOfWeek.Wednesday;
            output = String.Format(new AcctNumberFormat(), 
                                    "On {2}, the balance of account {0:I} was {1:C2}.", 
                                    acctNumber, balance, wday);
            Console.WriteLine(output);
        }
    }
        
    public class AcctNumberFormat : IFormatProvider, ICustomFormatter
    {
        private const int ACCT_LENGTH = 12;

        public object GetFormat(Type formatType) // implicit IFormatProvider
        {
            if (formatType == typeof(ICustomFormatter))
            {
                // Return self to implement custom formatting.
                // A culture info class would instead return a NumberFormatInfo or DateTimeFormatInfo here
                // w/o having to implement ICustomFormatter.
                return this;
            }
            else
                return null; // use default formatter
        }

        // Implicit ICustomFormatter
        public string Format(string fmt, object arg, IFormatProvider provider)
        {
            // Display information about method call.
            string formatString = fmt ?? "<null>";
            Console.WriteLine(Environment.NewLine + "Provider: {0}, Object: {1}, Format String: {2}",
                              provider, arg ?? "<null>", formatString);

            // Provide default formatting if arg is not an Int64.
            if (arg.GetType() != typeof(Int64))
            { 
                try
                {
                    return HandleOtherFormats(fmt, arg);
                }
                catch (FormatException e)
                {
                    throw new FormatException(String.Format("Custom: The format of '{0}' is invalid.", fmt), e);
                }
            }

            // Provide default formatting for unsupported format strings.
            fmt = fmt ?? "G"; // handle null format as G
            string ufmt = fmt.ToUpperInvariant();
            if (!(ufmt == "H" || ufmt == "I"))
                try
                {
                    return HandleOtherFormats(fmt, arg);
                }
                catch (FormatException e)
                {
                    throw new FormatException(String.Format("Custom: The format of '{0}' is invalid.", fmt), e);
                }

            // Convert argument to a string.
            string result = arg.ToString();

            // If account number is less than 12 characters, pad with leading zeroes.
            if (result.Length < ACCT_LENGTH)
                result = result.PadLeft(ACCT_LENGTH, '0');
            // If account number is more than 12 characters, truncate to 12 characters.
            if (result.Length > ACCT_LENGTH)
                result = result.Substring(0, ACCT_LENGTH);

            if (ufmt == "I") // Integer-only format. 
                return result;
            // Add hyphens for H format specifier.
            else // Hyphenated format.
                return result.Substring(0, 5) + "-" + result.Substring(5, 3) + "-" + result.Substring(8);
        }

        private string HandleOtherFormats(string format, object arg)
        {
            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
            else if (arg != null)
                return arg.ToString();
            else // handle null
                return String.Empty;
        }
    }
}
