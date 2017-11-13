namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Xsl;

    internal class QilPatternFactory
    {
        private bool debug;
        private QilFactory f;

        public QilPatternFactory(QilFactory f, bool debug)
        {
            this.f = f;
            this.debug = debug;
        }

        public QilList ActualParameterList() => 
            this.f.ActualParameterList();

        public QilList ActualParameterList(QilNode arg1)
        {
            QilList list = this.f.ActualParameterList();
            list.Add(arg1);
            return list;
        }

        public QilList ActualParameterList(params QilNode[] args) => 
            this.f.ActualParameterList(args);

        public QilList ActualParameterList(QilNode arg1, QilNode arg2)
        {
            QilList list = this.f.ActualParameterList();
            list.Add(arg1);
            list.Add(arg2);
            return list;
        }

        public QilNode Add(QilNode left, QilNode right) => 
            this.f.Add(left, right);

        public QilNode After(QilNode left, QilNode right) => 
            this.f.After(left, right);

        public QilNode Ancestor(QilNode expr) => 
            this.f.Ancestor(expr);

        public QilNode AncestorOrSelf(QilNode expr) => 
            this.f.AncestorOrSelf(expr);

        public QilNode And(QilNode left, QilNode right)
        {
            CheckLogicArg(left);
            CheckLogicArg(right);
            if (!this.debug)
            {
                if ((left.NodeType == QilNodeType.True) || (right.NodeType == QilNodeType.False))
                {
                    return right;
                }
                if ((left.NodeType == QilNodeType.False) || (right.NodeType == QilNodeType.True))
                {
                    return left;
                }
            }
            return this.f.And(left, right);
        }

        public QilNode AttributeCtor(QilNode name, QilNode val) => 
            this.f.AttributeCtor(name, val);

        public QilNode Before(QilNode left, QilNode right) => 
            this.f.Before(left, right);

        public QilNode Boolean(bool b)
        {
            if (!b)
            {
                return this.False();
            }
            return this.True();
        }

        public QilList BranchList(params QilNode[] args) => 
            this.f.BranchList(args);

        private static void CheckLogicArg(QilNode arg)
        {
        }

        public QilNode Choice(QilNode expr, QilList branches)
        {
            if (!this.debug)
            {
                switch (branches.Count)
                {
                    case 1:
                        return this.f.Loop(this.f.Let(expr), branches[0]);

                    case 2:
                        return this.f.Conditional(this.f.Eq(expr, this.f.LiteralInt32(0)), branches[0], branches[1]);
                }
            }
            return this.f.Choice(expr, branches);
        }

        public QilNode CommentCtor(QilNode content) => 
            this.f.CommentCtor(content);

        public QilNode Conditional(QilNode condition, QilNode trueBranch, QilNode falseBranch)
        {
            if (!this.debug)
            {
                switch (condition.NodeType)
                {
                    case QilNodeType.True:
                        return trueBranch;

                    case QilNodeType.False:
                        return falseBranch;

                    case QilNodeType.Not:
                        return this.Conditional(((QilUnary) condition).Child, falseBranch, trueBranch);
                }
            }
            return this.f.Conditional(condition, trueBranch, falseBranch);
        }

        public QilNode Content(QilNode context) => 
            this.f.Content(context);

        public QilNode DataSource(QilNode name, QilNode baseUri) => 
            this.f.DataSource(name, baseUri);

        public QilBinary Deref(QilNode context, QilNode id) => 
            this.f.Deref(context, id);

        public QilNode Descendant(QilNode expr) => 
            this.f.Descendant(expr);

        public QilNode DescendantOrSelf(QilNode context) => 
            this.f.DescendantOrSelf(context);

        public QilNode Divide(QilNode left, QilNode right) => 
            this.f.Divide(left, right);

        public QilNode DocOrderDistinct(QilNode collection)
        {
            if (collection.NodeType == QilNodeType.DocOrderDistinct)
            {
                return collection;
            }
            return this.f.DocOrderDistinct(collection);
        }

        public QilNode DocumentCtor(QilNode child) => 
            this.f.DocumentCtor(child);

        public QilLiteral Double(double val) => 
            this.f.LiteralDouble(val);

        public QilNode ElementCtor(QilNode name, QilNode content) => 
            this.f.ElementCtor(name, content);

        public QilNode Eq(QilNode left, QilNode right) => 
            this.f.Eq(left, right);

        public QilNode Error(QilNode text) => 
            this.f.Error(text);

        public QilNode False() => 
            this.f.False();

        public QilNode Filter(QilIterator variable, QilNode expr)
        {
            if (!this.debug && (expr.NodeType == QilNodeType.True))
            {
                return variable.Binding;
            }
            return this.f.Filter(variable, expr);
        }

        public QilNode FollowingSibling(QilNode expr) => 
            this.f.FollowingSibling(expr);

        public QilIterator For(QilNode binding) => 
            this.f.For(binding);

        public QilList FormalParameterList() => 
            this.f.FormalParameterList();

        public QilList FormalParameterList(QilNode arg1)
        {
            QilList list = this.f.FormalParameterList();
            list.Add(arg1);
            return list;
        }

        public QilList FormalParameterList(params QilNode[] args) => 
            this.f.FormalParameterList(args);

        public QilList FormalParameterList(QilNode arg1, QilNode arg2)
        {
            QilList list = this.f.FormalParameterList();
            list.Add(arg1);
            list.Add(arg2);
            return list;
        }

        public QilFunction Function(QilList args, QilNode defn, QilNode sideEffects) => 
            this.f.Function(args, defn, sideEffects, defn.XmlType);

        public QilFunction Function(QilList args, QilNode sideEffects, XmlQueryType resultType) => 
            this.f.Function(args, sideEffects, resultType);

        public QilList FunctionList() => 
            this.f.FunctionList();

        public QilNode Ge(QilNode left, QilNode right) => 
            this.f.Ge(left, right);

        public QilList GlobalParameterList() => 
            this.f.GlobalParameterList();

        public QilList GlobalVariableList() => 
            this.f.GlobalVariableList();

        public QilNode Gt(QilNode left, QilNode right) => 
            this.f.Gt(left, right);

        public QilLiteral Int32(int val) => 
            this.f.LiteralInt32(val);

        public QilNode Invoke(QilFunction func, QilList args) => 
            this.f.Invoke(func, args);

        public QilNode Is(QilNode left, QilNode right) => 
            this.f.Is(left, right);

        public QilNode IsEmpty(QilNode set) => 
            this.f.IsEmpty(set);

        public QilNode IsType(QilNode expr, XmlQueryType t) => 
            this.f.IsType(expr, t);

        public QilNode Le(QilNode left, QilNode right) => 
            this.f.Le(left, right);

        public QilNode Length(QilNode child) => 
            this.f.Length(child);

        public QilIterator Let(QilNode binding) => 
            this.f.Let(binding);

        public QilNode LocalNameOf(QilNode expr) => 
            this.f.LocalNameOf(expr);

        public QilNode Loop(QilIterator variable, QilNode body)
        {
            if (!this.debug && (body == variable.Binding))
            {
                return body;
            }
            return this.f.Loop(variable, body);
        }

        public QilNode Lt(QilNode left, QilNode right) => 
            this.f.Lt(left, right);

        public QilNode Modulo(QilNode left, QilNode right) => 
            this.f.Modulo(left, right);

        public QilNode Multiply(QilNode left, QilNode right) => 
            this.f.Multiply(left, right);

        public QilNode NameOf(QilNode expr) => 
            this.f.NameOf(expr);

        public QilNode NamespaceDecl(QilNode prefix, QilNode uri) => 
            this.f.NamespaceDecl(prefix, uri);

        public QilNode NamespaceUriOf(QilNode expr) => 
            this.f.NamespaceUriOf(expr);

        public QilNode Ne(QilNode left, QilNode right) => 
            this.f.Ne(left, right);

        public QilNode Negate(QilNode child) => 
            this.f.Negate(child);

        public QilNode NodeRange(QilNode left, QilNode right) => 
            this.f.NodeRange(left, right);

        public QilNode Nop(QilNode child) => 
            this.f.Nop(child);

        public QilNode Not(QilNode child)
        {
            if (!this.debug)
            {
                switch (child.NodeType)
                {
                    case QilNodeType.True:
                        return this.f.False();

                    case QilNodeType.False:
                        return this.f.True();

                    case QilNodeType.Not:
                        return ((QilUnary) child).Child;
                }
            }
            return this.f.Not(child);
        }

        public QilNode OptimizeBarrier(QilNode child) => 
            this.f.OptimizeBarrier(child);

        public QilNode Or(QilNode left, QilNode right)
        {
            CheckLogicArg(left);
            CheckLogicArg(right);
            if (!this.debug)
            {
                if ((left.NodeType == QilNodeType.True) || (right.NodeType == QilNodeType.False))
                {
                    return left;
                }
                if ((left.NodeType == QilNodeType.False) || (right.NodeType == QilNodeType.True))
                {
                    return right;
                }
            }
            return this.f.Or(left, right);
        }

        public QilParameter Parameter(XmlQueryType t) => 
            this.f.Parameter(t);

        public QilParameter Parameter(QilNode defaultValue, QilName name, XmlQueryType t) => 
            this.f.Parameter(defaultValue, name, t);

        public QilNode Parent(QilNode context) => 
            this.f.Parent(context);

        public QilNode PICtor(QilNode name, QilNode content) => 
            this.f.PICtor(name, content);

        public QilNode PositionOf(QilIterator expr) => 
            this.f.PositionOf(expr);

        public QilNode Preceding(QilNode expr) => 
            this.f.Preceding(expr);

        public QilNode PrecedingSibling(QilNode expr) => 
            this.f.PrecedingSibling(expr);

        public QilNode PrefixOf(QilNode expr) => 
            this.f.PrefixOf(expr);

        public System.Xml.Xsl.Qil.QilExpression QilExpression(QilNode root, QilFactory factory) => 
            this.f.QilExpression(root, factory);

        public QilName QName(string local) => 
            this.f.LiteralQName(local, string.Empty, string.Empty);

        public QilName QName(string local, string uri) => 
            this.f.LiteralQName(local, uri, string.Empty);

        public QilName QName(string local, string uri, string prefix) => 
            this.f.LiteralQName(local, uri, prefix);

        public QilNode RawTextCtor(QilNode content) => 
            this.f.RawTextCtor(content);

        public QilNode Root(QilNode context) => 
            this.f.Root(context);

        public QilNode RtfCtor(QilNode content, QilNode baseUri) => 
            this.f.RtfCtor(content, baseUri);

        public QilNode Sequence() => 
            this.f.Sequence();

        public QilNode Sequence(QilNode child)
        {
            if (!this.debug)
            {
                return child;
            }
            QilList list = this.f.Sequence();
            list.Add(child);
            return list;
        }

        public QilNode Sequence(params QilNode[] args)
        {
            if (!this.debug)
            {
                switch (args.Length)
                {
                    case 0:
                        return this.f.Sequence();

                    case 1:
                        return args[0];
                }
            }
            QilList list = this.f.Sequence();
            foreach (QilNode node in args)
            {
                list.Add(node);
            }
            return list;
        }

        public QilNode Sequence(QilNode child1, QilNode child2)
        {
            QilList list = this.f.Sequence();
            list.Add(child1);
            list.Add(child2);
            return list;
        }

        public QilNode Sort(QilIterator iter, QilNode keys) => 
            this.f.Sort(iter, keys);

        public QilSortKey SortKey(QilNode key, QilNode collation) => 
            this.f.SortKey(key, collation);

        public QilList SortKeyList() => 
            this.f.SortKeyList();

        public QilList SortKeyList(QilSortKey key)
        {
            QilList list = this.f.SortKeyList();
            list.Add((QilNode) key);
            return list;
        }

        public QilNode StrConcat(IList<QilNode> args)
        {
            if (!this.debug)
            {
                switch (args.Count)
                {
                    case 0:
                        return this.f.LiteralString(string.Empty);

                    case 1:
                        return this.StrConcat(args[0]);
                }
            }
            return this.StrConcat((QilNode) this.f.Sequence(args));
        }

        public QilNode StrConcat(QilNode values)
        {
            if (!this.debug && values.XmlType.IsSingleton)
            {
                return values;
            }
            return this.f.StrConcat(values);
        }

        public QilNode StrConcat(params QilNode[] args) => 
            this.StrConcat((IList<QilNode>) args);

        public QilLiteral String(string val) => 
            this.f.LiteralString(val);

        public QilNode StrLength(QilNode str) => 
            this.f.StrLength(str);

        public QilNode StrParseQName(QilNode str, QilNode ns) => 
            this.f.StrParseQName(str, ns);

        public QilNode Subtract(QilNode left, QilNode right) => 
            this.f.Subtract(left, right);

        public QilNode Sum(QilNode collection) => 
            this.f.Sum(collection);

        public QilNode TextCtor(QilNode content) => 
            this.f.TextCtor(content);

        public QilNode True() => 
            this.f.True();

        public QilNode TypeAssert(QilNode expr, XmlQueryType t) => 
            this.f.TypeAssert(expr, t);

        public QilNode Union(QilNode left, QilNode right) => 
            this.f.Union(left, right);

        public QilNode Unknown(XmlQueryType t) => 
            this.f.Unknown(t);

        public QilNode Warning(QilNode text) => 
            this.f.Warning(text);

        public QilNode XmlContext() => 
            this.f.XmlContext();

        public QilNode XPathFollowing(QilNode expr) => 
            this.f.XPathFollowing(expr);

        public QilNode XPathNamespace(QilNode expr) => 
            this.f.XPathNamespace(expr);

        public QilNode XPathNodeValue(QilNode expr) => 
            this.f.XPathNodeValue(expr);

        public QilNode XPathPreceding(QilNode expr) => 
            this.f.XPathPreceding(expr);

        public QilNode XsltConvert(QilNode expr, XmlQueryType t) => 
            this.f.XsltConvert(expr, t);

        public QilNode XsltCopy(QilNode expr, QilNode content) => 
            this.f.XsltCopy(expr, content);

        public QilNode XsltCopyOf(QilNode expr) => 
            this.f.XsltCopyOf(expr);

        public QilNode XsltGenerateId(QilNode expr) => 
            this.f.XsltGenerateId(expr);

        public QilNode XsltInvokeEarlyBound(QilNode name, MethodInfo d, XmlQueryType t, IList<QilNode> args)
        {
            QilList arguments = this.f.ActualParameterList();
            arguments.Add(args);
            return this.f.XsltInvokeEarlyBound(name, this.f.LiteralObject(d), arguments, t);
        }

        public QilNode XsltInvokeLateBound(QilNode name, IList<QilNode> args)
        {
            QilList arguments = this.f.ActualParameterList();
            arguments.Add(args);
            return this.f.XsltInvokeLateBound(name, arguments);
        }

        public QilFactory BaseFactory =>
            this.f;

        public bool IsDebug =>
            this.debug;
    }
}

