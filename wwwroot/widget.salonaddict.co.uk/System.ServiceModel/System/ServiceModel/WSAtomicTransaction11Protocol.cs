namespace System.ServiceModel
{
    using System;

    internal class WSAtomicTransaction11Protocol : TransactionProtocol
    {
        private static TransactionProtocol instance = new WSAtomicTransaction11Protocol();

        internal static TransactionProtocol Instance =>
            instance;

        internal override string Name =>
            "WSAtomicTransaction11";
    }
}

