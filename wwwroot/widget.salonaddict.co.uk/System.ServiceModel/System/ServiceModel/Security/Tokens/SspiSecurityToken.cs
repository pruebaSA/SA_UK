﻿namespace System.ServiceModel.Security.Tokens
{
    using System;
    using System.Collections.ObjectModel;
    using System.IdentityModel.Tokens;
    using System.Net;
    using System.Security.Principal;
    using System.ServiceModel.Security;

    public class SspiSecurityToken : SecurityToken
    {
        private bool allowNtlm;
        private bool allowUnauthenticatedCallers;
        private DateTime effectiveTime;
        private DateTime expirationTime;
        private bool extractGroupsForWindowsAccounts;
        private string id;
        private TokenImpersonationLevel impersonationLevel;
        private System.Net.NetworkCredential networkCredential;

        public SspiSecurityToken(System.Net.NetworkCredential networkCredential, bool extractGroupsForWindowsAccounts, bool allowUnauthenticatedCallers)
        {
            this.networkCredential = System.ServiceModel.Security.SecurityUtils.GetNetworkCredentialsCopy(networkCredential);
            this.extractGroupsForWindowsAccounts = extractGroupsForWindowsAccounts;
            this.allowUnauthenticatedCallers = allowUnauthenticatedCallers;
            this.effectiveTime = DateTime.UtcNow;
            this.expirationTime = this.effectiveTime.AddHours(10.0);
        }

        public SspiSecurityToken(TokenImpersonationLevel impersonationLevel, bool allowNtlm, System.Net.NetworkCredential networkCredential)
        {
            this.impersonationLevel = impersonationLevel;
            this.allowNtlm = allowNtlm;
            this.networkCredential = System.ServiceModel.Security.SecurityUtils.GetNetworkCredentialsCopy(networkCredential);
            this.effectiveTime = DateTime.UtcNow;
            this.expirationTime = this.effectiveTime.AddHours(10.0);
        }

        public bool AllowNtlm =>
            this.allowNtlm;

        public bool AllowUnauthenticatedCallers =>
            this.allowUnauthenticatedCallers;

        public bool ExtractGroupsForWindowsAccounts =>
            this.extractGroupsForWindowsAccounts;

        public override string Id
        {
            get
            {
                if (this.id == null)
                {
                    this.id = SecurityUniqueId.Create().Value;
                }
                return this.id;
            }
        }

        public TokenImpersonationLevel ImpersonationLevel =>
            this.impersonationLevel;

        public System.Net.NetworkCredential NetworkCredential =>
            this.networkCredential;

        public override ReadOnlyCollection<SecurityKey> SecurityKeys =>
            EmptyReadOnlyCollection<SecurityKey>.Instance;

        public override DateTime ValidFrom =>
            this.effectiveTime;

        public override DateTime ValidTo =>
            this.expirationTime;
    }
}

