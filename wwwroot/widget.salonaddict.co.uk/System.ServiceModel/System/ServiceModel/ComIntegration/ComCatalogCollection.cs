namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;

    internal class ComCatalogCollection
    {
        private ICatalogCollection catalogCollection;

        public ComCatalogCollection(ICatalogCollection catalogCollection)
        {
            this.catalogCollection = catalogCollection;
        }

        public Enumerator GetEnumerator() => 
            new Enumerator(this);

        public ComCatalogObject Item(int index) => 
            new ComCatalogObject((ICatalogObject) this.catalogCollection.Item(index), this.catalogCollection);

        public int Count =>
            this.catalogCollection.Count();

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator
        {
            private ComCatalogCollection collection;
            private ComCatalogObject current;
            private int count;
            public Enumerator(ComCatalogCollection collection)
            {
                this.collection = collection;
                this.current = null;
                this.count = -1;
            }

            public ComCatalogObject Current =>
                this.current;
            public bool MoveNext()
            {
                this.count++;
                if (this.count >= this.collection.Count)
                {
                    return false;
                }
                this.current = this.collection.Item(this.count);
                return true;
            }

            public void Reset()
            {
                this.count = -1;
            }
        }
    }
}

