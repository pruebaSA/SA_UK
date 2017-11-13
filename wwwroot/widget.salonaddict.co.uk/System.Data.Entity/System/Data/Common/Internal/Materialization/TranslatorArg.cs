namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct TranslatorArg
    {
        internal readonly Type RequestedType;
        internal TranslatorArg(Type requestedType)
        {
            this.RequestedType = requestedType;
        }
    }
}

