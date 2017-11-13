namespace System.Runtime.CompilerServices
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, Inherited=false)]
    public abstract class CustomConstantAttribute : Attribute
    {
        protected CustomConstantAttribute()
        {
        }

        public abstract object Value { get; }
    }
}

