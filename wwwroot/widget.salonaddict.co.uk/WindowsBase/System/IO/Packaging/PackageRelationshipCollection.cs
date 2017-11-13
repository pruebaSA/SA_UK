namespace System.IO.Packaging
{
    using MS.Internal.IO.Packaging;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class PackageRelationshipCollection : IEnumerable<PackageRelationship>, IEnumerable
    {
        private string _filter;
        private InternalRelationshipCollection _relationships;

        internal PackageRelationshipCollection(InternalRelationshipCollection relationships, string filter)
        {
            this._relationships = relationships;
            this._filter = filter;
        }

        public IEnumerator<PackageRelationship> GetEnumerator()
        {
            List<PackageRelationship>.Enumerator enumerator = this._relationships.GetEnumerator();
            if (this._filter == null)
            {
                return enumerator;
            }
            return new FilteredEnumerator(enumerator, this._filter);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        private sealed class FilteredEnumerator : IEnumerator<PackageRelationship>, IDisposable, IEnumerator
        {
            private IEnumerator<PackageRelationship> _enumerator;
            private string _filter;

            internal FilteredEnumerator(IEnumerator<PackageRelationship> enumerator, string filter)
            {
                this._enumerator = enumerator;
                this._filter = filter;
            }

            public void Dispose()
            {
                this._enumerator.Dispose();
            }

            private bool RelationshipTypeMatches() => 
                (string.CompareOrdinal(this._enumerator.Current.RelationshipType, this._filter) == 0);

            bool IEnumerator.MoveNext()
            {
                while (this._enumerator.MoveNext())
                {
                    if (this.RelationshipTypeMatches())
                    {
                        return true;
                    }
                }
                return false;
            }

            void IEnumerator.Reset()
            {
                this._enumerator.Reset();
            }

            public PackageRelationship Current =>
                this._enumerator.Current;

            object IEnumerator.Current =>
                this._enumerator.Current;
        }
    }
}

