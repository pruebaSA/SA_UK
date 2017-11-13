﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public class ServiceCredentialsElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        protected internal void ApplyConfiguration(ServiceCredentials behavior)
        {
            if (behavior == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("behavior");
            }
            PropertyInformationCollection properties = base.ElementInformation.Properties;
            if (properties["userNameAuthentication"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.UserNameAuthentication.ApplyConfiguration(behavior.UserNameAuthentication);
            }
            if (properties["windowsAuthentication"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.WindowsAuthentication.ApplyConfiguration(behavior.WindowsAuthentication);
            }
            if (properties["clientCertificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.ClientCertificate.ApplyConfiguration(behavior.ClientCertificate);
            }
            if (properties["serviceCertificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.ServiceCertificate.ApplyConfiguration(behavior.ServiceCertificate);
            }
            if (properties["peer"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.Peer.ApplyConfiguration(behavior.Peer);
            }
            if (properties["issuedTokenAuthentication"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.IssuedTokenAuthentication.ApplyConfiguration(behavior.IssuedTokenAuthentication);
            }
            if (properties["secureConversationAuthentication"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.SecureConversationAuthentication.ApplyConfiguration(behavior.SecureConversationAuthentication);
            }
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            ServiceCredentialsElement element = (ServiceCredentialsElement) from;
            this.ClientCertificate.Copy(element.ClientCertificate);
            this.ServiceCertificate.Copy(element.ServiceCertificate);
            this.UserNameAuthentication.Copy(element.UserNameAuthentication);
            this.WindowsAuthentication.Copy(element.WindowsAuthentication);
            this.Peer.Copy(element.Peer);
            this.IssuedTokenAuthentication.Copy(element.IssuedTokenAuthentication);
            this.SecureConversationAuthentication.Copy(element.SecureConversationAuthentication);
            this.Type = element.Type;
        }

        protected internal override object CreateBehavior()
        {
            ServiceCredentials credentials;
            if (string.IsNullOrEmpty(this.Type))
            {
                credentials = new ServiceCredentials();
            }
            else
            {
                System.Type c = System.Type.GetType(this.Type, true);
                if (!typeof(ServiceCredentials).IsAssignableFrom(c))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidServiceCredentialsType", new object[] { this.Type, c.AssemblyQualifiedName })));
                }
                credentials = (ServiceCredentials) Activator.CreateInstance(c);
            }
            this.ApplyConfiguration(credentials);
            return credentials;
        }

        public override System.Type BehaviorType =>
            typeof(ServiceCredentials);

        [ConfigurationProperty("clientCertificate")]
        public X509InitiatorCertificateServiceElement ClientCertificate =>
            ((X509InitiatorCertificateServiceElement) base["clientCertificate"]);

        [ConfigurationProperty("issuedTokenAuthentication")]
        public IssuedTokenServiceElement IssuedTokenAuthentication =>
            ((IssuedTokenServiceElement) base["issuedTokenAuthentication"]);

        [ConfigurationProperty("peer")]
        public PeerCredentialElement Peer =>
            ((PeerCredentialElement) base["peer"]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("type", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("clientCertificate", typeof(X509InitiatorCertificateServiceElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("serviceCertificate", typeof(X509RecipientCertificateServiceElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("userNameAuthentication", typeof(UserNameServiceElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("windowsAuthentication", typeof(WindowsServiceElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("peer", typeof(PeerCredentialElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("issuedTokenAuthentication", typeof(IssuedTokenServiceElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("secureConversationAuthentication", typeof(SecureConversationServiceElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("secureConversationAuthentication")]
        public SecureConversationServiceElement SecureConversationAuthentication =>
            ((SecureConversationServiceElement) base["secureConversationAuthentication"]);

        [ConfigurationProperty("serviceCertificate")]
        public X509RecipientCertificateServiceElement ServiceCertificate =>
            ((X509RecipientCertificateServiceElement) base["serviceCertificate"]);

        [ConfigurationProperty("type", DefaultValue=""), StringValidator(MinLength=0)]
        public string Type
        {
            get => 
                ((string) base["type"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["type"] = value;
            }
        }

        [ConfigurationProperty("userNameAuthentication")]
        public UserNameServiceElement UserNameAuthentication =>
            ((UserNameServiceElement) base["userNameAuthentication"]);

        [ConfigurationProperty("windowsAuthentication")]
        public WindowsServiceElement WindowsAuthentication =>
            ((WindowsServiceElement) base["windowsAuthentication"]);
    }
}

