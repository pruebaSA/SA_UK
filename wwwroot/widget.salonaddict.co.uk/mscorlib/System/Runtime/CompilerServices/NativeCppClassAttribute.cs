namespace System.Runtime.CompilerServices
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, AttributeUsage(AttributeTargets.Struct, Inherited=true), ComVisible(true)]
    public sealed class NativeCppClassAttribute : Attribute
    {
    }
}

