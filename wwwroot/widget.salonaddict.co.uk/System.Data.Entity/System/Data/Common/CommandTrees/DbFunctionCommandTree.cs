namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public sealed class DbFunctionCommandTree : DbCommandTree
    {
        private readonly System.Data.Metadata.Edm.EdmFunction _edmFunction;
        private readonly TypeUsage _resultType;

        internal DbFunctionCommandTree(MetadataWorkspace metadata, DataSpace dataSpace, System.Data.Metadata.Edm.EdmFunction edmFunction, TypeUsage resultType) : base(metadata, dataSpace)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbFunctionCommandTree.ctor|API> %d#", base.ObjectId))
            {
                EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.EdmFunction>(edmFunction, "edmFunction");
                this._edmFunction = edmFunction;
                this._resultType = resultType;
            }
        }

        internal override void DumpStructure(ExpressionDumper dumper)
        {
            if (this.EdmFunction != null)
            {
                dumper.Dump(this.EdmFunction);
            }
        }

        internal override string PrintTree(ExpressionPrinter printer) => 
            printer.Print(this);

        internal override void Replace(ExpressionReplacer callback)
        {
            throw EntityUtil.NotSupported();
        }

        internal override DbCommandTreeKind CommandTreeKind =>
            DbCommandTreeKind.Function;

        public System.Data.Metadata.Edm.EdmFunction EdmFunction =>
            this._edmFunction;

        public TypeUsage ResultType =>
            this._resultType;
    }
}

