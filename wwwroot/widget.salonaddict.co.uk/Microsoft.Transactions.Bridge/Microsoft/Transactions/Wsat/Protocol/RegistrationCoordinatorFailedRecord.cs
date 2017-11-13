namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class RegistrationCoordinatorFailedRecord
    {
        public static void Trace(Guid enlistmentId, CoordinationContext context, ControlProtocol protocol, Exception e)
        {
            RegistrationCoordinatorFailedSchema trace = new RegistrationCoordinatorFailedSchema(context, protocol);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.RegistrationCoordinatorFailed, Microsoft.Transactions.SR.GetString("RegistrationCoordinatorFailed"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceWarning;
    }
}

