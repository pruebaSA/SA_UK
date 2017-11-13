namespace System.Web.Compilation.WCFModel
{
    using System;

    internal class XmlStrings
    {
        internal class DataServices
        {
            internal const string NamespaceUri = "http://schemas.microsoft.com/ado/2007/06/edmx";

            internal class Elements
            {
                internal const string Root = "Edmx";
            }
        }

        internal class DataSet
        {
            internal const string NamespaceUri = "urn:schemas-microsoft-com:xml-msdata";

            internal class Attributes
            {
                internal const string IsDataSet = "IsDataSet";
            }
        }

        internal class DISCO
        {
            internal const string NamespaceUri = "http://schemas.xmlsoap.org/disco/";
            internal const string Prefix = "disco";

            internal class Elements
            {
                internal const string Root = "discovery";
            }
        }

        internal class MetadataExchange
        {
            internal const string Name = "WS-MetadataExchange";
            internal const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2004/09/mex";
            internal const string Prefix = "wsx";

            internal class Elements
            {
                internal const string Metadata = "Metadata";
            }
        }

        internal class WSAddressing
        {
            internal const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2004/08/addressing";
            internal const string Prefix = "wsa";

            internal class Elements
            {
                internal const string EndpointReference = "EndpointReference";
            }
        }

        internal class WSDL
        {
            internal const string NamespaceUri = "http://schemas.xmlsoap.org/wsdl/";
            internal const string Prefix = "wsdl";

            internal class Elements
            {
                internal const string Root = "definitions";
            }
        }

        internal class WsdlContractInheritance
        {
            internal const string NamespaceUri = "http://schemas.microsoft.com/ws/2005/01/WSDL/Extensions/ContractInheritance";
            internal const string Prefix = "wsdl-ex";
        }

        internal class WSPolicy
        {
            internal const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2004/09/policy";
            internal const string NamespaceUri15 = "http://www.w3.org/ns/ws-policy";
            internal const string Prefix = "wsp";

            internal class Attributes
            {
                internal const string PolicyURIs = "PolicyURIs";
            }

            internal class Elements
            {
                internal const string All = "All";
                internal const string ExactlyOne = "ExactlyOne";
                internal const string Policy = "Policy";
                internal const string PolicyReference = "PolicyReference";
            }
        }

        internal class Wsu
        {
            internal const string NamespaceUri = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
            internal const string Prefix = "wsu";

            internal class Attributes
            {
                internal const string Id = "Id";
            }
        }

        internal class Xml
        {
            internal const string NamespaceUri = "http://www.w3.org/XML/1998/namespace";
            internal const string Prefix = "xml";

            internal class Attributes
            {
                internal const string Base = "base";
                internal const string Id = "id";
            }
        }

        internal class XmlSchema
        {
            internal const string NamespaceUri = "http://www.w3.org/2001/XMLSchema";
            internal const string Prefix = "xsd";

            internal class Elements
            {
                internal const string Root = "schema";
            }
        }
    }
}

