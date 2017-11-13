namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class ComPlusChannelCreatedTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, Uri address, Type contractType)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusChannelCreatedSchema trace = new ComPlusChannelCreatedSchema(address, contractType?.ToString());
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }
    }
}

