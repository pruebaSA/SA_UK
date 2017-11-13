namespace System.ServiceModel.Security
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Security.Tokens;
    using System.Xml;

    internal class WSSecurityXXX2005 : WSSecurityJan2004
    {
        public WSSecurityXXX2005(WSSecurityTokenSerializer tokenSerializer, SamlSerializer samlSerializer) : base(tokenSerializer, samlSerializer)
        {
        }

        public override void PopulateKeyIdentifierClauseEntries(IList<WSSecurityTokenSerializer.KeyIdentifierClauseEntry> clauseEntries)
        {
            List<WSSecurityTokenSerializer.StrEntry> strEntries = new List<WSSecurityTokenSerializer.StrEntry>();
            base.WSSecurityTokenSerializer.PopulateStrEntries(strEntries);
            SecurityTokenReferenceXXX2005ClauseEntry item = new SecurityTokenReferenceXXX2005ClauseEntry(base.WSSecurityTokenSerializer, strEntries);
            clauseEntries.Add(item);
        }

        public override void PopulateStrEntries(IList<WSSecurityTokenSerializer.StrEntry> strEntries)
        {
            base.PopulateJan2004StrEntries(strEntries);
            strEntries.Add(new SamlXXX2005KeyIdentifierStrEntry());
            strEntries.Add(new SamlDirectStrEntry());
            strEntries.Add(new X509ThumbprintStrEntry(base.WSSecurityTokenSerializer));
            strEntries.Add(new EncryptedKeyHashStrEntry(base.WSSecurityTokenSerializer));
        }

        public override void PopulateTokenEntries(IList<WSSecurityTokenSerializer.TokenEntry> tokenEntryList)
        {
            base.PopulateJan2004TokenEntries(tokenEntryList);
            tokenEntryList.Add(new WrappedKeyTokenEntry(base.WSSecurityTokenSerializer));
            tokenEntryList.Add(new SamlTokenEntry(base.WSSecurityTokenSerializer, base.SamlSerializer));
        }

        private class EncryptedKeyHashStrEntry : WSSecurityJan2004.KeyIdentifierStrEntry
        {
            public EncryptedKeyHashStrEntry(WSSecurityTokenSerializer tokenSerializer) : base(tokenSerializer)
            {
            }

            public override bool CanReadClause(XmlDictionaryReader reader, string tokenType)
            {
                if ((tokenType != null) && (tokenType != "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#EncryptedKey"))
                {
                    return false;
                }
                return base.CanReadClause(reader, tokenType);
            }

            protected override SecurityKeyIdentifierClause CreateClause(byte[] bytes, byte[] derivationNonce, int derivationLength) => 
                new EncryptedKeyHashIdentifierClause(bytes, true, derivationNonce, derivationLength);

            public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
            {
                writer.WriteAttributeString(XD.SecurityXXX2005Dictionary.Prefix.Value, XD.SecurityXXX2005Dictionary.TokenTypeAttribute, XD.SecurityXXX2005Dictionary.Namespace, "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#EncryptedKey");
                base.WriteContent(writer, clause);
            }

            protected override Type ClauseType =>
                typeof(EncryptedKeyHashIdentifierClause);

            public override Type TokenType =>
                typeof(WrappedKeySecurityToken);

            protected override string ValueTypeUri =>
                "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#EncryptedKeySHA1";
        }

        private class SamlDirectStrEntry : WSSecurityTokenSerializer.StrEntry
        {
            public override bool CanReadClause(XmlDictionaryReader reader, string tokenType)
            {
                if (tokenType != XD.SecurityXXX2005Dictionary.Saml20TokenType.Value)
                {
                    return false;
                }
                return reader.IsStartElement(XD.SecurityJan2004Dictionary.Reference, XD.SecurityJan2004Dictionary.Namespace);
            }

            public override Type GetTokenType(SecurityKeyIdentifierClause clause) => 
                null;

            public override SecurityKeyIdentifierClause ReadClause(XmlDictionaryReader reader, byte[] derivationNone, int derivationLength, string tokenType)
            {
                string attribute = reader.GetAttribute(XD.SecurityJan2004Dictionary.URI, null);
                if (reader.IsEmptyElement)
                {
                    reader.Read();
                }
                else
                {
                    reader.ReadStartElement();
                    reader.ReadEndElement();
                }
                return new SamlAssertionDirectKeyIdentifierClause(attribute, derivationNone, derivationLength);
            }

            public override bool SupportsCore(SecurityKeyIdentifierClause clause) => 
                typeof(SamlAssertionDirectKeyIdentifierClause).IsAssignableFrom(clause.GetType());

            public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
            {
                writer.WriteAttributeString(XD.SecurityXXX2005Dictionary.Prefix.Value, XD.SecurityXXX2005Dictionary.TokenTypeAttribute, XD.SecurityXXX2005Dictionary.Namespace, XD.SecurityXXX2005Dictionary.Saml20TokenType.Value);
                SamlAssertionDirectKeyIdentifierClause clause2 = clause as SamlAssertionDirectKeyIdentifierClause;
                writer.WriteStartElement(XD.SecurityJan2004Dictionary.Prefix.Value, XD.SecurityJan2004Dictionary.Reference, XD.SecurityJan2004Dictionary.Namespace);
                writer.WriteAttributeString(XD.SecurityJan2004Dictionary.URI, null, clause2.SamlUri);
                writer.WriteEndElement();
            }
        }

        private class SamlTokenEntry : WSSecurityJan2004.SamlTokenEntry
        {
            public SamlTokenEntry(WSSecurityTokenSerializer tokenSerializer, SamlSerializer samlSerializer) : base(tokenSerializer, samlSerializer)
            {
            }

            public override string TokenTypeUri =>
                "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1";
        }

        private class SamlXXX2005KeyIdentifierStrEntry : WSSecurityJan2004.SamlJan2004KeyIdentifierStrEntry
        {
            protected override bool IsMatchingValueType(string valueType)
            {
                if (!base.IsMatchingValueType(valueType))
                {
                    return (valueType == XD.SecurityXXX2005Dictionary.Saml11AssertionValueType.Value);
                }
                return true;
            }

            public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
            {
                SamlAssertionKeyIdentifierClause clause2 = (SamlAssertionKeyIdentifierClause) clause;
                if (clause2.TokenTypeUri != null)
                {
                    writer.WriteAttributeString(XD.SecurityXXX2005Dictionary.Prefix.Value, XD.SecurityXXX2005Dictionary.TokenTypeAttribute, XD.SecurityXXX2005Dictionary.Namespace, clause2.TokenTypeUri);
                }
                base.WriteContent(writer, clause);
            }
        }

        private class SecurityTokenReferenceXXX2005ClauseEntry : WSSecurityJan2004.SecurityTokenReferenceJan2004ClauseEntry
        {
            public SecurityTokenReferenceXXX2005ClauseEntry(WSSecurityTokenSerializer tokenSerializer, IList<WSSecurityTokenSerializer.StrEntry> strEntries) : base(tokenSerializer, strEntries)
            {
            }

            protected override string ReadTokenType(XmlDictionaryReader reader) => 
                reader.GetAttribute(XD.SecurityXXX2005Dictionary.TokenTypeAttribute, XD.SecurityXXX2005Dictionary.Namespace);
        }

        private class WrappedKeyTokenEntry : WSSecurityJan2004.WrappedKeyTokenEntry
        {
            public WrappedKeyTokenEntry(WSSecurityTokenSerializer tokenSerializer) : base(tokenSerializer)
            {
            }

            public override string TokenTypeUri =>
                "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#EncryptedKey";
        }

        private class X509ThumbprintStrEntry : WSSecurityJan2004.KeyIdentifierStrEntry
        {
            public X509ThumbprintStrEntry(WSSecurityTokenSerializer tokenSerializer) : base(tokenSerializer)
            {
            }

            protected override SecurityKeyIdentifierClause CreateClause(byte[] bytes, byte[] derivationNonce, int derivationLength) => 
                new X509ThumbprintKeyIdentifierClause(bytes);

            protected override Type ClauseType =>
                typeof(X509ThumbprintKeyIdentifierClause);

            public override Type TokenType =>
                typeof(X509SecurityToken);

            protected override string ValueTypeUri =>
                "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#ThumbprintSHA1";
        }
    }
}

