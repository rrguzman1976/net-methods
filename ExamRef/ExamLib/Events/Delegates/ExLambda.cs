using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Events.Delegates
{
    public class ExLambda
    {
        // Ex 1: Use lambda expression to initialize delegates.
        private Action<int, int> calc; // built in delegate type instance
        private delegate void ActionEquiv<T1, T2>(T1 arg1, T2 arg2); // equivalent to above instance type

        private Func<int, int, double> calc2; // built in delegate type instance

        // equivalent to above instance type (out is implicit)
        private delegate TResult FuncEquiv<T1, T2, /*out*/ TResult>(T1 arg1, T2 arg2); 

        public void Ex1_Lambda()
        {
            calc = (x, y) => Console.WriteLine("Lambda1: {0}", x * y);
            ActionEquiv<int, int> a1 = (x, y) => Console.WriteLine("Lambda2: {0}", x / y);

            calc2 = (x, y) =>
                {
                    double result = Math.Pow(x, y);
                    return result;
                };

            calc(12, 18);
            a1(100, 10);

            Console.WriteLine("Delegate 2: {0}", calc2(10, 2));

        }

        // Ex 2: Create expression tree
        public void Ex2_ExprTree()
        {
            BlockExpression be = Expression.Block(
                Expression.Call(
                    null
                    , typeof(Console).GetMethod("Write", new Type[] { typeof(string) })
                    , Expression.Constant("Hello ")
                ),
                Expression.Call(
                    null
                    , typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) })
                    , Expression.Constant("World!")
                ),
                Expression.Call(
                    null
                    , typeof(Console).GetMethod("WriteLine", new Type[] { typeof(Int32) })
                    , Expression.Constant(7)
                )
            );

            (Expression.Lambda<Action>(be).Compile())();
        }
    }
}
