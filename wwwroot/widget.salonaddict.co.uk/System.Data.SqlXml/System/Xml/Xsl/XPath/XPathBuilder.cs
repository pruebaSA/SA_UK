namespace System.Xml.Xsl.XPath
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Schema;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;

    internal class XPathBuilder : IXPathBuilder<QilNode>, IXPathEnvironment, IFocus
    {
        public static readonly XmlTypeCode[] argAny;
        public static readonly XmlTypeCode[] argBoolean;
        public static readonly XmlTypeCode[] argDouble;
        public static readonly XmlTypeCode[] argFnSubstr;
        public static readonly XmlTypeCode[] argNodeSet;
        public static readonly XmlTypeCode[] argString;
        public static readonly XmlTypeCode[] argString2;
        public static readonly XmlTypeCode[] argString3;
        private IXPathEnvironment environment;
        private XPathQilFactory f;
        protected QilNode fixupCurrent;
        protected QilNode fixupLast;
        protected QilNode fixupPosition;
        private FixupVisitor fixupVisitor;
        public static Dictionary<string, FunctionInfo<FuncId>> FunctionTable;
        private bool inTheBuild;
        protected int numFixupCurrent;
        protected int numFixupLast;
        protected int numFixupPosition;
        private static XPathOperatorGroup[] OperatorGroup;
        private static QilNodeType[] QilOperator;
        private static XmlNodeKindFlags[] XPathAxisMask;
        private static XmlNodeKindFlags[] XPathNodeType2QilXmlNodeKind = new XmlNodeKindFlags[] { XmlNodeKindFlags.Document, XmlNodeKindFlags.Element, XmlNodeKindFlags.Attribute, XmlNodeKindFlags.Namespace, XmlNodeKindFlags.Text, XmlNodeKindFlags.Text, XmlNodeKindFlags.Text, XmlNodeKindFlags.PI, XmlNodeKindFlags.Comment, XmlNodeKindFlags.Any };

        static XPathBuilder()
        {
            XPathOperatorGroup[] groupArray = new XPathOperatorGroup[0x10];
            groupArray[1] = XPathOperatorGroup.Logical;
            groupArray[2] = XPathOperatorGroup.Logical;
            groupArray[3] = XPathOperatorGroup.Equality;
            groupArray[4] = XPathOperatorGroup.Equality;
            groupArray[5] = XPathOperatorGroup.Relational;
            groupArray[6] = XPathOperatorGroup.Relational;
            groupArray[7] = XPathOperatorGroup.Relational;
            groupArray[8] = XPathOperatorGroup.Relational;
            groupArray[9] = XPathOperatorGroup.Arithmetic;
            groupArray[10] = XPathOperatorGroup.Arithmetic;
            groupArray[11] = XPathOperatorGroup.Arithmetic;
            groupArray[12] = XPathOperatorGroup.Arithmetic;
            groupArray[13] = XPathOperatorGroup.Arithmetic;
            groupArray[14] = XPathOperatorGroup.Negate;
            groupArray[15] = XPathOperatorGroup.Union;
            OperatorGroup = groupArray;
            QilOperator = new QilNodeType[] { QilNodeType.Unknown, QilNodeType.Or, QilNodeType.And, QilNodeType.Eq, QilNodeType.Ne, QilNodeType.Lt, QilNodeType.Le, QilNodeType.Gt, QilNodeType.Ge, QilNodeType.Add, QilNodeType.Subtract, QilNodeType.Multiply, QilNodeType.Divide, QilNodeType.Modulo, QilNodeType.Negate, QilNodeType.Sequence };
            XmlNodeKindFlags[] flagsArray2 = new XmlNodeKindFlags[15];
            flagsArray2[1] = XmlNodeKindFlags.Element | XmlNodeKindFlags.Document;
            flagsArray2[2] = XmlNodeKindFlags.Any;
            flagsArray2[3] = XmlNodeKindFlags.Attribute;
            flagsArray2[4] = XmlNodeKindFlags.Content;
            flagsArray2[5] = XmlNodeKindFlags.Content;
            flagsArray2[6] = XmlNodeKindFlags.Any;
            flagsArray2[7] = XmlNodeKindFlags.Content;
            flagsArray2[8] = XmlNodeKindFlags.Content;
            flagsArray2[9] = XmlNodeKindFlags.Namespace;
            flagsArray2[10] = XmlNodeKindFlags.Element | XmlNodeKindFlags.Document;
            flagsArray2[11] = XmlNodeKindFlags.Content;
            flagsArray2[12] = XmlNodeKindFlags.Content;
            flagsArray2[13] = XmlNodeKindFlags.Any;
            flagsArray2[14] = XmlNodeKindFlags.Document;
            XPathAxisMask = flagsArray2;
            argAny = new XmlTypeCode[] { XmlTypeCode.Item };
            argNodeSet = new XmlTypeCode[] { XmlTypeCode.Node };
            argBoolean = new XmlTypeCode[] { XmlTypeCode.Boolean };
            argDouble = new XmlTypeCode[] { XmlTypeCode.Double };
            argString = new XmlTypeCode[] { XmlTypeCode.String };
            argString2 = new XmlTypeCode[] { XmlTypeCode.String, XmlTypeCode.String };
            argString3 = new XmlTypeCode[] { XmlTypeCode.String, XmlTypeCode.String, XmlTypeCode.String };
            argFnSubstr = new XmlTypeCode[] { XmlTypeCode.String, XmlTypeCode.Double, XmlTypeCode.Double };
            FunctionTable = CreateFunctionTable();
        }

        public XPathBuilder(IXPathEnvironment environment)
        {
            this.environment = environment;
            this.f = this.environment.Factory;
            this.fixupCurrent = this.f.Unknown(XmlQueryTypeFactory.NodeNotRtf);
            this.fixupPosition = this.f.Unknown(XmlQueryTypeFactory.DoubleX);
            this.fixupLast = this.f.Unknown(XmlQueryTypeFactory.DoubleX);
            this.fixupVisitor = new FixupVisitor(this.f, this.fixupCurrent, this.fixupPosition, this.fixupLast);
        }

        private QilNode ArithmeticOperator(XPathOperator op, QilNode left, QilNode right)
        {
            left = this.f.ConvertToNumber(left);
            right = this.f.ConvertToNumber(right);
            switch (op)
            {
                case XPathOperator.Plus:
                    return this.f.Add(left, right);

                case XPathOperator.Minus:
                    return this.f.Subtract(left, right);

                case XPathOperator.Multiply:
                    return this.f.Multiply(left, right);

                case XPathOperator.Divide:
                    return this.f.Divide(left, right);

                case XPathOperator.Modulo:
                    return this.f.Modulo(left, right);
            }
            return null;
        }

        public virtual QilNode Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name)
        {
            string nsUri = (prefix == null) ? null : this.environment.ResolvePrefix(prefix);
            return this.BuildAxis(xpathAxis, nodeType, nsUri, name);
        }

        public static XmlNodeKindFlags AxisTypeMask(XmlNodeKindFlags inputTypeMask, XPathNodeType nodeType, XPathAxis xpathAxis) => 
            ((inputTypeMask & XPathNodeType2QilXmlNodeKind[(int) nodeType]) & XPathAxisMask[(int) xpathAxis]);

        private QilNode BuildAxis(XPathAxis xpathAxis, XPathNodeType nodeType, string nsUri, string name)
        {
            QilNode node2;
            QilNode currentNode = this.GetCurrentNode();
            switch (xpathAxis)
            {
                case XPathAxis.Ancestor:
                    node2 = this.f.Ancestor(currentNode);
                    break;

                case XPathAxis.AncestorOrSelf:
                    node2 = this.f.AncestorOrSelf(currentNode);
                    break;

                case XPathAxis.Attribute:
                    node2 = this.f.Content(currentNode);
                    break;

                case XPathAxis.Child:
                    node2 = this.f.Content(currentNode);
                    break;

                case XPathAxis.Descendant:
                    node2 = this.f.Descendant(currentNode);
                    break;

                case XPathAxis.DescendantOrSelf:
                    node2 = this.f.DescendantOrSelf(currentNode);
                    break;

                case XPathAxis.Following:
                    node2 = this.f.XPathFollowing(currentNode);
                    break;

                case XPathAxis.FollowingSibling:
                    node2 = this.f.FollowingSibling(currentNode);
                    break;

                case XPathAxis.Namespace:
                    node2 = this.f.XPathNamespace(currentNode);
                    break;

                case XPathAxis.Parent:
                    node2 = this.f.Parent(currentNode);
                    break;

                case XPathAxis.Preceding:
                    node2 = this.f.XPathPreceding(currentNode);
                    break;

                case XPathAxis.PrecedingSibling:
                    node2 = this.f.PrecedingSibling(currentNode);
                    break;

                case XPathAxis.Self:
                    node2 = currentNode;
                    break;

                case XPathAxis.Root:
                    return this.f.Root(currentNode);

                default:
                    node2 = null;
                    break;
            }
            QilNode child = this.BuildAxisFilter(node2, xpathAxis, nodeType, name, nsUri);
            if (((xpathAxis != XPathAxis.Ancestor) && (xpathAxis != XPathAxis.Preceding)) && ((xpathAxis != XPathAxis.AncestorOrSelf) && (xpathAxis != XPathAxis.PrecedingSibling)))
            {
                return child;
            }
            return this.f.BaseFactory.DocOrderDistinct(child);
        }

        private QilNode BuildAxisFilter(QilNode qilAxis, XPathAxis xpathAxis, XPathNodeType nodeType, string name, string nsUri)
        {
            QilIterator iterator;
            XmlNodeKindFlags nodeKinds = qilAxis.XmlType.NodeKinds;
            XmlNodeKindFlags kinds = AxisTypeMask(nodeKinds, nodeType, xpathAxis);
            if (kinds == XmlNodeKindFlags.None)
            {
                return this.f.Sequence();
            }
            if (kinds != nodeKinds)
            {
                qilAxis = this.f.Filter(iterator = this.f.For(qilAxis), this.f.IsType(iterator, XmlQueryTypeFactory.NodeChoice(kinds)));
                qilAxis.XmlType = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeChoice(kinds), qilAxis.XmlType.Cardinality);
                if (qilAxis.NodeType == QilNodeType.Filter)
                {
                    QilLoop loop = (QilLoop) qilAxis;
                    loop.Body = this.f.And(loop.Body, ((name != null) && (nsUri != null)) ? this.f.Eq(this.f.NameOf(iterator), this.f.QName(name, nsUri)) : ((nsUri != null) ? this.f.Eq(this.f.NamespaceUriOf(iterator), this.f.String(nsUri)) : ((name != null) ? this.f.Eq(this.f.LocalNameOf(iterator), this.f.String(name)) : this.f.True())));
                    return loop;
                }
            }
            return this.f.Filter(iterator = this.f.For(qilAxis), ((name != null) && (nsUri != null)) ? this.f.Eq(this.f.NameOf(iterator), this.f.QName(name, nsUri)) : ((nsUri != null) ? this.f.Eq(this.f.NamespaceUriOf(iterator), this.f.String(nsUri)) : ((name != null) ? this.f.Eq(this.f.LocalNameOf(iterator), this.f.String(name)) : this.f.True())));
        }

        private QilNode CompareNodeSetAndNodeSet(XPathOperator op, QilNode left, QilNode right, XmlTypeCode compType)
        {
            if (right.XmlType.IsSingleton)
            {
                return this.CompareNodeSetAndValue(op, left, right, compType);
            }
            if (left.XmlType.IsSingleton)
            {
                op = InvertOp(op);
                return this.CompareNodeSetAndValue(op, right, left, compType);
            }
            QilIterator variable = this.f.For(left);
            QilIterator iterator2 = this.f.For(right);
            return this.f.Not(this.f.IsEmpty(this.f.Loop(variable, this.f.Filter(iterator2, this.CompareValues(op, this.f.XPathNodeValue(variable), this.f.XPathNodeValue(iterator2), compType)))));
        }

        private QilNode CompareNodeSetAndValue(XPathOperator op, QilNode nodeset, QilNode val, XmlTypeCode compType)
        {
            if ((compType == XmlTypeCode.Boolean) || nodeset.XmlType.IsSingleton)
            {
                return this.CompareValues(op, nodeset, val, compType);
            }
            QilIterator variable = this.f.For(nodeset);
            return this.f.Not(this.f.IsEmpty(this.f.Filter(variable, this.CompareValues(op, this.f.XPathNodeValue(variable), val, compType))));
        }

        private QilNode CompareValues(XPathOperator op, QilNode left, QilNode right, XmlTypeCode compType)
        {
            left = this.f.ConvertToType(compType, left);
            right = this.f.ConvertToType(compType, right);
            switch (op)
            {
                case XPathOperator.Eq:
                    return this.f.Eq(left, right);

                case XPathOperator.Ne:
                    return this.f.Ne(left, right);

                case XPathOperator.Lt:
                    return this.f.Lt(left, right);

                case XPathOperator.Le:
                    return this.f.Le(left, right);

                case XPathOperator.Gt:
                    return this.f.Gt(left, right);

                case XPathOperator.Ge:
                    return this.f.Ge(left, right);
            }
            return null;
        }

        private static Dictionary<string, FunctionInfo<FuncId>> CreateFunctionTable() => 
            new Dictionary<string, FunctionInfo<FuncId>>(0x24) { 
                { 
                    "last",
                    new FunctionInfo<FuncId>(FuncId.Last, 0, 0, null)
                },
                { 
                    "position",
                    new FunctionInfo<FuncId>(FuncId.Position, 0, 0, null)
                },
                { 
                    "name",
                    new FunctionInfo<FuncId>(FuncId.Name, 0, 1, argNodeSet)
                },
                { 
                    "namespace-uri",
                    new FunctionInfo<FuncId>(FuncId.NamespaceUri, 0, 1, argNodeSet)
                },
                { 
                    "local-name",
                    new FunctionInfo<FuncId>(FuncId.LocalName, 0, 1, argNodeSet)
                },
                { 
                    "count",
                    new FunctionInfo<FuncId>(FuncId.Count, 1, 1, argNodeSet)
                },
                { 
                    "id",
                    new FunctionInfo<FuncId>(FuncId.Id, 1, 1, argAny)
                },
                { 
                    "string",
                    new FunctionInfo<FuncId>(FuncId.String, 0, 1, argAny)
                },
                { 
                    "concat",
                    new FunctionInfo<FuncId>(FuncId.Concat, 2, 0x7fffffff, null)
                },
                { 
                    "starts-with",
                    new FunctionInfo<FuncId>(FuncId.StartsWith, 2, 2, argString2)
                },
                { 
                    "contains",
                    new FunctionInfo<FuncId>(FuncId.Contains, 2, 2, argString2)
                },
                { 
                    "substring-before",
                    new FunctionInfo<FuncId>(FuncId.SubstringBefore, 2, 2, argString2)
                },
                { 
                    "substring-after",
                    new FunctionInfo<FuncId>(FuncId.SubstringAfter, 2, 2, argString2)
                },
                { 
                    "substring",
                    new FunctionInfo<FuncId>(FuncId.Substring, 2, 3, argFnSubstr)
                },
                { 
                    "string-length",
                    new FunctionInfo<FuncId>(FuncId.StringLength, 0, 1, argString)
                },
                { 
                    "normalize-space",
                    new FunctionInfo<FuncId>(FuncId.Normalize, 0, 1, argString)
                },
                { 
                    "translate",
                    new FunctionInfo<FuncId>(FuncId.Translate, 3, 3, argString3)
                },
                { 
                    "boolean",
                    new FunctionInfo<FuncId>(FuncId.Boolean, 1, 1, argAny)
                },
                { 
                    "not",
                    new FunctionInfo<FuncId>(FuncId.Not, 1, 1, argBoolean)
                },
                { 
                    "true",
                    new FunctionInfo<FuncId>(FuncId.True, 0, 0, null)
                },
                { 
                    "false",
                    new FunctionInfo<FuncId>(FuncId.False, 0, 0, null)
                },
                { 
                    "lang",
                    new FunctionInfo<FuncId>(FuncId.Lang, 1, 1, argString)
                },
                { 
                    "number",
                    new FunctionInfo<FuncId>(FuncId.Number, 0, 1, argAny)
                },
                { 
                    "sum",
                    new FunctionInfo<FuncId>(FuncId.Sum, 1, 1, argNodeSet)
                },
                { 
                    "floor",
                    new FunctionInfo<FuncId>(FuncId.Floor, 1, 1, argDouble)
                },
                { 
                    "ceiling",
                    new FunctionInfo<FuncId>(FuncId.Ceiling, 1, 1, argDouble)
                },
                { 
                    "round",
                    new FunctionInfo<FuncId>(FuncId.Round, 1, 1, argDouble)
                }
            };

        public virtual QilNode EndBuild(QilNode result)
        {
            if (result == null)
            {
                this.inTheBuild = false;
                return result;
            }
            if ((result.XmlType.MaybeMany && result.XmlType.IsNode) && result.XmlType.IsNotRtf)
            {
                result = this.f.DocOrderDistinct(result);
            }
            result = this.fixupVisitor.Fixup(result, this.environment);
            this.numFixupCurrent -= this.fixupVisitor.numCurrent;
            this.numFixupPosition -= this.fixupVisitor.numPosition;
            this.numFixupLast -= this.fixupVisitor.numLast;
            this.inTheBuild = false;
            return result;
        }

        private QilNode EqualityOperator(XPathOperator op, QilNode left, QilNode right)
        {
            XmlQueryType xmlType = left.XmlType;
            XmlQueryType type2 = right.XmlType;
            if (this.f.IsAnyType(left) || this.f.IsAnyType(right))
            {
                return this.f.InvokeEqualityOperator(QilOperator[(int) op], left, right);
            }
            if (xmlType.IsNode && type2.IsNode)
            {
                return this.CompareNodeSetAndNodeSet(op, left, right, XmlTypeCode.String);
            }
            if (xmlType.IsNode)
            {
                return this.CompareNodeSetAndValue(op, left, right, type2.TypeCode);
            }
            if (type2.IsNode)
            {
                return this.CompareNodeSetAndValue(op, right, left, xmlType.TypeCode);
            }
            XmlTypeCode compType = ((xmlType.TypeCode == XmlTypeCode.Boolean) || (type2.TypeCode == XmlTypeCode.Boolean)) ? XmlTypeCode.Boolean : (((xmlType.TypeCode == XmlTypeCode.Double) || (type2.TypeCode == XmlTypeCode.Double)) ? XmlTypeCode.Double : XmlTypeCode.String);
            return this.CompareValues(op, left, right, compType);
        }

        public virtual QilNode Function(string prefix, string name, IList<QilNode> args)
        {
            FunctionInfo<FuncId> info;
            if ((prefix.Length != 0) || !FunctionTable.TryGetValue(name, out info))
            {
                return this.environment.ResolveFunction(prefix, name, args, this);
            }
            info.CastArguments(args, name, this.f);
            switch (info.id)
            {
                case FuncId.Last:
                    return this.GetLastPosition();

                case FuncId.Position:
                    return this.GetCurrentPosition();

                case FuncId.Count:
                    return this.f.XsltConvert(this.f.Length(this.f.DocOrderDistinct(args[0])), XmlQueryTypeFactory.DoubleX);

                case FuncId.LocalName:
                    if (args.Count == 0)
                    {
                        return this.f.LocalNameOf(this.GetCurrentNode());
                    }
                    return this.LocalNameOfFirstNode(args[0]);

                case FuncId.NamespaceUri:
                    if (args.Count == 0)
                    {
                        return this.f.NamespaceUriOf(this.GetCurrentNode());
                    }
                    return this.NamespaceOfFirstNode(args[0]);

                case FuncId.Name:
                    if (args.Count == 0)
                    {
                        return this.NameOf(this.GetCurrentNode());
                    }
                    return this.NameOfFirstNode(args[0]);

                case FuncId.String:
                    if (args.Count == 0)
                    {
                        return this.f.XPathNodeValue(this.GetCurrentNode());
                    }
                    return this.f.ConvertToString(args[0]);

                case FuncId.Number:
                    if (args.Count == 0)
                    {
                        return this.f.XsltConvert(this.f.XPathNodeValue(this.GetCurrentNode()), XmlQueryTypeFactory.DoubleX);
                    }
                    return this.f.ConvertToNumber(args[0]);

                case FuncId.Boolean:
                    return this.f.ConvertToBoolean(args[0]);

                case FuncId.True:
                    return this.f.True();

                case FuncId.False:
                    return this.f.False();

                case FuncId.Not:
                    return this.f.Not(args[0]);

                case FuncId.Id:
                    return this.f.DocOrderDistinct(this.f.Id(this.GetCurrentNode(), args[0]));

                case FuncId.Concat:
                    return this.f.StrConcat(args);

                case FuncId.StartsWith:
                    return this.f.InvokeStartsWith(args[0], args[1]);

                case FuncId.Contains:
                    return this.f.InvokeContains(args[0], args[1]);

                case FuncId.SubstringBefore:
                    return this.f.InvokeSubstringBefore(args[0], args[1]);

                case FuncId.SubstringAfter:
                    return this.f.InvokeSubstringAfter(args[0], args[1]);

                case FuncId.Substring:
                    if (args.Count == 2)
                    {
                        return this.f.InvokeSubstring(args[0], args[1]);
                    }
                    return this.f.InvokeSubstring(args[0], args[1], args[2]);

                case FuncId.StringLength:
                    return this.f.XsltConvert(this.f.StrLength((args.Count == 0) ? this.f.XPathNodeValue(this.GetCurrentNode()) : args[0]), XmlQueryTypeFactory.DoubleX);

                case FuncId.Normalize:
                    return this.f.InvokeNormalizeSpace((args.Count == 0) ? this.f.XPathNodeValue(this.GetCurrentNode()) : args[0]);

                case FuncId.Translate:
                    return this.f.InvokeTranslate(args[0], args[1], args[2]);

                case FuncId.Lang:
                    return this.f.InvokeLang(args[0], this.GetCurrentNode());

                case FuncId.Sum:
                    return this.Sum(this.f.DocOrderDistinct(args[0]));

                case FuncId.Floor:
                    return this.f.InvokeFloor(args[0]);

                case FuncId.Ceiling:
                    return this.f.InvokeCeiling(args[0]);

                case FuncId.Round:
                    return this.f.InvokeRound(args[0]);
            }
            return null;
        }

        private QilNode GetCurrentNode()
        {
            this.numFixupCurrent++;
            return this.fixupCurrent;
        }

        private QilNode GetCurrentPosition()
        {
            this.numFixupPosition++;
            return this.fixupPosition;
        }

        private QilNode GetLastPosition()
        {
            this.numFixupLast++;
            return this.fixupLast;
        }

        private static XPathOperator InvertOp(XPathOperator op)
        {
            if (op == XPathOperator.Lt)
            {
                return XPathOperator.Gt;
            }
            if (op == XPathOperator.Le)
            {
                return XPathOperator.Ge;
            }
            if (op == XPathOperator.Gt)
            {
                return XPathOperator.Lt;
            }
            if (op != XPathOperator.Ge)
            {
                return op;
            }
            return XPathOperator.Le;
        }

        public static bool IsFunctionAvailable(string localName, string nsUri)
        {
            if (nsUri.Length != 0)
            {
                return false;
            }
            return FunctionTable.ContainsKey(localName);
        }

        public virtual QilNode JoinStep(QilNode left, QilNode right)
        {
            QilIterator current = this.f.For(this.f.EnsureNodeSet(left));
            right = this.fixupVisitor.Fixup(right, current, null);
            this.numFixupCurrent -= this.fixupVisitor.numCurrent;
            this.numFixupPosition -= this.fixupVisitor.numPosition;
            this.numFixupLast -= this.fixupVisitor.numLast;
            return this.f.DocOrderDistinct(this.f.Loop(current, right));
        }

        private QilNode LocalNameOfFirstNode(QilNode arg)
        {
            QilIterator iterator;
            if (arg.XmlType.IsSingleton)
            {
                return this.f.LocalNameOf(arg);
            }
            return this.f.StrConcat(this.f.Loop(iterator = this.f.FirstNode(arg), this.f.LocalNameOf(iterator)));
        }

        private QilNode LogicalOperator(XPathOperator op, QilNode left, QilNode right)
        {
            left = this.f.ConvertToBoolean(left);
            right = this.f.ConvertToBoolean(right);
            if (op != XPathOperator.Or)
            {
                return this.f.And(left, right);
            }
            return this.f.Or(left, right);
        }

        private QilNode NameOf(QilNode arg)
        {
            if (arg is QilIterator)
            {
                QilIterator iterator;
                QilIterator iterator2;
                return this.f.Loop(iterator = this.f.Let(this.f.PrefixOf(arg)), this.f.Loop(iterator2 = this.f.Let(this.f.LocalNameOf(arg)), this.f.Conditional(this.f.Eq(this.f.StrLength(iterator), this.f.Int32(0)), iterator2, this.f.StrConcat(new QilNode[] { iterator, this.f.String(":"), iterator2 }))));
            }
            QilIterator variable = this.f.Let(arg);
            return this.f.Loop(variable, this.NameOf(variable));
        }

        private QilNode NameOfFirstNode(QilNode arg)
        {
            QilIterator iterator;
            if (arg.XmlType.IsSingleton)
            {
                return this.NameOf(arg);
            }
            return this.f.StrConcat(this.f.Loop(iterator = this.f.FirstNode(arg), this.NameOf(iterator)));
        }

        private QilNode NamespaceOfFirstNode(QilNode arg)
        {
            QilIterator iterator;
            if (arg.XmlType.IsSingleton)
            {
                return this.f.NamespaceUriOf(arg);
            }
            return this.f.StrConcat(this.f.Loop(iterator = this.f.FirstNode(arg), this.f.NamespaceUriOf(iterator)));
        }

        private QilNode NegateOperator(XPathOperator op, QilNode left, QilNode right) => 
            this.f.Negate(this.f.ConvertToNumber(left));

        public virtual QilNode Number(double value) => 
            this.f.Double(value);

        public virtual QilNode Operator(XPathOperator op, QilNode left, QilNode right)
        {
            switch (OperatorGroup[(int) op])
            {
                case XPathOperatorGroup.Logical:
                    return this.LogicalOperator(op, left, right);

                case XPathOperatorGroup.Equality:
                    return this.EqualityOperator(op, left, right);

                case XPathOperatorGroup.Relational:
                    return this.RelationalOperator(op, left, right);

                case XPathOperatorGroup.Arithmetic:
                    return this.ArithmeticOperator(op, left, right);

                case XPathOperatorGroup.Negate:
                    return this.NegateOperator(op, left, right);

                case XPathOperatorGroup.Union:
                    return this.UnionOperator(op, left, right);
            }
            return null;
        }

        public virtual QilNode Predicate(QilNode nodeset, QilNode predicate, bool isReverseStep)
        {
            QilNode node;
            if (isReverseStep)
            {
                nodeset = ((QilUnary) nodeset).Child;
            }
            nodeset = this.f.EnsureNodeSet(nodeset);
            if (!this.f.IsAnyType(predicate))
            {
                if (predicate.XmlType.TypeCode == XmlTypeCode.Double)
                {
                    predicate = this.f.Eq(this.GetCurrentPosition(), predicate);
                }
                else
                {
                    predicate = this.f.ConvertToBoolean(predicate);
                }
            }
            else
            {
                QilIterator iterator;
                predicate = this.f.Loop(iterator = this.f.Let(predicate), this.f.Conditional(this.f.IsType(iterator, XmlQueryTypeFactory.Double), this.f.Eq(this.GetCurrentPosition(), this.f.TypeAssert(iterator, XmlQueryTypeFactory.DoubleX)), this.f.ConvertToBoolean(iterator)));
            }
            if ((this.numFixupLast != 0) && (this.fixupVisitor.CountUnfixedLast(predicate) != 0))
            {
                QilIterator child = this.f.Let(nodeset);
                QilIterator last = this.f.Let(this.f.XsltConvert(this.f.Length(child), XmlQueryTypeFactory.DoubleX));
                QilIterator current = this.f.For(child);
                predicate = this.fixupVisitor.Fixup(predicate, current, last);
                this.numFixupCurrent -= this.fixupVisitor.numCurrent;
                this.numFixupPosition -= this.fixupVisitor.numPosition;
                this.numFixupLast -= this.fixupVisitor.numLast;
                node = this.f.Loop(child, this.f.Loop(last, this.f.Filter(current, predicate)));
            }
            else
            {
                QilIterator iterator5 = this.f.For(nodeset);
                predicate = this.fixupVisitor.Fixup(predicate, iterator5, null);
                this.numFixupCurrent -= this.fixupVisitor.numCurrent;
                this.numFixupPosition -= this.fixupVisitor.numPosition;
                this.numFixupLast -= this.fixupVisitor.numLast;
                node = this.f.Filter(iterator5, predicate);
            }
            if (isReverseStep)
            {
                node = this.f.DocOrderDistinct(node);
            }
            return node;
        }

        private QilNode RelationalOperator(XPathOperator op, QilNode left, QilNode right)
        {
            XmlQueryType xmlType = left.XmlType;
            XmlQueryType type2 = right.XmlType;
            if (this.f.IsAnyType(left) || this.f.IsAnyType(right))
            {
                return this.f.InvokeRelationalOperator(QilOperator[(int) op], left, right);
            }
            if (xmlType.IsNode && type2.IsNode)
            {
                return this.CompareNodeSetAndNodeSet(op, left, right, XmlTypeCode.Double);
            }
            if (xmlType.IsNode)
            {
                XmlTypeCode compType = (type2.TypeCode == XmlTypeCode.Boolean) ? XmlTypeCode.Boolean : XmlTypeCode.Double;
                return this.CompareNodeSetAndValue(op, left, right, compType);
            }
            if (type2.IsNode)
            {
                XmlTypeCode code2 = (xmlType.TypeCode == XmlTypeCode.Boolean) ? XmlTypeCode.Boolean : XmlTypeCode.Double;
                op = InvertOp(op);
                return this.CompareNodeSetAndValue(op, right, left, code2);
            }
            return this.CompareValues(op, left, right, XmlTypeCode.Double);
        }

        public virtual void StartBuild()
        {
            this.inTheBuild = true;
            this.numFixupCurrent = this.numFixupPosition = this.numFixupLast = 0;
        }

        public virtual QilNode String(string value) => 
            this.f.String(value);

        private QilNode Sum(QilNode arg)
        {
            QilIterator iterator;
            return this.f.Sum(this.f.Sequence(this.f.Double(0.0), this.f.Loop(iterator = this.f.For(arg), this.f.ConvertToNumber(iterator))));
        }

        QilNode IFocus.GetCurrent() => 
            this.GetCurrentNode();

        QilNode IFocus.GetLast() => 
            this.GetLastPosition();

        QilNode IFocus.GetPosition() => 
            this.GetCurrentPosition();

        QilNode IXPathEnvironment.ResolveFunction(string prefix, string name, IList<QilNode> args, IFocus env) => 
            null;

        string IXPathEnvironment.ResolvePrefix(string prefix) => 
            this.environment.ResolvePrefix(prefix);

        QilNode IXPathEnvironment.ResolveVariable(string prefix, string name) => 
            this.Variable(prefix, name);

        private QilNode UnionOperator(XPathOperator op, QilNode left, QilNode right)
        {
            if (left == null)
            {
                return this.f.EnsureNodeSet(right);
            }
            left = this.f.EnsureNodeSet(left);
            right = this.f.EnsureNodeSet(right);
            if (left.NodeType == QilNodeType.Sequence)
            {
                ((QilList) left).Add(right);
                return left;
            }
            return this.f.Union(left, right);
        }

        public virtual QilNode Variable(string prefix, string name) => 
            this.environment.ResolveVariable(prefix, name);

        XPathQilFactory IXPathEnvironment.Factory =>
            this.f;

        private class FixupVisitor : QilReplaceVisitor
        {
            private QilIterator current;
            private IXPathEnvironment environment;
            private QilPatternFactory f;
            private QilNode fixupCurrent;
            private QilNode fixupLast;
            private QilNode fixupPosition;
            private bool justCount;
            private QilNode last;
            public int numCurrent;
            public int numLast;
            public int numPosition;

            public FixupVisitor(QilPatternFactory f, QilNode fixupCurrent, QilNode fixupPosition, QilNode fixupLast) : base(f.BaseFactory)
            {
                this.f = f;
                this.fixupCurrent = fixupCurrent;
                this.fixupPosition = fixupPosition;
                this.fixupLast = fixupLast;
            }

            public int CountUnfixedLast(QilNode inExpr)
            {
                this.justCount = true;
                this.numCurrent = this.numPosition = this.numLast = 0;
                this.VisitAssumeReference(inExpr);
                return this.numLast;
            }

            public QilNode Fixup(QilNode inExpr, IXPathEnvironment environment)
            {
                QilDepthChecker.Check(inExpr);
                this.justCount = false;
                this.current = null;
                this.environment = environment;
                this.numCurrent = this.numPosition = this.numLast = 0;
                inExpr = this.VisitAssumeReference(inExpr);
                return inExpr;
            }

            public QilNode Fixup(QilNode inExpr, QilIterator current, QilNode last)
            {
                QilDepthChecker.Check(inExpr);
                this.current = current;
                this.last = last;
                this.justCount = false;
                this.environment = null;
                this.numCurrent = this.numPosition = this.numLast = 0;
                inExpr = this.VisitAssumeReference(inExpr);
                return inExpr;
            }

            protected override QilNode VisitUnknown(QilNode unknown)
            {
                if (unknown == this.fixupCurrent)
                {
                    this.numCurrent++;
                    if (!this.justCount)
                    {
                        if (this.environment != null)
                        {
                            unknown = this.environment.GetCurrent();
                            return unknown;
                        }
                        if (this.current != null)
                        {
                            unknown = this.current;
                        }
                    }
                    return unknown;
                }
                if (unknown == this.fixupPosition)
                {
                    this.numPosition++;
                    if (!this.justCount)
                    {
                        if (this.environment != null)
                        {
                            unknown = this.environment.GetPosition();
                            return unknown;
                        }
                        if (this.current != null)
                        {
                            unknown = this.f.XsltConvert(this.f.PositionOf(this.current), XmlQueryTypeFactory.DoubleX);
                        }
                    }
                    return unknown;
                }
                if (unknown == this.fixupLast)
                {
                    this.numLast++;
                    if (this.justCount)
                    {
                        return unknown;
                    }
                    if (this.environment != null)
                    {
                        unknown = this.environment.GetLast();
                        return unknown;
                    }
                    if (this.current != null)
                    {
                        unknown = this.last;
                    }
                }
                return unknown;
            }
        }

        internal enum FuncId
        {
            Last,
            Position,
            Count,
            LocalName,
            NamespaceUri,
            Name,
            String,
            Number,
            Boolean,
            True,
            False,
            Not,
            Id,
            Concat,
            StartsWith,
            Contains,
            SubstringBefore,
            SubstringAfter,
            Substring,
            StringLength,
            Normalize,
            Translate,
            Lang,
            Sum,
            Floor,
            Ceiling,
            Round
        }

        internal class FunctionInfo<T>
        {
            public XmlTypeCode[] argTypes;
            public T id;
            public const int Infinity = 0x7fffffff;
            public int maxArgs;
            public int minArgs;

            public FunctionInfo(T id, int minArgs, int maxArgs, XmlTypeCode[] argTypes)
            {
                this.id = id;
                this.minArgs = minArgs;
                this.maxArgs = maxArgs;
                this.argTypes = argTypes;
            }

            public void CastArguments(IList<QilNode> args, string name, XPathQilFactory f)
            {
                XPathBuilder.FunctionInfo<T>.CheckArity(this.minArgs, this.maxArgs, name, args.Count);
                if (this.maxArgs == 0x7fffffff)
                {
                    for (int i = 0; i < args.Count; i++)
                    {
                        args[i] = f.ConvertToType(XmlTypeCode.String, args[i]);
                    }
                }
                else
                {
                    for (int j = 0; j < args.Count; j++)
                    {
                        if ((this.argTypes[j] == XmlTypeCode.Node) && f.CannotBeNodeSet(args[j]))
                        {
                            string[] strArray = new string[] { name, (j + 1).ToString(CultureInfo.InvariantCulture) };
                            throw new XPathCompileException("XPath_NodeSetArgumentExpected", strArray);
                        }
                        args[j] = f.ConvertToType(this.argTypes[j], args[j]);
                    }
                }
            }

            public static void CheckArity(int minArgs, int maxArgs, string name, int numArgs)
            {
                if ((minArgs > numArgs) || (numArgs > maxArgs))
                {
                    string str;
                    if (minArgs == maxArgs)
                    {
                        str = "XPath_NArgsExpected";
                    }
                    else if (maxArgs == (minArgs + 1))
                    {
                        str = "XPath_NOrMArgsExpected";
                    }
                    else if (numArgs < minArgs)
                    {
                        str = "XPath_AtLeastNArgsExpected";
                    }
                    else
                    {
                        str = "XPath_AtMostMArgsExpected";
                    }
                    throw new XPathCompileException(str, new string[] { name, minArgs.ToString(CultureInfo.InvariantCulture), maxArgs.ToString(CultureInfo.InvariantCulture) });
                }
            }
        }

        private enum XPathOperatorGroup
        {
            Unknown,
            Logical,
            Equality,
            Relational,
            Arithmetic,
            Negate,
            Union
        }
    }
}

