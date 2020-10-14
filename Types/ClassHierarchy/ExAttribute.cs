#define COMPILE_OPTION1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    public class ExAttribute2 : ExAttribute
    {
        public override void Ex3_CustomAttribute(string s)
        {
            Console.WriteLine("Attribute not inherited");
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ExCustomAttribute : Attribute
    {
        public ExCustomAttribute()
        {
            this.Prop1 = "Empty";
        }

        public ExCustomAttribute(string s)
        {
            this.Prop1 = s;
        }

        public string Prop1 { get; set; }
    }

    //[ExCustom("Not allowed")]
    public class ExAttribute
    {
        // Ex 1: Use ConditionalAttribute
        public void Ex1_Conditional()
        {
            // Removing #define will result in method not being called.
            conditionalMethod();
        }

        [Conditional("COMPILE_OPTION1"), Conditional("COMPILE_OPTION2")]
        public void conditionalMethod()
        {
            Console.WriteLine("conditionalMethod()");
        }

        // Ex 2: Get defined attribute using Attribute class
        public void Ex2_Attribute()
        {
            ExAttribute ea = new ExAttribute();
            MemberInfo[] m1 = ea.GetType().GetMember("conditionalMethod");

            if (m1.Length > 0 && Attribute.IsDefined(m1[0], typeof(ConditionalAttribute)))
            {
                Console.WriteLine("ConditionalAttribute exists on {0}", m1[0].ToString());
            }
        }

        // Ex 3: GEt custom Attribute
        [ExCustomAttribute("Method")]
        public virtual void Ex3_CustomAttribute([ExCustom("Param")] string s)
        {
            ExAttribute ea = new ExAttribute();

            MethodInfo m1 = ea.GetType().GetMethod("Ex3_CustomAttribute", new Type[] { typeof(string) });

            if (m1 != null && Attribute.IsDefined(m1, typeof(ExCustomAttribute)))
            {
                ExCustomAttribute a = (ExCustomAttribute) m1.GetCustomAttribute(typeof(ExCustomAttribute));

                Console.WriteLine("ExCustomAttribute defined on Ex3_CustomAttribute: {0}", a.Prop1);
            }
        }

        // Ex 4: Test Inherited flag
        public void Ex4_Inherited()
        {
            ExAttribute2 ea = new ExAttribute2();

            MethodInfo m1 = ea.GetType().GetMethod("Ex3_CustomAttribute", new Type[] { typeof(string) });

            if (m1 != null && Attribute.IsDefined(m1, typeof(ExCustomAttribute)))
            {
                Attribute a = m1.GetCustomAttribute(typeof(ExCustomAttribute));

                Console.WriteLine("ExCustomAttribute defined on Ex4_CustomAttribute: {0}", a.ToString());
            }
        }
    }

}
