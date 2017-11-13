﻿namespace System.ServiceModel.Security
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;

    public sealed class X509CertificateInitiatorClientCredential
    {
        private X509Certificate2 certificate;
        internal const X509FindType DefaultFindType = X509FindType.FindBySubjectDistinguishedName;
        internal const StoreLocation DefaultStoreLocation = StoreLocation.CurrentUser;
        internal const StoreName DefaultStoreName = StoreName.My;
        private bool isReadOnly;

        internal X509CertificateInitiatorClientCredential()
        {
        }

        internal X509CertificateInitiatorClientCredential(X509CertificateInitiatorClientCredential other)
        {
            this.certificate = other.certificate;
            this.isReadOnly = other.isReadOnly;
        }

        internal void MakeReadOnly()
        {
            this.isReadOnly = true;
        }

        public void SetCertificate(string subjectName, StoreLocation storeLocation, StoreName storeName)
        {
            if (subjectName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("subjectName");
            }
            this.SetCertificate(storeLocation, storeName, X509FindType.FindBySubjectDistinguishedName, subjectName);
        }

        public void SetCertificate(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findValue)
        {
            if (findValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("findValue");
            }
            this.ThrowIfImmutable();
            this.certificate = System.ServiceModel.Security.SecurityUtils.GetCertificateFromStore(storeName, storeLocation, findType, findValue, null);
        }

        private void ThrowIfImmutable()
        {
            if (this.isReadOnly)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("ObjectIsReadOnly")));
            }
        }

        public X509Certificate2 Certificate
        {
            get => 
                this.certificate;
            set
            {
                this.ThrowIfImmutable();
                this.certificate = value;
            }
        }
    }
}

