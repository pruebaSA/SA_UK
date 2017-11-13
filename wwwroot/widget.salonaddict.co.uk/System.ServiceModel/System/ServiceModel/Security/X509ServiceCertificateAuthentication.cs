﻿namespace System.ServiceModel.Security
{
    using System;
    using System.IdentityModel.Selectors;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;

    public class X509ServiceCertificateAuthentication
    {
        private X509CertificateValidationMode certificateValidationMode;
        private X509CertificateValidator customCertificateValidator;
        internal const X509CertificateValidationMode DefaultCertificateValidationMode = X509CertificateValidationMode.ChainTrust;
        private static X509CertificateValidator defaultCertificateValidator;
        internal const X509RevocationMode DefaultRevocationMode = X509RevocationMode.Online;
        internal const StoreLocation DefaultTrustedStoreLocation = StoreLocation.CurrentUser;
        private bool isReadOnly;
        private X509RevocationMode revocationMode;
        private StoreLocation trustedStoreLocation;

        internal X509ServiceCertificateAuthentication()
        {
            this.certificateValidationMode = X509CertificateValidationMode.ChainTrust;
            this.revocationMode = X509RevocationMode.Online;
            this.trustedStoreLocation = StoreLocation.CurrentUser;
        }

        internal X509ServiceCertificateAuthentication(X509ServiceCertificateAuthentication other)
        {
            this.certificateValidationMode = X509CertificateValidationMode.ChainTrust;
            this.revocationMode = X509RevocationMode.Online;
            this.trustedStoreLocation = StoreLocation.CurrentUser;
            this.certificateValidationMode = other.certificateValidationMode;
            this.customCertificateValidator = other.customCertificateValidator;
            this.revocationMode = other.revocationMode;
            this.trustedStoreLocation = other.trustedStoreLocation;
            this.isReadOnly = other.isReadOnly;
        }

        internal X509CertificateValidator GetCertificateValidator()
        {
            X509CertificateValidator validator;
            if (!this.TryGetCertificateValidator(out validator))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("MissingCustomCertificateValidator")));
            }
            return validator;
        }

        internal void MakeReadOnly()
        {
            this.isReadOnly = true;
        }

        private void ThrowIfImmutable()
        {
            if (this.isReadOnly)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("ObjectIsReadOnly")));
            }
        }

        internal bool TryGetCertificateValidator(out X509CertificateValidator validator)
        {
            validator = null;
            if (this.certificateValidationMode == X509CertificateValidationMode.None)
            {
                validator = X509CertificateValidator.None;
            }
            else if (this.certificateValidationMode == X509CertificateValidationMode.PeerTrust)
            {
                validator = X509CertificateValidator.PeerTrust;
            }
            else if (this.certificateValidationMode == X509CertificateValidationMode.Custom)
            {
                validator = this.customCertificateValidator;
            }
            else
            {
                bool useMachineContext = this.trustedStoreLocation == StoreLocation.LocalMachine;
                X509ChainPolicy chainPolicy = new X509ChainPolicy {
                    RevocationMode = this.revocationMode
                };
                if (this.certificateValidationMode == X509CertificateValidationMode.ChainTrust)
                {
                    validator = X509CertificateValidator.CreateChainTrustValidator(useMachineContext, chainPolicy);
                }
                else
                {
                    validator = X509CertificateValidator.CreatePeerOrChainTrustValidator(useMachineContext, chainPolicy);
                }
            }
            return (validator != null);
        }

        public X509CertificateValidationMode CertificateValidationMode
        {
            get => 
                this.certificateValidationMode;
            set
            {
                X509CertificateValidationModeHelper.Validate(value);
                this.ThrowIfImmutable();
                this.certificateValidationMode = value;
            }
        }

        public X509CertificateValidator CustomCertificateValidator
        {
            get => 
                this.customCertificateValidator;
            set
            {
                this.ThrowIfImmutable();
                this.customCertificateValidator = value;
            }
        }

        internal static X509CertificateValidator DefaultCertificateValidator
        {
            get
            {
                if (defaultCertificateValidator == null)
                {
                    bool useMachineContext = false;
                    X509ChainPolicy chainPolicy = new X509ChainPolicy {
                        RevocationMode = X509RevocationMode.Online
                    };
                    defaultCertificateValidator = X509CertificateValidator.CreateChainTrustValidator(useMachineContext, chainPolicy);
                }
                return defaultCertificateValidator;
            }
        }

        public X509RevocationMode RevocationMode
        {
            get => 
                this.revocationMode;
            set
            {
                this.ThrowIfImmutable();
                this.revocationMode = value;
            }
        }

        public StoreLocation TrustedStoreLocation
        {
            get => 
                this.trustedStoreLocation;
            set
            {
                this.ThrowIfImmutable();
                this.trustedStoreLocation = value;
            }
        }
    }
}

