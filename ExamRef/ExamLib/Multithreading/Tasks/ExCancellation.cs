using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ExamRef.ExamLib.Multithreading.Tasks
{
    public class ExCancellation
    {
        // Ex 1: Use CancellationToken on a Task
        public void Ex1_CancellationToken()
        {
            // CancellationTokenSource signals cancel message to token.
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            Task task = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {

                    Console.Write("*");
                    Thread.Sleep(1000);
                }

                Console.WriteLine("Token cancel message received.");

            }, token); // works with or without token parameter

            Console.WriteLine("Press enter to stop the task");
            Console.ReadLine();

            cancellationTokenSource.Cancel(); // signal
            task.Wait(); // redundant / immediately exits

            Thread.Sleep(1000);
            Console.WriteLine("Final state: {0}", task.Status); // RanToCompletion
        }

        // Ex 2: Signal cancellation and change final state
        public void Ex2_OperationCanceledException()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            Task task = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Console.Write("*");
                    Thread.Sleep(1000);
                }

                token.ThrowIfCancellationRequested();

            }, token);

            try
            {
                Console.WriteLine("Press enter to stop the task");
                Console.ReadLine();

                cancellationTokenSource.Cancel();
                task.Wait(); // throws TaskCanceledException
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Exception: {0}", e.InnerExceptions[0].ToString());
            }

            Thread.Sleep(1000);
            Console.WriteLine("Final state: {0}", task.Status); // Canceled
        }

        // Ex 3: Continue on Cancellation
        public void Ex3_ContinueOnCancel()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            Task task = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Console.Write("*");
                    Thread.Sleep(1000);
                }

                throw new TaskCanceledException(); // same as token.ThrowIfCancellationRequested()
                //throw new OperationCanceledException(); // same as token.ThrowIfCancellationRequested()

            }, token);

            Task continueTask = task.ContinueWith((t) =>
            {
                //t.Exception.Handle((e) => true); // t.Exception is always null
                Console.WriteLine("Continuation: You have canceled the task");
            }, TaskContinuationOptions.OnlyOnCanceled);

            Console.WriteLine("Press enter to stop the task. ID 1: {0}", task.Id);
            Console.ReadLine();
            cancellationTokenSource.Cancel(); // signal
            
            try
            {
                task.Wait(); // throws AggregateException
                //continueTask.Wait(); // doesn't throw exception
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                }
            }
            
            Thread.Sleep(1000);
            Console.WriteLine("Final state: {0}", task.Status); // Cancelled
        }

    }
}
