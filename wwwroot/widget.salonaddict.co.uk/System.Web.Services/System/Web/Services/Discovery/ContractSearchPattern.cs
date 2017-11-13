namespace System.Web.Services.Discovery
{
    using System;

    public sealed class ContractSearchPattern : DiscoverySearchPattern
    {
        public override DiscoveryReference GetDiscoveryReference(string filename) => 
            new ContractReference(filename + "?wsdl", filename);

        public override string Pattern =>
            "*.asmx";
    }
}

