namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;

    internal class AtomicTransactionStrings11 : AtomicTransactionStrings
    {
        private static AtomicTransactionStrings instance = new AtomicTransactionStrings11();

        public override string AbortedAction =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Aborted";

        public override string CommitAction =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Commit";

        public override string CommittedAction =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Committed";

        public override string CompletionUri =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Completion";

        public override string Durable2PCUri =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Durable2PC";

        public override string FaultAction =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/fault";

        public static AtomicTransactionStrings Instance =>
            instance;

        public override string Namespace =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06";

        public override string PrepareAction =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Prepare";

        public override string PreparedAction =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Prepared";

        public override string ReadOnlyAction =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/ReadOnly";

        public override string ReplayAction =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Replay";

        public override string RollbackAction =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Rollback";

        public override string Volatile2PCUri =>
            "http://docs.oasis-open.org/ws-tx/wsat/2006/06/Volatile2PC";
    }
}

