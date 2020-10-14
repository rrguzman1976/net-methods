using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ExamRef.ExamLib.Multithreading.Synchronize
{
    public class ExSynchronize
    {
        // Ex 1: Use lock to synchronize access to data
        public void Ex1_Lock()
        {
            int n = 0;

            object _lock = new object();

            var up = Task.Run(() =>
            {

                for (int i = 0; i < 1000000; i++)
                {
                    lock (_lock)
                    {
                        n++;
                    }
                }
            });

            for (int i = 0; i < 1000000; i++)
            {
                lock (_lock)
                {
                    n--;
                }
            }

            up.Wait();
            Console.WriteLine(n);
        }

        // Ex 2: Manufacture a deadlock
        public void Ex2_Deadlock()
        {
            object lockA = new object();
            object lockB = new object();

            var up = Task.Run(() =>
            {
                lock (lockA)
                {
                    Console.WriteLine("Locked A");
                    Thread.Sleep(1000);
                
                    lock (lockB)
                    {
                        Console.WriteLine("Locked A and B");
                    }
                }
            });

            lock (lockB)
            {
                Console.WriteLine("Locked B");
                Thread.Sleep(1000);

                lock (lockA)
                {
                    Console.WriteLine("Locked B and A");
                }
            }

            up.Wait(5000);
            Console.WriteLine("Deadlocked?");
        }

        // Ex 3: Use the volatile keyword
        public void Ex3_Volatile()
        {
            Task.Run(() =>
                {
                    Thread1();
                });

            Task t2 = new Task(() =>
            {
                Thread2();
            });

            t2.Start();
            t2.Wait();
        }

        private static int _flag = 0;
        private static volatile int _value = 0; // disable compiler optimizations

        public static void Thread1()
        {
            // These lines could be switched by the compiler.
            _value = 5;
            _flag = 1;
        }

        public static void Thread2()
        {
            if (_flag == 1)
                Console.WriteLine(_value);
        }

        // Ex 4: Use the Interlocked class
        public void Ex4_Interlocked()
        {
            int n = 0;

            var up = Task.Run(() =>
            {
                for (int i = 0; i < 1000000; i++)
                {
                    Interlocked.Increment(ref n);
                }
            });

            for (int i = 0; i < 1000000; i++)
            {
                Interlocked.Decrement(ref n);
            }

            up.Wait();
            Console.WriteLine(n);
        }

        // Ex 5: Use the Interlocked exchange methods.
        public void Ex5_InterlockedCompare()
        {
            int value = 1;

            Task t1 = Task.Run(() =>
            {
                // Use the following to exchange atomically.
                int prev = Interlocked.CompareExchange(ref value, 2, 1);

                // Same as the following:
                /*
                if (value == 1)
                {
                    value = 2;
                }
                */
                Console.WriteLine(value); // Displays 2
            });

            Thread.Sleep(500);

            Task t2 = Task.Run(() =>
            {
                int prev = Interlocked.Exchange(ref value, 3);
            });

            Task.WaitAll(t1, t2);
            Console.WriteLine(value); // Displays 2
        }
    }
}
