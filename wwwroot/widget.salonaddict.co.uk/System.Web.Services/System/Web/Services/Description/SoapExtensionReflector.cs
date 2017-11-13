namespace System.Web.Services.Description
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class SoapExtensionReflector
    {
        private ProtocolReflector protocolReflector;

        protected SoapExtensionReflector()
        {
        }

        public virtual void ReflectDescription()
        {
        }

        public abstract void ReflectMethod();

        public ProtocolReflector ReflectionContext
        {
            get => 
                this.protocolReflector;
            set
            {
                this.protocolReflector = value;
            }
        }
    }
}

