using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    public interface ISample<T>
        where T : class
    {
        T GetResult(); // generic method
        int Prop1 { get; } // property
        event EventHandler Event1; // field
        int this[int i] { get; set; } // indexer
    }

    public class ExClassHierarchy : ISample<object> // internal by default
    {
        // Ex 1: Interface inheritance
        private int _val;
        private int[] _data = new int[10];

        public object GetResult()
        {
            throw new NotImplementedException();
        }

        public int Prop1
        {
            get { throw new NotImplementedException(); }
            // Add mutator
            set
            {
                _val = value;
            }
        }

        public event EventHandler Event1 = delegate { };

        // Must implement both get and set.
        public int this[int i]
        {
            get
            {
                throw new NotImplementedException();
            }
            // Cannot change access modifier to less restrictive
            /*private*/ set
            {
                throw new NotImplementedException();
            }
        }

        // Ex 2: Use base keyword
        public void Ex2_Base()
        {
            ChildA a = new ChildA("hello");
        }

        // Ex 3: Use virtual/override
        public void Ex3_Virtual()
        {
            ChildA a = new ChildA("hello");
            a.Method01(); // no override
            a.Method02(); // override
            a.Prop01 = "test";
            Console.WriteLine("Prop01: {0}", a.Prop01);

            ChildA b = new ChildB();
            b.Method03(); // Sealed
            b[0] = "Hello world";
            Console.WriteLine("Indexer: {0}", b[0]);
            b.OnChangeCustom += (o, e) => { Console.WriteLine("Test override"); };
        }

        // Ex 4: Use new keyword to prevent polymorphism
        public void Ex4_New()
        {
            ParentA a = new ChildB();
            a.Method02(); // calls childA.method2

            ParentA b = new ChildB();
            b.Method02(); // calls childA.method2
        }

        // Ex 5: Abstract class
        public void Ex5_Abstract()
        {
            //AbstractA a = new AbstractA(); // not allowed

            AbstractA b = new ConcreteB();
            b.Method01(); // polymorphic
            b.Method02();
        }
        
        // Ex 6: Constructor chaining
        public void Ex6_Chain()
        {
            AbstractA a = new ConcreteB();
        }
    }

    public abstract class AbstractA
    {
        public AbstractA() // allowed
        {
            Console.WriteLine("AbstractA default constructor");
        }

        public virtual void Method01()
        {
            Console.WriteLine("AbstractA.Method01");
        }

        public abstract void Method02(); // no impl
    }

    public sealed class ConcreteB: AbstractA
    {
        public ConcreteB(): this(null)
        {
            Console.WriteLine("ConcreteB default constructor");
        }

        public ConcreteB(string s)
        {
            Console.WriteLine("ConcreteB non-default constructor: {0}", s);
        }

        public override void Method02()
        {
            Console.WriteLine("ConcreteB.Method01");
        }
    }

    public class ConcreteC //: ConcreteB // not allowed
    {
    }

    public class ParentA
    {
        public ParentA()
        {
            Console.WriteLine("ParentA default constructor");
        }

        public ParentA(string s)
        {
            Console.WriteLine("ParentA constructor: {0}", s);
        }

        public virtual void Method01()
        {
            Console.WriteLine("ParentA.Method01()");
        }

        public virtual void Method02()
        {
            Console.WriteLine("ParentA.Method02()");
        }

        public virtual void Method03()
        {
            Console.WriteLine("ParentA.Method03()");
        }

        private string prop01;
        public virtual string Prop01
        {
            get
            {
                return "Parent A: " + prop01;
            }
            set
            {
                Console.WriteLine("ParentA.Prop01 set");
                prop01 = value;
            }
        }
    }

    public class ChildA : ParentA
    {
        public ChildA()
        // calls ParentA() by default
        {
            Console.WriteLine("ChildA default constructor");
        }

        public ChildA(string s) : base(s)
        {
            Console.WriteLine("ChildA constructor: {0}", s);
        }

        public override void Method02()
        {
            base.Method02();
            Console.WriteLine("ChildA.Method02() override");
        }

        public sealed override void Method03()
        {
            Console.WriteLine("ChildA.Method03() override");
        }

        // Property override
        private string prop01 = null;
        public override string Prop01
        {
            get
            {
                return "Child A: " + prop01;
            }
            /*
            set
            {
                prop01 = value;
            }
             */
        }

        protected string[] _data = new string[10];
        public virtual string this[int i]
        {
            get
            {
                Console.WriteLine("ChildA.Indexer.Get");
                return _data[i];
            }
            set
            {
                Console.WriteLine("ChildA.Indexer.Set");
                _data[i] = value;
            }
        }

        protected event EventHandler onChangeCustom = delegate { };
        public virtual event EventHandler OnChangeCustom
        {
            add
            {
                Console.WriteLine("ChildA.OnChangeCustom.Add");
                lock (onChangeCustom)
                {
                    onChangeCustom += value;
                }
            }
            remove
            {
                lock (onChangeCustom)
                {
                    onChangeCustom -= value;
                }
            }
        }
    }

    public class ChildB : ChildA
    {
        public ChildB()
        // calls ChildA() by default
        {
            Console.WriteLine("ChildB default constructor");
        }

        public ChildB(string s)
            : base(s)
        {
            Console.WriteLine("ChildB constructor: {0}", s);
        }

        public new void Method02()
        {
            //base.Method02(); // allowed
            Console.WriteLine("ChildB.Method02() new");
        }

        // Not allowed
        /*
        public override void Method03()
        {
            Console.WriteLine("ChildA.Method03() override");
        }
         */

        // Override indexer
        public override string this[int i]
        {
            get
            {
                Console.WriteLine("ChildB.Indexer.Get");
                return _data[i];
            }
        }

        // Override event accessor
        public override event EventHandler OnChangeCustom
        {
            add
            {
                Console.WriteLine("ChildB.OnChangeCustom.Add");
                onChangeCustom += value;
            }
            remove
            {
                onChangeCustom -= value;
            }
        }
    }
}
