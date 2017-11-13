namespace Microsoft.Contracts
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=false)]
    internal sealed class InvariantMethodAttribute : Attribute
    {
    }
}

