namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class RegisterCoordinatorRecord
    {
        private static Type type = typeof(RegisterCoordinatorRecord);

        private static TraceCode GetCode(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, type, "GetCode");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return TraceCode.RegisterCoordinator;

                case ProtocolVersion.Version11:
                    return TraceCode.RegisterCoordinator11;
            }
            return 0;
        }

        public static void Trace(Guid enlistmentId, CoordinationContext context, ControlProtocol protocol, EndpointAddress coordinatorService, ProtocolVersion protocolVersion)
        {
            RegisterCoordinatorRecordSchema trace = RegisterCoordinatorRecordSchema.Instance(context, protocol, coordinatorService, protocolVersion);
            Microsoft.Transactions.Bridge.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, GetCode(protocolVersion), Microsoft.Transactions.SR.GetString("RegisterCoordinator"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            Microsoft.Transactions.Bridge.DiagnosticUtility.ShouldTraceInformation;
    }
}

