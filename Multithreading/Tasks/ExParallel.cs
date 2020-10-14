using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.Tasks
{
    public class ExParallel
    {
        // Ex 1: Use the Parallel class to split work concurrently.
        public void Ex1_ParallelFor()
        {
            Parallel.For(0, 10, i =>
            {
                Thread.Sleep(1000);

                Console.WriteLine("For: {0}", i);
            });

            var numbers = Enumerable.Range(0, 10);
            Parallel.ForEach(numbers, i =>
            {
                Thread.Sleep(1000);

                Console.WriteLine("ForEach: {0}", i);
            });
        }

        // Ex 2: Use the Break/Stop to cancel work.
        public void Ex2_ParallelBreak()
        {
            ParallelLoopResult result = Parallel.
                For(0, 1000, (int i, ParallelLoopState loopState) =>
                {
                    Console.WriteLine("For: {0}", i);

                    if (i == 10)
                    {
                        Console.WriteLine("Breaking loop");
                        loopState.Break(); // wait for all iterations currently running to stop
                        //loopState.Stop(); // stop immediately
                    }

                    return;
                });

            Console.WriteLine("IsCompleted: {0}, LowestBreakIteration: {1}", result.IsCompleted, result.LowestBreakIteration);
        }
    }
}
