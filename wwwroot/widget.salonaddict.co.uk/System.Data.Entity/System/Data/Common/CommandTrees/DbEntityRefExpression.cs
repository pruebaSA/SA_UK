namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbEntityRefExpression : DbUnaryExpression
    {
        internal DbEntityRefExpression(DbCommandTree cmdTree, DbExpression entity) : base(cmdTree, DbExpressionKind.EntityRef, entity)
        {
            EntityType type = null;
            if (!TypeHelpers.TryGetEdmType<EntityType>(base.Argument.ResultType, out type) || (type == null))
            {
                throw EntityUtil.Argument(Strings.Cqt_GetEntityRef_EntityRequired, "Argument");
            }
            base.ResultType = CommandTreeTypeHelper.CreateReferenceResultType(type);
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

