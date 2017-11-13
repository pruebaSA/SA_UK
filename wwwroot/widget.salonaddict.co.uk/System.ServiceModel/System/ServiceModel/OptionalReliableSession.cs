namespace System.ServiceModel
{
    using System;
    using System.ServiceModel.Channels;

    public class OptionalReliableSession : ReliableSession
    {
        private bool enabled;

        public OptionalReliableSession(ReliableSessionBindingElement reliableSessionBindingElement) : base(reliableSessionBindingElement)
        {
        }

        public bool Enabled
        {
            get => 
                this.enabled;
            set
            {
                this.enabled = value;
            }
        }
    }
}

