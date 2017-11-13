namespace Microsoft.Contracts
{
    using System;

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    internal sealed class PureAttribute : Attribute
    {
    }
}

