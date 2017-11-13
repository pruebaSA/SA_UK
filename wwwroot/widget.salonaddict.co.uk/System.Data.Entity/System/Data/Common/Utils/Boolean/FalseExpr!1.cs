namespace System.Data.Common.Utils.Boolean
{
    using System;

    internal sealed class FalseExpr<T_Identifier> : BoolExpr<T_Identifier>
    {
        private static readonly FalseExpr<T_Identifier> s_value;

        static FalseExpr()
        {
            FalseExpr<T_Identifier>.s_value = new FalseExpr<T_Identifier>();
        }

        private FalseExpr()
        {
        }

        internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor) => 
            visitor.VisitFalse((FalseExpr<T_Identifier>) this);

        protected override bool EquivalentTypeEquals(BoolExpr<T_Identifier> other) => 
            object.ReferenceEquals(this, other);

        internal override BoolExpr<T_Identifier> MakeNegated() => 
            TrueExpr<T_Identifier>.Value;

        internal override System.Data.Common.Utils.Boolean.ExprType ExprType =>
            System.Data.Common.Utils.Boolean.ExprType.False;

        internal static FalseExpr<T_Identifier> Value =>
            FalseExpr<T_Identifier>.s_value;
    }
}

