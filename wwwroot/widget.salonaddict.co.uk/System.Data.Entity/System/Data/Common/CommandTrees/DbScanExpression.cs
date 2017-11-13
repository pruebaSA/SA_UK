namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public sealed class DbScanExpression : DbExpression
    {
        private EntitySetBase _targetSet;

        internal DbScanExpression(DbCommandTree cmdTree, EntitySetBase entitySet) : base(cmdTree, DbExpressionKind.Scan)
        {
            cmdTree.TypeHelper.CheckEntitySet(entitySet);
            cmdTree.TrackContainer(entitySet.EntityContainer);
            this._targetSet = entitySet;
            base.ResultType = CommandTreeTypeHelper.CreateCollectionResultType(entitySet.ElementType);
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

        public EntitySetBase Target =>
            this._targetSet;
    }
}

