namespace System.Web.Caching
{
    using System;

    internal class CacheKey
    {
        protected byte _bits;
        private int _hashCode;
        protected string _key;
        protected const byte BitPublic = 0x20;

        internal CacheKey(string key, bool isPublic)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            this._key = key;
            if (isPublic)
            {
                this._bits = 0x20;
            }
        }

        public override int GetHashCode()
        {
            if (this._hashCode == 0)
            {
                this._hashCode = this._key.GetHashCode();
            }
            return this._hashCode;
        }

        internal bool IsPublic =>
            ((this._bits & 0x20) != 0);

        internal string Key =>
            this._key;
    }
}

