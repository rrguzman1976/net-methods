using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ExamRef.ExamLib.Types.Strings
{
    public class ExIFormattable
    {
        // Ex 1: Implement the IFormattable interface
        public void Ex1_IFormattable()
        {
            // Use composite formatting with format string in the format item.
            Temperature t1 = new Temperature(0);
            Console.WriteLine("{0:C} (Celsius) = {0:K} (Kelvin) = {0:F} (Fahrenheit)\n", t1); // ci culture

            // Use composite formatting with a format provider.
            t1 = new Temperature(-40);
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0:C} (Celsius) = {0:K} (Kelvin) = {0:F} (Fahrenheit)", t1));
            Console.WriteLine(String.Format(new CultureInfo("fr-FR"), "{0:C} (Celsius) = {0:K} (Kelvin) = {0:F} (Fahrenheit)\n", t1));
            
            // Call ToString method with format string.
            t1 = new Temperature(32);
            IFormatProvider fr = new CultureInfo("fr-FR");
            Console.WriteLine("{0} (Celsius) = {1} (Kelvin) = {2} (Fahrenheit)\n"
                , t1.ToString("C", fr), t1.ToString("K", fr), t1.ToString("F", fr));

            // Call ToString with format string and format provider
            t1 = new Temperature(100);
            NumberFormatInfo ci = NumberFormatInfo.CurrentInfo;
            CultureInfo nl = new CultureInfo("nl-NL");
            Console.WriteLine("{0} (Celsius) = {1} (Kelvin) = {2} (Fahrenheit)",
                              t1.ToString("C", ci), t1.ToString("K", ci), t1.ToString("F", ci));
            Console.WriteLine("{0} (Celsius) = {1} (Kelvin) = {2} (Fahrenheit)",
                              t1.ToString("C", nl), t1.ToString("K", nl), t1.ToString("F", nl));
        }
    }

    public class Temperature : IFormattable
    {
        private decimal temp;

        public Temperature(decimal temperature)
        {
            if (temperature < -273.15m)
                throw new ArgumentOutOfRangeException(String.Format("{0} is less than absolute zero.",
                                                      temperature));
            this.temp = temperature;
        }

        public decimal Celsius
        {
            get { return temp; }
        }

        public decimal Fahrenheit
        {
            get { return (temp * 9 / 5) + 32; }
        }

        public decimal Kelvin
        {
            get { return temp + 273.15m; }
        }
        
        // This override allows Temperature.ToString() to be formatted by default.
        public override string ToString()
        {
            return this.ToString("G", CultureInfo.CurrentCulture);
        }

        // Implicit IFormattable
        public string ToString(string format, IFormatProvider provider)
        {
            if (String.IsNullOrEmpty(format))
            {
                format = "G";
            }

            if (provider == null) // handle null
            {
                provider = CultureInfo.CurrentCulture;
            }

            // decimal structure implements IFormattable so reuse it's implementation (F2: 2 fixed points).
            switch (format.ToUpperInvariant())
            {
                case "G": // handle general
                case "C":
                    //return temp.ToString("F2", provider) + " °C";
                    // Or use built-in NumberFormatInfo via CultureInfo class.
                    return String.Format(provider, "{0:F2}", temp) + " °C";
                case "F":
                    return Fahrenheit.ToString("F2", provider) + " °F";
                case "K":
                    return Kelvin.ToString("F2", provider) + " K";
                // Handle unsupported formats
                default:
                    throw new FormatException(String.Format("The {0} format string is not supported.", format));
            }
        }
    }
}
