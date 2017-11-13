namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Internal.Materialization;
    using System.Data.Metadata.Edm;

    public sealed class ObjectResult<T> : ObjectResult, IEnumerable<T>, IEnumerable
    {
        private IBindingList _cachedBindingList;
        private DbDataReader _reader;
        private readonly TypeUsage _resultItemType;
        private Shaper<T> _shaper;
        private readonly EntitySet _singleEntitySet;

        internal ObjectResult(Shaper<T> shaper, EntitySet singleEntitySet, TypeUsage resultItemType)
        {
            this._shaper = shaper;
            this._reader = this._shaper.Reader;
            this._singleEntitySet = singleEntitySet;
            this._resultItemType = resultItemType;
        }

        public override void Dispose()
        {
            DbDataReader reader = this._reader;
            this._reader = null;
            if (reader != null)
            {
                reader.Dispose();
            }
            if (this._shaper != null)
            {
                if (this._shaper.Context != null)
                {
                    this._shaper.Context.ReleaseConnection();
                }
                this._shaper = null;
            }
        }

        private void EnsureCanEnumerateResults()
        {
            if (this._shaper == null)
            {
                throw EntityUtil.CannotReEnumerateQueryResults();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            this.EnsureCanEnumerateResults();
            Shaper<T> shaper = this._shaper;
            this._shaper = null;
            return shaper.GetEnumerator();
        }

        internal override IEnumerator GetEnumeratorInternal() => 
            this.GetEnumerator();

        internal override IList GetIListSourceListInternal()
        {
            if (this._cachedBindingList == null)
            {
                this.EnsureCanEnumerateResults();
                bool forceReadOnly = this._shaper.MergeOption == MergeOption.NoTracking;
                this._cachedBindingList = ObjectViewFactory.CreateViewForQuery<T>(this._resultItemType, this, this._shaper.Context, forceReadOnly, this._singleEntitySet);
            }
            return this._cachedBindingList;
        }

        public override Type ElementType =>
            typeof(T);
    }
}

