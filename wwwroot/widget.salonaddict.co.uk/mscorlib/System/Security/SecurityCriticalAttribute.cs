namespace System.Security
{
    using System;

    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Interface | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Module | AttributeTargets.Assembly, AllowMultiple=false, Inherited=false)]
    public sealed class SecurityCriticalAttribute : Attribute
    {
        internal SecurityCriticalScope _val;

        public SecurityCriticalAttribute()
        {
        }

        public SecurityCriticalAttribute(SecurityCriticalScope scope)
        {
            this._val = scope;
        }

        public SecurityCriticalScope Scope =>
            this._val;
    }
}

