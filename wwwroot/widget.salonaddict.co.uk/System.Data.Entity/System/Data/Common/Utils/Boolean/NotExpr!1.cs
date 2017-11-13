namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Globalization;
    using System.Linq;

    internal sealed class NotExpr<T_Identifier> : TreeExpr<T_Identifier>
    {
        internal NotExpr(BoolExpr<T_Identifier> child) : base(new BoolExpr<T_Identifier>[] { child })
        {
        }

        internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor) => 
            visitor.VisitNot((NotExpr<T_Identifier>) this);

        internal override BoolExpr<T_Identifier> MakeNegated() => 
            this.Child;

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "!{0}", new object[] { this.Child });

        internal BoolExpr<T_Identifier> Child =>
            base.Children.First<BoolExpr<T_Identifier>>();

        internal override System.Data.Common.Utils.Boolean.ExprType ExprType =>
            System.Data.Common.Utils.Boolean.ExprType.Not;
    }
}

