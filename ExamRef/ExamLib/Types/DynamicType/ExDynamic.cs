using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;

namespace ExamRef.ExamLib.Types.DynamicType
{
    public class ExDynamic
    {
        // Ex 1: Use dynamic keyword
        public void Ex1_Dynamic()
        {
            ExDynamic ed = new ExDynamic();
            //ed.Method1("hello", "world"); // compile time check error

            dynamic ed2 = new ExDynamic();
            ed2.Method1("hello", "world"); // no compile time error (runtime error)
        }

        private void Method1(string p1, int p2)
        {
            Console.WriteLine("p1: {0}, p2: {1}", p1, p2);
        }

        // Ex 2: Use DynamicObject
        public void Ex2_DynamicObject()
        {
            dynamic o = new MyDO();
            o.Prop1 = "Hello";
            o.Prop2 = "World!";

            Console.WriteLine("Dynamic: {0} {1}", o.Prop1, o.Prop2);

            o.Print();
            o.Clear();
            o.Print();
        }

        private class MyDO : DynamicObject
        {
            ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();

            // Allow dynamic setters
            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                string name = binder.Name.ToLower();

                return _data.TryAdd(name, value);
            }

            // Allow dynamic getters
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                string name = binder.Name.ToLower();

                return _data.TryGetValue(name, out result);
            }

            // Allow method invocation
            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                // Get ConcurrentDictionary type
                Type dictType = typeof(ConcurrentDictionary<string, object>);
                try
                {
                    // Invoke any member in the type
                    result = dictType.InvokeMember(
                                 binder.Name,
                                 BindingFlags.InvokeMethod,
                                 null, _data, args);
                    return true;
                }
                catch (MissingMethodException e)
                {
                    Console.WriteLine("Exception: {0}", e.Message);
                    result = null;
                    return false;
                }
            }

            public void Print()
            {
                Console.WriteLine(ToString());

                if (_data.Count == 0)
                {
                    Console.WriteLine("No elements in the dictionary.");
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                foreach (var pair in _data)
                {
                    sb.AppendLine(pair.Key + " " + pair.Value);
                }

                return sb.ToString();
            }
        }

        // Ex 3: Use ExpandoObject
        public void Ex3_ExpandoObject()
        {
            dynamic eo = new ExpandoObject();

            // Dynamic setter/getter
            eo.DynamicProp = "Hello World!";
            Console.WriteLine("Dynamic prop: {0}, {1}", eo.DynamicProp, eo.DynamicProp.GetType());

            // Dynamic method
            eo.DynamicProp2 = 10;
            eo.DynamicMethod1 = (Func<int, int>) (i => ++i);

            Console.WriteLine("Pre: {0}, Post: {1}", eo.DynamicProp2, eo.DynamicMethod1(eo.DynamicProp2));
        }
    }
}
