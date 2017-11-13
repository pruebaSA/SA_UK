namespace System.Configuration
{
    using System;

    public class SettingsLoadedEventArgs : EventArgs
    {
        private SettingsProvider _provider;

        public SettingsLoadedEventArgs(SettingsProvider provider)
        {
            this._provider = provider;
        }

        public SettingsProvider Provider =>
            this._provider;
    }
}

