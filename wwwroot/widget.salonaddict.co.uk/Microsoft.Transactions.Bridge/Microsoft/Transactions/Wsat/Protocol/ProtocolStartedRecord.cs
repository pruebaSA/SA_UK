namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;

    internal static class ProtocolStartedRecord
    {
        public static void Trace(Guid protocolId, string protocolName)
        {
            ProtocolServiceRecordSchema trace = new ProtocolServiceRecordSchema(protocolName, protocolId);
            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ProtocolStarted, Microsoft.Transactions.SR.GetString("ProtocolStarted"), trace, null, protocolId, null);
        }

        public static bool ShouldTrace =>
            DiagnosticUtility.ShouldTraceInformation;
    }
}

