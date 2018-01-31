using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.SignalWaits
{
    public class ExReaderWriterLockSlim
    {
        public void Ex1_ReaderWriterLockSlim()
        {
            var sc = new SynchronizedCache();
            ManualResetEvent writeFinished = new ManualResetEvent(initialState: false); // unsignaled
            CountdownEvent readyRead = new CountdownEvent(initialCount: 2);

            var tasks = new List<Task>();
            int itemsWritten = 0;

            // Execute a writer.
            tasks.Add(Task.Run(() =>
            {
                String[] vegetables = { "broccoli", "cauliflower",
                                        "carrot", "sorrel", "baby turnip",
                                        "beet", "brussel sprout",
                                        "cabbage", "plantain",
                                        "spinach", "grape leaves",
                                        "lime leaves", "corn",
                                        "radish", "cucumber",
                                        "raddichio", "lima beans" };

                for (int ctr = 1; ctr <= vegetables.Length; ctr++)
                {
                    sc.Add(ctr, vegetables[ctr - 1]); // write lock
                }

                itemsWritten = vegetables.Length;
                Console.WriteLine("Task {0} wrote {1} items\n", Task.CurrentId, itemsWritten);

                // Signal write finished.
                writeFinished.Set();
            }));

            // Execute two readers, one to read from first to last and the second from last to first.
            for (int ctr = 0; ctr <= 1; ctr++)
            {
                bool desc = Convert.ToBoolean(ctr);

                tasks.Add(Task.Run(() =>
                {
                    int start, last, step;
                    int items;
                    
                    // Wait until write complete, then read.
                    writeFinished.WaitOne();

                    String output = String.Empty;
                    items = sc.Count;
                    if (!desc)
                    {
                        start = 1;
                        step = 1;
                        last = items;
                    }
                    else
                    {
                        start = items;
                        step = -1;
                        last = 1;
                    }

                    for (int index = start; desc ? index >= last : index <= last; index += step)
                    {
                        output += String.Format("[{0}] ", sc.Read(index)); // shared read lock
                    }

                    Console.WriteLine("Task {0} read {1} items: {2}\n", Task.CurrentId, items, output);
                    
                    // Signal read finished.
                    readyRead.Signal();
                }));
            }

            tasks.Add(Task.Run(() =>
            {
                // Wait until reads complete.
                readyRead.Wait();

                for (int ctr = 1; ctr <= sc.Count; ctr++)
                {
                    String value = sc.Read(ctr); // shared read lock

                    if (value == "cucumber")
                    {
                        // Enter upgradeable read.
                        if (sc.AddOrUpdate(ctr, "green bean") != SynchronizedCache.AddOrUpdateStatus.Unchanged)
                        {
                            Console.WriteLine("Changed 'cucumber' to 'green bean'");
                        }
                    }
                }

            }));

            // Wait for all tasks to complete.
            Task.WaitAll(tasks.ToArray());

            // Display the final contents of the cache.
            Console.WriteLine();
            Console.WriteLine("Values in synchronized cache: ");
            for (int ctr = 1; ctr <= sc.Count; ctr++)
            {
                Console.WriteLine("   {0}: {1}", ctr, sc.Read(ctr));
            }

            // Cleanup
            sc.Dispose();
            writeFinished.Dispose();
            readyRead.Dispose();
        }
    }

    // Like a ConcurrentDictionary
    public class SynchronizedCache: IDisposable
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private Dictionary<int, string> innerCache = new Dictionary<int, string>();

        public int Count
        { 
            get 
            { 
                return innerCache.Count; 
            } 
        }

        public string Read(int key)
        {
            cacheLock.EnterReadLock(); // multiple readers allowed
            try
            {
                return innerCache[key];
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        public void Add(int key, string value)
        {
            cacheLock.EnterWriteLock(); // only 1 allowed at any time
            try
            {
                innerCache.Add(key, value);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public bool AddWithTimeout(int key, string value, int timeout)
        {
            if (cacheLock.TryEnterWriteLock(timeout)) // only 1 allowed at any time
            {
                try
                {
                    innerCache.Add(key, value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public AddOrUpdateStatus AddOrUpdate(int key, string value)
        {
            cacheLock.EnterUpgradeableReadLock(); // only 1 allowed at any time
            try
            {
                string result = null;
                if (innerCache.TryGetValue(key, out result))
                {
                    if (result == value)
                    {
                        return AddOrUpdateStatus.Unchanged;
                    }
                    else
                    {
                        cacheLock.EnterWriteLock();
                        try
                        {
                            innerCache[key] = value;
                        }
                        finally
                        {
                            cacheLock.ExitWriteLock();
                        }
                        return AddOrUpdateStatus.Updated;
                    }
                }
                else
                {
                    cacheLock.EnterWriteLock();
                    try
                    {
                        innerCache.Add(key, value);
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                    return AddOrUpdateStatus.Added;
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }

        public void Delete(int key)
        {
            cacheLock.EnterWriteLock();
            try
            {
                innerCache.Remove(key);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public enum AddOrUpdateStatus
        {
            Added,
            Updated,
            Unchanged
        };

        // IDisposable pattern
        ~SynchronizedCache()
        {
            //if (cacheLock != null) cacheLock.Dispose();
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing) // allow overrides
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources. 
                    if (cacheLock != null)
                    {
                        cacheLock.Dispose();
                    }
                }

                // Release unmanaged resources only. Managed resources will
                // be called by finalization process.

                _disposed = true;
            }
        }
    }
}
