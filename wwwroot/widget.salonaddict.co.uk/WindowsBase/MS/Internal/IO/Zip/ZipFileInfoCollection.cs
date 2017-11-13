namespace MS.Internal.IO.Zip
{
    using System;
    using System.Collections;

    internal class ZipFileInfoCollection : IEnumerable
    {
        private ICollection _zipFileInfoCollection;

        internal ZipFileInfoCollection(ICollection zipFileInfoCollection)
        {
            this._zipFileInfoCollection = zipFileInfoCollection;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this._zipFileInfoCollection.GetEnumerator();
    }
}

