namespace System.Data.Linq.Mapping
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class ProviderAttribute : Attribute
    {
        private System.Type providerType;

        public ProviderAttribute()
        {
        }

        public ProviderAttribute(System.Type type)
        {
            this.providerType = type;
        }

        public System.Type Type =>
            this.providerType;
    }
}

