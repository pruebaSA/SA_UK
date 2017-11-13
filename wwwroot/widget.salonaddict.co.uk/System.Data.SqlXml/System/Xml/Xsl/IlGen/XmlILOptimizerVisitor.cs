namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Schema;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;

    internal class XmlILOptimizerVisitor : QilPatternVisitor
    {
        private XmlILStateAnalyzer contentAnalyzer;
        private XmlILElementAnalyzer elemAnalyzer;
        private XmlILNamespaceAnalyzer nmspAnalyzer;
        private NodeCounter nodeCounter;
        private static readonly QilPatternVisitor.QilPatterns PatternsNoOpt = new QilPatternVisitor.QilPatterns(0x8d, false);
        private static readonly QilPatternVisitor.QilPatterns PatternsOpt = new QilPatternVisitor.QilPatterns(0x8d, true);
        private QilExpression qil;
        private SubstitutionList subs;

        static XmlILOptimizerVisitor()
        {
            PatternsNoOpt.Add(0x68);
            PatternsNoOpt.Add(0x58);
            PatternsNoOpt.Add(0x61);
            PatternsNoOpt.Add(0x47);
            PatternsNoOpt.Add(70);
            PatternsNoOpt.Add(0x3a);
            PatternsNoOpt.Add(0x60);
            PatternsNoOpt.Add(0x4f);
            PatternsNoOpt.Add(0x4e);
            PatternsNoOpt.Add(0x5b);
            PatternsNoOpt.Add(0x5d);
            PatternsNoOpt.Add(0x86);
            PatternsNoOpt.Add(0x76);
            PatternsNoOpt.Add(0x70);
            PatternsNoOpt.Add(0x29);
            PatternsNoOpt.Add(0x30);
            PatternsNoOpt.Add(15);
            PatternsNoOpt.Add(8);
            PatternsNoOpt.Add(0x17);
            PatternsNoOpt.Add(0x18);
            PatternsNoOpt.Add(7);
            PatternsNoOpt.Add(0x12);
        }

        public XmlILOptimizerVisitor(QilExpression qil, bool optimize) : base(optimize ? PatternsOpt : PatternsNoOpt, qil.Factory)
        {
            this.nodeCounter = new NodeCounter();
            this.subs = new SubstitutionList();
            this.qil = qil;
            this.elemAnalyzer = new XmlILElementAnalyzer(qil.Factory);
            this.contentAnalyzer = new XmlILStateAnalyzer(qil.Factory);
            this.nmspAnalyzer = new XmlILNamespaceAnalyzer();
        }

        private void AddStepPattern(QilNode nd, QilNode input)
        {
            OptimizerPatterns patterns = OptimizerPatterns.Write(nd);
            patterns.AddPattern(OptimizerPatternName.Step);
            patterns.AddArgument(OptimizerPatternArgument.StepNode, nd);
            patterns.AddArgument(OptimizerPatternArgument.StepInput, input);
        }

        private bool AllowDodReverse(QilNode nd)
        {
            OptimizerPatterns patt = OptimizerPatterns.Read(nd);
            return (((patt.MatchesPattern(OptimizerPatternName.Axis) || patt.MatchesPattern(OptimizerPatternName.FilterElements)) || patt.MatchesPattern(OptimizerPatternName.FilterContentKind)) && ((this.IsStepPattern(patt, QilNodeType.Ancestor) || this.IsStepPattern(patt, QilNodeType.AncestorOrSelf)) || (this.IsStepPattern(patt, QilNodeType.XPathPreceding) || this.IsStepPattern(patt, QilNodeType.PrecedingSibling))));
        }

        private bool AllowJoinAndDod(QilNode nd)
        {
            OptimizerPatterns patt = OptimizerPatterns.Read(nd);
            return ((patt.MatchesPattern(OptimizerPatternName.FilterElements) || patt.MatchesPattern(OptimizerPatternName.FilterContentKind)) && (((this.IsStepPattern(patt, QilNodeType.DescendantOrSelf) || this.IsStepPattern(patt, QilNodeType.Descendant)) || (this.IsStepPattern(patt, QilNodeType.Content) || this.IsStepPattern(patt, QilNodeType.XPathPreceding))) || (this.IsStepPattern(patt, QilNodeType.XPathFollowing) || this.IsStepPattern(patt, QilNodeType.FollowingSibling))));
        }

        protected bool AllowReplace(XmlILOptimization pattern, QilNode original) => 
            base.AllowReplace((int) pattern, original);

        private bool AreLiteralArgs(QilNode nd)
        {
            foreach (QilNode node in nd)
            {
                if (!this.IsLiteral(node))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CanFoldArithmetic(QilNodeType opType, QilLiteral left, QilLiteral right) => 
            (this.FoldArithmetic(opType, left, right) is QilLiteral);

        private bool CanFoldXsltConvert(QilNode ndLiteral, XmlQueryType typTarget) => 
            (this.FoldXsltConvert(ndLiteral, typTarget).NodeType != QilNodeType.XsltConvert);

        private bool CanFoldXsltConvertNonLossy(QilNode ndLiteral, XmlQueryType typTarget)
        {
            QilNode node = this.FoldXsltConvert(ndLiteral, typTarget);
            if (node.NodeType == QilNodeType.XsltConvert)
            {
                return false;
            }
            node = this.FoldXsltConvert(node, ndLiteral.XmlType);
            if (node.NodeType == QilNodeType.XsltConvert)
            {
                return false;
            }
            return this.ExtractLiteralValue(ndLiteral).Equals(this.ExtractLiteralValue(node));
        }

        private bool DependsOn(QilNode expr, QilNode target) => 
            new NodeFinder().Find(expr, target);

        private object ExtractLiteralValue(QilNode nd)
        {
            if (nd.NodeType == QilNodeType.True)
            {
                return true;
            }
            if (nd.NodeType == QilNodeType.False)
            {
                return false;
            }
            if (nd.NodeType == QilNodeType.LiteralQName)
            {
                return nd;
            }
            return ((QilLiteral) nd).Value;
        }

        private QilNode FoldArithmetic(QilNodeType opType, QilLiteral left, QilLiteral right)
        {
            try
            {
                switch (left.NodeType)
                {
                    case QilNodeType.LiteralInt32:
                    {
                        int num = (int) left;
                        int num2 = (int) right;
                        switch (opType)
                        {
                            case QilNodeType.Add:
                                return base.f.LiteralInt32(num + num2);

                            case QilNodeType.Subtract:
                                return base.f.LiteralInt32(num - num2);

                            case QilNodeType.Multiply:
                                return base.f.LiteralInt32(num * num2);

                            case QilNodeType.Divide:
                                return base.f.LiteralInt32(num / num2);

                            case QilNodeType.Modulo:
                                return base.f.LiteralInt32(num % num2);
                        }
                        goto Label_02CB;
                    }
                    case QilNodeType.LiteralInt64:
                    {
                        long num3 = (long) left;
                        long num4 = (long) right;
                        switch (opType)
                        {
                            case QilNodeType.Add:
                                return base.f.LiteralInt64(num3 + num4);

                            case QilNodeType.Subtract:
                                return base.f.LiteralInt64(num3 - num4);

                            case QilNodeType.Multiply:
                                return base.f.LiteralInt64(num3 * num4);

                            case QilNodeType.Divide:
                                return base.f.LiteralInt64(num3 / num4);

                            case QilNodeType.Modulo:
                                return base.f.LiteralInt64(num3 % num4);
                        }
                        goto Label_02CB;
                    }
                    case QilNodeType.LiteralDouble:
                    {
                        double num7 = (double) left;
                        double num8 = (double) right;
                        switch (opType)
                        {
                            case QilNodeType.Add:
                                return base.f.LiteralDouble(num7 + num8);

                            case QilNodeType.Subtract:
                                return base.f.LiteralDouble(num7 - num8);

                            case QilNodeType.Multiply:
                                return base.f.LiteralDouble(num7 * num8);

                            case QilNodeType.Divide:
                                return base.f.LiteralDouble(num7 / num8);

                            case QilNodeType.Modulo:
                                return base.f.LiteralDouble(num7 % num8);
                        }
                        goto Label_02CB;
                    }
                    case QilNodeType.LiteralDecimal:
                    {
                        decimal num5 = (decimal) left;
                        decimal num6 = (decimal) right;
                        switch (opType)
                        {
                            case QilNodeType.Add:
                                return base.f.LiteralDecimal(num5 + num6);

                            case QilNodeType.Subtract:
                                return base.f.LiteralDecimal(num5 - num6);

                            case QilNodeType.Multiply:
                                return base.f.LiteralDecimal(num5 * num6);

                            case QilNodeType.Divide:
                                return base.f.LiteralDecimal(num5 / num6);

                            case QilNodeType.Modulo:
                                return base.f.LiteralDecimal(num5 % num6);
                        }
                        goto Label_02CB;
                    }
                }
            }
            catch (OverflowException)
            {
            }
            catch (DivideByZeroException)
            {
            }
        Label_02CB:
            switch (opType)
            {
                case QilNodeType.Add:
                    return base.f.Add(left, right);

                case QilNodeType.Subtract:
                    return base.f.Subtract(left, right);

                case QilNodeType.Multiply:
                    return base.f.Multiply(left, right);

                case QilNodeType.Divide:
                    return base.f.Divide(left, right);

                case QilNodeType.Modulo:
                    return base.f.Modulo(left, right);
            }
            return null;
        }

        private QilNode FoldComparison(QilNodeType opType, QilNode left, QilNode right)
        {
            int num;
            object obj2 = this.ExtractLiteralValue(left);
            object obj3 = this.ExtractLiteralValue(right);
            if ((left.NodeType == QilNodeType.LiteralDouble) && (double.IsNaN((double) obj2) || double.IsNaN((double) obj3)))
            {
                if (opType != QilNodeType.Ne)
                {
                    return base.f.False();
                }
                return base.f.True();
            }
            if (opType == QilNodeType.Eq)
            {
                if (!obj2.Equals(obj3))
                {
                    return base.f.False();
                }
                return base.f.True();
            }
            if (opType == QilNodeType.Ne)
            {
                if (!obj2.Equals(obj3))
                {
                    return base.f.True();
                }
                return base.f.False();
            }
            if (left.NodeType == QilNodeType.LiteralString)
            {
                num = string.CompareOrdinal((string) obj2, (string) obj3);
            }
            else
            {
                num = ((IComparable) obj2).CompareTo(obj3);
            }
            switch (opType)
            {
                case QilNodeType.Gt:
                    if (num > 0)
                    {
                        return base.f.True();
                    }
                    return base.f.False();

                case QilNodeType.Ge:
                    if (num >= 0)
                    {
                        return base.f.True();
                    }
                    return base.f.False();

                case QilNodeType.Lt:
                    if (num < 0)
                    {
                        return base.f.True();
                    }
                    return base.f.False();

                case QilNodeType.Le:
                    if (num <= 0)
                    {
                        return base.f.True();
                    }
                    return base.f.False();
            }
            return null;
        }

        private QilNode FoldXsltConvert(QilNode ndLiteral, XmlQueryType typTarget)
        {
            try
            {
                if (typTarget.IsAtomicValue)
                {
                    XmlAtomicValue value2 = new XmlAtomicValue(ndLiteral.XmlType.SchemaType, this.ExtractLiteralValue(ndLiteral));
                    value2 = XsltConvert.ConvertToType(value2, typTarget);
                    if (typTarget == XmlQueryTypeFactory.StringX)
                    {
                        return base.f.LiteralString(value2.Value);
                    }
                    if (typTarget == XmlQueryTypeFactory.IntX)
                    {
                        return base.f.LiteralInt32(value2.ValueAsInt);
                    }
                    if (typTarget == XmlQueryTypeFactory.IntegerX)
                    {
                        return base.f.LiteralInt64(value2.ValueAsLong);
                    }
                    if (typTarget == XmlQueryTypeFactory.DecimalX)
                    {
                        return base.f.LiteralDecimal((decimal) value2.ValueAs(XsltConvert.DecimalType));
                    }
                    if (typTarget == XmlQueryTypeFactory.DoubleX)
                    {
                        return base.f.LiteralDouble(value2.ValueAsDouble);
                    }
                    if (typTarget == XmlQueryTypeFactory.BooleanX)
                    {
                        return (value2.ValueAsBoolean ? base.f.True() : base.f.False());
                    }
                }
            }
            catch (OverflowException)
            {
            }
            catch (FormatException)
            {
            }
            return base.f.XsltConvert(ndLiteral, typTarget);
        }

        private bool HasNestedSequence(QilNode nd)
        {
            foreach (QilNode node in nd)
            {
                if (node.NodeType == QilNodeType.Sequence)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsConstructedExpression(QilNode nd)
        {
            if (this.qil.IsDebug)
            {
                return true;
            }
            if (nd.XmlType.IsNode)
            {
                switch (nd.NodeType)
                {
                    case QilNodeType.Conditional:
                    {
                        QilTernary ternary = (QilTernary) nd;
                        if (!this.IsConstructedExpression(ternary.Center))
                        {
                            return this.IsConstructedExpression(ternary.Right);
                        }
                        return true;
                    }
                    case QilNodeType.Choice:
                    case QilNodeType.ElementCtor:
                    case QilNodeType.AttributeCtor:
                    case QilNodeType.CommentCtor:
                    case QilNodeType.PICtor:
                    case QilNodeType.TextCtor:
                    case QilNodeType.RawTextCtor:
                    case QilNodeType.DocumentCtor:
                    case QilNodeType.NamespaceDecl:
                    case QilNodeType.XsltCopy:
                    case QilNodeType.XsltCopyOf:
                        return true;

                    case QilNodeType.Sequence:
                        if (nd.Count != 0)
                        {
                            foreach (QilNode node in nd)
                            {
                                if (this.IsConstructedExpression(node))
                                {
                                    return true;
                                }
                            }
                            break;
                        }
                        return true;

                    case QilNodeType.Loop:
                        return this.IsConstructedExpression(((QilLoop) nd).Body);

                    case QilNodeType.Invoke:
                        return !((QilInvoke) nd).Function.XmlType.IsAtomicValue;
                }
            }
            return false;
        }

        private bool IsDocOrderDistinct(QilNode nd) => 
            OptimizerPatterns.Read(nd).MatchesPattern(OptimizerPatternName.IsDocOrderDistinct);

        private bool IsGlobalVariable(QilIterator iter) => 
            this.qil.GlobalVariableList.Contains(iter);

        private bool IsLiteral(QilNode nd)
        {
            switch (nd.NodeType)
            {
                case QilNodeType.True:
                case QilNodeType.False:
                case QilNodeType.LiteralString:
                case QilNodeType.LiteralInt32:
                case QilNodeType.LiteralInt64:
                case QilNodeType.LiteralDouble:
                case QilNodeType.LiteralDecimal:
                case QilNodeType.LiteralQName:
                    return true;
            }
            return false;
        }

        private bool IsPrimitiveNumeric(XmlQueryType typ) => 
            ((typ == XmlQueryTypeFactory.IntX) || ((typ == XmlQueryTypeFactory.IntegerX) || ((typ == XmlQueryTypeFactory.DecimalX) || ((typ == XmlQueryTypeFactory.FloatX) || (typ == XmlQueryTypeFactory.DoubleX)))));

        private bool IsStepPattern(OptimizerPatterns patt, QilNodeType stepType) => 
            (patt.MatchesPattern(OptimizerPatternName.Step) && (((QilNode) patt.GetArgument(OptimizerPatternArgument.StepNode)).NodeType == stepType));

        private bool IsStepPattern(QilNode nd, QilNodeType stepType) => 
            this.IsStepPattern(OptimizerPatterns.Read(nd), stepType);

        private bool MatchesContentTest(XmlQueryType typ) => 
            ((typ == XmlQueryTypeFactory.Element) || ((typ == XmlQueryTypeFactory.Text) || ((typ == XmlQueryTypeFactory.Comment) || ((typ == XmlQueryTypeFactory.PI) || (typ == XmlQueryTypeFactory.Content)))));

        protected bool NonPositional(QilNode expr, QilNode iter) => 
            !new PositionOfFinder().Find(expr, iter);

        protected override QilNode NoReplace(QilNode node)
        {
            int num;
            if (node == null)
            {
                return node;
            }
            switch (node.NodeType)
            {
                case QilNodeType.Error:
                case QilNodeType.Warning:
                case QilNodeType.XsltInvokeLateBound:
                    break;

                case QilNodeType.Invoke:
                    if (((QilInvoke) node).Function.MaybeSideEffects)
                    {
                        break;
                    }
                    goto Label_0070;

                case QilNodeType.XsltInvokeEarlyBound:
                    if (((QilInvokeEarlyBound) node).Name.NamespaceUri.Length != 0)
                    {
                        break;
                    }
                    goto Label_0070;

                default:
                    goto Label_0070;
            }
        Label_0036:
            OptimizerPatterns.Write(node).AddPattern(OptimizerPatternName.MaybeSideEffects);
            return node;
        Label_0070:
            num = 0;
            while (num < node.Count)
            {
                if ((node[num] != null) && OptimizerPatterns.Read(node[num]).MatchesPattern(OptimizerPatternName.MaybeSideEffects))
                {
                    goto Label_0036;
                }
                num++;
            }
            return node;
        }

        public QilExpression Optimize()
        {
            QilExpression qil = (QilExpression) this.Visit(this.qil);
            if (this[XmlILOptimization.TailCall])
            {
                TailCallAnalyzer.Analyze(qil);
            }
            return qil;
        }

        protected override void RecalculateType(QilNode node, XmlQueryType oldType)
        {
            if ((node.NodeType != QilNodeType.Let) || !this.qil.GlobalVariableList.Contains(node))
            {
                base.RecalculateType(node, oldType);
            }
        }

        protected QilNode Replace(XmlILOptimization pattern, QilNode original, QilNode replacement) => 
            base.Replace((int) pattern, original, replacement);

        private QilNode Subs(QilNode expr, QilNode refOld, QilNode refNew)
        {
            QilNode node;
            this.subs.AddSubstitutionPair(refOld, refNew);
            if (expr is QilReference)
            {
                node = this.VisitReference(expr);
            }
            else
            {
                node = this.Visit(expr);
            }
            this.subs.RemoveLastSubstitutionPair();
            return node;
        }

        protected override QilNode Visit(QilNode nd)
        {
            if ((nd != null) && this[XmlILOptimization.EliminateNamespaceDecl])
            {
                switch (nd.NodeType)
                {
                    case QilNodeType.QilExpression:
                        this.nmspAnalyzer.Analyze(((QilExpression) nd).Root, true);
                        break;

                    case QilNodeType.ElementCtor:
                        if (!XmlILConstructInfo.Read(nd).IsNamespaceInScope)
                        {
                            this.nmspAnalyzer.Analyze(nd, false);
                        }
                        break;

                    case QilNodeType.DocumentCtor:
                        this.nmspAnalyzer.Analyze(nd, true);
                        break;
                }
            }
            return base.Visit(nd);
        }

        protected override QilNode VisitAdd(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (((this[XmlILOptimization.EliminateAdd] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.CanFoldArithmetic(QilNodeType.Add, (QilLiteral) child, (QilLiteral) node2))) && this.AllowReplace(XmlILOptimization.EliminateAdd, local0))
            {
                return this.Replace(XmlILOptimization.EliminateAdd, local0, this.FoldArithmetic(QilNodeType.Add, (QilLiteral) child, (QilLiteral) node2));
            }
            if ((this[XmlILOptimization.NormalizeAddLiteral] && this.IsLiteral(child)) && (!this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.NormalizeAddLiteral, local0)))
            {
                return this.Replace(XmlILOptimization.NormalizeAddLiteral, local0, this.VisitAdd(base.f.Add(node2, child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitAfter(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateAfter] && (node2 == child)) && this.AllowReplace(XmlILOptimization.EliminateAfter, local0))
            {
                return this.Replace(XmlILOptimization.EliminateAfter, local0, this.VisitFalse(base.f.False()));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitAncestor(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateAncestor] && this.AllowReplace(XmlILOptimization.AnnotateAncestor, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitAncestorOrSelf(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateAncestorSelf] && this.AllowReplace(XmlILOptimization.AnnotateAncestorSelf, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitAnd(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateAnd] && (child.NodeType == QilNodeType.True)) && this.AllowReplace(XmlILOptimization.EliminateAnd, local0))
            {
                return this.Replace(XmlILOptimization.EliminateAnd, local0, node2);
            }
            if ((this[XmlILOptimization.EliminateAnd] && (child.NodeType == QilNodeType.False)) && this.AllowReplace(XmlILOptimization.EliminateAnd, local0))
            {
                return this.Replace(XmlILOptimization.EliminateAnd, local0, child);
            }
            if ((this[XmlILOptimization.EliminateAnd] && (node2.NodeType == QilNodeType.True)) && this.AllowReplace(XmlILOptimization.EliminateAnd, local0))
            {
                return this.Replace(XmlILOptimization.EliminateAnd, local0, child);
            }
            if ((this[XmlILOptimization.EliminateAnd] && (node2.NodeType == QilNodeType.False)) && this.AllowReplace(XmlILOptimization.EliminateAnd, local0))
            {
                return this.Replace(XmlILOptimization.EliminateAnd, local0, node2);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitAttribute(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (this[XmlILOptimization.AnnotateAttribute] && this.AllowReplace(XmlILOptimization.AnnotateAttribute, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitAttributeCtor(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                local0.Right = this.contentAnalyzer.Analyze(local0, node2);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitAverage(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateAverage] && (child.XmlType.Cardinality == XmlQueryCardinality.Zero)) && this.AllowReplace(XmlILOptimization.EliminateAverage, local0))
            {
                return this.Replace(XmlILOptimization.EliminateAverage, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitBefore(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateBefore] && (node2 == child)) && this.AllowReplace(XmlILOptimization.EliminateBefore, local0))
            {
                return this.Replace(XmlILOptimization.EliminateBefore, local0, this.VisitFalse(base.f.False()));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitChoice(QilChoice local0)
        {
            QilNode node1 = local0[0];
            QilNode node2 = local0[1];
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                this.contentAnalyzer.Analyze(local0, null);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitCommentCtor(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                local0.Child = this.contentAnalyzer.Analyze(local0, child);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitConditional(QilTernary local0)
        {
            QilNode child = local0[0];
            QilNode replacement = local0[1];
            QilNode node3 = local0[2];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateConditional] && (child.NodeType == QilNodeType.True)) && this.AllowReplace(XmlILOptimization.EliminateConditional, local0))
            {
                return this.Replace(XmlILOptimization.EliminateConditional, local0, replacement);
            }
            if ((this[XmlILOptimization.EliminateConditional] && (child.NodeType == QilNodeType.False)) && this.AllowReplace(XmlILOptimization.EliminateConditional, local0))
            {
                return this.Replace(XmlILOptimization.EliminateConditional, local0, node3);
            }
            if ((this[XmlILOptimization.EliminateConditional] && (replacement.NodeType == QilNodeType.True)) && ((node3.NodeType == QilNodeType.False) && this.AllowReplace(XmlILOptimization.EliminateConditional, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateConditional, local0, child);
            }
            if ((this[XmlILOptimization.EliminateConditional] && (replacement.NodeType == QilNodeType.False)) && ((node3.NodeType == QilNodeType.True) && this.AllowReplace(XmlILOptimization.EliminateConditional, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateConditional, local0, this.VisitNot(base.f.Not(child)));
            }
            if (this[XmlILOptimization.FoldConditionalNot] && (child.NodeType == QilNodeType.Not))
            {
                QilNode left = child[0];
                if (this.AllowReplace(XmlILOptimization.FoldConditionalNot, local0))
                {
                    return this.Replace(XmlILOptimization.FoldConditionalNot, local0, this.VisitConditional(base.f.Conditional(left, node3, replacement)));
                }
            }
            if (this[XmlILOptimization.NormalizeConditionalText] && (replacement.NodeType == QilNodeType.TextCtor))
            {
                QilNode center = replacement[0];
                if (node3.NodeType == QilNodeType.TextCtor)
                {
                    QilNode right = node3[0];
                    if (this.AllowReplace(XmlILOptimization.NormalizeConditionalText, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeConditionalText, local0, this.VisitTextCtor(base.f.TextCtor(this.VisitConditional(base.f.Conditional(child, center, right)))));
                    }
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitContent(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateContent] && this.AllowReplace(XmlILOptimization.AnnotateContent, local0))
            {
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitDataSource(QilDataSource local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitDeref(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitDescendant(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateDescendant] && this.AllowReplace(XmlILOptimization.AnnotateDescendant, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitDescendantOrSelf(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateDescendantSelf] && this.AllowReplace(XmlILOptimization.AnnotateDescendantSelf, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitDifference(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateDifference] && (child.NodeType == QilNodeType.Sequence)) && ((child.Count == 0) && this.AllowReplace(XmlILOptimization.EliminateDifference, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateDifference, local0, child);
            }
            if ((this[XmlILOptimization.EliminateDifference] && (node2.NodeType == QilNodeType.Sequence)) && ((node2.Count == 0) && this.AllowReplace(XmlILOptimization.EliminateDifference, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateDifference, local0, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(child)));
            }
            if ((this[XmlILOptimization.EliminateDifference] && (node2 == child)) && this.AllowReplace(XmlILOptimization.EliminateDifference, local0))
            {
                return this.Replace(XmlILOptimization.EliminateDifference, local0, this.VisitSequence(base.f.Sequence()));
            }
            if ((this[XmlILOptimization.EliminateDifference] && (child.NodeType == QilNodeType.XmlContext)) && ((node2.NodeType == QilNodeType.XmlContext) && this.AllowReplace(XmlILOptimization.EliminateDifference, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateDifference, local0, this.VisitSequence(base.f.Sequence()));
            }
            if ((this[XmlILOptimization.NormalizeDifference] && (!this.IsDocOrderDistinct(child) || !this.IsDocOrderDistinct(node2))) && this.AllowReplace(XmlILOptimization.NormalizeDifference, local0))
            {
                return this.Replace(XmlILOptimization.NormalizeDifference, local0, this.VisitDifference(base.f.Difference(this.VisitDocOrderDistinct(base.f.DocOrderDistinct(child)), this.VisitDocOrderDistinct(base.f.DocOrderDistinct(node2)))));
            }
            if (this[XmlILOptimization.AnnotateDifference] && this.AllowReplace(XmlILOptimization.AnnotateDifference, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitDivide(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (((this[XmlILOptimization.EliminateDivide] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.CanFoldArithmetic(QilNodeType.Divide, (QilLiteral) child, (QilLiteral) node2))) && this.AllowReplace(XmlILOptimization.EliminateDivide, local0))
            {
                return this.Replace(XmlILOptimization.EliminateDivide, local0, this.FoldArithmetic(QilNodeType.Divide, (QilLiteral) child, (QilLiteral) node2));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitDocOrderDistinct(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateDod] && this.IsDocOrderDistinct(child)) && this.AllowReplace(XmlILOptimization.EliminateDod, local0))
            {
                return this.Replace(XmlILOptimization.EliminateDod, local0, child);
            }
            if (this[XmlILOptimization.FoldNamedDescendants] && (child.NodeType == QilNodeType.Loop))
            {
                QilNode node2 = child[0];
                QilNode nd = child[1];
                if (node2.NodeType == QilNodeType.For)
                {
                    QilNode node4 = node2[0];
                    if (node4.NodeType == QilNodeType.Loop)
                    {
                        QilNode variable = node4[0];
                        QilNode node6 = node4[1];
                        if (node6.NodeType == QilNodeType.DescendantOrSelf)
                        {
                            QilNode node7 = node6[0];
                            if (nd.NodeType == QilNodeType.Filter)
                            {
                                QilNode refOld = nd[0];
                                QilNode expr = nd[1];
                                if ((OptimizerPatterns.Read(nd).MatchesPattern(OptimizerPatternName.FilterElements) || OptimizerPatterns.Read(nd).MatchesPattern(OptimizerPatternName.FilterContentKind)) && (this.IsStepPattern(nd, QilNodeType.Content) && this.AllowReplace(XmlILOptimization.FoldNamedDescendants, local0)))
                                {
                                    QilNode node10 = this.VisitFor(base.f.For(this.VisitDescendant(base.f.Descendant(node7))));
                                    return this.Replace(XmlILOptimization.FoldNamedDescendants, local0, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(this.VisitLoop(base.f.Loop(variable, this.VisitFilter(base.f.Filter(node10, this.Subs(expr, refOld, node10))))))));
                                }
                            }
                        }
                    }
                }
            }
            if (this[XmlILOptimization.FoldNamedDescendants] && (child.NodeType == QilNodeType.Loop))
            {
                QilNode node11 = child[0];
                QilNode node12 = child[1];
                if (node11.NodeType == QilNodeType.For)
                {
                    QilNode node13 = node11[0];
                    if (node13.NodeType == QilNodeType.DescendantOrSelf)
                    {
                        QilNode node14 = node13[0];
                        if (node12.NodeType == QilNodeType.Filter)
                        {
                            QilNode node15 = node12[0];
                            QilNode node16 = node12[1];
                            if ((OptimizerPatterns.Read(node12).MatchesPattern(OptimizerPatternName.FilterElements) || OptimizerPatterns.Read(node12).MatchesPattern(OptimizerPatternName.FilterContentKind)) && (this.IsStepPattern(node12, QilNodeType.Content) && this.AllowReplace(XmlILOptimization.FoldNamedDescendants, local0)))
                            {
                                QilNode node17 = this.VisitFor(base.f.For(this.VisitDescendant(base.f.Descendant(node14))));
                                return this.Replace(XmlILOptimization.FoldNamedDescendants, local0, this.VisitFilter(base.f.Filter(node17, this.Subs(node16, node15, node17))));
                            }
                        }
                    }
                }
            }
            if (this[XmlILOptimization.CommuteDodFilter] && (child.NodeType == QilNodeType.Filter))
            {
                QilNode node18 = child[0];
                QilNode node19 = child[1];
                if (node18.NodeType == QilNodeType.For)
                {
                    QilNode node20 = node18[0];
                    if (((!OptimizerPatterns.Read(node18).MatchesPattern(OptimizerPatternName.IsPositional) && !OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.FilterElements)) && (!OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.FilterContentKind) && !OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.FilterAttributeKind))) && this.AllowReplace(XmlILOptimization.CommuteDodFilter, local0))
                    {
                        QilNode node21 = this.VisitFor(base.f.For(this.VisitDocOrderDistinct(base.f.DocOrderDistinct(node20))));
                        return this.Replace(XmlILOptimization.CommuteDodFilter, local0, this.VisitFilter(base.f.Filter(node21, this.Subs(node19, node18, node21))));
                    }
                }
            }
            if (this[XmlILOptimization.CommuteDodFilter] && (child.NodeType == QilNodeType.Loop))
            {
                QilNode target = child[0];
                QilNode node23 = child[1];
                if (node23.NodeType == QilNodeType.Filter)
                {
                    QilNode node24 = node23[0];
                    QilNode node25 = node23[1];
                    if (node24.NodeType == QilNodeType.For)
                    {
                        QilNode body = node24[0];
                        if (((!OptimizerPatterns.Read(node24).MatchesPattern(OptimizerPatternName.IsPositional) && !this.DependsOn(node25, target)) && (!OptimizerPatterns.Read(node23).MatchesPattern(OptimizerPatternName.FilterElements) && !OptimizerPatterns.Read(node23).MatchesPattern(OptimizerPatternName.FilterContentKind))) && (!OptimizerPatterns.Read(node23).MatchesPattern(OptimizerPatternName.FilterAttributeKind) && this.AllowReplace(XmlILOptimization.CommuteDodFilter, local0)))
                        {
                            QilNode node27 = this.VisitFor(base.f.For(this.VisitDocOrderDistinct(base.f.DocOrderDistinct(this.VisitLoop(base.f.Loop(target, body))))));
                            return this.Replace(XmlILOptimization.CommuteDodFilter, local0, this.VisitFilter(base.f.Filter(node27, this.Subs(node25, node24, node27))));
                        }
                    }
                }
            }
            if (this[XmlILOptimization.IntroduceDod] && (child.NodeType == QilNodeType.Loop))
            {
                QilNode node28 = child[0];
                QilNode node29 = child[1];
                if (node28.NodeType == QilNodeType.For)
                {
                    QilNode node30 = node28[0];
                    if (((!this.IsDocOrderDistinct(node30) && !OptimizerPatterns.Read(node28).MatchesPattern(OptimizerPatternName.IsPositional)) && (node30.XmlType.IsSubtypeOf(XmlQueryTypeFactory.NodeNotRtfS) && !OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.FilterElements))) && ((!OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.FilterContentKind) && !OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.FilterAttributeKind)) && this.AllowReplace(XmlILOptimization.IntroduceDod, local0)))
                    {
                        QilNode node31 = this.VisitFor(base.f.For(this.VisitDocOrderDistinct(base.f.DocOrderDistinct(node30))));
                        return this.Replace(XmlILOptimization.IntroduceDod, local0, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(this.VisitLoop(base.f.Loop(node31, this.Subs(node29, node28, node31))))));
                    }
                }
            }
            if (this[XmlILOptimization.IntroducePrecedingDod] && (child.NodeType == QilNodeType.Loop))
            {
                QilNode node32 = child[0];
                QilNode node33 = child[1];
                if ((!this.IsDocOrderDistinct(node33) && this.IsStepPattern(node33, QilNodeType.PrecedingSibling)) && this.AllowReplace(XmlILOptimization.IntroducePrecedingDod, local0))
                {
                    return this.Replace(XmlILOptimization.IntroducePrecedingDod, local0, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(this.VisitLoop(base.f.Loop(node32, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(node33)))))));
                }
            }
            if (this[XmlILOptimization.EliminateReturnDod] && (child.NodeType == QilNodeType.Loop))
            {
                QilNode node34 = child[0];
                QilNode node35 = child[1];
                if (node35.NodeType == QilNodeType.DocOrderDistinct)
                {
                    QilNode node36 = node35[0];
                    if (!this.IsStepPattern(node36, QilNodeType.PrecedingSibling) && this.AllowReplace(XmlILOptimization.EliminateReturnDod, local0))
                    {
                        return this.Replace(XmlILOptimization.EliminateReturnDod, local0, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(this.VisitLoop(base.f.Loop(node34, node36)))));
                    }
                }
            }
            if (this[XmlILOptimization.AnnotateDod] && this.AllowReplace(XmlILOptimization.AnnotateDod, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                OptimizerPatterns.Inherit(child, local0, OptimizerPatternName.SameDepth);
            }
            if ((this[XmlILOptimization.AnnotateDodReverse] && this.AllowDodReverse(child)) && this.AllowReplace(XmlILOptimization.AnnotateDodReverse, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.DodReverse);
                OptimizerPatterns.Write(local0).AddArgument(OptimizerPatternArgument.ElementQName, child);
            }
            if (this[XmlILOptimization.AnnotateJoinAndDod] && (child.NodeType == QilNodeType.Loop))
            {
                QilNode node37 = child[0];
                QilNode node38 = child[1];
                if (node37.NodeType == QilNodeType.For)
                {
                    QilNode node39 = node37[0];
                    if ((this.IsDocOrderDistinct(node39) && this.AllowJoinAndDod(node38)) && ((node37 == OptimizerPatterns.Read(node38).GetArgument(OptimizerPatternArgument.StepInput)) && this.AllowReplace(XmlILOptimization.AnnotateJoinAndDod, local0)))
                    {
                        OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.JoinAndDod);
                        OptimizerPatterns.Write(local0).AddArgument(OptimizerPatternArgument.ElementQName, node38);
                    }
                }
            }
            if (this[XmlILOptimization.AnnotateDodMerge] && (child.NodeType == QilNodeType.Loop))
            {
                QilNode node40 = child[1];
                if (((node40.NodeType == QilNodeType.Invoke) && this.IsDocOrderDistinct(node40)) && this.AllowReplace(XmlILOptimization.AnnotateDodMerge, local0))
                {
                    OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.DodMerge);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitDocumentCtor(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                local0.Child = this.contentAnalyzer.Analyze(local0, child);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitElementCtor(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                local0.Right = this.elemAnalyzer.Analyze(local0, node2);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitEq(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateEq] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.EliminateEq, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateEq, local0, this.FoldComparison(QilNodeType.Eq, child, node2));
            }
            if ((this[XmlILOptimization.NormalizeEqLiteral] && this.IsLiteral(child)) && (!this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.NormalizeEqLiteral, local0)))
            {
                return this.Replace(XmlILOptimization.NormalizeEqLiteral, local0, this.VisitEq(base.f.Eq(node2, child)));
            }
            if (this[XmlILOptimization.NormalizeXsltConvertEq] && (child.NodeType == QilNodeType.XsltConvert))
            {
                QilNode left = child[0];
                QilNode node4 = child[1];
                if (node4.NodeType == QilNodeType.LiteralType)
                {
                    XmlQueryType typ = (XmlQueryType) ((QilLiteral) node4).Value;
                    if (((this.IsPrimitiveNumeric(left.XmlType) && this.IsPrimitiveNumeric(typ)) && (this.IsLiteral(node2) && this.CanFoldXsltConvertNonLossy(node2, left.XmlType))) && this.AllowReplace(XmlILOptimization.NormalizeXsltConvertEq, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeXsltConvertEq, local0, this.VisitEq(base.f.Eq(left, this.FoldXsltConvert(node2, left.XmlType))));
                    }
                }
            }
            if (this[XmlILOptimization.NormalizeAddEq] && (child.NodeType == QilNodeType.Add))
            {
                QilNode node5 = child[0];
                QilNode nd = child[1];
                if ((this.IsLiteral(nd) && this.IsLiteral(node2)) && (this.CanFoldArithmetic(QilNodeType.Subtract, (QilLiteral) node2, (QilLiteral) nd) && this.AllowReplace(XmlILOptimization.NormalizeAddEq, local0)))
                {
                    return this.Replace(XmlILOptimization.NormalizeAddEq, local0, this.VisitEq(base.f.Eq(node5, this.FoldArithmetic(QilNodeType.Subtract, (QilLiteral) node2, (QilLiteral) nd))));
                }
            }
            if (this[XmlILOptimization.NormalizeIdEq] && (child.NodeType == QilNodeType.XsltGenerateId))
            {
                QilNode node7 = child[0];
                if (node7.XmlType.IsSingleton && (node2.NodeType == QilNodeType.XsltGenerateId))
                {
                    QilNode right = node2[0];
                    if (right.XmlType.IsSingleton && this.AllowReplace(XmlILOptimization.NormalizeIdEq, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeIdEq, local0, this.VisitIs(base.f.Is(node7, right)));
                    }
                }
            }
            if (this[XmlILOptimization.NormalizeIdEq] && (child.NodeType == QilNodeType.XsltGenerateId))
            {
                QilNode node9 = child[0];
                if (node9.XmlType.IsSingleton && (node2.NodeType == QilNodeType.StrConcat))
                {
                    QilNode node10 = node2[1];
                    if (node10.NodeType == QilNodeType.Loop)
                    {
                        QilNode node11 = node10[0];
                        QilNode node12 = node10[1];
                        if (node11.NodeType == QilNodeType.For)
                        {
                            QilNode binding = node11[0];
                            if (!binding.XmlType.MaybeMany && (node12.NodeType == QilNodeType.XsltGenerateId))
                            {
                                QilNode node14 = node12[0];
                                if ((node14 == node11) && this.AllowReplace(XmlILOptimization.NormalizeIdEq, local0))
                                {
                                    QilNode variable = this.VisitFor(base.f.For(binding));
                                    return this.Replace(XmlILOptimization.NormalizeIdEq, local0, this.VisitNot(base.f.Not(this.VisitIsEmpty(base.f.IsEmpty(this.VisitFilter(base.f.Filter(variable, this.VisitIs(base.f.Is(node9, variable)))))))));
                                }
                            }
                        }
                    }
                }
            }
            if (this[XmlILOptimization.NormalizeIdEq] && (child.NodeType == QilNodeType.StrConcat))
            {
                QilNode node16 = child[1];
                if (node16.NodeType == QilNodeType.Loop)
                {
                    QilNode node17 = node16[0];
                    QilNode node18 = node16[1];
                    if (node17.NodeType == QilNodeType.For)
                    {
                        QilNode node19 = node17[0];
                        if (!node19.XmlType.MaybeMany && (node18.NodeType == QilNodeType.XsltGenerateId))
                        {
                            QilNode node20 = node18[0];
                            if ((node20 == node17) && (node2.NodeType == QilNodeType.XsltGenerateId))
                            {
                                QilNode node21 = node2[0];
                                if (node21.XmlType.IsSingleton && this.AllowReplace(XmlILOptimization.NormalizeIdEq, local0))
                                {
                                    QilNode node22 = this.VisitFor(base.f.For(node19));
                                    return this.Replace(XmlILOptimization.NormalizeIdEq, local0, this.VisitNot(base.f.Not(this.VisitIsEmpty(base.f.IsEmpty(this.VisitFilter(base.f.Filter(node22, this.VisitIs(base.f.Is(node21, node22)))))))));
                                }
                            }
                        }
                    }
                }
            }
            if (this[XmlILOptimization.NormalizeMuenchian] && (child.NodeType == QilNodeType.Length))
            {
                QilNode node23 = child[0];
                if (node23.NodeType == QilNodeType.Union)
                {
                    QilNode node24 = node23[0];
                    QilNode node25 = node23[1];
                    if ((node24.XmlType.IsSingleton && !node25.XmlType.MaybeMany) && (node2.NodeType == QilNodeType.LiteralInt32))
                    {
                        int num = (int) ((QilLiteral) node2).Value;
                        if ((num == 1) && this.AllowReplace(XmlILOptimization.NormalizeMuenchian, local0))
                        {
                            QilNode node26 = this.VisitFor(base.f.For(node25));
                            return this.Replace(XmlILOptimization.NormalizeMuenchian, local0, this.VisitIsEmpty(base.f.IsEmpty(this.VisitFilter(base.f.Filter(node26, this.VisitNot(base.f.Not(this.VisitIs(base.f.Is(node24, node26)))))))));
                        }
                    }
                }
            }
            if (this[XmlILOptimization.NormalizeMuenchian] && (child.NodeType == QilNodeType.Length))
            {
                QilNode node27 = child[0];
                if (node27.NodeType == QilNodeType.Union)
                {
                    QilNode node28 = node27[0];
                    QilNode node29 = node27[1];
                    if ((!node28.XmlType.MaybeMany && node29.XmlType.IsSingleton) && (node2.NodeType == QilNodeType.LiteralInt32))
                    {
                        int num2 = (int) ((QilLiteral) node2).Value;
                        if ((num2 == 1) && this.AllowReplace(XmlILOptimization.NormalizeMuenchian, local0))
                        {
                            QilNode node30 = this.VisitFor(base.f.For(node28));
                            return this.Replace(XmlILOptimization.NormalizeMuenchian, local0, this.VisitIsEmpty(base.f.IsEmpty(this.VisitFilter(base.f.Filter(node30, this.VisitNot(base.f.Not(this.VisitIs(base.f.Is(node30, node29)))))))));
                        }
                    }
                }
            }
            if ((this[XmlILOptimization.AnnotateMaxLengthEq] && (child.NodeType == QilNodeType.Length)) && (node2.NodeType == QilNodeType.LiteralInt32))
            {
                int arg = (int) ((QilLiteral) node2).Value;
                if (this.AllowReplace(XmlILOptimization.AnnotateMaxLengthEq, local0))
                {
                    OptimizerPatterns.Write(child).AddPattern(OptimizerPatternName.MaxPosition);
                    OptimizerPatterns.Write(child).AddArgument(OptimizerPatternArgument.ElementQName, arg);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitError(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitFilter(QilLoop local0)
        {
            QilNode variable = local0[0];
            QilNode body = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (body.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitLoop(base.f.Loop(variable, body)));
            }
            if ((this[XmlILOptimization.EliminateFilter] && !OptimizerPatterns.Read(variable).MatchesPattern(OptimizerPatternName.MaybeSideEffects)) && ((body.NodeType == QilNodeType.False) && this.AllowReplace(XmlILOptimization.EliminateFilter, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateFilter, local0, this.VisitSequence(base.f.Sequence()));
            }
            if ((this[XmlILOptimization.EliminateFilter] && (body.NodeType == QilNodeType.True)) && this.AllowReplace(XmlILOptimization.EliminateFilter, local0))
            {
                return this.Replace(XmlILOptimization.EliminateFilter, local0, variable[0]);
            }
            if (this[XmlILOptimization.NormalizeAttribute] && (variable.NodeType == QilNodeType.For))
            {
                QilNode node3 = variable[0];
                if (node3.NodeType == QilNodeType.Content)
                {
                    QilNode left = node3[0];
                    if (body.NodeType == QilNodeType.And)
                    {
                        QilNode node5 = body[0];
                        QilNode node6 = body[1];
                        if (node5.NodeType == QilNodeType.IsType)
                        {
                            QilNode node7 = node5[0];
                            QilNode node8 = node5[1];
                            if ((node7 == variable) && (node8.NodeType == QilNodeType.LiteralType))
                            {
                                XmlQueryType type = (XmlQueryType) ((QilLiteral) node8).Value;
                                if ((type == XmlQueryTypeFactory.Attribute) && (node6.NodeType == QilNodeType.Eq))
                                {
                                    QilNode node9 = node6[0];
                                    QilNode right = node6[1];
                                    if (node9.NodeType == QilNodeType.NameOf)
                                    {
                                        QilNode node11 = node9[0];
                                        if (((node11 == variable) && (right.NodeType == QilNodeType.LiteralQName)) && this.AllowReplace(XmlILOptimization.NormalizeAttribute, local0))
                                        {
                                            return this.Replace(XmlILOptimization.NormalizeAttribute, local0, this.VisitAttribute(base.f.Attribute(left, right)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (this[XmlILOptimization.CommuteFilterLoop] && (variable.NodeType == QilNodeType.For))
            {
                QilNode nd = variable[0];
                if (nd.NodeType == QilNodeType.Loop)
                {
                    QilNode node13 = nd[0];
                    QilNode binding = nd[1];
                    if ((this.NonPositional(body, variable) && !this.IsDocOrderDistinct(nd)) && this.AllowReplace(XmlILOptimization.CommuteFilterLoop, local0))
                    {
                        QilNode node15 = this.VisitFor(base.f.For(binding));
                        return this.Replace(XmlILOptimization.CommuteFilterLoop, local0, this.VisitLoop(base.f.Loop(node13, this.VisitFilter(base.f.Filter(node15, this.Subs(body, variable, node15))))));
                    }
                }
            }
            if (((this[XmlILOptimization.NormalizeLoopInvariant] && !OptimizerPatterns.Read(variable).MatchesPattern(OptimizerPatternName.MaybeSideEffects)) && ((variable[0].NodeType != QilNodeType.OptimizeBarrier) && !this.DependsOn(body, variable))) && (!OptimizerPatterns.Read(body).MatchesPattern(OptimizerPatternName.MaybeSideEffects) && this.AllowReplace(XmlILOptimization.NormalizeLoopInvariant, local0)))
            {
                return this.Replace(XmlILOptimization.NormalizeLoopInvariant, local0, this.VisitConditional(base.f.Conditional(body, variable[0], this.VisitSequence(base.f.Sequence()))));
            }
            if (this[XmlILOptimization.AnnotateMaxPositionEq] && (body.NodeType == QilNodeType.Eq))
            {
                QilNode node16 = body[0];
                QilNode node17 = body[1];
                if (node16.NodeType == QilNodeType.PositionOf)
                {
                    QilNode node18 = node16[0];
                    if ((node18 == variable) && (node17.NodeType == QilNodeType.LiteralInt32))
                    {
                        int arg = (int) ((QilLiteral) node17).Value;
                        if (this.AllowReplace(XmlILOptimization.AnnotateMaxPositionEq, local0))
                        {
                            OptimizerPatterns.Write(variable).AddPattern(OptimizerPatternName.MaxPosition);
                            OptimizerPatterns.Write(variable).AddArgument(OptimizerPatternArgument.ElementQName, arg);
                        }
                    }
                }
            }
            if (this[XmlILOptimization.AnnotateMaxPositionLe] && (body.NodeType == QilNodeType.Le))
            {
                QilNode node19 = body[0];
                QilNode node20 = body[1];
                if (node19.NodeType == QilNodeType.PositionOf)
                {
                    QilNode node21 = node19[0];
                    if ((node21 == variable) && (node20.NodeType == QilNodeType.LiteralInt32))
                    {
                        int num2 = (int) ((QilLiteral) node20).Value;
                        if (this.AllowReplace(XmlILOptimization.AnnotateMaxPositionLe, local0))
                        {
                            OptimizerPatterns.Write(variable).AddPattern(OptimizerPatternName.MaxPosition);
                            OptimizerPatterns.Write(variable).AddArgument(OptimizerPatternArgument.ElementQName, num2);
                        }
                    }
                }
            }
            if (this[XmlILOptimization.AnnotateMaxPositionLt] && (body.NodeType == QilNodeType.Lt))
            {
                QilNode node22 = body[0];
                QilNode node23 = body[1];
                if (node22.NodeType == QilNodeType.PositionOf)
                {
                    QilNode node24 = node22[0];
                    if ((node24 == variable) && (node23.NodeType == QilNodeType.LiteralInt32))
                    {
                        int num3 = (int) ((QilLiteral) node23).Value;
                        if (this.AllowReplace(XmlILOptimization.AnnotateMaxPositionLt, local0))
                        {
                            OptimizerPatterns.Write(variable).AddPattern(OptimizerPatternName.MaxPosition);
                            OptimizerPatterns.Write(variable).AddArgument(OptimizerPatternArgument.ElementQName, num3 - 1);
                        }
                    }
                }
            }
            if (this[XmlILOptimization.AnnotateFilter] && (variable.NodeType == QilNodeType.For))
            {
                QilNode ndSrc = variable[0];
                if (this.AllowReplace(XmlILOptimization.AnnotateFilter, local0))
                {
                    OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.Step);
                    OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.IsDocOrderDistinct);
                    OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.SameDepth);
                }
            }
            if (this[XmlILOptimization.AnnotateFilterElements] && (variable.NodeType == QilNodeType.For))
            {
                QilNode node26 = variable[0];
                if (OptimizerPatterns.Read(node26).MatchesPattern(OptimizerPatternName.Axis) && (body.NodeType == QilNodeType.And))
                {
                    QilNode node27 = body[0];
                    QilNode node28 = body[1];
                    if (node27.NodeType == QilNodeType.IsType)
                    {
                        QilNode node29 = node27[0];
                        QilNode node30 = node27[1];
                        if ((node29 == variable) && (node30.NodeType == QilNodeType.LiteralType))
                        {
                            XmlQueryType type2 = (XmlQueryType) ((QilLiteral) node30).Value;
                            if ((type2 == XmlQueryTypeFactory.Element) && (node28.NodeType == QilNodeType.Eq))
                            {
                                QilNode node31 = node28[0];
                                QilNode node32 = node28[1];
                                if (node31.NodeType == QilNodeType.NameOf)
                                {
                                    QilNode node33 = node31[0];
                                    if (((node33 == variable) && (node32.NodeType == QilNodeType.LiteralQName)) && this.AllowReplace(XmlILOptimization.AnnotateFilterElements, local0))
                                    {
                                        OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.FilterElements);
                                        OptimizerPatterns.Write(local0).AddArgument(OptimizerPatternArgument.ElementQName, node32);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (this[XmlILOptimization.AnnotateFilterContentKind] && (variable.NodeType == QilNodeType.For))
            {
                QilNode node34 = variable[0];
                if (OptimizerPatterns.Read(node34).MatchesPattern(OptimizerPatternName.Axis) && (body.NodeType == QilNodeType.IsType))
                {
                    QilNode node35 = body[0];
                    QilNode node36 = body[1];
                    if ((node35 == variable) && (node36.NodeType == QilNodeType.LiteralType))
                    {
                        XmlQueryType typ = (XmlQueryType) ((QilLiteral) node36).Value;
                        if (this.MatchesContentTest(typ) && this.AllowReplace(XmlILOptimization.AnnotateFilterContentKind, local0))
                        {
                            OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.FilterContentKind);
                            OptimizerPatterns.Write(local0).AddArgument(OptimizerPatternArgument.ElementQName, typ);
                        }
                    }
                }
            }
            if (this[XmlILOptimization.AnnotateFilterAttributeKind] && (variable.NodeType == QilNodeType.For))
            {
                QilNode node37 = variable[0];
                if ((node37.NodeType == QilNodeType.Content) && (body.NodeType == QilNodeType.IsType))
                {
                    QilNode node38 = body[0];
                    QilNode node39 = body[1];
                    if ((node38 == variable) && (node39.NodeType == QilNodeType.LiteralType))
                    {
                        XmlQueryType type4 = (XmlQueryType) ((QilLiteral) node39).Value;
                        if ((type4 == XmlQueryTypeFactory.Attribute) && this.AllowReplace(XmlILOptimization.AnnotateFilterAttributeKind, local0))
                        {
                            OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.FilterAttributeKind);
                        }
                    }
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitFollowingSibling(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateFollowingSibling] && this.AllowReplace(XmlILOptimization.AnnotateFollowingSibling, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitFunction(QilFunction local0)
        {
            QilNode node = local0[0];
            QilNode nd = local0[1];
            QilNode node1 = local0[2];
            XmlQueryType xmlType = local0.XmlType;
            if (((local0.XmlType.IsSubtypeOf(XmlQueryTypeFactory.NodeS) && this[XmlILOptimization.AnnotateIndex1]) && ((node.Count == 2) && node[0].XmlType.IsSubtypeOf(XmlQueryTypeFactory.Node))) && ((node[1].XmlType == XmlQueryTypeFactory.StringX) && (nd.NodeType == QilNodeType.Filter)))
            {
                QilNode arg = nd[0];
                QilNode node4 = nd[1];
                if (arg.NodeType == QilNodeType.For)
                {
                    QilNode expr = arg[0];
                    if (node4.NodeType == QilNodeType.Not)
                    {
                        QilNode node6 = node4[0];
                        if (node6.NodeType == QilNodeType.IsEmpty)
                        {
                            QilNode node7 = node6[0];
                            if (node7.NodeType == QilNodeType.Filter)
                            {
                                QilNode node8 = node7[0];
                                QilNode node9 = node7[1];
                                if (node8.NodeType == QilNodeType.For)
                                {
                                    QilNode node10 = node8[0];
                                    if (node9.NodeType == QilNodeType.Eq)
                                    {
                                        QilNode node11 = node9[0];
                                        QilNode key = node9[1];
                                        if ((((node11 == node8) && (key.NodeType == QilNodeType.Parameter)) && ((key == node[1]) && this.IsDocOrderDistinct(nd))) && this.AllowReplace(XmlILOptimization.AnnotateIndex1, local0))
                                        {
                                            EqualityIndexVisitor visitor = new EqualityIndexVisitor();
                                            if (visitor.Scan(expr, node[0], key) && visitor.Scan(node10, node[0], key))
                                            {
                                                OptimizerPatterns patterns = OptimizerPatterns.Write(nd);
                                                patterns.AddPattern(OptimizerPatternName.EqualityIndex);
                                                patterns.AddArgument(OptimizerPatternArgument.StepNode, arg);
                                                patterns.AddArgument(OptimizerPatternArgument.StepInput, node10);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (((local0.XmlType.IsSubtypeOf(XmlQueryTypeFactory.NodeS) && this[XmlILOptimization.AnnotateIndex2]) && ((node.Count == 2) && (node[0].XmlType == XmlQueryTypeFactory.Node))) && ((node[1].XmlType == XmlQueryTypeFactory.StringX) && (nd.NodeType == QilNodeType.Filter)))
            {
                QilNode node13 = nd[0];
                QilNode node14 = nd[1];
                if (node13.NodeType == QilNodeType.For)
                {
                    QilNode node15 = node13[0];
                    if (node14.NodeType == QilNodeType.Eq)
                    {
                        QilNode node16 = node14[0];
                        QilNode node17 = node14[1];
                        if (((node17.NodeType == QilNodeType.Parameter) && (node17 == node[1])) && (this.IsDocOrderDistinct(nd) && this.AllowReplace(XmlILOptimization.AnnotateIndex2, local0)))
                        {
                            EqualityIndexVisitor visitor2 = new EqualityIndexVisitor();
                            if (visitor2.Scan(node15, node[0], node17) && visitor2.Scan(node16, node[0], node17))
                            {
                                OptimizerPatterns patterns2 = OptimizerPatterns.Write(nd);
                                patterns2.AddPattern(OptimizerPatternName.EqualityIndex);
                                patterns2.AddArgument(OptimizerPatternArgument.StepNode, node13);
                                patterns2.AddArgument(OptimizerPatternArgument.StepInput, node16);
                            }
                        }
                    }
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitGe(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateGe] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.EliminateGe, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateGe, local0, this.FoldComparison(QilNodeType.Ge, child, node2));
            }
            if ((this[XmlILOptimization.NormalizeGeLiteral] && this.IsLiteral(child)) && (!this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.NormalizeGeLiteral, local0)))
            {
                return this.Replace(XmlILOptimization.NormalizeGeLiteral, local0, this.VisitLe(base.f.Le(node2, child)));
            }
            if (this[XmlILOptimization.NormalizeXsltConvertGe] && (child.NodeType == QilNodeType.XsltConvert))
            {
                QilNode left = child[0];
                QilNode node4 = child[1];
                if (node4.NodeType == QilNodeType.LiteralType)
                {
                    XmlQueryType typ = (XmlQueryType) ((QilLiteral) node4).Value;
                    if (((this.IsPrimitiveNumeric(left.XmlType) && this.IsPrimitiveNumeric(typ)) && (this.IsLiteral(node2) && this.CanFoldXsltConvertNonLossy(node2, left.XmlType))) && this.AllowReplace(XmlILOptimization.NormalizeXsltConvertGe, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeXsltConvertGe, local0, this.VisitGe(base.f.Ge(left, this.FoldXsltConvert(node2, left.XmlType))));
                    }
                }
            }
            if ((this[XmlILOptimization.AnnotateMaxLengthGe] && (child.NodeType == QilNodeType.Length)) && (node2.NodeType == QilNodeType.LiteralInt32))
            {
                int arg = (int) ((QilLiteral) node2).Value;
                if (this.AllowReplace(XmlILOptimization.AnnotateMaxLengthGe, local0))
                {
                    OptimizerPatterns.Write(child).AddPattern(OptimizerPatternName.MaxPosition);
                    OptimizerPatterns.Write(child).AddArgument(OptimizerPatternArgument.ElementQName, arg);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitGt(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateGt] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.EliminateGt, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateGt, local0, this.FoldComparison(QilNodeType.Gt, child, node2));
            }
            if ((this[XmlILOptimization.NormalizeGtLiteral] && this.IsLiteral(child)) && (!this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.NormalizeGtLiteral, local0)))
            {
                return this.Replace(XmlILOptimization.NormalizeGtLiteral, local0, this.VisitLt(base.f.Lt(node2, child)));
            }
            if (this[XmlILOptimization.NormalizeXsltConvertGt] && (child.NodeType == QilNodeType.XsltConvert))
            {
                QilNode left = child[0];
                QilNode node4 = child[1];
                if (node4.NodeType == QilNodeType.LiteralType)
                {
                    XmlQueryType typ = (XmlQueryType) ((QilLiteral) node4).Value;
                    if (((this.IsPrimitiveNumeric(left.XmlType) && this.IsPrimitiveNumeric(typ)) && (this.IsLiteral(node2) && this.CanFoldXsltConvertNonLossy(node2, left.XmlType))) && this.AllowReplace(XmlILOptimization.NormalizeXsltConvertGt, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeXsltConvertGt, local0, this.VisitGt(base.f.Gt(left, this.FoldXsltConvert(node2, left.XmlType))));
                    }
                }
            }
            if (this[XmlILOptimization.NormalizeLengthGt] && (child.NodeType == QilNodeType.Length))
            {
                QilNode node5 = child[0];
                if (((node2.NodeType == QilNodeType.LiteralInt32) && (((int) ((QilLiteral) node2).Value) == 0)) && this.AllowReplace(XmlILOptimization.NormalizeLengthGt, local0))
                {
                    return this.Replace(XmlILOptimization.NormalizeLengthGt, local0, this.VisitNot(base.f.Not(this.VisitIsEmpty(base.f.IsEmpty(node5)))));
                }
            }
            if ((this[XmlILOptimization.AnnotateMaxLengthGt] && (child.NodeType == QilNodeType.Length)) && (node2.NodeType == QilNodeType.LiteralInt32))
            {
                int arg = (int) ((QilLiteral) node2).Value;
                if (this.AllowReplace(XmlILOptimization.AnnotateMaxLengthGt, local0))
                {
                    OptimizerPatterns.Write(child).AddPattern(OptimizerPatternName.MaxPosition);
                    OptimizerPatterns.Write(child).AddArgument(OptimizerPatternArgument.ElementQName, arg);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitIntersection(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateIntersection] && (node2 == child)) && this.AllowReplace(XmlILOptimization.EliminateIntersection, local0))
            {
                return this.Replace(XmlILOptimization.EliminateIntersection, local0, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(child)));
            }
            if ((this[XmlILOptimization.EliminateIntersection] && (child.NodeType == QilNodeType.Sequence)) && ((child.Count == 0) && this.AllowReplace(XmlILOptimization.EliminateIntersection, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateIntersection, local0, child);
            }
            if ((this[XmlILOptimization.EliminateIntersection] && (node2.NodeType == QilNodeType.Sequence)) && ((node2.Count == 0) && this.AllowReplace(XmlILOptimization.EliminateIntersection, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateIntersection, local0, node2);
            }
            if ((this[XmlILOptimization.EliminateIntersection] && (child.NodeType == QilNodeType.XmlContext)) && ((node2.NodeType == QilNodeType.XmlContext) && this.AllowReplace(XmlILOptimization.EliminateIntersection, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateIntersection, local0, child);
            }
            if ((this[XmlILOptimization.NormalizeIntersect] && (!this.IsDocOrderDistinct(child) || !this.IsDocOrderDistinct(node2))) && this.AllowReplace(XmlILOptimization.NormalizeIntersect, local0))
            {
                return this.Replace(XmlILOptimization.NormalizeIntersect, local0, this.VisitIntersection(base.f.Intersection(this.VisitDocOrderDistinct(base.f.DocOrderDistinct(child)), this.VisitDocOrderDistinct(base.f.DocOrderDistinct(node2)))));
            }
            if (this[XmlILOptimization.AnnotateIntersect] && this.AllowReplace(XmlILOptimization.AnnotateIntersect, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitInvoke(QilInvoke local0)
        {
            QilNode nd = local0[0];
            QilNode node1 = local0[1];
            if (this[XmlILOptimization.NormalizeInvokeEmpty] && (nd.NodeType == QilNodeType.Function))
            {
                QilNode node2 = nd[1];
                if (((node2.NodeType == QilNodeType.Sequence) && (node2.Count == 0)) && this.AllowReplace(XmlILOptimization.NormalizeInvokeEmpty, local0))
                {
                    return this.Replace(XmlILOptimization.NormalizeInvokeEmpty, local0, this.VisitSequence(base.f.Sequence()));
                }
            }
            if (this[XmlILOptimization.AnnotateTrackCallers] && this.AllowReplace(XmlILOptimization.AnnotateTrackCallers, local0))
            {
                XmlILConstructInfo.Write(nd).CallersInfo.Add(XmlILConstructInfo.Write(local0));
            }
            if (this[XmlILOptimization.AnnotateInvoke] && (nd.NodeType == QilNodeType.Function))
            {
                QilNode ndSrc = nd[1];
                if (this.AllowReplace(XmlILOptimization.AnnotateInvoke, local0))
                {
                    OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.IsDocOrderDistinct);
                    OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.SameDepth);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitIs(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateIs] && (node2 == child)) && this.AllowReplace(XmlILOptimization.EliminateIs, local0))
            {
                return this.Replace(XmlILOptimization.EliminateIs, local0, this.VisitTrue(base.f.True()));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitIsEmpty(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateIsEmpty] && (child.NodeType == QilNodeType.Sequence)) && ((child.Count == 0) && this.AllowReplace(XmlILOptimization.EliminateIsEmpty, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateIsEmpty, local0, this.VisitTrue(base.f.True()));
            }
            if ((this[XmlILOptimization.EliminateIsEmpty] && !child.XmlType.MaybeEmpty) && (!OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.MaybeSideEffects) && this.AllowReplace(XmlILOptimization.EliminateIsEmpty, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateIsEmpty, local0, this.VisitFalse(base.f.False()));
            }
            if ((this[XmlILOptimization.EliminateIsEmpty] && !child.XmlType.MaybeEmpty) && this.AllowReplace(XmlILOptimization.EliminateIsEmpty, local0))
            {
                return this.Replace(XmlILOptimization.EliminateIsEmpty, local0, this.VisitLoop(base.f.Loop(this.VisitLet(base.f.Let(child)), this.VisitFalse(base.f.False()))));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitIsType(QilTargetType local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateIsType] && !OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.MaybeSideEffects)) && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType baseType = (XmlQueryType) ((QilLiteral) node2).Value;
                if (child.XmlType.IsSubtypeOf(baseType) && this.AllowReplace(XmlILOptimization.EliminateIsType, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateIsType, local0, this.VisitTrue(base.f.True()));
                }
            }
            if ((this[XmlILOptimization.EliminateIsType] && !OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.MaybeSideEffects)) && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType type2 = (XmlQueryType) ((QilLiteral) node2).Value;
                if (child.XmlType.NeverSubtypeOf(type2) && this.AllowReplace(XmlILOptimization.EliminateIsType, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateIsType, local0, this.VisitFalse(base.f.False()));
                }
            }
            if (this[XmlILOptimization.EliminateIsType] && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType type3 = (XmlQueryType) ((QilLiteral) node2).Value;
                if (child.XmlType.Prime.NeverSubtypeOf(type3.Prime) && this.AllowReplace(XmlILOptimization.EliminateIsType, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateIsType, local0, this.VisitIsEmpty(base.f.IsEmpty(child)));
                }
            }
            if ((this[XmlILOptimization.EliminateIsType] && OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.MaybeSideEffects)) && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType type4 = (XmlQueryType) ((QilLiteral) node2).Value;
                if (child.XmlType.IsSubtypeOf(type4) && this.AllowReplace(XmlILOptimization.EliminateIsType, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateIsType, local0, this.VisitLoop(base.f.Loop(this.VisitLet(base.f.Let(child)), this.VisitTrue(base.f.True()))));
                }
            }
            if ((this[XmlILOptimization.EliminateIsType] && OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.MaybeSideEffects)) && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType type5 = (XmlQueryType) ((QilLiteral) node2).Value;
                if (child.XmlType.NeverSubtypeOf(type5) && this.AllowReplace(XmlILOptimization.EliminateIsType, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateIsType, local0, this.VisitLoop(base.f.Loop(this.VisitLet(base.f.Let(child)), this.VisitFalse(base.f.False()))));
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitLe(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateLe] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.EliminateLe, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateLe, local0, this.FoldComparison(QilNodeType.Le, child, node2));
            }
            if ((this[XmlILOptimization.NormalizeLeLiteral] && this.IsLiteral(child)) && (!this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.NormalizeLeLiteral, local0)))
            {
                return this.Replace(XmlILOptimization.NormalizeLeLiteral, local0, this.VisitGe(base.f.Ge(node2, child)));
            }
            if (this[XmlILOptimization.NormalizeXsltConvertLe] && (child.NodeType == QilNodeType.XsltConvert))
            {
                QilNode left = child[0];
                QilNode node4 = child[1];
                if (node4.NodeType == QilNodeType.LiteralType)
                {
                    XmlQueryType typ = (XmlQueryType) ((QilLiteral) node4).Value;
                    if (((this.IsPrimitiveNumeric(left.XmlType) && this.IsPrimitiveNumeric(typ)) && (this.IsLiteral(node2) && this.CanFoldXsltConvertNonLossy(node2, left.XmlType))) && this.AllowReplace(XmlILOptimization.NormalizeXsltConvertLe, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeXsltConvertLe, local0, this.VisitLe(base.f.Le(left, this.FoldXsltConvert(node2, left.XmlType))));
                    }
                }
            }
            if ((this[XmlILOptimization.AnnotateMaxLengthLe] && (child.NodeType == QilNodeType.Length)) && (node2.NodeType == QilNodeType.LiteralInt32))
            {
                int arg = (int) ((QilLiteral) node2).Value;
                if (this.AllowReplace(XmlILOptimization.AnnotateMaxLengthLe, local0))
                {
                    OptimizerPatterns.Write(child).AddPattern(OptimizerPatternName.MaxPosition);
                    OptimizerPatterns.Write(child).AddArgument(OptimizerPatternArgument.ElementQName, arg);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitLength(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateLength] && (child.NodeType == QilNodeType.Sequence)) && ((child.Count == 0) && this.AllowReplace(XmlILOptimization.EliminateLength, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateLength, local0, this.VisitLiteralInt32(base.f.LiteralInt32(0)));
            }
            if ((this[XmlILOptimization.EliminateLength] && child.XmlType.IsSingleton) && (!OptimizerPatterns.Read(child).MatchesPattern(OptimizerPatternName.MaybeSideEffects) && this.AllowReplace(XmlILOptimization.EliminateLength, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateLength, local0, this.VisitLiteralInt32(base.f.LiteralInt32(1)));
            }
            if (((this[XmlILOptimization.IntroducePrecedingDod] && !this.IsDocOrderDistinct(child)) && (this.IsStepPattern(child, QilNodeType.XPathPreceding) || this.IsStepPattern(child, QilNodeType.PrecedingSibling))) && this.AllowReplace(XmlILOptimization.IntroducePrecedingDod, local0))
            {
                return this.Replace(XmlILOptimization.IntroducePrecedingDod, local0, this.VisitLength(base.f.Length(this.VisitDocOrderDistinct(base.f.DocOrderDistinct(child)))));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitLet(QilIterator local0)
        {
            QilNode ndSrc = local0[0];
            if ((local0.XmlType.IsSingleton && !this.IsGlobalVariable(local0)) && (this[XmlILOptimization.NormalizeSingletonLet] && this.AllowReplace(XmlILOptimization.NormalizeSingletonLet, local0)))
            {
                local0.NodeType = QilNodeType.For;
                this.VisitFor(local0);
            }
            if (this[XmlILOptimization.AnnotateLet] && this.AllowReplace(XmlILOptimization.AnnotateLet, local0))
            {
                OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.Step);
                OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.IsDocOrderDistinct);
                OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitLocalNameOf(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitLoop(QilLoop local0)
        {
            QilNode nd = local0[0];
            QilNode expr = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (nd.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(nd[0])));
            }
            if (this[XmlILOptimization.EliminateIterator] && (nd.NodeType == QilNodeType.For))
            {
                QilNode refNew = nd[0];
                if (((refNew.NodeType == QilNodeType.For) && !OptimizerPatterns.Read(nd).MatchesPattern(OptimizerPatternName.IsPositional)) && this.AllowReplace(XmlILOptimization.EliminateIterator, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateIterator, local0, this.Subs(expr, nd, refNew));
                }
            }
            if (this[XmlILOptimization.EliminateLoop] && (nd.NodeType == QilNodeType.For))
            {
                QilNode node4 = nd[0];
                if (((node4.NodeType == QilNodeType.Sequence) && (node4.Count == 0)) && this.AllowReplace(XmlILOptimization.EliminateLoop, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateLoop, local0, this.VisitSequence(base.f.Sequence()));
                }
            }
            if (((this[XmlILOptimization.EliminateLoop] && !OptimizerPatterns.Read(nd).MatchesPattern(OptimizerPatternName.MaybeSideEffects)) && ((expr.NodeType == QilNodeType.Sequence) && (expr.Count == 0))) && this.AllowReplace(XmlILOptimization.EliminateLoop, local0))
            {
                return this.Replace(XmlILOptimization.EliminateLoop, local0, this.VisitSequence(base.f.Sequence()));
            }
            if ((this[XmlILOptimization.EliminateLoop] && (expr == nd)) && this.AllowReplace(XmlILOptimization.EliminateLoop, local0))
            {
                return this.Replace(XmlILOptimization.EliminateLoop, local0, nd[0]);
            }
            if (this[XmlILOptimization.NormalizeLoopText] && (nd.NodeType == QilNodeType.For))
            {
                QilNode node5 = nd[0];
                if (node5.XmlType.IsSingleton && (expr.NodeType == QilNodeType.TextCtor))
                {
                    QilNode body = expr[0];
                    if (this.AllowReplace(XmlILOptimization.NormalizeLoopText, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeLoopText, local0, this.VisitTextCtor(base.f.TextCtor(this.VisitLoop(base.f.Loop(nd, body)))));
                    }
                }
            }
            if ((this[XmlILOptimization.EliminateIteratorUsedAtMostOnce] && ((nd.NodeType == QilNodeType.Let) || nd[0].XmlType.IsSingleton)) && ((!OptimizerPatterns.Read(nd).MatchesPattern(OptimizerPatternName.MaybeSideEffects) && (this.nodeCounter.Count(expr, nd) <= 1)) && (!OptimizerPatterns.Read(expr).MatchesPattern(OptimizerPatternName.MaybeSideEffects) && this.AllowReplace(XmlILOptimization.EliminateIteratorUsedAtMostOnce, local0))))
            {
                return this.Replace(XmlILOptimization.EliminateIteratorUsedAtMostOnce, local0, this.Subs(expr, nd, nd[0]));
            }
            if (this[XmlILOptimization.NormalizeLoopConditional] && (expr.NodeType == QilNodeType.Conditional))
            {
                QilNode child = expr[0];
                QilNode node8 = expr[1];
                QilNode node9 = expr[2];
                if (((node8.NodeType == QilNodeType.Sequence) && (node8.Count == 0)) && ((node9 == nd) && this.AllowReplace(XmlILOptimization.NormalizeLoopConditional, local0)))
                {
                    return this.Replace(XmlILOptimization.NormalizeLoopConditional, local0, this.VisitFilter(base.f.Filter(nd, this.VisitNot(base.f.Not(child)))));
                }
            }
            if (this[XmlILOptimization.NormalizeLoopConditional] && (expr.NodeType == QilNodeType.Conditional))
            {
                QilNode node10 = expr[0];
                QilNode node11 = expr[1];
                QilNode node12 = expr[2];
                if (((node11 == nd) && (node12.NodeType == QilNodeType.Sequence)) && ((node12.Count == 0) && this.AllowReplace(XmlILOptimization.NormalizeLoopConditional, local0)))
                {
                    return this.Replace(XmlILOptimization.NormalizeLoopConditional, local0, this.VisitFilter(base.f.Filter(nd, node10)));
                }
            }
            if ((this[XmlILOptimization.NormalizeLoopConditional] && (nd.NodeType == QilNodeType.For)) && (expr.NodeType == QilNodeType.Conditional))
            {
                QilNode node13 = expr[0];
                QilNode node14 = expr[1];
                QilNode node15 = expr[2];
                if (((node14.NodeType == QilNodeType.Sequence) && (node14.Count == 0)) && (this.NonPositional(node15, nd) && this.AllowReplace(XmlILOptimization.NormalizeLoopConditional, local0)))
                {
                    QilNode variable = this.VisitFor(base.f.For(this.VisitFilter(base.f.Filter(nd, this.VisitNot(base.f.Not(node13))))));
                    return this.Replace(XmlILOptimization.NormalizeLoopConditional, local0, this.VisitLoop(base.f.Loop(variable, this.Subs(node15, nd, variable))));
                }
            }
            if ((this[XmlILOptimization.NormalizeLoopConditional] && (nd.NodeType == QilNodeType.For)) && (expr.NodeType == QilNodeType.Conditional))
            {
                QilNode node17 = expr[0];
                QilNode node18 = expr[1];
                QilNode node19 = expr[2];
                if ((this.NonPositional(node18, nd) && (node19.NodeType == QilNodeType.Sequence)) && ((node19.Count == 0) && this.AllowReplace(XmlILOptimization.NormalizeLoopConditional, local0)))
                {
                    QilNode node20 = this.VisitFor(base.f.For(this.VisitFilter(base.f.Filter(nd, node17))));
                    return this.Replace(XmlILOptimization.NormalizeLoopConditional, local0, this.VisitLoop(base.f.Loop(node20, this.Subs(node18, nd, node20))));
                }
            }
            if (this[XmlILOptimization.NormalizeLoopLoop] && (expr.NodeType == QilNodeType.Loop))
            {
                QilNode iter = expr[0];
                QilNode node22 = expr[1];
                if (iter.NodeType == QilNodeType.For)
                {
                    QilNode node23 = iter[0];
                    if ((!this.DependsOn(node22, nd) && this.NonPositional(node22, iter)) && this.AllowReplace(XmlILOptimization.NormalizeLoopLoop, local0))
                    {
                        QilNode node24 = this.VisitFor(base.f.For(this.VisitLoop(base.f.Loop(nd, node23))));
                        return this.Replace(XmlILOptimization.NormalizeLoopLoop, local0, this.VisitLoop(base.f.Loop(node24, this.Subs(node22, iter, node24))));
                    }
                }
            }
            if (this[XmlILOptimization.AnnotateSingletonLoop] && (nd.NodeType == QilNodeType.For))
            {
                QilNode node25 = nd[0];
                if (!node25.XmlType.MaybeMany && this.AllowReplace(XmlILOptimization.AnnotateSingletonLoop, local0))
                {
                    OptimizerPatterns.Inherit(expr, local0, OptimizerPatternName.IsDocOrderDistinct);
                    OptimizerPatterns.Inherit(expr, local0, OptimizerPatternName.SameDepth);
                }
            }
            if ((this[XmlILOptimization.AnnotateRootLoop] && this.IsStepPattern(expr, QilNodeType.Root)) && this.AllowReplace(XmlILOptimization.AnnotateRootLoop, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
            }
            if (this[XmlILOptimization.AnnotateContentLoop] && (nd.NodeType == QilNodeType.For))
            {
                QilNode node26 = nd[0];
                if ((OptimizerPatterns.Read(node26).MatchesPattern(OptimizerPatternName.SameDepth) && (this.IsStepPattern(expr, QilNodeType.Content) || this.IsStepPattern(expr, QilNodeType.Union))) && ((nd == OptimizerPatterns.Read(expr).GetArgument(OptimizerPatternArgument.StepInput)) && this.AllowReplace(XmlILOptimization.AnnotateContentLoop, local0)))
                {
                    OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
                    OptimizerPatterns.Inherit(node26, local0, OptimizerPatternName.IsDocOrderDistinct);
                }
            }
            if (this[XmlILOptimization.AnnotateAttrNmspLoop] && (nd.NodeType == QilNodeType.For))
            {
                QilNode ndSrc = nd[0];
                if (((this.IsStepPattern(expr, QilNodeType.Attribute) || this.IsStepPattern(expr, QilNodeType.XPathNamespace)) || OptimizerPatterns.Read(expr).MatchesPattern(OptimizerPatternName.FilterAttributeKind)) && ((nd == OptimizerPatterns.Read(expr).GetArgument(OptimizerPatternArgument.StepInput)) && this.AllowReplace(XmlILOptimization.AnnotateAttrNmspLoop, local0)))
                {
                    OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.SameDepth);
                    OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.IsDocOrderDistinct);
                }
            }
            if (this[XmlILOptimization.AnnotateDescendantLoop] && (nd.NodeType == QilNodeType.For))
            {
                QilNode node28 = nd[0];
                if ((OptimizerPatterns.Read(node28).MatchesPattern(OptimizerPatternName.SameDepth) && (this.IsStepPattern(expr, QilNodeType.Descendant) || this.IsStepPattern(expr, QilNodeType.DescendantOrSelf))) && ((nd == OptimizerPatterns.Read(expr).GetArgument(OptimizerPatternArgument.StepInput)) && this.AllowReplace(XmlILOptimization.AnnotateDescendantLoop, local0)))
                {
                    OptimizerPatterns.Inherit(node28, local0, OptimizerPatternName.IsDocOrderDistinct);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitLt(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateLt] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.EliminateLt, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateLt, local0, this.FoldComparison(QilNodeType.Lt, child, node2));
            }
            if ((this[XmlILOptimization.NormalizeLtLiteral] && this.IsLiteral(child)) && (!this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.NormalizeLtLiteral, local0)))
            {
                return this.Replace(XmlILOptimization.NormalizeLtLiteral, local0, this.VisitGt(base.f.Gt(node2, child)));
            }
            if (this[XmlILOptimization.NormalizeXsltConvertLt] && (child.NodeType == QilNodeType.XsltConvert))
            {
                QilNode left = child[0];
                QilNode node4 = child[1];
                if (node4.NodeType == QilNodeType.LiteralType)
                {
                    XmlQueryType typ = (XmlQueryType) ((QilLiteral) node4).Value;
                    if (((this.IsPrimitiveNumeric(left.XmlType) && this.IsPrimitiveNumeric(typ)) && (this.IsLiteral(node2) && this.CanFoldXsltConvertNonLossy(node2, left.XmlType))) && this.AllowReplace(XmlILOptimization.NormalizeXsltConvertLt, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeXsltConvertLt, local0, this.VisitLt(base.f.Lt(left, this.FoldXsltConvert(node2, left.XmlType))));
                    }
                }
            }
            if ((this[XmlILOptimization.AnnotateMaxLengthLt] && (child.NodeType == QilNodeType.Length)) && (node2.NodeType == QilNodeType.LiteralInt32))
            {
                int arg = (int) ((QilLiteral) node2).Value;
                if (this.AllowReplace(XmlILOptimization.AnnotateMaxLengthLt, local0))
                {
                    OptimizerPatterns.Write(child).AddPattern(OptimizerPatternName.MaxPosition);
                    OptimizerPatterns.Write(child).AddArgument(OptimizerPatternArgument.ElementQName, arg);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitMaximum(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateMaximum] && (child.XmlType.Cardinality == XmlQueryCardinality.Zero)) && this.AllowReplace(XmlILOptimization.EliminateMaximum, local0))
            {
                return this.Replace(XmlILOptimization.EliminateMaximum, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitMinimum(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateMinimum] && (child.XmlType.Cardinality == XmlQueryCardinality.Zero)) && this.AllowReplace(XmlILOptimization.EliminateMinimum, local0))
            {
                return this.Replace(XmlILOptimization.EliminateMinimum, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitModulo(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (((this[XmlILOptimization.EliminateModulo] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.CanFoldArithmetic(QilNodeType.Modulo, (QilLiteral) child, (QilLiteral) node2))) && this.AllowReplace(XmlILOptimization.EliminateModulo, local0))
            {
                return this.Replace(XmlILOptimization.EliminateModulo, local0, this.FoldArithmetic(QilNodeType.Modulo, (QilLiteral) child, (QilLiteral) node2));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitMultiply(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (((this[XmlILOptimization.EliminateMultiply] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.CanFoldArithmetic(QilNodeType.Multiply, (QilLiteral) child, (QilLiteral) node2))) && this.AllowReplace(XmlILOptimization.EliminateMultiply, local0))
            {
                return this.Replace(XmlILOptimization.EliminateMultiply, local0, this.FoldArithmetic(QilNodeType.Multiply, (QilLiteral) child, (QilLiteral) node2));
            }
            if ((this[XmlILOptimization.NormalizeMultiplyLiteral] && this.IsLiteral(child)) && (!this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.NormalizeMultiplyLiteral, local0)))
            {
                return this.Replace(XmlILOptimization.NormalizeMultiplyLiteral, local0, this.VisitMultiply(base.f.Multiply(node2, child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitNameOf(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitNamespaceDecl(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((XmlILConstructInfo.Read(local0).IsNamespaceInScope && this[XmlILOptimization.EliminateNamespaceDecl]) && this.AllowReplace(XmlILOptimization.EliminateNamespaceDecl, local0))
            {
                return this.Replace(XmlILOptimization.EliminateNamespaceDecl, local0, this.VisitSequence(base.f.Sequence()));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                this.contentAnalyzer.Analyze(local0, null);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitNamespaceUriOf(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitNe(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateNe] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.EliminateNe, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateNe, local0, this.FoldComparison(QilNodeType.Ne, child, node2));
            }
            if ((this[XmlILOptimization.NormalizeNeLiteral] && this.IsLiteral(child)) && (!this.IsLiteral(node2) && this.AllowReplace(XmlILOptimization.NormalizeNeLiteral, local0)))
            {
                return this.Replace(XmlILOptimization.NormalizeNeLiteral, local0, this.VisitNe(base.f.Ne(node2, child)));
            }
            if (this[XmlILOptimization.NormalizeXsltConvertNe] && (child.NodeType == QilNodeType.XsltConvert))
            {
                QilNode left = child[0];
                QilNode node4 = child[1];
                if (node4.NodeType == QilNodeType.LiteralType)
                {
                    XmlQueryType typ = (XmlQueryType) ((QilLiteral) node4).Value;
                    if (((this.IsPrimitiveNumeric(left.XmlType) && this.IsPrimitiveNumeric(typ)) && (this.IsLiteral(node2) && this.CanFoldXsltConvertNonLossy(node2, left.XmlType))) && this.AllowReplace(XmlILOptimization.NormalizeXsltConvertNe, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeXsltConvertNe, local0, this.VisitNe(base.f.Ne(left, this.FoldXsltConvert(node2, left.XmlType))));
                    }
                }
            }
            if (this[XmlILOptimization.NormalizeIdNe] && (child.NodeType == QilNodeType.XsltGenerateId))
            {
                QilNode node5 = child[0];
                if (node5.XmlType.IsSingleton && (node2.NodeType == QilNodeType.XsltGenerateId))
                {
                    QilNode right = node2[0];
                    if (right.XmlType.IsSingleton && this.AllowReplace(XmlILOptimization.NormalizeIdNe, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeIdNe, local0, this.VisitNot(base.f.Not(this.VisitIs(base.f.Is(node5, right)))));
                    }
                }
            }
            if (this[XmlILOptimization.NormalizeLengthNe] && (child.NodeType == QilNodeType.Length))
            {
                QilNode node7 = child[0];
                if (((node2.NodeType == QilNodeType.LiteralInt32) && (((int) ((QilLiteral) node2).Value) == 0)) && this.AllowReplace(XmlILOptimization.NormalizeLengthNe, local0))
                {
                    return this.Replace(XmlILOptimization.NormalizeLengthNe, local0, this.VisitNot(base.f.Not(this.VisitIsEmpty(base.f.IsEmpty(node7)))));
                }
            }
            if ((this[XmlILOptimization.AnnotateMaxLengthNe] && (child.NodeType == QilNodeType.Length)) && (node2.NodeType == QilNodeType.LiteralInt32))
            {
                int arg = (int) ((QilLiteral) node2).Value;
                if (this.AllowReplace(XmlILOptimization.AnnotateMaxLengthNe, local0))
                {
                    OptimizerPatterns.Write(child).AddPattern(OptimizerPatternName.MaxPosition);
                    OptimizerPatterns.Write(child).AddArgument(OptimizerPatternArgument.ElementQName, arg);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitNegate(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.EliminateNegate] && (child.NodeType == QilNodeType.LiteralDecimal))
            {
                decimal num = (decimal) ((QilLiteral) child).Value;
                if (this.AllowReplace(XmlILOptimization.EliminateNegate, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateNegate, local0, this.VisitLiteralDecimal(base.f.LiteralDecimal(-num)));
                }
            }
            if (this[XmlILOptimization.EliminateNegate] && (child.NodeType == QilNodeType.LiteralDouble))
            {
                double num2 = (double) ((QilLiteral) child).Value;
                if (this.AllowReplace(XmlILOptimization.EliminateNegate, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateNegate, local0, this.VisitLiteralDouble(base.f.LiteralDouble(-num2)));
                }
            }
            if (this[XmlILOptimization.EliminateNegate] && (child.NodeType == QilNodeType.LiteralInt32))
            {
                int num3 = (int) ((QilLiteral) child).Value;
                if (this.AllowReplace(XmlILOptimization.EliminateNegate, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateNegate, local0, this.VisitLiteralInt32(base.f.LiteralInt32(-num3)));
                }
            }
            if (this[XmlILOptimization.EliminateNegate] && (child.NodeType == QilNodeType.LiteralInt64))
            {
                long num4 = (long) ((QilLiteral) child).Value;
                if (this.AllowReplace(XmlILOptimization.EliminateNegate, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateNegate, local0, this.VisitLiteralInt64(base.f.LiteralInt64(-num4)));
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitNodeRange(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (this[XmlILOptimization.AnnotateNodeRange] && this.AllowReplace(XmlILOptimization.AnnotateNodeRange, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitNop(QilUnary local0)
        {
            QilNode replacement = local0[0];
            if (this[XmlILOptimization.EliminateNop] && this.AllowReplace(XmlILOptimization.EliminateNop, local0))
            {
                return this.Replace(XmlILOptimization.EliminateNop, local0, replacement);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitNot(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateNot] && (child.NodeType == QilNodeType.True)) && this.AllowReplace(XmlILOptimization.EliminateNot, local0))
            {
                return this.Replace(XmlILOptimization.EliminateNot, local0, this.VisitFalse(base.f.False()));
            }
            if ((this[XmlILOptimization.EliminateNot] && (child.NodeType == QilNodeType.False)) && this.AllowReplace(XmlILOptimization.EliminateNot, local0))
            {
                return this.Replace(XmlILOptimization.EliminateNot, local0, this.VisitTrue(base.f.True()));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitOptimizeBarrier(QilUnary local0)
        {
            QilNode ndSrc = local0[0];
            if (this[XmlILOptimization.AnnotateBarrier] && this.AllowReplace(XmlILOptimization.AnnotateBarrier, local0))
            {
                OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.IsDocOrderDistinct);
                OptimizerPatterns.Inherit(ndSrc, local0, OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitOr(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateOr] && (child.NodeType == QilNodeType.True)) && this.AllowReplace(XmlILOptimization.EliminateOr, local0))
            {
                return this.Replace(XmlILOptimization.EliminateOr, local0, child);
            }
            if ((this[XmlILOptimization.EliminateOr] && (child.NodeType == QilNodeType.False)) && this.AllowReplace(XmlILOptimization.EliminateOr, local0))
            {
                return this.Replace(XmlILOptimization.EliminateOr, local0, node2);
            }
            if ((this[XmlILOptimization.EliminateOr] && (node2.NodeType == QilNodeType.True)) && this.AllowReplace(XmlILOptimization.EliminateOr, local0))
            {
                return this.Replace(XmlILOptimization.EliminateOr, local0, node2);
            }
            if ((this[XmlILOptimization.EliminateOr] && (node2.NodeType == QilNodeType.False)) && this.AllowReplace(XmlILOptimization.EliminateOr, local0))
            {
                return this.Replace(XmlILOptimization.EliminateOr, local0, child);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitParent(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateParent] && this.AllowReplace(XmlILOptimization.AnnotateParent, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitPICtor(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                local0.Right = this.contentAnalyzer.Analyze(local0, node2);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitPositionOf(QilUnary local0)
        {
            QilNode nd = local0[0];
            if ((this[XmlILOptimization.EliminatePositionOf] && (nd.NodeType != QilNodeType.For)) && this.AllowReplace(XmlILOptimization.EliminatePositionOf, local0))
            {
                return this.Replace(XmlILOptimization.EliminatePositionOf, local0, this.VisitLiteralInt32(base.f.LiteralInt32(1)));
            }
            if (this[XmlILOptimization.EliminatePositionOf] && (nd.NodeType == QilNodeType.For))
            {
                QilNode node2 = nd[0];
                if (node2.XmlType.IsSingleton && this.AllowReplace(XmlILOptimization.EliminatePositionOf, local0))
                {
                    return this.Replace(XmlILOptimization.EliminatePositionOf, local0, this.VisitLiteralInt32(base.f.LiteralInt32(1)));
                }
            }
            if (this[XmlILOptimization.AnnotatePositionalIterator] && this.AllowReplace(XmlILOptimization.AnnotatePositionalIterator, local0))
            {
                OptimizerPatterns.Write(nd).AddPattern(OptimizerPatternName.IsPositional);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitPreceding(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotatePreceding] && this.AllowReplace(XmlILOptimization.AnnotatePreceding, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitPrecedingSibling(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotatePrecedingSibling] && this.AllowReplace(XmlILOptimization.AnnotatePrecedingSibling, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitPrefixOf(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitQilExpression(QilExpression local0)
        {
            QilNode node1 = local0[0];
            if (this[XmlILOptimization.EliminateUnusedFunctions] && this.AllowReplace(XmlILOptimization.EliminateUnusedFunctions, local0))
            {
                IList<QilNode> functionList = local0.FunctionList;
                for (int i = functionList.Count - 1; i >= 0; i--)
                {
                    if (XmlILConstructInfo.Write(functionList[i]).CallersInfo.Count == 0)
                    {
                        functionList.RemoveAt(i);
                    }
                }
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                foreach (QilFunction function in local0.FunctionList)
                {
                    if (this.IsConstructedExpression(function.Definition))
                    {
                        function.Definition = this.contentAnalyzer.Analyze(function, function.Definition);
                    }
                }
                local0.Root = this.contentAnalyzer.Analyze(null, local0.Root);
                XmlILConstructInfo.Write(local0.Root).PushToWriterLast = true;
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitRawTextCtor(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                this.contentAnalyzer.Analyze(local0, null);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitReference(QilNode oldNode)
        {
            QilNode original = this.subs.FindReplacement(oldNode);
            if (original == null)
            {
                original = oldNode;
            }
            if ((this[XmlILOptimization.FoldConstant] && (original != null)) && ((original.NodeType == QilNodeType.Let) || (original.NodeType == QilNodeType.For)))
            {
                QilNode binding = ((QilIterator) oldNode).Binding;
                if (this.IsLiteral(binding))
                {
                    return this.Replace(XmlILOptimization.FoldConstant, original, binding.ShallowClone(base.f));
                }
            }
            return base.VisitReference(original);
        }

        protected override QilNode VisitRoot(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateRoot] && this.AllowReplace(XmlILOptimization.AnnotateRoot, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitRtfCtor(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node1 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                local0.Left = this.contentAnalyzer.Analyze(local0, child);
            }
            if (this[XmlILOptimization.AnnotateSingleTextRtf] && (child.NodeType == QilNodeType.TextCtor))
            {
                QilNode arg = child[0];
                if (this.AllowReplace(XmlILOptimization.AnnotateSingleTextRtf, local0))
                {
                    OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SingleTextRtf);
                    OptimizerPatterns.Write(local0).AddArgument(OptimizerPatternArgument.ElementQName, arg);
                    XmlILConstructInfo.Write(local0).PullFromIteratorFirst = true;
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitSequence(QilList local0)
        {
            if (((local0.Count == 1) && this[XmlILOptimization.EliminateSequence]) && this.AllowReplace(XmlILOptimization.EliminateSequence, local0))
            {
                return this.Replace(XmlILOptimization.EliminateSequence, local0, local0[0]);
            }
            if ((!this.HasNestedSequence(local0) || !this[XmlILOptimization.NormalizeNestedSequences]) || !this.AllowReplace(XmlILOptimization.NormalizeNestedSequences, local0))
            {
                return this.NoReplace(local0);
            }
            QilNode replacement = this.VisitSequence(base.f.Sequence());
            foreach (QilNode node2 in local0)
            {
                if (node2.NodeType == QilNodeType.Sequence)
                {
                    replacement.Add((IList<QilNode>) node2);
                }
                else
                {
                    replacement.Add(node2);
                }
            }
            replacement = this.VisitSequence((QilList) replacement);
            return this.Replace(XmlILOptimization.NormalizeNestedSequences, local0, replacement);
        }

        protected override QilNode VisitSort(QilLoop local0)
        {
            QilNode node = local0[0];
            QilNode node1 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (node.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node[0])));
            }
            if (this[XmlILOptimization.EliminateSort] && (node.NodeType == QilNodeType.For))
            {
                QilNode child = node[0];
                if (child.XmlType.IsSingleton && this.AllowReplace(XmlILOptimization.EliminateSort, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateSort, local0, this.VisitNop(base.f.Nop(child)));
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitSortKey(QilSortKey local0)
        {
            QilNode node = local0[0];
            QilNode collation = local0[1];
            if (this[XmlILOptimization.NormalizeSortXsltConvert] && (node.NodeType == QilNodeType.XsltConvert))
            {
                QilNode key = node[0];
                QilNode node4 = node[1];
                if (node4.NodeType == QilNodeType.LiteralType)
                {
                    XmlQueryType type = (XmlQueryType) ((QilLiteral) node4).Value;
                    if (((key.XmlType == XmlQueryTypeFactory.IntX) && (type == XmlQueryTypeFactory.DoubleX)) && this.AllowReplace(XmlILOptimization.NormalizeSortXsltConvert, local0))
                    {
                        return this.Replace(XmlILOptimization.NormalizeSortXsltConvert, local0, this.VisitSortKey(base.f.SortKey(key, collation)));
                    }
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitStrConcat(QilStrConcat local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((node2.XmlType.IsSingleton && this[XmlILOptimization.EliminateStrConcatSingle]) && this.AllowReplace(XmlILOptimization.EliminateStrConcatSingle, local0))
            {
                return this.Replace(XmlILOptimization.EliminateStrConcatSingle, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (this[XmlILOptimization.EliminateStrConcat] && (child.NodeType == QilNodeType.LiteralString))
            {
                string str = (string) ((QilLiteral) child).Value;
                if (((node2.NodeType == QilNodeType.Sequence) && this.AreLiteralArgs(node2)) && this.AllowReplace(XmlILOptimization.EliminateStrConcat, local0))
                {
                    StringConcat concat = new StringConcat {
                        Delimiter = str
                    };
                    foreach (QilLiteral literal in node2)
                    {
                        concat.Concat((string) literal);
                    }
                    return this.Replace(XmlILOptimization.EliminateStrConcat, local0, this.VisitLiteralString(base.f.LiteralString(concat.GetResult())));
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitStrLength(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.EliminateStrLength] && (child.NodeType == QilNodeType.LiteralString))
            {
                string str = (string) ((QilLiteral) child).Value;
                if (this.AllowReplace(XmlILOptimization.EliminateStrLength, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateStrLength, local0, this.VisitLiteralInt32(base.f.LiteralInt32(str.Length)));
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitStrParseQName(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitSubtract(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (((this[XmlILOptimization.EliminateSubtract] && this.IsLiteral(child)) && (this.IsLiteral(node2) && this.CanFoldArithmetic(QilNodeType.Subtract, (QilLiteral) child, (QilLiteral) node2))) && this.AllowReplace(XmlILOptimization.EliminateSubtract, local0))
            {
                return this.Replace(XmlILOptimization.EliminateSubtract, local0, this.FoldArithmetic(QilNodeType.Subtract, (QilLiteral) child, (QilLiteral) node2));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitSum(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.EliminateSum] && (child.XmlType.Cardinality == XmlQueryCardinality.Zero)) && this.AllowReplace(XmlILOptimization.EliminateSum, local0))
            {
                return this.Replace(XmlILOptimization.EliminateSum, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitTextCtor(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                this.contentAnalyzer.Analyze(local0, null);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitTypeAssert(QilTargetType local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.EliminateTypeAssert] && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType baseType = (XmlQueryType) ((QilLiteral) node2).Value;
                if (child.XmlType.NeverSubtypeOf(baseType) && this.AllowReplace(XmlILOptimization.EliminateTypeAssert, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateTypeAssert, local0, this.VisitError(base.f.Error(this.VisitLiteralString(base.f.LiteralString(string.Empty)))));
                }
            }
            if (this[XmlILOptimization.EliminateTypeAssert] && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType type2 = (XmlQueryType) ((QilLiteral) node2).Value;
                if (child.XmlType.Prime.NeverSubtypeOf(type2.Prime) && this.AllowReplace(XmlILOptimization.EliminateTypeAssert, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateTypeAssert, local0, this.VisitConditional(base.f.Conditional(this.VisitIsEmpty(base.f.IsEmpty(child)), this.VisitSequence(base.f.Sequence()), this.VisitError(base.f.Error(this.VisitLiteralString(base.f.LiteralString(string.Empty)))))));
                }
            }
            if (this[XmlILOptimization.EliminateTypeAssertOptional] && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType type3 = (XmlQueryType) ((QilLiteral) node2).Value;
                if (child.XmlType.IsSubtypeOf(type3) && this.AllowReplace(XmlILOptimization.EliminateTypeAssertOptional, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateTypeAssertOptional, local0, child);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitUnion(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if ((this[XmlILOptimization.EliminateUnion] && (node2 == child)) && this.AllowReplace(XmlILOptimization.EliminateUnion, local0))
            {
                return this.Replace(XmlILOptimization.EliminateUnion, local0, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(child)));
            }
            if ((this[XmlILOptimization.EliminateUnion] && (child.NodeType == QilNodeType.Sequence)) && ((child.Count == 0) && this.AllowReplace(XmlILOptimization.EliminateUnion, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateUnion, local0, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(node2)));
            }
            if ((this[XmlILOptimization.EliminateUnion] && (node2.NodeType == QilNodeType.Sequence)) && ((node2.Count == 0) && this.AllowReplace(XmlILOptimization.EliminateUnion, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateUnion, local0, this.VisitDocOrderDistinct(base.f.DocOrderDistinct(child)));
            }
            if ((this[XmlILOptimization.EliminateUnion] && (child.NodeType == QilNodeType.XmlContext)) && ((node2.NodeType == QilNodeType.XmlContext) && this.AllowReplace(XmlILOptimization.EliminateUnion, local0)))
            {
                return this.Replace(XmlILOptimization.EliminateUnion, local0, child);
            }
            if ((this[XmlILOptimization.NormalizeUnion] && (!this.IsDocOrderDistinct(child) || !this.IsDocOrderDistinct(node2))) && this.AllowReplace(XmlILOptimization.NormalizeUnion, local0))
            {
                return this.Replace(XmlILOptimization.NormalizeUnion, local0, this.VisitUnion(base.f.Union(this.VisitDocOrderDistinct(base.f.DocOrderDistinct(child)), this.VisitDocOrderDistinct(base.f.DocOrderDistinct(node2)))));
            }
            if (this[XmlILOptimization.AnnotateUnion] && this.AllowReplace(XmlILOptimization.AnnotateUnion, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
            }
            if (((this[XmlILOptimization.AnnotateUnionContent] && (this.IsStepPattern(child, QilNodeType.Content) || this.IsStepPattern(child, QilNodeType.Union))) && (this.IsStepPattern(node2, QilNodeType.Content) || this.IsStepPattern(node2, QilNodeType.Union))) && ((OptimizerPatterns.Read(child).GetArgument(OptimizerPatternArgument.StepInput) == OptimizerPatterns.Read(node2).GetArgument(OptimizerPatternArgument.StepInput)) && this.AllowReplace(XmlILOptimization.AnnotateUnionContent, local0)))
            {
                this.AddStepPattern(local0, (QilNode) OptimizerPatterns.Read(child).GetArgument(OptimizerPatternArgument.StepInput));
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitWarning(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitXPathFollowing(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateXPathFollowing] && this.AllowReplace(XmlILOptimization.AnnotateXPathFollowing, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitXPathNamespace(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateNamespace] && this.AllowReplace(XmlILOptimization.AnnotateNamespace, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.IsDocOrderDistinct);
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.SameDepth);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitXPathNodeValue(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitXPathPreceding(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateXPathPreceding] && this.AllowReplace(XmlILOptimization.AnnotateXPathPreceding, local0))
            {
                OptimizerPatterns.Write(local0).AddPattern(OptimizerPatternName.Axis);
                this.AddStepPattern(local0, child);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitXsltConvert(QilTargetType local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldXsltConvertLiteral] && this.IsLiteral(child)) && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType typTarget = (XmlQueryType) ((QilLiteral) node2).Value;
                if (this.CanFoldXsltConvert(child, typTarget) && this.AllowReplace(XmlILOptimization.FoldXsltConvertLiteral, local0))
                {
                    return this.Replace(XmlILOptimization.FoldXsltConvertLiteral, local0, this.FoldXsltConvert(child, typTarget));
                }
            }
            if (this[XmlILOptimization.EliminateXsltConvert] && (node2.NodeType == QilNodeType.LiteralType))
            {
                XmlQueryType type2 = (XmlQueryType) ((QilLiteral) node2).Value;
                if ((child.XmlType == type2) && this.AllowReplace(XmlILOptimization.EliminateXsltConvert, local0))
                {
                    return this.Replace(XmlILOptimization.EliminateXsltConvert, local0, child);
                }
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitXsltCopy(QilBinary local0)
        {
            QilNode child = local0[0];
            QilNode node2 = local0[1];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if ((this[XmlILOptimization.FoldNone] && (node2.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(node2)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                local0.Right = this.contentAnalyzer.Analyze(local0, node2);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitXsltCopyOf(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            if (this[XmlILOptimization.AnnotateConstruction] && this.AllowReplace(XmlILOptimization.AnnotateConstruction, local0))
            {
                this.contentAnalyzer.Analyze(local0, null);
            }
            return this.NoReplace(local0);
        }

        protected override QilNode VisitXsltGenerateId(QilUnary local0)
        {
            QilNode child = local0[0];
            if ((this[XmlILOptimization.FoldNone] && (child.XmlType == XmlQueryTypeFactory.None)) && this.AllowReplace(XmlILOptimization.FoldNone, local0))
            {
                return this.Replace(XmlILOptimization.FoldNone, local0, this.VisitNop(base.f.Nop(child)));
            }
            return this.NoReplace(local0);
        }

        private bool this[XmlILOptimization ann] =>
            base.Patterns.IsSet((int) ann);

        private class EqualityIndexVisitor : QilVisitor
        {
            protected QilNode ctxt;
            protected QilNode key;
            protected bool result;

            public bool Scan(QilNode expr, QilNode ctxt, QilNode key)
            {
                this.result = true;
                this.ctxt = ctxt;
                this.key = key;
                this.Visit(expr);
                return this.result;
            }

            protected override QilNode VisitReference(QilNode expr)
            {
                if (this.result && ((expr == this.key) || (expr == this.ctxt)))
                {
                    this.result = false;
                }
                return expr;
            }

            protected override QilNode VisitRoot(QilUnary root)
            {
                if (root.Child == this.ctxt)
                {
                    return root;
                }
                return this.VisitChildren(root);
            }
        }

        private class NodeCounter : QilVisitor
        {
            protected int cnt;
            protected QilNode target;

            public int Count(QilNode expr, QilNode target)
            {
                this.cnt = 0;
                this.target = target;
                this.Visit(expr);
                return this.cnt;
            }

            protected override QilNode Visit(QilNode n)
            {
                if (n == null)
                {
                    return null;
                }
                if (n == this.target)
                {
                    this.cnt++;
                }
                return this.VisitChildren(n);
            }

            protected override QilNode VisitReference(QilNode n)
            {
                if (n == this.target)
                {
                    this.cnt++;
                }
                return n;
            }
        }

        private class NodeFinder : QilVisitor
        {
            protected QilNode parent;
            protected bool result;
            protected QilNode target;

            public bool Find(QilNode expr, QilNode target)
            {
                this.result = false;
                this.target = target;
                this.parent = null;
                this.VisitAssumeReference(expr);
                return this.result;
            }

            protected virtual bool OnFound(QilNode expr) => 
                true;

            protected override QilNode Visit(QilNode expr)
            {
                if (!this.result)
                {
                    if (expr == this.target)
                    {
                        this.result = this.OnFound(expr);
                    }
                    if (!this.result)
                    {
                        QilNode parent = this.parent;
                        this.parent = expr;
                        this.VisitChildren(expr);
                        this.parent = parent;
                    }
                }
                return expr;
            }

            protected override QilNode VisitReference(QilNode expr)
            {
                if (expr == this.target)
                {
                    this.result = this.OnFound(expr);
                }
                return expr;
            }
        }

        private class PositionOfFinder : XmlILOptimizerVisitor.NodeFinder
        {
            protected override bool OnFound(QilNode expr) => 
                ((base.parent != null) && (base.parent.NodeType == QilNodeType.PositionOf));
        }
    }
}

