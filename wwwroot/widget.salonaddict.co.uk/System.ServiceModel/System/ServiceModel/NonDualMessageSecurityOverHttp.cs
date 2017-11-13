namespace System.ServiceModel
{
    using System;

    public sealed class NonDualMessageSecurityOverHttp : MessageSecurityOverHttp
    {
        internal const bool DefaultEstablishSecurityContext = true;
        private bool establishSecurityContext = true;

        internal NonDualMessageSecurityOverHttp()
        {
        }

        protected override bool IsSecureConversationEnabled() => 
            this.establishSecurityContext;

        public bool EstablishSecurityContext
        {
            get => 
                this.establishSecurityContext;
            set
            {
                this.establishSecurityContext = value;
            }
        }
    }
}

