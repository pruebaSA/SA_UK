namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public sealed class DbFilterExpression : DbExpression
    {
        private DbExpressionBinding _input;
        private ExpressionLink _predicate;

        internal DbFilterExpression(DbCommandTree cmdTree, DbExpressionBinding input, DbExpression predExpr) : base(cmdTree, DbExpressionKind.Filter)
        {
            this._predicate = new ExpressionLink("Predicate", cmdTree, PrimitiveTypeKind.Boolean, predExpr);
            DbExpressionBinding.Check("Input", input, cmdTree);
            this._input = input;
            base.ResultType = this._input.Expression.ResultType;
        }

        public override void Accept(DbExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw EntityUtil.ArgumentNull("visitor");
            }
            visitor.Visit(this);
        }

        public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor) => 
            visitor?.Visit(this);

        public DbExpressionBinding Input =>
            this._input;

        public DbExpression Predicate
        {
            get => 
                this._predicate.Expression;
            internal set
            {
                this._predicate.Expression = value;
            }
        }
    }
}

