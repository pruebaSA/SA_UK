namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows;
    using System.Xml;

    internal class XmlDigitalSignatureProcessor
    {
        private X509Certificate2 _certificate;
        private bool _dataObjectParsed;
        private string _hashAlgorithmName;
        private bool _lookForEmbeddedCert;
        private PackageDigitalSignatureManager _manager;
        private List<PartManifestEntry> _partEntryManifest;
        private List<Uri> _partManifest;
        private List<PackageRelationshipSelector> _relationshipManifest;
        private static readonly Dictionary<string, string> _rsaSigMethodLookup;
        private PackageDigitalSignature _signature;
        private PackagePart _signaturePart;
        private SignedXml _signedXml;
        private DateTime _signingTime;
        private string _signingTimeFormat;
        private static readonly MS.Internal.ContentType _xmlSignaturePartType = new MS.Internal.ContentType("application/vnd.openxmlformats-package.digital-signature-xmlsignature+xml");

        static XmlDigitalSignatureProcessor()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { 
                    "http://www.w3.org/2001/04/xmlenc#sha256",
                    "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"
                },
                { 
                    "http://www.w3.org/2001/04/xmldsig-more#sha384",
                    "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384"
                },
                { 
                    "http://www.w3.org/2001/04/xmlenc#sha512",
                    "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512"
                }
            };
            _rsaSigMethodLookup = dictionary;
        }

        private XmlDigitalSignatureProcessor(PackageDigitalSignatureManager manager, PackagePart signaturePart)
        {
            Invariant.Assert(manager != null);
            Invariant.Assert(signaturePart != null);
            this._signaturePart = signaturePart;
            this._manager = manager;
            this._lookForEmbeddedCert = true;
        }

        internal XmlDigitalSignatureProcessor(PackageDigitalSignatureManager manager, PackagePart signaturePart, PackageDigitalSignature packageSignature) : this(manager, signaturePart)
        {
            this._signature = packageSignature;
        }

        private void AddCustomObjectTags(IEnumerable<DataObject> signatureObjects, IEnumerable<Reference> objectReferences)
        {
            Invariant.Assert(this._signedXml != null);
            if (objectReferences != null)
            {
                this.ValidateReferences(objectReferences, false);
                foreach (Reference reference in objectReferences)
                {
                    reference.DigestMethod = this._hashAlgorithmName;
                    this._signedXml.AddReference(reference);
                }
            }
            if (signatureObjects != null)
            {
                foreach (DataObject obj2 in signatureObjects)
                {
                    this._signedXml.AddObject(obj2);
                }
            }
        }

        private SignedXml EnsureXmlSignatureParsed()
        {
            if (this._signedXml == null)
            {
                this._signedXml = new CustomSignedXml();
                XmlDocument document = new XmlDocument {
                    PreserveWhitespace = true
                };
                using (Stream stream = this.SignaturePart.GetStream())
                {
                    using (XmlTextReader reader = new XmlTextReader(stream))
                    {
                        reader.ProhibitDtd = true;
                        PackagingUtilities.PerformInitailReadAndVerifyEncoding(reader);
                        document.Load(reader);
                        XmlNodeList childNodes = document.ChildNodes;
                        if (((childNodes == null) || (childNodes.Count == 0)) || (childNodes.Count > 2))
                        {
                            throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        XmlNode node = childNodes[0];
                        if (childNodes.Count == 2)
                        {
                            if (childNodes[0].NodeType != XmlNodeType.XmlDeclaration)
                            {
                                throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                            }
                            node = childNodes[1];
                        }
                        if (((node.NodeType != XmlNodeType.Element) || (string.CompareOrdinal(node.NamespaceURI, "http://www.w3.org/2000/09/xmldsig#") != 0)) || (string.CompareOrdinal(node.LocalName, XTable.Get(XTable.ID.SignatureTagName)) != 0))
                        {
                            throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        this._signedXml.LoadXml((XmlElement) node);
                    }
                }
            }
            if (!IsValidXmlCanonicalizationTransform(this._signedXml.SignedInfo.CanonicalizationMethod))
            {
                throw new XmlException(System.Windows.SR.Get("UnsupportedCanonicalizationMethod"));
            }
            if (this._signedXml.Signature.Id != null)
            {
                try
                {
                    XmlConvert.VerifyNCName(this._signedXml.Signature.Id);
                }
                catch (XmlException)
                {
                    throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                }
            }
            return this._signedXml;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static string GenerateDigestValue(Stream s, List<string> transforms, HashAlgorithm hashAlgorithm)
        {
            s.Seek(0L, SeekOrigin.Begin);
            Stream source = new IgnoreFlushAndCloseStream(s);
            List<Stream> list = null;
            if (transforms != null)
            {
                list = new List<Stream>(transforms.Count) {
                    source
                };
                foreach (string str in transforms)
                {
                    if ((str.Length != 0) && (string.CompareOrdinal(str, XTable.Get(XTable.ID.RelationshipsTransformName)) != 0))
                    {
                        Transform xForm = StringToTransform(str);
                        if (xForm == null)
                        {
                            throw new XmlException(System.Windows.SR.Get("UnsupportedTransformAlgorithm"));
                        }
                        source = TransformXml(xForm, source);
                        list.Add(source);
                    }
                }
            }
            string str2 = Convert.ToBase64String(HashStream(hashAlgorithm, source));
            if (list != null)
            {
                foreach (Stream stream2 in list)
                {
                    stream2.Close();
                }
            }
            return str2;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static string GenerateDigestValue(Stream s, string transformName, HashAlgorithm hashAlgorithm)
        {
            List<string> transforms = null;
            if (transformName != null)
            {
                transforms = new List<string>(1) {
                    transformName
                };
            }
            return GenerateDigestValue(s, transforms, hashAlgorithm);
        }

        private KeyInfo GenerateKeyInfo(AsymmetricAlgorithm key, X509Certificate2 signer)
        {
            KeyInfo info = new KeyInfo();
            KeyInfoName clause = new KeyInfoName {
                Value = signer.Subject
            };
            info.AddClause(clause);
            if (key is RSA)
            {
                info.AddClause(new RSAKeyValue((RSA) key));
            }
            else
            {
                if (!(key is DSA))
                {
                    throw new ArgumentException(System.Windows.SR.Get("CertificateKeyTypeNotSupported"), "signer");
                }
                info.AddClause(new DSAKeyValue((DSA) key));
            }
            info.AddClause(new KeyInfoX509Data(signer));
            return info;
        }

        private DataObject GenerateObjectTag(HashAlgorithm hashAlgorithm, IEnumerable<Uri> parts, IEnumerable<PackageRelationshipSelector> relationshipSelectors, string signatureId)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.AppendChild(xDoc.CreateNode(XmlNodeType.Element, "root", "namespace"));
            xDoc.DocumentElement.AppendChild(XmlSignatureManifest.GenerateManifest(this._manager, xDoc, hashAlgorithm, parts, relationshipSelectors));
            xDoc.DocumentElement.AppendChild(XmlSignatureProperties.AssembleSignatureProperties(xDoc, DateTime.Now, this._manager.TimeFormat, signatureId));
            return new DataObject { 
                Data = xDoc.DocumentElement.ChildNodes,
                Id = XTable.Get(XTable.ID.OpcAttrValue)
            };
        }

        internal static Stream GenerateRelationshipNodeStream(IEnumerable<PackageRelationship> relationships)
        {
            Stream stream = new MemoryStream();
            using (XmlTextWriter writer = new XmlTextWriter(new IgnoreFlushAndCloseStream(stream), Encoding.UTF8))
            {
                writer.WriteStartElement(XTable.Get(XTable.ID.RelationshipsTagName), PackagingUtilities.RelationshipNamespaceUri);
                InternalRelationshipCollection.WriteRelationshipsAsXml(writer, relationships, true, false);
                writer.WriteEndElement();
            }
            return stream;
        }

        internal static HashAlgorithm GetHashAlgorithm(string hashAlgorithmName)
        {
            object obj2 = CryptoConfig.CreateFromName(hashAlgorithmName);
            HashAlgorithm algorithm = obj2 as HashAlgorithm;
            if ((algorithm == null) && (obj2 != null))
            {
                IDisposable disposable = obj2 as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return algorithm;
        }

        private DataObject GetPackageDataObject()
        {
            this.EnsureXmlSignatureParsed();
            string strB = XTable.Get(XTable.ID.OpcAttrValue);
            DataObject obj2 = null;
            foreach (DataObject obj3 in this._signedXml.Signature.ObjectList)
            {
                if (string.CompareOrdinal(obj3.Id, strB) == 0)
                {
                    if (obj2 != null)
                    {
                        throw new XmlException(System.Windows.SR.Get("SignatureObjectIdMustBeUnique"));
                    }
                    obj2 = obj3;
                }
            }
            if (obj2 == null)
            {
                throw new XmlException(System.Windows.SR.Get("PackageSignatureObjectTagRequired"));
            }
            return obj2;
        }

        internal List<string> GetPartTransformList(Uri partName)
        {
            this.ParsePackageDataObject();
            List<string> transforms = null;
            foreach (PartManifestEntry entry in this._partEntryManifest)
            {
                if (PackUriHelper.ComparePartUri(entry.Uri, partName) == 0)
                {
                    transforms = entry.Transforms;
                    break;
                }
            }
            if (transforms == null)
            {
                transforms = new List<string>();
            }
            return transforms;
        }

        private static AsymmetricAlgorithm GetPrivateKeyForSigning(X509Certificate2 signer)
        {
            Invariant.Assert(!signer.HasPrivateKey);
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.OpenExistingOnly);
                X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByThumbprint, signer.Thumbprint, true);
                if (certificates.Count <= 0)
                {
                    throw new CryptographicException(System.Windows.SR.Get("DigSigCannotLocateCertificate"));
                }
                if (certificates.Count > 1)
                {
                    throw new CryptographicException(System.Windows.SR.Get("DigSigDuplicateCertificate"));
                }
                signer = certificates[0];
            }
            finally
            {
                store.Close();
            }
            return signer.PrivateKey;
        }

        private Stream GetRelationshipStream(PartManifestEntry partEntry)
        {
            SortedDictionary<string, PackageRelationship> dictionary = new SortedDictionary<string, PackageRelationship>(StringComparer.Ordinal);
            foreach (PackageRelationshipSelector selector in partEntry.RelationshipSelectors)
            {
                foreach (PackageRelationship relationship in selector.Select(this._manager.Package))
                {
                    if (!dictionary.ContainsKey(relationship.Id))
                    {
                        dictionary.Add(relationship.Id, relationship);
                    }
                }
            }
            return GenerateRelationshipNodeStream(dictionary.Values);
        }

        private static byte[] HashStream(HashAlgorithm hashAlgorithm, Stream s)
        {
            s.Seek(0L, SeekOrigin.Begin);
            hashAlgorithm.Initialize();
            return hashAlgorithm.ComputeHash(s);
        }

        internal static bool IsValidXmlCanonicalizationTransform(string transformName)
        {
            Invariant.Assert(transformName != null);
            if ((string.CompareOrdinal(transformName, "http://www.w3.org/TR/2001/REC-xml-c14n-20010315") != 0) && (string.CompareOrdinal(transformName, "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments") != 0))
            {
                return false;
            }
            return true;
        }

        private void ParsePackageDataObject()
        {
            if (!this._dataObjectParsed)
            {
                this.EnsureXmlSignatureParsed();
                XmlNodeList data = this.GetPackageDataObject().Data;
                if (data.Count != 2)
                {
                    throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
                }
                XmlReader reader = new XmlNodeReader(data[0].ParentNode);
                reader.Read();
                if (string.CompareOrdinal(reader.NamespaceURI, "http://www.w3.org/2000/09/xmldsig#") != 0)
                {
                    throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
                }
                string strB = XTable.Get(XTable.ID.SignaturePropertiesTagName);
                string str2 = XTable.Get(XTable.ID.ManifestTagName);
                bool flag = false;
                bool flag2 = false;
                while (reader.Read() && (reader.NodeType == XmlNodeType.Element))
                {
                    if (((reader.MoveToContent() == XmlNodeType.Element) && (string.CompareOrdinal(reader.NamespaceURI, "http://www.w3.org/2000/09/xmldsig#") == 0)) && (reader.Depth == 1))
                    {
                        if (!flag && (string.CompareOrdinal(reader.LocalName, strB) == 0))
                        {
                            flag = true;
                            this._signingTime = XmlSignatureProperties.ParseSigningTime(reader, this._signedXml.Signature.Id, out this._signingTimeFormat);
                            continue;
                        }
                        if (!flag2 && (string.CompareOrdinal(reader.LocalName, str2) == 0))
                        {
                            flag2 = true;
                            XmlSignatureManifest.ParseManifest(this._manager, reader, out this._partManifest, out this._partEntryManifest, out this._relationshipManifest);
                            continue;
                        }
                    }
                    throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
                }
                if (!flag || !flag2)
                {
                    throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
                }
                this._dataObjectParsed = true;
            }
        }

        private string SelectSignatureMethod(AsymmetricAlgorithm key)
        {
            string str = null;
            if (key is RSA)
            {
                _rsaSigMethodLookup.TryGetValue(this._manager.HashAlgorithm, out str);
            }
            return str;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private PackageDigitalSignature Sign(IEnumerable<Uri> parts, IEnumerable<PackageRelationshipSelector> relationshipSelectors, X509Certificate2 signer, string signatureId, bool embedCertificate, IEnumerable<DataObject> signatureObjects, IEnumerable<Reference> objectReferences)
        {
            this._hashAlgorithmName = this._manager.HashAlgorithm;
            if (this._manager.CertificateOption == CertificateEmbeddingOption.NotEmbedded)
            {
                this._lookForEmbeddedCert = false;
            }
            else
            {
                this._certificate = signer;
            }
            AsymmetricAlgorithm key = null;
            bool flag = false;
            if (signer.HasPrivateKey)
            {
                key = signer.PrivateKey;
            }
            else
            {
                flag = true;
                key = GetPrivateKeyForSigning(signer);
            }
            try
            {
                this._signedXml = new CustomSignedXml();
                this._signedXml.SigningKey = key;
                this._signedXml.Signature.Id = signatureId;
                if (BaseCompatibilityPreferences.MatchPackageSignatureMethodToPackagePartDigestMethod)
                {
                    this._signedXml.SignedInfo.SignatureMethod = this.SelectSignatureMethod(key);
                }
                bool flag2 = this._signedXml.SignedInfo.SignatureMethod != null;
                if (embedCertificate)
                {
                    this._signedXml.KeyInfo = this.GenerateKeyInfo(key, signer);
                }
                using (HashAlgorithm algorithm2 = GetHashAlgorithm(this._hashAlgorithmName))
                {
                    if (algorithm2 == null)
                    {
                        throw new InvalidOperationException(System.Windows.SR.Get("UnsupportedHashAlgorithm"));
                    }
                    this._signedXml.AddObject(this.GenerateObjectTag(algorithm2, parts, relationshipSelectors, signatureId));
                }
                Reference reference = new Reference(XTable.Get(XTable.ID.OpcLinkAttrValue)) {
                    Type = XTable.Get(XTable.ID.W3CSignatureNamespaceRoot) + "Object",
                    DigestMethod = this._hashAlgorithmName
                };
                this._signedXml.AddReference(reference);
                this.AddCustomObjectTags(signatureObjects, objectReferences);
                SignedXml xml = this._signedXml;
                new PermissionSet(PermissionState.Unrestricted).Assert();
                try
                {
                    xml.ComputeSignature();
                }
                catch (CryptographicException)
                {
                    if (!flag2)
                    {
                        throw;
                    }
                    BaseCompatibilityPreferences.MatchPackageSignatureMethodToPackagePartDigestMethod = false;
                    xml.SignedInfo.SignatureMethod = null;
                    xml.ComputeSignature();
                }
                finally
                {
                    PermissionSet.RevertAssert();
                }
                this.UpdatePartFromSignature(this._signedXml.Signature);
            }
            finally
            {
                if ((key != null) && flag)
                {
                    ((IDisposable) key).Dispose();
                }
            }
            this._signature = new PackageDigitalSignature(this._manager, this);
            return this._signature;
        }

        internal static PackageDigitalSignature Sign(PackageDigitalSignatureManager manager, PackagePart signaturePart, IEnumerable<Uri> parts, IEnumerable<PackageRelationshipSelector> relationshipSelectors, X509Certificate2 signer, string signatureId, bool embedCertificate, IEnumerable<DataObject> signatureObjects, IEnumerable<Reference> objectReferences)
        {
            XmlDigitalSignatureProcessor processor = new XmlDigitalSignatureProcessor(manager, signaturePart);
            return processor.Sign(parts, relationshipSelectors, signer, signatureId, embedCertificate, signatureObjects, objectReferences);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private static Transform StringToTransform(string transformName)
        {
            Invariant.Assert(transformName != null);
            if (string.CompareOrdinal(transformName, "http://www.w3.org/TR/2001/REC-xml-c14n-20010315") == 0)
            {
                return new XmlDsigC14NTransform();
            }
            if (string.CompareOrdinal(transformName, "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments") == 0)
            {
                return new XmlDsigC14NWithCommentsTransform();
            }
            return null;
        }

        [SecurityCritical]
        private static Stream TransformXml(Transform xForm, object source)
        {
            new PermissionSet(PermissionState.Unrestricted).Assert();
            try
            {
                xForm.LoadInput(source);
            }
            finally
            {
                PermissionSet.RevertAssert();
            }
            return (Stream) xForm.GetOutput();
        }

        private void UpdatePartFromSignature(System.Security.Cryptography.Xml.Signature sig)
        {
            using (Stream stream = this.SignaturePart.GetStream(FileMode.Create, FileAccess.Write))
            {
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    writer.WriteStartDocument(true);
                    sig.GetXml().WriteTo(writer);
                    writer.WriteEndDocument();
                }
            }
            this._signedXml = null;
        }

        private void ValidateReferences(IEnumerable references, bool allowPackageSpecificReferences)
        {
            bool flag = false;
            foreach (Reference reference in references)
            {
                if (!reference.Uri.StartsWith("#", StringComparison.Ordinal))
                {
                    throw new XmlException(System.Windows.SR.Get("InvalidUriAttribute"));
                }
                if (string.CompareOrdinal(reference.Uri, XTable.Get(XTable.ID.OpcLinkAttrValue)) == 0)
                {
                    if (!allowPackageSpecificReferences)
                    {
                        throw new ArgumentException(System.Windows.SR.Get("PackageSpecificReferenceTagMustBeUnique"));
                    }
                    if (flag)
                    {
                        throw new XmlException(System.Windows.SR.Get("MoreThanOnePackageSpecificReference"));
                    }
                    flag = true;
                }
                TransformChain transformChain = reference.TransformChain;
                for (int i = 0; i < transformChain.Count; i++)
                {
                    if (!IsValidXmlCanonicalizationTransform(transformChain[i].Algorithm))
                    {
                        throw new XmlException(System.Windows.SR.Get("UnsupportedTransformAlgorithm"));
                    }
                }
            }
            if (allowPackageSpecificReferences && !flag)
            {
                throw new XmlException(System.Windows.SR.Get("PackageSignatureReferenceTagRequired"));
            }
        }

        internal bool Verify() => 
            this.Verify(this.Signer);

        [SecurityCritical, SecurityTreatAsSafe]
        internal bool Verify(X509Certificate2 signer)
        {
            Invariant.Assert(signer != null);
            SignedXml xml = this.EnsureXmlSignatureParsed();
            bool flag = false;
            this.ValidateReferences(xml.SignedInfo.References, true);
            new PermissionSet(PermissionState.Unrestricted).Assert();
            try
            {
                flag = xml.CheckSignature(signer, true);
            }
            finally
            {
                PermissionSet.RevertAssert();
            }
            if (flag)
            {
                HashAlgorithm hashAlgorithm = null;
                string strB = string.Empty;
                try
                {
                    try
                    {
                        this.ParsePackageDataObject();
                    }
                    catch (XmlException)
                    {
                        return false;
                    }
                    foreach (PartManifestEntry entry in this._partEntryManifest)
                    {
                        Stream s = null;
                        if (entry.IsRelationshipEntry)
                        {
                            s = this.GetRelationshipStream(entry);
                        }
                        else
                        {
                            PackagePart part = this._manager.Package.GetPart(entry.Uri);
                            if (string.CompareOrdinal(entry.ContentType.OriginalString, part.ValidatedContentType.OriginalString) != 0)
                            {
                                return false;
                            }
                            s = part.GetStream(FileMode.Open, FileAccess.Read);
                        }
                        using (s)
                        {
                            if (((hashAlgorithm != null) && !hashAlgorithm.CanReuseTransform) || (string.CompareOrdinal(entry.HashAlgorithm, strB) != 0))
                            {
                                if (hashAlgorithm != null)
                                {
                                    ((IDisposable) hashAlgorithm).Dispose();
                                }
                                strB = entry.HashAlgorithm;
                                hashAlgorithm = GetHashAlgorithm(strB);
                                if (hashAlgorithm == null)
                                {
                                    return false;
                                }
                            }
                            string strA = GenerateDigestValue(s, entry.Transforms, hashAlgorithm);
                            if (string.CompareOrdinal(strA, entry.HashValue) != 0)
                            {
                                return false;
                            }
                        }
                    }
                    return flag;
                }
                finally
                {
                    if (hashAlgorithm != null)
                    {
                        ((IDisposable) hashAlgorithm).Dispose();
                    }
                }
            }
            return flag;
        }

        internal static MS.Internal.ContentType ContentType =>
            _xmlSignaturePartType;

        internal PackageDigitalSignature PackageSignature =>
            this._signature;

        internal List<Uri> PartManifest
        {
            get
            {
                this.ParsePackageDataObject();
                return this._partManifest;
            }
        }

        internal List<PackageRelationshipSelector> RelationshipManifest
        {
            get
            {
                this.ParsePackageDataObject();
                return this._relationshipManifest;
            }
        }

        internal System.Security.Cryptography.Xml.Signature Signature
        {
            get => 
                this.EnsureXmlSignatureParsed().Signature;
            set
            {
                this.UpdatePartFromSignature(value);
            }
        }

        internal PackagePart SignaturePart =>
            this._signaturePart;

        internal byte[] SignatureValue =>
            this.EnsureXmlSignatureParsed().SignatureValue;

        internal X509Certificate2 Signer
        {
            get
            {
                if (this._certificate == null)
                {
                    if (this.PackageSignature.GetCertificatePart() != null)
                    {
                        this._certificate = this.PackageSignature.GetCertificatePart().GetCertificate();
                    }
                    else if (this._lookForEmbeddedCert)
                    {
                        IEnumerator enumerator = this.EnsureXmlSignatureParsed().KeyInfo.GetEnumerator(typeof(KeyInfoX509Data));
                        while (enumerator.MoveNext())
                        {
                            KeyInfoX509Data current = (KeyInfoX509Data) enumerator.Current;
                            foreach (X509Certificate2 certificate in current.Certificates)
                            {
                                this._certificate = certificate;
                                break;
                            }
                            if (this._certificate != null)
                            {
                                break;
                            }
                        }
                        this._lookForEmbeddedCert = false;
                    }
                }
                return this._certificate;
            }
        }

        internal DateTime SigningTime
        {
            get
            {
                this.ParsePackageDataObject();
                return this._signingTime;
            }
        }

        internal string TimeFormat
        {
            get
            {
                this.ParsePackageDataObject();
                return this._signingTimeFormat;
            }
        }
    }
}

