namespace System.Security.Authentication.ExtendedProtection
{
    using System;
    using System.Text;

    public class ExtendedProtectionPolicy
    {
        private ChannelBinding customChannelBinding;
        private ServiceNameCollection customServiceNames;
        private System.Security.Authentication.ExtendedProtection.PolicyEnforcement policyEnforcement;
        private System.Security.Authentication.ExtendedProtection.ProtectionScenario protectionScenario;

        public ExtendedProtectionPolicy(System.Security.Authentication.ExtendedProtection.PolicyEnforcement policyEnforcement)
        {
            this.policyEnforcement = policyEnforcement;
            this.protectionScenario = System.Security.Authentication.ExtendedProtection.ProtectionScenario.TransportSelected;
        }

        public ExtendedProtectionPolicy(System.Security.Authentication.ExtendedProtection.PolicyEnforcement policyEnforcement, ChannelBinding customChannelBinding)
        {
            if (policyEnforcement == System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Never)
            {
                throw new ArgumentException(SR.GetString("security_ExtendedProtectionPolicy_UseDifferentConstructorForNever"), "policyEnforcement");
            }
            if (customChannelBinding == null)
            {
                throw new ArgumentNullException("customChannelBinding");
            }
            this.policyEnforcement = policyEnforcement;
            this.protectionScenario = System.Security.Authentication.ExtendedProtection.ProtectionScenario.TransportSelected;
            this.customChannelBinding = customChannelBinding;
        }

        public ExtendedProtectionPolicy(System.Security.Authentication.ExtendedProtection.PolicyEnforcement policyEnforcement, System.Security.Authentication.ExtendedProtection.ProtectionScenario protectionScenario, ServiceNameCollection customServiceNames)
        {
            if (policyEnforcement == System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Never)
            {
                throw new ArgumentException(SR.GetString("security_ExtendedProtectionPolicy_UseDifferentConstructorForNever"), "policyEnforcement");
            }
            if ((customServiceNames != null) && (customServiceNames.Count == 0))
            {
                throw new ArgumentException(SR.GetString("security_ExtendedProtectionPolicy_NoEmptyServiceNameCollection"), "customServiceNames");
            }
            this.policyEnforcement = policyEnforcement;
            this.protectionScenario = protectionScenario;
            this.customServiceNames = customServiceNames;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ProtectionScenario=");
            builder.Append(this.protectionScenario.ToString());
            builder.Append("; PolicyEnforcement=");
            builder.Append(this.policyEnforcement.ToString());
            builder.Append("; CustomChannelBinding=");
            if (this.customChannelBinding == null)
            {
                builder.Append("<null>");
            }
            else
            {
                builder.Append(this.customChannelBinding.ToString());
            }
            builder.Append("; ServiceNames=");
            if (this.customServiceNames == null)
            {
                builder.Append("<null>");
            }
            else
            {
                bool flag = true;
                foreach (string str in this.customServiceNames)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        builder.Append(", ");
                    }
                    builder.Append(str);
                }
            }
            return builder.ToString();
        }

        public ChannelBinding CustomChannelBinding =>
            this.customChannelBinding;

        public ServiceNameCollection CustomServiceNames =>
            this.customServiceNames;

        public System.Security.Authentication.ExtendedProtection.PolicyEnforcement PolicyEnforcement =>
            this.policyEnforcement;

        public System.Security.Authentication.ExtendedProtection.ProtectionScenario ProtectionScenario =>
            this.protectionScenario;
    }
}

