namespace System.Runtime.CompilerServices
{
    using System;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.All), ComVisible(false)]
    internal sealed class DecoratedNameAttribute : Attribute
    {
        public DecoratedNameAttribute(string decoratedName)
        {
        }
    }
}

