namespace Microsoft.Contracts
{
    using System;

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    internal sealed class ImmutableAttribute : Attribute
    {
    }
}

