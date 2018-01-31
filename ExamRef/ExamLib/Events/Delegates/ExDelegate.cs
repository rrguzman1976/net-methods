using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Events.Delegates
{
    public class ExDelegate
    {
        // Ex 1: Create delegate type
        private delegate int Calculate(int x, int y); // delegate type

        private int Add(int x, int y) { return x + y; } // match signature
        private int Multiply(int x, int y) { return x * y; } // match signature

        public void Ex1_UseDelegate()
        {
            Calculate calc = this.Add; // automatically instantiate and assign
            Console.WriteLine(calc(3, 4)); // invoke just like a method

            calc = this.Multiply; // re-assign
            Console.WriteLine(calc(3, 4)); // Displays 12

            Calculate calc2 = new Calculate((x, y) => x - y); // manual instantiation and assign
            Console.WriteLine(calc2(4, 3)); // Displays 1

            calc2 = (x, y) => x / y;
            Console.WriteLine(calc2(4, 2)); // Displays 2
        }

        // Ex 2: Multicast delegates (function group)
        private delegate int Del(); // action
        private int Method2() 
        { 
            Console.WriteLine("MethodTwo");
            return 2;
        }

        public void Ex2_Multicast()
        {
            Del d = new Del(() =>
            {
                Console.WriteLine("MethodOne");
                return 1;
            });

            d = d + this.Method2;
            d += () =>
            {
                Console.WriteLine("MethodThree");
                return 3;
            };

            Console.WriteLine("Count: {0}", d.GetInvocationList().GetLength(0));
            d -= this.Method2; // removes delegate

            Console.WriteLine("Count: {0}", d.GetInvocationList().GetLength(0));
            
            int r = d(); // invokes group

            Console.WriteLine("Return: {0}", r);
        }

        // Ex 3: Covariance: return type more derived than delegate type
        private class Base : Object { }
        private class Derived : Base { }

        private delegate Base CovarianceDel();

        private Derived MethodCovar() 
        {
            Console.WriteLine("MethodCovar");
            return new Derived(); 
        }

        public void Ex3_Covariance()
        {
            CovarianceDel del = MethodCovar;
            
            Base d = del(); // returns delegate type
        }

        // Ex 4: Contravariance: parameter type less derived than delegate type
        private delegate void ContravarianceDel(Derived tw);

        private void MethodContra(Base tw) 
        {
            Console.WriteLine("MethodContra");
        }

        public void Ex4_Contravariance()
        {
            ContravarianceDel del = MethodContra;

            del(new Derived()); // requires delegate type
        }

        // Ex 5: Use delegate to control access to a private method
        private void privateMethod()
        {
            Console.WriteLine("privateMethod access allowed.");
        }

        public Action Ex5_ReturnDelegate(string pw)
        {
            if (pw.CompareTo("hello world!") == 0)
            {
                Action callback = () => privateMethod();

                return callback;
            }
            else
            {
                throw new InvalidOperationException("Access is not authorized!");
            }
        }
    }
}
