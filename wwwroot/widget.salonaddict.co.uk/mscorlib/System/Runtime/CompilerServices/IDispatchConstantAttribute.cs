namespace System.Runtime.CompilerServices
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, Inherited=false)]
    public sealed class IDispatchConstantAttribute : CustomConstantAttribute
    {
        public override object Value =>
            new DispatchWrapper(null);
    }
}

