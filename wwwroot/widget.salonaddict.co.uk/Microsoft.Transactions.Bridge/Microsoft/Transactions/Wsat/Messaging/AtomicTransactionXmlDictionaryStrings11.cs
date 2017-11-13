namespace Microsoft.Transactions.Wsat.Messaging
{
    using System.ServiceModel;
    using System.Xml;

    internal class AtomicTransactionXmlDictionaryStrings11 : AtomicTransactionXmlDictionaryStrings
    {
        private static AtomicTransactionXmlDictionaryStrings instance = new AtomicTransactionXmlDictionaryStrings11();

        public override XmlDictionaryString AbortedAction =>
            DXD.AtomicTransactionExternal11Dictionary.AbortedAction;

        public override XmlDictionaryString CommitAction =>
            DXD.AtomicTransactionExternal11Dictionary.CommitAction;

        public override XmlDictionaryString CommittedAction =>
            DXD.AtomicTransactionExternal11Dictionary.CommittedAction;

        public override XmlDictionaryString CompletionUri =>
            DXD.AtomicTransactionExternal11Dictionary.CompletionUri;

        public override XmlDictionaryString Durable2PCUri =>
            DXD.AtomicTransactionExternal11Dictionary.Durable2PCUri;

        public override XmlDictionaryString FaultAction =>
            DXD.AtomicTransactionExternal11Dictionary.FaultAction;

        public static AtomicTransactionXmlDictionaryStrings Instance =>
            instance;

        public override XmlDictionaryString Namespace =>
            DXD.AtomicTransactionExternal11Dictionary.Namespace;

        public override XmlDictionaryString PrepareAction =>
            DXD.AtomicTransactionExternal11Dictionary.PrepareAction;

        public override XmlDictionaryString PreparedAction =>
            DXD.AtomicTransactionExternal11Dictionary.PreparedAction;

        public override XmlDictionaryString ReadOnlyAction =>
            DXD.AtomicTransactionExternal11Dictionary.ReadOnlyAction;

        public override XmlDictionaryString ReplayAction =>
            DXD.AtomicTransactionExternal11Dictionary.ReplayAction;

        public override XmlDictionaryString RollbackAction =>
            DXD.AtomicTransactionExternal11Dictionary.RollbackAction;

        public override XmlDictionaryString Volatile2PCUri =>
            DXD.AtomicTransactionExternal11Dictionary.Volatile2PCUri;
    }
}

