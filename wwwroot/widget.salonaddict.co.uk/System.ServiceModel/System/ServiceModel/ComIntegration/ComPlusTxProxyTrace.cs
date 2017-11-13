namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class ComPlusTxProxyTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, Guid appid, Guid clsid, Guid transactionID, int instanceID)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusTxProxySchema trace = new ComPlusTxProxySchema(appid, clsid, transactionID, instanceID);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }
    }
}

