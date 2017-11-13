namespace System.Data.Common.QueryCache
{
    using System;

    internal sealed class CompiledQueryCacheKey : QueryCacheKey
    {
        private readonly Guid _cacheIdentity;

        internal CompiledQueryCacheKey(Guid cacheIdentity)
        {
            this._cacheIdentity = cacheIdentity;
        }

        public override bool Equals(object compareTo)
        {
            if (typeof(CompiledQueryCacheKey) != compareTo.GetType())
            {
                return false;
            }
            return ((CompiledQueryCacheKey) compareTo)._cacheIdentity.Equals(this._cacheIdentity);
        }

        public override int GetHashCode() => 
            this._cacheIdentity.GetHashCode();

        public override string ToString() => 
            this._cacheIdentity.ToString();
    }
}

