namespace System.Security.Cryptography.Xml
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;
    using System.Xml;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public class SignedXml
    {
        private byte[] _digestedSignedInfo;
        private const string AllowHMACTruncationValue = "AllowHMACTruncation";
        private bool bCacheValid;
        private bool m_bResolverSet;
        private XmlDocument m_containingDocument;
        internal XmlElement m_context;
        private System.Security.Cryptography.Xml.EncryptedXml m_exml;
        private IEnumerator m_keyInfoEnum;
        private int[] m_refLevelCache;
        private bool[] m_refProcessed;
        protected System.Security.Cryptography.Xml.Signature m_signature;
        private AsymmetricAlgorithm m_signingKey;
        protected string m_strSigningKeyName;
        private X509Certificate2Collection m_x509Collection;
        private IEnumerator m_x509Enum;
        internal XmlResolver m_xmlResolver;
        private static bool? s_allowHmacTruncation;
        private static List<string> s_defaultSafeTransformMethods;
        private static List<string> s_safeCanonicalizationMethods;
        public const string XmlDecryptionTransformUrl = "http://www.w3.org/2002/07/decrypt#XML";
        public const string XmlDsigBase64TransformUrl = "http://www.w3.org/2000/09/xmldsig#base64";
        public const string XmlDsigC14NTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
        public const string XmlDsigC14NWithCommentsTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";
        public const string XmlDsigCanonicalizationUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
        public const string XmlDsigCanonicalizationWithCommentsUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";
        public const string XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
        public const string XmlDsigEnvelopedSignatureTransformUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
        public const string XmlDsigExcC14NTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#";
        public const string XmlDsigExcC14NWithCommentsTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";
        public const string XmlDsigHMACSHA1Url = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
        public const string XmlDsigMinimalCanonicalizationUrl = "http://www.w3.org/2000/09/xmldsig#minimal";
        private const string XmlDsigMoreHMACMD5Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";
        private const string XmlDsigMoreHMACRIPEMD160Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";
        private const string XmlDsigMoreHMACSHA256Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
        private const string XmlDsigMoreHMACSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
        private const string XmlDsigMoreHMACSHA512Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
        public const string XmlDsigNamespaceUrl = "http://www.w3.org/2000/09/xmldsig#";
        public const string XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        internal const string XmlDsigRSASHA256Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        internal const string XmlDsigRSASHA384Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";
        internal const string XmlDsigRSASHA512Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";
        public const string XmlDsigSHA1Url = "http://www.w3.org/2000/09/xmldsig#sha1";
        internal const string XmlDsigSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";
        internal const string XmlDsigSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#sha384";
        internal const string XmlDsigSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";
        public const string XmlDsigXPathTransformUrl = "http://www.w3.org/TR/1999/REC-xpath-19991116";
        public const string XmlDsigXsltTransformUrl = "http://www.w3.org/TR/1999/REC-xslt-19991116";
        public const string XmlLicenseTransformUrl = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";

        public SignedXml()
        {
            this.Initialize(null);
        }

        public SignedXml(XmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            this.Initialize(document.DocumentElement);
        }

        public SignedXml(XmlElement elem)
        {
            if (elem == null)
            {
                throw new ArgumentNullException("elem");
            }
            this.Initialize(elem);
        }

        public void AddObject(DataObject dataObject)
        {
            this.m_signature.AddObject(dataObject);
        }

        public void AddReference(Reference reference)
        {
            this.m_signature.SignedInfo.AddReference(reference);
        }

        private X509Certificate2Collection BuildBagOfCerts()
        {
            X509Certificate2Collection certificates = new X509Certificate2Collection();
            if (this.KeyInfo != null)
            {
                foreach (KeyInfoClause clause in this.KeyInfo)
                {
                    KeyInfoX509Data data = clause as KeyInfoX509Data;
                    if (data != null)
                    {
                        certificates.AddRange(System.Security.Cryptography.Xml.Utils.BuildBagOfCerts(data, CertUsageType.Verification));
                    }
                }
            }
            return certificates;
        }

        private void BuildDigestedReferences()
        {
            ArrayList references = this.SignedInfo.References;
            this.m_refProcessed = new bool[references.Count];
            this.m_refLevelCache = new int[references.Count];
            ReferenceLevelSortOrder comparer = new ReferenceLevelSortOrder {
                References = references
            };
            ArrayList list2 = new ArrayList();
            foreach (Reference reference in references)
            {
                list2.Add(reference);
            }
            list2.Sort(comparer);
            CanonicalXmlNodeList refList = new CanonicalXmlNodeList();
            foreach (DataObject obj2 in this.m_signature.ObjectList)
            {
                refList.Add(obj2.GetXml());
            }
            foreach (Reference reference2 in list2)
            {
                if (reference2.DigestMethod == null)
                {
                    reference2.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
                }
                reference2.UpdateHashValue(this.m_containingDocument, refList);
                if (reference2.Id != null)
                {
                    refList.Add(reference2.GetXml());
                }
            }
        }

        private bool CheckDigestedReferences()
        {
            ArrayList references = this.m_signature.SignedInfo.References;
            for (int i = 0; i < references.Count; i++)
            {
                Reference reference = (Reference) references[i];
                if (!this.ReferenceUsesSafeTransformMethods(reference))
                {
                    return false;
                }
                byte[] a = null;
                try
                {
                    a = reference.CalculateHashValue(this.m_containingDocument, this.m_signature.ReferencedItems);
                }
                catch (CryptoSignedXmlRecursionException)
                {
                    return false;
                }
                if (!CryptographicEquals(a, reference.DigestValue))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckSignature()
        {
            AsymmetricAlgorithm publicKey;
            bool flag = false;
            do
            {
                publicKey = this.GetPublicKey();
                if (publicKey != null)
                {
                    flag = this.CheckSignature(publicKey);
                }
            }
            while ((publicKey != null) && !flag);
            return flag;
        }

        public bool CheckSignature(AsymmetricAlgorithm key)
        {
            if (!DefaultSignatureFormatValidator(this))
            {
                return false;
            }
            if (!this.CheckSignedInfo(key))
            {
                return false;
            }
            return this.CheckDigestedReferences();
        }

        public bool CheckSignature(KeyedHashAlgorithm macAlg)
        {
            if (!DefaultSignatureFormatValidator(this))
            {
                return false;
            }
            if (!this.CheckSignedInfo(macAlg))
            {
                return false;
            }
            return this.CheckDigestedReferences();
        }

        [ComVisible(false)]
        public bool CheckSignature(X509Certificate2 certificate, bool verifySignatureOnly)
        {
            if (!verifySignatureOnly)
            {
                X509ExtensionEnumerator enumerator = certificate.Extensions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    X509Extension current = enumerator.Current;
                    if (string.Compare(current.Oid.Value, "2.5.29.15", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        X509KeyUsageExtension extension2 = new X509KeyUsageExtension();
                        extension2.CopyFrom(current);
                        if (((extension2.KeyUsages & X509KeyUsageFlags.DigitalSignature) != X509KeyUsageFlags.None) || ((extension2.KeyUsages & X509KeyUsageFlags.NonRepudiation) != X509KeyUsageFlags.None))
                        {
                            break;
                        }
                        return false;
                    }
                }
                X509Chain chain = new X509Chain();
                chain.ChainPolicy.ExtraStore.AddRange(this.BuildBagOfCerts());
                if (!chain.Build(certificate))
                {
                    return false;
                }
            }
            if (!DefaultSignatureFormatValidator(this))
            {
                return false;
            }
            if (!this.CheckSignedInfo(certificate.PublicKey.Key))
            {
                return false;
            }
            if (!this.CheckDigestedReferences())
            {
                return false;
            }
            return true;
        }

        public bool CheckSignatureReturningKey(out AsymmetricAlgorithm signingKey)
        {
            bool flag = false;
            AsymmetricAlgorithm key = null;
            do
            {
                key = this.GetPublicKey();
                if (key != null)
                {
                    flag = this.CheckSignature(key);
                }
            }
            while ((key != null) && !flag);
            signingKey = key;
            return flag;
        }

        private bool CheckSignedInfo(AsymmetricAlgorithm key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            SignatureDescription description = this.CreateSignatureDescriptionFromName(this.SignatureMethod);
            if (description == null)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureDescriptionNotCreated"));
            }
            Type c = Type.GetType(description.KeyAlgorithm);
            Type type = key.GetType();
            if (((c != type) && !c.IsSubclassOf(type)) && !type.IsSubclassOf(c))
            {
                return false;
            }
            HashAlgorithm hash = description.CreateDigest();
            if (hash == null)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CreateHashAlgorithmFailed"));
            }
            byte[] rgbHash = this.GetC14NDigest(hash);
            return description.CreateDeformatter(key).VerifySignature(rgbHash, this.m_signature.SignatureValue);
        }

        private bool CheckSignedInfo(KeyedHashAlgorithm macAlg)
        {
            int hashSize;
            if (macAlg == null)
            {
                throw new ArgumentNullException("macAlg");
            }
            if (this.m_signature.SignedInfo.SignatureLength == null)
            {
                hashSize = macAlg.HashSize;
            }
            else
            {
                hashSize = Convert.ToInt32(this.m_signature.SignedInfo.SignatureLength, (IFormatProvider) null);
            }
            if ((hashSize < 0) || (hashSize > macAlg.HashSize))
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength"));
            }
            if ((hashSize % 8) != 0)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength2"));
            }
            if (this.m_signature.SignatureValue == null)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureValueRequired"));
            }
            if (this.m_signature.SignatureValue.Length != (hashSize / 8))
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength"));
            }
            byte[] buffer = this.GetC14NDigest(macAlg);
            for (int i = 0; i < this.m_signature.SignatureValue.Length; i++)
            {
                if (this.m_signature.SignatureValue[i] != buffer[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void ComputeSignature()
        {
            this.BuildDigestedReferences();
            AsymmetricAlgorithm signingKey = this.SigningKey;
            if (signingKey == null)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_LoadKeyFailed"));
            }
            if (this.SignedInfo.SignatureMethod == null)
            {
                if (!(signingKey is DSA))
                {
                    if (!(signingKey is RSA))
                    {
                        throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CreatedKeyFailed"));
                    }
                    if (this.SignedInfo.SignatureMethod == null)
                    {
                        this.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
                    }
                }
                else
                {
                    this.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
                }
            }
            SignatureDescription description = this.CreateSignatureDescriptionFromName(this.SignedInfo.SignatureMethod);
            if (description == null)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureDescriptionNotCreated"));
            }
            HashAlgorithm hash = description.CreateDigest();
            if (hash == null)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_CreateHashAlgorithmFailed"));
            }
            this.GetC14NDigest(hash);
            this.m_signature.SignatureValue = description.CreateFormatter(signingKey).CreateSignature(hash);
        }

        public void ComputeSignature(KeyedHashAlgorithm macAlg)
        {
            int hashSize;
            if (macAlg == null)
            {
                throw new ArgumentNullException("macAlg");
            }
            HMAC hash = macAlg as HMAC;
            if (hash == null)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureMethodKeyMismatch"));
            }
            if (this.m_signature.SignedInfo.SignatureLength == null)
            {
                hashSize = hash.HashSize;
            }
            else
            {
                hashSize = Convert.ToInt32(this.m_signature.SignedInfo.SignatureLength, (IFormatProvider) null);
            }
            if ((hashSize < 0) || (hashSize > hash.HashSize))
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength"));
            }
            if ((hashSize % 8) != 0)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidSignatureLength2"));
            }
            this.BuildDigestedReferences();
            switch (hash.HashName)
            {
                case "SHA1":
                    this.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
                    break;

                case "SHA256":
                    this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
                    break;

                case "SHA384":
                    this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
                    break;

                case "SHA512":
                    this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
                    break;

                case "MD5":
                    this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";
                    break;

                case "RIPEMD160":
                    this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";
                    break;

                default:
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_SignatureMethodKeyMismatch"));
            }
            byte[] src = this.GetC14NDigest(hash);
            this.m_signature.SignatureValue = new byte[hashSize / 8];
            System.Buffer.BlockCopy(src, 0, this.m_signature.SignatureValue, 0, hashSize / 8);
        }

        private SignatureDescription CreateSignatureDescriptionFromName(string name)
        {
            SignatureDescription description = CryptoConfig.CreateFromName(name) as SignatureDescription;
            if (description != null)
            {
                return description;
            }
            StringComparison ordinalIgnoreCase = StringComparison.OrdinalIgnoreCase;
            if (name.Equals("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", ordinalIgnoreCase))
            {
                return new RSAPKCS1SHA256SignatureDescription();
            }
            if (name.Equals("http://www.w3.org/2001/04/xmldsig-more#rsa-sha384", ordinalIgnoreCase))
            {
                return new RSAPKCS1SHA384SignatureDescription();
            }
            if (name.Equals("http://www.w3.org/2001/04/xmldsig-more#rsa-sha512", ordinalIgnoreCase))
            {
                return new RSAPKCS1SHA512SignatureDescription();
            }
            return null;
        }

        private static bool CryptographicEquals(byte[] a, byte[] b)
        {
            int num = 0;
            if (a.Length != b.Length)
            {
                return false;
            }
            int length = a.Length;
            for (int i = 0; i < length; i++)
            {
                num |= a[i] - b[i];
            }
            return (0 == num);
        }

        internal static XmlElement DefaultGetIdElement(XmlDocument document, string idValue)
        {
            if (document == null)
            {
                return null;
            }
            if (System.Security.Cryptography.Xml.Utils.RequireNCNameIdentifier())
            {
                try
                {
                    XmlConvert.VerifyNCName(idValue);
                }
                catch (XmlException)
                {
                    return null;
                }
            }
            XmlElement elementById = document.GetElementById(idValue);
            if (elementById != null)
            {
                if (!System.Security.Cryptography.Xml.Utils.AllowAmbiguousReferenceTargets())
                {
                    XmlDocument document2 = (XmlDocument) document.CloneNode(true);
                    XmlElement element2 = document2.GetElementById(idValue);
                    if (element2 != null)
                    {
                        element2.Attributes.RemoveAll();
                        if (document2.GetElementById(idValue) != null)
                        {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
                        }
                    }
                }
                return elementById;
            }
            elementById = GetSingleReferenceTarget(document, "Id", idValue);
            if (elementById != null)
            {
                return elementById;
            }
            elementById = GetSingleReferenceTarget(document, "id", idValue);
            if (elementById != null)
            {
                return elementById;
            }
            return GetSingleReferenceTarget(document, "ID", idValue);
        }

        private static bool DefaultSignatureFormatValidator(SignedXml signedXml)
        {
            if (!AllowHmacTruncation && signedXml.DoesSignatureUseTruncatedHmac())
            {
                return false;
            }
            if (!signedXml.DoesSignatureUseSafeCanonicalizationMethod())
            {
                return false;
            }
            return true;
        }

        private bool DoesSignatureUseSafeCanonicalizationMethod()
        {
            foreach (string str in SafeCanonicalizationMethods)
            {
                if (string.Equals(str, this.SignedInfo.CanonicalizationMethod, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool DoesSignatureUseTruncatedHmac()
        {
            if ((this.SignedInfo == null) || (this.SignedInfo.SignatureLength == null))
            {
                return false;
            }
            HMAC hmac = CryptoConfig.CreateFromName(this.SignatureMethod) as HMAC;
            if (hmac == null)
            {
                if (string.Equals(this.SignatureMethod, "http://www.w3.org/2000/09/xmldsig#hmac-sha1", StringComparison.Ordinal))
                {
                    hmac = new HMACSHA1();
                }
                else if (string.Equals(this.SignatureMethod, "http://www.w3.org/2001/04/xmldsig-more#hmac-md5", StringComparison.Ordinal))
                {
                    hmac = new HMACMD5();
                }
            }
            if (hmac == null)
            {
                return false;
            }
            int result = 0;
            if (!int.TryParse(this.SignedInfo.SignatureLength, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
            {
                return true;
            }
            int num2 = Math.Max(80, hmac.HashSize / 2);
            return (result < num2);
        }

        private byte[] GetC14NDigest(HashAlgorithm hash)
        {
            if (!this.bCacheValid || !this.SignedInfo.CacheValid)
            {
                string baseURI = this.m_containingDocument?.BaseURI;
                XmlResolver xmlResolver = this.m_bResolverSet ? this.m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), baseURI);
                XmlDocument document = System.Security.Cryptography.Xml.Utils.PreProcessElementInput(this.SignedInfo.GetXml(), xmlResolver, baseURI);
                CanonicalXmlNodeList namespaces = (this.m_context == null) ? null : System.Security.Cryptography.Xml.Utils.GetPropagatedAttributes(this.m_context);
                System.Security.Cryptography.Xml.Utils.AddNamespaces(document.DocumentElement, namespaces);
                Transform canonicalizationMethodObject = this.SignedInfo.CanonicalizationMethodObject;
                canonicalizationMethodObject.Resolver = xmlResolver;
                canonicalizationMethodObject.BaseURI = baseURI;
                canonicalizationMethodObject.LoadInput(document);
                this._digestedSignedInfo = canonicalizationMethodObject.GetDigestedOutput(hash);
                this.bCacheValid = true;
            }
            return this._digestedSignedInfo;
        }

        public virtual XmlElement GetIdElement(XmlDocument document, string idValue) => 
            DefaultGetIdElement(document, idValue);

        private AsymmetricAlgorithm GetNextCertificatePublicKey()
        {
            while (this.m_x509Enum.MoveNext())
            {
                X509Certificate2 current = (X509Certificate2) this.m_x509Enum.Current;
                if (current != null)
                {
                    return current.PublicKey.Key;
                }
            }
            return null;
        }

        protected virtual AsymmetricAlgorithm GetPublicKey()
        {
            if (this.KeyInfo == null)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_KeyInfoRequired"));
            }
            if (this.m_x509Enum != null)
            {
                AsymmetricAlgorithm nextCertificatePublicKey = this.GetNextCertificatePublicKey();
                if (nextCertificatePublicKey != null)
                {
                    return nextCertificatePublicKey;
                }
            }
            if (this.m_keyInfoEnum == null)
            {
                this.m_keyInfoEnum = this.KeyInfo.GetEnumerator();
            }
            while (this.m_keyInfoEnum.MoveNext())
            {
                RSAKeyValue current = this.m_keyInfoEnum.Current as RSAKeyValue;
                if (current != null)
                {
                    return current.Key;
                }
                DSAKeyValue value3 = this.m_keyInfoEnum.Current as DSAKeyValue;
                if (value3 != null)
                {
                    return value3.Key;
                }
                KeyInfoX509Data data = this.m_keyInfoEnum.Current as KeyInfoX509Data;
                if (data != null)
                {
                    this.m_x509Collection = System.Security.Cryptography.Xml.Utils.BuildBagOfCerts(data, CertUsageType.Verification);
                    if (this.m_x509Collection.Count > 0)
                    {
                        this.m_x509Enum = this.m_x509Collection.GetEnumerator();
                        AsymmetricAlgorithm algorithm2 = this.GetNextCertificatePublicKey();
                        if (algorithm2 != null)
                        {
                            return algorithm2;
                        }
                    }
                }
            }
            return null;
        }

        private int GetReferenceLevel(int index, ArrayList references)
        {
            if (this.m_refProcessed[index])
            {
                return this.m_refLevelCache[index];
            }
            this.m_refProcessed[index] = true;
            Reference reference = (Reference) references[index];
            if (((reference.Uri == null) || (reference.Uri.Length == 0)) || ((reference.Uri.Length > 0) && (reference.Uri[0] != '#')))
            {
                this.m_refLevelCache[index] = 0;
                return 0;
            }
            if ((reference.Uri.Length <= 0) || (reference.Uri[0] != '#'))
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
            }
            string str = System.Security.Cryptography.Xml.Utils.ExtractIdFromLocalUri(reference.Uri);
            if (str == "xpointer(/)")
            {
                this.m_refLevelCache[index] = 0;
                return 0;
            }
            for (int i = 0; i < references.Count; i++)
            {
                if (((Reference) references[i]).Id == str)
                {
                    this.m_refLevelCache[index] = this.GetReferenceLevel(i, references) + 1;
                    return this.m_refLevelCache[index];
                }
            }
            this.m_refLevelCache[index] = 0;
            return 0;
        }

        private static XmlElement GetSingleReferenceTarget(XmlDocument document, string idAttributeName, string idValue)
        {
            string xpath = "//*[@" + idAttributeName + "=\"" + idValue + "\"]";
            if (System.Security.Cryptography.Xml.Utils.AllowAmbiguousReferenceTargets())
            {
                return (document.SelectSingleNode(xpath) as XmlElement);
            }
            XmlNodeList list = document.SelectNodes(xpath);
            if ((list == null) || (list.Count == 0))
            {
                return null;
            }
            if (list.Count != 1)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
            }
            return (list[0] as XmlElement);
        }

        public XmlElement GetXml()
        {
            if (this.m_containingDocument != null)
            {
                return this.m_signature.GetXml(this.m_containingDocument);
            }
            return this.m_signature.GetXml();
        }

        private void Initialize(XmlElement element)
        {
            this.m_containingDocument = element?.OwnerDocument;
            this.m_context = element;
            this.m_signature = new System.Security.Cryptography.Xml.Signature();
            this.m_signature.SignedXml = this;
            this.m_signature.SignedInfo = new System.Security.Cryptography.Xml.SignedInfo();
            this.m_signingKey = null;
        }

        private bool IsSafeTransform(string transformAlgorithm)
        {
            foreach (string str in SafeCanonicalizationMethods)
            {
                if (string.Equals(str, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            foreach (string str2 in DefaultSafeTransformMethods)
            {
                if (string.Equals(str2, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadXml(XmlElement value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.m_signature.LoadXml(value);
            this.m_context = value;
            this.bCacheValid = false;
        }

        [RegistryPermission(SecurityAction.Assert, Unrestricted=true)]
        private static List<string> ReadAdditionalSafeCanonicalizationMethods() => 
            ReadFxSecurityStringValues("SafeCanonicalizationMethods");

        [RegistryPermission(SecurityAction.Assert, Unrestricted=true)]
        private static List<string> ReadAdditionalSafeTransformMethods() => 
            ReadFxSecurityStringValues("SafeTransformMethods");

        private static List<string> ReadFxSecurityStringValues(string subkey)
        {
            List<string> list = new List<string>();
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\Security\" + subkey, false))
                {
                    if (key != null)
                    {
                        foreach (string str in key.GetValueNames())
                        {
                            if (key.GetValueKind(str) == RegistryValueKind.String)
                            {
                                string str2 = key.GetValue(str) as string;
                                if (!string.IsNullOrEmpty(str2))
                                {
                                    list.Add(str2);
                                }
                            }
                        }
                    }
                    return list;
                }
            }
            catch (SecurityException)
            {
            }
            return list;
        }

        [RegistryPermission(SecurityAction.Assert, Unrestricted=true)]
        private static bool ReadHmacTruncationSetting()
        {
            bool flag;
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework", false))
                {
                    if (key == null)
                    {
                        return false;
                    }
                    object obj2 = key.GetValue("AllowHMACTruncation");
                    if (obj2 == null)
                    {
                        return false;
                    }
                    if (key.GetValueKind("AllowHMACTruncation") != RegistryValueKind.DWord)
                    {
                        return false;
                    }
                    flag = ((int) obj2) != 0;
                }
            }
            catch (SecurityException)
            {
                flag = false;
            }
            return flag;
        }

        private bool ReferenceUsesSafeTransformMethods(Reference reference)
        {
            TransformChain transformChain = reference.TransformChain;
            int count = transformChain.Count;
            for (int i = 0; i < count; i++)
            {
                Transform transform = transformChain[i];
                if (!this.IsSafeTransform(transform.Algorithm))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AllowHmacTruncation
        {
            get
            {
                if (!s_allowHmacTruncation.HasValue)
                {
                    s_allowHmacTruncation = new bool?(ReadHmacTruncationSetting());
                }
                return s_allowHmacTruncation.Value;
            }
        }

        private static IList<string> DefaultSafeTransformMethods
        {
            get
            {
                if (s_defaultSafeTransformMethods == null)
                {
                    List<string> list = ReadAdditionalSafeTransformMethods();
                    list.Add("http://www.w3.org/2000/09/xmldsig#enveloped-signature");
                    list.Add("http://www.w3.org/2000/09/xmldsig#base64");
                    list.Add("urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform");
                    list.Add("http://www.w3.org/2002/07/decrypt#XML");
                    s_defaultSafeTransformMethods = list;
                }
                return s_defaultSafeTransformMethods;
            }
        }

        [ComVisible(false)]
        public System.Security.Cryptography.Xml.EncryptedXml EncryptedXml
        {
            get
            {
                if (this.m_exml == null)
                {
                    this.m_exml = new System.Security.Cryptography.Xml.EncryptedXml(this.m_containingDocument);
                }
                return this.m_exml;
            }
            set
            {
                this.m_exml = value;
            }
        }

        public System.Security.Cryptography.Xml.KeyInfo KeyInfo
        {
            get => 
                this.m_signature.KeyInfo;
            set
            {
                this.m_signature.KeyInfo = value;
            }
        }

        [ComVisible(false)]
        public XmlResolver Resolver
        {
            set
            {
                this.m_xmlResolver = value;
                this.m_bResolverSet = true;
            }
        }

        internal bool ResolverSet =>
            this.m_bResolverSet;

        private static IList<string> SafeCanonicalizationMethods
        {
            get
            {
                if (s_safeCanonicalizationMethods == null)
                {
                    List<string> list = ReadAdditionalSafeCanonicalizationMethods();
                    list.Add("http://www.w3.org/TR/2001/REC-xml-c14n-20010315");
                    list.Add("http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments");
                    list.Add("http://www.w3.org/2001/10/xml-exc-c14n#");
                    list.Add("http://www.w3.org/2001/10/xml-exc-c14n#WithComments");
                    s_safeCanonicalizationMethods = list;
                }
                return s_safeCanonicalizationMethods;
            }
        }

        public System.Security.Cryptography.Xml.Signature Signature =>
            this.m_signature;

        public string SignatureLength =>
            this.m_signature.SignedInfo.SignatureLength;

        public string SignatureMethod =>
            this.m_signature.SignedInfo.SignatureMethod;

        public byte[] SignatureValue =>
            this.m_signature.SignatureValue;

        public System.Security.Cryptography.Xml.SignedInfo SignedInfo =>
            this.m_signature.SignedInfo;

        public AsymmetricAlgorithm SigningKey
        {
            get => 
                this.m_signingKey;
            set
            {
                this.m_signingKey = value;
            }
        }

        public string SigningKeyName
        {
            get => 
                this.m_strSigningKeyName;
            set
            {
                this.m_strSigningKeyName = value;
            }
        }

        private class ReferenceLevelSortOrder : IComparer
        {
            private ArrayList m_references;

            public int Compare(object a, object b)
            {
                Reference reference = a as Reference;
                Reference reference2 = b as Reference;
                int index = 0;
                int num2 = 0;
                int num3 = 0;
                foreach (Reference reference3 in this.References)
                {
                    if (reference3 == reference)
                    {
                        index = num3;
                    }
                    if (reference3 == reference2)
                    {
                        num2 = num3;
                    }
                    num3++;
                }
                int referenceLevel = reference.SignedXml.GetReferenceLevel(index, this.References);
                int num5 = reference2.SignedXml.GetReferenceLevel(num2, this.References);
                return referenceLevel.CompareTo(num5);
            }

            public ArrayList References
            {
                get => 
                    this.m_references;
                set
                {
                    this.m_references = value;
                }
            }
        }
    }
}

