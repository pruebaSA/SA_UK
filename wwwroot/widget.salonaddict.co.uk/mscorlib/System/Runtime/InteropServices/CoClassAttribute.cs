namespace System.Runtime.InteropServices
{
    using System;

    [ComVisible(true), AttributeUsage(AttributeTargets.Interface, Inherited=false)]
    public sealed class CoClassAttribute : Attribute
    {
        internal Type _CoClass;

        public CoClassAttribute(Type coClass)
        {
            this._CoClass = coClass;
        }

        public Type CoClass =>
            this._CoClass;
    }
}

