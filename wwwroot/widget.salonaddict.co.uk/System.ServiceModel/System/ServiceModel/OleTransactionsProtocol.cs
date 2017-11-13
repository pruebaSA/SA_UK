namespace System.ServiceModel
{
    using System;

    internal class OleTransactionsProtocol : TransactionProtocol
    {
        private static TransactionProtocol instance = new OleTransactionsProtocol();

        internal static TransactionProtocol Instance =>
            instance;

        internal override string Name =>
            "OleTransactions";
    }
}

