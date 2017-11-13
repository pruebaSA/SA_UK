namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class ParticipantRecoveredRecord
    {
        private static Type type = typeof(ParticipantRecoveredRecord);

        private static TraceCode GetCode(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, type, "GetCode");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return TraceCode.ParticipantRecovered;

                case ProtocolVersion.Version11:
                    return TraceCode.ParticipantRecovered11;
            }
            return 0;
        }

        public static void Trace(Guid enlistmentId, string transactionId, EndpointAddress protocolService, ProtocolVersion protocolVersion)
        {
            RecoverParticipantRecordSchema trace = RecoverParticipantRecordSchema.Instance(transactionId, enlistmentId, protocolService, protocolVersion);
            Microsoft.Transactions.Bridge.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, GetCode(protocolVersion), Microsoft.Transactions.SR.GetString("ParticipantRecovered"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            Microsoft.Transactions.Bridge.DiagnosticUtility.ShouldTraceInformation;
    }
}

