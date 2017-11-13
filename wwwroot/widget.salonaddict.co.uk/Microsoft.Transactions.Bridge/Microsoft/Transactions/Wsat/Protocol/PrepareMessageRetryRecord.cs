namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class PrepareMessageRetryRecord
    {
        public static void Trace(Guid enlistmentId, string transactionId, int count)
        {
            ParticipantRetryMessageRecordSchema trace = new ParticipantRetryMessageRecordSchema(transactionId, enlistmentId, count);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.PrepareMessageRetry, Microsoft.Transactions.SR.GetString("PrepareMessageRetry"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceInformation;
    }
}

