namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Data.Common.Utils;

    internal sealed class DnfClause<T_Identifier> : Clause<T_Identifier>, IEquatable<DnfClause<T_Identifier>>
    {
        internal DnfClause(Set<Literal<T_Identifier>> literals) : base(literals, ExprType.And)
        {
        }

        public bool Equals(DnfClause<T_Identifier> other) => 
            ((other != null) && other.Literals.SetEquals(base.Literals));
    }
}

