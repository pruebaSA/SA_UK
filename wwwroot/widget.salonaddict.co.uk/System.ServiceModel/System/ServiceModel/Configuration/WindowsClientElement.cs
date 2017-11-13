namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public sealed class WindowsClientElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(WindowsClientCredential windows)
        {
            if (windows == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("windows");
            }
            windows.AllowNtlm = this.AllowNtlm;
            windows.AllowedImpersonationLevel = this.AllowedImpersonationLevel;
        }

        public void Copy(WindowsClientElement from)
        {
            if (this.IsReadOnly())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigReadOnly")));
            }
            if (from == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("from");
            }
            this.AllowNtlm = from.AllowNtlm;
            this.AllowedImpersonationLevel = from.AllowedImpersonationLevel;
        }

        [ServiceModelEnumValidator(typeof(TokenImpersonationLevelHelper)), ConfigurationProperty("allowedImpersonationLevel", DefaultValue=2)]
        public TokenImpersonationLevel AllowedImpersonationLevel
        {
            get => 
                ((TokenImpersonationLevel) base["allowedImpersonationLevel"]);
            set
            {
                base["allowedImpersonationLevel"] = value;
            }
        }

        [ConfigurationProperty("allowNtlm", DefaultValue=true)]
        public bool AllowNtlm
        {
            get => 
                ((bool) base["allowNtlm"]);
            set
            {
                base["allowNtlm"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("allowNtlm", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("allowedImpersonationLevel", typeof(TokenImpersonationLevel), TokenImpersonationLevel.Identification, null, new ServiceModelEnumValidator(typeof(TokenImpersonationLevelHelper)), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

