namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Collections;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class CompiledXpathExpr : XPathExpression
    {
        private string expr;
        private bool needContext;
        private Query query;

        internal CompiledXpathExpr(Query query, string expression, bool needContext)
        {
            this.query = query;
            this.expr = expression;
            this.needContext = needContext;
        }

        public override void AddSort(object expr, IComparer comparer)
        {
            Query queryTree;
            if (expr is string)
            {
                queryTree = new QueryBuilder().Build((string) expr, out this.needContext);
            }
            else
            {
                if (!(expr is CompiledXpathExpr))
                {
                    throw XPathException.Create("Xp_BadQueryObject");
                }
                queryTree = ((CompiledXpathExpr) expr).QueryTree;
            }
            SortQuery query2 = this.query as SortQuery;
            if (query2 == null)
            {
                this.query = query2 = new SortQuery(this.query);
            }
            query2.AddSort(queryTree, comparer);
        }

        public override void AddSort(object expr, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
        {
            this.AddSort(expr, new XPathComparerHelper(order, caseOrder, lang, dataType));
        }

        public virtual void CheckErrors()
        {
        }

        public override XPathExpression Clone() => 
            new CompiledXpathExpr(Query.Clone(this.query), this.expr, this.needContext);

        public override void SetContext(IXmlNamespaceResolver nsResolver)
        {
            XmlNamespaceManager nsManager = nsResolver as XmlNamespaceManager;
            if ((nsManager == null) && (nsResolver != null))
            {
                nsManager = new XmlNamespaceManager(new NameTable());
            }
            this.SetContext(nsManager);
        }

        public override void SetContext(XmlNamespaceManager nsManager)
        {
            XsltContext context = nsManager as XsltContext;
            if (context == null)
            {
                if (nsManager == null)
                {
                    nsManager = new XmlNamespaceManager(new NameTable());
                }
                context = new UndefinedXsltContext(nsManager);
            }
            this.query.SetXsltContext(context);
            this.needContext = false;
        }

        public override string Expression =>
            this.expr;

        internal Query QueryTree
        {
            get
            {
                if (this.needContext)
                {
                    throw XPathException.Create("Xp_NoContext");
                }
                return this.query;
            }
        }

        public override XPathResultType ReturnType =>
            this.query.StaticType;

        private class UndefinedXsltContext : XsltContext
        {
            private XmlNamespaceManager nsManager;

            public UndefinedXsltContext(XmlNamespaceManager nsManager) : base(false)
            {
                this.nsManager = nsManager;
            }

            public override int CompareDocument(string baseUri, string nextbaseUri) => 
                string.CompareOrdinal(baseUri, nextbaseUri);

            public override string LookupNamespace(string prefix)
            {
                if (prefix.Length == 0)
                {
                    return string.Empty;
                }
                string str = this.nsManager.LookupNamespace(prefix);
                if (str == null)
                {
                    throw XPathException.Create("XmlUndefinedAlias", prefix);
                }
                return str;
            }

            public override bool PreserveWhitespace(XPathNavigator node) => 
                false;

            public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
            {
                throw XPathException.Create("Xp_UndefinedXsltContext");
            }

            public override IXsltContextVariable ResolveVariable(string prefix, string name)
            {
                throw XPathException.Create("Xp_UndefinedXsltContext");
            }

            public override string DefaultNamespace =>
                string.Empty;

            public override bool Whitespace =>
                false;
        }
    }
}

