namespace System.Runtime.InteropServices
{
    using System;

    [ComVisible(true), AttributeUsage(AttributeTargets.Interface, Inherited=false)]
    public sealed class InterfaceTypeAttribute : Attribute
    {
        internal ComInterfaceType _val;

        public InterfaceTypeAttribute(short interfaceType)
        {
            this._val = (ComInterfaceType) interfaceType;
        }

        public InterfaceTypeAttribute(ComInterfaceType interfaceType)
        {
            this._val = interfaceType;
        }

        public ComInterfaceType Value =>
            this._val;
    }
}

