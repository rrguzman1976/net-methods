using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    public partial class ExPartialClass
    {
        public void Ex1_PartialClass()
        {
            Console.WriteLine("Ex1_PartialClass");
        }

        partial void PartialMethod(int i)
        {
            Console.WriteLine("Partial: {0}", i);
        }
    }
}
