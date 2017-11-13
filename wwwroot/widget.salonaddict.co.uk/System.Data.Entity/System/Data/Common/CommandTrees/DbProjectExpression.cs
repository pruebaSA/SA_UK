namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;

    public sealed class DbProjectExpression : DbExpression
    {
        private DbExpressionBinding _input;
        private ExpressionLink _proj;

        internal DbProjectExpression(DbCommandTree cmdTree, DbExpressionBinding input, DbExpression projection) : base(cmdTree, DbExpressionKind.Project)
        {
            DbExpressionBinding.Check("Input", input, cmdTree);
            this._input = input;
            this._proj = new ExpressionLink("Projection", cmdTree, projection);
            base.ResultType = CommandTreeTypeHelper.CreateCollectionResultType(this.Projection.ResultType);
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

        public DbExpression Projection
        {
            get => 
                this._proj.Expression;
            internal set
            {
                this._proj.Expression = value;
            }
        }
    }
}

