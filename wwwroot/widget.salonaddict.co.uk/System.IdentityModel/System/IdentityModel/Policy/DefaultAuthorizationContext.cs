﻿namespace System.IdentityModel.Policy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IdentityModel;

    internal class DefaultAuthorizationContext : AuthorizationContext
    {
        private ReadOnlyCollection<ClaimSet> claimSets;
        private static DefaultAuthorizationContext empty;
        private DateTime expirationTime;
        private SecurityUniqueId id;
        private IDictionary<string, object> properties;

        public DefaultAuthorizationContext(DefaultEvaluationContext evaluationContext)
        {
            this.claimSets = evaluationContext.ClaimSets;
            this.expirationTime = evaluationContext.ExpirationTime;
            this.properties = evaluationContext.Properties;
        }

        public override ReadOnlyCollection<ClaimSet> ClaimSets =>
            this.claimSets;

        public static DefaultAuthorizationContext Empty
        {
            get
            {
                if (empty == null)
                {
                    empty = new DefaultAuthorizationContext(new DefaultEvaluationContext());
                }
                return empty;
            }
        }

        public override DateTime ExpirationTime =>
            this.expirationTime;

        public override string Id =>
            this.id?.Value;

        public override IDictionary<string, object> Properties =>
            this.properties;
    }
}

