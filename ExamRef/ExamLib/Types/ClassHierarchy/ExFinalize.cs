using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    public class ExFinalize
    {
        // Ex 1: Define a finalizer
        public void Ex1_Finalize()
        {
            ExFinalizer ef = new ExFinalizer();
            ef = null; // clear reference

            // Force finalizers to run
            GC.Collect();
        }

    }

    public class ExFinalizer
    {
        ~ExFinalizer()
        {
            Console.WriteLine("Finalizer called");
        }
    }
}
