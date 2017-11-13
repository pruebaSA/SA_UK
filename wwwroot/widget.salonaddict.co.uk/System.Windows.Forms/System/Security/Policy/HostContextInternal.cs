namespace System.Security.Policy
{
    using System;

    internal class HostContextInternal
    {
        private bool ignorePersistedDecision;
        private bool noPrompt;
        private bool persist;
        private ApplicationIdentity previousAppId;

        public HostContextInternal(TrustManagerContext trustManagerContext)
        {
            if (trustManagerContext == null)
            {
                this.persist = true;
            }
            else
            {
                this.ignorePersistedDecision = trustManagerContext.IgnorePersistedDecision;
                this.noPrompt = trustManagerContext.NoPrompt;
                this.persist = trustManagerContext.Persist;
                this.previousAppId = trustManagerContext.PreviousApplicationIdentity;
            }
        }

        public bool IgnorePersistedDecision =>
            this.ignorePersistedDecision;

        public bool NoPrompt =>
            this.noPrompt;

        public bool Persist =>
            this.persist;

        public ApplicationIdentity PreviousAppId =>
            this.previousAppId;
    }
}

