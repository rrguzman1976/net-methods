using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.Encapsulation
{
    public interface IAPI31
    {
        void Method1(string p1);
    }

    public interface IAPI32
    {
        void Method1(string p1);
    }

    public interface IAPI33
    {
        void Method1(string p1);
    }

    public class ExInterface: IAPI31, IAPI32, IAPI33
    {
        void IAPI31.Method1(string p1)
        {
            Console.WriteLine("IAPI31.Method1: {0}", p1);
        }

        void IAPI32.Method1(string p1)
        {
            Console.WriteLine("IAPI32.Method1: {0}", p1);
        }

        // Assumed to be IAPI33 implicitly.
        public void Method1(string p1)
        {
            Console.WriteLine("IAPI33.Method1: {0}", p1);
        }
    }
}
