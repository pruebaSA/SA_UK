namespace System.ServiceModel
{
    using System;

    public abstract class TransactionProtocol
    {
        protected TransactionProtocol()
        {
        }

        internal static bool IsDefined(TransactionProtocol transactionProtocol)
        {
            if ((transactionProtocol != OleTransactions) && (transactionProtocol != WSAtomicTransactionOctober2004))
            {
                return (transactionProtocol == WSAtomicTransaction11);
            }
            return true;
        }

        public static TransactionProtocol Default =>
            OleTransactions;

        internal abstract string Name { get; }

        public static TransactionProtocol OleTransactions =>
            OleTransactionsProtocol.Instance;

        public static TransactionProtocol WSAtomicTransaction11 =>
            WSAtomicTransaction11Protocol.Instance;

        public static TransactionProtocol WSAtomicTransactionOctober2004 =>
            WSAtomicTransactionOctober2004Protocol.Instance;
    }
}

