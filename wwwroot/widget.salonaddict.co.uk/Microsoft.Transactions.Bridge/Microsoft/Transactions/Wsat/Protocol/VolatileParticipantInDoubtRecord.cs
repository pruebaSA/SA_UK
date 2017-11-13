namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class VolatileParticipantInDoubtRecord
    {
        private static Type type = typeof(VolatileParticipantInDoubtRecord);

        private static TraceCode GetCode(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, type, "GetCode");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return TraceCode.VolatileParticipantInDoubt;

                case ProtocolVersion.Version11:
                    return TraceCode.VolatileParticipantInDoubt11;
            }
            return 0;
        }

        public static void Trace(Guid enlistmentId, EndpointAddress replyTo, ProtocolVersion protocolVersion)
        {
            VolatileEnlistmentInDoubtRecordSchema trace = VolatileEnlistmentInDoubtRecordSchema.Instance(enlistmentId, replyTo, protocolVersion);
            Microsoft.Transactions.Bridge.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, GetCode(protocolVersion), Microsoft.Transactions.SR.GetString("VolatileParticipantInDoubt"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            Microsoft.Transactions.Bridge.DiagnosticUtility.ShouldTraceWarning;
    }
}

