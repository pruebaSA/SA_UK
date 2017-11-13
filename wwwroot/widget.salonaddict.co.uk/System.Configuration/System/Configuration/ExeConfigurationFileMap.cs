namespace System.Configuration
{
    using System;

    public sealed class ExeConfigurationFileMap : ConfigurationFileMap
    {
        private string _exeConfigFilename;
        private string _localUserConfigFilename;
        private string _roamingUserConfigFilename;

        public ExeConfigurationFileMap()
        {
            this._exeConfigFilename = string.Empty;
            this._roamingUserConfigFilename = string.Empty;
            this._localUserConfigFilename = string.Empty;
        }

        private ExeConfigurationFileMap(string machineConfigFilename, string exeConfigFilename, string roamingUserConfigFilename, string localUserConfigFilename) : base(machineConfigFilename)
        {
            this._exeConfigFilename = exeConfigFilename;
            this._roamingUserConfigFilename = roamingUserConfigFilename;
            this._localUserConfigFilename = localUserConfigFilename;
        }

        public override object Clone() => 
            new ExeConfigurationFileMap(base.MachineConfigFilename, this._exeConfigFilename, this._roamingUserConfigFilename, this._localUserConfigFilename);

        public string ExeConfigFilename
        {
            get => 
                this._exeConfigFilename;
            set
            {
                this._exeConfigFilename = value;
            }
        }

        public string LocalUserConfigFilename
        {
            get => 
                this._localUserConfigFilename;
            set
            {
                this._localUserConfigFilename = value;
            }
        }

        public string RoamingUserConfigFilename
        {
            get => 
                this._roamingUserConfigFilename;
            set
            {
                this._roamingUserConfigFilename = value;
            }
        }
    }
}

