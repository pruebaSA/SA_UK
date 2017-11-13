namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public sealed class DbInsertCommandTree : DbModificationCommandTree
    {
        private DbExpression _returning;
        private IList<DbModificationClause> _setClauses;

        internal DbInsertCommandTree(MetadataWorkspace metadata, DataSpace dataSpace) : base(metadata, dataSpace)
        {
        }

        internal override void DumpStructure(ExpressionDumper dumper)
        {
            base.DumpStructure(dumper);
            dumper.Begin("SetClauses");
            foreach (DbModificationClause clause in this.SetClauses)
            {
                if (clause != null)
                {
                    clause.DumpStructure(dumper);
                }
            }
            dumper.End("SetClauses");
            if (this.Returning != null)
            {
                dumper.Dump(this.Returning, "Returning");
            }
        }

        internal void InitializeSetClauses(List<DbModificationClause> setClauses)
        {
            this._setClauses = setClauses.AsReadOnly();
        }

        internal override string PrintTree(ExpressionPrinter printer) => 
            printer.Print(this);

        internal override void Replace(ExpressionReplacer replacer)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbInsertCommandTree.Replace(ExpressionReplacer)|API> %d#", base.ObjectId))
            {
                throw EntityUtil.NotSupported();
            }
        }

        internal override DbCommandTreeKind CommandTreeKind =>
            DbCommandTreeKind.Insert;

        internal override bool HasReader =>
            (null != this.Returning);

        public DbExpression Returning
        {
            get => 
                this._returning;
            internal set
            {
                this._returning = value;
            }
        }

        public IList<DbModificationClause> SetClauses =>
            this._setClauses;
    }
}

