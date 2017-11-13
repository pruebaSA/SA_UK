namespace System.Security
{
    using System;

    [AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=false)]
    public sealed class SecuritySafeCriticalAttribute : Attribute
    {
    }
}

