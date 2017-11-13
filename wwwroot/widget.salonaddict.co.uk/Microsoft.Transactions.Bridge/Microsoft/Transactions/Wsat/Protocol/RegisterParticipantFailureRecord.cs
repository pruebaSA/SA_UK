namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class RegisterParticipantFailureRecord
    {
        private static Type type = typeof(RegisterParticipantFailureRecord);

        private static TraceCode GetCode(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, type, "GetCode");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return TraceCode.RegisterParticipantFailure;

                case ProtocolVersion.Version11:
                    return TraceCode.RegisterParticipantFailure11;
            }
            return 0;
        }

        public static void Trace(Guid enlistmentId, string transactionId, ControlProtocol protocol, EndpointAddress protocolService, string reason, ProtocolVersion protocolVersion)
        {
            RegisterFailureRecordSchema trace = RegisterFailureRecordSchema.Instance(transactionId, protocol, protocolService, reason, protocolVersion);
            Microsoft.Transactions.Bridge.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, GetCode(protocolVersion), Microsoft.Transactions.SR.GetString("RegisterParticipantFailure"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            Microsoft.Transactions.Bridge.DiagnosticUtility.ShouldTraceWarning;
    }
}

