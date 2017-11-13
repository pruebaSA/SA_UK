namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public sealed class DbQueryCommandTree : DbCommandTree
    {
        private ExpressionLink _query;

        internal DbQueryCommandTree(MetadataWorkspace metadata, DataSpace dataSpace) : base(metadata, dataSpace)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbQueryCommandTree.ctor|API> %d#", base.ObjectId))
            {
                this._query = new ExpressionLink("Query", this);
            }
        }

        internal DbQueryCommandTree Clone()
        {
            using (new EntityBid.ScopeAuto("<cqt.DbQueryCommandTree.Clone|API> %d#", base.ObjectId))
            {
                DbQueryCommandTree tree = new DbQueryCommandTree(base.MetadataWorkspace, base.DataSpace);
                base.CopyParametersTo(tree);
                if (this.Query != null)
                {
                    tree.Query = ExpressionCopier.Copy(tree, this.Query);
                }
                return tree;
            }
        }

        internal override void DumpStructure(ExpressionDumper dumper)
        {
            if (this.Query != null)
            {
                dumper.Dump(this.Query, "Query");
            }
        }

        internal override string PrintTree(ExpressionPrinter printer) => 
            printer.Print(this);

        internal override void Replace(ExpressionReplacer replacer)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbQueryCommandTree.Replace(ExpressionReplacer)|API> %d#", base.ObjectId))
            {
                if (this.Query != null)
                {
                    DbExpression objA = replacer.Replace(this.Query);
                    if (!object.ReferenceEquals(objA, this.Query))
                    {
                        this.Query = objA;
                    }
                }
            }
        }

        internal override void Validate(Validator v)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbQueryCommandTree.Validate(Validator)|API> %d#", base.ObjectId))
            {
                v.Validate(this);
            }
        }

        internal override DbCommandTreeKind CommandTreeKind =>
            DbCommandTreeKind.Query;

        public DbExpression Query
        {
            get => 
                this._query.Expression;
            internal set
            {
                using (new EntityBid.ScopeAuto("<cqt.DbQueryCommandTree.set_Query|API> %d#", base.ObjectId))
                {
                    EntityBid.Trace("<cqt.DbQueryCommandTree.set_Query|INFO> %d#, value=%d#, %d{cqt.DbExpressionKind}\n", base.ObjectId, DbExpression.GetObjectId(value), DbExpression.GetExpressionKind(value));
                    this._query.Expression = value;
                }
            }
        }
    }
}

