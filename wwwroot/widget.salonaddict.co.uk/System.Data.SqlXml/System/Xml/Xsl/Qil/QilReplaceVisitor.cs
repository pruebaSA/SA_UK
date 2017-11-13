namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Xml.Xsl;

    internal abstract class QilReplaceVisitor : QilVisitor
    {
        protected QilFactory f;

        public QilReplaceVisitor(QilFactory f)
        {
            this.f = f;
        }

        protected virtual void RecalculateType(QilNode node, XmlQueryType oldType)
        {
            XmlQueryType type = this.f.TypeChecker.Check(node);
            node.XmlType = type;
        }

        protected override QilNode VisitChildren(QilNode parent)
        {
            XmlQueryType xmlType = parent.XmlType;
            bool flag = false;
            for (int i = 0; i < parent.Count; i++)
            {
                QilNode node2;
                QilNode n = parent[i];
                XmlQueryType objA = n?.XmlType;
                if (this.IsReference(parent, i))
                {
                    node2 = this.VisitReference(n);
                }
                else
                {
                    node2 = this.Visit(n);
                }
                if (!object.Equals(n, node2) || ((node2 != null) && !object.Equals(objA, node2.XmlType)))
                {
                    flag = true;
                    parent[i] = node2;
                }
            }
            if (flag)
            {
                this.RecalculateType(parent, xmlType);
            }
            return parent;
        }
    }
}

