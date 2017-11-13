namespace System.Web.Services.Protocols
{
    using System;
    using System.Reflection;
    using System.Web.Services.Description;
    using System.Xml.Serialization;

    internal class DocumentationServerType : ServerType
    {
        private LogicalMethodInfo methodInfo;
        private XmlSchemas schemas;
        private XmlSchemas schemasWithPost;
        private ServiceDescriptionCollection serviceDescriptions;
        private ServiceDescriptionCollection serviceDescriptionsWithPost;

        internal DocumentationServerType(Type type, string uri) : base(typeof(DocumentationServerProtocol))
        {
            uri = new Uri(uri, true).GetLeftPart(UriPartial.Path);
            this.methodInfo = new LogicalMethodInfo(typeof(DocumentationServerProtocol).GetMethod("Documentation", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance));
            ServiceDescriptionReflector reflector = new ServiceDescriptionReflector();
            reflector.Reflect(type, uri);
            this.schemas = reflector.Schemas;
            this.serviceDescriptions = reflector.ServiceDescriptions;
            this.schemasWithPost = reflector.SchemasWithPost;
            this.serviceDescriptionsWithPost = reflector.ServiceDescriptionsWithPost;
        }

        internal LogicalMethodInfo MethodInfo =>
            this.methodInfo;

        internal XmlSchemas Schemas =>
            this.schemas;

        internal XmlSchemas SchemasWithPost =>
            this.schemasWithPost;

        internal ServiceDescriptionCollection ServiceDescriptions =>
            this.serviceDescriptions;

        internal ServiceDescriptionCollection ServiceDescriptionsWithPost =>
            this.serviceDescriptionsWithPost;
    }
}

