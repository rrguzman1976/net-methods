using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.SignalWaits
{
    public class ExMutex
    {
        // Create a new Mutex. 
        // Initially owned = true => the creating thread does own the mutex. Like an
        // unsignalled (blocking) state.
        private Mutex mut = new Mutex(initiallyOwned: true);
        private const int numIterations = 1;
        private const int numThreads = 3;

        public void Ex1_Mutex()
        {
            Task[] t = new Task[numThreads];

            // Create 3 threads that will use the protected resource.
            for (int i = 0; i < numThreads; i++)
            {
                t[i] = Task.Factory.StartNew(ThreadProc, i);
            }

            // Release initial mutex
            mut.ReleaseMutex();
            Task.WaitAll(t);

            Console.WriteLine("Disposing");
            mut.Dispose();
        }

        private void ThreadProc(object t)
        {
            for (int i = 0; i < numIterations; i++)
            {
                UseResource(t);
            }
        }

        // This method represents a resource that must be synchronized
        // so that only one thread at a time can enter.
        private void UseResource(object t)
        {
            // Wait until it is safe to enter.
            Console.WriteLine("Task {0} is requesting the mutex", t);
            mut.WaitOne();

            Console.WriteLine("Task {0} has entered the protected area", t);

            // Place code to access non-reentrant resources here.

            // Simulate some work.
            Thread.Sleep(500);

            Console.WriteLine("Task {0} is leaving the protected area", t);

            // Release the Mutex.
            mut.ReleaseMutex();
            Console.WriteLine("Task {0} has released the mutex", t);
        }
    }
}
