using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.Threads
{
    public class ExThreadPool
    {
        public void Ex1_CreateFromPool()
        {
            bool sentinal = true;

            // Accepts delegate: void WaitCallback(object state)
            ThreadPool.QueueUserWorkItem((s) =>
            {
                while (sentinal)
                {
                    Console.WriteLine("Working on a thread from threadpool: {0}", s);
                    Thread.Sleep(1000);
                }
            }, "custom state");

            Console.ReadKey();

            sentinal = false;
        }
    }
}
