namespace System.Xml.Xsl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Xml;
    using System.Xml.XmlConfiguration;
    using System.Xml.XPath;
    using System.Xml.Xsl.XsltOld;
    using System.Xml.Xsl.XsltOld.Debugger;

    [Obsolete("This class has been deprecated. Please use System.Xml.Xsl.XslCompiledTransform instead. http://go.microsoft.com/fwlink/?linkid=14202")]
    public sealed class XslTransform
    {
        private Stylesheet _CompiledStylesheet;
        private System.Xml.XmlResolver _documentResolver;
        private List<TheQuery> _QueryStore;
        private RootAction _RootAction;
        private IXsltDebugger debugger;
        private bool isDocumentResolverSet;

        public XslTransform()
        {
        }

        internal XslTransform(object debugger)
        {
            if (debugger != null)
            {
                this.debugger = new DebuggerAddapter(debugger);
            }
        }

        private void CheckCommand()
        {
            if (this._CompiledStylesheet == null)
            {
                throw new InvalidOperationException(Res.GetString("Xslt_NoStylesheetLoaded"));
            }
        }

        private void Compile(XPathNavigator stylesheet, System.Xml.XmlResolver resolver, Evidence evidence)
        {
            Compiler compiler = (this.Debugger == null) ? new Compiler() : new DbgCompiler(this.Debugger);
            NavigatorInput input = new NavigatorInput(stylesheet);
            compiler.Compile(input, resolver, evidence);
            this._CompiledStylesheet = compiler.CompiledStylesheet;
            this._QueryStore = compiler.QueryStore;
            this._RootAction = compiler.RootAction;
        }

        public void Load(string url)
        {
            XmlTextReaderImpl reader = new XmlTextReaderImpl(url);
            Evidence evidence = XmlSecureResolver.CreateEvidenceForUrl(reader.BaseURI);
            this.Compile(Compiler.LoadDocument(reader).CreateNavigator(), XsltConfigSection.CreateDefaultResolver(), evidence);
        }

        public void Load(XmlReader stylesheet)
        {
            this.Load(stylesheet, XsltConfigSection.CreateDefaultResolver());
        }

        public void Load(IXPathNavigable stylesheet)
        {
            this.Load(stylesheet, XsltConfigSection.CreateDefaultResolver());
        }

        public void Load(XPathNavigator stylesheet)
        {
            if (stylesheet == null)
            {
                throw new ArgumentNullException("stylesheet");
            }
            this.Load(stylesheet, XsltConfigSection.CreateDefaultResolver());
        }

        public void Load(string url, System.Xml.XmlResolver resolver)
        {
            XmlTextReaderImpl reader = new XmlTextReaderImpl(url) {
                XmlResolver = resolver
            };
            Evidence evidence = XmlSecureResolver.CreateEvidenceForUrl(reader.BaseURI);
            if (resolver == null)
            {
                resolver = new XmlNullResolver();
            }
            this.Compile(Compiler.LoadDocument(reader).CreateNavigator(), resolver, evidence);
        }

        public void Load(XmlReader stylesheet, System.Xml.XmlResolver resolver)
        {
            this.Load(new XPathDocument(stylesheet, XmlSpace.Preserve), resolver);
        }

        public void Load(IXPathNavigable stylesheet, System.Xml.XmlResolver resolver)
        {
            if (stylesheet == null)
            {
                throw new ArgumentNullException("stylesheet");
            }
            this.Load(stylesheet.CreateNavigator(), resolver);
        }

        public void Load(XPathNavigator stylesheet, System.Xml.XmlResolver resolver)
        {
            if (stylesheet == null)
            {
                throw new ArgumentNullException("stylesheet");
            }
            if (resolver == null)
            {
                resolver = new XmlNullResolver();
            }
            this.Compile(stylesheet, resolver, null);
        }

        public void Load(XmlReader stylesheet, System.Xml.XmlResolver resolver, Evidence evidence)
        {
            if (stylesheet == null)
            {
                throw new ArgumentNullException("stylesheet");
            }
            this.Load(new XPathDocument(stylesheet, XmlSpace.Preserve), resolver, evidence);
        }

        public void Load(IXPathNavigable stylesheet, System.Xml.XmlResolver resolver, Evidence evidence)
        {
            if (stylesheet == null)
            {
                throw new ArgumentNullException("stylesheet");
            }
            this.Load(stylesheet.CreateNavigator(), resolver, evidence);
        }

        public void Load(XPathNavigator stylesheet, System.Xml.XmlResolver resolver, Evidence evidence)
        {
            if (stylesheet == null)
            {
                throw new ArgumentNullException("stylesheet");
            }
            if (resolver == null)
            {
                resolver = new XmlNullResolver();
            }
            if (evidence == null)
            {
                evidence = new Evidence();
            }
            else
            {
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
            }
            this.Compile(stylesheet, resolver, evidence);
        }

        public void Transform(string inputfile, string outputfile)
        {
            this.Transform(inputfile, outputfile, this._DocumentResolver);
        }

        public XmlReader Transform(IXPathNavigable input, XsltArgumentList args)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            return this.Transform(input.CreateNavigator(), args, this._DocumentResolver);
        }

        public XmlReader Transform(XPathNavigator input, XsltArgumentList args) => 
            this.Transform(input, args, this._DocumentResolver);

        public void Transform(string inputfile, string outputfile, System.Xml.XmlResolver resolver)
        {
            FileStream stream = null;
            try
            {
                XPathDocument document = new XPathDocument(inputfile);
                stream = new FileStream(outputfile, FileMode.Create, FileAccess.ReadWrite);
                this.Transform((IXPathNavigable) document, null, (Stream) stream, resolver);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Transform(input.CreateNavigator(), args, output, this._DocumentResolver);
        }

        public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Transform(input.CreateNavigator(), args, output, this._DocumentResolver);
        }

        public XmlReader Transform(IXPathNavigable input, XsltArgumentList args, System.Xml.XmlResolver resolver)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            return this.Transform(input.CreateNavigator(), args, resolver);
        }

        public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Transform(input.CreateNavigator(), args, output, this._DocumentResolver);
        }

        public void Transform(XPathNavigator input, XsltArgumentList args, Stream output)
        {
            this.Transform(input, args, output, this._DocumentResolver);
        }

        public void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output)
        {
            this.CheckCommand();
            new Processor(input, args, this._DocumentResolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger).Execute(output);
        }

        public XmlReader Transform(XPathNavigator input, XsltArgumentList args, System.Xml.XmlResolver resolver)
        {
            this.CheckCommand();
            Processor processor = new Processor(input, args, resolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger);
            return processor.StartReader();
        }

        public void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output)
        {
            this.Transform(input, args, output, this._DocumentResolver);
        }

        public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output, System.Xml.XmlResolver resolver)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Transform(input.CreateNavigator(), args, output, resolver);
        }

        public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output, System.Xml.XmlResolver resolver)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Transform(input.CreateNavigator(), args, output, resolver);
        }

        public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output, System.Xml.XmlResolver resolver)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Transform(input.CreateNavigator(), args, output, resolver);
        }

        public void Transform(XPathNavigator input, XsltArgumentList args, Stream output, System.Xml.XmlResolver resolver)
        {
            this.CheckCommand();
            new Processor(input, args, resolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger).Execute(output);
        }

        public void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output, System.Xml.XmlResolver resolver)
        {
            this.CheckCommand();
            new Processor(input, args, resolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger).Execute(output);
        }

        public void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output, System.Xml.XmlResolver resolver)
        {
            this.CheckCommand();
            new Processor(input, args, resolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger).Execute(output);
        }

        private System.Xml.XmlResolver _DocumentResolver
        {
            get
            {
                if (this.isDocumentResolverSet)
                {
                    return this._documentResolver;
                }
                return XsltConfigSection.CreateDefaultResolver();
            }
        }

        internal IXsltDebugger Debugger =>
            this.debugger;

        public System.Xml.XmlResolver XmlResolver
        {
            set
            {
                this._documentResolver = value;
                this.isDocumentResolverSet = true;
            }
        }

        private class DebuggerAddapter : IXsltDebugger
        {
            private MethodInfo getBltIn;
            private MethodInfo onCompile;
            private MethodInfo onExecute;
            private object unknownDebugger;

            public DebuggerAddapter(object unknownDebugger)
            {
                this.unknownDebugger = unknownDebugger;
                BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                Type type = unknownDebugger.GetType();
                this.getBltIn = type.GetMethod("GetBuiltInTemplatesUri", bindingAttr);
                this.onCompile = type.GetMethod("OnInstructionCompile", bindingAttr);
                this.onExecute = type.GetMethod("OnInstructionExecute", bindingAttr);
            }

            public string GetBuiltInTemplatesUri()
            {
                if (this.getBltIn == null)
                {
                    return null;
                }
                return (string) this.getBltIn.Invoke(this.unknownDebugger, new object[0]);
            }

            public void OnInstructionCompile(XPathNavigator styleSheetNavigator)
            {
                if (this.onCompile != null)
                {
                    this.onCompile.Invoke(this.unknownDebugger, new object[] { styleSheetNavigator });
                }
            }

            public void OnInstructionExecute(IXsltProcessor xsltProcessor)
            {
                if (this.onExecute != null)
                {
                    this.onExecute.Invoke(this.unknownDebugger, new object[] { xsltProcessor });
                }
            }
        }
    }
}

