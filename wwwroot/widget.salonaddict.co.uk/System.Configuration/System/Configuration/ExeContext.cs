namespace System.Configuration
{
    using System;

    public sealed class ExeContext
    {
        private string _exePath;
        private ConfigurationUserLevel _userContext;

        internal ExeContext(ConfigurationUserLevel userContext, string exePath)
        {
            this._userContext = userContext;
            this._exePath = exePath;
        }

        public string ExePath =>
            this._exePath;

        public ConfigurationUserLevel UserLevel =>
            this._userContext;
    }
}

