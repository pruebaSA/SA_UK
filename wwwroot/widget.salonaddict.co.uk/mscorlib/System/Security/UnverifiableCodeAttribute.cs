namespace System.Security
{
    using System;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Module, AllowMultiple=true, Inherited=false), ComVisible(true)]
    public sealed class UnverifiableCodeAttribute : Attribute
    {
    }
}

