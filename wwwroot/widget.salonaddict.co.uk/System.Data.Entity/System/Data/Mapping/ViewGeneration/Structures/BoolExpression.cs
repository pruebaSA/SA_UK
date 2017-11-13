namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class BoolExpression : InternalBase
    {
        private static readonly CopyVisitor CopyVisitorInstance = new CopyVisitor();
        internal static readonly IEqualityComparer<BoolExpression> EqualityComparer = new BoolComparer();
        internal static readonly BoolExpression False = new BoolExpression(false);
        private Converter<DomainConstraint<BoolLiteral, CellConstant>> m_converter;
        private readonly MemberDomainMap m_memberDomainMap;
        private BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> m_tree;
        internal static readonly BoolExpression True = new BoolExpression(true);

        private BoolExpression(bool isTrue)
        {
            if (isTrue)
            {
                this.m_tree = TrueExpr<DomainConstraint<BoolLiteral, CellConstant>>.Value;
            }
            else
            {
                this.m_tree = FalseExpr<DomainConstraint<BoolLiteral, CellConstant>>.Value;
            }
        }

        internal BoolExpression(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expr, MemberDomainMap memberDomainMap)
        {
            this.m_tree = expr;
            this.m_memberDomainMap = memberDomainMap;
        }

        private BoolExpression(ExprType opType, IEnumerable<BoolExpression> children)
        {
            List<BoolExpression> nodes = new List<BoolExpression>(children);
            foreach (BoolExpression expression in children)
            {
                if (expression.m_memberDomainMap != null)
                {
                    this.m_memberDomainMap = expression.m_memberDomainMap;
                    break;
                }
            }
            switch (opType)
            {
                case ExprType.And:
                    this.m_tree = new AndExpr<DomainConstraint<BoolLiteral, CellConstant>>(this.ToBoolExprList(nodes));
                    return;

                case ExprType.Not:
                    this.m_tree = new NotExpr<DomainConstraint<BoolLiteral, CellConstant>>(nodes[0].m_tree);
                    return;

                case ExprType.Or:
                    this.m_tree = new OrExpr<DomainConstraint<BoolLiteral, CellConstant>>(this.ToBoolExprList(nodes));
                    return;
            }
        }

        internal static List<BoolExpression> AddConjunctionToBools(List<BoolExpression> bools, BoolExpression conjunct)
        {
            List<BoolExpression> list = new List<BoolExpression>();
            foreach (BoolExpression expression in bools)
            {
                if (expression == null)
                {
                    list.Add(null);
                }
                else
                {
                    list.Add(CreateAnd(new BoolExpression[] { expression, conjunct }));
                }
            }
            return list;
        }

        internal StringBuilder AsCql(StringBuilder builder, string blockAlias) => 
            AsCqlVisitor.AsCql(this.m_tree, builder, blockAlias);

        internal StringBuilder AsUserString(StringBuilder builder, string blockAlias)
        {
            builder.AppendLine(System.Data.Entity.Strings.Viewgen_ConfigurationErrorMsg(blockAlias));
            builder.Append("  ");
            return AsUserStringVisitor.AsUserString(this.m_tree, builder, blockAlias);
        }

        internal BoolExpression Create(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
            new BoolExpression(expression, this.m_memberDomainMap);

        internal BoolExpression Create(BoolLiteral literal) => 
            new BoolExpression(literal.GetDomainBoolExpression(this.m_memberDomainMap), this.m_memberDomainMap);

        internal static BoolExpression CreateAnd(params BoolExpression[] children) => 
            new BoolExpression(ExprType.And, children);

        internal static BoolExpression CreateAndNot(BoolExpression e1, BoolExpression e2) => 
            CreateAnd(new BoolExpression[] { e1, CreateNot(e2) });

        internal static BoolExpression CreateLiteral(BoolLiteral literal, MemberDomainMap memberDomainMap) => 
            new BoolExpression(literal.GetDomainBoolExpression(memberDomainMap), memberDomainMap);

        internal static BoolExpression CreateNot(BoolExpression expression) => 
            new BoolExpression(ExprType.Not, new BoolExpression[] { expression });

        internal static BoolExpression CreateOr(params BoolExpression[] children) => 
            new BoolExpression(ExprType.Or, children);

        internal void ExpensiveSimplify()
        {
            if (!this.IsFinal())
            {
                this.m_tree = this.m_tree.Simplify();
            }
            else
            {
                this.InitializeConverter();
                this.m_tree = this.m_tree.ExpensiveSimplify(out this.m_converter);
                this.FixDomainMap(this.m_memberDomainMap);
            }
        }

        internal void FixDomainMap(MemberDomainMap domainMap)
        {
            this.m_tree = FixRangeVisitor.FixRange(this.m_tree, domainMap);
        }

        internal static BoolLiteral GetBoolLiteral(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> term) => 
            term.Identifier.Variable.Identifier;

        internal virtual void GetRequiredSlots(MemberPathMapBase projectedSlotMap, bool[] requiredSlots)
        {
            RequiredSlotsVisitor.GetRequiredSlots(this.m_tree, projectedSlotMap, requiredSlots);
        }

        private void InitializeConverter()
        {
            if (this.m_converter == null)
            {
                this.m_converter = new Converter<DomainConstraint<BoolLiteral, CellConstant>>(this.m_tree, IdentifierService<DomainConstraint<BoolLiteral, CellConstant>>.Instance.CreateConversionContext());
            }
        }

        internal bool IsAlwaysTrue()
        {
            this.InitializeConverter();
            return this.m_converter.Vertex.IsOne();
        }

        private bool IsFinal() => 
            ((this.m_memberDomainMap != null) && IsFinalVisitor.IsFinal(this.m_tree));

        internal bool IsSatisfiable() => 
            !this.IsUnsatisfiable();

        internal bool IsUnsatisfiable()
        {
            this.InitializeConverter();
            return this.m_converter.Vertex.IsZero();
        }

        internal BoolExpression MakeCopy() => 
            this.Create(this.m_tree.Accept<BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>>(CopyVisitorInstance));

        internal BoolExpression RemapBool(Dictionary<JoinTreeNode, JoinTreeNode> remap) => 
            new BoolExpression(RemapBoolVisitor.RemapJoinTreeNodes(this.m_tree, this.m_memberDomainMap, remap), this.m_memberDomainMap);

        internal static void RemapBools(List<BoolExpression> bools, Dictionary<JoinTreeNode, JoinTreeNode> remap)
        {
            for (int i = 0; i < bools.Count; i++)
            {
                if (bools[i] != null)
                {
                    bools[i] = bools[i].RemapBool(remap);
                }
            }
        }

        internal BoolExpression RemapLiterals(Dictionary<BoolLiteral, BoolLiteral> remap)
        {
            BooleanExpressionTermRewriter<DomainConstraint<BoolLiteral, CellConstant>, DomainConstraint<BoolLiteral, CellConstant>> visitor = new BooleanExpressionTermRewriter<DomainConstraint<BoolLiteral, CellConstant>, DomainConstraint<BoolLiteral, CellConstant>>(delegate (TermExpr<DomainConstraint<BoolLiteral, CellConstant>> term) {
                BoolLiteral literal;
                if (!remap.TryGetValue(GetBoolLiteral(term), out literal))
                {
                    return term;
                }
                return literal.GetDomainBoolExpression(this.m_memberDomainMap);
            });
            return new BoolExpression(this.m_tree.Accept<BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>>(visitor), this.m_memberDomainMap);
        }

        private IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>> ToBoolExprList(IEnumerable<BoolExpression> nodes)
        {
            foreach (BoolExpression iteratorVariable0 in nodes)
            {
                yield return iteratorVariable0.m_tree;
            }
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            CompactStringVisitor.ToBuilder(this.m_tree, builder);
        }

        internal BoolLiteral AsLiteral
        {
            get
            {
                TermExpr<DomainConstraint<BoolLiteral, CellConstant>> tree = this.m_tree as TermExpr<DomainConstraint<BoolLiteral, CellConstant>>;
                if (tree == null)
                {
                    return null;
                }
                return GetBoolLiteral(tree);
            }
        }

        internal IEnumerable<BoolExpression> Atoms
        {
            get
            {
                IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> terms = TermVisitor.GetTerms(this.m_tree, false);
                foreach (TermExpr<DomainConstraint<BoolLiteral, CellConstant>> iteratorVariable1 in terms)
                {
                    yield return new BoolExpression(iteratorVariable1, this.m_memberDomainMap);
                }
            }
        }

        internal bool IsFalse =>
            (this.m_tree.ExprType == ExprType.False);

        internal bool IsTrue =>
            (this.m_tree.ExprType == ExprType.True);

        internal IEnumerable<BoolLiteral> Leaves
        {
            get
            {
                IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> terms = TermVisitor.GetTerms(this.m_tree, true);
                foreach (TermExpr<DomainConstraint<BoolLiteral, CellConstant>> iteratorVariable1 in terms)
                {
                    yield return iteratorVariable1.Identifier.Variable.Identifier;
                }
            }
        }

        internal IEnumerable<OneOfConst> OneOfConstVariables
        {
            get
            {
                foreach (DomainVariable<BoolLiteral, CellConstant> iteratorVariable0 in this.Variables)
                {
                    OneOfConst identifier = iteratorVariable0.Identifier as OneOfConst;
                    if (identifier != null)
                    {
                        yield return identifier;
                    }
                }
            }
        }

        internal bool RepresentsAllTypeConditions =>
            this.OneOfConstVariables.All<OneOfConst>(var => (var is OneOfTypeConst));

        internal BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> Tree =>
            this.m_tree;

        internal IEnumerable<DomainConstraint<BoolLiteral, CellConstant>> VariableConstraints =>
            LeafVisitor<DomainConstraint<BoolLiteral, CellConstant>>.GetLeaves(this.m_tree);

        internal IEnumerable<DomainVariable<BoolLiteral, CellConstant>> Variables =>
            (from domainConstraint in this.VariableConstraints select domainConstraint.Variable);





        private class AsCqlVisitor : Visitor<DomainConstraint<BoolLiteral, CellConstant>, StringBuilder>
        {
            private string m_blockAlias;
            private StringBuilder m_builder;
            private bool m_canSkipIsNotNull;

            private AsCqlVisitor(StringBuilder builder, string blockAlias)
            {
                this.m_builder = builder;
                this.m_blockAlias = blockAlias;
                this.m_canSkipIsNotNull = true;
            }

            internal static StringBuilder AsCql(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, StringBuilder builder, string blockAlias)
            {
                BoolExpression.AsCqlVisitor visitor = new BoolExpression.AsCqlVisitor(builder, blockAlias);
                return expression.Accept<StringBuilder>(visitor);
            }

            internal override StringBuilder VisitAnd(AndExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitAndOr(expression, ExprType.And);

            private StringBuilder VisitAndOr(TreeExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, ExprType kind)
            {
                this.m_builder.Append('(');
                bool flag = true;
                foreach (BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expr in expression.Children)
                {
                    if (!flag)
                    {
                        if (kind == ExprType.And)
                        {
                            this.m_builder.Append(" AND ");
                        }
                        else
                        {
                            this.m_builder.Append(" OR ");
                        }
                    }
                    flag = false;
                    expr.Accept<StringBuilder>(this);
                }
                this.m_builder.Append(')');
                return this.m_builder;
            }

            internal override StringBuilder VisitFalse(FalseExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                this.m_builder.Append("False");
                return this.m_builder;
            }

            internal override StringBuilder VisitNot(NotExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                this.m_canSkipIsNotNull = false;
                this.m_builder.Append("NOT(");
                expression.Child.Accept<StringBuilder>(this);
                this.m_builder.Append(")");
                return this.m_builder;
            }

            internal override StringBuilder VisitOr(OrExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitAndOr(expression, ExprType.Or);

            internal override StringBuilder VisitTerm(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                BoolExpression.GetBoolLiteral(expression).AsCql(this.m_builder, this.m_blockAlias, this.m_canSkipIsNotNull);

            internal override StringBuilder VisitTrue(TrueExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                this.m_builder.Append("True");
                return this.m_builder;
            }
        }

        private class AsUserStringVisitor : Visitor<DomainConstraint<BoolLiteral, CellConstant>, StringBuilder>
        {
            private string m_blockAlias;
            private StringBuilder m_builder;
            private bool m_canSkipIsNotNull;

            private AsUserStringVisitor(StringBuilder builder, string blockAlias)
            {
                this.m_builder = builder;
                this.m_blockAlias = blockAlias;
                this.m_canSkipIsNotNull = true;
            }

            internal static StringBuilder AsUserString(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, StringBuilder builder, string blockAlias)
            {
                BoolExpression.AsUserStringVisitor visitor = new BoolExpression.AsUserStringVisitor(builder, blockAlias);
                return expression.Accept<StringBuilder>(visitor);
            }

            internal override StringBuilder VisitAnd(AndExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitAndOr(expression, ExprType.And);

            private StringBuilder VisitAndOr(TreeExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, ExprType kind)
            {
                this.m_builder.Append('(');
                bool flag = true;
                foreach (BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expr in expression.Children)
                {
                    if (!flag)
                    {
                        if (kind == ExprType.And)
                        {
                            this.m_builder.Append(" AND ");
                        }
                        else
                        {
                            this.m_builder.Append(" OR ");
                        }
                    }
                    flag = false;
                    expr.Accept<StringBuilder>(this);
                }
                this.m_builder.Append(')');
                return this.m_builder;
            }

            internal override StringBuilder VisitFalse(FalseExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                this.m_builder.Append("False");
                return this.m_builder;
            }

            internal override StringBuilder VisitNot(NotExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                this.m_canSkipIsNotNull = false;
                TermExpr<DomainConstraint<BoolLiteral, CellConstant>> child = expression.Child as TermExpr<DomainConstraint<BoolLiteral, CellConstant>>;
                if (child != null)
                {
                    return BoolExpression.GetBoolLiteral(child).AsNegatedUserString(this.m_builder, this.m_blockAlias, this.m_canSkipIsNotNull);
                }
                this.m_builder.Append("NOT(");
                expression.Child.Accept<StringBuilder>(this);
                this.m_builder.Append(")");
                return this.m_builder;
            }

            internal override StringBuilder VisitOr(OrExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitAndOr(expression, ExprType.Or);

            internal override StringBuilder VisitTerm(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                BoolLiteral boolLiteral = BoolExpression.GetBoolLiteral(expression);
                if ((boolLiteral is OneOfScalarConst) || (boolLiteral is OneOfTypeConst))
                {
                    return boolLiteral.AsUserString(this.m_builder, System.Data.Entity.Strings.ViewGen_EntityInstanceToken, this.m_canSkipIsNotNull);
                }
                return boolLiteral.AsUserString(this.m_builder, this.m_blockAlias, this.m_canSkipIsNotNull);
            }

            internal override StringBuilder VisitTrue(TrueExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                this.m_builder.Append("True");
                return this.m_builder;
            }
        }

        private class BoolComparer : IEqualityComparer<BoolExpression>
        {
            public bool Equals(BoolExpression left, BoolExpression right) => 
                (object.ReferenceEquals(left, right) || (((left != null) && (right != null)) && left.m_tree.Equals(right.m_tree)));

            public int GetHashCode(BoolExpression expression) => 
                expression.m_tree.GetHashCode();
        }

        private class CompactStringVisitor : Visitor<DomainConstraint<BoolLiteral, CellConstant>, StringBuilder>
        {
            private StringBuilder m_builder;

            private CompactStringVisitor(StringBuilder builder)
            {
                this.m_builder = builder;
            }

            internal static StringBuilder ToBuilder(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, StringBuilder builder)
            {
                BoolExpression.CompactStringVisitor visitor = new BoolExpression.CompactStringVisitor(builder);
                return expression.Accept<StringBuilder>(visitor);
            }

            internal override StringBuilder VisitAnd(AndExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitAndOr(expression, "AND");

            private StringBuilder VisitAndOr(TreeExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, string opAsString)
            {
                List<string> list = new List<string>();
                StringBuilder builder = this.m_builder;
                foreach (BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expr in expression.Children)
                {
                    this.m_builder = new StringBuilder();
                    expr.Accept<StringBuilder>(this);
                    list.Add(this.m_builder.ToString());
                }
                this.m_builder = builder;
                this.m_builder.Append('(');
                StringUtil.ToSeparatedStringSorted(this.m_builder, list, " " + opAsString + " ");
                this.m_builder.Append(')');
                return this.m_builder;
            }

            internal override StringBuilder VisitFalse(FalseExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                this.m_builder.Append("False");
                return this.m_builder;
            }

            internal override StringBuilder VisitNot(NotExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                this.m_builder.Append("NOT(");
                expression.Child.Accept<StringBuilder>(this);
                this.m_builder.Append(")");
                return this.m_builder;
            }

            internal override StringBuilder VisitOr(OrExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitAndOr(expression, "OR");

            internal override StringBuilder VisitTerm(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                BoolExpression.GetBoolLiteral(expression).ToCompactString(this.m_builder);
                return this.m_builder;
            }

            internal override StringBuilder VisitTrue(TrueExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                this.m_builder.Append("True");
                return this.m_builder;
            }
        }

        private class CopyVisitor : BasicVisitor<DomainConstraint<BoolLiteral, CellConstant>>
        {
        }

        private class FixRangeVisitor : BasicVisitor<DomainConstraint<BoolLiteral, CellConstant>>
        {
            private MemberDomainMap m_memberDomainMap;

            private FixRangeVisitor(MemberDomainMap memberDomainMap)
            {
                this.m_memberDomainMap = memberDomainMap;
            }

            internal static BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> FixRange(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, MemberDomainMap memberDomainMap)
            {
                BoolExpression.FixRangeVisitor visitor = new BoolExpression.FixRangeVisitor(memberDomainMap);
                return expression.Accept<BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>>(visitor);
            }

            internal override BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> VisitTerm(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                BoolExpression.GetBoolLiteral(expression).FixRange(expression.Identifier.Range, this.m_memberDomainMap);
        }

        private class IsFinalVisitor : Visitor<DomainConstraint<BoolLiteral, CellConstant>, bool>
        {
            internal static bool IsFinal(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                BoolExpression.IsFinalVisitor visitor = new BoolExpression.IsFinalVisitor();
                return expression.Accept<bool>(visitor);
            }

            internal override bool VisitAnd(AndExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitAndOr(expression);

            private bool VisitAndOr(TreeExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                bool flag = true;
                bool flag2 = true;
                foreach (BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expr in expression.Children)
                {
                    if (!(expr is FalseExpr<DomainConstraint<BoolLiteral, CellConstant>>) && !(expr is TrueExpr<DomainConstraint<BoolLiteral, CellConstant>>))
                    {
                        bool flag3 = expr.Accept<bool>(this);
                        if (flag)
                        {
                            flag2 = flag3;
                        }
                        flag = false;
                    }
                }
                return flag2;
            }

            internal override bool VisitFalse(FalseExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                true;

            internal override bool VisitNot(NotExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                expression.Child.Accept<bool>(this);

            internal override bool VisitOr(OrExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitAndOr(expression);

            internal override bool VisitTerm(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                OneOfConst boolLiteral = BoolExpression.GetBoolLiteral(expression) as OneOfConst;
                return ((boolLiteral == null) || boolLiteral.IsFullyDone);
            }

            internal override bool VisitTrue(TrueExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                true;
        }

        private class RemapBoolVisitor : BasicVisitor<DomainConstraint<BoolLiteral, CellConstant>>
        {
            private MemberDomainMap m_memberDomainMap;
            private Dictionary<JoinTreeNode, JoinTreeNode> m_remap;

            private RemapBoolVisitor(MemberDomainMap memberDomainMap, Dictionary<JoinTreeNode, JoinTreeNode> remap)
            {
                this.m_remap = remap;
                this.m_memberDomainMap = memberDomainMap;
            }

            internal static BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> RemapJoinTreeNodes(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, MemberDomainMap memberDomainMap, Dictionary<JoinTreeNode, JoinTreeNode> remap)
            {
                BoolExpression.RemapBoolVisitor visitor = new BoolExpression.RemapBoolVisitor(memberDomainMap, remap);
                return expression.Accept<BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>>(visitor);
            }

            internal override BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> VisitTerm(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                BoolExpression.GetBoolLiteral(expression).RemapBool(this.m_remap).GetDomainBoolExpression(this.m_memberDomainMap);
        }

        private class RequiredSlotsVisitor : BasicVisitor<DomainConstraint<BoolLiteral, CellConstant>>
        {
            private MemberPathMapBase m_projectedSlotMap;
            private bool[] m_requiredSlots;

            private RequiredSlotsVisitor(MemberPathMapBase projectedSlotMap, bool[] requiredSlots)
            {
                this.m_projectedSlotMap = projectedSlotMap;
                this.m_requiredSlots = requiredSlots;
            }

            internal static void GetRequiredSlots(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, MemberPathMapBase projectedSlotMap, bool[] requiredSlots)
            {
                BoolExpression.RequiredSlotsVisitor visitor = new BoolExpression.RequiredSlotsVisitor(projectedSlotMap, requiredSlots);
                expression.Accept<BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>>(visitor);
            }

            internal override BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> VisitTerm(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                BoolExpression.GetBoolLiteral(expression).GetRequiredSlots(this.m_projectedSlotMap, this.m_requiredSlots);
                return expression;
            }
        }

        private class TermVisitor : Visitor<DomainConstraint<BoolLiteral, CellConstant>, IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>>
        {
            private bool m_allowAllOperators;

            private TermVisitor(bool allowAllOperators)
            {
                this.m_allowAllOperators = allowAllOperators;
            }

            internal static IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> GetTerms(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> expression, bool allowAllOperators)
            {
                BoolExpression.TermVisitor visitor = new BoolExpression.TermVisitor(allowAllOperators);
                return expression.Accept<IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>>(visitor);
            }

            internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> VisitAnd(AndExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitTreeNode(expression);

            internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> VisitFalse(FalseExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                new <VisitFalse>d__25(-2) { <>4__this = this };

            internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> VisitNot(NotExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitTreeNode(expression);

            internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> VisitOr(OrExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this.VisitTreeNode(expression);

            internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> VisitTerm(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                yield return expression;
            }

            private IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> VisitTreeNode(TreeExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                foreach (BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> iteratorVariable0 in expression.Children)
                {
                    foreach (TermExpr<DomainConstraint<BoolLiteral, CellConstant>> iteratorVariable1 in iteratorVariable0.Accept<IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>>(this))
                    {
                        yield return iteratorVariable1;
                    }
                }
            }

            internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> VisitTrue(TrueExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                new <VisitTrue>d__22(-2) { <>4__this = this };

            [CompilerGenerated]
            private sealed class <VisitFalse>d__25 : IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>, IEnumerable, IEnumerator<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>, IEnumerator, IDisposable
            {
                private int <>1__state;
                private TermExpr<DomainConstraint<BoolLiteral, CellConstant>> <>2__current;
                public BoolExpression.TermVisitor <>4__this;
                private int <>l__initialThreadId;

                [DebuggerHidden]
                public <VisitFalse>d__25(int <>1__state)
                {
                    this.<>1__state = <>1__state;
                    this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
                }

                private bool MoveNext()
                {
                    if (this.<>1__state == 0)
                    {
                        this.<>1__state = -1;
                    }
                    return false;
                }

                [DebuggerHidden]
                IEnumerator<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>.GetEnumerator()
                {
                    if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                    {
                        this.<>1__state = 0;
                        return this;
                    }
                    return new BoolExpression.TermVisitor.<VisitFalse>d__25(0) { <>4__this = this.<>4__this };
                }

                [DebuggerHidden]
                IEnumerator IEnumerable.GetEnumerator() => 
                    this.System.Collections.Generic.IEnumerable<System.Data.Common.Utils.Boolean.TermExpr<System.Data.Common.Utils.Boolean.DomainConstraint<System.Data.Mapping.ViewGeneration.Structures.BoolLiteral,System.Data.Mapping.ViewGeneration.Structures.CellConstant>>>.GetEnumerator();

                [DebuggerHidden]
                void IEnumerator.Reset()
                {
                    throw new NotSupportedException();
                }

                void IDisposable.Dispose()
                {
                }

                TermExpr<DomainConstraint<BoolLiteral, CellConstant>> IEnumerator<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>.Current =>
                    this.<>2__current;

                object IEnumerator.Current =>
                    this.<>2__current;
            }



            [CompilerGenerated]
            private sealed class <VisitTrue>d__22 : IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>, IEnumerable, IEnumerator<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>, IEnumerator, IDisposable
            {
                private int <>1__state;
                private TermExpr<DomainConstraint<BoolLiteral, CellConstant>> <>2__current;
                public BoolExpression.TermVisitor <>4__this;
                private int <>l__initialThreadId;

                [DebuggerHidden]
                public <VisitTrue>d__22(int <>1__state)
                {
                    this.<>1__state = <>1__state;
                    this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
                }

                private bool MoveNext()
                {
                    if (this.<>1__state == 0)
                    {
                        this.<>1__state = -1;
                    }
                    return false;
                }

                [DebuggerHidden]
                IEnumerator<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>> IEnumerable<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>.GetEnumerator()
                {
                    if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                    {
                        this.<>1__state = 0;
                        return this;
                    }
                    return new BoolExpression.TermVisitor.<VisitTrue>d__22(0) { <>4__this = this.<>4__this };
                }

                [DebuggerHidden]
                IEnumerator IEnumerable.GetEnumerator() => 
                    this.System.Collections.Generic.IEnumerable<System.Data.Common.Utils.Boolean.TermExpr<System.Data.Common.Utils.Boolean.DomainConstraint<System.Data.Mapping.ViewGeneration.Structures.BoolLiteral,System.Data.Mapping.ViewGeneration.Structures.CellConstant>>>.GetEnumerator();

                [DebuggerHidden]
                void IEnumerator.Reset()
                {
                    throw new NotSupportedException();
                }

                void IDisposable.Dispose()
                {
                }

                TermExpr<DomainConstraint<BoolLiteral, CellConstant>> IEnumerator<TermExpr<DomainConstraint<BoolLiteral, CellConstant>>>.Current =>
                    this.<>2__current;

                object IEnumerator.Current =>
                    this.<>2__current;
            }
        }
    }
}

