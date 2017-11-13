﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.IdentityModel.Selectors;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public sealed class X509PeerCertificateAuthenticationElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(X509PeerCertificateAuthentication cert)
        {
            if (cert == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("cert");
            }
            cert.CertificateValidationMode = this.CertificateValidationMode;
            cert.RevocationMode = this.RevocationMode;
            cert.TrustedStoreLocation = this.TrustedStoreLocation;
            if (!string.IsNullOrEmpty(this.CustomCertificateValidatorType))
            {
                Type c = Type.GetType(this.CustomCertificateValidatorType, true);
                if (!typeof(X509CertificateValidator).IsAssignableFrom(c))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidCertificateValidatorType", new object[] { this.CustomCertificateValidatorType, typeof(X509CertificateValidator).ToString() })));
                }
                cert.CustomCertificateValidator = (X509CertificateValidator) Activator.CreateInstance(c);
            }
        }

        public void Copy(X509PeerCertificateAuthenticationElement from)
        {
            if (this.IsReadOnly())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigReadOnly")));
            }
            if (from == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("from");
            }
            this.CertificateValidationMode = from.CertificateValidationMode;
            this.RevocationMode = from.RevocationMode;
            this.TrustedStoreLocation = from.TrustedStoreLocation;
            this.CustomCertificateValidatorType = from.CustomCertificateValidatorType;
        }

        [ConfigurationProperty("certificateValidationMode", DefaultValue=3), ServiceModelEnumValidator(typeof(X509CertificateValidationModeHelper))]
        public X509CertificateValidationMode CertificateValidationMode
        {
            get => 
                ((X509CertificateValidationMode) base["certificateValidationMode"]);
            set
            {
                base["certificateValidationMode"] = value;
            }
        }

        [ConfigurationProperty("customCertificateValidatorType", DefaultValue=""), StringValidator(MinLength=0)]
        public string CustomCertificateValidatorType
        {
            get => 
                ((string) base["customCertificateValidatorType"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["customCertificateValidatorType"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("customCertificateValidatorType", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("certificateValidationMode", typeof(X509CertificateValidationMode), X509CertificateValidationMode.PeerOrChainTrust, null, new ServiceModelEnumValidator(typeof(X509CertificateValidationModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("revocationMode", typeof(X509RevocationMode), X509RevocationMode.Online, null, new StandardRuntimeEnumValidator(typeof(X509RevocationMode)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("trustedStoreLocation", typeof(StoreLocation), StoreLocation.CurrentUser, null, new StandardRuntimeEnumValidator(typeof(StoreLocation)), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [StandardRuntimeEnumValidator(typeof(X509RevocationMode)), ConfigurationProperty("revocationMode", DefaultValue=1)]
        public X509RevocationMode RevocationMode
        {
            get => 
                ((X509RevocationMode) base["revocationMode"]);
            set
            {
                base["revocationMode"] = value;
            }
        }

        [ConfigurationProperty("trustedStoreLocation", DefaultValue=1), StandardRuntimeEnumValidator(typeof(StoreLocation))]
        public StoreLocation TrustedStoreLocation
        {
            get => 
                ((StoreLocation) base["trustedStoreLocation"]);
            set
            {
                base["trustedStoreLocation"] = value;
            }
        }
    }
}

