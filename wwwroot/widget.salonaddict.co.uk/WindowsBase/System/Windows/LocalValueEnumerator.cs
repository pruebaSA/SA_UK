namespace System.Windows
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LocalValueEnumerator : IEnumerator
    {
        private int _index;
        private LocalValueEntry[] _snapshot;
        private int _count;
        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool Equals(object obj)
        {
            LocalValueEnumerator enumerator = (LocalValueEnumerator) obj;
            return (((this._count == enumerator._count) && (this._index == enumerator._index)) && (this._snapshot == enumerator._snapshot));
        }

        public static bool operator ==(LocalValueEnumerator obj1, LocalValueEnumerator obj2) => 
            obj1.Equals(obj2);

        public static bool operator !=(LocalValueEnumerator obj1, LocalValueEnumerator obj2) => 
            !(obj1 == obj2);

        public LocalValueEntry Current
        {
            get
            {
                if (this._index == -1)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("LocalValueEnumerationReset"));
                }
                if (this._index >= this.Count)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("LocalValueEnumerationOutOfBounds"));
                }
                return this._snapshot[this._index];
            }
        }
        object IEnumerator.Current =>
            this.Current;
        public bool MoveNext()
        {
            this._index++;
            return (this._index < this.Count);
        }

        public void Reset()
        {
            this._index = -1;
        }

        public int Count =>
            this._count;
        internal LocalValueEnumerator(LocalValueEntry[] snapshot, int count)
        {
            this._index = -1;
            this._count = count;
            this._snapshot = snapshot;
        }
    }
}

