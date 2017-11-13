namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class EnlistTransactionRecord
    {
        public static void Trace(Guid enlistmentId, CoordinationContext context)
        {
            CoordinationContextRecordSchema trace = new CoordinationContextRecordSchema(context);
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.EnlistTransaction, Microsoft.Transactions.SR.GetString("EnlistTransaction"), trace, null, enlistmentId, null);
            }
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceInformation;
    }
}

