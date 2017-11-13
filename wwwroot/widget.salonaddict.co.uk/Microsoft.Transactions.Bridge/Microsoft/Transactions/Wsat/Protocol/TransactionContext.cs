namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.ServiceModel.Security;

    internal class TransactionContext
    {
        private Microsoft.Transactions.Wsat.Messaging.CoordinationContext context;
        private RequestSecurityTokenResponse issuedToken;

        public TransactionContext(Microsoft.Transactions.Wsat.Messaging.CoordinationContext context, RequestSecurityTokenResponse issuedToken)
        {
            this.context = context;
            this.issuedToken = issuedToken;
        }

        public override string ToString() => 
            this.context.Identifier;

        public Microsoft.Transactions.Wsat.Messaging.CoordinationContext CoordinationContext =>
            this.context;

        public RequestSecurityTokenResponse IssuedToken =>
            this.issuedToken;
    }
}

