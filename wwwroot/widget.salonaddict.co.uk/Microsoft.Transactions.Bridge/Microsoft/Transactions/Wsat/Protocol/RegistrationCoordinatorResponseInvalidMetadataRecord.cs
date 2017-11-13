namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class RegistrationCoordinatorResponseInvalidMetadataRecord
    {
        private static Type type = typeof(RegistrationCoordinatorResponseInvalidMetadataRecord);

        private static TraceCode GetCode(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, type, "GetCode");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return TraceCode.RegistrationCoordinatorResponseInvalidMetadata;

                case ProtocolVersion.Version11:
                    return TraceCode.RegistrationCoordinatorResponseInvalidMetadata11;
            }
            return 0;
        }

        public static void Trace(Guid enlistmentId, CoordinationContext context, ControlProtocol protocol, EndpointAddress coordinatorService, Exception e, ProtocolVersion protocolVersion)
        {
            RegistrationCoordinatorResponseInvalidMetadataSchema trace = RegistrationCoordinatorResponseInvalidMetadataSchema.Instance(context, protocol, coordinatorService, protocolVersion);
            Microsoft.Transactions.Bridge.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, GetCode(protocolVersion), Microsoft.Transactions.SR.GetString("RegistrationCoordinatorResponseInvalidMetadata"), trace, e, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            Microsoft.Transactions.Bridge.DiagnosticUtility.ShouldTraceWarning;
    }
}

