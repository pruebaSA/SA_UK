namespace System.Xml
{
    using System;
    using System.Collections.Generic;

    internal class XmlCharCheckingReaderWithNS : XmlCharCheckingReader, IXmlNamespaceResolver
    {
        internal IXmlNamespaceResolver readerAsNSResolver;

        internal XmlCharCheckingReaderWithNS(XmlReader reader, IXmlNamespaceResolver readerAsNSResolver, bool checkCharacters, bool ignoreWhitespace, bool ignoreComments, bool ignorePis, bool prohibitDtd) : base(reader, checkCharacters, ignoreWhitespace, ignoreComments, ignorePis, prohibitDtd)
        {
            this.readerAsNSResolver = readerAsNSResolver;
        }

        IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope) => 
            this.readerAsNSResolver.GetNamespacesInScope(scope);

        string IXmlNamespaceResolver.LookupNamespace(string prefix) => 
            this.readerAsNSResolver.LookupNamespace(prefix);

        string IXmlNamespaceResolver.LookupPrefix(string namespaceName) => 
            this.readerAsNSResolver.LookupPrefix(namespaceName);
    }
}

