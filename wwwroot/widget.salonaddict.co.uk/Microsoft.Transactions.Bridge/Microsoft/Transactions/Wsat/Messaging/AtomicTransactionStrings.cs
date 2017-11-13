namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class AtomicTransactionStrings
    {
        protected AtomicTransactionStrings()
        {
        }

        public static AtomicTransactionStrings Version(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(AtomicTransactionStrings), "V");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return AtomicTransactionStrings10.Instance;

                case ProtocolVersion.Version11:
                    return AtomicTransactionStrings11.Instance;
            }
            return null;
        }

        public string Aborted =>
            "Aborted";

        public abstract string AbortedAction { get; }

        public string Commit =>
            "Commit";

        public abstract string CommitAction { get; }

        public string Committed =>
            "Committed";

        public abstract string CommittedAction { get; }

        public string CompletionCoordinatorPortType =>
            "CompletionCoordinatorPortType";

        public string CompletionParticipantPortType =>
            "CompletionParticipantPortType";

        public abstract string CompletionUri { get; }

        public string CoordinatorPortType =>
            "CoordinatorPortType";

        public abstract string Durable2PCUri { get; }

        public abstract string FaultAction { get; }

        public string InconsistentInternalState =>
            "InconsistentInternalState";

        public abstract string Namespace { get; }

        public string ParticipantPortType =>
            "ParticipantPortType";

        public string Prefix =>
            "wsat";

        public string Prepare =>
            "Prepare";

        public abstract string PrepareAction { get; }

        public string Prepared =>
            "Prepared";

        public abstract string PreparedAction { get; }

        public string ReadOnly =>
            "ReadOnly";

        public abstract string ReadOnlyAction { get; }

        public string Replay =>
            "Replay";

        public abstract string ReplayAction { get; }

        public string Rollback =>
            "Rollback";

        public abstract string RollbackAction { get; }

        public string UnknownTransaction =>
            "UnknownTransaction";

        public abstract string Volatile2PCUri { get; }
    }
}

