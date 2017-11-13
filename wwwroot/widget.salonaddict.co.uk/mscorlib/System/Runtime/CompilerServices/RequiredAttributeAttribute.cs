namespace System.Runtime.CompilerServices
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public sealed class RequiredAttributeAttribute : Attribute
    {
        private Type requiredContract;

        public RequiredAttributeAttribute(Type requiredContract)
        {
            this.requiredContract = requiredContract;
        }

        public Type RequiredContract =>
            this.requiredContract;
    }
}

