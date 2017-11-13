namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections;
    using System.Xml;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;

    internal class XmlILElementAnalyzer : XmlILStateAnalyzer
    {
        private NameTable attrNames;
        private ArrayList dupAttrs;

        public XmlILElementAnalyzer(QilFactory fac) : base(fac)
        {
            this.attrNames = new NameTable();
            this.dupAttrs = new ArrayList();
        }

        public override QilNode Analyze(QilNode ndElem, QilNode ndContent)
        {
            base.parentInfo = XmlILConstructInfo.Write(ndElem);
            base.parentInfo.MightHaveNamespacesAfterAttributes = false;
            base.parentInfo.MightHaveAttributes = false;
            base.parentInfo.MightHaveDuplicateAttributes = false;
            base.parentInfo.MightHaveNamespaces = !base.parentInfo.IsNamespaceInScope;
            this.dupAttrs.Clear();
            return base.Analyze(ndElem, ndContent);
        }

        private void AnalyzeAttributeCtor(QilBinary ndAttr, XmlILConstructInfo info)
        {
            if (ndAttr.Left.NodeType == QilNodeType.LiteralQName)
            {
                QilName left = ndAttr.Left as QilName;
                base.parentInfo.MightHaveAttributes = true;
                if (!base.parentInfo.MightHaveDuplicateAttributes)
                {
                    XmlQualifiedName name2 = new XmlQualifiedName(this.attrNames.Add(left.LocalName), this.attrNames.Add(left.NamespaceUri));
                    int num = 0;
                    while (num < this.dupAttrs.Count)
                    {
                        XmlQualifiedName name3 = (XmlQualifiedName) this.dupAttrs[num];
                        if ((name3.Name == name2.Name) && (name3.Namespace == name2.Namespace))
                        {
                            base.parentInfo.MightHaveDuplicateAttributes = true;
                        }
                        num++;
                    }
                    if (num >= this.dupAttrs.Count)
                    {
                        this.dupAttrs.Add(name2);
                    }
                }
                if (!info.IsNamespaceInScope)
                {
                    base.parentInfo.MightHaveNamespaces = true;
                }
            }
            else
            {
                this.CheckAttributeNamespaceConstruct(ndAttr.XmlType);
            }
        }

        protected override void AnalyzeCopy(QilNode ndCopy, XmlILConstructInfo info)
        {
            if (ndCopy.NodeType == QilNodeType.AttributeCtor)
            {
                this.AnalyzeAttributeCtor(ndCopy as QilBinary, info);
            }
            else
            {
                this.CheckAttributeNamespaceConstruct(ndCopy.XmlType);
            }
            base.AnalyzeCopy(ndCopy, info);
        }

        protected override void AnalyzeLoop(QilLoop ndLoop, XmlILConstructInfo info)
        {
            if (ndLoop.XmlType.MaybeMany)
            {
                this.CheckAttributeNamespaceConstruct(ndLoop.XmlType);
            }
            base.AnalyzeLoop(ndLoop, info);
        }

        private void CheckAttributeNamespaceConstruct(XmlQueryType typ)
        {
            if ((typ.NodeKinds & XmlNodeKindFlags.Attribute) != XmlNodeKindFlags.None)
            {
                base.parentInfo.MightHaveAttributes = true;
                base.parentInfo.MightHaveDuplicateAttributes = true;
                base.parentInfo.MightHaveNamespaces = true;
            }
            if ((typ.NodeKinds & XmlNodeKindFlags.Namespace) != XmlNodeKindFlags.None)
            {
                base.parentInfo.MightHaveNamespaces = true;
                if (base.parentInfo.MightHaveAttributes)
                {
                    base.parentInfo.MightHaveNamespacesAfterAttributes = true;
                }
            }
        }
    }
}

