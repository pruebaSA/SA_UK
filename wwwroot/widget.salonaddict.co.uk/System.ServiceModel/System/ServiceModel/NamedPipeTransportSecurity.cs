namespace System.ServiceModel
{
    using System;
    using System.Net.Security;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;

    public sealed class NamedPipeTransportSecurity
    {
        internal const System.Net.Security.ProtectionLevel DefaultProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
        private System.Net.Security.ProtectionLevel protectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

        internal NamedPipeTransportSecurity()
        {
        }

        internal WindowsStreamSecurityBindingElement CreateTransportProtectionAndAuthentication() => 
            new WindowsStreamSecurityBindingElement { ProtectionLevel = this.protectionLevel };

        internal static bool IsTransportProtectionAndAuthentication(WindowsStreamSecurityBindingElement wssbe, NamedPipeTransportSecurity transportSecurity)
        {
            transportSecurity.protectionLevel = wssbe.ProtectionLevel;
            return true;
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
            }
        }
    }
}

