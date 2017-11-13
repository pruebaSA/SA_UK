namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.Web.Services.Description;
    using System.Xml;

    internal static class ComPlusServiceHostTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, ServiceInfo info)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusServiceHostSchema trace = new ComPlusServiceHostSchema(info.AppID, info.Clsid);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }

        public static void Trace(TraceEventType type, TraceCode code, string description, ServiceInfo info, ContractDescription contract)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                XmlQualifiedName contractQname = new XmlQualifiedName(contract.Name, contract.Namespace);
                ComPlusServiceHostCreatedServiceContractSchema trace = new ComPlusServiceHostCreatedServiceContractSchema(info.AppID, info.Clsid, contractQname, contract.ContractType.ToString());
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }

        public static void Trace(TraceEventType type, TraceCode code, string description, ServiceInfo info, System.ServiceModel.Description.ServiceDescription service)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                WsdlExporter exporter = new WsdlExporter();
                string ns = "http://tempuri.org/";
                XmlQualifiedName wsdlServiceQName = new XmlQualifiedName("comPlusService", ns);
                exporter.ExportEndpoints(service.Endpoints, wsdlServiceQName);
                System.Web.Services.Description.ServiceDescription wsdl = exporter.GeneratedWsdlDocuments[ns];
                ComPlusServiceHostStartedServiceDetailsSchema trace = new ComPlusServiceHostStartedServiceDetailsSchema(info.AppID, info.Clsid, wsdl);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }

        public static void Trace(TraceEventType type, TraceCode code, string description, ServiceInfo info, ServiceEndpointCollection endpointCollection)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                foreach (ServiceEndpoint endpoint in endpointCollection)
                {
                    ComPlusServiceHostCreatedServiceEndpointSchema trace = new ComPlusServiceHostCreatedServiceEndpointSchema(info.AppID, info.Clsid, endpoint.Contract.Name, endpoint.Address.Uri, endpoint.Binding.Name);
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
                }
            }
        }
    }
}

