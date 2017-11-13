namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class VolatileOutcomeTimeoutRecord
    {
        public static void Trace(Guid enlistmentId, string transactionId, TransactionOutcome outcome, TimeSpan timeout)
        {
            EnlistmentTimeoutRecordSchema trace = new EnlistmentTimeoutRecordSchema(transactionId, enlistmentId, outcome, timeout);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.VolatileOutcomeTimeout, Microsoft.Transactions.SR.GetString("VolatileOutcomeTimeout"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceWarning;
    }
}

