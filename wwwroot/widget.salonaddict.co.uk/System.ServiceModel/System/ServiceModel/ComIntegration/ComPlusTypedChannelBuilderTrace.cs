namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;

    internal static class ComPlusTypedChannelBuilderTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, Type contractType, Binding binding)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusTypedChannelBuilderSchema trace = new ComPlusTypedChannelBuilderSchema(contractType.ToString(), binding?.GetType().ToString());
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }
    }
}

