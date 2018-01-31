using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExamRef.ExamLib.Types.ValueType;

namespace ExamRef.ExamLib.Types.ReferenceType
{
    public class ExReferenceType : IBase1
    {
        // Ex 1: Use named and optional arguments
        public void Ex1_Arguments()
        {
            ArgMethod("required param", p3: 21, p2: "Test");
        }

        //
        private void ArgMethod(string p1, string p2 = "Hello World!", int p3 = 7)
        {
            Console.WriteLine("p1: {0}, p2: {1}, p3: {2}", p1, p2, p3);
        }

        // Ex 2: Method overloading
        public void Ex2_Overload()
        {
            String s = OverloadMethod("hello", 7);
        }

        private string OverloadMethod(string p1, string p2)
        {
            Console.WriteLine("string, string");
            return null;
        }

        // Return types cannot differ, only parameters.
        private string OverloadMethod(string p1, int p2)
        {
            Console.WriteLine("string, int");
            return null;
        }

        // Methods that have the same parameters but differ only in optional parameters
        // are ambiguous and are not allowed.
        /*
        private string OverloadMethod(string p1, int p2 = 7)
        {
            Console.WriteLine("string, int");
            return null;
        }
        */

        private string OverloadMethod(string p1, int p2, int p3 = 7)
        {
            Console.WriteLine("string, int, int");
            return null;
        }

        // Ex 3: Extension method on class and interface (see below)

        // Ex 4: Read-only field
        private readonly int _roField1 = 1; // allowed

        public ExReferenceType()
        {
            _roField1 = 21; // allowed
        }

        public void Ex4_ReadOnlyField()
        {
            //_roField1 = 21; // not allowed
            Console.WriteLine("TEST: {0}", _roField1);
        }

        // Ex 5: Constant field
        public class Ex5_Class
        {
            private const int _roField2 = 7;

            public Ex5_Class()
            {
                //_roField2 = 21; // not allowed
            }
        }

        public void Ex5_ReadOnlyField()
        {
            Ex5_Class ec = new Ex5_Class();
        }

        // Ex 6: Define a class indexer
        private string[] _data = new string[10];

        public string this[int i]
        {
            get
            {
                if (i < 0 || i > 9)
                {
                    throw new IndexOutOfRangeException("Index must be between 0 and 9.");
                }
                else
                {
                    return _data[i];
                }
            }
            set
            {

                if (i < 0 || i > 9)
                {
                    throw new IndexOutOfRangeException("Index must be between 0 and 9.");
                }
                else
                {
                    _data[i] = value;
                }
            }
        }

        public void Ex6_Indexer()
        {
            this[7] = "21";
            Console.WriteLine("Index 7: {0}", this[7]);
        }

        // Ex 7: Use a nullable value/primitive type
        private ExValueType.MyFlags? _flags;
        private int? _nullInt;

        public void Ex7_Nullable()
        {
            _flags = ExValueType.MyFlags.Flag2;
            _flags = null;

            _nullInt = 21;
            _nullInt = null;

            Console.WriteLine("_flags: {0}, _nullInt: {1}", _flags ?? ExValueType.MyFlags.Flag1, _nullInt ?? 7);
        }

        // Ex 8: Generic example using method
        public TResult Ex8_Generic<T1, T2, TResult>(T1 p1, T2 p2)
            where TResult: class
            where T1: new()
            where T2: IBase1
        {
            T1 t = new T1();

            Console.WriteLine("T1: {0}", p1.ToString());
            p2.ExtMethod2("hello", "world");

            TResult tr = default(TResult);
            return tr;
        }

        // Ex 9: Boxing/unboxing
        public void Ex9_BoxUnbox()
        {
            int i = 7;
            
            Object o = i; // boxing
            int j = (int)o; // unboxing (cast required)

            Type t = 21.GetType(); // boxing

            Console.WriteLine("Type: {0}", t.ToString());

            IFormattable f = 10; // boxing

            Console.WriteLine("IFormattable: {0}", f.ToString());

        }
    }

    public interface IBase1 { }

    // Class and method must be public static
    public static class Extension1
    {
        public static void ExtMethod1(this ExReferenceType rt, string p1, string p2)
        {
            Console.WriteLine("ExtMethod1: {0}, {1}", p1, p2);
        }
    }

    // Extension method on an interface
    public static class Extension2
    {
        public static void ExtMethod2(this IBase1 rt, string p1, string p2)
        {
            Console.WriteLine("ExtMethod2: {0}, {1}", p1, p2);
        }
    }
}
