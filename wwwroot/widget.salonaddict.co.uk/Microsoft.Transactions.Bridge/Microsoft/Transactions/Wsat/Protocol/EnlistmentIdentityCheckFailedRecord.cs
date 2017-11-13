namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class EnlistmentIdentityCheckFailedRecord
    {
        public static void Trace(Guid enlistmentId)
        {
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.EnlistmentIdentityCheckFailed, Microsoft.Transactions.SR.GetString("EnlistmentIdentityCheckFailed"), null, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceInformation;
    }
}

