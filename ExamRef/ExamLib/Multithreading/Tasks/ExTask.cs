using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.Tasks
{
    public class ExTask
    {
        // Ex 1
        public void Ex1_CreateTask()
        {
            Task t = Task.Run(() =>
            {
                for (int x = 0; x < 100; x++)
                {
                    Console.Write('*');
                }
            });

            t.Wait(); // equivalent to Thread.Join
        }

        // Ex 2: Task<T> can be used to return a result.
        public void Ex2_CreateTaskT()
        {
            Task<int> t = Task.Run(() =>
            {
                return 42;
            });
            
            // Equivalent to Task.Wait (blocks)
            Console.WriteLine(t.Result); // Displays 42
        }

        // Ex 3: ContinueWith can be used to continue an operation when a task
        // finishes.
        public void Ex3_Continuation()
        {
            Task<int> t = Task.Run(() =>
            {
                Console.WriteLine("Base task");
                return 42;
            });

            t.ContinueWith((i) =>
            {
                Console.WriteLine("Canceled");
            }, TaskContinuationOptions.OnlyOnCanceled);

            t.ContinueWith((i) =>
            {
                Console.WriteLine("Faulted");
            }, TaskContinuationOptions.OnlyOnFaulted);

            var completedTask = t.ContinueWith((i) =>
            {
                Console.WriteLine("Completed");
                
                return i.Result * 2;

            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            Console.WriteLine("First: {0}", t.Result); // Displays 42, then calls completedTask
            Console.WriteLine("Second: {0}", completedTask.Result); // Displays 84
        }

        // Ex 4: Attach child tasks to a parent task.
        public void Ex4_ChildTasks()
        {
            Task<Int32[]> parent = Task.Run(() =>
            {
                var results = new Int32[3];
        
                new Task(() => results[0] = 0, 
                    TaskCreationOptions.AttachedToParent).Start();
                
                new Task(() => results[1] = 1, 
                    TaskCreationOptions.AttachedToParent).Start();
                
                new Task(() => results[2] = 2, 
                    TaskCreationOptions.AttachedToParent).Start();

                return results;
            });

            var finalTask = parent.ContinueWith(parentTask => 
            {
                foreach (int i in parentTask.Result)
                {
                    Console.WriteLine(i);
                }
            });

            finalTask.Wait();
        }

        // Ex 5: Example 4 re-written using TaskFactory.
        public void Ex5_TaskFactory()
        {
            Task<Int32[]> parent = Task.Run(() =>
            {
                var results = new Int32[3];

                TaskFactory tf = new TaskFactory(TaskCreationOptions.AttachedToParent,
                   TaskContinuationOptions.ExecuteSynchronously);

                tf.StartNew(() => results[0] = 0);
                tf.StartNew(() => results[1] = 1);
                tf.StartNew(() => results[2] = 2);

                return results;
            });

            var finalTask = parent.ContinueWith(parentTask =>
            {
                foreach (int i in parentTask.Result)
                {
                    Console.WriteLine(i);
                }
            });

            finalTask.Wait();
        }

        // Ex 6: Use WaitAll to wait for all tasks to finish.
        public void Ex6_WaitAll()
        {
            Task<Int32>[] tasks = new Task<Int32>[3];

            tasks[0] = Task.Run(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("1");
                return 1;
            });
            
            tasks[1] = Task.Run(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("2");
                return 2;
            });

            tasks[2] = Task.Run(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("3");
                return 3;
            });

            Task.WaitAll(tasks);
        }

        // Ex 7: Use WaitAny to wait for the first task to finish.
        public void Ex7_WaitAny()
        {
            Task<int>[] tasks = new Task<int>[3];

            tasks[0] = Task.Run(() => { Thread.Sleep(2000); return 1; });
            tasks[1] = Task.Run(() => { Thread.Sleep(1000); return 2; });
            tasks[2] = Task.Run(() => { Thread.Sleep(3000); return 3; });

            while (tasks.Length > 0)
            {
                int i = Task.WaitAny(tasks); // returns array index of completed task
                Task<int> completedTask = tasks[i];

                Console.WriteLine(completedTask.Result);

                var temp = tasks.ToList();
                temp.RemoveAt(i);
                tasks = temp.ToArray();
            }
        }

        // Ex 8: Use WaitAny with timeout.
        public void Ex8_WaitAnyTO()
        {
            Task longRunning = Task.Run(() =>
            {
                Thread.Sleep(10000);
                Console.WriteLine("longRunning Task");
            });

            int index = Task.WaitAny(new[] { longRunning }, 1000);

            if (index == -1)
            {
                Console.WriteLine("Task timed out");
            }

            // Does not cancel task
            longRunning.Wait();
            Console.WriteLine("Task complete");
        }
    }
}
