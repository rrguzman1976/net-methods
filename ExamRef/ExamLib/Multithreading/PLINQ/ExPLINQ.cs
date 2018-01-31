using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.PLINQ
{
    public class ExPLINQ
    {
        // Ex 1: AsParallel
        public void Ex1_AsParallel()
        {
            var numbers = Enumerable.Range(0, 20);

            var parallelResult = numbers.AsParallel()
                .Where(i => i % 2 == 0)
                .ToArray();

            // Unordered result
            foreach (var n in parallelResult)
            {
                Console.WriteLine("parallelResult: {0}", n);
            }
        }

        // Ex 2: Order the results
        public void Ex2_AsOrdered()
        {
            var numbers = Enumerable.Range(0, 20);

            var parallelResult = numbers.AsParallel().AsOrdered()
                .Where(i => i % 2 == 0)
                .ToArray();

            // Ordered result
            foreach (var n in parallelResult)
            {
                Console.WriteLine("parallelResult Ordered: {0}", n);
            }
        }

        // Ex 3: Use AsSequential to force a query to be processed sequentially.
        public void Ex3_AsSequential()
        {
            var numbers = Enumerable.Range(0, 2000);

            var parallelResult = numbers.AsParallel().AsOrdered()
                .Where(i => i % 2 == 0)
                .AsSequential();

            // Use AsSequential to make sure that the Take method doesn’t mess up your order.
            foreach (int i in parallelResult.Take(5))
                Console.WriteLine(i);
        }

        // Ex 4: Use ForAll to iterate over a collection in parallel.
        public void Ex4_ForAll()
        {
            var numbers = Enumerable.Range(0, 20);

            var parallelResult = numbers.AsParallel()
                .Where(i => i % 2 == 0);

            parallelResult.ForAll(e => Console.WriteLine(e));
        }

        // Ex 5: AggregateException wraps all exceptions during parallel processing.
        public void Ex5_AggregateException()
        {
            var numbers = Enumerable.Range(0, 20);

            try
            {

                var parallelResult = numbers.AsParallel()
                .Where((i) => 
                {
                    if (i % 10 == 0) 
                        throw new ArgumentException("i");

                    return i % 2 == 0;
                });

                // LINQ query is not executed until result is iterated.
                parallelResult.ForAll(n => Console.WriteLine(n));
            }
            catch (AggregateException e)
            {
                Console.WriteLine("There where {0} exceptions",
                                    e.InnerExceptions.Count);
            }
        }
    }
}
