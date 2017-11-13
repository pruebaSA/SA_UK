﻿namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class RecoveredParticipantInvalidMetadataRecord
    {
        private static Type type = typeof(RecoveredParticipantInvalidMetadataRecord);

        private static TraceCode GetCode(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, type, "GetCode");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return TraceCode.RecoveredParticipantInvalidMetadata;

                case ProtocolVersion.Version11:
                    return TraceCode.RecoveredParticipantInvalidMetadata11;
            }
            return 0;
        }

        public static void Trace(Guid enlistmentId, string transactionId, EndpointAddress participantService, ProtocolVersion protocolVersion)
        {
            RecoverParticipantRecordSchema trace = RecoverParticipantRecordSchema.Instance(transactionId, enlistmentId, participantService, protocolVersion);
            Microsoft.Transactions.Bridge.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, GetCode(protocolVersion), Microsoft.Transactions.SR.GetString("RecoveredParticipantInvalidMetadata"), trace, null, enlistmentId, null);
        }

        public static bool ShouldTrace =>
            Microsoft.Transactions.Bridge.DiagnosticUtility.ShouldTraceWarning;
    }
}

