using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    public enum Color
    {
        Red,
        Green,
        Blue
    }

    public class ExYield
    {
        // Legal use of yield
        public static IEnumerable<Color> GetColors()
        {
            yield return Color.Red;
            yield return Color.Green;
            yield return Color.Blue;
        }

        public void Ex1_Yield()
        {
            foreach (Color c in GetColors())
            {
                Console.WriteLine(c);
            }
        }
    }
}
