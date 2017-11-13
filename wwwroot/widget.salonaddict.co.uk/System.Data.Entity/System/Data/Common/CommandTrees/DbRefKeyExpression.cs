namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbRefKeyExpression : DbUnaryExpression
    {
        internal DbRefKeyExpression(DbCommandTree cmdTree, DbExpression reference) : base(cmdTree, DbExpressionKind.RefKey, reference)
        {
            RefType type = null;
            if (!TypeHelpers.TryGetEdmType<RefType>(base.Argument.ResultType, out type) || (type == null))
            {
                throw EntityUtil.Argument(Strings.Cqt_GetRefKey_RefRequired, "Argument");
            }
            if (type.ElementType == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_GetRefKey_InvalidRef, "Argument");
            }
            TypeUsage usage = CommandTreeTypeHelper.CreateResultType(TypeHelpers.CreateKeyRowType(type.ElementType, cmdTree.MetadataWorkspace));
            base.ResultType = usage;
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

