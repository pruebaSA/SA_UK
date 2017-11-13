namespace System.ServiceModel.Security.Tokens
{
    using System;
    using System.Collections.ObjectModel;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel.Security;

    internal class NonValidatingSecurityTokenAuthenticator<TTokenType> : SecurityTokenAuthenticator
    {
        protected override bool CanValidateTokenCore(SecurityToken token) => 
            (token is TTokenType);

        protected override ReadOnlyCollection<IAuthorizationPolicy> ValidateTokenCore(SecurityToken token) => 
            EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance;
    }
}

