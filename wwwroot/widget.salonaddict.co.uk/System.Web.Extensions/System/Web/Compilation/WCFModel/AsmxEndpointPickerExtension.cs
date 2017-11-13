namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Permissions;
    using System.ServiceModel.Description;
    using System.Web.Services.Description;
    using System.Xml.Schema;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    internal class AsmxEndpointPickerExtension : IWsdlImportExtension
    {
        private bool IsAsmxUri(string location)
        {
            Uri result = null;
            if (Uri.TryCreate(location, UriKind.Absolute, out result))
            {
                string[] segments = result.Segments;
                if (segments.Length > 0)
                {
                    try
                    {
                        string path = segments[segments.Length - 1];
                        if (string.Equals(Path.GetExtension(path), ".asmx", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
            return false;
        }

        private bool IsSoapAsmxPort(Type addressType, Port port)
        {
            SoapAddressBinding binding = port.Extensions.Find(addressType) as SoapAddressBinding;
            return (((binding != null) && (binding.GetType() == addressType)) && this.IsAsmxUri(binding.Location));
        }

        void IWsdlImportExtension.BeforeImport(ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy)
        {
            if (wsdlDocuments == null)
            {
                throw new ArgumentNullException("wsdlDocuments");
            }
            foreach (System.Web.Services.Description.ServiceDescription description in wsdlDocuments)
            {
                foreach (Service service in description.Services)
                {
                    if (service.Ports.Count == 2)
                    {
                        Port port = null;
                        if (this.IsSoapAsmxPort(typeof(SoapAddressBinding), service.Ports[0]) && this.IsSoapAsmxPort(typeof(Soap12AddressBinding), service.Ports[1]))
                        {
                            port = service.Ports[1];
                        }
                        else if (this.IsSoapAsmxPort(typeof(SoapAddressBinding), service.Ports[1]) && this.IsSoapAsmxPort(typeof(Soap12AddressBinding), service.Ports[0]))
                        {
                            port = service.Ports[0];
                        }
                        if (port != null)
                        {
                            service.Ports.Remove(port);
                            if (port.Binding != null)
                            {
                                List<Binding> list = new List<Binding>();
                                foreach (Binding binding in description.Bindings)
                                {
                                    if (string.Equals(binding.Name, port.Binding.Name, StringComparison.Ordinal))
                                    {
                                        list.Add(binding);
                                    }
                                }
                                foreach (Binding binding2 in list)
                                {
                                    description.Bindings.Remove(binding2);
                                }
                            }
                        }
                    }
                }
            }
        }

        void IWsdlImportExtension.ImportContract(WsdlImporter importer, WsdlContractConversionContext context)
        {
        }

        void IWsdlImportExtension.ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
        {
        }
    }
}

