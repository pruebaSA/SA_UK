namespace System.IdentityModel.Tokens
{
    using System;
    using System.Globalization;
    using System.IdentityModel;

    public class SamlAssertionKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        private readonly string assertionId;
        private readonly string authorityKind;
        private readonly string binding;
        private readonly string location;
        private readonly string tokenTypeUri;
        private readonly string valueType;

        public SamlAssertionKeyIdentifierClause(string assertionId) : this(assertionId, null, 0)
        {
        }

        public SamlAssertionKeyIdentifierClause(string assertionId, byte[] derivationNonce, int derivationLength) : this(assertionId, derivationNonce, derivationLength, null, null, null, null, null)
        {
        }

        internal SamlAssertionKeyIdentifierClause(string assertionId, byte[] derivationNonce, int derivationLength, string valueType, string tokenTypeUri, string binding, string location, string authorityKind) : base(null, derivationNonce, derivationLength)
        {
            if (assertionId == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("assertionId");
            }
            this.assertionId = assertionId;
            this.valueType = valueType;
            this.tokenTypeUri = tokenTypeUri;
            this.binding = binding;
            this.location = location;
            this.authorityKind = authorityKind;
        }

        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            SamlAssertionKeyIdentifierClause objB = keyIdentifierClause as SamlAssertionKeyIdentifierClause;
            return (object.ReferenceEquals(this, objB) || ((objB != null) && objB.Matches(this.assertionId)));
        }

        public bool Matches(string assertionId) => 
            (this.assertionId == assertionId);

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "SamlAssertionKeyIdentifierClause(AssertionId = '{0}')", new object[] { this.AssertionId });

        public string AssertionId =>
            this.assertionId;

        internal string AuthorityKind =>
            this.authorityKind;

        internal string Binding =>
            this.binding;

        internal string Location =>
            this.location;

        internal string TokenTypeUri =>
            this.tokenTypeUri;

        internal string ValueType =>
            this.valueType;
    }
}

