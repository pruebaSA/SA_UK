namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public class ClientCredentialsElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        protected internal void ApplyConfiguration(ClientCredentials behavior)
        {
            if (behavior == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("behavior");
            }
            PropertyInformationCollection properties = base.ElementInformation.Properties;
            if (properties["windows"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.Windows.ApplyConfiguration(behavior.Windows);
            }
            if (properties["clientCertificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.ClientCertificate.ApplyConfiguration(behavior.ClientCertificate);
            }
            if (properties["serviceCertificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.ServiceCertificate.ApplyConfiguration(behavior.ServiceCertificate);
            }
            if (properties["issuedToken"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.IssuedToken.ApplyConfiguration(behavior.IssuedToken);
            }
            if (properties["httpDigest"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.HttpDigest.ApplyConfiguration(behavior.HttpDigest);
            }
            if (properties["peer"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.Peer.ApplyConfiguration(behavior.Peer);
            }
            behavior.SupportInteractive = this.SupportInteractive;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            ClientCredentialsElement element = (ClientCredentialsElement) from;
            this.ClientCertificate.Copy(element.ClientCertificate);
            this.ServiceCertificate.Copy(element.ServiceCertificate);
            this.Windows.Copy(element.Windows);
            this.IssuedToken.Copy(element.IssuedToken);
            this.HttpDigest.Copy(element.HttpDigest);
            this.Peer.Copy(element.Peer);
            this.SupportInteractive = element.SupportInteractive;
            this.Type = element.Type;
        }

        protected internal override object CreateBehavior()
        {
            ClientCredentials credentials;
            if (string.IsNullOrEmpty(this.Type))
            {
                credentials = new ClientCredentials();
            }
            else
            {
                System.Type c = System.Type.GetType(this.Type, true);
                if (!typeof(ClientCredentials).IsAssignableFrom(c))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidClientCredentialsType", new object[] { this.Type, c.AssemblyQualifiedName })));
                }
                credentials = (ClientCredentials) Activator.CreateInstance(c);
            }
            this.ApplyConfiguration(credentials);
            return credentials;
        }

        public override System.Type BehaviorType =>
            typeof(ClientCredentials);

        [ConfigurationProperty("clientCertificate")]
        public X509InitiatorCertificateClientElement ClientCertificate =>
            ((X509InitiatorCertificateClientElement) base["clientCertificate"]);

        [ConfigurationProperty("httpDigest")]
        public HttpDigestClientElement HttpDigest =>
            ((HttpDigestClientElement) base["httpDigest"]);

        [ConfigurationProperty("issuedToken")]
        public IssuedTokenClientElement IssuedToken =>
            ((IssuedTokenClientElement) base["issuedToken"]);

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
                        new ConfigurationProperty("clientCertificate", typeof(X509InitiatorCertificateClientElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("serviceCertificate", typeof(X509RecipientCertificateClientElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("windows", typeof(WindowsClientElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("issuedToken", typeof(IssuedTokenClientElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpDigest", typeof(HttpDigestClientElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("peer", typeof(PeerCredentialElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("supportInteractive", typeof(bool), true, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("serviceCertificate")]
        public X509RecipientCertificateClientElement ServiceCertificate =>
            ((X509RecipientCertificateClientElement) base["serviceCertificate"]);

        [ConfigurationProperty("supportInteractive", DefaultValue=true)]
        public bool SupportInteractive
        {
            get => 
                ((bool) base["supportInteractive"]);
            set
            {
                base["supportInteractive"] = value;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("type", DefaultValue="")]
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

        [ConfigurationProperty("windows")]
        public WindowsClientElement Windows =>
            ((WindowsClientElement) base["windows"]);
    }
}

