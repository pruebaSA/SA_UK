namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class DurableParticipantReplayWhilePreparingRecord
    {
        public static void Trace(Guid enlistmentId, string transactionId)
        {
            EnlistmentRecordSchema trace = new EnlistmentRecordSchema(transactionId, enlistmentId);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.DurableParticipantReplayWhilePreparing, Microsoft.Transactions.SR.GetString("DurableParticipantReplayWhilePreparing"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceWarning;
    }
}

