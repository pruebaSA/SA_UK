namespace System.ServiceModel
{
    using System;
    using System.Net.Security;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class ServiceContractAttribute : Attribute
    {
        private Type callbackContract;
        private string configurationName;
        private bool hasProtectionLevel;
        private string name;
        private string ns;
        private System.Net.Security.ProtectionLevel protectionLevel;
        private System.ServiceModel.SessionMode sessionMode;

        public Type CallbackContract
        {
            get => 
                this.callbackContract;
            set
            {
                this.callbackContract = value;
            }
        }

        public string ConfigurationName
        {
            get => 
                this.configurationName;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value == string.Empty)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", System.ServiceModel.SR.GetString("SFxConfigurationNameCannotBeEmpty")));
                }
                this.configurationName = value;
            }
        }

        public bool HasProtectionLevel =>
            this.hasProtectionLevel;

        public string Name
        {
            get => 
                this.name;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value == string.Empty)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", System.ServiceModel.SR.GetString("SFxNameCannotBeEmpty")));
                }
                this.name = value;
            }
        }

        public string Namespace
        {
            get => 
                this.ns;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    NamingHelper.CheckUriProperty(value, "Namespace");
                }
                this.ns = value;
            }
        }

        public System.Net.Security.ProtectionLevel ProtectionLevel
        {
            get => 
                this.protectionLevel;
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.protectionLevel = value;
                this.hasProtectionLevel = true;
            }
        }

        public System.ServiceModel.SessionMode SessionMode
        {
            get => 
                this.sessionMode;
            set
            {
                if (!SessionModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.sessionMode = value;
            }
        }
    }
}

