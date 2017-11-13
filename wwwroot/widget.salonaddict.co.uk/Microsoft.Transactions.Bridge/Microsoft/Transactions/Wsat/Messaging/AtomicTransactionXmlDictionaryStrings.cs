namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal abstract class AtomicTransactionXmlDictionaryStrings
    {
        protected AtomicTransactionXmlDictionaryStrings()
        {
        }

        public static AtomicTransactionXmlDictionaryStrings Version(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(AtomicTransactionXmlDictionaryStrings), "V");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return AtomicTransactionXmlDictionaryStrings10.Instance;

                case ProtocolVersion.Version11:
                    return AtomicTransactionXmlDictionaryStrings11.Instance;
            }
            return null;
        }

        public XmlDictionaryString Aborted =>
            XD.AtomicTransactionExternalDictionary.Aborted;

        public abstract XmlDictionaryString AbortedAction { get; }

        public XmlDictionaryString Commit =>
            XD.AtomicTransactionExternalDictionary.Commit;

        public abstract XmlDictionaryString CommitAction { get; }

        public XmlDictionaryString Committed =>
            XD.AtomicTransactionExternalDictionary.Committed;

        public abstract XmlDictionaryString CommittedAction { get; }

        public XmlDictionaryString CompletionCoordinatorPortType =>
            XD.AtomicTransactionExternalDictionary.CompletionCoordinatorPortType;

        public XmlDictionaryString CompletionParticipantPortType =>
            XD.AtomicTransactionExternalDictionary.CompletionParticipantPortType;

        public abstract XmlDictionaryString CompletionUri { get; }

        public XmlDictionaryString CoordinatorPortType =>
            XD.AtomicTransactionExternalDictionary.CoordinatorPortType;

        public abstract XmlDictionaryString Durable2PCUri { get; }

        public abstract XmlDictionaryString FaultAction { get; }

        public XmlDictionaryString InconsistentInternalState =>
            XD.AtomicTransactionExternalDictionary.InconsistentInternalState;

        public abstract XmlDictionaryString Namespace { get; }

        public XmlDictionaryString ParticipantPortType =>
            XD.AtomicTransactionExternalDictionary.ParticipantPortType;

        public XmlDictionaryString Prefix =>
            XD.AtomicTransactionExternalDictionary.Prefix;

        public XmlDictionaryString Prepare =>
            XD.AtomicTransactionExternalDictionary.Prepare;

        public abstract XmlDictionaryString PrepareAction { get; }

        public XmlDictionaryString Prepared =>
            XD.AtomicTransactionExternalDictionary.Prepared;

        public abstract XmlDictionaryString PreparedAction { get; }

        public XmlDictionaryString ReadOnly =>
            XD.AtomicTransactionExternalDictionary.ReadOnly;

        public abstract XmlDictionaryString ReadOnlyAction { get; }

        public XmlDictionaryString Replay =>
            XD.AtomicTransactionExternalDictionary.Replay;

        public abstract XmlDictionaryString ReplayAction { get; }

        public XmlDictionaryString Rollback =>
            XD.AtomicTransactionExternalDictionary.Rollback;

        public abstract XmlDictionaryString RollbackAction { get; }

        public XmlDictionaryString UnknownTransaction =>
            DXD.AtomicTransactionExternal11Dictionary.UnknownTransaction;

        public abstract XmlDictionaryString Volatile2PCUri { get; }
    }
}

