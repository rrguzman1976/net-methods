using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

// Custom attributes

// Ex 2: Use InternalsVisibleToAttribute so that test harness
// can access internal method.
[assembly: InternalsVisibleTo("ExamRef_Test")]

namespace ExamRef.ExamLib.Types.Encapsulation
{
    public class ExEncapsulation: IAPI1, IAPI2
    {
        // Ex 1: Use internal modifier
        internal void Ex1_Internal()
        {
            Console.WriteLine("Current assembly only");
        }

        // Ex 3: Use protected modifier
        protected void Ex3_Protected()
        {
            Console.WriteLine("Protected Ex 3");
        }

        // Ex 4: Use protected internal
        protected internal void Ex4_ProtectedInternal()
        {
            Console.WriteLine("Protected internal Ex 4");
        }

        // Ex 5: Use private
        private void Ex5_Private()
        {
            Console.WriteLine("Private Ex 5");
        }

        public void Ex5b_Private2(ExEncapsulation e)
        {
            e.Ex5_Private();
        }

        // Ex 6: Properties
        public string StringA { get; set; }
        public string StringB { get; private set; }
        public string StringC { get; internal set; }
        public string StringD { get; protected set; }
        public string StringE { get; protected internal set; }

        // Ex 7: Explicit interface implementation
        public void Method1(string p1) // implicit
        {
            Console.WriteLine("Hello world Method1 implicit");
        }
        
        void IAPI1.Method1(string p1)
        {
            Console.WriteLine("Hello world Method1 explicit 1");
        }
        
        void IAPI2.Method1(string p1)
        {
            Console.WriteLine("Hello world Method1 explicit 2");
        }
    }

    public class DerivedA : ExEncapsulation
    {
        public void Ex3b_Protected()
        {
            base.Ex3_Protected();
        }
    }

    public interface IAPI1
    {
        void Method1(string p1);
    }

    public interface IAPI2
    {
        void Method1(string p1);
    }
}
