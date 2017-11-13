namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbCastExpression : DbUnaryExpression
    {
        internal DbCastExpression(DbCommandTree cmdTree, TypeUsage type, DbExpression arg) : base(cmdTree, DbExpressionKind.Cast)
        {
            cmdTree.TypeHelper.CheckType(type);
            if (!TypeSemantics.IsCastAllowed(arg.ResultType, type))
            {
                throw EntityUtil.Argument(Strings.Cqt_Cast_InvalidCast(TypeHelpers.GetFullName(arg.ResultType), TypeHelpers.GetFullName(type)));
            }
            base.ArgumentLink.InitializeValue(arg);
            base.ArgumentLink.SetExpectedType(arg.ResultType);
            base.ResultType = type;
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

