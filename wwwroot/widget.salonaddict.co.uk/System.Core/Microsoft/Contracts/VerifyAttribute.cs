namespace Microsoft.Contracts
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly)]
    internal sealed class VerifyAttribute : Attribute
    {
        private bool _value;

        public VerifyAttribute()
        {
            this._value = true;
        }

        public VerifyAttribute(bool value)
        {
            this._value = value;
        }

        public bool Value =>
            this._value;
    }
}

