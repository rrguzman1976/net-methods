using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    public partial class ExPartialClass
    {
        public void Ex2_PartialClass()
        {
            Console.WriteLine("Ex2_PartialClass");
            PartialMethod(1); // removed from compilation output if not implemented.
        }

        partial void PartialMethod(int i);
    }
}
