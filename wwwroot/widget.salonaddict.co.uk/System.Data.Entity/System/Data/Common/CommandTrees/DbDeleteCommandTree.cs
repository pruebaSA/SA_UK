namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public sealed class DbDeleteCommandTree : DbModificationCommandTree
    {
        private DbExpression _predicate;

        internal DbDeleteCommandTree(MetadataWorkspace metadata, DataSpace dataSpace) : base(metadata, dataSpace)
        {
        }

        internal override void DumpStructure(ExpressionDumper dumper)
        {
            base.DumpStructure(dumper);
            if (this.Predicate != null)
            {
                dumper.Dump(this.Predicate, "Predicate");
            }
        }

        internal override string PrintTree(ExpressionPrinter printer) => 
            printer.Print(this);

        internal override void Replace(ExpressionReplacer replacer)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbDeleteCommandTree.Replace(ExpressionReplacer)|API> %d#", base.ObjectId))
            {
                throw EntityUtil.NotSupported();
            }
        }

        internal override DbCommandTreeKind CommandTreeKind =>
            DbCommandTreeKind.Delete;

        internal override bool HasReader =>
            false;

        public DbExpression Predicate
        {
            get => 
                this._predicate;
            internal set
            {
                this._predicate = value;
            }
        }
    }
}

