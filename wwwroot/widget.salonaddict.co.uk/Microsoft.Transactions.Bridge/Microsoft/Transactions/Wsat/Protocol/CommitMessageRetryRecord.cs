namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class CommitMessageRetryRecord
    {
        public static void Trace(Guid enlistmentId, string transactionId, int count)
        {
            ParticipantRetryMessageRecordSchema trace = new ParticipantRetryMessageRecordSchema(transactionId, enlistmentId, count);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.CommitMessageRetry, Microsoft.Transactions.SR.GetString("CommitMessageRetry"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceInformation;
    }
}

