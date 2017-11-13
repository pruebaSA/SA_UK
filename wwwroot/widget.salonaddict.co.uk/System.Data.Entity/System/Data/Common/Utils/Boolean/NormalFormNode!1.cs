namespace System.Data.Common.Utils.Boolean
{
    using System;

    internal abstract class NormalFormNode<T_Identifier>
    {
        private readonly BoolExpr<T_Identifier> _expr;

        protected NormalFormNode(BoolExpr<T_Identifier> expr)
        {
            this._expr = expr.Simplify();
        }

        protected static BoolExpr<T_Identifier> ExprSelector<T_NormalFormNode>(T_NormalFormNode node) where T_NormalFormNode: NormalFormNode<T_Identifier> => 
            node._expr;

        internal BoolExpr<T_Identifier> Expr =>
            this._expr;
    }
}

