namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;

    internal class AtomicTransactionStrings10 : AtomicTransactionStrings
    {
        private static AtomicTransactionStrings instance = new AtomicTransactionStrings10();

        public override string AbortedAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Aborted";

        public override string CommitAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Commit";

        public override string CommittedAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Committed";

        public override string CompletionUri =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Completion";

        public override string Durable2PCUri =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Durable2PC";

        public override string FaultAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/fault";

        public static AtomicTransactionStrings Instance =>
            instance;

        public override string Namespace =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat";

        public override string PrepareAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Prepare";

        public override string PreparedAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Prepared";

        public override string ReadOnlyAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/ReadOnly";

        public override string ReplayAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Replay";

        public override string RollbackAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Rollback";

        public override string Volatile2PCUri =>
            "http://schemas.xmlsoap.org/ws/2004/10/wsat/Volatile2PC";
    }
}

