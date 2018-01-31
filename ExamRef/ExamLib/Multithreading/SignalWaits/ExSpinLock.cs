using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.SignalWaits
{
    public struct Data31
    {
        public string Name { get; set; }
        public double Number { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ExSpinLock
    {
        private readonly int N;
        private Queue<Data31> _queue = new Queue<Data31>();
        private object _lock = new Object();
        private SpinLock _spinlock = new SpinLock();

        public ExSpinLock()
        {
            N = 100;
        }

        public void Ex1_SpinLock()
        {
            // First use a standard lock for comparison purposes.
            //UseLock();
            _queue.Clear();
            UseSpinLock();
        }

        private void UpdateWithSpinLock(Data31 d, int i)
        {
            bool lockTaken = false;
            try
            {
                _spinlock.Enter(ref lockTaken);

                if (!_queue.Contains(d))
                {
                    Thread.Sleep(250);
                    _queue.Enqueue(d);
                }
                else
                    Console.WriteLine("Already exists: {0}", d);
            }
            finally
            {
                if (lockTaken) _spinlock.Exit();
            }
        }

        private void UseSpinLock()
        {

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(
                    () =>
                    {
                        for (int i = 0; i < N; i++)
                        {
                            Console.WriteLine("Task 1: {0}", i.ToString());
                            UpdateWithSpinLock(new Data31() { Name = i.ToString(), Number = i }, i);
                        }
                    },
                    () =>
                    {
                        for (int i = 0; i < N; i++)
                        {
                            Console.WriteLine("Task 2: {0}", i.ToString());
                            UpdateWithSpinLock(new Data31() { Name = i.ToString(), Number = i }, i);
                        }
                    }
                );
            sw.Stop();
            Console.WriteLine("elapsed ms with spinlock: {0}", sw.ElapsedMilliseconds);

            Console.WriteLine("{0}", String.Join(",", _queue));
        }

        private void UpdateWithLock(Data31 d, int i)
        {
            bool lockTaken = false;

            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                _queue.Enqueue(d);
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        private void UseLock()
        {
            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(
                    () =>
                    {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithLock(new Data31() { Name = i.ToString(), Number = i }, i);
                        }
                    },
                    () =>
                    {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithLock(new Data31() { Name = i.ToString(), Number = i }, i);
                        }
                    }
                );
            sw.Stop();
            Console.WriteLine("elapsed ms with lock: {0}", sw.ElapsedMilliseconds);
        }
    }
}
