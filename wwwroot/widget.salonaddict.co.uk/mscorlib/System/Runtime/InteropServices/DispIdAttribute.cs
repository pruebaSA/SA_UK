namespace System.Runtime.InteropServices
{
    using System;

    [ComVisible(true), AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, Inherited=false)]
    public sealed class DispIdAttribute : Attribute
    {
        internal int _val;

        public DispIdAttribute(int dispId)
        {
            this._val = dispId;
        }

        public int Value =>
            this._val;
    }
}

