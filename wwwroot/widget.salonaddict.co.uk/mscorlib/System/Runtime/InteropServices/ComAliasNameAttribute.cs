namespace System.Runtime.InteropServices
{
    using System;

    [ComVisible(true), AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, Inherited=false)]
    public sealed class ComAliasNameAttribute : Attribute
    {
        internal string _val;

        public ComAliasNameAttribute(string alias)
        {
            this._val = alias;
        }

        public string Value =>
            this._val;
    }
}

