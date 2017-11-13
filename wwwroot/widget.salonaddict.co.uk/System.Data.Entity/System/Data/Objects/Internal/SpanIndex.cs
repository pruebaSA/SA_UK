namespace System.Data.Objects.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    internal sealed class SpanIndex
    {
        private Dictionary<RowType, TypeUsage> _rowMap;
        private Dictionary<RowType, Dictionary<int, AssociationEndMember>> _spanMap;

        internal SpanIndex()
        {
        }

        internal void AddSpanMap(RowType rowType, Dictionary<int, AssociationEndMember> columnMap)
        {
            if (this._spanMap == null)
            {
                this._spanMap = new Dictionary<RowType, Dictionary<int, AssociationEndMember>>(RowTypeEqualityComparer.Instance);
            }
            this._spanMap[rowType] = columnMap;
        }

        internal void AddSpannedRowType(RowType spannedRowType, TypeUsage originalRowType)
        {
            if (this._rowMap == null)
            {
                this._rowMap = new Dictionary<RowType, TypeUsage>(RowTypeEqualityComparer.Instance);
            }
            this._rowMap[spannedRowType] = originalRowType;
        }

        internal Dictionary<int, AssociationEndMember> GetSpanMap(RowType rowType)
        {
            Dictionary<int, AssociationEndMember> dictionary = null;
            if ((this._spanMap != null) && this._spanMap.TryGetValue(rowType, out dictionary))
            {
                return dictionary;
            }
            return null;
        }

        internal TypeUsage GetSpannedRowType(RowType spannedRowType)
        {
            TypeUsage usage;
            if ((this._rowMap != null) && this._rowMap.TryGetValue(spannedRowType, out usage))
            {
                return usage;
            }
            return null;
        }

        internal bool HasSpanMap(RowType spanRowType) => 
            this._spanMap?.ContainsKey(spanRowType);

        private sealed class RowTypeEqualityComparer : IEqualityComparer<RowType>
        {
            internal static readonly SpanIndex.RowTypeEqualityComparer Instance = new SpanIndex.RowTypeEqualityComparer();

            private RowTypeEqualityComparer()
            {
            }

            public bool Equals(RowType x, RowType y) => 
                (((x != null) && (y != null)) && x.EdmEquals(y));

            public int GetHashCode(RowType obj) => 
                obj.Identity.GetHashCode();
        }
    }
}

