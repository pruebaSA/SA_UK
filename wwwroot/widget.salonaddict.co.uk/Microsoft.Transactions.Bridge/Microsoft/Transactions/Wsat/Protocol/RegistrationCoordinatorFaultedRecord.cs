namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;

    internal static class RegistrationCoordinatorFaultedRecord
    {
        public static void Trace(Guid enlistmentId, CoordinationContext context, ControlProtocol protocol, MessageFault fault)
        {
            RegistrationCoordinatorFaultedSchema trace = new RegistrationCoordinatorFaultedSchema(context, protocol, fault);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.RegistrationCoordinatorFaulted, Microsoft.Transactions.SR.GetString("RegistrationCoordinatorFaulted"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceWarning;
    }
}

