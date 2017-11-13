namespace System.Configuration
{
    using System;
    using System.Security.Permissions;

    public class ConfigurationFileMap : ICloneable
    {
        private string _machineConfigFilename;
        private bool _requirePathDiscovery;

        public ConfigurationFileMap()
        {
            this._machineConfigFilename = ClientConfigurationHost.MachineConfigFilePath;
            this._requirePathDiscovery = true;
        }

        public ConfigurationFileMap(string machineConfigFilename)
        {
            this._machineConfigFilename = machineConfigFilename;
        }

        public virtual object Clone() => 
            new ConfigurationFileMap(this._machineConfigFilename);

        public string MachineConfigFilename
        {
            get
            {
                string path = this._machineConfigFilename;
                if (this._requirePathDiscovery)
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
                }
                return path;
            }
            set
            {
                this._requirePathDiscovery = false;
                this._machineConfigFilename = value;
            }
        }
    }
}

