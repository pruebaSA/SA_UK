namespace System.Configuration
{
    using System;

    internal class StreamInfo
    {
        private string _configSource;
        private bool _isMonitored;
        private string _sectionName;
        private string _streamName;
        private object _version;

        private StreamInfo()
        {
        }

        internal StreamInfo(string sectionName, string configSource, string streamName)
        {
            this._sectionName = sectionName;
            this._configSource = configSource;
            this._streamName = streamName;
        }

        internal StreamInfo Clone() => 
            new StreamInfo { 
                _sectionName = this._sectionName,
                _configSource = this._configSource,
                _streamName = this._streamName,
                _isMonitored = this._isMonitored,
                _version = this._version
            };

        internal string ConfigSource =>
            this._configSource;

        internal bool IsMonitored
        {
            get => 
                this._isMonitored;
            set
            {
                this._isMonitored = value;
            }
        }

        internal string SectionName =>
            this._sectionName;

        internal string StreamName =>
            this._streamName;

        internal object Version
        {
            get => 
                this._version;
            set
            {
                this._version = value;
            }
        }
    }
}

