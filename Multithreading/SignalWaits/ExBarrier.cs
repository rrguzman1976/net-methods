using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Multithreading.SignalWaits
{
    public class ExBarrier
    {
        private string[] words1 = new string[] { "brown", "jumped", "the", "fox", "quick" };
        private string[] words2 = new string[] { "dog", "lazy", "the", "over" };
        private string solution = "the quick brown fox jumped over the lazy dog.";

        private bool success = false;
        private Barrier barrier; 
        
        public ExBarrier()
        {
            // 2 participants
            barrier = new Barrier(2, (b) =>
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < words1.Length; i++)
                {
                    sb.Append(words1[i]);
                    sb.Append(" ");
                }
                for (int i = 0; i < words2.Length; i++)
                {
                    sb.Append(words2[i]);

                    if (i < words2.Length - 1)
                        sb.Append(" ");
                }
                sb.Append(".");

                Console.CursorLeft = 0;
                Console.Write("Current phase {0}: ", barrier.CurrentPhaseNumber);
                Console.WriteLine(sb.ToString());
                if (String.CompareOrdinal(solution, sb.ToString()) == 0)
                {
                    success = true;
                    Console.WriteLine("\r\nThe solution was found in {0} attempts", barrier.CurrentPhaseNumber);
                }
                // Else, go to next phase and repeat.
            });
        }

        // Use Knuth-Fisher-Yates shuffle to randomly reorder each array.
        // For simplicity, we require that both wordArrays be solved in the same phase.
        // Success of right or left side only is not stored and does not count.       
        private void Solve(string[] wordArray)
        {
            while (success == false)
            {
                Random random = new Random();
                for (int i = wordArray.Length - 1; i > 0; i--)
                {
                    int swapIndex = random.Next(i + 1);
                    string temp = wordArray[i];
                    wordArray[i] = wordArray[swapIndex];
                    wordArray[swapIndex] = temp;
                }

                // We need to stop here to examine results
                // of all thread activity. This is done in the post-phase
                // delegate that is defined in the Barrier constructor.
                barrier.SignalAndWait();
            }
        }

        public void Ex1_Barrier()
        {
            Thread t1 = new Thread(() => Solve(words1));
            Thread t2 = new Thread(() => Solve(words2));
            t1.Start();
            t2.Start();
        }
    }
}
