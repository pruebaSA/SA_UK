namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;

    public sealed class DbDistinctExpression : DbUnaryExpression
    {
        internal DbDistinctExpression(DbCommandTree cmdTree, DbExpression arg) : base(cmdTree, DbExpressionKind.Distinct, arg)
        {
            base.CheckCollectionArgument();
            if (!TypeHelpers.IsValidDistinctOpType(TypeHelpers.GetEdmType<CollectionType>(arg.ResultType).TypeUsage))
            {
                throw EntityUtil.Argument(Strings.Cqt_Distinct_InvalidCollection, "Argument");
            }
            base.ResultType = arg.ResultType;
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

