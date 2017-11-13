namespace System.IO.Packaging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class PackagePartCollection : IEnumerable<PackagePart>, IEnumerable
    {
        private SortedList<PackUriHelper.ValidatedPartUri, PackagePart> _partList;

        internal PackagePartCollection(SortedList<PackUriHelper.ValidatedPartUri, PackagePart> partList)
        {
            this._partList = partList;
        }

        public IEnumerator<PackagePart> GetEnumerator() => 
            this._partList.Values.GetEnumerator();

        IEnumerator<PackagePart> IEnumerable<PackagePart>.GetEnumerator() => 
            this.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();
    }
}

