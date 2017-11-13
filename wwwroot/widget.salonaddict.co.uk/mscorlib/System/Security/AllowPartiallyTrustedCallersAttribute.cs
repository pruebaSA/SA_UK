namespace System.Security
{
    using System;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false, Inherited=false), ComVisible(true)]
    public sealed class AllowPartiallyTrustedCallersAttribute : Attribute
    {
    }
}

