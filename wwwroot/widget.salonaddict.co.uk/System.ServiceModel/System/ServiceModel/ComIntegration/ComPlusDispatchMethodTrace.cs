namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal static class ComPlusDispatchMethodTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, Dictionary<uint, DispatchProxy.MethodInfo> dispToOperationDescription)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                uint key = 10;
                DispatchProxy.MethodInfo info = null;
                while (dispToOperationDescription.TryGetValue(key, out info))
                {
                    ComPlusDispatchMethodSchema trace = new ComPlusDispatchMethodSchema(info.opDesc.Name, info.paramList, info.ReturnVal);
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
                    key++;
                }
            }
        }
    }
}

