namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbDerefExpression : DbUnaryExpression
    {
        internal DbDerefExpression(DbCommandTree cmdTree, DbExpression refExpr) : base(cmdTree, DbExpressionKind.Deref, refExpr)
        {
            EntityType type;
            if (!TypeHelpers.TryGetRefEntityType(base.Argument.ResultType, out type))
            {
                throw EntityUtil.Argument(Strings.Cqt_DeRef_RefRequired, "Argument");
            }
            base.ResultType = CommandTreeTypeHelper.CreateResultType(type);
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

