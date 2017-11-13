namespace System.Configuration
{
    using System;

    internal sealed class InvalidPropValue
    {
        private ConfigurationException _error;
        private string _value;

        internal InvalidPropValue(string value, ConfigurationException error)
        {
            this._value = value;
            this._error = error;
        }

        internal ConfigurationException Error =>
            this._error;

        internal string Value =>
            this._value;
    }
}

