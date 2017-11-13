namespace System.Runtime.InteropServices
{
    using System;

    [ComVisible(true), AttributeUsage(AttributeTargets.Module, Inherited=false)]
    public sealed class DefaultCharSetAttribute : Attribute
    {
        internal System.Runtime.InteropServices.CharSet _CharSet;

        public DefaultCharSetAttribute(System.Runtime.InteropServices.CharSet charSet)
        {
            this._CharSet = charSet;
        }

        public System.Runtime.InteropServices.CharSet CharSet =>
            this._CharSet;
    }
}

