namespace MS.Utility
{
    using System;

    internal abstract class FrugalListBase<T>
    {
        protected int _count;

        protected FrugalListBase()
        {
        }

        public abstract FrugalListStoreState Add(T value);
        public abstract void Clear();
        public abstract object Clone();
        public abstract bool Contains(T value);
        public abstract void CopyTo(T[] array, int index);
        public abstract T EntryAt(int index);
        public abstract int IndexOf(T value);
        public abstract void Insert(int index, T value);
        public virtual Compacter<T> NewCompacter(int newCount) => 
            new Compacter<T>((FrugalListBase<T>) this, newCount);

        public abstract void Promote(FrugalListBase<T> newList);
        public abstract bool Remove(T value);
        public abstract void RemoveAt(int index);
        public abstract void SetAt(int index, T value);
        public abstract T[] ToArray();
        internal void TrustedSetCount(int newCount)
        {
            this._count = newCount;
        }

        public abstract int Capacity { get; }

        public int Count =>
            this._count;

        internal class Compacter
        {
            private int _newCount;
            protected int _previousEnd;
            protected FrugalListBase<T> _store;
            protected int _validItemCount;

            public Compacter(FrugalListBase<T> store, int newCount)
            {
                this._store = store;
                this._newCount = newCount;
            }

            public virtual FrugalListBase<T> Finish()
            {
                T local = default(T);
                int index = this._validItemCount;
                int num2 = this._store._count;
                while (index < num2)
                {
                    this._store.SetAt(index, local);
                    index++;
                }
                this._store._count = this._validItemCount;
                return this._store;
            }

            public void Include(int start, int end)
            {
                this.IncludeOverride(start, end);
                this._previousEnd = end;
            }

            protected virtual void IncludeOverride(int start, int end)
            {
                for (int i = start; i < end; i++)
                {
                    this._store.SetAt(this._validItemCount++, this._store.EntryAt(i));
                }
            }
        }
    }
}

