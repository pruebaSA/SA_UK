namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SettingsDescriptionAttribute : Attribute
    {
        private readonly string _desc;

        public SettingsDescriptionAttribute(string description)
        {
            this._desc = description;
        }

        public string Description =>
            this._desc;
    }
}

