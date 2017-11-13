namespace System.Transactions
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct TransactionTraceIdentifier
    {
        public static readonly TransactionTraceIdentifier Empty;
        private string transactionIdentifier;
        private int cloneIdentifier;
        public TransactionTraceIdentifier(string transactionIdentifier, int cloneIdentifier)
        {
            this.transactionIdentifier = transactionIdentifier;
            this.cloneIdentifier = cloneIdentifier;
        }

        public string TransactionIdentifier =>
            this.transactionIdentifier;
        public int CloneIdentifier =>
            this.cloneIdentifier;
        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool Equals(object objectToCompare)
        {
            if (!(objectToCompare is TransactionTraceIdentifier))
            {
                return false;
            }
            TransactionTraceIdentifier identifier = (TransactionTraceIdentifier) objectToCompare;
            return ((identifier.TransactionIdentifier == this.TransactionIdentifier) && (identifier.CloneIdentifier == this.CloneIdentifier));
        }

        public static bool operator ==(TransactionTraceIdentifier id1, TransactionTraceIdentifier id2) => 
            id1.Equals(id2);

        public static bool operator !=(TransactionTraceIdentifier id1, TransactionTraceIdentifier id2) => 
            !id1.Equals(id2);

        static TransactionTraceIdentifier()
        {
            Empty = new TransactionTraceIdentifier();
        }
    }
}

