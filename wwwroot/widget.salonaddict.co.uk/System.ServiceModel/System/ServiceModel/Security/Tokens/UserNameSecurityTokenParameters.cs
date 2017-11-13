namespace System.ServiceModel.Security.Tokens
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;

    public class UserNameSecurityTokenParameters : SecurityTokenParameters
    {
        public UserNameSecurityTokenParameters()
        {
            base.RequireDerivedKeys = false;
        }

        protected UserNameSecurityTokenParameters(UserNameSecurityTokenParameters other) : base(other)
        {
            base.RequireDerivedKeys = false;
        }

        protected override SecurityTokenParameters CloneCore() => 
            new UserNameSecurityTokenParameters(this);

        protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle) => 
            base.CreateKeyIdentifierClause<SecurityKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);

        protected internal override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
        {
            requirement.TokenType = SecurityTokenTypes.UserName;
            requirement.RequireCryptographicToken = false;
        }

        protected internal override bool HasAsymmetricKey =>
            false;

        protected internal override bool SupportsClientAuthentication =>
            true;

        protected internal override bool SupportsClientWindowsIdentity =>
            true;

        protected internal override bool SupportsServerAuthentication =>
            false;
    }
}

