using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExamRef.ExamLib.Multithreading.Tasks;

namespace ExStrongDLL
{
    public class ExStrongDLL
    {
        public void Method01()
        {
            Console.WriteLine("Hello world! ExStrongDLL.dll");

            // not allowed
            /*
            ExTask et = new ExTask();
            et.Ex1_CreateTask();
            */
        }
    }
}
