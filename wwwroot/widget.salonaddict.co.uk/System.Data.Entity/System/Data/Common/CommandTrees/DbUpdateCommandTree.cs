namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public sealed class DbUpdateCommandTree : DbModificationCommandTree
    {
        private DbExpression _predicate;
        private DbExpression _returning;
        private IList<DbModificationClause> _setClauses;

        internal DbUpdateCommandTree(MetadataWorkspace metadata, DataSpace dataSpace) : base(metadata, dataSpace)
        {
        }

        internal override void DumpStructure(ExpressionDumper dumper)
        {
            base.DumpStructure(dumper);
            if (this.Predicate != null)
            {
                dumper.Dump(this.Predicate, "Predicate");
            }
            dumper.Begin("SetClauses", (Dictionary<string, object>) null);
            foreach (DbModificationClause clause in this.SetClauses)
            {
                if (clause != null)
                {
                    clause.DumpStructure(dumper);
                }
            }
            dumper.End("SetClauses");
            dumper.Dump(this.Returning, "Returning");
        }

        internal void InitializeSetClauses(List<DbModificationClause> setClauses)
        {
            this._setClauses = setClauses.AsReadOnly();
        }

        internal override string PrintTree(ExpressionPrinter printer) => 
            printer.Print(this);

        internal override void Replace(ExpressionReplacer replacer)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbUpdateCommandTree.Replace(ExpressionReplacer)|API> %d#", base.ObjectId))
            {
                throw EntityUtil.NotSupported();
            }
        }

        internal override DbCommandTreeKind CommandTreeKind =>
            DbCommandTreeKind.Update;

        internal override bool HasReader =>
            (null != this.Returning);

        public DbExpression Predicate
        {
            get => 
                this._predicate;
            internal set
            {
                this._predicate = value;
            }
        }

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

