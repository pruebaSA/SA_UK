namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;

    public sealed class DbUnionAllExpression : DbBinaryExpression
    {
        internal DbUnionAllExpression(DbCommandTree cmdTree, DbExpression left, DbExpression right) : base(cmdTree, DbExpressionKind.UnionAll, left, right)
        {
            base.ResultType = base.CheckCollectionArguments();
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
    }
}

