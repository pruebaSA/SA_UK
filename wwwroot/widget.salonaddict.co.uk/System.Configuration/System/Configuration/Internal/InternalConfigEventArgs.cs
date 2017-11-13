namespace System.Configuration.Internal
{
    using System;

    public sealed class InternalConfigEventArgs : EventArgs
    {
        private string _configPath;

        public InternalConfigEventArgs(string configPath)
        {
            this._configPath = configPath;
        }

        public string ConfigPath
        {
            get => 
                this._configPath;
            set
            {
                this._configPath = value;
            }
        }
    }
}

