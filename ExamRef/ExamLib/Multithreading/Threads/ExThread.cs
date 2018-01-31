using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.Threads
{
    public class ExThread
    {
        // Ex 1: Create thread without parameter initialization.
        public void Ex1_CreateThread()
        {
            // Use ThreadStart which takes no parameters
            Thread t = new Thread(new ThreadStart(ThreadMethod));
            t.Start();

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Main thread: Do some work.");
                Thread.Sleep(0); // force context switch
            }

            // Wait until t finishes.
            t.Join();
        }

        private void ThreadMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadProc: {0}", i);
                Thread.Sleep(0); // force context switch
            }
        }

        // Ex 2: Create thread with parameter initialization.
        public void Ex2_CreateParamThread()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadMethod));
            t.Start(5); // pass parameter (boxed to object)

            t.Join();
        }

        private void ThreadMethod(object o)
        {
            for (int i = 0; i < (int)o; i++)
            {
                Console.WriteLine("ThreadProc: {0}", i);
                Thread.Sleep(0); // force context switch
            }
        }

        // Ex 3: Use a shared variable to stop a thread.
        public void Ex3_StopThread()
        {
            bool stopped = false;

            // Use lambda expression so local variable can be accessed.
            Thread t = new Thread(() =>
            {
                int i = 0;

                while (!stopped)
                {
                    Console.WriteLine("Running {0}...", i++);
                    Thread.Sleep(1000);
                }
            });

            t.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            stopped = true;
            t.Join(); // blocks until finished
        }

        // Ex 4: Use the ThreadStatic attribute so that each thread gets its own copy of a static field.
        [ThreadStatic]
        private static int _field;
        
        public void Ex4_ThreadStatic()
        {

            new Thread(() =>
            {
                for (int x = 0; x < 10; x++)
                {
                    _field++;
                    Console.WriteLine("Thread A: {0}", _field);
                }
            }).Start();


            new Thread(() =>
            {
                for (int x = 0; x < 10; x++)
                {
                    _field++;
                    Console.WriteLine("Thread B: {0}", _field);
                }
            }).Start();
        }
        
        // Ex 5: Use ThreadLocal<T> to make data local and initialized to a thread.
        private ThreadLocal<int> _field2 =
            new ThreadLocal<int>(() =>
            {
                return Thread.CurrentThread.ManagedThreadId;
            });

        public void Ex5_ThreadStaticInit()
        {
            new Thread(() =>
            {
                Console.WriteLine("Thread A ID: {0}", Thread.CurrentThread.ManagedThreadId);

                for (int x = 0; x < _field2.Value; x++)
                {
                    Console.WriteLine("Thread A: {0}", x);
                }

            }).Start();

            new Thread(() =>
            {
                Console.WriteLine("Thread B ID: {0}", Thread.CurrentThread.ManagedThreadId);

                for (int x = 0; x < _field2.Value; x++)
                {
                    Console.WriteLine("Thread B: {0}", x);
                }
            }).Start();
        }

    }
}
