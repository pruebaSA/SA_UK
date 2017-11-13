namespace System.Security
{
    using System;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=true, Inherited=false), ComVisible(true)]
    public sealed class SuppressUnmanagedCodeSecurityAttribute : Attribute
    {
    }
}

