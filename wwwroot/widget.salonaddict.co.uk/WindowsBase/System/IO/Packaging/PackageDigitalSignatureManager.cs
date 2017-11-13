namespace System.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Packaging;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Security.Permissions;
    using System.Windows;
    using System.Xml;

    public sealed class PackageDigitalSignatureManager
    {
        private CertificateEmbeddingOption _certificateEmbeddingOption;
        private System.IO.Packaging.Package _container;
        private static readonly string _defaultHashAlgorithm = "http://www.w3.org/2000/09/xmldsig#sha1";
        private static Uri _defaultOriginPartName = PackUriHelper.CreatePartUri(new Uri("/package/services/digital-signature/origin.psdsor", UriKind.Relative));
        private static readonly string _defaultSignaturePartNameExtension = ".psdsxs";
        private static readonly string _defaultSignaturePartNamePrefix = "/package/services/digital-signature/xml-signature/";
        private static readonly string _guidStorageFormatString = "N";
        private string _hashAlgorithmString = _defaultHashAlgorithm;
        private PackagePart _originPart;
        private static readonly ContentType _originPartContentType = new ContentType("application/vnd.openxmlformats-package.digital-signature-origin");
        private bool _originPartExists;
        private Uri _originPartName = _defaultOriginPartName;
        private static readonly string _originRelationshipType = "http://schemas.openxmlformats.org/package/2006/relationships/digital-signature/origin";
        private bool _originSearchConducted;
        private static readonly string _originToSignatureRelationshipType = "http://schemas.openxmlformats.org/package/2006/relationships/digital-signature/signature";
        private IntPtr _parentWindow;
        private ReadOnlyCollection<PackageDigitalSignature> _signatureList;
        private List<PackageDigitalSignature> _signatures;
        private string _signatureTimeFormat = XmlSignatureProperties.DefaultDateTimeFormat;
        private Dictionary<string, string> _transformDictionary;

        public event InvalidSignatureEventHandler InvalidSignatureEvent;

        public PackageDigitalSignatureManager(System.IO.Packaging.Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }
            this._parentWindow = IntPtr.Zero;
            this._container = package;
            this._transformDictionary = new Dictionary<string, string>(4);
            this._transformDictionary[PackagingUtilities.RelationshipPartContentType.ToString()] = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
            this._transformDictionary[XmlDigitalSignatureProcessor.ContentType.ToString()] = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
        }

        private int CertificatePartReferenceCount(Uri certificatePartUri)
        {
            int num = 0;
            for (int i = 0; i < this._signatures.Count; i++)
            {
                if ((this._signatures[i].GetCertificatePart() != null) && (PackUriHelper.ComparePartUri(certificatePartUri, this._signatures[i].GetCertificatePart().Uri) == 0))
                {
                    num++;
                }
            }
            return num;
        }

        public PackageDigitalSignature Countersign()
        {
            if (!this.IsSigned)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("NoCounterSignUnsignedContainer"));
            }
            X509Certificate certificate = PromptForSigningCertificate(this.ParentWindow);
            if (certificate == null)
            {
                return null;
            }
            return this.Countersign(certificate);
        }

        public PackageDigitalSignature Countersign(X509Certificate certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }
            if (!this.IsSigned)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("NoCounterSignUnsignedContainer"));
            }
            List<Uri> parts = new List<Uri>(this._signatures.Count);
            for (int i = 0; i < this._signatures.Count; i++)
            {
                parts.Add(this._signatures[i].SignaturePart.Uri);
            }
            return this.Sign(parts, certificate);
        }

        public PackageDigitalSignature Countersign(X509Certificate certificate, IEnumerable<Uri> signatures)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }
            if (signatures == null)
            {
                throw new ArgumentNullException("signatures");
            }
            if (!this.IsSigned)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("NoCounterSignUnsignedContainer"));
            }
            foreach (Uri uri in signatures)
            {
                if (!this._container.GetPart(uri).ValidatedContentType.AreTypeAndSubTypeEqual(XmlDigitalSignatureProcessor.ContentType))
                {
                    throw new ArgumentException(System.Windows.SR.Get("CanOnlyCounterSignSignatureParts", new object[] { signatures }));
                }
            }
            return this.Sign(signatures, certificate);
        }

        private bool DeleteCertificateIfReferenceCountBecomesZeroVisitor(PackageRelationship r, object context)
        {
            if (r.TargetMode != TargetMode.Internal)
            {
                throw new FileFormatException(System.Windows.SR.Get("PackageSignatureCorruption"));
            }
            Uri certificatePartUri = PackUriHelper.ResolvePartUri(r.SourceUri, r.TargetUri);
            if (this.CertificatePartReferenceCount(certificatePartUri) == 1)
            {
                this._container.DeletePart(certificatePartUri);
            }
            return true;
        }

        private void DeleteOriginPart()
        {
            try
            {
                this.SafeVisitRelationships(this._container.GetRelationshipsByType(_originRelationshipType), new RelationshipOperation(this.DeleteRelationshipOfTypePackageToOriginVisitor));
                this._container.DeletePart(this._originPartName);
            }
            finally
            {
                this._originPartExists = false;
                this._originSearchConducted = true;
                this._originPart = null;
            }
        }

        private bool DeleteRelationshipOfTypePackageToOriginVisitor(PackageRelationship r, object context)
        {
            if (r.TargetMode != TargetMode.Internal)
            {
                throw new FileFormatException(System.Windows.SR.Get("PackageSignatureCorruption"));
            }
            if (PackUriHelper.ComparePartUri(PackUriHelper.ResolvePartUri(r.SourceUri, r.TargetUri), this._originPartName) == 0)
            {
                this._container.DeleteRelationship(r.Id);
            }
            return true;
        }

        private bool DeleteRelationshipToSignature(PackageRelationship r, object signatureUri)
        {
            Uri secondPartUri = signatureUri as Uri;
            if (r.TargetMode != TargetMode.Internal)
            {
                throw new FileFormatException(System.Windows.SR.Get("PackageSignatureCorruption"));
            }
            if (PackUriHelper.ComparePartUri(PackUriHelper.ResolvePartUri(r.SourceUri, r.TargetUri), secondPartUri) == 0)
            {
                this.OriginPart.DeleteRelationship(r.Id);
            }
            return true;
        }

        private void EnsureSignatures()
        {
            if (this._signatures == null)
            {
                this._signatures = new List<PackageDigitalSignature>();
                if (this.OriginPartExists())
                {
                    foreach (PackageRelationship relationship in this._originPart.GetRelationshipsByType(_originToSignatureRelationshipType))
                    {
                        if (relationship.TargetMode != TargetMode.Internal)
                        {
                            throw new FileFormatException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        Uri partUri = PackUriHelper.ResolvePartUri(this._originPart.Uri, relationship.TargetUri);
                        if (!this._container.PartExists(partUri))
                        {
                            throw new FileFormatException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        PackagePart signaturePart = this._container.GetPart(partUri);
                        if (signaturePart.ValidatedContentType.AreTypeAndSubTypeEqual(XmlDigitalSignatureProcessor.ContentType))
                        {
                            PackageDigitalSignature item = new PackageDigitalSignature(this, signaturePart);
                            this._signatures.Add(item);
                        }
                    }
                }
            }
        }

        private bool EnumeratorEmptyCheck(IEnumerable enumerable)
        {
            if (enumerable != null)
            {
                ICollection is2 = enumerable as ICollection;
                if (is2 != null)
                {
                    return (is2.Count == 0);
                }
                using (IEnumerator enumerator = enumerable.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        return false;
                    }
                }
            }
            return true;
        }

        private Uri GenerateSignaturePartName() => 
            PackUriHelper.CreatePartUri(new Uri(_defaultSignaturePartNamePrefix + Guid.NewGuid().ToString(_guidStorageFormatString, null) + _defaultSignaturePartNameExtension, UriKind.Relative));

        public PackageDigitalSignature GetSignature(Uri signatureUri)
        {
            if (signatureUri == null)
            {
                throw new ArgumentNullException("signatureUri");
            }
            int signatureIndex = this.GetSignatureIndex(signatureUri);
            if (signatureIndex < 0)
            {
                return null;
            }
            return this._signatures[signatureIndex];
        }

        private int GetSignatureIndex(Uri uri)
        {
            this.EnsureSignatures();
            for (int i = 0; i < this._signatures.Count; i++)
            {
                if (PackUriHelper.ComparePartUri(uri, this._signatures[i].SignaturePart.Uri) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private void InternalRemoveSignature(Uri signatureUri, int countOfSignaturesRemaining)
        {
            if (countOfSignaturesRemaining == 0)
            {
                this.DeleteOriginPart();
            }
            else
            {
                this.SafeVisitRelationships(this.OriginPart.GetRelationshipsByType(_originToSignatureRelationshipType), new RelationshipOperation(this.DeleteRelationshipToSignature), signatureUri);
            }
            this.SafeVisitRelationships(this._container.GetPart(signatureUri).GetRelationshipsByType(CertificatePart.RelationshipType), new RelationshipOperation(this.DeleteCertificateIfReferenceCountBecomesZeroVisitor));
            this._container.DeletePart(signatureUri);
        }

        private bool OriginPartExists()
        {
            if (!this._originSearchConducted)
            {
                try
                {
                    foreach (PackageRelationship relationship in this._container.GetRelationshipsByType(_originRelationshipType))
                    {
                        if (relationship.TargetMode != TargetMode.Internal)
                        {
                            throw new FileFormatException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        Uri partUri = PackUriHelper.ResolvePartUri(relationship.SourceUri, relationship.TargetUri);
                        if (!this._container.PartExists(partUri))
                        {
                            throw new FileFormatException(System.Windows.SR.Get("SignatureOriginNotFound"));
                        }
                        PackagePart part = this._container.GetPart(partUri);
                        if (part.ValidatedContentType.AreTypeAndSubTypeEqual(_originPartContentType))
                        {
                            if (this._originPartExists)
                            {
                                throw new FileFormatException(System.Windows.SR.Get("MultipleSignatureOrigins"));
                            }
                            this._originPartName = partUri;
                            this._originPart = part;
                            this._originPartExists = true;
                        }
                    }
                }
                finally
                {
                    this._originSearchConducted = true;
                }
            }
            return this._originPartExists;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static X509Certificate PromptForSigningCertificate(IntPtr hwndParent)
        {
            X509Certificate2 certificate = null;
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.OpenExistingOnly);
            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true).Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, false);
            for (int i = certificates.Count - 1; i >= 0; i--)
            {
                if (!certificates[i].HasPrivateKey)
                {
                    certificates.RemoveAt(i);
                }
            }
            if (certificates.Count > 0)
            {
                certificates = X509Certificate2UI.SelectFromCollection(certificates, System.Windows.SR.Get("CertSelectionDialogTitle"), System.Windows.SR.Get("CertSelectionDialogMessage"), X509SelectionFlag.SingleSelection, hwndParent);
                if (certificates.Count > 0)
                {
                    certificate = certificates[0];
                }
            }
            return certificate;
        }

        public void RemoveAllSignatures()
        {
            if (this.ReadOnly)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("CannotRemoveSignatureFromReadOnlyFile"));
            }
            this.EnsureSignatures();
            try
            {
                for (int i = 0; i < this._signatures.Count; i++)
                {
                    PackagePart signaturePart = this._signatures[i].SignaturePart;
                    foreach (PackageRelationship relationship in signaturePart.GetRelationshipsByType(CertificatePart.RelationshipType))
                    {
                        if (relationship.TargetMode == TargetMode.Internal)
                        {
                            this._container.DeletePart(PackUriHelper.ResolvePartUri(relationship.SourceUri, relationship.TargetUri));
                        }
                    }
                    this._container.DeletePart(signaturePart.Uri);
                    this._signatures[i].Invalidate();
                }
                this.DeleteOriginPart();
            }
            finally
            {
                this._signatures.Clear();
            }
        }

        public void RemoveSignature(Uri signatureUri)
        {
            if (this.ReadOnly)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("CannotRemoveSignatureFromReadOnlyFile"));
            }
            if (signatureUri == null)
            {
                throw new ArgumentNullException("signatureUri");
            }
            if (this.IsSigned)
            {
                int signatureIndex = this.GetSignatureIndex(signatureUri);
                if (signatureIndex >= 0)
                {
                    try
                    {
                        this.InternalRemoveSignature(signatureUri, this._signatures.Count - 1);
                        this._signatures[signatureIndex].Invalidate();
                    }
                    finally
                    {
                        this._signatures.RemoveAt(signatureIndex);
                    }
                }
            }
        }

        private void SafeVisitRelationships(PackageRelationshipCollection relationships, RelationshipOperation visit)
        {
            this.SafeVisitRelationships(relationships, visit, null);
        }

        private void SafeVisitRelationships(PackageRelationshipCollection relationships, RelationshipOperation visit, object context)
        {
            List<PackageRelationship> list = new List<PackageRelationship>(relationships);
            for (int i = 0; i < list.Count; i++)
            {
                if (!visit(list[i], context))
                {
                    return;
                }
            }
        }

        public PackageDigitalSignature Sign(IEnumerable<Uri> parts)
        {
            X509Certificate certificate = PromptForSigningCertificate(this.ParentWindow);
            if (certificate == null)
            {
                return null;
            }
            return this.Sign(parts, certificate);
        }

        public PackageDigitalSignature Sign(IEnumerable<Uri> parts, X509Certificate certificate) => 
            this.Sign(parts, certificate, null);

        public PackageDigitalSignature Sign(IEnumerable<Uri> parts, X509Certificate certificate, IEnumerable<PackageRelationshipSelector> relationshipSelectors) => 
            this.Sign(parts, certificate, relationshipSelectors, XTable.Get(XTable.ID.OpcSignatureAttrValue));

        public PackageDigitalSignature Sign(IEnumerable<Uri> parts, X509Certificate certificate, IEnumerable<PackageRelationshipSelector> relationshipSelectors, string signatureId)
        {
            if ((parts == null) && (relationshipSelectors == null))
            {
                throw new ArgumentException(System.Windows.SR.Get("NothingToSign"));
            }
            return this.Sign(parts, certificate, relationshipSelectors, signatureId, null, null);
        }

        [SecurityCritical]
        public PackageDigitalSignature Sign(IEnumerable<Uri> parts, X509Certificate certificate, IEnumerable<PackageRelationshipSelector> relationshipSelectors, string signatureId, IEnumerable<DataObject> signatureObjects, IEnumerable<Reference> objectReferences)
        {
            if (this.ReadOnly)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("CannotSignReadOnlyFile"));
            }
            this.VerifySignArguments(parts, certificate, relationshipSelectors, signatureId, signatureObjects, objectReferences);
            if ((signatureId == null) || (signatureId == string.Empty))
            {
                signatureId = "packageSignature";
            }
            this.EnsureSignatures();
            Uri partUri = this.GenerateSignaturePartName();
            if (this._container.PartExists(partUri))
            {
                throw new ArgumentException(System.Windows.SR.Get("DuplicateSignature"));
            }
            this.OriginPart.CreateRelationship(partUri, TargetMode.Internal, _originToSignatureRelationshipType);
            this._container.Flush();
            this.VerifyPartsExist(parts);
            bool embedCertificate = this._certificateEmbeddingOption == CertificateEmbeddingOption.InSignaturePart;
            X509Certificate2 signer = certificate as X509Certificate2;
            if (signer == null)
            {
                signer = new X509Certificate2(certificate.Handle);
            }
            PackageDigitalSignature item = null;
            PackagePart signaturePart = null;
            try
            {
                signaturePart = this._container.CreatePart(partUri, XmlDigitalSignatureProcessor.ContentType.ToString());
                item = XmlDigitalSignatureProcessor.Sign(this, signaturePart, parts, relationshipSelectors, signer, signatureId, embedCertificate, signatureObjects, objectReferences);
            }
            catch (InvalidOperationException)
            {
                this.InternalRemoveSignature(partUri, this._signatures.Count);
                this._container.Flush();
                throw;
            }
            catch (IOException)
            {
                this.InternalRemoveSignature(partUri, this._signatures.Count);
                this._container.Flush();
                throw;
            }
            catch (CryptographicException)
            {
                this.InternalRemoveSignature(partUri, this._signatures.Count);
                this._container.Flush();
                throw;
            }
            this._signatures.Add(item);
            if (this._certificateEmbeddingOption == CertificateEmbeddingOption.InCertificatePart)
            {
                Uri partName = PackUriHelper.CreatePartUri(new Uri(CertificatePart.PartNamePrefix + signer.SerialNumber + CertificatePart.PartNameExtension, UriKind.Relative));
                CertificatePart certificatePart = new CertificatePart(this._container, partName);
                certificatePart.SetCertificate(signer);
                signaturePart.CreateRelationship(partName, TargetMode.Internal, CertificatePart.RelationshipType);
                item.SetCertificatePart(certificatePart);
            }
            this._container.Flush();
            return item;
        }

        [SecurityCritical, SecurityPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        public static X509ChainStatusFlags VerifyCertificate(X509Certificate certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }
            X509ChainStatusFlags noError = X509ChainStatusFlags.NoError;
            X509Chain chain = new X509Chain();
            if (!chain.Build(new X509Certificate2(certificate.Handle)))
            {
                X509ChainStatus[] chainStatus = chain.ChainStatus;
                for (int i = 0; i < chainStatus.Length; i++)
                {
                    noError |= chainStatus[i].Status;
                }
            }
            return noError;
        }

        private void VerifyPartsExist(IEnumerable<Uri> parts)
        {
            if (parts != null)
            {
                foreach (Uri uri in parts)
                {
                    if (!this._container.PartExists(uri))
                    {
                        if (this._signatures.Count == 0)
                        {
                            this.DeleteOriginPart();
                        }
                        throw new ArgumentException(System.Windows.SR.Get("PartToSignMissing"), "parts");
                    }
                }
            }
        }

        private void VerifySignArguments(IEnumerable<Uri> parts, X509Certificate certificate, IEnumerable<PackageRelationshipSelector> relationshipSelectors, string signatureId, IEnumerable<DataObject> signatureObjects, IEnumerable<Reference> objectReferences)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }
            if ((this.EnumeratorEmptyCheck(parts) && this.EnumeratorEmptyCheck(relationshipSelectors)) && (this.EnumeratorEmptyCheck(signatureObjects) && this.EnumeratorEmptyCheck(objectReferences)))
            {
                throw new ArgumentException(System.Windows.SR.Get("NothingToSign"));
            }
            if (signatureObjects != null)
            {
                List<string> list = new List<string>();
                foreach (DataObject obj2 in signatureObjects)
                {
                    if (string.CompareOrdinal(obj2.Id, XTable.Get(XTable.ID.OpcAttrValue)) == 0)
                    {
                        throw new ArgumentException(System.Windows.SR.Get("SignaturePackageObjectTagMustBeUnique"), "signatureObjects");
                    }
                    if (list.Exists(new Predicate<string>(new StringMatchPredicate(obj2.Id).Match)))
                    {
                        throw new ArgumentException(System.Windows.SR.Get("SignatureObjectIdMustBeUnique"), "signatureObjects");
                    }
                    list.Add(obj2.Id);
                }
            }
            if ((signatureId != null) && (signatureId != string.Empty))
            {
                try
                {
                    XmlConvert.VerifyNCName(signatureId);
                }
                catch (XmlException exception)
                {
                    throw new ArgumentException(System.Windows.SR.Get("NotAValidXmlIdString", new object[] { signatureId }), "signatureId", exception);
                }
            }
        }

        public VerifyResult VerifySignatures(bool exitOnFailure)
        {
            this.EnsureSignatures();
            if (this._signatures.Count == 0)
            {
                return VerifyResult.NotSigned;
            }
            VerifyResult success = VerifyResult.Success;
            for (int i = 0; i < this._signatures.Count; i++)
            {
                VerifyResult result = this._signatures[i].Verify();
                if (result != VerifyResult.Success)
                {
                    success = result;
                    if (this.InvalidSignatureEvent != null)
                    {
                        this.InvalidSignatureEvent(this, new SignatureVerificationEventArgs(this._signatures[i], result));
                    }
                    if (exitOnFailure)
                    {
                        return success;
                    }
                }
            }
            return success;
        }

        public CertificateEmbeddingOption CertificateOption
        {
            get => 
                this._certificateEmbeddingOption;
            set
            {
                if ((value < CertificateEmbeddingOption.InCertificatePart) || (value > CertificateEmbeddingOption.NotEmbedded))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._certificateEmbeddingOption = value;
            }
        }

        public static string DefaultHashAlgorithm =>
            _defaultHashAlgorithm;

        public string HashAlgorithm
        {
            get => 
                this._hashAlgorithmString;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (value == string.Empty)
                {
                    throw new ArgumentException(System.Windows.SR.Get("UnsupportedHashAlgorithm"), "value");
                }
                this._hashAlgorithmString = value;
            }
        }

        public bool IsSigned
        {
            get
            {
                this.EnsureSignatures();
                return (this._signatures.Count > 0);
            }
        }

        private PackagePart OriginPart
        {
            get
            {
                if ((this._originPart == null) && !this.OriginPartExists())
                {
                    this._originPart = this._container.CreatePart(this._originPartName, _originPartContentType.ToString());
                    this._container.CreateRelationship(this._originPartName, TargetMode.Internal, _originRelationshipType);
                }
                return this._originPart;
            }
        }

        internal System.IO.Packaging.Package Package =>
            this._container;

        public IntPtr ParentWindow
        {
            get => 
                this._parentWindow;
            set
            {
                this._parentWindow = value;
            }
        }

        private bool ReadOnly =>
            (this._container.FileOpenAccess == FileAccess.Read);

        public Uri SignatureOrigin
        {
            get
            {
                this.OriginPartExists();
                return this._originPartName;
            }
        }

        public static string SignatureOriginRelationshipType =>
            _originRelationshipType;

        public ReadOnlyCollection<PackageDigitalSignature> Signatures
        {
            get
            {
                this.EnsureSignatures();
                if (this._signatureList == null)
                {
                    this._signatureList = new ReadOnlyCollection<PackageDigitalSignature>(this._signatures);
                }
                return this._signatureList;
            }
        }

        public string TimeFormat
        {
            get => 
                this._signatureTimeFormat;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!XmlSignatureProperties.LegalFormat(value))
                {
                    throw new FormatException(System.Windows.SR.Get("BadSignatureTimeFormatString"));
                }
                this._signatureTimeFormat = value;
            }
        }

        public Dictionary<string, string> TransformMapping =>
            this._transformDictionary;

        private delegate bool RelationshipOperation(PackageRelationship r, object context);

        private class StringMatchPredicate
        {
            private string _id;

            public StringMatchPredicate(string id)
            {
                this._id = id;
            }

            public bool Match(string id) => 
                (string.CompareOrdinal(this._id, id) == 0);
        }
    }
}

