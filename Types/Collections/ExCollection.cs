using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.Collections
{
    public class ExCollection
    {
        public void Ex1_Queue()
        {
            Queue<string> myQueue = new Queue<string>();
            myQueue.Enqueue("Hello");
            myQueue.Enqueue("World");
            myQueue.Enqueue("From");
            myQueue.Enqueue("A");
            myQueue.Enqueue("Queue");

            foreach (string s in myQueue) // foreach does not dequeue
            {
                Console.Write(s + " ");
            }

            Console.WriteLine(Environment.NewLine + "Queue count {0}", myQueue.Count);

            while (myQueue.Count > 0)
            {
                Console.Write(myQueue.Dequeue() + " ");
            }

            Console.WriteLine(Environment.NewLine + "Queue count {0}", myQueue.Count);
        }

        public void Ex2_Stack()
        {
            Stack<string> myStack = new Stack<string>();
            myStack.Push("Hello");
            myStack.Push("World");
            myStack.Push("From");
            myStack.Push("A");
            myStack.Push("Queue");

            foreach (string s in myStack)
            {
                Console.Write(s + " ");
            }

            Console.WriteLine(Environment.NewLine + "Queue count {0}", myStack.Count);

            while (myStack.Count > 0)
            {
                Console.Write(myStack.Pop() + " ");
            }

            Console.WriteLine(Environment.NewLine + "Queue count {0}", myStack.Count);

        }
    }
}
