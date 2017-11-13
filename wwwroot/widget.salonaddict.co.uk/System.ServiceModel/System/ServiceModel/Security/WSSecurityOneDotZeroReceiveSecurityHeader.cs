namespace System.ServiceModel.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IdentityModel;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Security.Tokens;
    using System.Xml;

    internal class WSSecurityOneDotZeroReceiveSecurityHeader : ReceiveSecurityHeader
    {
        private List<string> earlyDecryptedDataReferences;
        private WrappedKeySecurityToken pendingDecryptionToken;
        private ReferenceList pendingReferenceList;
        private SignedXml pendingSignature;

        public WSSecurityOneDotZeroReceiveSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay, SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithmSuite, int headerIndex, MessageDirection transferDirection) : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, headerIndex, transferDirection)
        {
        }

        protected static SymmetricAlgorithm CreateDecryptionAlgorithm(SecurityToken token, string encryptionMethod, SecurityAlgorithmSuite suite)
        {
            if (encryptionMethod == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("EncryptionMethodMissingInEncryptedData")));
            }
            suite.EnsureAcceptableEncryptionAlgorithm(encryptionMethod);
            SymmetricSecurityKey securityKey = System.ServiceModel.Security.SecurityUtils.GetSecurityKey<SymmetricSecurityKey>(token);
            if (securityKey == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("TokenCannotCreateSymmetricCrypto", new object[] { token })));
            }
            suite.EnsureAcceptableDecryptionSymmetricKeySize(securityKey, token);
            SymmetricAlgorithm symmetricAlgorithm = securityKey.GetSymmetricAlgorithm(encryptionMethod);
            if (symmetricAlgorithm == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("UnableToCreateSymmetricAlgorithmFromToken", new object[] { encryptionMethod })));
            }
            return symmetricAlgorithm;
        }

        private void DecryptBody(XmlDictionaryReader bodyContentReader, SecurityToken token)
        {
            EncryptedData data = new EncryptedData {
                SecurityTokenSerializer = base.StandardsManager.SecurityTokenSerializer
            };
            data.ReadFrom(bodyContentReader, base.MaxReceivedMessageSize);
            if (!bodyContentReader.EOF && (bodyContentReader.NodeType != XmlNodeType.EndElement))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(System.ServiceModel.SR.GetString("BadEncryptedBody")));
            }
            if (token == null)
            {
                token = ResolveKeyIdentifier(data.KeyIdentifier, base.PrimaryTokenResolver, false);
            }
            base.RecordEncryptionToken(token);
            using (SymmetricAlgorithm algorithm = CreateDecryptionAlgorithm(token, data.EncryptionMethod, base.AlgorithmSuite))
            {
                data.SetUpDecryption(algorithm);
                base.SecurityVerifiedMessage.SetDecryptedBody(data.GetDecryptedBuffer());
            }
        }

        protected virtual DecryptedHeader DecryptHeader(XmlDictionaryReader reader, WrappedKeySecurityToken wrappedKeyToken)
        {
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("HeaderDecryptionNotSupportedInWsSecurityJan2004")));
        }

        protected override byte[] DecryptSecurityHeaderElement(EncryptedData encryptedData, WrappedKeySecurityToken wrappedKeyToken, out SecurityToken encryptionToken)
        {
            if ((encryptedData.KeyIdentifier != null) || (wrappedKeyToken == null))
            {
                encryptionToken = ResolveKeyIdentifier(encryptedData.KeyIdentifier, base.CombinedPrimaryTokenResolver, false);
                if ((((wrappedKeyToken != null) && (wrappedKeyToken.ReferenceList != null)) && (encryptedData.HasId && wrappedKeyToken.ReferenceList.ContainsReferredId(encryptedData.Id))) && (wrappedKeyToken != encryptionToken))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("EncryptedKeyWasNotEncryptedWithTheRequiredEncryptingToken", new object[] { wrappedKeyToken })));
                }
            }
            else
            {
                encryptionToken = wrappedKeyToken;
            }
            using (SymmetricAlgorithm algorithm = CreateDecryptionAlgorithm(encryptionToken, encryptedData.EncryptionMethod, base.AlgorithmSuite))
            {
                encryptedData.SetUpDecryption(algorithm);
                return encryptedData.GetDecryptedBuffer();
            }
        }

        protected override WrappedKeySecurityToken DecryptWrappedKey(XmlDictionaryReader reader)
        {
            WrappedKeySecurityToken token = (WrappedKeySecurityToken) base.StandardsManager.SecurityTokenSerializer.ReadToken(reader, base.PrimaryTokenResolver);
            base.AlgorithmSuite.EnsureAcceptableKeyWrapAlgorithm(token.WrappingAlgorithm, token.WrappingSecurityKey is AsymmetricSecurityKey);
            return token;
        }

        protected override void EnsureDecryptionComplete()
        {
            if (this.earlyDecryptedDataReferences != null)
            {
                for (int i = 0; i < this.earlyDecryptedDataReferences.Count; i++)
                {
                    if (!this.TryDeleteReferenceListEntry(this.earlyDecryptedDataReferences[i]))
                    {
                        throw TraceUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("UnexpectedEncryptedElementInSecurityHeader")), base.Message);
                    }
                }
            }
            if (this.HasPendingDecryptionItem())
            {
                throw TraceUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("UnableToResolveDataReference", new object[] { this.pendingReferenceList.GetReferredId(0) })), base.Message);
            }
        }

        private bool EnsureDigestValidityIfIdMatches(SignedInfo signedInfo, string id, XmlDictionaryReader reader, bool doSoapAttributeChecks, MessagePartSpecification signatureParts, MessageHeaderInfo info, bool checkForTokensAtHeaders)
        {
            if (signedInfo == null)
            {
                return false;
            }
            if (doSoapAttributeChecks)
            {
                this.VerifySoapAttributeMatchForHeader(info, signatureParts, reader);
            }
            bool flag = false;
            bool flag2 = checkForTokensAtHeaders && base.StandardsManager.SecurityTokenSerializer.CanReadToken(reader);
            try
            {
                flag = signedInfo.EnsureDigestValidityIfIdMatches(id, reader);
            }
            catch (CryptographicException exception)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("FailedSignatureVerification"), exception));
            }
            if (flag && flag2)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("SecurityTokenFoundOutsideSecurityHeader", new object[] { info.Namespace, info.Name })));
            }
            return flag;
        }

        protected override void ExecuteMessageProtectionPass(bool hasAtLeastOneSupportingTokenExpectedToBeSigned)
        {
            bool flag8;
            bool flag9;
            SignatureTargetIdManager idManager = base.StandardsManager.IdManager;
            MessagePartSpecification specification = base.RequiredEncryptionParts ?? MessagePartSpecification.NoParts;
            MessagePartSpecification signatureParts = base.RequiredSignatureParts ?? MessagePartSpecification.NoParts;
            bool checkForTokensAtHeaders = hasAtLeastOneSupportingTokenExpectedToBeSigned;
            bool doSoapAttributeChecks = !signatureParts.IsBodyIncluded;
            bool encryptBeforeSignMode = base.EncryptBeforeSignMode;
            SignedInfo signedInfo = this.pendingSignature?.Signature.SignedInfo;
            SignatureConfirmations sentSignatureConfirmations = base.GetSentSignatureConfirmations();
            if (((sentSignatureConfirmations != null) && (sentSignatureConfirmations.Count > 0)) && sentSignatureConfirmations.IsMarkedForEncryption)
            {
                base.VerifySignatureEncryption();
            }
            MessageHeaders headers = base.SecurityVerifiedMessage.Headers;
            XmlDictionaryReader readerAtFirstHeader = base.SecurityVerifiedMessage.GetReaderAtFirstHeader();
            bool atLeastOneHeaderOrBodyEncrypted = false;
            for (int i = 0; i < headers.Count; i++)
            {
                if (readerAtFirstHeader.NodeType != XmlNodeType.Element)
                {
                    readerAtFirstHeader.MoveToContent();
                }
                string str = idManager.ExtractId(readerAtFirstHeader);
                base.ElementManager.VerifyUniquenessAndSetHeaderId(str, i);
                if (i == base.HeaderIndex)
                {
                    readerAtFirstHeader.Skip();
                }
                else
                {
                    bool flag6;
                    MessageHeaderInfo info = headers[i];
                    bool flag5 = (str != null) && this.TryDeleteReferenceListEntry(str);
                    if (!flag5 && specification.IsHeaderIncluded(info.Name, info.Namespace))
                    {
                        base.SecurityVerifiedMessage.OnUnencryptedPart(info.Name, info.Namespace);
                    }
                    if ((!flag5 || encryptBeforeSignMode) && (str != null))
                    {
                        flag6 = this.EnsureDigestValidityIfIdMatches(signedInfo, str, readerAtFirstHeader, doSoapAttributeChecks, signatureParts, info, checkForTokensAtHeaders);
                    }
                    else
                    {
                        flag6 = false;
                    }
                    if (flag5)
                    {
                        XmlDictionaryReader reader = flag6 ? headers.GetReaderAtHeader(i) : readerAtFirstHeader;
                        DecryptedHeader header = this.DecryptHeader(reader, this.pendingDecryptionToken);
                        info = header;
                        str = header.Id;
                        base.ElementManager.VerifyUniquenessAndSetDecryptedHeaderId(str, i);
                        headers.ReplaceAt(i, header);
                        if (!object.ReferenceEquals(reader, readerAtFirstHeader))
                        {
                            reader.Close();
                        }
                        if (!encryptBeforeSignMode && (str != null))
                        {
                            XmlDictionaryReader headerReader = header.GetHeaderReader();
                            flag6 = this.EnsureDigestValidityIfIdMatches(signedInfo, str, headerReader, doSoapAttributeChecks, signatureParts, info, checkForTokensAtHeaders);
                            headerReader.Close();
                        }
                    }
                    if (!flag6 && signatureParts.IsHeaderIncluded(info.Name, info.Namespace))
                    {
                        base.SecurityVerifiedMessage.OnUnsignedPart(info.Name, info.Namespace);
                    }
                    if (flag6 && flag5)
                    {
                        base.VerifySignatureEncryption();
                    }
                    if (flag5 && !flag6)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("EncryptedHeaderNotSigned", new object[] { info.Name, info.Namespace })));
                    }
                    if (!flag6 && !flag5)
                    {
                        readerAtFirstHeader.Skip();
                    }
                    atLeastOneHeaderOrBodyEncrypted |= flag5;
                }
            }
            readerAtFirstHeader.ReadEndElement();
            if (readerAtFirstHeader.NodeType != XmlNodeType.Element)
            {
                readerAtFirstHeader.MoveToContent();
            }
            string id = idManager.ExtractId(readerAtFirstHeader);
            base.ElementManager.VerifyUniquenessAndSetBodyId(id);
            base.SecurityVerifiedMessage.SetBodyPrefixAndAttributes(readerAtFirstHeader);
            bool flag7 = specification.IsBodyIncluded || this.HasPendingDecryptionItem();
            if ((!flag7 || encryptBeforeSignMode) && (id != null))
            {
                flag8 = this.EnsureDigestValidityIfIdMatches(signedInfo, id, readerAtFirstHeader, false, null, null, false);
            }
            else
            {
                flag8 = false;
            }
            if (flag7)
            {
                XmlDictionaryReader reader4 = flag8 ? base.SecurityVerifiedMessage.CreateFullBodyReader() : readerAtFirstHeader;
                reader4.ReadStartElement();
                string str3 = idManager.ExtractId(reader4);
                base.ElementManager.VerifyUniquenessAndSetBodyContentId(str3);
                flag9 = (str3 != null) && this.TryDeleteReferenceListEntry(str3);
                if (flag9)
                {
                    this.DecryptBody(reader4, this.pendingDecryptionToken);
                }
                if (!object.ReferenceEquals(reader4, readerAtFirstHeader))
                {
                    reader4.Close();
                }
                if ((!encryptBeforeSignMode && (signedInfo != null)) && signedInfo.HasUnverifiedReference(id))
                {
                    reader4 = base.SecurityVerifiedMessage.CreateFullBodyReader();
                    flag8 = this.EnsureDigestValidityIfIdMatches(signedInfo, id, reader4, false, null, null, false);
                    reader4.Close();
                }
            }
            else
            {
                flag9 = false;
            }
            if (flag8 && flag9)
            {
                base.VerifySignatureEncryption();
            }
            readerAtFirstHeader.Close();
            if (this.pendingSignature != null)
            {
                this.pendingSignature.CompleteSignatureVerification();
                this.pendingSignature = null;
            }
            this.pendingDecryptionToken = null;
            atLeastOneHeaderOrBodyEncrypted |= flag9;
            if (!flag8 && signatureParts.IsBodyIncluded)
            {
                base.SecurityVerifiedMessage.OnUnsignedPart(System.ServiceModel.XD.MessageDictionary.Body.Value, base.Version.Envelope.Namespace);
            }
            if (!flag9 && specification.IsBodyIncluded)
            {
                base.SecurityVerifiedMessage.OnUnencryptedPart(System.ServiceModel.XD.MessageDictionary.Body.Value, base.Version.Envelope.Namespace);
            }
            base.SecurityVerifiedMessage.OnMessageProtectionPassComplete(atLeastOneHeaderOrBodyEncrypted);
        }

        private bool HasPendingDecryptionItem() => 
            ((this.pendingReferenceList != null) && (this.pendingReferenceList.DataReferenceCount > 0));

        protected override bool IsReaderAtEncryptedData(XmlDictionaryReader reader)
        {
            bool flag = reader.IsStartElement(EncryptedData.ElementName, System.ServiceModel.XD.XmlEncryptionDictionary.Namespace);
            if (flag)
            {
                base.HasAtLeastOneItemInsideSecurityHeaderEncrypted = true;
            }
            return flag;
        }

        protected override bool IsReaderAtEncryptedKey(XmlDictionaryReader reader) => 
            reader.IsStartElement(EncryptedKey.ElementName, System.ServiceModel.XD.XmlEncryptionDictionary.Namespace);

        protected override bool IsReaderAtReferenceList(XmlDictionaryReader reader) => 
            reader.IsStartElement(ReferenceList.ElementName, ReferenceList.NamespaceUri);

        protected override bool IsReaderAtSecurityTokenReference(XmlDictionaryReader reader) => 
            reader.IsStartElement(System.ServiceModel.XD.SecurityJan2004Dictionary.SecurityTokenReference, System.ServiceModel.XD.SecurityJan2004Dictionary.Namespace);

        protected override bool IsReaderAtSignature(XmlDictionaryReader reader) => 
            reader.IsStartElement(System.ServiceModel.XD.XmlSignatureDictionary.Signature, System.ServiceModel.XD.XmlSignatureDictionary.Namespace);

        protected override void OnDecryptionOfSecurityHeaderItemRequiringReferenceListEntry(string id)
        {
            if (!this.TryDeleteReferenceListEntry(id))
            {
                if (this.earlyDecryptedDataReferences == null)
                {
                    this.earlyDecryptedDataReferences = new List<string>(4);
                }
                this.earlyDecryptedDataReferences.Add(id);
            }
        }

        protected override void ProcessReferenceListCore(ReferenceList referenceList, WrappedKeySecurityToken wrappedKeyToken)
        {
            this.pendingReferenceList = referenceList;
            this.pendingDecryptionToken = wrappedKeyToken;
        }

        protected override ReferenceList ReadReferenceListCore(XmlDictionaryReader reader)
        {
            ReferenceList list = new ReferenceList();
            list.ReadFrom(reader);
            return list;
        }

        protected override EncryptedData ReadSecurityHeaderEncryptedItem(XmlDictionaryReader reader)
        {
            EncryptedData data = new EncryptedData {
                SecurityTokenSerializer = base.StandardsManager.SecurityTokenSerializer
            };
            data.ReadFrom(reader);
            return data;
        }

        protected override void ReadSecurityTokenReference(XmlDictionaryReader reader)
        {
            string attribute = reader.GetAttribute(System.ServiceModel.XD.UtilityDictionary.IdAttribute, System.ServiceModel.XD.UtilityDictionary.Namespace);
            SecurityKeyIdentifierClause strClause = base.StandardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause(reader);
            if (string.IsNullOrEmpty(strClause.Id))
            {
                strClause.Id = attribute;
            }
            if (!string.IsNullOrEmpty(strClause.Id))
            {
                base.ElementManager.AppendSecurityTokenReference(strClause, strClause.Id);
            }
        }

        protected override SignedXml ReadSignatureCore(XmlDictionaryReader signatureReader)
        {
            SignedXml xml = new SignedXml(ServiceModelDictionaryManager.Instance, base.StandardsManager.SecurityTokenSerializer) {
                Signature = { SignedInfo = { ResourcePool = base.ResourcePool } }
            };
            xml.ReadFrom(signatureReader);
            return xml;
        }

        protected static SecurityToken ResolveKeyIdentifier(SecurityKeyIdentifier keyIdentifier, SecurityTokenResolver resolver, bool isFromSignature)
        {
            SecurityToken token;
            if (TryResolveKeyIdentifier(keyIdentifier, resolver, isFromSignature, out token))
            {
                return token;
            }
            if (isFromSignature)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("UnableToResolveKeyInfoForVerifyingSignature", new object[] { keyIdentifier, resolver })));
            }
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("UnableToResolveKeyInfoForDecryption", new object[] { keyIdentifier, resolver })));
        }

        private SecurityToken ResolveSignatureToken(SecurityKeyIdentifier keyIdentifier, SecurityTokenResolver resolver, bool isPrimarySignature)
        {
            SecurityToken token;
            RsaKeyIdentifierClause clause;
            TryResolveKeyIdentifier(keyIdentifier, resolver, true, out token);
            if (((token == null) && !isPrimarySignature) && ((keyIdentifier.Count == 1) && keyIdentifier.TryFind<RsaKeyIdentifierClause>(out clause)))
            {
                RsaSecurityTokenAuthenticator tokenAuthenticator = base.FindAllowedAuthenticator<RsaSecurityTokenAuthenticator>(false);
                if (tokenAuthenticator != null)
                {
                    SupportingTokenAuthenticatorSpecification specification;
                    token = new RsaSecurityToken(clause.Rsa);
                    ReadOnlyCollection<IAuthorizationPolicy> onlys = tokenAuthenticator.ValidateToken(token);
                    TokenTracker supportingTokenTracker = base.GetSupportingTokenTracker(tokenAuthenticator, out specification);
                    if (supportingTokenTracker == null)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(System.ServiceModel.SR.GetString("UnknownTokenAuthenticatorUsedInTokenProcessing", new object[] { tokenAuthenticator })));
                    }
                    supportingTokenTracker.RecordToken(token);
                    base.SecurityTokenAuthorizationPoliciesMapping.Add(token, onlys);
                }
            }
            if (token == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("UnableToResolveKeyInfoForVerifyingSignature", new object[] { keyIdentifier, resolver })));
            }
            return token;
        }

        protected override bool TryDeleteReferenceListEntry(string id) => 
            ((this.pendingReferenceList != null) && this.pendingReferenceList.TryRemoveReferredId(id));

        protected static bool TryResolveKeyIdentifier(SecurityKeyIdentifier keyIdentifier, SecurityTokenResolver resolver, bool isFromSignature, out SecurityToken token)
        {
            if (keyIdentifier != null)
            {
                return resolver.TryResolveToken(keyIdentifier, out token);
            }
            if (isFromSignature)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("NoKeyInfoInSignatureToFindVerificationToken")));
            }
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("NoKeyInfoInEncryptedItemToFindDecryptingToken")));
        }

        private void ValidateDigestsOfTargetsInSecurityHeader(StandardSignedInfo signedInfo, SecurityTimestamp timestamp, bool encryptedFormRequired, bool isPrimarySignature, object signatureTarget, string id)
        {
            for (int i = 0; i < signedInfo.ReferenceCount; i++)
            {
                Reference reference = signedInfo[i];
                base.AlgorithmSuite.EnsureAcceptableDigestAlgorithm(reference.DigestMethod);
                string str = reference.ExtractReferredId();
                if (isPrimarySignature || (id == str))
                {
                    if ((((timestamp != null) && (timestamp.Id == str)) && (!reference.TransformChain.NeedsInclusiveContext && (timestamp.DigestAlgorithm == reference.DigestMethod))) && (timestamp.GetDigest() != null))
                    {
                        reference.EnsureDigestValidity(str, timestamp.GetDigest());
                        base.ElementManager.SetTimestampSigned(str);
                    }
                    else if (signatureTarget != null)
                    {
                        reference.EnsureDigestValidity(id, signatureTarget);
                    }
                    else
                    {
                        int index = -1;
                        XmlDictionaryReader resolvedXmlSource = null;
                        if (reference.IsStrTranform())
                        {
                            if (base.ElementManager.TryGetTokenElementIndexFromStrId(str, out index))
                            {
                                ReceiveSecurityHeaderEntry entry;
                                base.ElementManager.GetElementEntry(index, out entry);
                                bool requiresEncryptedFormReader = (entry.bindingMode == ReceiveSecurityHeaderBindingModes.Signed) || (entry.bindingMode == ReceiveSecurityHeaderBindingModes.SignedEndorsing);
                                if (!base.ElementManager.IsPrimaryTokenSigned)
                                {
                                    base.ElementManager.IsPrimaryTokenSigned = (entry.bindingMode == ReceiveSecurityHeaderBindingModes.Primary) && (entry.elementCategory == ReceiveSecurityHeaderElementCategory.Token);
                                }
                                base.ElementManager.SetSigned(index);
                                resolvedXmlSource = base.ElementManager.GetReader(index, requiresEncryptedFormReader);
                            }
                        }
                        else
                        {
                            resolvedXmlSource = base.ElementManager.GetSignatureVerificationReader(str, encryptedFormRequired);
                        }
                        if (resolvedXmlSource != null)
                        {
                            reference.EnsureDigestValidity(str, resolvedXmlSource);
                            resolvedXmlSource.Close();
                        }
                    }
                    if (!isPrimarySignature)
                    {
                        break;
                    }
                }
            }
            if ((isPrimarySignature && base.RequireSignedPrimaryToken) && !base.ElementManager.IsPrimaryTokenSigned)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("SupportingTokenIsNotSigned", new object[] { new IssuedSecurityTokenParameters() })));
            }
        }

        protected override SecurityToken VerifySignature(SignedXml signedXml, bool isPrimarySignature, SecurityHeaderTokenResolver resolver, object signatureTarget, string id)
        {
            SecurityToken token = this.ResolveSignatureToken(signedXml.Signature.KeyIdentifier, resolver, isPrimarySignature);
            if (isPrimarySignature)
            {
                base.RecordSignatureToken(token);
            }
            ReadOnlyCollection<SecurityKey> securityKeys = token.SecurityKeys;
            SecurityKey securityKey = ((securityKeys != null) && (securityKeys.Count > 0)) ? securityKeys[0] : null;
            if (securityKey == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("UnableToCreateICryptoFromTokenForSignatureVerification", new object[] { token })));
            }
            base.AlgorithmSuite.EnsureAcceptableSignatureKeySize(securityKey, token);
            base.AlgorithmSuite.EnsureAcceptableSignatureAlgorithm(securityKey, signedXml.Signature.SignedInfo.SignatureMethod);
            signedXml.StartSignatureVerification(securityKey);
            StandardSignedInfo signedInfo = (StandardSignedInfo) signedXml.Signature.SignedInfo;
            bool encryptBeforeSignMode = base.EncryptBeforeSignMode;
            this.ValidateDigestsOfTargetsInSecurityHeader(signedInfo, base.Timestamp, encryptBeforeSignMode, isPrimarySignature, signatureTarget, id);
            if (!isPrimarySignature)
            {
                if ((!base.RequireMessageProtection && (securityKey is AsymmetricSecurityKey)) && (base.Version.Addressing != AddressingVersion.None))
                {
                    int headerIndex = base.Message.Headers.FindHeader(System.ServiceModel.XD.AddressingDictionary.To.Value, base.Message.Version.Addressing.Namespace);
                    if (headerIndex == -1)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("TransportSecuredMessageMissingToHeader")));
                    }
                    XmlDictionaryReader readerAtHeader = base.Message.Headers.GetReaderAtHeader(headerIndex);
                    id = readerAtHeader.GetAttribute(System.ServiceModel.XD.UtilityDictionary.IdAttribute, System.ServiceModel.XD.UtilityDictionary.Namespace);
                    if (id == null)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("UnsignedToHeaderInTransportSecuredMessage")));
                    }
                    signedXml.EnsureDigestValidity(id, readerAtHeader);
                }
                signedXml.CompleteSignatureVerification();
                return token;
            }
            this.pendingSignature = signedXml;
            return token;
        }

        private void VerifySoapAttributeMatchForHeader(MessageHeaderInfo info, MessagePartSpecification signatureParts, XmlDictionaryReader reader)
        {
            if (signatureParts.IsHeaderIncluded(info.Name, info.Namespace))
            {
                EnvelopeVersion envelope = base.Version.Envelope;
                EnvelopeVersion version2 = (envelope == EnvelopeVersion.Soap11) ? EnvelopeVersion.Soap12 : EnvelopeVersion.Soap11;
                bool flag = null != reader.GetAttribute(System.ServiceModel.XD.MessageDictionary.MustUnderstand, envelope.DictionaryNamespace);
                if ((null != reader.GetAttribute(System.ServiceModel.XD.MessageDictionary.MustUnderstand, version2.DictionaryNamespace)) && !flag)
                {
                    throw TraceUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("InvalidAttributeInSignedHeader", new object[] { info.Name, info.Namespace, System.ServiceModel.XD.MessageDictionary.MustUnderstand, version2.DictionaryNamespace, System.ServiceModel.XD.MessageDictionary.MustUnderstand, envelope.DictionaryNamespace })), base.SecurityVerifiedMessage);
                }
                flag = null != reader.GetAttribute(envelope.DictionaryActor, envelope.DictionaryNamespace);
                if ((null != reader.GetAttribute(version2.DictionaryActor, version2.DictionaryNamespace)) && !flag)
                {
                    throw TraceUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("InvalidAttributeInSignedHeader", new object[] { info.Name, info.Namespace, version2.DictionaryActor, version2.DictionaryNamespace, envelope.DictionaryActor, envelope.DictionaryNamespace })), base.SecurityVerifiedMessage);
                }
            }
        }
    }
}

