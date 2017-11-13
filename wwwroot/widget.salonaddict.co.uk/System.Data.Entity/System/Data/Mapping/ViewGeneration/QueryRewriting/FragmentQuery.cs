namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Collections.Generic;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Text;

    internal class FragmentQuery : ITileQuery
    {
        private HashSet<MemberPath> m_attributes;
        private BoolExpression m_condition;
        private BoolExpression m_fromVariable;
        private string m_label;

        internal FragmentQuery(string label, BoolExpression fromVariable, IEnumerable<MemberPath> attrs, BoolExpression condition)
        {
            this.m_label = label;
            this.m_fromVariable = fromVariable;
            this.m_condition = condition;
            this.m_attributes = new HashSet<MemberPath>(attrs);
        }

        public static FragmentQuery Create(BoolExpression whereClause) => 
            new FragmentQuery(null, null, new MemberPath[0], whereClause);

        public static FragmentQuery Create(IEnumerable<MemberPath> attrs, BoolExpression whereClause) => 
            new FragmentQuery(null, null, attrs, whereClause);

        public static FragmentQuery Create(BoolExpression fromVariable, CellQuery cellQuery)
        {
            BoolExpression condition = cellQuery.WhereClause.MakeCopy();
            condition.ExpensiveSimplify();
            return new FragmentQuery(null, fromVariable, new HashSet<MemberPath>(cellQuery.GetProjectedMembers()), condition);
        }

        public static FragmentQuery Create(string label, RoleBoolean roleBoolean, CellQuery cellQuery)
        {
            BoolExpression condition = cellQuery.WhereClause.Create(roleBoolean);
            condition = BoolExpression.CreateAnd(new BoolExpression[] { condition, cellQuery.WhereClause }).MakeCopy();
            condition.ExpensiveSimplify();
            return new FragmentQuery(label, null, new HashSet<MemberPath>(), condition);
        }

        internal static BoolExpression CreateMemberCondition(MemberPath path, CellConstant domainValue, MemberDomainMap domainMap, MetadataWorkspace workspace)
        {
            if (domainValue is TypeConstant)
            {
                return BoolExpression.CreateLiteral(new OneOfTypeConst(CreateSlot(path, workspace), new CellConstantDomain(domainValue, domainMap.GetDomain(path))), domainMap);
            }
            return BoolExpression.CreateLiteral(new OneOfScalarConst(CreateSlot(path, workspace), new CellConstantDomain(domainValue, domainMap.GetDomain(path))), domainMap);
        }

        internal static JoinTreeSlot CreateSlot(MemberPath path, MetadataWorkspace workspace)
        {
            IList<EdmMember> members = path.Members;
            if (members.Count > 0)
            {
                MemberJoinTreeNode node = new MemberJoinTreeNode(members[members.Count - 1], false, new MemberJoinTreeNode[0], workspace);
                MemberJoinTreeNode node2 = node;
                for (int i = members.Count - 2; i >= 0; i--)
                {
                    node2 = new MemberJoinTreeNode(members[i], false, new MemberJoinTreeNode[] { node2 }, workspace);
                }
                new ExtentJoinTreeNode(path.Extent, new MemberJoinTreeNode[] { node2 }, workspace);
                return new JoinTreeSlot(node);
            }
            return new JoinTreeSlot(new ExtentJoinTreeNode(path.Extent, new MemberJoinTreeNode[0], workspace));
        }

        internal static IEqualityComparer<FragmentQuery> GetEqualityComparer(FragmentQueryProcessor qp) => 
            new FragmentQueryEqualityComparer(qp);

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (MemberPath path in this.Attributes)
            {
                if (builder.Length > 0)
                {
                    builder.Append(',');
                }
                builder.Append(path.ToString());
            }
            if ((this.Description != null) && (this.Description != builder.ToString()))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}: [{1} where {2}]", new object[] { this.Description, builder, this.Condition });
            }
            return string.Format(CultureInfo.InvariantCulture, "[{0} where {1}]", new object[] { builder, this.Condition });
        }

        public HashSet<MemberPath> Attributes =>
            this.m_attributes;

        public BoolExpression Condition =>
            this.m_condition;

        public string Description
        {
            get
            {
                string label = this.m_label;
                if ((label == null) && (this.m_fromVariable != null))
                {
                    label = this.m_fromVariable.ToString();
                }
                return label;
            }
        }

        public BoolExpression FromVariable =>
            this.m_fromVariable;

        private class FragmentQueryEqualityComparer : IEqualityComparer<FragmentQuery>
        {
            private FragmentQueryProcessor _qp;

            internal FragmentQueryEqualityComparer(FragmentQueryProcessor qp)
            {
                this._qp = qp;
            }

            public bool Equals(FragmentQuery x, FragmentQuery y)
            {
                if (!x.Attributes.SetEquals(y.Attributes))
                {
                    return false;
                }
                return this._qp.IsEquivalentTo(x, y);
            }

            public int GetHashCode(FragmentQuery q)
            {
                int num = 0;
                foreach (MemberPath path in q.Attributes)
                {
                    num ^= MemberPath.EqualityComparer.GetHashCode(path);
                }
                int num2 = 0;
                int num3 = 0;
                foreach (OneOfConst @const in q.Condition.OneOfConstVariables)
                {
                    num2 ^= MemberPath.EqualityComparer.GetHashCode(@const.Slot.MemberPath);
                    foreach (CellConstant constant in @const.Values.Values)
                    {
                        num3 ^= CellConstant.EqualityComparer.GetHashCode(constant);
                    }
                }
                return (((num * 13) + (num2 * 7)) + num3);
            }
        }
    }
}

