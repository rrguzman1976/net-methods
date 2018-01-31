using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.Arrays
{
    public class ExArray
    {
        public void Ex1_ValueArray()
        {
            int[] arrayOfInt = new int[10];

            for (int x = 0; x < arrayOfInt.Length; x++)
            {
                arrayOfInt[x] = x;
            }

            foreach (int i in arrayOfInt)
            {
                Console.Write(i); // Displays 0123456789
            }
        }

        // Object initilization can only be used during construction
        // of an object.
        public void Ex2_ObjectInit()
        {
            int[] arrayOfInt = new int[10]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10
                };

            // Equivalent to:
            int[] arrayOfInt2 = {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10
                };

            foreach (int i in arrayOfInt2)
            {
                Console.Write(i);
            }
        }

        public void ex3_ObjectArray()
        {
            Point[] vector = new Point[10];

            for (int x = 0; x < vector.Length; x++)
            {
                vector[x] = new Point { X = x, Y = x };
            }

            foreach (Point x in vector)
            {
                Console.WriteLine(x);
            }
        }

        public void ex4_ObjectArray()
        {
            Point[] vector = new Point[]
                {
                    new Point(1, 1)
                    , new Point { X = 2, Y = 2}
                    , new Point (3, 3)
                    , new Point() { X = 4, Y = 4}
                };

            foreach (Point x in vector)
            {
                Console.WriteLine(x);
            }
        }

        // Multi-dimensional array
        public void ex5_MultiDimArray()
        {
            int[,,] array3D = new int[4, 3, 2] 
            { 
                { 
                    {1, 4}, 
                    {2, 5},
                    {3, 6}
                }, 
                { 
                    {7, 10}, 
                    {8, 11},
                    {9, 12}
                }, 
                { 
                    {13, 16}, 
                    {14, 17},
                    {15, 18}
                }, 
                { 
                    {19, 22}, 
                    {20, 23},
                    {21, 24}
                }, 
            };

            // Equivalent to:
            int[, ,] array3D2 = // or new int[,,]
            { 
                { 
                    {1, 4}, 
                    {2, 5},
                    {3, 6}
                }, 
                { 
                    {7, 10}, 
                    {8, 11},
                    {9, 12}
                }, 
                { 
                    {13, 16}, 
                    {14, 17},
                    {15, 18}
                }, 
                { 
                    {19, 22}, 
                    {20, 23},
                    {21, 24}
                }, 
            };
            Console.WriteLine(array3D2[0, 0, 0]); // one
            Console.WriteLine(array3D2[0, 1, 0]); // two
            Console.WriteLine(array3D2[0, 2, 0]); // three
            Console.WriteLine(array3D2[0, 0, 1]); // four
            Console.WriteLine(array3D2[0, 1, 1]); // five
            Console.WriteLine(array3D2[0, 2, 1]); // six
            Console.WriteLine(array3D2[1, 0, 0]); // seven
        }

        // Array of arrays.
        public void ex_JaggedArray()
        {
            int[][] jaggedArray = 
                {
                    new int[] {1,3,5,7,9},
                    new int[] {0,2,4,6},
                    new int[] {42,21}
                };

            for (int i = 0; i < jaggedArray.Length ; i++)
            {
                for (int j = 0; j < jaggedArray[i].Length; j++ )
                {
                    Console.WriteLine("Array{0}: {1}", i, jaggedArray[i][j]);
                }
            }
        }
    }

    public class Point
    {
        public Point(): this(0, 0)
        {

        }

        public Point (int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }
        public int Y { get; set;}

        public override string ToString()
        {
            return String.Format("{0}, {1}", X, Y);
        }
    }
}
