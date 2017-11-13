namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class RegisterParticipantRecord
    {
        private static Type type = typeof(RegisterParticipantRecord);

        private static TraceCode GetCode(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, type, "GetCode");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return TraceCode.RegisterParticipant;

                case ProtocolVersion.Version11:
                    return TraceCode.RegisterParticipant11;
            }
            return 0;
        }

        public static void Trace(Guid enlistmentId, string transactionId, ControlProtocol protocol, EndpointAddress participantService, ProtocolVersion protocolVersion)
        {
            RegisterParticipantRecordSchema trace = RegisterParticipantRecordSchema.Instance(transactionId, enlistmentId, protocol, participantService, protocolVersion);
            Microsoft.Transactions.Bridge.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, GetCode(protocolVersion), Microsoft.Transactions.SR.GetString("RegisterParticipant"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            Microsoft.Transactions.Bridge.DiagnosticUtility.ShouldTraceInformation;
    }
}

