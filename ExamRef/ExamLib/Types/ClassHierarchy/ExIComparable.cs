using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    // Ex 1: Implement IComparable
    public class ExIComparable : IComparable
    {
        public ExIComparable() : this(0)
        { }

        public ExIComparable(int val)
        {
            this.Val = val;
        }

        public int Val { get; set; }

        public int CompareTo(object obj) // non-generic interface
        {
            ExIComparable tmp = obj as ExIComparable;

            if (tmp == null)
                throw new ArgumentException("Invalid object in CompareTo");

            return this.Val < tmp.Val ? -1 :
                this.Val > tmp.Val ? 1 : 0;
        }

        public override string ToString()
        {
            return Val.ToString();
        }
    }

    // Ex 2: Implement IComparable<T> so that ExIComparable2 can be sorted. 
    public class ExIComparable2 : IComparable<ExIComparable2>, IEquatable<ExIComparable2>
    {
        public int Val { get; set; }

        // strongly typed
        public int CompareTo(ExIComparable2 other)
        {
            return this.Val.CompareTo(other.Val);
        }

        public static bool operator >(ExIComparable2 operand1, ExIComparable2 operand2)
        {
            return operand1.CompareTo(operand2) == 1;
        }

        public static bool operator <(ExIComparable2 operand1, ExIComparable2 operand2)
        {
            return operand1.CompareTo(operand2) == -1;
        }

        // Define the is greater than operator.
        public static bool operator >=(ExIComparable2 operand1, ExIComparable2 operand2)
        {
            return operand1.CompareTo(operand2) == 1
                || operand1.CompareTo(operand2) == 0;
        }

        // Define the is less than operator.
        public static bool operator <=(ExIComparable2 operand1, ExIComparable2 operand2)
        {
            return operand1.CompareTo(operand2) == -1
                || operand1.CompareTo(operand2) == 0;
        }

        public override string ToString()
        {
            return Val.ToString();
        }

        // Strongly typed
        public bool Equals(ExIComparable2 other)
        {
            return this.CompareTo(other) == 0;
        }
    }
}
