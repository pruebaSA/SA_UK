namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public sealed class DbQuantifierExpression : DbExpression
    {
        private DbExpressionBinding _input;
        private ExpressionLink _pred;

        internal DbQuantifierExpression(DbCommandTree cmdTree, DbExpressionKind kind, DbExpressionBinding input, DbExpression pred) : base(cmdTree, kind)
        {
            DbExpressionBinding.Check("Input", input, cmdTree);
            this._input = input;
            this._pred = new ExpressionLink("Predicate", cmdTree, PrimitiveTypeKind.Boolean, pred);
            base.ResultType = cmdTree.TypeHelper.CreateBooleanResultType();
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
                this._pred.Expression;
            internal set
            {
                this._pred.Expression = value;
            }
        }
    }
}

