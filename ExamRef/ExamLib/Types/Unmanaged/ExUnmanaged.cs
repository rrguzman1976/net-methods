using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.Unmanaged
{
    public class Point31 
    { 
        public int x;
        public int y; 
    }

    public unsafe struct MyBuffer
    {
        public fixed char fixedBuffer[128];
    }

    // unsafe keyword on the enclosing class is optional when methods 
    // and blocks are declared unsafe as appropriate.
    public /*unsafe*/ class ExUnmanaged 
    {
        public MyBuffer myBuffer = default(MyBuffer);

        public void Ex1_Unsafe()
        {
            ExUnmanaged myC = new ExUnmanaged();

            unsafe
            {
                // Pin the buffer to a fixed location in memory.
                fixed (char* charPtr = myC.myBuffer.fixedBuffer)
                {
                    *charPtr = 'A';

                    Console.WriteLine(*charPtr);
                }
            }
        }

        public void Ex2_Unsafe()
        {
            // Normal pointer to an object.
            int[] a = new int[5] { 10, 20, 30, 40, 50 };

            // Must be in unsafe code to use interior pointers.
            unsafe
            {
                // Must pin object on heap so that it doesn't move while using interior pointers.
                fixed (int* p = a) // or &a[0]
                {
                    // p is pinned as well as object, so create another pointer to show incrementing it.
                    int* p2 = p;
                    Console.WriteLine(*p2);
            
                    // Incrementing p2 bumps the pointer by four bytes due to its type ...
                    p2 += 1;
                    Console.WriteLine(*p2);
                    
                    p2 += 1;
                    
                    Console.WriteLine(*p2);
                    Console.WriteLine("--------");
                    Console.WriteLine(*p);
                    
                    // Deferencing p and incrementing changes the value of a[0] ...
                    *p += 1;
                    Console.WriteLine(*p);
                    *p += 1;
                    Console.WriteLine(*p);
                }
            }
        }

        public unsafe void Ex3_Unsafe()
        {
            Point31 point = new Point31();
            double[] arr = { 7, 1.5, 2.3, 3.4, 4.0, 5.9 };
            string str = "Hello World";

            // The following two assignments are equivalent. Each assigns the address
            // of the first element in array arr to pointer p.

            // You can initialize a pointer by using an array.
            fixed (double* p = arr) 
            {
                Console.WriteLine(*p);

                double* p2 = p + 1;

                Console.WriteLine(*p2);
            }

            // You can initialize a pointer by using the address of a variable. 
            fixed (double* p = &arr[0]) { /*...*/ }

            // The following assignment initializes p by using a string.
            fixed (char* p = str) { /*...*/ }

            // The following assignment is not valid, because str[0] is a char, 
            // which is a value, not a variable.
            //fixed (char* p = &str[0]) { /*...*/ } 


            // You can initialize a pointer by using the address of a variable, such
            // as point.x or arr[5].
            fixed (int* p1 = &point.x)
            {
                fixed (double* p2 = &arr[5])
                {
                    // Do something with p1 and p2.
                }
            }
        }

        /*
         * The following example uses pointers to copy bytes from one array to another.
         */
        public static unsafe void CopyArray(byte[] source, int sourceOffset, byte[] target,
                int targetOffset, int count)
        {
            // The following fixed statement pins the location of the source and
            // target objects in memory so that they will not be moved by garbage
            // collection.
            fixed (byte* pSource = source, pTarget = target)
            {
                // Set the starting points in source and target for the copying.
                byte* ps = pSource + sourceOffset;
                byte* pt = pTarget + targetOffset;

                // Copy the specified number of bytes from source to target.
                for (int i = 0; i < count; i++)
                {
                    *pt = *ps;
                    pt++;
                    ps++;
                }
            }
        }

        // Unsafe method: takes a pointer to an int.
        public unsafe void SquarePtrParam(int* p)
        {
            *p *= *p;
        }
    }
}
