namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;

    internal sealed class DummyDataSource : ICollection, IEnumerable
    {
        private int dataItemCount;

        internal DummyDataSource(int dataItemCount)
        {
            this.dataItemCount = dataItemCount;
        }

        public void CopyTo(Array array, int index)
        {
            IEnumerator enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                array.SetValue(enumerator.Current, index++);
            }
        }

        public IEnumerator GetEnumerator() => 
            new DummyDataSourceEnumerator(this.dataItemCount);

        public int Count =>
            this.dataItemCount;

        public bool IsSynchronized =>
            false;

        public object SyncRoot =>
            this;

        private class DummyDataSourceEnumerator : IEnumerator
        {
            private int count;
            private int index;

            public DummyDataSourceEnumerator(int count)
            {
                this.count = count;
                this.index = -1;
            }

            public bool MoveNext()
            {
                this.index++;
                return (this.index < this.count);
            }

            public void Reset()
            {
                this.index = -1;
            }

            public object Current =>
                null;
        }
    }
}

