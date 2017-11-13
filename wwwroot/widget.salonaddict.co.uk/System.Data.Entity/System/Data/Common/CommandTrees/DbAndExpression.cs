namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbAndExpression : DbBinaryExpression
    {
        internal DbAndExpression(DbCommandTree commandTree, DbExpression left, DbExpression right) : base(commandTree, DbExpressionKind.And)
        {
            EntityUtil.CheckArgumentNull<DbExpression>(left, "Left");
            EntityUtil.CheckArgumentNull<DbExpression>(right, "Right");
            TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(left.ResultType, right.ResultType);
            if ((commonTypeUsage == null) || !TypeSemantics.IsPrimitiveType(commonTypeUsage, PrimitiveTypeKind.Boolean))
            {
                throw EntityUtil.Argument(Strings.Cqt_And_BooleanArgumentsRequired);
            }
            base.LeftLink.SetExpectedType(left.ResultType);
            base.LeftLink.InitializeValue(left);
            base.RightLink.SetExpectedType(right.ResultType);
            base.RightLink.InitializeValue(right);
            base.ResultType = commonTypeUsage;
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

