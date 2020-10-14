using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

namespace ExamRef.ExamLib.Types.Strings
{
    public class ExStrings
    {
        // Ex 1: Use CultureInfo
        private Random rnd = new Random();
        public void Ex1_CultureInfo()
        {
            // When a new application thread is started, its ci culture and ci UI culture 
            // are defined by the ci system culture, and not by the ci thread culture. 
            if (Thread.CurrentThread.CurrentCulture.Name != "fr-FR")
            {
                // If ci culture is not fr-FR, set culture to fr-FR.
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");

                // Set these properties instead to ensure that all threads in the default application 
                // domain share the same culture.
                //CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
                //CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");
            }
            else
            {
                // Set culture to en-US.
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
            }
            DisplayThreadInfo();
            DisplayValues();

            Thread worker = new Thread((obj) =>
            {
                //Thread.CurrentThread.CurrentCulture = (CultureInfo)obj;
                //Thread.CurrentThread.CurrentUICulture = (CultureInfo)obj;
                DisplayThreadInfo();
                DisplayValues();
            });

            worker.Name = "WorkerThread";
            worker.Start();
            //worker.Start(Thread.CurrentThread.CurrentCulture); // pass in culture to override system culture
        }

        private void DisplayThreadInfo()
        {
            Console.WriteLine("\nCurrent Thread Name: '{0}'",
                              Thread.CurrentThread.Name);
            Console.WriteLine("Current Thread Culture/UI Culture: {0}/{1}",
                              Thread.CurrentThread.CurrentCulture.Name,
                              Thread.CurrentThread.CurrentUICulture.Name);
        }

        private void DisplayValues()
        {
            // Create new thread and display three random numbers.
            Console.WriteLine("Some currency values:");
            for (int ctr = 0; ctr <= 3; ctr++)
            {
                Console.WriteLine("   {0:C2}", rnd.NextDouble() * 10);
            }
        }

        // Ex 2: Use CultureInfo with tasks.
        /*
         * For apps that target versions of the .NET Framework prior to the .NET Framework 4.6, or for apps that do not 
         * target a particular version of the .NET Framework, the culture of the calling thread is not part of a task's 
         * context. Instead, unless one is explicitly defined, the culture of new threads by default is the system culture.
         */
        public void Ex2_CultureInfoTask()
        {
            decimal[] values = { 163025412.32m, 18905365.59m };
            string formatString = "C2";
            Func<String> formatDelegate = () =>
            {
                string output = String.Format("Formatting using the {0} culture on thread {1}.\n",
                                              CultureInfo.CurrentCulture.Name,
                                              Thread.CurrentThread.ManagedThreadId);
                foreach (var value in values)
                    output += String.Format("{0}   ", value.ToString(formatString));

                output += Environment.NewLine;
                return output;
            };

            Console.WriteLine("The example is running on thread {0}",
                              Thread.CurrentThread.ManagedThreadId);
            // Make the ci culture different from the system culture.
            Console.WriteLine("The current culture is {0}",
                              CultureInfo.CurrentCulture.Name);

            if (CultureInfo.CurrentCulture.Name == "fr-FR")
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            }

            Console.WriteLine("Changed the current culture to {0}.\n",
                              CultureInfo.CurrentCulture.Name);
            
            /*
             * For apps that target versions of the .NET Framework from the .NET Framework 4.5 and later but prior 
             * to the .NET Framework 4.6, you can use the DefaultThreadCurrentCulture and DefaultThreadCurrentUICulture 
             * properties to ensure that the culture of the calling thread is used in asynchronous tasks that 
             * execute on thread pool threads.
             */
            //CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;

            // Execute the delegate synchronously.
            Console.WriteLine("Executing the delegate synchronously:");
            Console.WriteLine(formatDelegate());

            // Call an async delegate to format the values using one format string.
            Console.WriteLine("Executing a task asynchronously:");
            var t1 = Task.Run(formatDelegate);
            Console.WriteLine(t1.Result);

            Console.WriteLine("Executing a task synchronously:");
            var t2 = new Task<String>(formatDelegate);
            t2.RunSynchronously(); // on main thread
            Console.WriteLine(t2.Result);
        }

