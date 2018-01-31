using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    // Ex 1: Implement IEnumerable the hard way
    public class MyNode
    {
        public string Val { get; set; }

        public override string ToString()
        {
            return this.Val;
        }
    }

    // ExIEnumerable is a collection of MyNode.
    public class ExIEnumerable : IEnumerable
    {
        private MyNode[] _data;

        //public ExIEnumerable() { } // don't allow empty set

        public ExIEnumerable(MyNode[] p)
        {
            _data = new MyNode[p.Length];
            Array.Copy(p, _data, p.Length); // copy by value
        }

        public IEnumerator GetEnumerator()
        {
            return new MyNodeEnum(_data);
        }
    }

    public class MyNodeEnum : IEnumerator
    {
        private MyNode[] _data;

        public MyNodeEnum(MyNode[] p)
        {
            _data = p; // copy by reference
        }

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        private int _position = -1;

        // Not strongly typed (no intellisense)
        public object Current
        {
            get
            {
                try
                {
                    return _data[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        // TODO: What if object added to collection during iteration?
        public bool MoveNext()
        {
            _position++;
            return (_position < _data.Length);
        }

        public void Reset()
        {
            _position = -1;
        }
    }

    // Ex 2: Implement IEnumerable using yield
    public class ExIEnumerable2 : IEnumerable
    {
        private MyNode[] _data;

        //public ExIEnumerable() { } // don't allow empty set

        public ExIEnumerable2(MyNode[] p)
        {
            _data = new MyNode[p.Length];
            Array.Copy(p, _data, p.Length); // copy by value
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                yield return _data[i];
            }
        }
    }

    // Ex 3: Implement IEnumerable<T>
    public class ExIEnumerable3 : IEnumerable<MyNode>
    {
        private MyNode[] _data;

        public ExIEnumerable3(MyNode[] p)
        {
            _data = new MyNode[p.Length];
            Array.Copy(p, _data, p.Length); // copy by value
        }

        // Strongly typed (with intellisense)
        public IEnumerator<MyNode> GetEnumerator()
        {
            return new MyNodeEnum2(_data);
        }

        // Explicit, non-generic implementation
        IEnumerator IEnumerable.GetEnumerator() 
        {
            return this.GetEnumerator();
        }
    }

    public class MyNodeEnum2 : IEnumerator<MyNode>
    {
        private MyNode[] _data;

        public MyNodeEnum2(MyNode[] p)
        {
            _data = p; // copy by reference
        }

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        private int _position = -1;

        // Strongly typed (with Intellisense)
        public MyNode Current
        {
            get 
            {
                try
                {
                    return _data[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            _position++;
            return (_position < _data.Length);
        }

        public void Reset()
        {
            _position = -1;
        }

        // IDisposable
        private bool disposedValue = false;
    
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // resources disposed so finalization is unneccessary
        }

        // Allow override in same assembly or derived classes only
        protected internal virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue) // first time Dispose is called
            {
                if (disposing) // called explicitly (user or using)
                {
                    // Dispose of managed resources.
                }
                // Else, managed resources will be released via their own finalization method.
                // Always dispose of unmanaged resources 
            }

            this.disposedValue = true;
        }

        // Finalizer
        ~MyNodeEnum2()
        {
            Dispose(false);
        }
    }

    // Ex 4: Implement IEnumerable<T> using yield
    public class ExIEnumerable4 : IEnumerable<MyNode>
    {
        private MyNode[] _data;

        public ExIEnumerable4(MyNode[] p)
        {
            _data = new MyNode[p.Length];
            Array.Copy(p, _data, p.Length); // copy by value
        }

        // Strongly typed (with intellisense)
        public IEnumerator<MyNode> GetEnumerator()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                yield return _data[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() // explicit
        {
            return this.GetEnumerator();
        }
    }
}
