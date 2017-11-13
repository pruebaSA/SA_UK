namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Globalization;

    internal class FragmentQueryProcessor : TileQueryProcessor<FragmentQuery>
    {
        private FragmentQueryKB _kb;

        public FragmentQueryProcessor(FragmentQueryKB kb)
        {
            this._kb = kb;
        }

        internal override FragmentQuery CreateDerivedViewBySelectingConstantAttributes(FragmentQuery view)
        {
            HashSet<MemberPath> attrs = new HashSet<MemberPath>();
            foreach (DomainVariable<BoolLiteral, CellConstant> variable in view.Condition.Variables)
            {
                OneOfConst identifier = variable.Identifier as OneOfConst;
                if (identifier != null)
                {
                    MemberPath memberPath = identifier.Slot.MemberPath;
                    CellConstantDomain values = identifier.Values;
                    if (!view.Attributes.Contains(memberPath))
                    {
                        foreach (CellConstant constant in values.Values)
                        {
                            DomainConstraint<BoolLiteral, CellConstant> constraint = new DomainConstraint<BoolLiteral, CellConstant>(variable, new Set<CellConstant>(new CellConstant[] { constant }, CellConstant.EqualityComparer));
                            BoolExpression condition = view.Condition.Create(new AndExpr<DomainConstraint<BoolLiteral, CellConstant>>(new BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>[] { view.Condition.Tree, new NotExpr<DomainConstraint<BoolLiteral, CellConstant>>(new TermExpr<DomainConstraint<BoolLiteral, CellConstant>>(constraint)) }));
                            if (!this.IsSatisfiable(condition))
                            {
                                attrs.Add(memberPath);
                            }
                        }
                    }
                }
            }
            if (attrs.Count > 0)
            {
                attrs.UnionWith(view.Attributes);
                return new FragmentQuery(string.Format(CultureInfo.InvariantCulture, "project({0})", new object[] { view.Description }), view.FromVariable, attrs, view.Condition);
            }
            return null;
        }

        internal override FragmentQuery Difference(FragmentQuery qA, FragmentQuery qB) => 
            FragmentQuery.Create(qA.Attributes, BoolExpression.CreateAndNot(qA.Condition, qB.Condition));

        internal override FragmentQuery Intersect(FragmentQuery q1, FragmentQuery q2)
        {
            HashSet<MemberPath> attrs = new HashSet<MemberPath>(q1.Attributes);
            attrs.IntersectWith(q2.Attributes);
            BoolExpression whereClause = BoolExpression.CreateAnd(new BoolExpression[] { q1.Condition, q2.Condition });
            return FragmentQuery.Create(attrs, whereClause);
        }

        internal bool IsContainedIn(FragmentQuery q1, FragmentQuery q2) => 
            !this.IsSatisfiable(this.Difference(q1, q2));

        internal bool IsDisjointFrom(FragmentQuery q1, FragmentQuery q2) => 
            !this.IsSatisfiable(this.Intersect(q1, q2));

        internal bool IsEquivalentTo(FragmentQuery q1, FragmentQuery q2) => 
            (this.IsContainedIn(q1, q2) && this.IsContainedIn(q2, q1));

        internal override bool IsSatisfiable(FragmentQuery query) => 
            this.IsSatisfiable(query.Condition);

        private bool IsSatisfiable(BoolExpression condition)
        {
            BoolExpression expression = condition.Create(new AndExpr<DomainConstraint<BoolLiteral, CellConstant>>(new BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>[] { this._kb.KbExpression, condition.Tree }));
            ConversionContext<DomainConstraint<BoolLiteral, CellConstant>> context = IdentifierService<DomainConstraint<BoolLiteral, CellConstant>>.Instance.CreateConversionContext();
            Converter<DomainConstraint<BoolLiteral, CellConstant>> converter = new Converter<DomainConstraint<BoolLiteral, CellConstant>>(expression.Tree, context);
            return !converter.Vertex.IsZero();
        }

        internal static FragmentQueryProcessor Merge(FragmentQueryProcessor qp1, FragmentQueryProcessor qp2)
        {
            FragmentQueryKB kb = new FragmentQueryKB();
            kb.AddKnowledgeBase(qp1.KnowledgeBase);
            kb.AddKnowledgeBase(qp2.KnowledgeBase);
            return new FragmentQueryProcessor(kb);
        }

        public override string ToString() => 
            this._kb.ToString();

        internal override FragmentQuery Union(FragmentQuery q1, FragmentQuery q2)
        {
            HashSet<MemberPath> attrs = new HashSet<MemberPath>(q1.Attributes);
            attrs.IntersectWith(q2.Attributes);
            BoolExpression whereClause = BoolExpression.CreateOr(new BoolExpression[] { q1.Condition, q2.Condition });
            return FragmentQuery.Create(attrs, whereClause);
        }

        internal FragmentQueryKB KnowledgeBase =>
            this._kb;

        private class AttributeSetComparator : IEqualityComparer<HashSet<MemberPath>>
        {
            internal static readonly FragmentQueryProcessor.AttributeSetComparator DefaultInstance = new FragmentQueryProcessor.AttributeSetComparator();

            public bool Equals(HashSet<MemberPath> x, HashSet<MemberPath> y) => 
                x.SetEquals(y);

            public int GetHashCode(HashSet<MemberPath> attrs)
            {
                int num = 0x7b;
                foreach (MemberPath path in attrs)
                {
                    num += MemberPath.EqualityComparer.GetHashCode(path) * 7;
                }
                return num;
            }
        }
    }
}

