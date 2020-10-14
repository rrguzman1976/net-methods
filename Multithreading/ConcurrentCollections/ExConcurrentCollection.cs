using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.ConcurrentCollections
{
    public class ExConcurrentCollection
    {
        // Ex 1: Can be used to block calling thread when empty/full.
        public void Ex1_BlockingCollection()
        {
            // Limits upper bound to 2.
            BlockingCollection<string> col = new BlockingCollection<string>(2);

            Task read = Task.Run(() =>
            {
                while (true)
                {
                    // Blocks if empty
                    Console.WriteLine("Take: {0}", col.Take());
                }
            });

            Task write = Task.Run(() =>
            {
                while (true)
                {
                    string s = Console.ReadLine();
            
                    if (string.IsNullOrWhiteSpace(s)) 
                        break;

                    col.Add(s);
                    col.Add(s);
                    col.Add(s); // blocks if full

                    // Signals calling thread to not wait for new items.
                    // Causes InvalidOperationException to be thrown by Take().
                    //col.CompleteAdding();
                }
            });

            write.Wait();
        }

        // Ex 2: GetConsumingEnumerable returns an IEnumerable that blocks until non-empty.
        public void Ex2_GetConsumingEnumerable()
        {
            BlockingCollection<string> col = new BlockingCollection<string>();

            Task read = Task.Run(() =>
            {
                foreach (string v in col.GetConsumingEnumerable())
                {
                    Console.WriteLine(v);
                }
            });

            Task write = Task.Run(() =>
            {
                while (true)
                {
                    string s = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(s))
                        break;

                    col.Add(s);
                }
            });

            write.Wait();
        }

        // Ex 3: ConcurrentBag of items.
        public void Ex3_ConcurrentBag()
        {
            ConcurrentBag<int> bag = new ConcurrentBag<int>();

            bag.Add(42);
            bag.Add(21);

            int result;

            if (bag.TryTake(out result)) // out variable / pass by reference
            {
                Console.WriteLine("TryTake 1: {0}", result);
            }

            if (bag.TryPeek(out result)) // out variable
            {
                Console.WriteLine("There is a next item: {0}", result);

                if (bag.TryTake(out result)) // out variable / pass by reference
                {
                    Console.WriteLine("TryTake 2: {0}", result);
                }
            }
        }

        // Ex 4: Enumerate ConcurrentBag of items.
        public void Ex4_EnumerateConcurrentBag()
        {
            ConcurrentBag<int> bag = new ConcurrentBag<int>();

            Task.Run(() =>
            {
                bag.Add(42);
                Thread.Sleep(1000);
                bag.Add(21);
            });
            
            Task t = Task.Run(() =>
            {
                foreach (int i in bag)
                {
                    Console.WriteLine(i);
                }
            });
            
            t.Wait();

            // Only displays 42 b/c 21 added after snapshot taken.
        }

        // Ex 5: ConcurrentStack of items.
        public void Ex5_ConcurrentStack()
        {
            ConcurrentStack<int> stack = new ConcurrentStack<int>();

            stack.Push(42);

            int result;

            if (stack.TryPop(out result))
            {
                Console.WriteLine("Popped: {0}", result);
            }

            stack.PushRange(new int[] { 1, 2, 3 });

            int[] values = new int[3];

            if (stack.TryPopRange(values) > 0)
            {
                foreach (int i in values)
                {
                    Console.WriteLine("TryPopRange: {0}", i);
                }
            }
        }

        // Ex 6: ConcurrentQueue of items.
        public void Ex6_ConcurrentQueue()
        {
            ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
            queue.Enqueue(42);
            queue.Enqueue(43);

            foreach (int n in queue)
            {
                Console.WriteLine("Enumerate: {0}", n);
            }

            int result;

            if (queue.TryDequeue(out result))
            {
                Console.WriteLine("Dequeued: {0}", result);
            }

            // Dequeued: 42
        }

        // Ex 7: ConcurrentDictionary (key/value pairs) of items.
        public void Ex7_ConcurrentDictionary()
        {
            var dict = new ConcurrentDictionary<string, int>();

            // Returns false if key already exists.
            if (dict.TryAdd("k1", 42))
            {
                Console.WriteLine("Added");
            }

            foreach (KeyValuePair<string, int> kp in dict)
            {
                Console.WriteLine("KeyValuePair: {0}", kp);
            }

            if (dict.TryUpdate("k1", 21, 42))
            {
                Console.WriteLine("42 updated to 21");
            }

            foreach (var kp in dict)
            {
                Console.WriteLine("KeyValuePair: {0}", kp);
            }

            dict["k1"] = 43; // Overwrite unconditionally

            foreach (var kp in dict)
            {
                Console.WriteLine("KeyValuePair: {0}", kp);
            }

            // If k1 exists, update, else add new key.
            int r1 = dict.AddOrUpdate("k1", 3, (k, v) => v * 2);

            foreach (var kp in dict)
            {
                Console.WriteLine("KeyValuePair: {0}", kp);
            }

            // If k2 exists, get, else add new key.
            int r2 = dict.GetOrAdd("k2", 3);

            foreach (var kp in dict)
            {
                Console.WriteLine("KeyValuePair: {0}", kp);
            }
        }
    }
}
