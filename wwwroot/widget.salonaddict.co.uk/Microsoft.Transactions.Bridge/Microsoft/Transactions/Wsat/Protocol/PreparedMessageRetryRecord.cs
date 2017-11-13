namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class PreparedMessageRetryRecord
    {
        public static void Trace(Guid enlistmentId, string transactionId, int count)
        {
            CoordinatorRetryMessageRecordSchema trace = new CoordinatorRetryMessageRecordSchema(transactionId, count);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.PreparedMessageRetry, Microsoft.Transactions.SR.GetString("PreparedMessageRetry"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceInformation;
    }
}

