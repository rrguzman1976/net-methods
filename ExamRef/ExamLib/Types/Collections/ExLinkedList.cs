using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.Collections
{
    public class ExLinkedList
    {
        public void Ex1_LinkedList()
        {
            LinkedList<string> ll1 = new LinkedList<string>();

            ll1.AddLast(new LinkedListNode<string>("Hello"));
            ll1.AddLast(new LinkedListNode<string>("world"));

            LinkedListNode<string> n3 = new LinkedListNode<string>("!");
            ll1.AddLast(n3);

            ll1.AddAfter(n3, "!!!");

            Display(ll1);
            DisplayNodes(ll1);
        }

        private void Display(LinkedList<string> list)
        {
            foreach (string s in list)
            {
                Console.WriteLine(s);
            }
        }

        private void DisplayNodes(LinkedList<string> list)
        {
            if (list.Count > 0)
            {
                LinkedListNode<string> first = list.First;

                for (LinkedListNode<string> node = list.First; node != null; node = node.Next)
                {
                    Console.WriteLine("Node Value: {0}", node.Value);
                    Console.WriteLine("\tNode Prev: {0}", node.Previous == null ? "NULL" : node.Previous.Value);
                    Console.WriteLine("\tNode Next: {0}", node.Next == null ? "NULL" : node.Next.Value);
                }
            }
        }
    }
}
