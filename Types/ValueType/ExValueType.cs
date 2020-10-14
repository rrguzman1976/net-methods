using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ValueType
{
    public class ExValueType
    {
        // Ex 1: Enumeration
        [Flags]
        public enum MyFlags : byte
        {
            Flag1 = 1
            , Flag2 = 2
            , Flag3 = 4
            , Flag4 = 8
        }

        public void Ex1_Enum()
        {
            MyFlags f1 = MyFlags.Flag1 | MyFlags.Flag2 | MyFlags.Flag3;

            if ((f1 & MyFlags.Flag1) == MyFlags.Flag1)
            {
                Console.WriteLine("Flag f1: {0}", f1);
            }

            if ((f1 & MyFlags.Flag2) == MyFlags.Flag2)
            {
                Console.WriteLine("Flag f2: {0}", f1);
            }

            if ((f1 & MyFlags.Flag3) == MyFlags.Flag3)
            {
                Console.WriteLine("Flag f3: {0}", f1);
            }

            if ((f1 & MyFlags.Flag4) == MyFlags.Flag4)
            {
                Console.WriteLine("Flag f4: {0}", f1);
            }

            Console.WriteLine("Flag raw: {0}", (byte)f1);
        }

        // Ex 2: Structure
        public struct XPoint
        {
            // Default constructor not allowed
            // public XPoint() { } 
            public XPoint(Int64 x, Int64 y)
            {
                _x = x;
                _y = y;
            }

            // Fields
            private Int64 _x, _y;

            // Properties
            public Int64 X
            {
                get { return _x; }
                set { _x = value; }
            }

            public Int64 Y
            {
                get { return _y; }
                set { _y = value; }
            }

            // Methods
            public double Distance(XPoint to)
            {
                double d = Math.Sqrt(Math.Pow(to.X - this.X, 2) + Math.Pow(to.Y - this.Y, 2));
                return d;
            }
        }

        // Inheritance not allowed
        //public struct ZPoint : XPoint { }

        public void Ex2_Struct()
        {
            XPoint p1 = new XPoint(1, 1);
            XPoint p2 = new XPoint(2, 2);

            PassByValue(p2); // pass by value not reference
            p1 = PassByValue(p2);

            Console.WriteLine("Distance: {0}", p2.Distance(p1));
        }

        private XPoint PassByValue(XPoint p)
        {
            p.X = 7;
            p.Y = 7;
            return p;
        }

        // Ex 3: Initialize properties in a struct requires chaining to default constructor.
        public struct Num31
        {
            public Num31(int num, string test)
                : this()
            {
                Num = num;
                Test = test;
            }

            int _num;
            public int Num
            {
                get
                {
                    return _num;
                }
                set
                {
                    _num = value;
                }
            }

            public string Test { get; set; }
        }

    }
}
