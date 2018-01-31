using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.SignalWaits
{
    public class ExSemaphoreSlim
    {
        private SemaphoreSlim semaphore;

        // A padding interval to make the output more orderly.
        private int padding;

        public void Ex1_SemaphoreSlim()
        {
            // Create the semaphore.
            semaphore = new SemaphoreSlim(initialCount: 0, maxCount: 3);
            CountdownEvent cde = new CountdownEvent(5);

            Console.WriteLine("{0} tasks can enter the semaphore.", semaphore.CurrentCount);
            
            Task[] tasks = new Task[5];

            // Create and start five numbered tasks.
            for (int i = 0; i <= 4; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    cde.Signal();

                    // Each task begins by requesting the semaphore.
                    Console.WriteLine("Task {0} begins and waits for the semaphore.", Task.CurrentId);
                    semaphore.Wait();

                    Interlocked.Add(ref padding, 100);

                    Console.WriteLine("Task {0} enters the semaphore.", Task.CurrentId);

                    // The task just sleeps for 1+ seconds.
                    Thread.Sleep(1000 + padding);

                    Console.WriteLine("Task {0} releases the semaphore; previous count: {1}.", Task.CurrentId, semaphore.Release());
                });
            }

            // Wait for all the tasks to start and block.
            cde.Wait();

            // Restore the semaphore count to its maximum value.
            Console.Write("Main thread calls Release(3) --> ");
            semaphore.Release(3);

            Console.WriteLine("{0} tasks can enter the semaphore.", semaphore.CurrentCount);

            // Main thread waits for the tasks to complete.
            Task.WaitAll(tasks);
            semaphore.Dispose();
            cde.Dispose();

            Console.WriteLine("Main thread exits.");
        }
    }
}
