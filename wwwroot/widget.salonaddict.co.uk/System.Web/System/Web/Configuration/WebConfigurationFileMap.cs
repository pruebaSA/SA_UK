namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebConfigurationFileMap : ConfigurationFileMap
    {
        private string _site;
        private VirtualDirectoryMappingCollection _virtualDirectoryMapping;

        public WebConfigurationFileMap()
        {
            this._site = string.Empty;
            this._virtualDirectoryMapping = new VirtualDirectoryMappingCollection();
        }

        private WebConfigurationFileMap(string machineConfigFilename, string site, VirtualDirectoryMappingCollection VirtualDirectoryMapping) : base(machineConfigFilename)
        {
            this._site = site;
            this._virtualDirectoryMapping = VirtualDirectoryMapping;
        }

        public override object Clone() => 
            new WebConfigurationFileMap(base.MachineConfigFilename, this._site, this._virtualDirectoryMapping.Clone());

        internal string Site
        {
            get => 
                this._site;
            set
            {
                if (!WebConfigurationHost.IsValidSiteArgument(value))
                {
                    throw System.Web.Util.ExceptionUtil.PropertyInvalid("Site");
                }
                this._site = value;
            }
        }

        public VirtualDirectoryMappingCollection VirtualDirectories =>
            this._virtualDirectoryMapping;
    }
}

