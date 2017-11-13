namespace System.DirectoryServices.Protocols
{
    using System;

    public sealed class ReferralCallback
    {
        private DereferenceConnectionCallback dereference;
        private NotifyOfNewConnectionCallback notify;
        private QueryForConnectionCallback query;

        public ReferralCallback()
        {
            Utility.CheckOSVersion();
        }

        public DereferenceConnectionCallback DereferenceConnection
        {
            get => 
                this.dereference;
            set
            {
                this.dereference = value;
            }
        }

        public NotifyOfNewConnectionCallback NotifyNewConnection
        {
            get => 
                this.notify;
            set
            {
                this.notify = value;
            }
        }

        public QueryForConnectionCallback QueryForConnection
        {
            get => 
                this.query;
            set
            {
                this.query = value;
            }
        }
    }
}

