namespace Microsoft.Transactions.Bridge
{
    using System;
    using System.ServiceModel.Transactions;
    using System.Transactions;

    internal class EnlistmentOptions
    {
        private string description;
        private TimeSpan expires = new TimeSpan(0, 5, 0);
        private System.ServiceModel.Transactions.IsolationFlags isoFlags;
        private IsolationLevel isoLevel = IsolationLevel.Unspecified;

        public string Description
        {
            get => 
                this.description;
            set
            {
                this.description = value;
            }
        }

        public TimeSpan Expires
        {
            get => 
                this.expires;
            set
            {
                this.expires = value;
            }
        }

        public System.ServiceModel.Transactions.IsolationFlags IsolationFlags
        {
            get => 
                this.isoFlags;
            set
            {
                this.isoFlags = value;
            }
        }

        public ulong IsolationFlagsLong
        {
            get => 
                ((ulong) ((long) this.isoFlags));
            set
            {
                this.isoFlags = (System.ServiceModel.Transactions.IsolationFlags) ((int) value);
            }
        }

        public IsolationLevel IsoLevel
        {
            get => 
                this.isoLevel;
            set
            {
                this.isoLevel = value;
            }
        }
    }
}

