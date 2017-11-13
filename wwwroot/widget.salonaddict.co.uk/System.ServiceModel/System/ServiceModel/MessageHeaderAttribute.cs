namespace System.ServiceModel
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class MessageHeaderAttribute : MessageContractMemberAttribute
    {
        private string actor;
        private bool isMustUnderstandSet;
        private bool isRelaySet;
        private bool mustUnderstand;
        private bool relay;

        public string Actor
        {
            get => 
                this.actor;
            set
            {
                this.actor = value;
            }
        }

        internal bool IsMustUnderstandSet =>
            this.isMustUnderstandSet;

        internal bool IsRelaySet =>
            this.isRelaySet;

        public bool MustUnderstand
        {
            get => 
                this.mustUnderstand;
            set
            {
                this.mustUnderstand = value;
                this.isMustUnderstandSet = true;
            }
        }

        public bool Relay
        {
            get => 
                this.relay;
            set
            {
                this.relay = value;
                this.isRelaySet = true;
            }
        }
    }
}

