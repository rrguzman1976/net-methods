using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    public class ExWeakReference
    {
        // Show usage of WeakReference
        public void Ex1_WeakReference()
        {
            // Create the cache.
            int cacheSize = 50;
            Random r = new Random();
            ExCache c = new ExCache(cacheSize);

            string DataName = "";

            // Collect only most recently created objects
            // Comment out to show no weak references collected.
            GC.Collect(0); 

            // Randomly access objects in the cache.
            for (int i = 0; i < c.Count; i++)
            {
                int index = r.Next(c.Count);

                // Access the object by getting a property value.
                DataName = c[index].Name;
            }

            // Show results.
            double regenPercent = c.RegenerationCount / (double)c.Count;
            Console.WriteLine("Cache size: {0}, Regenerated: {1:P2}%", c.Count, regenPercent);
        }
    }

    public class ExCache
    {
        // Dictionary to contain the cache.
        private static ConcurrentDictionary<int, WeakReference> _cache;

        // Track the number of times an object is regenerated.
        private int regenCount = 0;

        public ExCache(int count)
        {
            _cache = new ConcurrentDictionary<int, WeakReference>();

            // Add objects with a short weak reference to the cache.
            for (int i = 0; i < count; i++)
            {
                // Short weak reference: Use false to retain object up until GC (finalizer called)
                // Long weak reference: Use true to retain object after GC finalizer called but before
                // final object removal.
                _cache.TryAdd(i, new WeakReference(new Data(i), false));
            }
        }

        // Number of items in the cache.
        public int Count
        {
            get { return _cache.Count; }
        }

        // Number of times an object needs to be regenerated.
        public int RegenerationCount
        {
            get { return regenCount; }
        }

        // Retrieve a data object from the cache.
        public Data this[int index]
        {
            get
            {
                WeakReference wr;

                if (_cache.TryGetValue(index, out wr))
                {
                    Data d = wr.Target as Data;

                    if (d == null)
                    {
                        // If the object was reclaimed, generate a new one.
                        Console.WriteLine("Regenerate object at {0}: Yes", index);
                        d = new Data(index);
                        wr.Target = d;
                        regenCount++;
                    }
                    else
                    {
                        // Object was obtained with the weak reference.
                        Console.WriteLine("Regenerate object at {0}: No", index);
                    }

                    return d;
                }
                else
                {
                    throw new InvalidOperationException("Null weak reference.");
                }
            }
        }
    }

    // This class creates byte arrays to simulate data.
    public class Data
    {
        private byte[] _data;

        public Data(int size)
        {
            _data = new byte[size * 1024];
            this.Name = size.ToString();
        }

        // Simple property.
        public string Name { get; set; }
    }
}
