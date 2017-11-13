namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class CoordinatorRecoveredRecord
    {
        private static Type type = typeof(CoordinatorRecoveredRecord);

        private static TraceCode GetCode(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, type, "GetCode");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return TraceCode.CoordinatorRecovered;

                case ProtocolVersion.Version11:
                    return TraceCode.CoordinatorRecovered11;
            }
            return 0;
        }

        public static void Trace(Guid enlistmentId, string transactionId, EndpointAddress protocolService, ProtocolVersion protocolVersion)
        {
            RecoverCoordinatorRecordSchema trace = RecoverCoordinatorRecordSchema.Instance(transactionId, protocolService, protocolVersion);
            Microsoft.Transactions.Bridge.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, GetCode(protocolVersion), Microsoft.Transactions.SR.GetString("CoordinatorRecovered"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            Microsoft.Transactions.Bridge.DiagnosticUtility.ShouldTraceInformation;
    }
}

