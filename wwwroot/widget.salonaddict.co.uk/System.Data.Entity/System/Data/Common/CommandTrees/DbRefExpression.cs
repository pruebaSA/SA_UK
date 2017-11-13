namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbRefExpression : DbUnaryExpression
    {
        private System.Data.Metadata.Edm.EntitySet _entitySet;

        internal DbRefExpression(DbCommandTree cmdTree, System.Data.Metadata.Edm.EntitySet entitySet, DbExpression refKeys, EntityType entityType) : base(cmdTree, DbExpressionKind.Ref)
        {
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.EntitySet>(entitySet, "entitySet");
            EntityUtil.CheckArgumentNull<DbExpression>(refKeys, "refKeys");
            CommandTreeTypeHelper.CheckType(entityType);
            cmdTree.TypeHelper.CheckEntitySet(entitySet);
            if (!TypeSemantics.IsValidPolymorphicCast(entitySet.ElementType, entityType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Ref_PolymorphicArgRequired);
            }
            TypeUsage expectedType = CommandTreeTypeHelper.CreateResultType(TypeHelpers.CreateKeyRowType(entitySet.ElementType, cmdTree.MetadataWorkspace));
            base.ArgumentLink.SetExpectedType(expectedType);
            base.ArgumentLink.InitializeValue(refKeys);
            cmdTree.TrackContainer(entitySet.EntityContainer);
            this._entitySet = entitySet;
            base.ResultType = CommandTreeTypeHelper.CreateReferenceResultType(entityType);
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

        public System.Data.Metadata.Edm.EntitySet EntitySet =>
            this._entitySet;
    }
}

