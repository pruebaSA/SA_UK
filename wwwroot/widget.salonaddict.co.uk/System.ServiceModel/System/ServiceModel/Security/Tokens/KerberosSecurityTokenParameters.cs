namespace System.ServiceModel.Security.Tokens
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;

    public class KerberosSecurityTokenParameters : SecurityTokenParameters
    {
        public KerberosSecurityTokenParameters()
        {
            base.InclusionMode = SecurityTokenInclusionMode.Once;
        }

        protected KerberosSecurityTokenParameters(KerberosSecurityTokenParameters other) : base(other)
        {
        }

        protected override SecurityTokenParameters CloneCore() => 
            new KerberosSecurityTokenParameters(this);

        protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle) => 
            base.CreateKeyIdentifierClause<KerberosTicketHashKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);

        protected internal override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
        {
            requirement.TokenType = SecurityTokenTypes.Kerberos;
            requirement.KeyType = SecurityKeyType.SymmetricKey;
            requirement.RequireCryptographicToken = true;
        }

        protected internal override bool HasAsymmetricKey =>
            false;

        protected internal override bool SupportsClientAuthentication =>
            true;

        protected internal override bool SupportsClientWindowsIdentity =>
            true;

        protected internal override bool SupportsServerAuthentication =>
            true;
    }
}

