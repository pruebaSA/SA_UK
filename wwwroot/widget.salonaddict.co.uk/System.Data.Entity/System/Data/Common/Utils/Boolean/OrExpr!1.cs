namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;

    internal class OrExpr<T_Identifier> : TreeExpr<T_Identifier>
    {
        internal OrExpr(IEnumerable<BoolExpr<T_Identifier>> children) : base(children)
        {
        }

        internal OrExpr(params BoolExpr<T_Identifier>[] children) : this((IEnumerable<BoolExpr<T_Identifier>>) children)
        {
        }

        internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor) => 
            visitor.VisitOr((OrExpr<T_Identifier>) this);

        internal override System.Data.Common.Utils.Boolean.ExprType ExprType =>
            System.Data.Common.Utils.Boolean.ExprType.Or;
    }
}

