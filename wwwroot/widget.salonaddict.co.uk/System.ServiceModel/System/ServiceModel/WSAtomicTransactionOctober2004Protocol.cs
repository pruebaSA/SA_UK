namespace System.ServiceModel
{
    using System;

    internal class WSAtomicTransactionOctober2004Protocol : TransactionProtocol
    {
        private static TransactionProtocol instance = new WSAtomicTransactionOctober2004Protocol();

        internal static TransactionProtocol Instance =>
            instance;

        internal override string Name =>
            "WSAtomicTransactionOctober2004";
    }
}

