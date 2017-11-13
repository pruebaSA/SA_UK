namespace System.Web.Services.Description
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class SoapTransportImporter
    {
        private SoapProtocolImporter protocolImporter;

        protected SoapTransportImporter()
        {
        }

        public abstract void ImportClass();
        public abstract bool IsSupportedTransport(string transport);

        public SoapProtocolImporter ImportContext
        {
            get => 
                this.protocolImporter;
            set
            {
                this.protocolImporter = value;
            }
        }
    }
}

