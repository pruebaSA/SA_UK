namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class ProtocolInitializedRecord
    {
        public static void Trace(Guid protocolId, string protocolName)
        {
            ProtocolServiceRecordSchema trace = new ProtocolServiceRecordSchema(protocolName, protocolId);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ProtocolInitialized, Microsoft.Transactions.SR.GetString("ProtocolInitialized"), trace, null, protocolId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceInformation;
    }
}

