namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;

    internal class AndExpr<T_Identifier> : TreeExpr<T_Identifier>
    {
        internal AndExpr(IEnumerable<BoolExpr<T_Identifier>> children) : base(children)
        {
        }

        internal AndExpr(params BoolExpr<T_Identifier>[] children) : this((IEnumerable<BoolExpr<T_Identifier>>) children)
        {
        }

        internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor) => 
            visitor.VisitAnd((AndExpr<T_Identifier>) this);

        internal override System.Data.Common.Utils.Boolean.ExprType ExprType =>
            System.Data.Common.Utils.Boolean.ExprType.And;
    }
}

