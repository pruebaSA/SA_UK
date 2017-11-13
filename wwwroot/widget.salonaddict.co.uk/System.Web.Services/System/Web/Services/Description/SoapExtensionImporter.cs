namespace System.Web.Services.Description
{
    using System;
    using System.CodeDom;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    public abstract class SoapExtensionImporter
    {
        private SoapProtocolImporter protocolImporter;

        protected SoapExtensionImporter()
        {
        }

        public abstract void ImportMethod(CodeAttributeDeclarationCollection metadata);

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

