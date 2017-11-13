namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class FilteredReadOnlyMetadataCollection<TDerived, TBase> : ReadOnlyMetadataCollection<TDerived>, IBaseList<TBase>, IList, ICollection, IEnumerable where TDerived: TBase where TBase: MetadataItem
    {
        private readonly Predicate<TBase> _predicate;
        private readonly ReadOnlyMetadataCollection<TBase> _source;

        internal FilteredReadOnlyMetadataCollection(ReadOnlyMetadataCollection<TBase> collection, Predicate<TBase> predicate) : base(FilteredReadOnlyMetadataCollection<TDerived, TBase>.FilterCollection(collection, predicate))
        {
            this._source = collection;
            this._predicate = predicate;
        }

        public override bool Contains(string identity)
        {
            TBase local;
            return (this._source.TryGetValue(identity, false, out local) && this._predicate(local));
        }

        internal static List<TDerived> FilterCollection(ReadOnlyMetadataCollection<TBase> collection, Predicate<TBase> predicate)
        {
            List<TDerived> list = new List<TDerived>(collection.Count);
            foreach (TBase local in collection)
            {
                if (predicate(local))
                {
                    list.Add((TDerived) local);
                }
            }
            return list;
        }

        public override TDerived GetValue(string identity, bool ignoreCase)
        {
            TBase local = this._source.GetValue(identity, ignoreCase);
            if (!this._predicate(local))
            {
                throw EntityUtil.ItemInvalidIdentity(identity, "identity");
            }
            return (TDerived) local;
        }

        public override int IndexOf(TDerived value)
        {
            TBase local;
            if (this._source.TryGetValue(value.Identity, false, out local) && this._predicate(local))
            {
                return base.IndexOf((TDerived) local);
            }
            return -1;
        }

        bool IList.get_IsReadOnly() => 
            base.IsReadOnly;

        int IBaseList<TBase>.IndexOf(TBase item)
        {
            if (this._predicate(item))
            {
                return this.IndexOf((TDerived) item);
            }
            return -1;
        }

        public override bool TryGetValue(string identity, bool ignoreCase, out TDerived item)
        {
            TBase local;
            item = default(TDerived);
            if (this._source.TryGetValue(identity, ignoreCase, out local) && this._predicate(local))
            {
                item = (TDerived) local;
                return true;
            }
            return false;
        }

        public override TDerived this[string identity]
        {
            get
            {
                TBase local = this._source[identity];
                if (!this._predicate(local))
                {
                    throw EntityUtil.ItemInvalidIdentity(identity, "identity");
                }
                return (TDerived) local;
            }
        }

        TBase IBaseList<TBase>.this[string identity] =>
            ((TBase) this[identity]);

        TBase IBaseList<TBase>.this[int index] =>
            ((TBase) base[index]);
    }
}

