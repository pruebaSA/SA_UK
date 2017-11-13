namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class RecoveredCoordinatorInvalidMetadataRecord
    {
        private static Type type = typeof(RecoveredCoordinatorInvalidMetadataRecord);

        private static TraceCode GetCode(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, type, "GetCode");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return TraceCode.RecoveredCoordinatorInvalidMetadata;

                case ProtocolVersion.Version11:
                    return TraceCode.RecoveredCoordinatorInvalidMetadata11;
            }
            return 0;
        }

        public static void Trace(Guid enlistmentId, string transactionId, EndpointAddress coordinatorService, ProtocolVersion protocolVersion)
        {
            RecoverCoordinatorRecordSchema trace = RecoverCoordinatorRecordSchema.Instance(transactionId, coordinatorService, protocolVersion);
            Microsoft.Transactions.Bridge.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, GetCode(protocolVersion), Microsoft.Transactions.SR.GetString("RecoveredCoordinatorInvalidMetadata"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            Microsoft.Transactions.Bridge.DiagnosticUtility.ShouldTraceWarning;
    }
}

