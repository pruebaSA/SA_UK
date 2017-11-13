namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;

    internal static class ComPlusMexChannelBuilderMexCompleteTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, ServiceEndpointCollection serviceEndpointsRetrieved)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                int num = 0;
                ComPlusMexBuilderMetadataRetrievedEndpoint[] endpoints = new ComPlusMexBuilderMetadataRetrievedEndpoint[serviceEndpointsRetrieved.Count];
                foreach (ServiceEndpoint endpoint in serviceEndpointsRetrieved)
                {
                    endpoints[num++] = new ComPlusMexBuilderMetadataRetrievedEndpoint(endpoint);
                }
                ComPlusMexBuilderMetadataRetrievedSchema trace = new ComPlusMexBuilderMetadataRetrievedSchema(endpoints);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }
    }
}

