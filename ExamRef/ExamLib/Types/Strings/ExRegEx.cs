using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ExamRef.ExamLib.Types.Strings
{
    public class ExRegEx
    {
        // Ex 1: Find duplicate word occurrences (consecutive).
        /*
         * \b: Start the match at a word boundary.
         * (\w+?): Match one or more word characters, but as few characters as possible. 
         *  Together, they form a group that can be referred to as \1.
         * \s+: Match one or more white-space characters.
         * (\s+\1)+: Match the substring that is equal to the group named \1 one or more times.
         * \b: Match a word boundary.
         */
        public void Ex1_DuplicateWords()
        {
            string input = "Hello world world world world ! Hello hello again  again again .";

            string pattern = @"\b" +
                             @"(\w+?)" +
                             @"(\s+\1)+" +
                             @"\b";

            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            MatchCollection matches = rgx.Matches(input);

            if (matches.Count > 0)
            {
                Console.WriteLine("{0} ({1} matches):", input, matches.Count);

                foreach (Match match in matches)
                {
                    Console.WriteLine(Environment.NewLine + "=".PadLeft(60, '='));
                    Console.WriteLine(">>>>Match: {0}", match.Value);
                    Console.WriteLine("=".PadLeft(60, '=') + Environment.NewLine);

                    GroupCollection groups = match.Groups;
                    foreach (Group g in groups)
                    {
                        Console.WriteLine(Environment.NewLine + "\t>>>>Groups");
                        Console.WriteLine("\t" + "*".PadLeft(50, '*'));

                        Console.WriteLine("\tGroup, Position {0}, Group: '{1}'" + Environment.NewLine,
                                          g.Index,
                                          g.Value);

                        Console.WriteLine("\t\t>>>>Captures");
                        Console.WriteLine("\t\t" + "*".PadLeft(50, '*'));
                        CaptureCollection captures = g.Captures;

                        foreach (Capture c in captures)
                        {
                            Console.WriteLine("\t\t" + "Capture, Position {0}, Group: '{1}'",
                                              c.Index,
                                              c.Value);
                        }
                    }
                }
            }
        }

        // Ex 2: Find values that could be a currency (does not support comma separators or fractional pennies).
        /*
         * ^: Start at the beginning of the string.
         * \s*: Match zero or more white-space characters.
         * [\+-]?: Match zero or one occurrence of either the positive sign or the negative sign.
         * \s?: Match zero or one white-space character.
         * \$?: Match zero or one occurrence of the dollar sign.
         * \s?: Match zero or one white-space character.
         * \d*: Match zero or more decimal digits.
         * \.?: Match zero or one decimal point symbol.
         * \d{2}?: Match two decimal digits zero or one time.
         * (\d*\.?\d{2}?){1}: Match the pattern of integral and fractional digits separated by a decimal point symbol
         *  at least one time.
         * $: Match the end of the string.
         */ 
        public void Ex2_FindCurrencyValues()
        {
            // The outer parentheses around this expression define it as a capturing group or a subexpression. 
            // If a match is found, information about this part of the matching string can be retrieved from 
            // the second Group object in the GroupCollection object returned by the Match.Groups property.
            string pattern = @"^\s*[\+-]?\s?\$?\s?(\d*\.?\d{2}?){1}$";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // Define some test strings.
            string[] tests = { "-42", "19.99", "0.001", "100 USD", 
                         ".34", "0.34", "1,052.21", " $10.62", 
                         "+1.43", "-$ 0.23" , " $ .10", " $ ." , " $ .10 "};

            // Check each test string against the regular expression.
            foreach (string test in tests)
            {
                if (rgx.IsMatch(test))
                    Console.WriteLine("{0} is a currency value.", test);
                else
                    Console.WriteLine("{0} is not a currency value.", test);
            }
        }

        // Ex 3: Use capturing groups
        /*
         * \b: Look for a word boundary.
         * (\w+?): Look for one or more word characters. Together, these form the trademarked name. 
         *  This is the first capturing group.
         * ([\u00AE\u2122]): Look for either the ® or the ™ character. This is the second capturing group.
         */
        public void Ex3_CapturingGroups()
        {
            // This regular expression assumes that a trademark consists of a single word.
            string pattern = @"\b(\w+?)([\u00AE\u2122])";
            string input = "Microsoft® Office Professional Edition combines several office " +
                           "productivity products, including Word, Excel®, Access®, Outlook®, " +
                           "PowerPoint®, and several others. Some guidelines for creating " +
                           "corporate documents using these productivity tools are available " +
                           "from the documents created using Silverlight™ on the corporate " +
                           "intranet site.";

            MatchCollection matches = Regex.Matches(input, pattern);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Console.WriteLine("Match {0}, Word {1}, Symbol {2}", groups[0], groups[1], groups[2]);
            }
            Console.WriteLine();
            Console.WriteLine("Found {0} trademarks or registered trademarks.", matches.Count);
        }

        // Ex 4: Match a whole sentence one time.
        /*
         * \b: Begin the match at a word boundary.
         * (\w+?): Match one or more word characters. This is the second (nested) capturing group.
         * [,:;]?: Match zero or one occurrence of a comma, colon, or semicolon.
         * \s?: Match zero or one occurrence of a white-space character.
         * (\b(\w+?)[,:;]?\s?)+: Match the entire pattern one or more times. This is the first 
         *  capturing group (i.e. a word sequence).
         * [?.!]: Match any occurrence of a period, question mark, or exclamation point.
         */
        public void Ex4_MatchSentences()
        {
            string pattern = @"(\b(\w+?)[,:;]?\s?)+[?.!]";
            string input = "This is one sentence. This is a second sentence pattern. Furthermore, this is a third.";

            Match match = Regex.Match(input, pattern);
            Console.WriteLine("Match: " + match.Value);

            while (match.Success)
            {
                Console.WriteLine(Environment.NewLine + "'{0}' found at position {1}." + Environment.NewLine
                    , match.Value, match.Index);

                int groupCtr = 0;

                foreach (Group group in match.Groups)
                {
                    groupCtr++;
                    // The subpattern (\w+?) is designed to match multiple words within a sentence. 
                    // However, the value of the Group object represents only the last match that (\w+?) captures, 
                    // whereas the Captures property returns a CaptureCollection that represents all captured text.
                    Console.WriteLine("   Group {0}: '{1}'", groupCtr, group.Value);
                    int captureCtr = 0;

                    foreach (Capture capture in group.Captures)
                    {
                        captureCtr++;
                        Console.WriteLine("      Capture {0}: '{1}'", captureCtr, capture.Value);
                    }
                }
                
                match = match.NextMatch();
            }
        }

        // Ex 5: Use Replace and MatchEvaluator delegate to replace a RegEx match.
        public void Ex5_Replace()
        {
            string sInput, sRegex;

            // The string to search.
            sInput = "aabbccddeeffcccgghhcccciijjcccckkcc";

            // A very simple regular expression.
            sRegex = "[c]+";

            // Assign the replace method to the MatchEvaluator delegate.
            MatchEvaluator myEvaluator = new MatchEvaluator(new ExRegEx().ReplaceCC);

            // Write out the original string.
            Console.WriteLine(sInput);

            // Replace matched characters using the delegate method.
            Regex r = new Regex(sRegex);
            sInput = r.Replace(sInput, (m) =>
                {
                    return (++i).ToString();
                });

            // Or:
            //sInput = r.Replace(sInput, myEvaluator);

            // Write out the modified string.
            Console.WriteLine(sInput);
        }

        private string ReplaceCC(Match m) // MatchEvaluator delegate
        // Replace each Regex cc match with the number of the occurrence.
        {
            i++;
            return i.ToString();
        }
        private static int i = 0;

        // Ex 6: Use a RegEx to check email validity.
        /*
         * ^: 
         *      Begin the match at the start of the string.
         * (?(""): 
         *      Determine whether the first character is a quotation mark. (?(") is the beginning of an alternation construct.
         *      Note "" is neccessary to match a single double-quote. The first double-quote is an escape.
         * (?("")("".+?(?<!\\)""@): 
         *      If the first character is a quotation mark, match a beginning quotation mark followed by at least one occurrence of 
         *      any character, followed by an ending quotation mark. The ending quotation mark must not be preceded by a backslash 
         *      character (\). (?<! is the beginning of a zero-width negative lookbehind assertion. The string should conclude with 
         *      an at sign (@).
         * |(([0-9a-z]: 
         *      If the first character is not a quotation mark, match any alphabetic character from a to z or A to Z (the comparison 
         *      is case insensitive), or any numeric character from 0 to 9.
         * (\.(?!\.)): 
         *      If the next character is a period, match it. If it is not a period, look ahead to the next character and continue 
         *      the match. (?!\.) is a zero-width negative lookahead assertion that prevents two consecutive periods from appearing 
         *      in the local part of an email address.
         * |[-!#\$%&'\*\+/=\?\^`\{\}\|~\w]: 
         *      If the next character is not a period, match any word character or one of the 
         *      following characters: -!#$%'*+=?^`{}|~.
         * ((\.(?!\.))|[-!#\$%'\*\+/=\?\^`\{\}\|~\w])*: 
         *      Match the alternation pattern (a period followed by a non-period, or one of a number of characters) zero or more times.
         * @: 
         *      Match the @ character.
         * (?<=[0-9a-z]): 
         *      Continue the match if the character that precedes the @ character is A through Z, a through z, or 0 through 9. The 
         *      (?<=[0-9a-z]) construct defines a zero-width positive lookbehind assertion.
         * (?(\[): 
         *      Check whether the character that follows @ is an opening bracket.
         * (\[(\d{1,3}\.){3}\d{1,3}\]): If it is an opening bracket, match the opening bracket followed by an IP address (four sets 
         *      of one to three digits, with each set separated by a period) and a closing bracket.
         * |(([0-9a-z][-\w]*[0-9a-z]*\.)+: 
         *      If the character that follows @ is not an opening bracket, match one alphanumeric character with a value of A-Z, a-z, 
         *      or 0-9, followed by zero or more occurrences of a word character or a hyphen, followed by zero or one alphanumeric 
         *      character with a value of A-Z, a-z, or 0-9, followed by a period. This pattern can be repeated one or more times, 
         *      and must be followed by the top-level domain name.
         * [a-z0-9][\-a-z0-9]{0,22}[a-z0-9])): 
         *      The top-level domain name must begin and end with an alphanumeric character (a-z, A-Z, and 0-9). It can also include 
         *      from zero to 22 ASCII characters that are either alphanumeric or hyphens.
         * $: 
         *      End the match at the end of the string.
         */
        public void Ex6_CheckEmail()
        {
            string pattern = @"^(?("")("".+?(?<!\\)""@)" +
                @"|(([0-9a-z]" +
                @"((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)" +
                @"(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])" +
                @"|(([0-9a-z][-\w]*[0-9a-z]*\.)+" +
                @"[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

            string[] emailAddresses = { "david.jones@proseware.com", "d.j@server1.proseware.com",
                                  "jones@ms1.proseware.com", "j.@server1.proseware.com",
                                  "j@proseware.com9", "js#internal@proseware.com",
                                  "j_9@[129.126.118.1]", "j..s@proseware.com",
                                  "js*@proseware.com", "js@proseware..com",
                                  "js@proseware.com9", "j.s@server1.proseware.com",
                                   "\"j\\\"s\\\"\"@proseware.com", "js@contoso.中国" };
            try
            {
                foreach (string e in emailAddresses)
                {
                    if (Regex.IsMatch(e, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5)))
                    {
                        Console.WriteLine("Valid: {0}", e);
                    }
                    else
                    {
                        Console.WriteLine("Invalid: {0}", e);
                    }
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                Console.WriteLine("RegEx timeout: {0}.", e.Message);
            }
        }

        // Ex 7: Strip invalid characters (strips out all nonalphanumeric characters except periods (.), 
        // at symbols (@), and hyphens (-))
        public void Ex7_Strip()
        {
            // Replace invalid characters with empty strings.
            string pattern = @"[^\w\.@-]";
            string strIn = "js#inte!rn$a*l@prose-ware.com";

            try
            {
                Console.WriteLine("Replaced: {0}", 
                    Regex.Replace(strIn, pattern, "", RegexOptions.None, TimeSpan.FromSeconds(1.5)));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException e)
            {
                Console.WriteLine("RegEx timeout: {0}.", e.Message);
            }
        }

        // Ex 8: Match open/close brackets.
        public void Ex8_MatchBrackets()
        {
            string pattern = "^[^<>]*" +
                                   "(" +
                                    "((?'Open'<)[^<>]*)+" +
                                    "((?'Close-Open'>)[^<>]*)+" +
                                   ")*" +
                                   "(?(Open)(?!))$";
            string input = "test<abc><mno<xyz>>";

            Match m = Regex.Match(input, pattern);
            if (m.Success == true)
            {
                Console.WriteLine("Input: \"{0}\" \nMatch: \"{1}\"", input, m);
                int grpCtr = 0;
                foreach (Group grp in m.Groups)
                {
                    Console.WriteLine("   Group {0}: {1}", grpCtr, grp.Value);
                    grpCtr++;
                    int capCtr = 0;
                    foreach (Capture cap in grp.Captures)
                    {
                        Console.WriteLine("      Capture {0}: {1}", capCtr, cap.Value);
                        capCtr++;
                    }
                }
            }
            else
            {
                Console.WriteLine("Match failed.");
            }
        }

        public void Scratch()
        {
            string input = "david.jones@proseware.com text d.j@server1.proseware.com" +
                           "jones@ms1.proseware.com test text j.@server1.proseware.com" +
                           "j@proseware.com9 test text js#internal@proseware.com" +
                           "j_9@[129.126.118.1] test text j..s@proseware.com" +
                           "js*@proseware.com test text js@proseware..com" +
                           "js@proseware.com9 text j.s@server1.proseware.com" +
                           "\"j\\\"s\\\"\"@proseware.com test js@contoso.中国";

            string pattern = @"\b(?("")("".+?(?<!\\)""@)" +
                @"|(([0-9a-z]" +
                @"((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)" +
                @"(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])" +
                @"|(([0-9a-z][-\w]*[0-9a-z]*\.)+" +
                @"[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))";
            
            MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Console.WriteLine("'{0}' at position {1}", match.Value, groups[0].Index);
            }
        }
    }
}
