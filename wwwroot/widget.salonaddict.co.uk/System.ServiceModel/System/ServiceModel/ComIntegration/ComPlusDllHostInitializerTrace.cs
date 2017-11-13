namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Diagnostics;

    internal static class ComPlusDllHostInitializerTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, Guid appid)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusDllHostInitializerSchema trace = new ComPlusDllHostInitializerSchema(appid);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }

        public static void Trace(TraceEventType type, TraceCode code, string description, Guid appid, Guid clsid, ServiceElement service)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                foreach (ServiceEndpointElement element in service.Endpoints)
                {
                    ComPlusDllHostInitializerAddingHostSchema trace = new ComPlusDllHostInitializerAddingHostSchema(appid, clsid, service.BehaviorConfiguration, service.Name, element.Address.ToString(), element.BindingConfiguration, element.BindingName, element.BindingNamespace, element.Binding, element.Contract);
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
                }
            }
        }
    }
}