        // Ex 3: Use format strings
        public void Ex3_FormatString()
        {
            DateTime dt = DateTime.Now;

            CultureInfo ci = new CultureInfo("fr-Fr");

            Console.WriteLine("{0}", dt.ToString("d", ci));
            Console.WriteLine("{0}", dt.ToString("D", ci));
            Console.WriteLine("{0}", dt.ToString("M", ci));
        }

        // Ex 4: Implement custom formatting
        // Use IFormattable instead see ExIFormattable.cs

        // Ex 5: Use String class methods
        public void Ex5_StringMethods()
        {
            string h1 = "Hello World!";
            char c = 'o';
            int i = h1.IndexOf(c);

            if (i == -1)
            {
                Console.WriteLine("{0} not found.", c);
            }
            else
            {
                Console.WriteLine("{1} found at index: {0}.", i, c);
            }

            i = h1.LastIndexOf(c);

            if (i == -1)
            {
                Console.WriteLine("{0} not found.", c);
            }
            else
            {
                Console.WriteLine("{1} found at index: {0}.", i, c);
            }

            string prefix = "hello";
            string suffix = "!";

            if (h1.StartsWith(prefix, ignoreCase: true, culture: CultureInfo.CurrentCulture))
            {
                Console.WriteLine("Prefix {0} found.", prefix);
            }
            else
            {
                Console.WriteLine("Prefix {0} not found.", prefix);
            }

            if (h1.EndsWith(suffix, ignoreCase: true, culture: CultureInfo.CurrentCulture))
            {
                Console.WriteLine("Suffix {0} found.", suffix);
            }
            else
            {
                Console.WriteLine("Suffix {0} not found.", suffix);
            }

            Console.WriteLine("Substring: {0}", h1.Substring(0, 5));
        }

        // Ex 6: Use string class methods with StringComparison enumeration
        public void Ex6_StringComparison()
        {
            // Use English / US culture (standard).
            if (Thread.CurrentThread.CurrentCulture.DisplayName != "en-US")
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            }

            Console.WriteLine("Current Culture: {0}", CultureInfo.CurrentCulture.Name);

            if (String.Equals("encyclop\u00e6dia", "encyclopaedia", StringComparison.CurrentCulture))
            {
                Console.WriteLine("encyclop\u00e6dia == encyclopaedia");
            }
            else
            {
                Console.WriteLine("encyclop\u00e6dia != encyclopaedia");
            }

            if (String.Equals("case", "Case", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("case == Case");
            }
            else
            {
                Console.WriteLine("case != Case");
            }

            // Use Sami / Sweden culture (standard).
            if (Thread.CurrentThread.CurrentCulture.DisplayName != "se-SE")
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("se-SE");
            }

            Console.WriteLine("Current Culture: {0}", CultureInfo.CurrentCulture.Name);

            if (String.Equals("encyclop\u00e6dia", "encyclopaedia", StringComparison.CurrentCulture))
            {
                Console.WriteLine("encyclop\u00e6dia == encyclopaedia");
            }
            else
            {
                Console.WriteLine("encyclop\u00e6dia != encyclopaedia");
            }

            if (String.Equals("case", "Case", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("case == Case");
            }
            else
            {
                Console.WriteLine("case != Case");
            }
        }

        // Ex 7: Use StringBuilder class
        public void Ex7_StringBuilder()
        {
            // Create a StringBuilder that expects to hold 50 characters.
            // Initialize the StringBuilder with "ABC".
            StringBuilder sb = new StringBuilder("ABC", 50);

            // Append three characters (D, E, and F) to the end of the StringBuilder.
            sb.Append(new char[] { 'D', 'E', 'F' });

            // Append a format string to the end of the StringBuilder.
            sb.AppendFormat("GHI{0}{1}", 'J', 'k');

            // Display the number of characters in the StringBuilder and its string.
            Console.WriteLine("{0} chars: {1}", sb.Length, sb.ToString());

            // Insert a string at the beginning of the StringBuilder.
            sb.Insert(0, "Alphabet: ");

            // Replace all lowercase k's with uppercase K's.
            sb.Replace('k', 'K');

            // Display the number of characters in the StringBuilder and its string.
            Console.WriteLine("{0} chars: {1}", sb.Length, sb.ToString());
        }
    }
}
