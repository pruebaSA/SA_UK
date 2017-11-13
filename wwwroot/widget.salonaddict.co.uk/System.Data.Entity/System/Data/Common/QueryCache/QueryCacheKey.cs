namespace System.Data.Common.QueryCache
{
    using System;

    internal abstract class QueryCacheKey
    {
        private int _agingIndex;
        private uint _hitCount = 1;
        protected static StringComparison _stringComparison = StringComparison.Ordinal;
        protected const int EstimatedParameterStringSize = 20;

        protected QueryCacheKey()
        {
        }

        public abstract override bool Equals(object obj);
        protected virtual bool Equals(string s, string t) => 
            string.Equals(s, t, _stringComparison);

        public abstract override int GetHashCode();
        internal void UpdateHit()
        {
            if (uint.MaxValue != this._hitCount)
            {
                this._hitCount++;
            }
        }

        internal int AgingIndex
        {
            get => 
                this._agingIndex;
            set
            {
                this._agingIndex = value;
            }
        }

        internal uint HitCount
        {
            get => 
                this._hitCount;
            set
            {
                this._hitCount = value;
            }
        }
    }
}

