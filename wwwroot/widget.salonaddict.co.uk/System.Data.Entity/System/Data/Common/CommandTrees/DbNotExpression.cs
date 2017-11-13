namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbNotExpression : DbUnaryExpression
    {
        internal DbNotExpression(DbCommandTree commandTree, DbExpression arg) : base(commandTree, DbExpressionKind.Not)
        {
            EntityUtil.CheckArgumentNull<DbExpression>(arg, "Argument");
            if (!TypeSemantics.IsPrimitiveType(arg.ResultType, PrimitiveTypeKind.Boolean))
            {
                throw EntityUtil.Argument(Strings.Cqt_Not_BooleanArgumentRequired);
            }
            base.ArgumentLink.SetExpectedType(arg.ResultType);
            base.ArgumentLink.InitializeValue(arg);
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

