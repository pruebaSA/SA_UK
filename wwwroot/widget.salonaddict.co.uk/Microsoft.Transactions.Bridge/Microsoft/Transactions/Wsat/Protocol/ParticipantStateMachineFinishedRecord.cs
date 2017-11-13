namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class ParticipantStateMachineFinishedRecord
    {
        public static void Trace(Guid enlistmentId, string transactionId, TransactionOutcome outcome)
        {
            ParticipantOutcomeRecordSchema trace = new ParticipantOutcomeRecordSchema(transactionId, enlistmentId, outcome);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Verbose, TraceCode.ParticipantStateMachineFinished, Microsoft.Transactions.SR.GetString("ParticipantStateMachineFinished"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceVerbose;
    }
}

