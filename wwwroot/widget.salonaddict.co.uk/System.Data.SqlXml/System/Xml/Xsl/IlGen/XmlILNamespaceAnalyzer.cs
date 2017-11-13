namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl.Qil;

    internal class XmlILNamespaceAnalyzer
    {
        private bool addInScopeNmsp;
        private int cntNmsp;
        private XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());

        public void Analyze(QilNode nd, bool defaultNmspInScope)
        {
            this.addInScopeNmsp = false;
            this.cntNmsp = 0;
            if (defaultNmspInScope)
            {
                this.nsmgr.PushScope();
                this.nsmgr.AddNamespace(string.Empty, string.Empty);
                this.cntNmsp++;
            }
            this.AnalyzeContent(nd);
            if (defaultNmspInScope)
            {
                this.nsmgr.PopScope();
            }
        }

        private void AnalyzeContent(QilNode nd)
        {
            switch (nd.NodeType)
            {
                case QilNodeType.ElementCtor:
                {
                    this.addInScopeNmsp = true;
                    this.nsmgr.PushScope();
                    int cntNmsp = this.cntNmsp;
                    if (this.CheckNamespaceInScope(nd as QilBinary))
                    {
                        this.AnalyzeContent((nd as QilBinary).Right);
                    }
                    this.nsmgr.PopScope();
                    this.addInScopeNmsp = false;
                    this.cntNmsp = cntNmsp;
                    return;
                }
                case QilNodeType.AttributeCtor:
                    this.addInScopeNmsp = false;
                    this.CheckNamespaceInScope(nd as QilBinary);
                    return;

                case QilNodeType.NamespaceDecl:
                    this.CheckNamespaceInScope(nd as QilBinary);
                    return;

                case QilNodeType.Loop:
                    this.addInScopeNmsp = false;
                    this.AnalyzeContent((nd as QilLoop).Body);
                    return;

                case QilNodeType.Conditional:
                    this.addInScopeNmsp = false;
                    this.AnalyzeContent((nd as QilTernary).Center);
                    this.AnalyzeContent((nd as QilTernary).Right);
                    return;

                case QilNodeType.Choice:
                {
                    this.addInScopeNmsp = false;
                    QilList branches = (nd as QilChoice).Branches;
                    for (int i = 0; i < branches.Count; i++)
                    {
                        this.AnalyzeContent(branches[i]);
                    }
                    return;
                }
                case QilNodeType.Sequence:
                    foreach (QilNode node in nd)
                    {
                        this.AnalyzeContent(node);
                    }
                    return;

                case QilNodeType.Nop:
                    this.AnalyzeContent((nd as QilUnary).Child);
                    return;
            }
            this.addInScopeNmsp = false;
        }

        private bool CheckNamespaceInScope(QilBinary nd)
        {
            string prefix;
            string namespaceUri;
            XPathNodeType type;
            switch (nd.NodeType)
            {
                case QilNodeType.ElementCtor:
                case QilNodeType.AttributeCtor:
                {
                    QilName left = nd.Left as QilName;
                    if (left == null)
                    {
                        return false;
                    }
                    prefix = left.Prefix;
                    namespaceUri = left.NamespaceUri;
                    type = (nd.NodeType == QilNodeType.ElementCtor) ? XPathNodeType.Element : XPathNodeType.Attribute;
                    break;
                }
                default:
                    prefix = (string) ((QilLiteral) nd.Left);
                    namespaceUri = (string) ((QilLiteral) nd.Right);
                    type = XPathNodeType.Namespace;
                    break;
            }
            if (((nd.NodeType == QilNodeType.AttributeCtor) && (namespaceUri.Length == 0)) || ((prefix == "xml") && (namespaceUri == "http://www.w3.org/XML/1998/namespace")))
            {
                XmlILConstructInfo.Write(nd).IsNamespaceInScope = true;
                return true;
            }
            if (!ValidateNames.ValidateName(prefix, string.Empty, namespaceUri, type, ValidateNames.Flags.CheckPrefixMapping))
            {
                return false;
            }
            prefix = this.nsmgr.NameTable.Add(prefix);
            namespaceUri = this.nsmgr.NameTable.Add(namespaceUri);
            for (int i = 0; i < this.cntNmsp; i++)
            {
                string str3;
                string str4;
                this.nsmgr.GetNamespaceDeclaration(i, out str3, out str4);
                if (prefix == str3)
                {
                    if (namespaceUri == str4)
                    {
                        XmlILConstructInfo.Write(nd).IsNamespaceInScope = true;
                    }
                    break;
                }
            }
            if (this.addInScopeNmsp)
            {
                this.nsmgr.AddNamespace(prefix, namespaceUri);
                this.cntNmsp++;
            }
            return true;
        }
    }
}

