namespace System.ServiceModel
{
    using System;
    using System.Net.Security;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=false)]
    public sealed class MessageContractAttribute : Attribute
    {
        private bool hasProtectionLevel;
        private bool isWrapped = true;
        private System.Net.Security.ProtectionLevel protectionLevel;
        internal const string ProtectionLevelPropertyName = "ProtectionLevel";
        private string wrappedName;
        private string wrappedNs;

        public bool HasProtectionLevel =>
            this.hasProtectionLevel;

        public bool IsWrapped
        {
            get => 
                this.isWrapped;
            set
            {
                this.isWrapped = value;
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

        public string WrapperName
        {
            get => 
                this.wrappedName;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value == string.Empty)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", System.ServiceModel.SR.GetString("SFxWrapperNameCannotBeEmpty")));
                }
                this.wrappedName = value;
            }
        }

        public string WrapperNamespace
        {
            get => 
                this.wrappedNs;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    NamingHelper.CheckUriProperty(value, "WrapperNamespace");
                }
                this.wrappedNs = value;
            }
        }
    }
}

