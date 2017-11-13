namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;

    internal static class ComPlusMexChannelBuilderTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, ContractDescription contract, Binding binding, string address)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusMexChannelBuilderSchema trace = new ComPlusMexChannelBuilderSchema(contract.Name, contract.Namespace, binding.Name, binding.Namespace, address);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }
    }
}

