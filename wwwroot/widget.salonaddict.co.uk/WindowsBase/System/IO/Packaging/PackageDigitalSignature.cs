namespace System.IO.Packaging
{
    using MS.Internal.IO.Packaging;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Windows;

    public class PackageDigitalSignature
    {
        private bool _alreadyLookedForCertPart;
        private CertificatePart _certificatePart;
        private bool _invalid;
        private PackageDigitalSignatureManager _manager;
        private XmlDigitalSignatureProcessor _processor;
        private ReadOnlyCollection<Uri> _signedParts;
        private ReadOnlyCollection<PackageRelationshipSelector> _signedRelationshipSelectors;

        internal PackageDigitalSignature(PackageDigitalSignatureManager manager, XmlDigitalSignatureProcessor processor)
        {
            this._manager = manager;
            this._processor = processor;
        }

        internal PackageDigitalSignature(PackageDigitalSignatureManager manager, PackagePart signaturePart)
        {
            this._manager = manager;
            this._processor = new XmlDigitalSignatureProcessor(manager, signaturePart, this);
        }

        internal CertificatePart GetCertificatePart()
        {
            if ((this._certificatePart == null) && !this._alreadyLookedForCertPart)
            {
                foreach (PackageRelationship relationship in this.SignaturePart.GetRelationshipsByType(CertificatePart.RelationshipType))
                {
                    if (relationship.TargetMode != TargetMode.Internal)
                    {
                        throw new FileFormatException(System.Windows.SR.Get("PackageSignatureCorruption"));
                    }
                    Uri partUri = PackUriHelper.ResolvePartUri(this.SignaturePart.Uri, relationship.TargetUri);
                    if (this._manager.Package.PartExists(partUri))
                    {
                        this._certificatePart = new CertificatePart(this._manager.Package, partUri);
                        break;
                    }
                }
                this._alreadyLookedForCertPart = true;
            }
            return this._certificatePart;
        }

        public List<string> GetPartTransformList(Uri partName)
        {
            this.ThrowIfInvalidated();
            return this._processor.GetPartTransformList(partName);
        }

        internal void Invalidate()
        {
            this._invalid = true;
        }

        internal void SetCertificatePart(CertificatePart certificatePart)
        {
            this._certificatePart = certificatePart;
        }

        private void ThrowIfInvalidated()
        {
            if (this._invalid)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("SignatureDeleted"));
            }
        }

        public VerifyResult Verify()
        {
            this.ThrowIfInvalidated();
            if (this.Signer == null)
            {
                return VerifyResult.CertificateRequired;
            }
            return this.Verify(this.Signer);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public VerifyResult Verify(X509Certificate signingCertificate)
        {
            this.ThrowIfInvalidated();
            if (signingCertificate == null)
            {
                throw new ArgumentNullException("signingCertificate");
            }
            foreach (Uri uri in this.SignedParts)
            {
                if (!this._manager.Package.PartExists(uri))
                {
                    return VerifyResult.ReferenceNotFound;
                }
            }
            X509Certificate2 signer = signingCertificate as X509Certificate2;
            if (signer == null)
            {
                signer = new X509Certificate2(signingCertificate.Handle);
            }
            if (this._processor.Verify(signer))
            {
                return VerifyResult.Success;
            }
            return VerifyResult.InvalidSignature;
        }

        public System.IO.Packaging.CertificateEmbeddingOption CertificateEmbeddingOption
        {
            get
            {
                this.ThrowIfInvalidated();
                if (this.GetCertificatePart() != null)
                {
                    return System.IO.Packaging.CertificateEmbeddingOption.InCertificatePart;
                }
                if (this.Signer == null)
                {
                    return System.IO.Packaging.CertificateEmbeddingOption.NotEmbedded;
                }
                return System.IO.Packaging.CertificateEmbeddingOption.InSignaturePart;
            }
        }

        public System.Security.Cryptography.Xml.Signature Signature
        {
            get
            {
                this.ThrowIfInvalidated();
                return this._processor.Signature;
            }
            set
            {
                this.ThrowIfInvalidated();
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this._processor.Signature = value;
            }
        }

        public PackagePart SignaturePart
        {
            get
            {
                this.ThrowIfInvalidated();
                return this._processor.SignaturePart;
            }
        }

        public string SignatureType
        {
            get
            {
                this.ThrowIfInvalidated();
                return XmlDigitalSignatureProcessor.ContentType.ToString();
            }
        }

        public byte[] SignatureValue
        {
            get
            {
                this.ThrowIfInvalidated();
                return this._processor.SignatureValue;
            }
        }

        public ReadOnlyCollection<Uri> SignedParts
        {
            get
            {
                this.ThrowIfInvalidated();
                if (this._signedParts == null)
                {
                    this._signedParts = new ReadOnlyCollection<Uri>(this._processor.PartManifest);
                }
                return this._signedParts;
            }
        }

        public ReadOnlyCollection<PackageRelationshipSelector> SignedRelationshipSelectors
        {
            get
            {
                this.ThrowIfInvalidated();
                if (this._signedRelationshipSelectors == null)
                {
                    this._signedRelationshipSelectors = new ReadOnlyCollection<PackageRelationshipSelector>(this._processor.RelationshipManifest);
                }
                return this._signedRelationshipSelectors;
            }
        }

        public X509Certificate Signer
        {
            get
            {
                this.ThrowIfInvalidated();
                return this._processor.Signer;
            }
        }

        public DateTime SigningTime
        {
            get
            {
                this.ThrowIfInvalidated();
                return this._processor.SigningTime;
            }
        }

        public string TimeFormat
        {
            get
            {
                this.ThrowIfInvalidated();
                return this._processor.TimeFormat;
            }
        }
    }
}

