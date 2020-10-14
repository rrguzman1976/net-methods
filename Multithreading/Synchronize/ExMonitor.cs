using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.Synchronize
{
    public class ExMonitor
    {
        public void Ex1_Monitor()
        {
            // Define the lock object.
            var obj = new Object();

            // Define the critical section.
            Monitor.Enter(obj);

            try
            {
                // Code to execute one thread at a time.
            }
            // catch blocks go here.
            finally
            {
                Monitor.Exit(obj); // release lock
            }

            // Equivalent to:
            lock (obj)
            {
                // Code to execute one thread at a time.
            }
        }

    }
}
