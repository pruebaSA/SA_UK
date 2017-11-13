namespace Microsoft.Transactions.Wsat.Messaging
{
    using System.ServiceModel;
    using System.Xml;

    internal class AtomicTransactionXmlDictionaryStrings10 : AtomicTransactionXmlDictionaryStrings
    {
        private static AtomicTransactionXmlDictionaryStrings instance = new AtomicTransactionXmlDictionaryStrings10();

        public override XmlDictionaryString AbortedAction =>
            XD.AtomicTransactionExternal10Dictionary.AbortedAction;

        public override XmlDictionaryString CommitAction =>
            XD.AtomicTransactionExternal10Dictionary.CommitAction;

        public override XmlDictionaryString CommittedAction =>
            XD.AtomicTransactionExternal10Dictionary.CommittedAction;

        public override XmlDictionaryString CompletionUri =>
            XD.AtomicTransactionExternal10Dictionary.CompletionUri;

        public override XmlDictionaryString Durable2PCUri =>
            XD.AtomicTransactionExternal10Dictionary.Durable2PCUri;

        public override XmlDictionaryString FaultAction =>
            XD.AtomicTransactionExternal10Dictionary.FaultAction;

        public static AtomicTransactionXmlDictionaryStrings Instance =>
            instance;

        public override XmlDictionaryString Namespace =>
            XD.AtomicTransactionExternal10Dictionary.Namespace;

        public override XmlDictionaryString PrepareAction =>
            XD.AtomicTransactionExternal10Dictionary.PrepareAction;

        public override XmlDictionaryString PreparedAction =>
            XD.AtomicTransactionExternal10Dictionary.PreparedAction;

        public override XmlDictionaryString ReadOnlyAction =>
            XD.AtomicTransactionExternal10Dictionary.ReadOnlyAction;

        public override XmlDictionaryString ReplayAction =>
            XD.AtomicTransactionExternal10Dictionary.ReplayAction;

        public override XmlDictionaryString RollbackAction =>
            XD.AtomicTransactionExternal10Dictionary.RollbackAction;

        public override XmlDictionaryString Volatile2PCUri =>
            XD.AtomicTransactionExternal10Dictionary.Volatile2PCUri;
    }
}

