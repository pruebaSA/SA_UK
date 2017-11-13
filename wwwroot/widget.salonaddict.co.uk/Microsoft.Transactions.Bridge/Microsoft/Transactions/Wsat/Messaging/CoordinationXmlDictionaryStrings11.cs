namespace Microsoft.Transactions.Wsat.Messaging
{
    using System.ServiceModel;
    using System.Xml;

    internal class CoordinationXmlDictionaryStrings11 : CoordinationXmlDictionaryStrings
    {
        private static CoordinationXmlDictionaryStrings instance = new CoordinationXmlDictionaryStrings11();

        public override XmlDictionaryString CreateCoordinationContextAction =>
            DXD.CoordinationExternal11Dictionary.CreateCoordinationContextAction;

        public override XmlDictionaryString CreateCoordinationContextResponseAction =>
            DXD.CoordinationExternal11Dictionary.CreateCoordinationContextResponseAction;

        public override XmlDictionaryString FaultAction =>
            DXD.CoordinationExternal11Dictionary.FaultAction;

        public static CoordinationXmlDictionaryStrings Instance =>
            instance;

        public override XmlDictionaryString Namespace =>
            DXD.CoordinationExternal11Dictionary.Namespace;

        public override XmlDictionaryString RegisterAction =>
            DXD.CoordinationExternal11Dictionary.RegisterAction;

        public override XmlDictionaryString RegisterResponseAction =>
            DXD.CoordinationExternal11Dictionary.RegisterResponseAction;
    }
}

