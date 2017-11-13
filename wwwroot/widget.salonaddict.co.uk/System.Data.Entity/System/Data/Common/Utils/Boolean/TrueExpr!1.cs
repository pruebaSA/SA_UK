namespace System.Data.Common.Utils.Boolean
{
    using System;

    internal sealed class TrueExpr<T_Identifier> : BoolExpr<T_Identifier>
    {
        private static readonly TrueExpr<T_Identifier> s_value;

        static TrueExpr()
        {
            TrueExpr<T_Identifier>.s_value = new TrueExpr<T_Identifier>();
        }

        private TrueExpr()
        {
        }

        internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor) => 
            visitor.VisitTrue((TrueExpr<T_Identifier>) this);

        protected override bool EquivalentTypeEquals(BoolExpr<T_Identifier> other) => 
            object.ReferenceEquals(this, other);

        internal override BoolExpr<T_Identifier> MakeNegated() => 
            FalseExpr<T_Identifier>.Value;

        internal override System.Data.Common.Utils.Boolean.ExprType ExprType =>
            System.Data.Common.Utils.Boolean.ExprType.True;

        internal static TrueExpr<T_Identifier> Value =>
            TrueExpr<T_Identifier>.s_value;
    }
}

