namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Data.Common.Utils;

    internal sealed class CnfClause<T_Identifier> : Clause<T_Identifier>, IEquatable<CnfClause<T_Identifier>>
    {
        internal CnfClause(Set<Literal<T_Identifier>> literals) : base(literals, ExprType.Or)
        {
        }

        public bool Equals(CnfClause<T_Identifier> other) => 
            ((other != null) && other.Literals.SetEquals(base.Literals));
    }
}

