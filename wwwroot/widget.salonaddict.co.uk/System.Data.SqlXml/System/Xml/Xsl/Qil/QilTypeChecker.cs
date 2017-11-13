namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml.Schema;
    using System.Xml.Xsl;

    internal class QilTypeChecker
    {
        public XmlQueryType Check(QilNode n)
        {
            switch (n.NodeType)
            {
                case QilNodeType.QilExpression:
                    return this.CheckQilExpression((QilExpression) n);

                case QilNodeType.FunctionList:
                    return this.CheckFunctionList((QilList) n);

                case QilNodeType.GlobalVariableList:
                    return this.CheckGlobalVariableList((QilList) n);

                case QilNodeType.GlobalParameterList:
                    return this.CheckGlobalParameterList((QilList) n);

                case QilNodeType.ActualParameterList:
                    return this.CheckActualParameterList((QilList) n);

                case QilNodeType.FormalParameterList:
                    return this.CheckFormalParameterList((QilList) n);

                case QilNodeType.SortKeyList:
                    return this.CheckSortKeyList((QilList) n);

                case QilNodeType.BranchList:
                    return this.CheckBranchList((QilList) n);

                case QilNodeType.OptimizeBarrier:
                    return this.CheckOptimizeBarrier((QilUnary) n);

                case QilNodeType.Unknown:
                    return this.CheckUnknown(n);

                case QilNodeType.DataSource:
                    return this.CheckDataSource((QilDataSource) n);

                case QilNodeType.Nop:
                    return this.CheckNop((QilUnary) n);

                case QilNodeType.Error:
                    return this.CheckError((QilUnary) n);

                case QilNodeType.Warning:
                    return this.CheckWarning((QilUnary) n);

                case QilNodeType.For:
                    return this.CheckFor((QilIterator) n);

                case QilNodeType.Let:
                    return this.CheckLet((QilIterator) n);

                case QilNodeType.Parameter:
                    return this.CheckParameter((QilParameter) n);

                case QilNodeType.PositionOf:
                    return this.CheckPositionOf((QilUnary) n);

                case QilNodeType.True:
                    return this.CheckTrue(n);

                case QilNodeType.False:
                    return this.CheckFalse(n);

                case QilNodeType.LiteralString:
                    return this.CheckLiteralString((QilLiteral) n);

                case QilNodeType.LiteralInt32:
                    return this.CheckLiteralInt32((QilLiteral) n);

                case QilNodeType.LiteralInt64:
                    return this.CheckLiteralInt64((QilLiteral) n);

                case QilNodeType.LiteralDouble:
                    return this.CheckLiteralDouble((QilLiteral) n);

                case QilNodeType.LiteralDecimal:
                    return this.CheckLiteralDecimal((QilLiteral) n);

                case QilNodeType.LiteralQName:
                    return this.CheckLiteralQName((QilName) n);

                case QilNodeType.LiteralType:
                    return this.CheckLiteralType((QilLiteral) n);

                case QilNodeType.LiteralObject:
                    return this.CheckLiteralObject((QilLiteral) n);

                case QilNodeType.And:
                    return this.CheckAnd((QilBinary) n);

                case QilNodeType.Or:
                    return this.CheckOr((QilBinary) n);

                case QilNodeType.Not:
                    return this.CheckNot((QilUnary) n);

                case QilNodeType.Conditional:
                    return this.CheckConditional((QilTernary) n);

                case QilNodeType.Choice:
                    return this.CheckChoice((QilChoice) n);

                case QilNodeType.Length:
                    return this.CheckLength((QilUnary) n);

                case QilNodeType.Sequence:
                    return this.CheckSequence((QilList) n);

                case QilNodeType.Union:
                    return this.CheckUnion((QilBinary) n);

                case QilNodeType.Intersection:
                    return this.CheckIntersection((QilBinary) n);

                case QilNodeType.Difference:
                    return this.CheckDifference((QilBinary) n);

                case QilNodeType.Average:
                    return this.CheckAverage((QilUnary) n);

                case QilNodeType.Sum:
                    return this.CheckSum((QilUnary) n);

                case QilNodeType.Minimum:
                    return this.CheckMinimum((QilUnary) n);

                case QilNodeType.Maximum:
                    return this.CheckMaximum((QilUnary) n);

                case QilNodeType.Negate:
                    return this.CheckNegate((QilUnary) n);

                case QilNodeType.Add:
                    return this.CheckAdd((QilBinary) n);

                case QilNodeType.Subtract:
                    return this.CheckSubtract((QilBinary) n);

                case QilNodeType.Multiply:
                    return this.CheckMultiply((QilBinary) n);

                case QilNodeType.Divide:
                    return this.CheckDivide((QilBinary) n);

                case QilNodeType.Modulo:
                    return this.CheckModulo((QilBinary) n);

                case QilNodeType.StrLength:
                    return this.CheckStrLength((QilUnary) n);

                case QilNodeType.StrConcat:
                    return this.CheckStrConcat((QilStrConcat) n);

                case QilNodeType.StrParseQName:
                    return this.CheckStrParseQName((QilBinary) n);

                case QilNodeType.Ne:
                    return this.CheckNe((QilBinary) n);

                case QilNodeType.Eq:
                    return this.CheckEq((QilBinary) n);

                case QilNodeType.Gt:
                    return this.CheckGt((QilBinary) n);

                case QilNodeType.Ge:
                    return this.CheckGe((QilBinary) n);

                case QilNodeType.Lt:
                    return this.CheckLt((QilBinary) n);

                case QilNodeType.Le:
                    return this.CheckLe((QilBinary) n);

                case QilNodeType.Is:
                    return this.CheckIs((QilBinary) n);

                case QilNodeType.After:
                    return this.CheckAfter((QilBinary) n);

                case QilNodeType.Before:
                    return this.CheckBefore((QilBinary) n);

                case QilNodeType.Loop:
                    return this.CheckLoop((QilLoop) n);

                case QilNodeType.Filter:
                    return this.CheckFilter((QilLoop) n);

                case QilNodeType.Sort:
                    return this.CheckSort((QilLoop) n);

                case QilNodeType.SortKey:
                    return this.CheckSortKey((QilSortKey) n);

                case QilNodeType.DocOrderDistinct:
                    return this.CheckDocOrderDistinct((QilUnary) n);

                case QilNodeType.Function:
                    return this.CheckFunction((QilFunction) n);

                case QilNodeType.Invoke:
                    return this.CheckInvoke((QilInvoke) n);

                case QilNodeType.Content:
                    return this.CheckContent((QilUnary) n);

                case QilNodeType.Attribute:
                    return this.CheckAttribute((QilBinary) n);

                case QilNodeType.Parent:
                    return this.CheckParent((QilUnary) n);

                case QilNodeType.Root:
                    return this.CheckRoot((QilUnary) n);

                case QilNodeType.XmlContext:
                    return this.CheckXmlContext(n);

                case QilNodeType.Descendant:
                    return this.CheckDescendant((QilUnary) n);

                case QilNodeType.DescendantOrSelf:
                    return this.CheckDescendantOrSelf((QilUnary) n);

                case QilNodeType.Ancestor:
                    return this.CheckAncestor((QilUnary) n);

                case QilNodeType.AncestorOrSelf:
                    return this.CheckAncestorOrSelf((QilUnary) n);

                case QilNodeType.Preceding:
                    return this.CheckPreceding((QilUnary) n);

                case QilNodeType.FollowingSibling:
                    return this.CheckFollowingSibling((QilUnary) n);

                case QilNodeType.PrecedingSibling:
                    return this.CheckPrecedingSibling((QilUnary) n);

                case QilNodeType.NodeRange:
                    return this.CheckNodeRange((QilBinary) n);

                case QilNodeType.Deref:
                    return this.CheckDeref((QilBinary) n);

                case QilNodeType.ElementCtor:
                    return this.CheckElementCtor((QilBinary) n);

                case QilNodeType.AttributeCtor:
                    return this.CheckAttributeCtor((QilBinary) n);

                case QilNodeType.CommentCtor:
                    return this.CheckCommentCtor((QilUnary) n);

                case QilNodeType.PICtor:
                    return this.CheckPICtor((QilBinary) n);

                case QilNodeType.TextCtor:
                    return this.CheckTextCtor((QilUnary) n);

                case QilNodeType.RawTextCtor:
                    return this.CheckRawTextCtor((QilUnary) n);

                case QilNodeType.DocumentCtor:
                    return this.CheckDocumentCtor((QilUnary) n);

                case QilNodeType.NamespaceDecl:
                    return this.CheckNamespaceDecl((QilBinary) n);

                case QilNodeType.RtfCtor:
                    return this.CheckRtfCtor((QilBinary) n);

                case QilNodeType.NameOf:
                    return this.CheckNameOf((QilUnary) n);

                case QilNodeType.LocalNameOf:
                    return this.CheckLocalNameOf((QilUnary) n);

                case QilNodeType.NamespaceUriOf:
                    return this.CheckNamespaceUriOf((QilUnary) n);

                case QilNodeType.PrefixOf:
                    return this.CheckPrefixOf((QilUnary) n);

                case QilNodeType.TypeAssert:
                    return this.CheckTypeAssert((QilTargetType) n);

                case QilNodeType.IsType:
                    return this.CheckIsType((QilTargetType) n);

                case QilNodeType.IsEmpty:
                    return this.CheckIsEmpty((QilUnary) n);

                case QilNodeType.XPathNodeValue:
                    return this.CheckXPathNodeValue((QilUnary) n);

                case QilNodeType.XPathFollowing:
                    return this.CheckXPathFollowing((QilUnary) n);

                case QilNodeType.XPathPreceding:
                    return this.CheckXPathPreceding((QilUnary) n);

                case QilNodeType.XPathNamespace:
                    return this.CheckXPathNamespace((QilUnary) n);

                case QilNodeType.XsltGenerateId:
                    return this.CheckXsltGenerateId((QilUnary) n);

                case QilNodeType.XsltInvokeLateBound:
                    return this.CheckXsltInvokeLateBound((QilInvokeLateBound) n);

                case QilNodeType.XsltInvokeEarlyBound:
                    return this.CheckXsltInvokeEarlyBound((QilInvokeEarlyBound) n);

                case QilNodeType.XsltCopy:
                    return this.CheckXsltCopy((QilBinary) n);

                case QilNodeType.XsltCopyOf:
                    return this.CheckXsltCopyOf((QilUnary) n);

                case QilNodeType.XsltConvert:
                    return this.CheckXsltConvert((QilTargetType) n);
            }
            return this.CheckUnknown(n);
        }

        [Conditional("DEBUG")]
        private void Check(bool value, QilNode node, string message)
        {
        }

        public XmlQueryType CheckActualParameterList(QilList node) => 
            node.XmlType;

        public XmlQueryType CheckAdd(QilBinary node)
        {
            if (node.Left.XmlType.TypeCode != XmlTypeCode.None)
            {
                return node.Left.XmlType;
            }
            return node.Right.XmlType;
        }

        public XmlQueryType CheckAfter(QilBinary node) => 
            this.CheckIs(node);

        public XmlQueryType CheckAncestor(QilUnary node) => 
            XmlQueryTypeFactory.DocumentOrElementS;

        public XmlQueryType CheckAncestorOrSelf(QilUnary node) => 
            XmlQueryTypeFactory.Choice(node.Child.XmlType, XmlQueryTypeFactory.DocumentOrElementS);

        public XmlQueryType CheckAnd(QilBinary node) => 
            XmlQueryTypeFactory.BooleanX;

        [Conditional("DEBUG")]
        private void CheckAtomicX(QilNode node)
        {
        }

        public XmlQueryType CheckAttribute(QilBinary node) => 
            XmlQueryTypeFactory.AttributeQ;

        public XmlQueryType CheckAttributeCtor(QilBinary node) => 
            XmlQueryTypeFactory.UntypedAttribute;

        public XmlQueryType CheckAverage(QilUnary node)
        {
            XmlQueryType xmlType = node.Child.XmlType;
            return XmlQueryTypeFactory.PrimeProduct(xmlType, xmlType.MaybeEmpty ? XmlQueryCardinality.ZeroOrOne : XmlQueryCardinality.One);
        }

        public XmlQueryType CheckBefore(QilBinary node) => 
            this.CheckIs(node);

        public XmlQueryType CheckBranchList(QilList node) => 
            node.XmlType;

        public XmlQueryType CheckChoice(QilChoice node) => 
            node.Branches.XmlType;

        [Conditional("DEBUG")]
        private void CheckClass(QilNode node, Type clrTypeClass)
        {
        }

        [Conditional("DEBUG")]
        private void CheckClassAndNodeType(QilNode node, Type clrTypeClass, QilNodeType nodeType)
        {
        }

        public XmlQueryType CheckCommentCtor(QilUnary node) => 
            XmlQueryTypeFactory.Comment;

        public XmlQueryType CheckConditional(QilTernary node) => 
            XmlQueryTypeFactory.Choice(node.Center.XmlType, node.Right.XmlType);

        public XmlQueryType CheckContent(QilUnary node) => 
            XmlQueryTypeFactory.AttributeOrContentS;

        public XmlQueryType CheckDataSource(QilDataSource node) => 
            XmlQueryTypeFactory.NodeNotRtfQ;

        public XmlQueryType CheckDeepCopy(QilUnary node) => 
            node.XmlType;

        public XmlQueryType CheckDeref(QilBinary node) => 
            XmlQueryTypeFactory.ElementS;

        public XmlQueryType CheckDescendant(QilUnary node) => 
            XmlQueryTypeFactory.ContentS;

        public XmlQueryType CheckDescendantOrSelf(QilUnary node) => 
            XmlQueryTypeFactory.Choice(node.Child.XmlType, XmlQueryTypeFactory.ContentS);

        public XmlQueryType CheckDifference(QilBinary node) => 
            XmlQueryTypeFactory.AtMost(node.Left.XmlType, node.Left.XmlType.Cardinality);

        public XmlQueryType CheckDivide(QilBinary node) => 
            this.CheckAdd(node);

        public XmlQueryType CheckDocOrderDistinct(QilUnary node) => 
            this.DistinctType(node.Child.XmlType);

        public XmlQueryType CheckDocumentCtor(QilUnary node) => 
            XmlQueryTypeFactory.UntypedDocument;

        public XmlQueryType CheckElementCtor(QilBinary node) => 
            XmlQueryTypeFactory.UntypedElement;

        public XmlQueryType CheckEq(QilBinary node) => 
            this.CheckNe(node);

        public XmlQueryType CheckError(QilUnary node) => 
            XmlQueryTypeFactory.None;

        public XmlQueryType CheckFalse(QilNode node) => 
            XmlQueryTypeFactory.BooleanX;

        public XmlQueryType CheckFilter(QilLoop node)
        {
            XmlQueryType type = this.FindFilterType(node.Variable, node.Body);
            if (type != null)
            {
                return type;
            }
            return XmlQueryTypeFactory.AtMost(node.Variable.Binding.XmlType, node.Variable.Binding.XmlType.Cardinality);
        }

        public XmlQueryType CheckFollowingSibling(QilUnary node) => 
            XmlQueryTypeFactory.ContentS;

        public XmlQueryType CheckFor(QilIterator node) => 
            node.Binding.XmlType.Prime;

        public XmlQueryType CheckFormalParameterList(QilList node)
        {
            using (IEnumerator<QilNode> enumerator = node.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    QilNode current = enumerator.Current;
                }
            }
            return node.XmlType;
        }

        public XmlQueryType CheckFunction(QilFunction node) => 
            node.XmlType;

        public XmlQueryType CheckFunctionList(QilList node)
        {
            using (IEnumerator<QilNode> enumerator = node.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    QilNode current = enumerator.Current;
                }
            }
            return node.XmlType;
        }

        public XmlQueryType CheckGe(QilBinary node) => 
            this.CheckNe(node);

        public XmlQueryType CheckGlobalParameterList(QilList node)
        {
            using (IEnumerator<QilNode> enumerator = node.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    QilNode current = enumerator.Current;
                }
            }
            return node.XmlType;
        }

        public XmlQueryType CheckGlobalVariableList(QilList node)
        {
            using (IEnumerator<QilNode> enumerator = node.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    QilNode current = enumerator.Current;
                }
            }
            return node.XmlType;
        }

        public XmlQueryType CheckGt(QilBinary node) => 
            this.CheckNe(node);

        public XmlQueryType CheckIntersection(QilBinary node) => 
            this.CheckUnion(node);

        public XmlQueryType CheckInvoke(QilInvoke node) => 
            node.Function.XmlType;

        public XmlQueryType CheckIs(QilBinary node) => 
            XmlQueryTypeFactory.BooleanX;

        public XmlQueryType CheckIsEmpty(QilUnary node) => 
            XmlQueryTypeFactory.BooleanX;

        public XmlQueryType CheckIsType(QilTargetType node) => 
            XmlQueryTypeFactory.BooleanX;

        public XmlQueryType CheckLe(QilBinary node) => 
            this.CheckNe(node);

        public XmlQueryType CheckLength(QilUnary node) => 
            XmlQueryTypeFactory.IntX;

        public XmlQueryType CheckLet(QilIterator node) => 
            node.Binding.XmlType;

        public XmlQueryType CheckLiteralDecimal(QilLiteral node) => 
            XmlQueryTypeFactory.DecimalX;

        public XmlQueryType CheckLiteralDouble(QilLiteral node) => 
            XmlQueryTypeFactory.DoubleX;

        public XmlQueryType CheckLiteralInt32(QilLiteral node) => 
            XmlQueryTypeFactory.IntX;

        public XmlQueryType CheckLiteralInt64(QilLiteral node) => 
            XmlQueryTypeFactory.IntegerX;

        public XmlQueryType CheckLiteralObject(QilLiteral node) => 
            XmlQueryTypeFactory.ItemS;

        public XmlQueryType CheckLiteralQName(QilName node) => 
            XmlQueryTypeFactory.QNameX;

        public XmlQueryType CheckLiteralString(QilLiteral node) => 
            XmlQueryTypeFactory.StringX;

        public XmlQueryType CheckLiteralType(QilLiteral node) => 
            ((XmlQueryType) node);

        [Conditional("DEBUG")]
        private void CheckLiteralValue(QilNode node, Type clrTypeValue)
        {
            ((QilLiteral) node).Value.GetType();
        }

        public XmlQueryType CheckLocalNameOf(QilUnary node) => 
            XmlQueryTypeFactory.StringX;

        public XmlQueryType CheckLoop(QilLoop node)
        {
            XmlQueryType xmlType = node.Body.XmlType;
            XmlQueryCardinality cardinality = (node.Variable.NodeType == QilNodeType.Let) ? XmlQueryCardinality.One : node.Variable.Binding.XmlType.Cardinality;
            if (xmlType.IsDod)
            {
                return XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeNotRtfS, cardinality * xmlType.Cardinality);
            }
            return XmlQueryTypeFactory.PrimeProduct(xmlType, cardinality * xmlType.Cardinality);
        }

        public XmlQueryType CheckLt(QilBinary node) => 
            this.CheckNe(node);

        public XmlQueryType CheckMaximum(QilUnary node) => 
            this.CheckAverage(node);

        public XmlQueryType CheckMinimum(QilUnary node) => 
            this.CheckAverage(node);

        public XmlQueryType CheckModulo(QilBinary node) => 
            this.CheckAdd(node);

        public XmlQueryType CheckMultiply(QilBinary node) => 
            this.CheckAdd(node);

        public XmlQueryType CheckNameOf(QilUnary node) => 
            XmlQueryTypeFactory.QNameX;

        public XmlQueryType CheckNamespaceDecl(QilBinary node) => 
            XmlQueryTypeFactory.Namespace;

        public XmlQueryType CheckNamespaceUriOf(QilUnary node) => 
            XmlQueryTypeFactory.StringX;

        public XmlQueryType CheckNe(QilBinary node) => 
            XmlQueryTypeFactory.BooleanX;

        public XmlQueryType CheckNegate(QilUnary node) => 
            node.Child.XmlType;

        public XmlQueryType CheckNoDefaultValue(QilNode node) => 
            XmlQueryTypeFactory.None;

        public XmlQueryType CheckNodeRange(QilBinary node) => 
            XmlQueryTypeFactory.Choice(new XmlQueryType[] { node.Left.XmlType, XmlQueryTypeFactory.ContentS, node.Right.XmlType });

        public XmlQueryType CheckNop(QilUnary node) => 
            node.Child.XmlType;

        public XmlQueryType CheckNot(QilUnary node) => 
            XmlQueryTypeFactory.BooleanX;

        [Conditional("DEBUG")]
        private void CheckNotDisjoint(QilBinary node)
        {
        }

        [Conditional("DEBUG")]
        private void CheckNumericX(QilNode node)
        {
        }

        [Conditional("DEBUG")]
        private void CheckNumericXS(QilNode node)
        {
        }

        public XmlQueryType CheckOptimizeBarrier(QilUnary node) => 
            node.Child.XmlType;

        public XmlQueryType CheckOr(QilBinary node) => 
            this.CheckAnd(node);

        public XmlQueryType CheckParameter(QilParameter node) => 
            node.XmlType;

        public XmlQueryType CheckParent(QilUnary node) => 
            XmlQueryTypeFactory.DocumentOrElementQ;

        public XmlQueryType CheckPICtor(QilBinary node) => 
            XmlQueryTypeFactory.PI;

        public XmlQueryType CheckPositionOf(QilUnary node) => 
            XmlQueryTypeFactory.IntX;

        public XmlQueryType CheckPreceding(QilUnary node) => 
            XmlQueryTypeFactory.DocumentOrContentS;

        public XmlQueryType CheckPrecedingSibling(QilUnary node) => 
            XmlQueryTypeFactory.ContentS;

        public XmlQueryType CheckPrefixOf(QilUnary node) => 
            XmlQueryTypeFactory.StringX;

        public XmlQueryType CheckQilExpression(QilExpression node) => 
            XmlQueryTypeFactory.ItemS;

        public XmlQueryType CheckRawTextCtor(QilUnary node) => 
            XmlQueryTypeFactory.Text;

        public XmlQueryType CheckRoot(QilUnary node) => 
            XmlQueryTypeFactory.NodeNotRtf;

        public XmlQueryType CheckRtfCtor(QilBinary node) => 
            XmlQueryTypeFactory.Node;

        public XmlQueryType CheckSequence(QilList node) => 
            node.XmlType;

        public XmlQueryType CheckSort(QilLoop node)
        {
            XmlQueryType xmlType = node.Variable.Binding.XmlType;
            if (xmlType.IsDod)
            {
                return XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeNotRtfS, xmlType.Cardinality);
            }
            return node.Variable.Binding.XmlType;
        }

        public XmlQueryType CheckSortKey(QilSortKey node) => 
            node.Key.XmlType;

        public XmlQueryType CheckSortKeyList(QilList node)
        {
            using (IEnumerator<QilNode> enumerator = node.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    QilNode current = enumerator.Current;
                }
            }
            return node.XmlType;
        }

        public XmlQueryType CheckStrConcat(QilStrConcat node) => 
            XmlQueryTypeFactory.StringX;

        public XmlQueryType CheckStrLength(QilUnary node) => 
            XmlQueryTypeFactory.IntX;

        public XmlQueryType CheckStrParseQName(QilBinary node) => 
            XmlQueryTypeFactory.QNameX;

        public XmlQueryType CheckSubtract(QilBinary node) => 
            this.CheckAdd(node);

        public XmlQueryType CheckSum(QilUnary node) => 
            this.CheckAverage(node);

        public XmlQueryType CheckTextCtor(QilUnary node) => 
            XmlQueryTypeFactory.Text;

        public XmlQueryType CheckTrue(QilNode node) => 
            XmlQueryTypeFactory.BooleanX;

        public XmlQueryType CheckTypeAssert(QilTargetType node) => 
            node.TargetType;

        public XmlQueryType CheckUnion(QilBinary node) => 
            this.DistinctType(XmlQueryTypeFactory.Sequence(node.Left.XmlType, node.Right.XmlType));

        public XmlQueryType CheckUnknown(QilNode node) => 
            node.XmlType;

        public XmlQueryType CheckWarning(QilUnary node) => 
            XmlQueryTypeFactory.Empty;

        public XmlQueryType CheckXmlContext(QilNode node) => 
            XmlQueryTypeFactory.NodeNotRtf;

        [Conditional("DEBUG")]
        private void CheckXmlType(QilNode node, XmlQueryType xmlType)
        {
        }

        public XmlQueryType CheckXPathFollowing(QilUnary node) => 
            XmlQueryTypeFactory.ContentS;

        public XmlQueryType CheckXPathNamespace(QilUnary node) => 
            XmlQueryTypeFactory.NamespaceS;

        public XmlQueryType CheckXPathNodeValue(QilUnary node) => 
            XmlQueryTypeFactory.StringX;

        public XmlQueryType CheckXPathPreceding(QilUnary node) => 
            XmlQueryTypeFactory.ContentS;

        public XmlQueryType CheckXsltConvert(QilTargetType node) => 
            node.TargetType;

        public XmlQueryType CheckXsltCopy(QilBinary node) => 
            XmlQueryTypeFactory.Choice(node.Left.XmlType, node.Right.XmlType);

        public XmlQueryType CheckXsltCopyOf(QilUnary node)
        {
            if ((node.Child.XmlType.NodeKinds & XmlNodeKindFlags.Document) != XmlNodeKindFlags.None)
            {
                return XmlQueryTypeFactory.NodeNotRtfS;
            }
            return node.Child.XmlType;
        }

        public XmlQueryType CheckXsltGenerateId(QilUnary node) => 
            XmlQueryTypeFactory.StringX;

        public XmlQueryType CheckXsltInvokeEarlyBound(QilInvokeEarlyBound node) => 
            node.XmlType;

        public XmlQueryType CheckXsltInvokeLateBound(QilInvokeLateBound node) => 
            XmlQueryTypeFactory.ItemS;

        private XmlQueryType DistinctType(XmlQueryType type)
        {
            if (type.Cardinality == XmlQueryCardinality.More)
            {
                return XmlQueryTypeFactory.PrimeProduct(type, XmlQueryCardinality.OneOrMore);
            }
            if (type.Cardinality == XmlQueryCardinality.NotOne)
            {
                return XmlQueryTypeFactory.PrimeProduct(type, XmlQueryCardinality.ZeroOrMore);
            }
            return type;
        }

        private XmlQueryType FindFilterType(QilIterator variable, QilNode body)
        {
            if (body.XmlType.TypeCode == XmlTypeCode.None)
            {
                return XmlQueryTypeFactory.None;
            }
            switch (body.NodeType)
            {
                case QilNodeType.Eq:
                {
                    QilBinary binary = (QilBinary) body;
                    if ((binary.Left.NodeType == QilNodeType.PositionOf) && object.Equals(((QilUnary) binary.Left).Child, variable))
                    {
                        return XmlQueryTypeFactory.AtMost(variable.Binding.XmlType, XmlQueryCardinality.ZeroOrOne);
                    }
                    break;
                }
                case QilNodeType.IsType:
                    if (!object.Equals(((QilTargetType) body).Source, variable))
                    {
                        break;
                    }
                    return XmlQueryTypeFactory.AtMost(((QilTargetType) body).TargetType, variable.Binding.XmlType.Cardinality);

                case QilNodeType.False:
                    return XmlQueryTypeFactory.Empty;

                case QilNodeType.And:
                {
                    XmlQueryType type = this.FindFilterType(variable, ((QilBinary) body).Left);
                    if (type != null)
                    {
                        return type;
                    }
                    return this.FindFilterType(variable, ((QilBinary) body).Right);
                }
            }
            return null;
        }
    }
}

