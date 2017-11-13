namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public abstract class DbModificationCommandTree : DbCommandTree
    {
        private DbExpressionBinding _target;

        internal DbModificationCommandTree(MetadataWorkspace metadata, DataSpace dataSpace) : base(metadata, dataSpace)
        {
        }

        internal override void DumpStructure(ExpressionDumper dumper)
        {
            if (this.Target != null)
            {
                dumper.Dump(this.Target, "Target");
            }
        }

        internal abstract bool HasReader { get; }

        public DbExpressionBinding Target
        {
            get => 
                this._target;
            internal set
            {
                using (new EntityBid.ScopeAuto("<cqt.DbModificationCommandTree.set_Target|API> %d#", base.ObjectId))
                {
                    DbExpressionBinding.Check("Target", value, this);
                    EntityBid.Trace("<cqt.DbModificationCommandTree.set_Target|INFO> %d#, value.VariableName='%ls'\n", base.ObjectId, value.VariableName);
                    EntityBid.Trace("<cqt.DbModificationCommandTree.set_Target|INFO> %d#, value.DbExpression=%d#, %d{cqt.DbExpressionKind}\n", base.ObjectId, DbExpression.GetObjectId(value.Expression), DbExpression.GetExpressionKind(value.Expression));
                    this._target = value;
                    base.SetModified();
                }
            }
        }
    }
}

