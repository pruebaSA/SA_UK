namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;

    public sealed class DbIsEmptyExpression : DbUnaryExpression
    {
        internal DbIsEmptyExpression(DbCommandTree cmdTree, DbExpression arg) : base(cmdTree, DbExpressionKind.IsEmpty, arg)
        {
            base.CheckCollectionArgument();
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
    }
}

