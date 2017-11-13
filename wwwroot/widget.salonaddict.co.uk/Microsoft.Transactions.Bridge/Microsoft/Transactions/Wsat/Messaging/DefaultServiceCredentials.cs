namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.IdentityModel.Selectors;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    internal class DefaultServiceCredentials : ServiceCredentials
    {
        public DefaultServiceCredentials()
        {
        }

        public DefaultServiceCredentials(DefaultServiceCredentials other) : base(other)
        {
        }

        protected override ServiceCredentials CloneCore() => 
            new DefaultServiceCredentials(this);

        public override SecurityTokenManager CreateSecurityTokenManager() => 
            new DefaultSecurityTokenManager(this);

        private class DefaultSecurityTokenManager : ServiceCredentialsSecurityTokenManager
        {
            private DefaultServiceCredentials serverCreds;

            public DefaultSecurityTokenManager(DefaultServiceCredentials serverCreds) : base(serverCreds)
            {
                this.serverCreds = serverCreds;
            }

            public override EndpointIdentity GetIdentityOfSelf(SecurityTokenRequirement tokenRequirement) => 
                null;
        }
    }
}

