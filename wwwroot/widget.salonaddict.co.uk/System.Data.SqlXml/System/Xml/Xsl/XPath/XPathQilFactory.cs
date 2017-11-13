namespace System.Xml.Xsl.XPath
{
    using System;
    using System.Diagnostics;
    using System.Xml.Schema;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;

    internal class XPathQilFactory : QilPatternFactory
    {
        public XPathQilFactory(QilFactory f, bool debug) : base(f, debug)
        {
        }

        public bool CannotBeNodeSet(QilNode n)
        {
            XmlQueryType xmlType = n.XmlType;
            return ((xmlType.IsAtomicValue && !xmlType.IsEmpty) && !(n is QilIterator));
        }

        [Conditional("DEBUG")]
        public void CheckAny(QilNode n)
        {
        }

        [Conditional("DEBUG")]
        public void CheckBool(QilNode n)
        {
        }

        [Conditional("DEBUG")]
        public void CheckDouble(QilNode n)
        {
        }

        [Conditional("DEBUG")]
        public void CheckNode(QilNode n)
        {
        }

        [Conditional("DEBUG")]
        public void CheckNodeNotRtf(QilNode n)
        {
        }

        [Conditional("DEBUG")]
        public void CheckNodeSet(QilNode n)
        {
        }

        [Conditional("DEBUG")]
        public void CheckString(QilNode n)
        {
        }

        [Conditional("DEBUG")]
        public void CheckStringS(QilNode n)
        {
        }

        public QilNode ConvertToBoolean(QilNode n)
        {
            switch (n.XmlType.TypeCode)
            {
                case XmlTypeCode.String:
                    if (n.NodeType == QilNodeType.LiteralString)
                    {
                        return base.Boolean(((QilLiteral) n).Length != 0);
                    }
                    return base.Ne(base.StrLength(n), base.Int32(0));

                case XmlTypeCode.Boolean:
                    return n;

                case XmlTypeCode.Double:
                    QilIterator iterator;
                    if (n.NodeType == QilNodeType.LiteralDouble)
                    {
                        return base.Boolean((((double) ((QilLiteral) n)) < 0.0) || (0.0 < ((double) ((QilLiteral) n))));
                    }
                    return base.Loop(iterator = base.Let(n), base.Or(base.Lt(iterator, base.Double(0.0)), base.Lt(base.Double(0.0), iterator)));
            }
            if (n.XmlType.IsNode)
            {
                return base.Not(base.IsEmpty(n));
            }
            return base.XsltConvert(n, XmlQueryTypeFactory.BooleanX);
        }

        public QilNode ConvertToNode(QilNode n)
        {
            if ((n.XmlType.IsNode && n.XmlType.IsNotRtf) && n.XmlType.IsSingleton)
            {
                return n;
            }
            return base.XsltConvert(n, XmlQueryTypeFactory.NodeNotRtf);
        }

        public QilNode ConvertToNodeSet(QilNode n)
        {
            if (n.XmlType.IsNode && n.XmlType.IsNotRtf)
            {
                return n;
            }
            return base.XsltConvert(n, XmlQueryTypeFactory.NodeNotRtfS);
        }

        public QilNode ConvertToNumber(QilNode n)
        {
            switch (n.XmlType.TypeCode)
            {
                case XmlTypeCode.String:
                    return base.XsltConvert(n, XmlQueryTypeFactory.DoubleX);

                case XmlTypeCode.Boolean:
                    if (n.NodeType == QilNodeType.True)
                    {
                        return base.Double(1.0);
                    }
                    if (n.NodeType == QilNodeType.False)
                    {
                        return base.Double(0.0);
                    }
                    return base.Conditional(n, base.Double(1.0), base.Double(0.0));

                case XmlTypeCode.Double:
                    return n;
            }
            if (n.XmlType.IsNode)
            {
                return base.XsltConvert(base.XPathNodeValue(this.SafeDocOrderDistinct(n)), XmlQueryTypeFactory.DoubleX);
            }
            return base.XsltConvert(n, XmlQueryTypeFactory.DoubleX);
        }

        public QilNode ConvertToString(QilNode n)
        {
            switch (n.XmlType.TypeCode)
            {
                case XmlTypeCode.String:
                    return n;

                case XmlTypeCode.Boolean:
                    if (n.NodeType == QilNodeType.True)
                    {
                        return base.String("true");
                    }
                    if (n.NodeType == QilNodeType.False)
                    {
                        return base.String("false");
                    }
                    return base.Conditional(n, base.String("true"), base.String("false"));

                case XmlTypeCode.Double:
                    if (n.NodeType == QilNodeType.LiteralDouble)
                    {
                        return base.String(XPathConvert.DoubleToString((double) ((QilLiteral) n)));
                    }
                    return base.XsltConvert(n, XmlQueryTypeFactory.StringX);
            }
            if (n.XmlType.IsNode)
            {
                return base.XPathNodeValue(this.SafeDocOrderDistinct(n));
            }
            return base.XsltConvert(n, XmlQueryTypeFactory.StringX);
        }

        public QilNode ConvertToType(XmlTypeCode requiredType, QilNode n)
        {
            switch (requiredType)
            {
                case XmlTypeCode.Item:
                    return n;

                case XmlTypeCode.Node:
                    return this.EnsureNodeSet(n);

                case XmlTypeCode.String:
                    return this.ConvertToString(n);

                case XmlTypeCode.Boolean:
                    return this.ConvertToBoolean(n);

                case XmlTypeCode.Double:
                    return this.ConvertToNumber(n);
            }
            return null;
        }

        public QilNode EnsureNodeSet(QilNode n)
        {
            if (n.XmlType.IsNode && n.XmlType.IsNotRtf)
            {
                return n;
            }
            if (this.CannotBeNodeSet(n))
            {
                throw new XPathCompileException("XPath_NodeSetExpected", new string[0]);
            }
            return this.InvokeEnsureNodeSet(n);
        }

        public QilNode Error(string res, QilNode args) => 
            base.Error(this.InvokeFormatMessage(base.String(res), args));

        public QilNode Error(ISourceLineInfo lineInfo, string res, params string[] args) => 
            base.Error(base.String(XslLoadException.CreateMessage(lineInfo, res, args)));

        [Conditional("DEBUG")]
        private void ExpectAny(QilNode n)
        {
        }

        public QilIterator FirstNode(QilNode n)
        {
            QilIterator variable = base.For(base.DocOrderDistinct(n));
            return base.For(base.Filter(variable, base.Eq(base.PositionOf(variable), base.Int32(1))));
        }

        public QilNode Id(QilNode context, QilNode id)
        {
            QilIterator iterator;
            if (id.XmlType.IsSingleton)
            {
                return base.Deref(context, this.ConvertToString(id));
            }
            return base.Loop(iterator = base.For(id), base.Deref(context, this.ConvertToString(iterator)));
        }

        public QilNode InvokeCeiling(QilNode value) => 
            base.XsltInvokeEarlyBound(base.QName("ceiling"), XsltMethods.Ceiling, XmlQueryTypeFactory.DoubleX, new QilNode[] { value });

        public QilNode InvokeContains(QilNode str1, QilNode str2) => 
            base.XsltInvokeEarlyBound(base.QName("contains"), XsltMethods.Contains, XmlQueryTypeFactory.BooleanX, new QilNode[] { str1, str2 });

        public QilNode InvokeEnsureNodeSet(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("ensure-node-set"), XsltMethods.EnsureNodeSet, XmlQueryTypeFactory.NodeDodS, new QilNode[] { n });

        public QilNode InvokeEqualityOperator(QilNodeType op, QilNode left, QilNode right)
        {
            double num;
            left = base.TypeAssert(left, XmlQueryTypeFactory.ItemS);
            right = base.TypeAssert(right, XmlQueryTypeFactory.ItemS);
            if (op == QilNodeType.Eq)
            {
                num = 0.0;
            }
            else
            {
                num = 1.0;
            }
            return base.XsltInvokeEarlyBound(base.QName("EqualityOperator"), XsltMethods.EqualityOperator, XmlQueryTypeFactory.BooleanX, new QilNode[] { base.Double(num), left, right });
        }

        public QilNode InvokeFloor(QilNode value) => 
            base.XsltInvokeEarlyBound(base.QName("floor"), XsltMethods.Floor, XmlQueryTypeFactory.DoubleX, new QilNode[] { value });

        public QilNode InvokeFormatMessage(QilNode res, QilNode args) => 
            base.XsltInvokeEarlyBound(base.QName("format-message"), XsltMethods.FormatMessage, XmlQueryTypeFactory.StringX, new QilNode[] { res, args });

        public QilNode InvokeLang(QilNode lang, QilNode context) => 
            base.XsltInvokeEarlyBound(base.QName("lang"), XsltMethods.Lang, XmlQueryTypeFactory.BooleanX, new QilNode[] { lang, context });

        public QilNode InvokeNormalizeSpace(QilNode str) => 
            base.XsltInvokeEarlyBound(base.QName("normalize-space"), XsltMethods.NormalizeSpace, XmlQueryTypeFactory.StringX, new QilNode[] { str });

        public QilNode InvokeRelationalOperator(QilNodeType op, QilNode left, QilNode right)
        {
            double num;
            left = base.TypeAssert(left, XmlQueryTypeFactory.ItemS);
            right = base.TypeAssert(right, XmlQueryTypeFactory.ItemS);
            switch (op)
            {
                case QilNodeType.Gt:
                    num = 4.0;
                    break;

                case QilNodeType.Lt:
                    num = 2.0;
                    break;

                case QilNodeType.Le:
                    num = 3.0;
                    break;

                default:
                    num = 5.0;
                    break;
            }
            return base.XsltInvokeEarlyBound(base.QName("RelationalOperator"), XsltMethods.RelationalOperator, XmlQueryTypeFactory.BooleanX, new QilNode[] { base.Double(num), left, right });
        }

        public QilNode InvokeRound(QilNode value) => 
            base.XsltInvokeEarlyBound(base.QName("round"), XsltMethods.Round, XmlQueryTypeFactory.DoubleX, new QilNode[] { value });

        public QilNode InvokeStartsWith(QilNode str1, QilNode str2) => 
            base.XsltInvokeEarlyBound(base.QName("starts-with"), XsltMethods.StartsWith, XmlQueryTypeFactory.BooleanX, new QilNode[] { str1, str2 });

        public QilNode InvokeSubstring(QilNode str, QilNode start) => 
            base.XsltInvokeEarlyBound(base.QName("substring"), XsltMethods.Substring2, XmlQueryTypeFactory.StringX, new QilNode[] { str, start });

        public QilNode InvokeSubstring(QilNode str, QilNode start, QilNode length) => 
            base.XsltInvokeEarlyBound(base.QName("substring"), XsltMethods.Substring3, XmlQueryTypeFactory.StringX, new QilNode[] { str, start, length });

        public QilNode InvokeSubstringAfter(QilNode str1, QilNode str2) => 
            base.XsltInvokeEarlyBound(base.QName("substring-after"), XsltMethods.SubstringAfter, XmlQueryTypeFactory.StringX, new QilNode[] { str1, str2 });

        public QilNode InvokeSubstringBefore(QilNode str1, QilNode str2) => 
            base.XsltInvokeEarlyBound(base.QName("substring-before"), XsltMethods.SubstringBefore, XmlQueryTypeFactory.StringX, new QilNode[] { str1, str2 });

        public QilNode InvokeTranslate(QilNode str1, QilNode str2, QilNode str3) => 
            base.XsltInvokeEarlyBound(base.QName("translate"), XsltMethods.Translate, XmlQueryTypeFactory.StringX, new QilNode[] { str1, str2, str3 });

        public bool IsAnyType(QilNode n)
        {
            XmlQueryType xmlType = n.XmlType;
            return (!xmlType.IsStrict && !xmlType.IsNode);
        }

        public QilNode SafeDocOrderDistinct(QilNode n)
        {
            XmlQueryType xmlType = n.XmlType;
            if (xmlType.MaybeMany)
            {
                if (xmlType.IsNode && xmlType.IsNotRtf)
                {
                    return base.DocOrderDistinct(n);
                }
                if (!xmlType.IsAtomicValue)
                {
                    QilIterator iterator;
                    return base.Loop(iterator = base.Let(n), base.Conditional(base.Gt(base.Length(iterator), base.Int32(1)), base.DocOrderDistinct(base.TypeAssert(iterator, XmlQueryTypeFactory.NodeNotRtfS)), iterator));
                }
            }
            return n;
        }
    }
}

