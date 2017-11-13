namespace System.Configuration
{
    using System;

    internal abstract class Update
    {
        private string _configKey;
        private bool _moved;
        private bool _retrieved;
        private string _updatedXml;

        internal Update(string configKey, bool moved, string updatedXml)
        {
            this._configKey = configKey;
            this._moved = moved;
            this._updatedXml = updatedXml;
        }

        internal string ConfigKey =>
            this._configKey;

        internal bool Moved =>
            this._moved;

        internal bool Retrieved
        {
            get => 
                this._retrieved;
            set
            {
                this._retrieved = value;
            }
        }

        internal string UpdatedXml =>
            this._updatedXml;
    }
}

