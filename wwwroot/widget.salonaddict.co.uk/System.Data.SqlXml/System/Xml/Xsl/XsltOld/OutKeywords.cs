namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Diagnostics;
    using System.Xml;

    internal class OutKeywords
    {
        private string _AtomEmpty;
        private string _AtomLang;
        private string _AtomSpace;
        private string _AtomXml;
        private string _AtomXmlNamespace;
        private string _AtomXmlns;
        private string _AtomXmlnsNamespace;

        internal OutKeywords(XmlNameTable nameTable)
        {
            this._AtomEmpty = nameTable.Add(string.Empty);
            this._AtomLang = nameTable.Add("lang");
            this._AtomSpace = nameTable.Add("space");
            this._AtomXmlns = nameTable.Add("xmlns");
            this._AtomXml = nameTable.Add("xml");
            this._AtomXmlNamespace = nameTable.Add("http://www.w3.org/XML/1998/namespace");
            this._AtomXmlnsNamespace = nameTable.Add("http://www.w3.org/2000/xmlns/");
        }

        [Conditional("DEBUG")]
        private void CheckKeyword(string keyword)
        {
        }

        internal string Empty =>
            this._AtomEmpty;

        internal string Lang =>
            this._AtomLang;

        internal string Space =>
            this._AtomSpace;

        internal string Xml =>
            this._AtomXml;

        internal string XmlNamespace =>
            this._AtomXmlNamespace;

        internal string Xmlns =>
            this._AtomXmlns;

        internal string XmlnsNamespace =>
            this._AtomXmlnsNamespace;
    }
}

