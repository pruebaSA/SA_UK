namespace System.Data.Common.QueryCache
{
    using System;
    using System.Data.Objects;

    internal class ShaperFactoryQueryCacheKey<T> : QueryCacheKey
    {
        private readonly string _columnMapKey;
        private readonly bool _isValueLayer;
        private readonly MergeOption _mergeOption;

        internal ShaperFactoryQueryCacheKey(string columnMapKey, MergeOption mergeOption, bool isValueLayer)
        {
            this._columnMapKey = columnMapKey;
            this._mergeOption = mergeOption;
            this._isValueLayer = isValueLayer;
        }

        public override bool Equals(object obj)
        {
            ShaperFactoryQueryCacheKey<T> key = obj as ShaperFactoryQueryCacheKey<T>;
            if (key == null)
            {
                return false;
            }
            return ((this._columnMapKey.Equals(key._columnMapKey, QueryCacheKey._stringComparison) && (this._mergeOption == key._mergeOption)) && (this._isValueLayer == key._isValueLayer));
        }

        public override int GetHashCode() => 
            this._columnMapKey.GetHashCode();
    }
}

