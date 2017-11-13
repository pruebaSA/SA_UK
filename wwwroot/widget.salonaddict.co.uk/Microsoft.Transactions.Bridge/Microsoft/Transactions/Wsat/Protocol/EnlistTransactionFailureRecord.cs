namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class EnlistTransactionFailureRecord
    {
        public static void Trace(Guid enlistmentId, CoordinationContext context, string reason)
        {
            ReasonRecordSchema trace = new ReasonRecordSchema(reason);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.EnlistTransactionFailure, Microsoft.Transactions.SR.GetString("EnlistTransactionFailure"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceWarning;
    }
}

