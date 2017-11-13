namespace System.Xml.Xsl
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl.Runtime;

    internal class XmlILCommand : XmlCommand
    {
        private System.Xml.Xsl.ExecuteDelegate delExec;
        private XmlQueryStaticData staticData;

        public XmlILCommand(System.Xml.Xsl.ExecuteDelegate delExec, XmlQueryStaticData staticData)
        {
            this.delExec = delExec;
            this.staticData = staticData;
        }

        public IList Evaluate(string contextDocumentUri, XmlResolver dataSources, XsltArgumentList argumentList)
        {
            XmlCachedSequenceWriter results = new XmlCachedSequenceWriter();
            this.Execute(contextDocumentUri, dataSources, argumentList, results);
            return results.ResultSequence;
        }

        public override IList Evaluate(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList)
        {
            XmlCachedSequenceWriter results = new XmlCachedSequenceWriter();
            this.Execute(contextDocument, dataSources, argumentList, results);
            return results.ResultSequence;
        }

        private void Execute(object defaultDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlSequenceWriter results)
        {
            if (dataSources == null)
            {
                dataSources = XmlNullResolver.Singleton;
            }
            this.delExec(new XmlQueryRuntime(this.staticData, defaultDocument, dataSources, argumentList, results));
        }

        public void Execute(string contextDocumentUri, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            this.Execute(contextDocumentUri, dataSources, argumentList, results, false);
        }

        public override void Execute(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, Stream results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            this.Execute(contextDocument, dataSources, argumentList, XmlWriter.Create(results, this.staticData.DefaultWriterSettings), true);
        }

        public override void Execute(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, TextWriter results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            this.Execute(contextDocument, dataSources, argumentList, XmlWriter.Create(results, this.staticData.DefaultWriterSettings), true);
        }

        public override void Execute(XmlReader contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            this.Execute(contextDocument, dataSources, argumentList, results, false);
        }

        public override void Execute(IXPathNavigable contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, Stream results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            this.Execute(contextDocument, dataSources, argumentList, XmlWriter.Create(results, this.staticData.DefaultWriterSettings));
        }

        public override void Execute(IXPathNavigable contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, TextWriter results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            this.Execute(contextDocument, dataSources, argumentList, XmlWriter.Create(results, this.staticData.DefaultWriterSettings));
        }

        public override void Execute(IXPathNavigable contextDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            if (contextDocument != null)
            {
                this.Execute(contextDocument.CreateNavigator(), dataSources, argumentList, results, false);
            }
            else
            {
                this.Execute(null, dataSources, argumentList, results, false);
            }
        }

        private void Execute(object defaultDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter writer, bool closeWriter)
        {
            try
            {
                XmlWellFormedWriter writer2 = writer as XmlWellFormedWriter;
                if (((writer2 != null) && (writer2.WriteState == WriteState.Start)) && (writer2.Settings.ConformanceLevel != ConformanceLevel.Document))
                {
                    this.Execute(defaultDocument, dataSources, argumentList, new XmlMergeSequenceWriter(writer2.RawWriter));
                }
                else
                {
                    this.Execute(defaultDocument, dataSources, argumentList, new XmlMergeSequenceWriter(new XmlRawWriterWrapper(writer)));
                }
            }
            finally
            {
                if (closeWriter)
                {
                    writer.Close();
                }
                else
                {
                    writer.Flush();
                }
            }
        }

        public System.Xml.Xsl.ExecuteDelegate ExecuteDelegate =>
            this.delExec;

        public XmlQueryStaticData StaticData =>
            this.staticData;
    }
}

