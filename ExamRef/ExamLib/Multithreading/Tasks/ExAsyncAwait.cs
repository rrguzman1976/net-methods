using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ExamRef.ExamLib.Multithreading.Tasks
{
    public class ExAsyncAwait
    {
        // Ex 1: Use async/await to free up the main thread for long-running I/O.
        public void Ex1_AsyncAwait()
        {
            // Accessing Result blocks until DownloadContent() completes.
            Console.WriteLine(DownloadContent().Result);
        }

        // Async marks the method as an asynchronous method.
        private static async Task<string> DownloadContent()
        {
            using (HttpClient client = new HttpClient())
            {
                // Await frees up the parent thread.
                string result = await client.GetStringAsync("http://www.microsoft.com");

                return result;
            }
        }

        // Ex 2: Use aysnc/await with simulated long running I/O.
        public void Ex1_AsyncAwait2()
        {
            Console.WriteLine("Call AysncMethod at {0}", DateTime.Now);
            Task<int> t = AsyncMethod(); // async method begins running

            // Do something else
            Console.WriteLine("Main thread free at {0}", DateTime.Now); // async method still running

            // Now, wait until async process finishes
            Console.WriteLine("AsyncMethod.Result: {0} at {1}", t.Result, DateTime.Now); // blocks here

            // Do something else...
        }

        // Equivalent to:
        public void Ex1_AsyncAwait2b()
        {
            Task<int> t = longMethod(); // 

            // Asynchronously hookup continuation.
            t.ContinueWith((s) =>
            {
                Console.WriteLine("AsyncMethod.Result: {0} at {1}", s.Result, DateTime.Now);

                // Do something...

            }, continuationOptions: TaskContinuationOptions.OnlyOnRanToCompletion);

            // Do something else
            Console.WriteLine("Main thread free at {0}", DateTime.Now); // async method still running
        }

        private async Task<int> AsyncMethod()
        {
            // await creates task, hooks up continuation and frees parent thread (main)
            int val = await longMethod(); 
            // blocks here

            return val;
        }

        private Task<int> longMethod()
        {
            Task<int> t = Task.Run(() =>
                {
                    Console.WriteLine("longMethod: Start");
                    Thread.Sleep(7000);
                    return 7;
                });

            return t;
        }

    }
}
