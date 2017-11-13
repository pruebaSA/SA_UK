namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class ReplayMessageRetryRecord
    {
        public static void Trace(Guid enlistmentId, string transactionId, int count)
        {
            CoordinatorRetryMessageRecordSchema trace = new CoordinatorRetryMessageRecordSchema(transactionId, count);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ReplayMessageRetry, Microsoft.Transactions.SR.GetString("ReplayMessageRetry"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceInformation;
    }
}

