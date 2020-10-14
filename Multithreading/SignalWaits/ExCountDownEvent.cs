using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.SignalWaits
{
    public class ExCountDownEvent
    {
        public void Ex1_CountdownEvent()
        {
            // Initialize a queue and a CountdownEvent
            ConcurrentQueue<int> queue = new ConcurrentQueue<int>(Enumerable.Range(0, 10000));

            // It's good for to release a CountdownEvent when you're done with it.
            using (CountdownEvent cde = new CountdownEvent(10000)) // initial count = 10000
            { 
                // This is the logic for all queue consumers
                Action<object> consumer = (s) =>
                {
                    int local;
                    // decrement CDE count once for each element consumed from queue
                    while (queue.TryDequeue(out local))
                    {
                        Console.WriteLine("Task {0}, Dequeue = {1}", s, local);
                        cde.Signal();
                    }
                };

                // Now empty the queue with a couple of asynchronous tasks
                Task t1 = Task.Factory.StartNew(consumer, "Task1");
                Task t2 = Task.Factory.StartNew(consumer, "Task2");

                // And wait for queue to empty by waiting on cde
                cde.Wait(); // will return when cde count reaches 0

                Console.WriteLine("Done emptying queue.  InitialCount={0}, CurrentCount={1}, IsSet={2}",
                    cde.InitialCount, cde.CurrentCount, cde.IsSet);

                // Proper form is to wait for the tasks to complete, even if you know that their work
                // is done already.
                Task.WaitAll(t1, t2);
            }
        }
    }
}
