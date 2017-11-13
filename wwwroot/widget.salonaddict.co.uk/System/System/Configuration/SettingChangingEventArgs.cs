namespace System.Configuration
{
    using System;
    using System.ComponentModel;

    public class SettingChangingEventArgs : CancelEventArgs
    {
        private object _newValue;
        private string _settingClass;
        private string _settingKey;
        private string _settingName;

        public SettingChangingEventArgs(string settingName, string settingClass, string settingKey, object newValue, bool cancel) : base(cancel)
        {
            this._settingName = settingName;
            this._settingClass = settingClass;
            this._settingKey = settingKey;
            this._newValue = newValue;
        }

        public object NewValue =>
            this._newValue;

        public string SettingClass =>
            this._settingClass;

        public string SettingKey =>
            this._settingKey;

        public string SettingName =>
            this._settingName;
    }
}

