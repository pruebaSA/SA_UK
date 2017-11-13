namespace System.Web.Services.Protocols
{
    using System;

    public abstract class SoapExtensionAttribute : Attribute
    {
        protected SoapExtensionAttribute()
        {
        }

        public abstract Type ExtensionType { get; }

        public abstract int Priority { get; set; }
    }
}

