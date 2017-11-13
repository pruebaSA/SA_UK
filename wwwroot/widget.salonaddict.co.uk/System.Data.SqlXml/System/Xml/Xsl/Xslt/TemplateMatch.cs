namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;

    internal class TemplateMatch
    {
        public static readonly TemplateMatchComparer Comparer = new TemplateMatchComparer();
        private QilNode condition;
        private QilIterator iterator;
        private XmlNodeKindFlags nodeKind;
        private double priority;
        private QilName qname;
        private Template template;

        public TemplateMatch(Template template, QilLoop filter)
        {
            this.template = template;
            this.priority = double.IsNaN(template.Priority) ? XPathPatternBuilder.GetPriority(filter) : template.Priority;
            this.iterator = filter.Variable;
            this.condition = filter.Body;
            XPathPatternBuilder.CleanAnnotation(filter);
            this.NipOffTypeNameCheck();
        }

        private void NipOffTypeNameCheck()
        {
            QilBinary[] binaryArray = new QilBinary[4];
            int num = -1;
            QilNode condition = this.condition;
            this.nodeKind = XmlNodeKindFlags.None;
            this.qname = null;
            while (condition.NodeType == QilNodeType.And)
            {
                QilBinary binary6;
                binaryArray[++num & 3] = binary6 = (QilBinary) condition;
                condition = binary6.Left;
            }
            if (condition.NodeType == QilNodeType.IsType)
            {
                QilBinary binary = (QilBinary) condition;
                if ((binary.Left == this.iterator) && (binary.Right.NodeType == QilNodeType.LiteralType))
                {
                    XmlNodeKindFlags nodeKinds = binary.Right.XmlType.NodeKinds;
                    if (Bits.ExactlyOne((uint) nodeKinds))
                    {
                        this.nodeKind = nodeKinds;
                        QilBinary binary2 = binaryArray[num & 3];
                        if ((binary2 != null) && (binary2.Right.NodeType == QilNodeType.Eq))
                        {
                            QilBinary right = (QilBinary) binary2.Right;
                            if (((right.Left.NodeType == QilNodeType.NameOf) && (((QilUnary) right.Left).Child == this.iterator)) && (right.Right.NodeType == QilNodeType.LiteralQName))
                            {
                                this.qname = (QilName) ((QilLiteral) right.Right).Value;
                                num--;
                            }
                        }
                        QilBinary binary4 = binaryArray[num & 3];
                        QilBinary binary5 = binaryArray[--num & 3];
                        if (binary5 != null)
                        {
                            binary5.Left = binary4.Right;
                        }
                        else if (binary4 != null)
                        {
                            this.condition = binary4.Right;
                        }
                        else
                        {
                            this.condition = null;
                        }
                    }
                }
            }
        }

        public QilNode Condition =>
            this.condition;

        public QilIterator Iterator =>
            this.iterator;

        public XmlNodeKindFlags NodeKind =>
            this.nodeKind;

        public QilName QName =>
            this.qname;

        public QilFunction TemplateFunction =>
            this.template.Function;

        internal class TemplateMatchComparer : IComparer<TemplateMatch>
        {
            public int Compare(TemplateMatch x, TemplateMatch y)
            {
                if (x.priority > y.priority)
                {
                    return 1;
                }
                if (x.priority >= y.priority)
                {
                    return (x.template.OrderNumber - y.template.OrderNumber);
                }
                return -1;
            }
        }
    }
}

