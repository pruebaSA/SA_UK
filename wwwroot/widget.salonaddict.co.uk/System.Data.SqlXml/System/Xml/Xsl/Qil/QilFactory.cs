namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml.Xsl;

    internal sealed class QilFactory
    {
        private QilTypeChecker typeCheck = new QilTypeChecker();

        public QilList ActualParameterList()
        {
            QilList node = new QilList(QilNodeType.ActualParameterList);
            node.XmlType = this.typeCheck.CheckActualParameterList(node);
            return node;
        }

        public QilList ActualParameterList(IList<QilNode> values)
        {
            QilList list = this.ActualParameterList();
            list.Add(values);
            return list;
        }

        public QilBinary Add(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Add, left, right);
            node.XmlType = this.typeCheck.CheckAdd(node);
            return node;
        }

        public QilBinary After(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.After, left, right);
            node.XmlType = this.typeCheck.CheckAfter(node);
            return node;
        }

        public QilUnary Ancestor(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Ancestor, child);
            node.XmlType = this.typeCheck.CheckAncestor(node);
            return node;
        }

        public QilUnary AncestorOrSelf(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.AncestorOrSelf, child);
            node.XmlType = this.typeCheck.CheckAncestorOrSelf(node);
            return node;
        }

        public QilBinary And(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.And, left, right);
            node.XmlType = this.typeCheck.CheckAnd(node);
            return node;
        }

        public QilBinary Attribute(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Attribute, left, right);
            node.XmlType = this.typeCheck.CheckAttribute(node);
            return node;
        }

        public QilBinary AttributeCtor(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.AttributeCtor, left, right);
            node.XmlType = this.typeCheck.CheckAttributeCtor(node);
            return node;
        }

        public QilUnary Average(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Average, child);
            node.XmlType = this.typeCheck.CheckAverage(node);
            return node;
        }

        public QilBinary Before(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Before, left, right);
            node.XmlType = this.typeCheck.CheckBefore(node);
            return node;
        }

        public QilList BranchList()
        {
            QilList node = new QilList(QilNodeType.BranchList);
            node.XmlType = this.typeCheck.CheckBranchList(node);
            return node;
        }

        public QilList BranchList(IList<QilNode> values)
        {
            QilList list = this.BranchList();
            list.Add(values);
            return list;
        }

        public QilChoice Choice(QilNode expression, QilNode branches)
        {
            QilChoice node = new QilChoice(QilNodeType.Choice, expression, branches);
            node.XmlType = this.typeCheck.CheckChoice(node);
            return node;
        }

        public QilUnary CommentCtor(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.CommentCtor, child);
            node.XmlType = this.typeCheck.CheckCommentCtor(node);
            return node;
        }

        public QilTernary Conditional(QilNode left, QilNode center, QilNode right)
        {
            QilTernary node = new QilTernary(QilNodeType.Conditional, left, center, right);
            node.XmlType = this.typeCheck.CheckConditional(node);
            return node;
        }

        public QilUnary Content(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Content, child);
            node.XmlType = this.typeCheck.CheckContent(node);
            return node;
        }

        public QilDataSource DataSource(QilNode name, QilNode baseUri)
        {
            QilDataSource node = new QilDataSource(QilNodeType.DataSource, name, baseUri);
            node.XmlType = this.typeCheck.CheckDataSource(node);
            return node;
        }

        public QilBinary Deref(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Deref, left, right);
            node.XmlType = this.typeCheck.CheckDeref(node);
            return node;
        }

        public QilUnary Descendant(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Descendant, child);
            node.XmlType = this.typeCheck.CheckDescendant(node);
            return node;
        }

        public QilUnary DescendantOrSelf(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.DescendantOrSelf, child);
            node.XmlType = this.typeCheck.CheckDescendantOrSelf(node);
            return node;
        }

        public QilBinary Difference(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Difference, left, right);
            node.XmlType = this.typeCheck.CheckDifference(node);
            return node;
        }

        public QilBinary Divide(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Divide, left, right);
            node.XmlType = this.typeCheck.CheckDivide(node);
            return node;
        }

        public QilUnary DocOrderDistinct(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.DocOrderDistinct, child);
            node.XmlType = this.typeCheck.CheckDocOrderDistinct(node);
            return node;
        }

        public QilUnary DocumentCtor(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.DocumentCtor, child);
            node.XmlType = this.typeCheck.CheckDocumentCtor(node);
            return node;
        }

        public QilBinary ElementCtor(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.ElementCtor, left, right);
            node.XmlType = this.typeCheck.CheckElementCtor(node);
            return node;
        }

        public QilBinary Eq(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Eq, left, right);
            node.XmlType = this.typeCheck.CheckEq(node);
            return node;
        }

        public QilUnary Error(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Error, child);
            node.XmlType = this.typeCheck.CheckError(node);
            return node;
        }

        public QilNode False()
        {
            QilNode node = new QilNode(QilNodeType.False);
            node.XmlType = this.typeCheck.CheckFalse(node);
            return node;
        }

        public QilLoop Filter(QilNode variable, QilNode body)
        {
            QilLoop node = new QilLoop(QilNodeType.Filter, variable, body);
            node.XmlType = this.typeCheck.CheckFilter(node);
            return node;
        }

        public QilUnary FollowingSibling(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.FollowingSibling, child);
            node.XmlType = this.typeCheck.CheckFollowingSibling(node);
            return node;
        }

        public QilIterator For(QilNode binding)
        {
            QilIterator node = new QilIterator(QilNodeType.For, binding);
            node.XmlType = this.typeCheck.CheckFor(node);
            return node;
        }

        public QilList FormalParameterList()
        {
            QilList node = new QilList(QilNodeType.FormalParameterList);
            node.XmlType = this.typeCheck.CheckFormalParameterList(node);
            return node;
        }

        public QilList FormalParameterList(IList<QilNode> values)
        {
            QilList list = this.FormalParameterList();
            list.Add(values);
            return list;
        }

        public QilFunction Function(QilNode arguments, QilNode sideEffects, XmlQueryType xmlType) => 
            this.Function(arguments, this.Unknown(xmlType), sideEffects, xmlType);

        public QilFunction Function(QilNode arguments, QilNode definition, QilNode sideEffects, XmlQueryType xmlType)
        {
            QilFunction node = new QilFunction(QilNodeType.Function, arguments, definition, sideEffects, xmlType);
            node.XmlType = this.typeCheck.CheckFunction(node);
            return node;
        }

        public QilList FunctionList()
        {
            QilList node = new QilList(QilNodeType.FunctionList);
            node.XmlType = this.typeCheck.CheckFunctionList(node);
            return node;
        }

        public QilList FunctionList(IList<QilNode> values)
        {
            QilList list = this.FunctionList();
            list.Add(values);
            return list;
        }

        public QilBinary Ge(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Ge, left, right);
            node.XmlType = this.typeCheck.CheckGe(node);
            return node;
        }

        public QilList GlobalParameterList()
        {
            QilList node = new QilList(QilNodeType.GlobalParameterList);
            node.XmlType = this.typeCheck.CheckGlobalParameterList(node);
            return node;
        }

        public QilList GlobalParameterList(IList<QilNode> values)
        {
            QilList list = this.GlobalParameterList();
            list.Add(values);
            return list;
        }

        public QilList GlobalVariableList()
        {
            QilList node = new QilList(QilNodeType.GlobalVariableList);
            node.XmlType = this.typeCheck.CheckGlobalVariableList(node);
            return node;
        }

        public QilList GlobalVariableList(IList<QilNode> values)
        {
            QilList list = this.GlobalVariableList();
            list.Add(values);
            return list;
        }

        public QilBinary Gt(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Gt, left, right);
            node.XmlType = this.typeCheck.CheckGt(node);
            return node;
        }

        public QilBinary Intersection(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Intersection, left, right);
            node.XmlType = this.typeCheck.CheckIntersection(node);
            return node;
        }

        public QilInvoke Invoke(QilNode function, QilNode arguments)
        {
            QilInvoke node = new QilInvoke(QilNodeType.Invoke, function, arguments);
            node.XmlType = this.typeCheck.CheckInvoke(node);
            return node;
        }

        public QilBinary Is(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Is, left, right);
            node.XmlType = this.typeCheck.CheckIs(node);
            return node;
        }

        public QilUnary IsEmpty(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.IsEmpty, child);
            node.XmlType = this.typeCheck.CheckIsEmpty(node);
            return node;
        }

        public QilTargetType IsType(QilNode source, QilNode targetType)
        {
            QilTargetType node = new QilTargetType(QilNodeType.IsType, source, targetType);
            node.XmlType = this.typeCheck.CheckIsType(node);
            return node;
        }

        public QilTargetType IsType(QilNode expr, XmlQueryType xmlType) => 
            this.IsType(expr, this.LiteralType(xmlType));

        public QilBinary Le(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Le, left, right);
            node.XmlType = this.typeCheck.CheckLe(node);
            return node;
        }

        public QilUnary Length(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Length, child);
            node.XmlType = this.typeCheck.CheckLength(node);
            return node;
        }

        public QilIterator Let(QilNode binding)
        {
            QilIterator node = new QilIterator(QilNodeType.Let, binding);
            node.XmlType = this.typeCheck.CheckLet(node);
            return node;
        }

        public QilLiteral LiteralDecimal(decimal value)
        {
            QilLiteral node = new QilLiteral(QilNodeType.LiteralDecimal, value);
            node.XmlType = this.typeCheck.CheckLiteralDecimal(node);
            return node;
        }

        public QilLiteral LiteralDouble(double value)
        {
            QilLiteral node = new QilLiteral(QilNodeType.LiteralDouble, value);
            node.XmlType = this.typeCheck.CheckLiteralDouble(node);
            return node;
        }

        public QilLiteral LiteralInt32(int value)
        {
            QilLiteral node = new QilLiteral(QilNodeType.LiteralInt32, value);
            node.XmlType = this.typeCheck.CheckLiteralInt32(node);
            return node;
        }

        public QilLiteral LiteralInt64(long value)
        {
            QilLiteral node = new QilLiteral(QilNodeType.LiteralInt64, value);
            node.XmlType = this.typeCheck.CheckLiteralInt64(node);
            return node;
        }

        public QilLiteral LiteralObject(object value)
        {
            QilLiteral node = new QilLiteral(QilNodeType.LiteralObject, value);
            node.XmlType = this.typeCheck.CheckLiteralObject(node);
            return node;
        }

        public QilName LiteralQName(string local) => 
            this.LiteralQName(local, string.Empty, string.Empty);

        public QilName LiteralQName(string localName, string namespaceUri, string prefix)
        {
            QilName node = new QilName(QilNodeType.LiteralQName, localName, namespaceUri, prefix);
            node.XmlType = this.typeCheck.CheckLiteralQName(node);
            return node;
        }

        public QilLiteral LiteralString(string value)
        {
            QilLiteral node = new QilLiteral(QilNodeType.LiteralString, value);
            node.XmlType = this.typeCheck.CheckLiteralString(node);
            return node;
        }

        public QilLiteral LiteralType(XmlQueryType value)
        {
            QilLiteral node = new QilLiteral(QilNodeType.LiteralType, value);
            node.XmlType = this.typeCheck.CheckLiteralType(node);
            return node;
        }

        public QilUnary LocalNameOf(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.LocalNameOf, child);
            node.XmlType = this.typeCheck.CheckLocalNameOf(node);
            return node;
        }

        public QilLoop Loop(QilNode variable, QilNode body)
        {
            QilLoop node = new QilLoop(QilNodeType.Loop, variable, body);
            node.XmlType = this.typeCheck.CheckLoop(node);
            return node;
        }

        public QilBinary Lt(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Lt, left, right);
            node.XmlType = this.typeCheck.CheckLt(node);
            return node;
        }

        public QilUnary Maximum(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Maximum, child);
            node.XmlType = this.typeCheck.CheckMaximum(node);
            return node;
        }

        public QilUnary Minimum(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Minimum, child);
            node.XmlType = this.typeCheck.CheckMinimum(node);
            return node;
        }

        public QilBinary Modulo(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Modulo, left, right);
            node.XmlType = this.typeCheck.CheckModulo(node);
            return node;
        }

        public QilBinary Multiply(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Multiply, left, right);
            node.XmlType = this.typeCheck.CheckMultiply(node);
            return node;
        }

        public QilUnary NameOf(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.NameOf, child);
            node.XmlType = this.typeCheck.CheckNameOf(node);
            return node;
        }

        public QilBinary NamespaceDecl(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.NamespaceDecl, left, right);
            node.XmlType = this.typeCheck.CheckNamespaceDecl(node);
            return node;
        }

        public QilUnary NamespaceUriOf(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.NamespaceUriOf, child);
            node.XmlType = this.typeCheck.CheckNamespaceUriOf(node);
            return node;
        }

        public QilBinary Ne(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Ne, left, right);
            node.XmlType = this.typeCheck.CheckNe(node);
            return node;
        }

        public QilUnary Negate(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Negate, child);
            node.XmlType = this.typeCheck.CheckNegate(node);
            return node;
        }

        public QilBinary NodeRange(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.NodeRange, left, right);
            node.XmlType = this.typeCheck.CheckNodeRange(node);
            return node;
        }

        public QilUnary Nop(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Nop, child);
            node.XmlType = this.typeCheck.CheckNop(node);
            return node;
        }

        public QilUnary Not(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Not, child);
            node.XmlType = this.typeCheck.CheckNot(node);
            return node;
        }

        public QilUnary OptimizeBarrier(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.OptimizeBarrier, child);
            node.XmlType = this.typeCheck.CheckOptimizeBarrier(node);
            return node;
        }

        public QilBinary Or(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Or, left, right);
            node.XmlType = this.typeCheck.CheckOr(node);
            return node;
        }

        public QilParameter Parameter(XmlQueryType xmlType) => 
            this.Parameter(null, null, xmlType);

        public QilParameter Parameter(QilNode defaultValue, QilNode name, XmlQueryType xmlType)
        {
            QilParameter node = new QilParameter(QilNodeType.Parameter, defaultValue, name, xmlType);
            node.XmlType = this.typeCheck.CheckParameter(node);
            return node;
        }

        public QilUnary Parent(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Parent, child);
            node.XmlType = this.typeCheck.CheckParent(node);
            return node;
        }

        public QilBinary PICtor(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.PICtor, left, right);
            node.XmlType = this.typeCheck.CheckPICtor(node);
            return node;
        }

        public QilUnary PositionOf(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.PositionOf, child);
            node.XmlType = this.typeCheck.CheckPositionOf(node);
            return node;
        }

        public QilUnary Preceding(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Preceding, child);
            node.XmlType = this.typeCheck.CheckPreceding(node);
            return node;
        }

        public QilUnary PrecedingSibling(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.PrecedingSibling, child);
            node.XmlType = this.typeCheck.CheckPrecedingSibling(node);
            return node;
        }

        public QilUnary PrefixOf(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.PrefixOf, child);
            node.XmlType = this.typeCheck.CheckPrefixOf(node);
            return node;
        }

        public System.Xml.Xsl.Qil.QilExpression QilExpression(QilNode root)
        {
            System.Xml.Xsl.Qil.QilExpression node = new System.Xml.Xsl.Qil.QilExpression(QilNodeType.QilExpression, root);
            node.XmlType = this.typeCheck.CheckQilExpression(node);
            return node;
        }

        public System.Xml.Xsl.Qil.QilExpression QilExpression(QilNode root, QilFactory factory)
        {
            System.Xml.Xsl.Qil.QilExpression node = new System.Xml.Xsl.Qil.QilExpression(QilNodeType.QilExpression, root, factory);
            node.XmlType = this.typeCheck.CheckQilExpression(node);
            return node;
        }

        public QilUnary RawTextCtor(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.RawTextCtor, child);
            node.XmlType = this.typeCheck.CheckRawTextCtor(node);
            return node;
        }

        public QilUnary Root(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Root, child);
            node.XmlType = this.typeCheck.CheckRoot(node);
            return node;
        }

        public QilBinary RtfCtor(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.RtfCtor, left, right);
            node.XmlType = this.typeCheck.CheckRtfCtor(node);
            return node;
        }

        public QilList Sequence()
        {
            QilList node = new QilList(QilNodeType.Sequence);
            node.XmlType = this.typeCheck.CheckSequence(node);
            return node;
        }

        public QilList Sequence(IList<QilNode> values)
        {
            QilList list = this.Sequence();
            list.Add(values);
            return list;
        }

        public QilLoop Sort(QilNode variable, QilNode body)
        {
            QilLoop node = new QilLoop(QilNodeType.Sort, variable, body);
            node.XmlType = this.typeCheck.CheckSort(node);
            return node;
        }

        public QilSortKey SortKey(QilNode key, QilNode collation)
        {
            QilSortKey node = new QilSortKey(QilNodeType.SortKey, key, collation);
            node.XmlType = this.typeCheck.CheckSortKey(node);
            return node;
        }

        public QilList SortKeyList()
        {
            QilList node = new QilList(QilNodeType.SortKeyList);
            node.XmlType = this.typeCheck.CheckSortKeyList(node);
            return node;
        }

        public QilList SortKeyList(IList<QilNode> values)
        {
            QilList list = this.SortKeyList();
            list.Add(values);
            return list;
        }

        public QilStrConcat StrConcat(QilNode values) => 
            this.StrConcat(this.LiteralString(""), values);

        public QilStrConcat StrConcat(QilNode delimiter, QilNode values)
        {
            QilStrConcat node = new QilStrConcat(QilNodeType.StrConcat, delimiter, values);
            node.XmlType = this.typeCheck.CheckStrConcat(node);
            return node;
        }

        public QilUnary StrLength(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.StrLength, child);
            node.XmlType = this.typeCheck.CheckStrLength(node);
            return node;
        }

        public QilBinary StrParseQName(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.StrParseQName, left, right);
            node.XmlType = this.typeCheck.CheckStrParseQName(node);
            return node;
        }

        public QilBinary Subtract(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Subtract, left, right);
            node.XmlType = this.typeCheck.CheckSubtract(node);
            return node;
        }

        public QilUnary Sum(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Sum, child);
            node.XmlType = this.typeCheck.CheckSum(node);
            return node;
        }

        public QilUnary TextCtor(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.TextCtor, child);
            node.XmlType = this.typeCheck.CheckTextCtor(node);
            return node;
        }

        [Conditional("QIL_TRACE_NODE_CREATION")]
        public void TraceNode(QilNode n)
        {
        }

        public QilNode True()
        {
            QilNode node = new QilNode(QilNodeType.True);
            node.XmlType = this.typeCheck.CheckTrue(node);
            return node;
        }

        public QilTargetType TypeAssert(QilNode source, QilNode targetType)
        {
            QilTargetType node = new QilTargetType(QilNodeType.TypeAssert, source, targetType);
            node.XmlType = this.typeCheck.CheckTypeAssert(node);
            return node;
        }

        public QilTargetType TypeAssert(QilNode expr, XmlQueryType xmlType) => 
            this.TypeAssert(expr, this.LiteralType(xmlType));

        public QilBinary Union(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.Union, left, right);
            node.XmlType = this.typeCheck.CheckUnion(node);
            return node;
        }

        public QilNode Unknown(XmlQueryType xmlType)
        {
            QilNode node = new QilNode(QilNodeType.Unknown, xmlType);
            node.XmlType = this.typeCheck.CheckUnknown(node);
            return node;
        }

        public QilUnary Warning(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.Warning, child);
            node.XmlType = this.typeCheck.CheckWarning(node);
            return node;
        }

        public QilNode XmlContext()
        {
            QilNode node = new QilNode(QilNodeType.XmlContext);
            node.XmlType = this.typeCheck.CheckXmlContext(node);
            return node;
        }

        public QilUnary XPathFollowing(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.XPathFollowing, child);
            node.XmlType = this.typeCheck.CheckXPathFollowing(node);
            return node;
        }

        public QilUnary XPathNamespace(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.XPathNamespace, child);
            node.XmlType = this.typeCheck.CheckXPathNamespace(node);
            return node;
        }

        public QilUnary XPathNodeValue(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.XPathNodeValue, child);
            node.XmlType = this.typeCheck.CheckXPathNodeValue(node);
            return node;
        }

        public QilUnary XPathPreceding(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.XPathPreceding, child);
            node.XmlType = this.typeCheck.CheckXPathPreceding(node);
            return node;
        }

        public QilTargetType XsltConvert(QilNode source, QilNode targetType)
        {
            QilTargetType node = new QilTargetType(QilNodeType.XsltConvert, source, targetType);
            node.XmlType = this.typeCheck.CheckXsltConvert(node);
            return node;
        }

        public QilTargetType XsltConvert(QilNode expr, XmlQueryType xmlType) => 
            this.XsltConvert(expr, this.LiteralType(xmlType));

        public QilBinary XsltCopy(QilNode left, QilNode right)
        {
            QilBinary node = new QilBinary(QilNodeType.XsltCopy, left, right);
            node.XmlType = this.typeCheck.CheckXsltCopy(node);
            return node;
        }

        public QilUnary XsltCopyOf(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.XsltCopyOf, child);
            node.XmlType = this.typeCheck.CheckXsltCopyOf(node);
            return node;
        }

        public QilUnary XsltGenerateId(QilNode child)
        {
            QilUnary node = new QilUnary(QilNodeType.XsltGenerateId, child);
            node.XmlType = this.typeCheck.CheckXsltGenerateId(node);
            return node;
        }

        public QilInvokeEarlyBound XsltInvokeEarlyBound(QilNode name, QilNode clrMethod, QilNode arguments, XmlQueryType xmlType)
        {
            QilInvokeEarlyBound node = new QilInvokeEarlyBound(QilNodeType.XsltInvokeEarlyBound, name, clrMethod, arguments, xmlType);
            node.XmlType = this.typeCheck.CheckXsltInvokeEarlyBound(node);
            return node;
        }

        public QilInvokeLateBound XsltInvokeLateBound(QilNode name, QilNode arguments)
        {
            QilInvokeLateBound node = new QilInvokeLateBound(QilNodeType.XsltInvokeLateBound, name, arguments);
            node.XmlType = this.typeCheck.CheckXsltInvokeLateBound(node);
            return node;
        }

        public QilTypeChecker TypeChecker =>
            this.typeCheck;
    }
}

