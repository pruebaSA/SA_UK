namespace System.Web.UI
{
    using System;
    using System.Web.Util;

    internal class ScriptKey
    {
        private bool _isInclude;
        private string _key;
        private Type _type;

        internal ScriptKey(Type type, string key) : this(type, key, false)
        {
        }

        internal ScriptKey(Type type, string key, bool isInclude)
        {
            this._type = type;
            if (string.IsNullOrEmpty(key))
            {
                key = null;
            }
            this._key = key;
            this._isInclude = isInclude;
        }

        public override bool Equals(object o)
        {
            ScriptKey key = (ScriptKey) o;
            return (((key._type == this._type) && (key._key == this._key)) && (key._isInclude == this._isInclude));
        }

        public override int GetHashCode() => 
            HashCodeCombiner.CombineHashCodes(this._type.GetHashCode(), this._key.GetHashCode(), this._isInclude.GetHashCode());
    }
}

