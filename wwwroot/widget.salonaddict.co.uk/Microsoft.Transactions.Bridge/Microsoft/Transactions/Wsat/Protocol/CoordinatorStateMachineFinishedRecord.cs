namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class CoordinatorStateMachineFinishedRecord
    {
        public static void Trace(Guid enlistmentId, string transactionId, TransactionOutcome outcome)
        {
            CoordinatorOutcomeRecordSchema trace = new CoordinatorOutcomeRecordSchema(transactionId, outcome);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Verbose, TraceCode.CoordinatorStateMachineFinished, Microsoft.Transactions.SR.GetString("CoordinatorStateMachineFinished"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceVerbose;
    }
}

