namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal abstract class AddressingHeader : DictionaryHeader, IMessageHeaderWithSharedNamespace
    {
        private AddressingVersion version;

        protected AddressingHeader(AddressingVersion version)
        {
            this.version = version;
        }

        public override XmlDictionaryString DictionaryNamespace =>
            this.version.DictionaryNamespace;

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedNamespace =>
            this.version.DictionaryNamespace;

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedPrefix =>
            XD.AddressingDictionary.Prefix;

        internal AddressingVersion Version =>
            this.version;
    }
}

