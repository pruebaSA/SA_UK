namespace System.Xml.Xsl
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;

    internal abstract class XmlCommand
    {
        protected XmlCommand()
        {
        }

        public abstract IList Evaluate(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList);
        public abstract void Execute(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, Stream results);
        public abstract void Execute(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, TextWriter results);
        public abstract void Execute(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter results);
        public abstract void Execute(IXPathNavigable contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, Stream results);
        public abstract void Execute(IXPathNavigable contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, TextWriter results);
        public abstract void Execute(IXPathNavigable contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter results);
    }
}

