namespace System.ServiceModel.Security
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel;
    using System.IdentityModel.Tokens;
    using System.Runtime.InteropServices;

    internal class SendSecurityHeaderElementContainer
    {
        private List<SendSecurityHeaderElement> basicSupportingTokens;
        public SecurityToken DerivedEncryptionToken;
        public SecurityToken DerivedSigningToken;
        private List<SecurityToken> endorsingDerivedSupportingTokens;
        private List<SendSecurityHeaderElement> endorsingSignatures;
        private List<SecurityToken> endorsingSupportingTokens;
        public SecurityToken PrerequisiteToken;
        public SendSecurityHeaderElement PrimarySignature;
        public ISecurityElement ReferenceList;
        private Dictionary<SecurityToken, SecurityKeyIdentifierClause> securityTokenMappedToIdentifierClause;
        private List<SendSecurityHeaderElement> signatureConfirmations;
        private List<SecurityToken> signedEndorsingDerivedSupportingTokens;
        private List<SecurityToken> signedEndorsingSupportingTokens;
        private List<SecurityToken> signedSupportingTokens;
        public SecurityToken SourceEncryptionToken;
        public SecurityToken SourceSigningToken;
        public SecurityTimestamp Timestamp;
        public SecurityToken WrappedEncryptionToken;

        private void Add<T>(ref List<T> list, T item)
        {
            if (list == null)
            {
                list = new List<T>();
            }
            list.Add(item);
        }

        public void AddBasicSupportingToken(SendSecurityHeaderElement tokenElement)
        {
            this.Add<SendSecurityHeaderElement>(ref this.basicSupportingTokens, tokenElement);
        }

        public void AddEndorsingDerivedSupportingToken(SecurityToken token)
        {
            this.Add<SecurityToken>(ref this.endorsingDerivedSupportingTokens, token);
        }

        public void AddEndorsingSignature(SendSecurityHeaderElement signature)
        {
            this.Add<SendSecurityHeaderElement>(ref this.endorsingSignatures, signature);
        }

        public void AddEndorsingSupportingToken(SecurityToken token)
        {
            this.Add<SecurityToken>(ref this.endorsingSupportingTokens, token);
        }

        public void AddSignatureConfirmation(SendSecurityHeaderElement confirmation)
        {
            this.Add<SendSecurityHeaderElement>(ref this.signatureConfirmations, confirmation);
        }

        public void AddSignedEndorsingDerivedSupportingToken(SecurityToken token)
        {
            this.Add<SecurityToken>(ref this.signedEndorsingDerivedSupportingTokens, token);
        }

        public void AddSignedEndorsingSupportingToken(SecurityToken token)
        {
            this.Add<SecurityToken>(ref this.signedEndorsingSupportingTokens, token);
        }

        public void AddSignedSupportingToken(SecurityToken token)
        {
            this.Add<SecurityToken>(ref this.signedSupportingTokens, token);
        }

        public SendSecurityHeaderElement[] GetBasicSupportingTokens() => 
            this.basicSupportingTokens?.ToArray();

        public SecurityToken[] GetEndorsingDerivedSupportingTokens() => 
            this.endorsingDerivedSupportingTokens?.ToArray();

        public SendSecurityHeaderElement[] GetEndorsingSignatures() => 
            this.endorsingSignatures?.ToArray();

        public SecurityToken[] GetEndorsingSupportingTokens() => 
            this.endorsingSupportingTokens?.ToArray();

        public SendSecurityHeaderElement[] GetSignatureConfirmations() => 
            this.signatureConfirmations?.ToArray();

        public SecurityToken[] GetSignedEndorsingDerivedSupportingTokens() => 
            this.signedEndorsingDerivedSupportingTokens?.ToArray();

        public SecurityToken[] GetSignedEndorsingSupportingTokens() => 
            this.signedEndorsingSupportingTokens?.ToArray();

        public SecurityToken[] GetSignedSupportingTokens() => 
            this.signedSupportingTokens?.ToArray();

        public void MapSecurityTokenToStrClause(SecurityToken securityToken, SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (this.securityTokenMappedToIdentifierClause == null)
            {
                this.securityTokenMappedToIdentifierClause = new Dictionary<SecurityToken, SecurityKeyIdentifierClause>();
            }
            if (!this.securityTokenMappedToIdentifierClause.ContainsKey(securityToken))
            {
                this.securityTokenMappedToIdentifierClause.Add(securityToken, keyIdentifierClause);
            }
        }

        public bool TryGetIdentifierClauseFromSecurityToken(SecurityToken securityToken, out SecurityKeyIdentifierClause keyIdentifierClause)
        {
            keyIdentifierClause = null;
            return (((securityToken != null) && (this.securityTokenMappedToIdentifierClause != null)) && this.securityTokenMappedToIdentifierClause.TryGetValue(securityToken, out keyIdentifierClause));
        }

        public List<SecurityToken> EndorsingSupportingTokens =>
            this.endorsingSupportingTokens;
    }
}

