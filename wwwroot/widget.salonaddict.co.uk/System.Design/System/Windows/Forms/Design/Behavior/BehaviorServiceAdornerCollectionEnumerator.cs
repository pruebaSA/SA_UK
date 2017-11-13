namespace System.Windows.Forms.Design.Behavior
{
    using System;
    using System.Collections;

    public class BehaviorServiceAdornerCollectionEnumerator : IEnumerator
    {
        private IEnumerator baseEnumerator;
        private IEnumerable temp;

        public BehaviorServiceAdornerCollectionEnumerator(BehaviorServiceAdornerCollection mappings)
        {
            this.temp = mappings;
            this.baseEnumerator = this.temp.GetEnumerator();
        }

        public bool MoveNext() => 
            this.baseEnumerator.MoveNext();

        public void Reset()
        {
            this.baseEnumerator.Reset();
        }

        bool IEnumerator.MoveNext() => 
            this.baseEnumerator.MoveNext();

        void IEnumerator.Reset()
        {
            this.baseEnumerator.Reset();
        }

        public Adorner Current =>
            ((Adorner) this.baseEnumerator.Current);

        object IEnumerator.Current =>
            this.baseEnumerator.Current;
    }
}

