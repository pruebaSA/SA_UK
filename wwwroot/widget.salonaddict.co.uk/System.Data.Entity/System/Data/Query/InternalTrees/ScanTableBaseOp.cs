namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class ScanTableBaseOp : RelOp
    {
        private System.Data.Query.InternalTrees.Table m_table;

        protected ScanTableBaseOp(OpType opType) : base(opType)
        {
        }

        protected ScanTableBaseOp(OpType opType, System.Data.Query.InternalTrees.Table table) : base(opType)
        {
            this.m_table = table;
        }

        internal System.Data.Query.InternalTrees.Table Table =>
            this.m_table;
    }
}

