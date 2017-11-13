namespace System.Web.Services.Discovery
{
    using System;

    public sealed class XmlSchemaSearchPattern : DiscoverySearchPattern
    {
        public override DiscoveryReference GetDiscoveryReference(string filename) => 
            new SchemaReference(filename);

        public override string Pattern =>
            "*.xsd";
    }
}

