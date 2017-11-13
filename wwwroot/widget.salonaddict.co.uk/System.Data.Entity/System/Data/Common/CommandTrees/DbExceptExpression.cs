namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbExceptExpression : DbBinaryExpression
    {
        internal DbExceptExpression(DbCommandTree cmdTree, DbExpression left, DbExpression right) : base(cmdTree, DbExpressionKind.Except, left, right)
        {
            base.CheckCollectionArguments();
            base.CheckComparableSetArguments();
            if (TypeSemantics.IsNullType(base.Left.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Except_LeftNullTypeInvalid, "Left");
            }
            base.ResultType = base.Left.ResultType;
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

