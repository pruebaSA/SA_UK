namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Text;

    internal abstract class BoolLiteral : InternalBase
    {
        internal static readonly IEqualityComparer<BoolLiteral> EqualityComparer = new BoolLiteralComparer();
        internal static readonly IEqualityComparer<BoolLiteral> EqualityIdentifierComparer = new IdentifierComparer();

        protected BoolLiteral()
        {
        }

        internal abstract StringBuilder AsCql(StringBuilder builder, string blockAlias, bool canSkipIsNotNull);
        internal abstract StringBuilder AsNegatedUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull);
        internal abstract StringBuilder AsUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull);
        internal abstract BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> FixRange(Set<CellConstant> range, MemberDomainMap memberDomainMap);
        internal abstract BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> GetDomainBoolExpression(MemberDomainMap domainMap);
        protected abstract int GetHash();
        protected virtual int GetIdentifierHash() => 
            this.GetHash();

        internal abstract void GetRequiredSlots(MemberPathMapBase projectedSlotMap, bool[] requiredSlots);
        protected abstract bool IsEqualTo(BoolLiteral right);
        protected virtual bool IsIdentifierEqualTo(BoolLiteral right) => 
            this.IsEqualTo(right);

        internal static TermExpr<DomainConstraint<BoolLiteral, CellConstant>> MakeTermExpression(BoolLiteral literal, IEnumerable<CellConstant> domain, IEnumerable<CellConstant> range)
        {
            Set<CellConstant> set = new Set<CellConstant>(domain, CellConstant.EqualityComparer);
            set.MakeReadOnly();
            DomainVariable<BoolLiteral, CellConstant> variable = new DomainVariable<BoolLiteral, CellConstant>(literal, set, EqualityIdentifierComparer);
            Set<CellConstant> set2 = new Set<CellConstant>(range, CellConstant.EqualityComparer);
            set2.MakeReadOnly();
            return new TermExpr<DomainConstraint<BoolLiteral, CellConstant>>(EqualityComparer<DomainConstraint<BoolLiteral, CellConstant>>.Default, new DomainConstraint<BoolLiteral, CellConstant>(variable, set2));
        }

        internal abstract BoolLiteral RemapBool(Dictionary<JoinTreeNode, JoinTreeNode> remap);

        private class BoolLiteralComparer : IEqualityComparer<BoolLiteral>
        {
            public bool Equals(BoolLiteral left, BoolLiteral right) => 
                (object.ReferenceEquals(left, right) || (((left != null) && (right != null)) && left.IsEqualTo(right)));

            public int GetHashCode(BoolLiteral literal) => 
                literal.GetHash();
        }

        private class IdentifierComparer : IEqualityComparer<BoolLiteral>
        {
            public bool Equals(BoolLiteral left, BoolLiteral right) => 
                (object.ReferenceEquals(left, right) || (((left != null) && (right != null)) && left.IsIdentifierEqualTo(right)));

            public int GetHashCode(BoolLiteral literal) => 
                literal.GetIdentifierHash();
        }
    }
}

