using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Events
{
    public class Ex1_Event
    {
        // Ex 1: Use an Action to raise an event
        public Action OnChange; // parameter-less Action

        public void Raise()
        {
            // OnChange is null if no subscribers have been added.
            /*
            if (OnChange != null)
            {
                OnChange(); // invokes delegate chain
            }
            */
            // Equivalent to above.
            OnChange?.Invoke();
        }
    }

    public class Ex2_Event
    {
        public event Action OnChangeEvent = delegate { }; // empty delegate eliminates need for null check

        public void Raise2()
        {
            OnChangeEvent();
        }
    }

    public class ExEvent
    {

        public void Ex1_ActionEvent()
        {
            Ex1_Event p = new Ex1_Event();

            // Register listeners
            p.OnChange += () => Console.WriteLine("Event raised to method 1");
            p.OnChange += () => Console.WriteLine("Event raised to method 2");
            //p.OnChange = null; // allowed

            // Raise event
            p.Raise();
            p.OnChange(); // allowed
        }

        // Ex 2: Use event keyword
        
        public void Ex2_EventKeyword()
        {
            Ex2_Event p = new Ex2_Event();

            // Register listeners
            p.OnChangeEvent += () => Console.WriteLine("Event2 raised to method 1");
            p.OnChangeEvent += () => Console.WriteLine("Event2 raised to method 2");
            //p.OnChangeEvent = null; // not allowed

            // Raise event
            p.Raise2();
            //p.OnChangeEvent(); // not allowed
        }

        // Ex 3: Use pre-defined event delegate instead of Action
        public class MyArgs : EventArgs
        {
            public int Value { get; set; }
        }
        
        public event EventHandler<MyArgs> OnChangeDelegate = delegate { };
        
        public void Ex3_EventDelegate()
        {
            ExEvent p = new ExEvent();

            // Register listeners
            p.OnChangeDelegate += (s, a) => Console.WriteLine("Event3 raised to method 1: {0}", a.Value);
            p.OnChangeDelegate += (s, a) => Console.WriteLine("Event3 raised to method 2: {0}", a.Value);

            // Raise event (sender, custom args)
            p.OnChangeDelegate(this, new MyArgs() { Value = 42 });
        }

        // Ex 4: Custom event accessor
        private event EventHandler<MyArgs> onChangeCustom = delegate { };
        public event EventHandler<MyArgs> OnChangeCustom
        {
            add
            {
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

        public void Ex4_EventAccessor()
        {
            ExEvent p = new ExEvent();

            // Register listeners
            p.OnChangeCustom += (s, a) => Console.WriteLine("Event4 raised to method 1: {0}", a.Value);
            p.OnChangeCustom += (s, a) => Console.WriteLine("Event4 raised to method 2: {0}", a.Value);

            // Raise event (sender, custom args)
            p.onChangeCustom(this, new MyArgs() { Value = 42 });
        }

        // Ex 5: Handle subscriber exception
        public event EventHandler OnChangeEx = delegate { };
        private void RaiseEx()
        {
            var exceptions = new List<Exception>();

            foreach (Delegate handler in OnChangeEx.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Caught exception: {0}", ex.Message);
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        public void Ex4_EventException()
        {
            ExEvent p = new ExEvent();

            p.OnChangeEx += (sender, e) => Console.WriteLine("Sub 1 called");

            p.OnChangeEx += (sender, e) => { throw new Exception("Test exception"); };

            p.OnChangeEx += (sender, e) => Console.WriteLine("Sub 3 called");

            try
            {
                p.RaiseEx();
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.InnerExceptions)
                {
                    Console.WriteLine("Exception: {0}", e.InnerException.Message);
                }
            }
        }
    }
}
