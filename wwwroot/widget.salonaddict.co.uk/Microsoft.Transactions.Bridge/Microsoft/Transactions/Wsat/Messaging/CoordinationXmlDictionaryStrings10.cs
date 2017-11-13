namespace Microsoft.Transactions.Wsat.Messaging
{
    using System.ServiceModel;
    using System.Xml;

    internal class CoordinationXmlDictionaryStrings10 : CoordinationXmlDictionaryStrings
    {
        private static CoordinationXmlDictionaryStrings instance = new CoordinationXmlDictionaryStrings10();

        public override XmlDictionaryString CreateCoordinationContextAction =>
            XD.CoordinationExternal10Dictionary.CreateCoordinationContextAction;

        public override XmlDictionaryString CreateCoordinationContextResponseAction =>
            XD.CoordinationExternal10Dictionary.CreateCoordinationContextResponseAction;

        public override XmlDictionaryString FaultAction =>
            XD.CoordinationExternal10Dictionary.FaultAction;

        public static CoordinationXmlDictionaryStrings Instance =>
            instance;

        public override XmlDictionaryString Namespace =>
            XD.CoordinationExternal10Dictionary.Namespace;

        public override XmlDictionaryString RegisterAction =>
            XD.CoordinationExternal10Dictionary.RegisterAction;

        public override XmlDictionaryString RegisterResponseAction =>
            XD.CoordinationExternal10Dictionary.RegisterResponseAction;
    }
}

