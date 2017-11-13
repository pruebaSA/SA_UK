namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;

    public sealed class DbIntersectExpression : DbBinaryExpression
    {
        internal DbIntersectExpression(DbCommandTree cmdTree, DbExpression left, DbExpression right) : base(cmdTree, DbExpressionKind.Intersect, left, right)
        {
            base.ResultType = base.CheckCollectionArguments();
            base.CheckComparableSetArguments();
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

