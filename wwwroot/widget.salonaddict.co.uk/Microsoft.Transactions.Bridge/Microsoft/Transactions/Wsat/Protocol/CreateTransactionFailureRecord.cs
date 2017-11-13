namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class CreateTransactionFailureRecord
    {
        public static void Trace(Guid enlistmentId, string reason)
        {
            ReasonRecordSchema trace = new ReasonRecordSchema(reason);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.CreateTransactionFailure, Microsoft.Transactions.SR.GetString("CreateTransactionFailure"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceWarning;
    }
}

