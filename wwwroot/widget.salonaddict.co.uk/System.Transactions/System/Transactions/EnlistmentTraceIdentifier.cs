namespace System.Transactions
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct EnlistmentTraceIdentifier
    {
        public static readonly EnlistmentTraceIdentifier Empty;
        private Guid resourceManagerIdentifier;
        private TransactionTraceIdentifier transactionTraceIdentifier;
        private int enlistmentIdentifier;
        public EnlistmentTraceIdentifier(Guid resourceManagerIdentifier, TransactionTraceIdentifier transactionTraceId, int enlistmentIdentifier)
        {
            this.resourceManagerIdentifier = resourceManagerIdentifier;
            this.transactionTraceIdentifier = transactionTraceId;
            this.enlistmentIdentifier = enlistmentIdentifier;
        }

        public Guid ResourceManagerIdentifier =>
            this.resourceManagerIdentifier;
        public TransactionTraceIdentifier TransactionTraceId =>
            this.transactionTraceIdentifier;
        public int EnlistmentIdentifier =>
            this.enlistmentIdentifier;
        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool Equals(object objectToCompare)
        {
            if (!(objectToCompare is EnlistmentTraceIdentifier))
            {
                return false;
            }
            EnlistmentTraceIdentifier identifier = (EnlistmentTraceIdentifier) objectToCompare;
            return ((!(identifier.ResourceManagerIdentifier != this.ResourceManagerIdentifier) && !(identifier.TransactionTraceId != this.TransactionTraceId)) && (identifier.EnlistmentIdentifier == this.EnlistmentIdentifier));
        }

        public static bool operator ==(EnlistmentTraceIdentifier id1, EnlistmentTraceIdentifier id2) => 
            id1.Equals(id2);

        public static bool operator !=(EnlistmentTraceIdentifier id1, EnlistmentTraceIdentifier id2) => 
            !id1.Equals(id2);

        static EnlistmentTraceIdentifier()
        {
            Empty = new EnlistmentTraceIdentifier();
        }
    }
}

